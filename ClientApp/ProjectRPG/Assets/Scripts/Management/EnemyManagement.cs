using GameCore;
using GameCore.NetPacket;
using Network;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemyManagement : Singleton<EnemyManagement>
{
    [SerializeField] private List<GameObject> _enemyPrefabs;
    
    // Table datas.
    private IEnumerable<TB_Enemy> _tbEnemies;

    // Packet.
    private NetPacketManager _netPacketManager;
    private ConcurrentQueue<UpdatePacket> _updatePacketQ;
    private ConcurrentQueue<ulong> _killedEnemyQ;

    /// <summary>
    /// Enemy GameObject dictionary.
    /// key: Enemy uid.
    /// value: Enemy GameObject.
    /// </summary>
    private ConcurrentDictionary<ulong, GameObject> _enemyObjectDict;

    public EnemyManagement()
    {
        _updatePacketQ = new ConcurrentQueue<UpdatePacket>();
        _netPacketManager = NetPacketManager.Instance;
        _enemyObjectDict = new ConcurrentDictionary<ulong, GameObject>();
        _killedEnemyQ = new ConcurrentQueue<ulong>();
    }

    protected override void Awake()
    {
        base.Awake();
    }

    private void Start()
    {
        _tbEnemies = TDBManagement.Instance.SelectAll<TB_Enemy>();
    }

    // Update is called once per frame
    void Update()
    {
        // Process update packet.
        Proc_UpdatePacket();

        // Update section.
    }

    /// <summary>
    /// Enqueue update packet.
    /// </summary>
    public void EnqueueUpdatePacket(UpdatePacket updatePacket)
    {
        _updatePacketQ.Enqueue(updatePacket);
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
                    SpawnEnemy(updatePacket);
                    break;
            }
        }
    }

    /// <summary>
    /// Spawn enemy.
    /// </summary>
    private void SpawnEnemy(UpdatePacket updatePacket)
    {
        var pakcetData = updatePacket.Data as NetPacketSC_SpawnEnemy;

        // Spawn enemy.
        foreach (var enemy in pakcetData.Enemies)
        {
            // Find enemy infos.
            var tbEnemy = _tbEnemies.FirstOrDefault(d => d.Id == enemy.EnemyId);

            if (default ==  tbEnemy)
            {
                continue;
            }

            var findEnemyPrefab = _enemyPrefabs.FirstOrDefault(d => d.GetComponent<EnemyAI>().GetEnemyId() == tbEnemy.Id);

            if (default == findEnemyPrefab)
            {
                continue;
            }

            // Create enemy object.
            var enemyObject = Instantiate(findEnemyPrefab, new Vector3(enemy.Position.Pos.X, enemy.Position.Pos.Y), Quaternion.identity);

            // Add Enemy chunk.
            _enemyObjectDict.AddOrUpdate(enemy.EnemeyUid, enemyObject, (_, _) => enemyObject);

            // Set Enemy AI.
            var enemyAI = enemyObject.GetComponent<EnemyAI>();
            enemyAI.Initialize_Spawn(enemy.EnemyId, tbEnemy.Attack, tbEnemy.AttackCooldown, enemy.Position.NextPos, enemy.Position.RoamingPos);

            // Set Enemy path.
            var enemyPathfinding = enemyObject.GetComponent<EnemyPathfinding>();
            enemyPathfinding.Initialize_Spawn(enemy.Position.Veltocity);

            // Set Enemy Health.
            var enemyHealth = enemyObject.GetComponent<EnemyHealth>();
            enemyHealth.Initialize_Spawn(enemy.Health, enemy.EnemeyUid);

            // Set Pick up spawner.
            var pickupSpawner = enemyObject.GetComponent<PickUpSpawner>();
            pickupSpawner.Initialize_Spawn(tbEnemy.DropItem);
        }
    }

    /// <summary>
    /// Remove enemy from enemy dictionary.
    /// </summary>
    public void RemoveEnemyFromField(ulong enemyUid)
    {
        _enemyObjectDict.TryRemove(enemyUid, out _);
    }

    /// <summary>
    /// Kill enemy.
    /// </summary>
    public void KillEnemy(ulong enemyUid)
    {
        _killedEnemyQ.Enqueue(enemyUid);
        _enemyObjectDict.TryRemove(enemyUid, out _);
    }
}
