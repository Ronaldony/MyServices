
namespace Server_Realm.Entities
{
    using GameCore;
    using Microsoft.AspNetCore.Components.Web;

    /// <summary>
    /// PlayerUnit.
    /// </summary>
    public partial class PlayerUnit
    {
        /// <summary>
        /// Root item.
        /// </summary>
        public bool ConsumeItem(TB_Item tbItem)
        {
            bool isConsume = false;

            // Consume 1.
            var tbItemConsume = _tdbService.GetById<TB_ItemConsume>(tbItem.Consume_Ref1);

            if (null != tbItemConsume)
            {
                UseItem(tbItemConsume, tbItem.Consume_Value1);
                isConsume = true;
            }

            // Consume 2.
            tbItemConsume = _tdbService.GetById<TB_ItemConsume>(tbItem.Consume_Ref2);

            if (null != tbItemConsume)
            {
                UseItem(tbItemConsume, tbItem.Consume_Value2);
                isConsume = true;
            }

            return isConsume;
        }

        /// <summary>
        /// Use item.
        /// </summary>
        private void UseItem(TB_ItemConsume tbItemConsume, int consumeValue)
        {
            switch (tbItemConsume.ConsumeType)
            {
                case Type_ItemConsume.HP_Plus:
                    StatHandler.Heal(consumeValue);
                    break;
            }
        }
    }
}
    