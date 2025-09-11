using GameCore;
using GameCore.Datas.Mobs;
using GameCore.GamePacket;
using GamePackets;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemyManagement : Singleton<EnemyManagement>
{
    [SerializeField] private List<GameObject> _mobPrefabs;

    private SectionManagement _sectionManagement;

    // Table datas.
    private IEnumerable<TB_Mob> _tbMobs;

    // Queue.
    private ConcurrentQueue<UpdateGamePacket> _updatePacketQ;
    private ConcurrentQueue<long> _killedEnemyQ;

    /// <summary>
    /// Enemy GameObject dictionary.
    /// key: Mob unit id.
    /// value: Enemy GameObject.
    /// </summary>
    private Dictionary<long, GameObject> _mobObjectDict;

    public EnemyManagement()
    {
        _updatePacketQ = new ConcurrentQueue<UpdateGamePacket>();
        _killedEnemyQ = new ConcurrentQueue<long>();
        _mobObjectDict = new Dictionary<long, GameObject>();
    }

    private void Start()
    {
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
    public void AddUpdatePacket(UpdateGamePacket updatePacket)
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
                case GamePacketCode.SC_AttackProjectile:
                    {
                        var packet = updatePacket.Data as GamePacketSC_AttackProjectile;
                        AttackProjectile(packet);
                    }
                    break;
                case GamePacketCode.SC_SpawnMob:
                    {
                        var packet = updatePacket.Data as GamePacketSC_SpawnMob;
                        Spawnmobs(packet.Mobs);
                    }
                    break;
                case GamePacketCode.SC_EnterPlayer:
                    {
                        var packet = updatePacket.Data as GamePacketSC_EnterPlayer;
                        Spawnmobs(packet.Mobs);

                        // Enable controls.
                        PlayerController.Instance.EnableControls();
                    }
                    break;
                case GamePacketCode.SC_SpawnUnits:
                    {
                        var packet = updatePacket.Data as GamePacketSC_SpawnUnits;
                        Spawnmobs(packet.Mobs);
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
                case GamePacketCode.SC_UpdateSection:
                    {
                        var packet = updatePacket.Data as GamePacketSC_UpdateSection;
                        Spawnmobs(packet.Mobs);
                    }
                    break;
                case GamePacketCode.SC_UpdateSectionNewEnter:
                    {
                        var packet = updatePacket.Data as GamePacketSC_UpdateSectionNewEnter;
                        Spawnmobs(packet.Mobs);
                    }
                    break;
                case GamePacketCode.SC_DropItems:
                    {
                        var packet = updatePacket.Data as GamePacketSC_DropItems;
                        DropItems(packet);
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
            var pathFinding = pair.Value.GetComponent<MobPathfinding>();
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
                //Debug.Log($"Destroy mob. uid: {pair.Key}, player section: {playerSection}, enemy section: {mobSection}");
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
            Debug.Log($"not exist mob. unitId: {packet.UnitId}");
            return;
        }

        // EnemyAI.
        var mobAI = mobObj.GetComponent<MobAI>();
        mobAI.AttackProjectile(packet.Projectile);
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
                Debug.Log($"not exist mob. unitId: {pair.Key}");
                continue;
            }

            // Update health.
            var mobPath = mobObj.GetComponent<MobHealth>();
            mobPath.UpdateHealth(pair.Value.Health);
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
                Debug.Log($"not exist mob. unitId: {pair.Key}");
                continue;
            }

            // Udpate hit.
            var mobPathfinding = mobObj.GetComponent<MobPathfinding>();
            mobPathfinding.UpdateMovement(pair.Value.MoveDirection, pair.Value.Pos, pair.Value.PosEnd);

            //Debug.Log($"update movement. now pos: {mobPathfinding.NowPos()} dir: {pair.Value.MoveDirection}");
        }
    }

    /// <summary>
    /// Spawn mobs.
    /// </summary>
    private void Spawnmobs(Dictionary<long, DTO_Mob> mobs)
    {
        foreach (var pair in mobs)
        {
            // Spawn enemy.
            if (false == SpawnMob(pair.Key, pair.Value))
            {
                continue;
            }
        }
    }

    /// <summary>
    /// Spawn mobs.
    /// </summary>
    private void Spawnmobs(ConcurrentDictionary<long, DTO_Mob> mobs)
    {
        foreach (var pair in mobs)
        {
            // Spawn enemy.
            if (false == SpawnMob(pair.Key, pair.Value))
            {
                continue;
            }
        }
    }

    /// <summary>
    /// Spawn enemy.
    /// </summary>
    private bool SpawnMob(long unitId, DTO_Mob mob)
    {
        // Find enemy infos.
        var tbMob = _tbMobs.FirstOrDefault(d => d.Id == mob.MobId);

        if (default == tbMob)
        {
            Debug.LogError($"Fail to spawn mob. 1");
            return false;
        }

        var findEnemyPrefab = _mobPrefabs.FirstOrDefault(d => d.GetComponent<MobAI>().GetMobId() == tbMob.Id);

        if (default == findEnemyPrefab)
        {
            Debug.LogError($"Fail to spawn mob. 2");
            return false;
        }

        // Add Enemy chunk.
        if (_mobObjectDict.ContainsKey(unitId))
        {
            //Debug.Log($"Already exist mob. uid: {unitId}, id: {mob.MobId}");
            return false;
        }

        // Create enemy object.
        var mobObject = Instantiate(findEnemyPrefab, new Vector3(mob.Location.Pos.X, mob.Location.Pos.Y), Quaternion.identity);

        _mobObjectDict.Add(unitId, mobObject);

        // Set Enemy AI.
        var enemyAI = mobObject.GetComponent<MobAI>();

        // Set Enemy path.
        var enemyPathfinding = mobObject.GetComponent<MobPathfinding>();

        // Set Enemy Health.
        var enemyHealth = mobObject.GetComponent<MobHealth>();
        enemyHealth.SetupHealth(mob.Stat.Health, unitId);

        // Set Pick up spawner.
        var pickupSpawner = mobObject.GetComponent<PickUpSpawner>();

        //Debug.Log($"Spawn enemy - unitId: {unitId}, pos: {mob.Location.Pos}");

        return true;
    }

    /// <summary>
    /// Drop items.
    /// </summary>
    private void DropItems(GamePacketSC_DropItems gp)
    {
        // Enemy Uid.
        if (false == _mobObjectDict.TryGetValue(gp.UnitId, out var mobObj))
        {
            Debug.Log($"not exist mob. unitId: {gp.UnitId}");
            return;
        }

        // PickUpSpawner.
        var pickUpSpawner = mobObj.GetComponent<PickUpSpawner>();

        foreach (var pair in gp.DropItems)
        {
            pickUpSpawner.DropItem(pair.Key, pair.Value);
        }
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
