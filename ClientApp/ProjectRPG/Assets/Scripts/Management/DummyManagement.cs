using GameCore;
using GameCore.Datas.Mobs;
using GameCore.Datas.Play;
using GameCore.GamePacket;
using Network;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DummyManagement : Singleton<DummyManagement>
{
    [SerializeField] private GameObject _dummyPrefab;

    // Queue.
    private ConcurrentQueue<UpdatePacket> _updatePacketQ;

    /// <summary>
    /// Dummy dictionary.
    /// key: unit id.
    /// value: GameObject.
    /// </summary>
    private Dictionary<long, GameObject> _dummyDict;

    public DummyManagement()
    {
        _updatePacketQ = new ConcurrentQueue<UpdatePacket>();
        _dummyDict = new Dictionary<long, GameObject>();
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
                case GamePacketCode.SC_SpawnMob:
                    {
                        var packet = updatePacket.Data as GamePacketSC_SpawnMob;
                        SpawnMobDummy(packet.Mobs);
                    }
                    break;
                case GamePacketCode.SC_UpdateSection:
                    {
                        var packet = updatePacket.Data as GamePacketSC_UpdateSection;
                        SpawnMobDummy(packet.Mobs);
                    }
                    break;
                case GamePacketCode.SC_EnterPlayer:
                    {
                        var packet = updatePacket.Data as GamePacketSC_EnterPlayer;
                        SpawnMobDummy(packet.Mobs);
                    }
                    break;
                case GamePacketCode.SC_SpawnUnits:
                    {
                        var packet = updatePacket.Data as GamePacketSC_SpawnUnits;
                        SpawnMobDummy(packet.Mobs);
                    }
                    break;
                case GamePacketCode.SC_DummyMove:
                    {
                        MoveMobs(updatePacket.Data as GamePacketSC_DummyMove);
                    }
                    break;
            }
        }
    }

    /// <summary>
    /// Spawn mob duymmy.
    /// </summary>
    private void SpawnMobDummy(Dictionary<long, DTO_Mob> mobs)
    {
        foreach (var pair in mobs)
        {
            var unitid = pair.Key;
            var mob = pair.Value;

            // Add Enemy chunk.
            if (_dummyDict.ContainsKey(unitid))
            {
                Debug.Log($"Already exist dummy. unitid: {unitid}");
                continue;
            }

            // Create enemy object.
            var dummyObject = Instantiate(_dummyPrefab, new Vector3(mob.Location.Pos.X, mob.Location.Pos.Y), Quaternion.identity);

            _dummyDict.Add(unitid, dummyObject);

            var enemyDummy = dummyObject.GetComponent<UnitDummy>();
            enemyDummy.Initialize(unitid, mob.Location.Pos);

            Debug.Log($"Spawn dummy - unitid: {unitid}, pos: {mob.Location.Pos}");
        }
    }

    /// <summary>
    /// Spawn mob duymmy.
    /// </summary>
    private void SpawnMobDummy(ConcurrentDictionary<long, DTO_Mob> mobs)
    {
        foreach (var pair in mobs)
        {
            var unitid = pair.Key;
            var mob = pair.Value;

            // Add Enemy chunk.
            if (_dummyDict.ContainsKey(unitid))
            {
                Debug.Log($"Already exist dummy. unitid: {unitid}");
                continue;
            }

            // Create enemy object.
            var dummyObject = Instantiate(_dummyPrefab, new Vector3(mob.Location.Pos.X, mob.Location.Pos.Y), Quaternion.identity);

            _dummyDict.Add(unitid, dummyObject);

            var enemyDummy = dummyObject.GetComponent<UnitDummy>();
            enemyDummy.Initialize(unitid, mob.Location.Pos);

            Debug.Log($"Spawn dummy - unitid: {unitid}, pos: {mob.Location.Pos}");
        }
    }

    /// <summary>
    /// Spawn player duymmy.
    /// </summary>
    private void SpawnPlayerDummy(Dictionary<long, DTO_PlayInfo> players)
    {
        foreach (var pair in players)
        {
            var unitid = pair.Key;
            var player = pair.Value;

            // Add Enemy chunk.
            if (_dummyDict.ContainsKey(unitid))
            {
                Debug.Log($"Already exist dummy. unitid: {unitid}");
                continue;
            }

            // Create enemy object.
            var dummyObject = Instantiate(_dummyPrefab, new Vector3(player.Location.Pos.X, player.Location.Pos.Y), Quaternion.identity);

            _dummyDict.Add(unitid, dummyObject);

            var enemyDummy = dummyObject.GetComponent<UnitDummy>();
            enemyDummy.Initialize(unitid, player.Location.Pos);

            Debug.Log($"Spawn dummy - unitid: {unitid}, pos: {player.Location.Pos}");
        }
    }

    private void MoveMobs(GamePacketSC_DummyMove packet)
    {
        foreach (var pair in packet.PlayInfos)
        {
            if (_dummyDict.TryGetValue(pair.Key, out var gameObject))
            {
                var dummy = gameObject.GetComponent<UnitDummy>();
                dummy.Move(pair.Value.Location.Pos);
            }
        }

        foreach (var pair in packet.Mobs)
        {
            if (_dummyDict.TryGetValue(pair.Key, out var gameObject))
            {
                var dummy = gameObject.GetComponent<UnitDummy>();
                dummy.Move(pair.Value.Location.Pos);
            }
        }
    }
}
