
using Network;
using System.Collections.Concurrent;
using System.Collections.Generic;
using UnityEngine;

public class OpponentManagement : Singleton<OpponentManagement>
{
    [SerializeField] private List<GameObject> _playerPrefabs;

    // Queue.
    private ConcurrentQueue<UpdatePacket> _updatePacketQ;
    private ConcurrentQueue<long> _killedEnemyQ;

    public OpponentManagement()
    {

    }
}