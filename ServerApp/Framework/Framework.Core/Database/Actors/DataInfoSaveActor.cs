using Akka.Actor;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ServerEngine.Core.Services.Interfaces;
using System.Collections.Concurrent;
using System.Diagnostics;

namespace Framework.Core.Database.Actors
{
    using Framework.Core.Database.Actors.Msgs;

    /// <summary>
    /// DataInfoSaveActor.
    /// </summary>
    public class DataInfoSaveActor : ReceiveActor
    {
        private readonly ILogger<DataInfoSaveActor> _logger;

        private readonly IMemoryPoolService _memoryPoolService;

        public DataInfoSaveActor(IServiceProvider serviceProvider)
        {
            _logger = serviceProvider.GetRequiredService<ILogger<DataInfoSaveActor>>();

            _memoryPoolService = serviceProvider.GetRequiredService<IMemoryPoolService>();

            ReceiveAsync<DBActorMsg_SaveInfo>(async msg => await SaveDatas(msg));
        }

        /// <summary>
        /// Props.
        /// </summary>
        public static Props Props(IServiceProvider serviceProvider)
        {
            return Akka.Actor.Props.Create(() => new DataInfoSaveActor(serviceProvider));
        }

        /// <summary>
        /// Save datas.
        /// </summary>
        private async Task SaveDatas(DBActorMsg_SaveInfo msg)
        {
            _logger.LogDebug($"Start save datas. count: {msg.SaveDatas.Count()}");

            var sw = new Stopwatch();
            sw.Start();

            var tasks = new ConcurrentBag<Task>();

            foreach (var pair in msg.SaveDatas)
            {
                foreach (var dataObject in pair.Value)
                {
                    tasks.Add(dataObject.UpsertAsync(pair.Key));
                }
            }

            await Task.WhenAll(tasks);
            sw.Stop();

            // Release msg.
            _memoryPoolService.Release(msg);

            _logger.LogDebug($"End save datas. ms: {sw.ElapsedMilliseconds}");
        }
    }
}
