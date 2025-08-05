using Microsoft.Extensions.DependencyInjection;

namespace Server_Realm.AI.Actions
{
    using GameCore;
    using GameCore.Interfaces;
    using Server_Realm.AI.Actions.Movement;
    using Server_Realm.AI.Actions.States;
    using Server_Realm.Entities;
    using Server_Realm.Units.Mob;

    /// <summary>
    /// AIActionCollection.
    /// </summary>
    public class AIActionCollection
    {
        /// <summary>
        /// AI dictionary.
        /// key: Type_AIState
        /// value: AIAction.
        /// </summary>
        private readonly Dictionary<Type_AIState, AIAction> _aiAcionDict;

        /// <summary>
        /// Unit.
        /// </summary>
        public Unit Owner { get; }
        
        public AIActionCollection(IServiceProvider serviceProvider, MobUnit owner)
        {
            Owner = owner;

            _aiAcionDict = Initialize_AIActions(serviceProvider, owner);
        }

        /// <summary>
        /// Initialize AIActions.
        /// </summary>
        private Dictionary<Type_AIState, AIAction> Initialize_AIActions(IServiceProvider serviceProvider, MobUnit owner)
        {
            var tdbService = serviceProvider.GetRequiredService<ITDBService>();
            var tbMob = tdbService.GetById<TB_Mob>(owner.Mob.MobId);

            var aiActions = new Dictionary<Type_AIState, AIAction>();

            // States.
            aiActions.Add(Type_AIState.Idle, new AIIdleAction(owner));
            aiActions.Add(Type_AIState.Dead, new AIDeadAction(owner));

            // Movement.
            aiActions.Add(Type_AIState.Roam, new AIRoamAction(owner));
            aiActions.Add(Type_AIState.Return, new AIReturnAction(owner));

            // Combat.
            aiActions.Add(Type_AIState.Combat, new AICombatAction(owner, tbMob));

            return aiActions;
        }

        /// <summary>
        /// Get AI Action by state.
        /// </summary>
        public AIAction GetAIActoin(Type_AIState state)
        {
            return _aiAcionDict.GetValueOrDefault(state);
        }
    }
}
