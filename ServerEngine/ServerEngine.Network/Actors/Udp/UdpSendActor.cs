using Akka.Actor;
using LiteNetLib;
using Microsoft.Extensions.DependencyInjection;

namespace ServerEngine.Network.Actors.Udp
{
    using ServerEngine.Core.Services.Interfaces;
    using ServerEngine.Network.UDP;

    /// <summary>
    /// UdpSendActor.
    /// </summary>
    internal class UdpSendActor : ReceiveActor
    {
        private readonly IMemoryPoolService _memoryPoolService;
        private readonly NetManager _netManager;

        public UdpSendActor(IServiceProvider serviceProvider, NetManager netManager)
        {
            _memoryPoolService = serviceProvider.GetRequiredService<IMemoryPoolService>();
            _netManager = netManager;

            Receive<UdpSendActorMsg_Send>(packet => SendPacket(packet));
            Receive<UdpSendActorMsg_Broadcast>(packet => Broadcast(packet));
        }

        /// <summary>
        /// Props.
        /// </summary>
        public static Props Props(IServiceProvider serviceProvider, NetManager netManager)
        {
            return Akka.Actor.Props.Create(() => new UdpSendActor(serviceProvider, netManager));
        }

        /// <summary>
        /// Send Packet..
        /// </summary>
        public void SendPacket(UdpSendActorMsg_Send msg)
        {
            var session = msg.Session;

            // Sesison lock: 세션 풀을 사용하기 때문에, 이미 Disconnection된 세션 메모리가 사용되는 것을 방지.
            lock (session)
            {
                // Check disconnection.
                if (session.IsDisconn)
                {
                    return;
                }

                // Check session uid.
                if (msg.SessionUid != session.SessionUid)
                {
                    return;
                }

                // Write packet.
                var writer = session.DataWriter;
                writer.Reset();

                // Packet.
                var packet = new UdpPacket();
                packet.PacketCode = msg.PacketCode;
                packet.Data = msg.PacketData;

                session.PacketProcessor.Write(writer, packet);

                // Send.
                session.Peer.Send(writer, DeliveryMethod.ReliableOrdered);
            }
        }

        /// <summary>
        /// Broadcast.
        /// </summary>
        public void Broadcast(UdpSendActorMsg_Broadcast msg)
        {
            var session = msg.Session;

            // Sesison lock: 세션 풀을 사용하기 때문에, 이미 Disconnection된 세션 메모리가 사용되는 것을 방지.
            lock (session)
            {
                // Check disconnection.
                if (session.IsDisconn)
                {
                    return;
                }

                // Check session uid.
                if (msg.SessionUid != session.SessionUid)
                {
                    return;
                }

                // Write packet.
                var writer = session.DataWriter;
                writer.Reset();

                // Packet.
                var packet = new UdpPacket
                {
                    PacketCode = msg.PacketCode,
                    Data = msg.PacketData
                };

                session.PacketProcessor.Write(writer, packet);

                // Send.
                _netManager.SendToAll(writer, DeliveryMethod.ReliableOrdered, session.Peer);
            }
        }
    }
}
