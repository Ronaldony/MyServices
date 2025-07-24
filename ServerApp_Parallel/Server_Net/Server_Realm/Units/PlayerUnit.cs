namespace Server_Realm.Entities
{
    using GameCore.Datas.Play;
    using Server_Realm.Core.Types;

    /// <summary>
    /// PlayerUnit.
    /// </summary>
    public class PlayerUnit : Unit
    {
        public PlayerUnit()
        {
            UnitType = Type_Unit.Player;
        }

        /// <summary>
        /// Pid.
        /// </summary>
        public string Pid { get; set; }

        /// <summary>
        /// 최근 패킷 수신 시간.
        /// </summary>
        public DateTime LastPacketTime { get; set; }

        /// <summary>
        /// PlayInfo.
        /// </summary>
        public DTO_PlayInfo PlayInfo { get; set; }

        /// <summary>
        /// Update.
        /// </summary>
        public override void Update(float deltaTime)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Dispose.
        /// </summary>
        public override void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
