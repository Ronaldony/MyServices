using GameCore;
using GameCore.Datas.Mobs;
using GameCore.GamePacket;
using Network;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;

public class EnemyManagement : Singleton<EnemyManagement>
{
    [SerializeField] private List<GameObject> _enemyPrefabs;

    private GamePacketManager _pakcetManager;
    private SectionManagement _sectionManagement;

    // Table datas.
    private IEnumerable<TB_Mob> _tbMobs;

    // Queue.
    private ConcurrentQueue<UpdatePacket> _updatePacketQ;
    private ConcurrentQueue<long> _killedEnemyQ;

    /// <summary>
    /// Enemy GameObject dictionary.
    /// key: Mob unit id.
    /// value: Enemy GameObject.
    /// </summary>
    private Dictionary<long, GameObject> _mobObjectDict;

    public EnemyManagement()
    {
        _updatePacketQ = new ConcurrentQueue<UpdatePacket>();
        _killedEnemyQ = new ConcurrentQueue<long>();
        _mobObjectDict = new Dictionary<long, GameObject>();
    }

    private void Start()
    {
        _pakcetManager = GamePacketManager.Instance;
        _sectionManagement = SectionManagement.Instance;
        _tbMobs = TDBManagement.Instance.SelectAll<TB_Mob>();
    }

    // Update is called once per frame
    void Update()
    {
        // Process alive enemy.
        Proc_DestroyEnemy();

        // Process update packet.
        Proc_UpdatePacket();

        // Process section.
        Proc_Section();
    }

    /// <summary>
    /// Enqueue update packet.
    /// </summary>
    public void AddUpdatePacket(UpdatePacket updatePacket)
    {
        _updatePacketQ.Enqueue(updatePacket);
    }

    /// <summary>
    /// Procewss destroy enemy.
    /// </summary>
    private void Proc_DestroyEnemy()
    {
        while (_killedEnemyQ.TryDequeue(out var enemyUid))
        {
            // Destroy enemy.
            Destroy(_mobObjectDict[enemyUid]);

            // Remove enemy.
            _mobObjectDict.Remove(enemyUid);
        }
    }

    /// <summary>
    /// Process update packet.
    /// </summary>
    private void Proc_UpdatePacket()
    {
        // Process update pakcet.
        while (_updatePacketQ.TryDequeue(out var updatePacket))
        {
            switch (updatePacket.PacketCode)
            {
                case GamePacketCode.CS_AttackProjectile:
                    {
                        var packet = updatePacket.Data as GamePacketSC_AttackProjectile;

                    }
                    break;
                case GamePacketCode.SC_SpawnMob:
                    {
                        var packet = updatePacket.Data as GamePacketSC_SpawnMob;
                        SpawnEnemies(packet.Mobs);
                    }
                    break;
                case GamePacketCode.SC_SpawnPlayer:
                    {
                        var packet = updatePacket.Data as GamePacketSC_SpawnPlayer;
                        SpawnEnemies(packet.Mobs);
                    }
                    break;
                case GamePacketCode.SC_UpdateSection:
                    {
                        var packet = updatePacket.Data as GamePacketSC_UpdateSection;
                        SpawnEnemies(packet.Mobs);
                    }
                    break;
                case GamePacketCode.SC_UpdateUnitMovement:
                    {
                        var packet = updatePacket.Data as GamePacketSC_UpdateUnitMovement;
                        UpdateMovement(packet);
                    }
                    break;
                case GamePacketCode.SC_UpdateUnitStat:
                    {
                        var packet = updatePacket.Data as GamePacketSC_UpdateUnitStat;
                        UpdateStat(packet);
                    }
                    break;
            }
        }
    }

    /// <summary>
    /// Process section.
    /// </summary>
    private void Proc_Section()
    {
        var removemobs = new List<long>();
        var playerPos = PlayerController.Instance.GetPosition();
        var playerSection = GetSection((int)playerPos.x, (int)playerPos.y);

        // Scan enemy.
        foreach (var pair in _mobObjectDict)
        {
            var pathFinding = pair.Value.GetComponent<EnemyPathfinding>();
            if (null == pathFinding)
            {
                continue;
            }

            var position = pathFinding.GetPosition();
            var mobSection = GetSection((int)position.x, (int)position.y);

            // Check around section.
            if (false == _sectionManagement.IsAroundSection(playerSection, mobSection))
            {
                removemobs.Add(pair.Key);
                Debug.Log($"Destroy mob. uid: {pair.Key}, player section: {playerSection}, enemy section: {mobSection}");
            }
        }

        // Remove enemies.
        foreach (var unitId in removemobs)
        {
            // Remove enemy.
            _mobObjectDict.Remove(unitId, out var mob);

            // Destroy enemy.
            Destroy(mob);
        }
    }

    /// <summary>
    /// Attack projectile.
    /// </summary>
    private void AttackProjectile(GamePacketSC_AttackProjectile packet)
    {
        // Enemy Uid.
        if (false == _mobObjectDict.TryGetValue(packet.UnitId, out var mobObj))
        {
            Debug.LogError($"not exist mob. unitId: {packet.UnitId}");
            return;
        }


    }

    /// <summary>
    /// Udpate stat.
    /// </summary>
    private void UpdateStat(GamePacketSC_UpdateUnitStat packet)
    {
        foreach (var pair in packet.UnitStats)
        {
            // Enemy Uid.
            if (false == _mobObjectDict.TryGetValue(pair.Key, out var mobObj))
            {
                Debug.LogError($"not exist mob. unitId: {pair.Key}");
                continue;
            }

            // Update health.
            var enemyPath = mobObj.GetComponent<EnemyHealth>();
            enemyPath.UpdateHealth(pair.Value.Health);
        }
    }

    /// <summary>
    /// Udpate movement.
    /// </summary>
    private void UpdateMovement(GamePacketSC_UpdateUnitMovement packet)
    {
        foreach (var pair in packet.UnitLocations)
        {
            // Enemy Uid.
            if (false == _mobObjectDict.TryGetValue(pair.Key, out var mobObj))
            {
                Debug.LogError($"not exist mob. unitId: {pair.Key}");
                continue;
            }

            // Udpate hit.
            var pathFinding = mobObj.GetComponent<EnemyPathfinding>();
            pathFinding.MoveTo(pair.Value.MoveDirection);
        }
    }

    /// <summary>
    /// Update section.
    /// </summary>
    private void SpawnEnemies(Dictionary<long, DTO_Mob> mobs)
    {
        foreach (var pair in mobs)
        {
            // Spawn enemy.
            if (false == SpawnEnemy(pair.Key, pair.Value))
            {
                continue;
            }
        }
    }

    /// <summary>
    /// Spawn enemy.
    /// </summary>
    private bool SpawnEnemy(long unitId, DTO_Mob mob)
    {
        // Find enemy infos.
        var tbEnemy = _tbMobs.FirstOrDefault(d => d.Id == mob.MobId);

        if (default == tbEnemy)
        {
            Debug.LogError($"Fail to spawn mob. 1");
            return false;
        }

        var findEnemyPrefab = _enemyPrefabs.FirstOrDefault(d => d.GetComponent<EnemyAI>().GetUnitId() == tbEnemy.Id);

        if (default == findEnemyPrefab)
        {
            Debug.LogError($"Fail to spawn mob. 2");
            return false;
        }

        // Add Enemy chunk.
        if (_mobObjectDict.ContainsKey(unitId))
        {
            Debug.Log($"Already exist mob. uid: {unitId}, id: {mob.MobId}");
            return false;
        }

        // Create enemy object.
        var enemyObject = Instantiate(findEnemyPrefab, new Vector3(mob.Location.Pos.X, mob.Location.Pos.Y), Quaternion.identity);

        _mobObjectDict.Add(unitId, enemyObject);

        // Set Enemy AI.
        var enemyAI = enemyObject.GetComponent<EnemyAI>();
        enemyAI.Initialize_Spawn(tbEnemy.Damage, tbEnemy.AttackCooldown);

        // Set Enemy path.
        var enemyPathfinding = enemyObject.GetComponent<EnemyPathfinding>();
        enemyPathfinding.Initialize_Spawn(mob.Stat.Velocity);

        // Set Enemy Health.
        var enemyHealth = enemyObject.GetComponent<EnemyHealth>();
        enemyHealth.Initialize_Spawn(mob.Stat.Health, unitId);

        // Set Pick up spawner.
        var pickupSpawner = enemyObject.GetComponent<PickUpSpawner>();
        pickupSpawner.Initialize_Spawn(mob.DropItems);

        Debug.Log($"Spawn enemy - unitId: {unitId}, pos: {mob.Location.Pos}");

        return true;
    }

    /// <summary>
    /// Kill enemy.
    /// </summary>
    public void KillEnemy(long unitId)
    {
        _killedEnemyQ.Enqueue(unitId);
    }

    /// <summary>
    /// Get section or default.
    /// </summary>
    private (int sectionX, int sectionY) GetSection(int posX, int posY)
    {
        return (posX / GameData.SECTION_WIDTH, posY / GameData.SECTION_HEIGHT);
    }
}
