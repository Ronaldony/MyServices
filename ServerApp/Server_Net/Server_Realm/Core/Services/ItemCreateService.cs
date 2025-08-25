using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Server_Realm.Core.Services
{
    using GameCore;
    using GameCore.Datas.Items;
    using GameCore.Interfaces;
    using ServerEngine.Core.Services.Interfaces;
    using ServerEngine.Core.Util;

    /// <summary>
    /// ItemCreateService.
    /// </summary>
    public class ItemCreateService
    {
        private readonly ILogger<ItemCreateService> _logger;

        private readonly ITDBService _tdbService;
        private readonly IDataSerializer _dataSerializer;

        private readonly Random _random;

        /// <summary>
        /// Item group dictionary.
        /// key: GroupId
        /// value: DTO_Item list bytes.
        /// </summary>
        private Dictionary<int, byte[]> _itemGroupDict;

        public ItemCreateService(IServiceProvider serviceProvider)
        {
            _logger = serviceProvider.GetRequiredService<ILogger<ItemCreateService>>();

            _tdbService = serviceProvider.GetRequiredService<ITDBService>();
            _dataSerializer = serviceProvider.GetRequiredService<IDataSerializer>();

            _random = new Random((int)TimeUtil.Now.Ticks);
        }

        /// <summary>
        /// Initialize.
        /// </summary>
        public void Initialize()
        {
            _itemGroupDict = Initialize_ItemGroups();

            _logger.LogInformation($"ItemCreateService initialize.");
        }

        /// <summary>
        /// Initialize ItemGroups.
        /// </summary>
        private Dictionary<int, byte[]> Initialize_ItemGroups()
        {
            var itemGroupDict = new Dictionary<int, byte[]>();

            var groupItems = _tdbService.SelectAll<TB_ItemGroup>()
                .GroupBy(d => d.GroupId)
                .ToDictionary(d => d.Key, d => d.ToList());

            foreach (var group in groupItems)
            {
                var dtoItems = new List<DTO_Item>();

                // TB_ItemGroup 부터 TB_Item, TB_ItemRate를 순서대로 검색.
                foreach (var tbItemGroup in group.Value)
                {
                    var tbItem = _tdbService.GetById<TB_Item>(tbItemGroup.ItemRefId);
                    var tbItemRate = _tdbService.GetById<TB_ItemRate>(tbItem.RateRef);

                    // Count를 미리 MaxCount로 세팅. (아이템 그룹 생성 시 다시 계산)
                    // Rate = Rate(TB_ItemRate) + RateWeight(TB_ItemGroup).
                    dtoItems.Add(new DTO_Item
                    {
                        RefId = tbItem.Id,
                        Count = tbItemGroup.MaxCount,
                        Rate = tbItemRate.Rate + tbItemGroup.RateWeight
                    });
                }

                // Serialize.
                var itemsBytes = _dataSerializer.Serialize(dtoItems);

                itemGroupDict.Add(group.Key, itemsBytes);
            }

            return itemGroupDict;
        }


        /// <summary>
        /// Create item groups.
        /// </summary>
        public List<DTO_Item> CreateItemGroups(int groupId)
        {
            if (false == _itemGroupDict.ContainsKey(groupId))
            {
                return new List<DTO_Item>();
            }

            var removeItems = new List<DTO_Item>();
            var itemGroups = _dataSerializer.Deserialize<List<DTO_Item>>(_itemGroupDict[groupId]);

            // Count 재계산.
            foreach (var item in itemGroups)
            {
                var totalRate = 100 / (double)(item.Rate + item.Rate);
                var randomNum = _random.NextDouble() * 100;

                _logger.LogDebug($"item refId: {item.RefId}, rate: {item.Rate}, total rate: {totalRate}, randomNum: {randomNum}");

                // 난수가 확률 보다 큰 경우, 해당 아이템 제외.
                if (totalRate < randomNum)
                {
                    removeItems.Add(item);
                }
                else
                {
                    item.Count = _random.Next(1, item.Count);
                }
            }

            // 제외 아이템 제거.
            foreach (var item in removeItems)
            {
                itemGroups.Remove(item);
            }

            return itemGroups;
        }
    }
}
