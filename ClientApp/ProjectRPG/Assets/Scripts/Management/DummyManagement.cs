using GameCore;
using GameCore.Datas.Enemy;
using GameCore.GamePacket;
using Network;
using System.Collections.Concurrent;
using System.Collections.Generic;
using UnityEngine;

public class DummyManagement : Singleton<DummyManagement>
{
    [SerializeField] private GameObject _dummyPrefab;

    // Queue.
    private ConcurrentQueue<UpdatePacket> _updatePacketQ;

    /// <summary>
    /// Player dummy dictionary.
    /// key: pid.
    /// value: GameObject.
    /// </summary>
    private Dictionary<string, GameObject> _playerDummyDict;

    /// <summary>
    /// Enemy GameObject dictionary.
    /// key: Enemy uid.
    /// value: Enemy GameObject.
    /// </summary>
    private Dictionary<ulong, GameObject> _enemyDummyDict;

    public DummyManagement()
    {
        _updatePacketQ = new ConcurrentQueue<UpdatePacket>();
        _enemyDummyDict = new Dictionary<ulong, GameObject>();
        _playerDummyDict = new Dictionary<string, GameObject>();
    }

    private void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        // Process update packet.
        Proc_UpdatePacket();
    }

    /// <summary>
    /// Enqueue update packet.
    /// </summary>
    public void AddUpdatePacket(UpdatePacket updatePacket)
    {
        //_updatePacketQ.Enqueue(updatePacket);
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
                case GamePacketCode.SC_SpawnEnemy:
                    {
                        var packet = updatePacket.Data as GamePacketSC_SpawnEnemy;
                        SpawnEnemies(packet.Enemies);
                    }
                    break;
                case GamePacketCode.SC_UpdateSection:
                    {
                        var packet = updatePacket.Data as GamePacketSC_UpdateSection;
                        SpawnEnemies(packet.Enemies);
                    }
                    break;
                case GamePacketCode.SC_SpawnPlayer:
                    {
                        var packet = updatePacket.Data as GamePacketSC_SpawnPlayer;
                        SpawnEnemies(packet.Enemies);
                    }
                    break;
                case GamePacketCode.SC_DummyMove:
                    {
                        MoveEnemies(updatePacket.Data as GamePacketSC_DummyMove);
                    }
                    break;
            }
        }
    }

    /// <summary>
    /// Update section.
    /// </summary>
    private void SpawnEnemies(List<DTO_Enemy> enemies)
    {
        foreach (var enemy in enemies)
        {
            SpawnEnemy(enemy);
        }
    }

    private void MoveEnemies(GamePacketSC_DummyMove packet)
    {
        foreach (var playInfo in packet.PlayInfos)
        {
            if (_playerDummyDict.TryGetValue(playInfo.Pid, out var gameObject))
            {
                var dummy = gameObject.GetComponent<UnitDummy>();
                dummy.Move(playInfo.Location.Position);
            }
        }

        foreach (var enemy in packet.Enemies)
        {
            if (_enemyDummyDict.TryGetValue(enemy.EnemeyUid, out var gameObject))
            {
                var dummy = gameObject.GetComponent<UnitDummy>();
                dummy.Move(enemy.Location.Pos);
            }
        }
    }

    /// <summary>
    /// Spawn enemy.
    /// </summary>
    private bool SpawnEnemy(DTO_Enemy enemy)
    {
        // Add Enemy chunk.
        if (_enemyDummyDict.ContainsKey(enemy.EnemeyUid))
        {
            Debug.Log($"Already exist dummy. uid: {enemy.EnemeyUid}");
            return false;
        }

        // Create enemy object.
        var dummyObject = Instantiate(_dummyPrefab, new Vector3(enemy.Location.Pos.X, enemy.Location.Pos.Y), Quaternion.identity);

        _enemyDummyDict.Add(enemy.EnemeyUid, dummyObject);

        var enemyDummy = dummyObject.GetComponent<UnitDummy>();
        enemyDummy.Initialize(enemy.EnemeyUid, enemy.Location.Pos);

        Debug.Log($"Spawn dummy - uid: {enemy.EnemeyUid}, pos: {enemy.Location.Pos}");

        return true;
    }
}
