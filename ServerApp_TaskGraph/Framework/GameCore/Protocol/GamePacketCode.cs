namespace GameCore
{
    /// <summary>
    /// 게임 패킷 코드.
    /// </summary>
    public static class GamePacketCode
    {
        //////////////////////////////////////////////////////////////////////
        /// UDP 

        // Auth.
        public const int CS_Enter = 10;                         // 플레이어 진입.
        public const int SC_Enter = 11;                         // 플레이어 진입.
        public const int CS_Leave = 12;                         // 플레이어 퇴장.
        public const int SC_Leave = 13;                         // 플레이어 퇴장.

        // Spawn.
        public const int CS_SpawnPlayer = 20;                   // 플레이어 스폰.
        public const int SC_SpawnPlayer = 21;                   // 플레이어 스폰.
        public const int CS_SpawnMob = 22;                      // 몹 스폰.
        public const int SC_SpawnMob = 23;                      // 몹 스폰.

        // 이동.
        public const int CS_MoveStart = 100;                    // 이동 시작.
        public const int SC_MoveStart = 101;                    // 이동 시작.
        public const int CS_MoveEnd = 102;                      // 이동 종료.
        public const int SC_MoveEnd = 103;                      // 이동 종료.
        public const int CS_MoveFixed = 104;                    // 이동 수정.
        public const int SC_MoveFixed = 105;                    // 이동 수정.

        // 공격.
        public const int CS_AttackMelee = 130;                  // 근접 공격.
        public const int SC_AttackMelee = 131;                  // 근접 공격.
        public const int CS_AttackProjectile = 132;             // 투사체 공격.
        public const int SC_AttackProjectile = 133;             // 투사체 공격.

        // 넉백.
        public const int CS_KnockBack = 150;                    // 넉백.
        public const int SC_KnockBack = 151;                    // 넉백.

        // 대시.
        public const int CS_Dash = 160;                         // 대시.
        public const int SC_Dash = 161;                         // 대시.

        // 업데이트.
        public const int CS_UpdateUnitMovement = 170;           // 업데이트 - 이동.
        public const int SC_UpdateUnitMovement = 171;           // 업데이트 - 이동.
        public const int CS_UpdateUnitStat = 172;               // 업데이트 - 스텟.
        public const int SC_UpdateUnitStat = 173;               // 업데이트 - 스텟.

        // 장비.
        public const int CS_SiwtchEquipment = 1000;             // 장비 교체.
        public const int SC_SiwtchEquipment = 1001;             // 장비 교체.

        // 몹 이동 업데이트.
        public const int CS_UpdateMobRoam = 2000;               // 몹 관찰 지역 업데이트.
        public const int SC_UpdateMobRoam = 2001;               // 몹 관찰 지역 업데이트.

        // 더미.
        public const int CS_DummyMove = 10000;                  // 더미 이동.
        public const int SC_DummyMove = 10001;                  // 더미 이동.
    }
}
