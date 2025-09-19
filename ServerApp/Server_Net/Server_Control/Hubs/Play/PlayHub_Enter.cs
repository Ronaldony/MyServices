namespace Server_Control.Hubs.Play
{
    using Framework.Core.Database.DataObjects;
    using Framework.Core.Database.Interfaces;
    using GameCore;
    using GameCore.Datas.Player;
    using GameCore.HubPacket;
    using Microsoft.Extensions.DependencyInjection;
    using Server_Control.Hubs.Msg;
    using Server_Control.Hubs.Play.Datas;
    using ServerEngine.Core.Util;
    using System.Collections.Generic;

    /// <summary>
    /// PlayHub.
    /// </summary>
    public partial class PlayHub
    {
        /// <summary>
        /// Enter
        /// </summary>
        private async Task EnterAsync(PlayHubDataSet dataSet, HubMsg_Enter msg)
        {
            var pid = dataSet.Pid;

            dataSet.IsEnter = true;

            // PostInfo가 없는 경우 추가.
            if (dataSet.PostInfo == null)
            {
                // PlayInfo.
                var postInfoObject = _serviceProvider.GetRequiredService<PostInfoObject>();
                await postInfoObject.LoadDataAsync(pid);

                var dataObjects = new List<IDataObject>();
                dataObjects.Add(postInfoObject);

                dataSet.PostInfo = postInfoObject.PostInfo;
                _hubGroup.DataObjectDict.AddOrUpdate(pid, dataObjects, (old, key) => dataObjects);
            }

            // 우편물 검사.
            CheckPosts(dataSet.PostInfo);

            // 우편물 정보 전송.
            SendPacket<HubPacketSC_Enter>(pid, HubPacketCode.SC_Enter, new HubPacketSC_Enter
            {
                PostInfo = dataSet.PostInfo,
            });
        }

        /// <summary>
        /// Leave
        /// </summary>
        private void Leave(PlayHubDataSet dataSet, HubMsg_Leave msg)
        {
            _hubGroup.DataSets.Remove(dataSet.Pid);
        }

        /// <summary>
        /// Enter zone.
        /// </summary>
        private void EnterZone(PlayHubDataSet dataSet, HubMsg_EnterZone msg)
        {
            dataSet.ZoneId = msg.ZoneId;
        }

        /// <summary>
        /// Check posts.
        /// </summary>
        private void CheckPosts(DTO_PostInfo postInfo)
        {
            var deletePostUids = new List<string>();
            var nowTime = TimeUtil.Now;

            foreach (var post in postInfo.Posts)
            {
                // 15일이 지난 우편 기록 삭제.
                if (TimeUtil.IsPastTime(post.Value.RegTime.AddDays(15), nowTime))
                {
                    deletePostUids.Add(post.Key);
                }
            }

            // 오랜 우편물 삭제.
            foreach (var postUid in deletePostUids)
            {
                postInfo.Posts.Remove(postUid);
            }
        }
    }
}
