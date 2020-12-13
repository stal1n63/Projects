using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VkNet;
using VkNet.Enums.SafetyEnums;
using VkNet.Model.RequestParams;
using VkNet.Model;
using System.Threading;
using VkNet.Model.GroupUpdate;

namespace CybersbrodBot
{
    class LongPoolHandler
    {
        public LongPoolHandler(VkApi groupApi, LongPollServerResponse poolGroupGetResponse)
        {
            new Thread(() => LoopGroup(groupApi, poolGroupGetResponse)) { Name = "GroupLool", IsBackground = false }.Start();
        }

        void LoopGroup(VkApi _GroupApi, LongPollServerResponse _poolGroupGetResponse)
        {
            TPLConsole.ConsoleWrite("GroupPool loop started >" + _GroupApi.UserId);

            int processed = 0;

            while (true)
            {
                var pool = _GroupApi.Groups.GetBotsLongPollHistory(
                        new BotsLongPollHistoryParams()
                        {
                            Server = _poolGroupGetResponse.Server,
                            Ts = _poolGroupGetResponse.Ts,
                            Key = _poolGroupGetResponse.Key,
                            Wait = 25
                        });

                if (pool?.Updates == null) continue;

                for (int i = processed; i < pool.Updates.Count(); i++)
                {
                    MessagesHandler.ReadMessage(pool.Updates.ElementAt(i));
                    processed++;
                }
            }
        }
    }
}
