using GameCore;
using GameCore.Datas.Enemy;
using GameCore.NetPacket;
using Network;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemyManagement : Singleton<EnemyManagement>
{
    [SerializeField] private List<GameObject> _enemyPrefabs;

    private NetPacketManager _pakcetManager;
    private SectionManagement _sectionManagement;

    // Table datas.
    private IEnumerable<TB_Enemy> _tbEnemies;

    // Queue.
    private ConcurrentQueue<UpdatePacket> _updatePacketQ;
    private ConcurrentQueue<ulong> _killedEnemyQ;

    /// <summary>
    /// Enemy GameObject dictionary.
    /// key: Enemy uid.
    /// value: Enemy GameObject.
    /// </summary>
    private Dictionary<ulong, GameObject> _enemyObjectDict;

    public EnemyManagement()
    {
        _updatePacketQ = new ConcurrentQueue<UpdatePacket>();
        _killedEnemyQ = new ConcurrentQueue<ulong>();
        _enemyObjectDict = new Dictionary<ulong, GameObject>();
    }

    private void Start()
    {
        _pakcetManager = NetPacketManager.Instance;
        _sectionManagement = SectionManagement.Instance;
        _tbEnemies = TDBManagement.Instance.SelectAll<TB_Enemy>();
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
            Destroy(_enemyObjectDict[enemyUid]);

            // Remove enemy.
            _enemyObjectDict.Remove(enemyUid);
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
                case NetPacketCode.SC_SpawnEnemy:
                    {
                        var packet = updatePacket.Data as NetPacketSC_SpawnEnemy;
                        SpawnEnemies(packet.Enemies);
                    }
                    break;
                case NetPacketCode.SC_UpdateMovePattern:
                    {
                        UpdateMovePattern(updatePacket.Data as NetPacketSC_UpdateMovePattern);
                    }
                    break;
                case NetPacketCode.SC_UpdateSection:
                    {
                        var packet = updatePacket.Data as NetPacketSC_UpdateSection;
                        SpawnEnemies(packet.Enemies);
                    }
                    break;
                case NetPacketCode.SC_SpawnPlayer:
                    {
                        var packet = updatePacket.Data as NetPacketSC_SpawnPlayer;
                        SpawnEnemies(packet.Enemies);
                    }
                    break;
                case NetPacketCode.SC_HitEnemy:
                    {
                        UpdateHitEnemies(updatePacket.Data as NetPacketSC_HitEnemy);
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
        var removeEnemies = new List<ulong>();
        var playerPos = PlayerController.Instance.GetPosition();
        var playerSection = GetSection((int)playerPos.x, (int)playerPos.y);

        // Scan enemy.
        foreach (var pair in _enemyObjectDict)
        {
            var pathFinding = pair.Value.GetComponent<EnemyPathfinding>();
            if (null == pathFinding)
            {
                continue;
            }

            var position = pathFinding.GetPosition();
            var enemySection = GetSection((int)position.x, (int)position.y);

            // Check around section.
            if (false == _sectionManagement.IsAroundSection(playerSection, enemySection))
            {
                removeEnemies.Add(pair.Key);
                Debug.Log($"Destroy enemy. uid: {pair.Key}, player section: {playerSection}, enemy section: {enemySection}");
            }
        }

        // Remove enemies.
        foreach (var enemyUid in removeEnemies)
        {
            // Destroy enemy.
            Destroy(_enemyObjectDict[enemyUid]);

            // Remove enemy.
            _enemyObjectDict.Remove(enemyUid);
        }
    }

    /// <summary>
    /// Update move pattern.
    /// </summary>
    private void UpdateMovePattern(NetPacketSC_UpdateMovePattern packetData)
    {
        foreach (var enemy in packetData.Enemies)
        {
            // Enemy Uid �˻�.
            if (false == _enemyObjectDict.TryGetValue(enemy.EnemeyUid, out var enemyObject))
            {
                Debug.LogError($"No enemy. uid: {enemy.EnemeyUid}");
                continue; 
            }

            // Udpate next position.
            var enemyPath = enemyObject.GetComponent<EnemyPathfinding>();
            enemyPath.MoveTo(enemy.Position.NextPos);
        }
    }

    /// <summary>
    /// Udpate hit enemies.
    /// </summary>
    private void UpdateHitEnemies(NetPacketSC_HitEnemy packet)
    {
        var srcPosition = new Vector3(packet.Position.X, packet.Position.Y);

        // Check hit.
        foreach (var enemy in packet.Enemies)
        {
            // Enemy Uid.
            if (false == _enemyObjectDict.TryGetValue(enemy.EnemeyUid, out var enemyObject))
            {
                Debug.LogError($"No hit enemy. uid: {enemy.EnemeyUid}");
                continue;
            }

            // Udpate hit.
            var enemyPath = enemyObject.GetComponent<EnemyHealth>();
            enemyPath.TakeDamage(packet.Damage, srcPosition);
        }
    }

    /// <summary>
    /// Update section.
    /// </summary>
    private void SpawnEnemies(List<DTO_Enemy> enemies)
    {
        foreach (var enemy in enemies)
        {
            // Spawn enemy.
            if (false == SpawnEnemy(enemy))
            {
                continue;
            }
        }
    }

    /// <summary>
    /// Spawn enemy.
    /// </summary>
    private bool SpawnEnemy(DTO_Enemy enemy)
    {
        // Find enemy infos.
        var tbEnemy = _tbEnemies.FirstOrDefault(d => d.Id == enemy.EnemyId);

        if (default == tbEnemy)
        {
            Debug.LogError($"Fail to spawn enemy. 1");
            return false;
        }

        var findEnemyPrefab = _enemyPrefabs.FirstOrDefault(d => d.GetComponent<EnemyAI>().GetEnemyId() == tbEnemy.Id);

        if (default == findEnemyPrefab)
        {
            Debug.LogError($"Fail to spawn enemy. 2");
            return false;
        }

        // Add Enemy chunk.
        if (_enemyObjectDict.ContainsKey(enemy.EnemeyUid))
        {
            Debug.Log($"Already exist enemy. uid: {enemy.EnemeyUid}, id: {enemy.EnemyId}");
            return false;
        }

        // Create enemy object.
        var enemyObject = Instantiate(findEnemyPrefab, new Vector3(enemy.Position.Pos.X, enemy.Position.Pos.Y), Quaternion.identity);

        _enemyObjectDict.Add(enemy.EnemeyUid, enemyObject);

        // Set Enemy AI.
        var enemyAI = enemyObject.GetComponent<EnemyAI>();
        enemyAI.Initialize_Spawn(tbEnemy.Attack, tbEnemy.AttackCooldown);

        // Set Enemy path.
        var enemyPathfinding = enemyObject.GetComponent<EnemyPathfinding>();
        enemyPathfinding.Initialize_Spawn(enemy.Position.Veltocity, enemy.Position.NextPos);

        // Set Enemy Health.
        var enemyHealth = enemyObject.GetComponent<EnemyHealth>();
        enemyHealth.Initialize_Spawn(enemy.Health, enemy.EnemeyUid);

        // Set Pick up spawner.
        var pickupSpawner = enemyObject.GetComponent<PickUpSpawner>();
        pickupSpawner.Initialize_Spawn(tbEnemy.DropItem);

        Debug.Log($"Spawn enemy - uid: {enemy.EnemeyUid}, pos: {enemy.Position.Pos}");

        return true;
    }

    /// <summary>
    /// Kill enemy.
    /// </summary>
    public void KillEnemy(ulong enemyUid)
    {
        _killedEnemyQ.Enqueue(enemyUid);
    }

    /// <summary>
    /// Get section or default.
    /// </summary>
    private (int sectionX, int sectionY) GetSection(int posX, int posY)
    {
        return (posX / GameData.SECTION_WIDTH, posY / GameData.SECTION_HEIGHT);
    }
}
