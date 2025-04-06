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
                    UpdateSpawn(updatePacket.Data as NetPacketSC_SpawnEnemy);
                    break;
                case NetPacketCode.SC_UpdateMovePattern:
                    UpdateMovePattern(updatePacket.Data as NetPacketSC_UpdateMovePattern);
                    break;
                case NetPacketCode.SC_UpdateSection:
                    UpdateSection(updatePacket.Data as NetPacketSC_UpdateSection);
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

        // 삭제 대상 적 검색.
        foreach (var pair in _enemyObjectDict)
        {
            var position = pair.Value.GetComponent<EnemyPathfinding>().GetPosition();
            var enemySection = GetSection((int)position.x, (int)position.y);

            // 인근 섹션이 아닌 경우, remove 대상 추가.
            if (false == _sectionManagement.IsAroundSection(playerSection, enemySection))
            {
                removeEnemies.Add(pair.Key);
                Debug.Log($"Destroy enemy. uid: {pair.Key}, player section: {playerSection}, enemy section: {enemySection}");
            }
        }

        // 적 삭제.
        foreach (var enemyUid in removeEnemies)
        {
            // 오브젝트 파괴.
            Destroy(_enemyObjectDict[enemyUid]);

            // 리스트에서 적 삭제.
            _enemyObjectDict.Remove(enemyUid);
        }
    }

    /// <summary>
    /// Spawn enemy.
    /// </summary>
    private void UpdateSpawn(NetPacketSC_SpawnEnemy packetData)
    {
        foreach (var enemy in packetData.Enemies)
        {
            // Spawn enemy.
            if (false == SpawnEnemy(enemy))
            {
                continue;
            }
        }
    }

    /// <summary>
    /// Update move pattern.
    /// </summary>
    private void UpdateMovePattern(NetPacketSC_UpdateMovePattern packetData)
    {
        foreach (var enemy in packetData.Enemies)
        {
            // Enemy Uid 검사.
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
    /// Update section.
    /// </summary>
    private void UpdateSection(NetPacketSC_UpdateSection packetData)
    {
        foreach (var enemy in packetData.Enemies)
        {
            // Spawn enemy.
            if (false == SpawnEnemy(enemy))
            {
                Debug.LogError($"Fail to spawn enemy. {enemy.EnemeyUid}");
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

        // Create enemy object.
        var enemyObject = Instantiate(findEnemyPrefab, new Vector3(enemy.Position.Pos.X, enemy.Position.Pos.Y), Quaternion.identity);

        // Add Enemy chunk.
        if (_enemyObjectDict.ContainsKey(enemy.EnemeyUid))
        {
            Debug.LogError($"Fail to spawn enemy. 3");
            return false;
        }

        _enemyObjectDict.Add(enemy.EnemeyUid, enemyObject);

        // Set Enemy AI.
        var enemyAI = enemyObject.GetComponent<EnemyAI>();
        enemyAI.Initialize_Spawn(tbEnemy.Attack, tbEnemy.AttackCooldown, enemy.Position.NextPos);

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
