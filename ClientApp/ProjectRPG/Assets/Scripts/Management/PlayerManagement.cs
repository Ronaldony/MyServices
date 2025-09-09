
using GameCore;
using GameCore.GamePacket;
using GamePackets;
using System.Collections.Concurrent;
using UnityEngine;

public class PlayerManagement : Singleton<PlayerManagement>
{
    // Player game object.
    public long UnitId;
    private GameObject _playerObject;

    // Queue.
    private ConcurrentQueue<UpdateGamePacket> _updatePacketQ;

    public PlayerManagement()
    {
        _updatePacketQ = new ConcurrentQueue<UpdateGamePacket>();
    }

    private void Start()
    {
    }

    void Update()
    {
        Proc_UpdatePacket();
    }

    /// <summary>
    /// Set player object.
    /// </summary>
    public void SetPlayerObject(GameObject playerGameObject)
    {
        _playerObject = playerGameObject;
    }

    /// <summary>
    /// Enqueue update packet.
    /// </summary>
    public void AddUpdatePacket(UpdateGamePacket updatePacket)
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
    /// Udpate stat.
    /// </summary>
    private void UpdateStat(GamePacketSC_UpdateUnitStat packet)
    {
        if (packet.UnitStats.ContainsKey(UnitId))
        {
            var unitStat = packet.UnitStats[UnitId];

            // Update health.
            var playerHealth = _playerObject.GetComponent<PlayerHealth>();
            playerHealth.UpdateHealth(unitStat.Health);
        }
    }
}