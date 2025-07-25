using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Numerics;

namespace Server_Realm.UnitHandler
{
    using GameCore.Datas.Plays;
    using Server_Realm.Entities;

    /// <summary>
    /// UnitStatHandler.
    /// </summary>
    public class UnitStatHandler
    {
        private readonly ILogger<UnitStatHandler> _logger;

        // Unit.
        private readonly Unit _owner;

        // DTO.
        private readonly DTO_Stat _stat;

        #region
        public int GetPower() => _stat.Power;
        #endregion

        public UnitStatHandler(IServiceProvider serviceProvider, Unit owner, DTO_Stat stat)
        {
            _logger = serviceProvider.GetRequiredService<ILogger<UnitStatHandler>>();

            _owner = owner;
            _stat = stat;
        }

        /// <summary>
        /// Update.
        /// </summary>
        public void Update(float deltaTime)
        {

        }

        /// <summary>
        /// Move.
        /// </summary>
        public void Hit(Vector2 position)
        {

        }
    }
}
