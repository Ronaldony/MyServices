
namespace Server_Realm.Entities
{
    using GameCore.Datas.Post;

    /// <summary>
    /// PlayerUnit.
    /// </summary>
    public partial class PlayerUnit
    {
        /// <summary>
        /// Retrieve post
        /// </summary>
        public void RetrievePost(DTO_Post post)
        {
            foreach (var item in post.Items)
            {
                if (false == InventoryInfo.Items.ContainsKey(item.RefId))
                {
                    InventoryInfo.Items.Add(item.RefId, item);
                    continue;
                }

                // 아이템 수량 증가.
                InventoryInfo.Items[item.RefId].Count += item.Count;
            }
        }
    }
}
    