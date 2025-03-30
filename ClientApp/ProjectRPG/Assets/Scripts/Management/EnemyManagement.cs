using GameCore;
using GameCore.Datas.Enemy;
using GameCore.NetPacket;
using Network;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class EnemyManagement : Singleton<EnemyManagement>
{
    [SerializeField] private List<GameObject> _enemyPrefabs;

    // Packet.
    private NetPacketManager _netPacketManager;
    private ConcurrentQueue<UpdatePacket> _updatePacketQ;

    private Dictionary<ulong, DTO_Enemy> _enemyDict;
    private IEnumerable<TB_Enemy> _tbEnemies;

    public EnemyManagement()
    {
        _updatePacketQ = new ConcurrentQueue<UpdatePacket>();
        _netPacketManager = NetPacketManager.Instance;
        _enemyDict = new Dictionary<ulong, DTO_Enemy>();
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
        Proc_UpdatePacket();
    }

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

            // Instance enemy object.
            var enemyObject = Instantiate(findEnemyPrefab, new Vector3(enemy.Pos.X, enemy.Pos.Y), Quaternion.identity);
            var enemyAI = enemyObject.GetComponent<EnemyAI>();
            var roamingPos = new List<Vector2>();

            foreach (var pos in enemy.RoamingPos)
            {
                roamingPos.Add(new Vector2(pos.X, pos.Y));
            }

            enemyAI.Initialize_Spawn(tbEnemy.Attack, tbEnemy.AttackCooldown, roamingPos);

            // Set Enemy path.
            var enemyPathfinding = enemyObject.GetComponent<EnemyPathfinding>();
            enemyPathfinding.Initialize_Spawn(enemy.MoveSpeed);

            // Set Enemy Health.
            var enemyHealth = enemyObject.GetComponent<EnemyHealth>();

            enemyHealth.Initialize_Spawn(enemy.Health);

            // Set Pick up spawner.
            var pickupSpawner = enemyObject.GetComponent<PickUpSpawner>();
            pickupSpawner.Initialize_Spawn(tbEnemy.DropItem);
        }
    }

    /// <summary>
    /// Enqueue update packet.
    /// </summary>
    public void EnqueueUpdatePacket(UpdatePacket updatePacket)
    {
        _updatePacketQ.Enqueue(updatePacket);
    }
}
