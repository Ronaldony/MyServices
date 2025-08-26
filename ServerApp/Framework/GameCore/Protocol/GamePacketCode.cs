namespace GameCore
{
    /// <summary>
    /// 게임 패킷 코드.
    /// </summary>
    public static class GamePacketCode
    {
        //////////////////////////////////////////////////////////////////////
        /// Game

        // Auth.
        public const int CS_Enter = 10;                         // 플레이어 진입.
        public const int SC_Enter = 11;                         // 플레이어 진입.
        public const int CS_Leave = 12;                         // 플레이어 퇴장.
        public const int SC_Leave = 13;                         // 플레이어 퇴장.

        // Spawn.
        public const int CS_EnterPlayer = 20;                   // 플레이어 진입.
        public const int SC_EnterPlayer = 21;                   // 플레이어 진입.
        public const int CS_SpawnUnits = 22;                   // 플레이어 스폰.
        public const int SC_SpawnUnits = 23;                   // 플레이어 스폰.
        public const int CS_SpawnMob = 24;                      // 몹 스폰.
        public const int SC_SpawnMob = 25;                      // 몹 스폰.

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
        public const int CS_UpdateSection = 174;                // 업데이트 - 섹션.
        public const int SC_UpdateSection = 175;                // 업데이트 - 섹션.
        public const int CS_UpdateSectionNewEnter = 176;        // 업데이트 - 섹션에 새로 진입한 유저.
        public const int SC_UpdateSectionNewEnter = 177;        // 업데이트 - 섹션에 새로 진입한 유저.

        // 아이템.
        public const int CS_DropItems = 190;                    // 아이템 드랍.
        public const int SC_DropItems = 191;                    // 아이템 드랍.
        public const int CS_RootItem = 192;                     // 아이템 루트.
        public const int SC_RootItem = 193;                     // 아이템 루트.

        // 장비.
        public const int CS_SwitchWeapon = 500;                 // 무기 교체.
        public const int SC_SwitchWeapon = 501;                 // 무기 교체.
        public const int CS_SwitchEquipment = 502;              // 장비 교체.
        public const int SC_SwitchEquipment = 503;              // 장비 교체.

        // 더미.
        public const int CS_DummyMove = 1000;                  // 더미 이동.
        public const int SC_DummyMove = 1001;                  // 더미 이동.
    }
}
