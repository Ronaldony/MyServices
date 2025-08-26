using System.Numerics;

namespace Server_Realm.Entities
{
    using GameCore;

    /// <summary>
    /// PlayerUnit.
    /// </summary>
    public partial class PlayerUnit
    {
        /// <summary>
        /// Siwtch weapon.
        /// </summary>
        public void SwitchWeapon(int weaponId)
        {
            EquipmentHandler.SwitchWeapon(weaponId);
        }
    }
}
