using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VkNet;
using VkNet.Enums;
using VkNet.Enums.SafetyEnums;
using VkNet.Model;
using VkNet.Model.RequestParams;
using VkNet.Utils;
using VkNet.AudioBypassService.Extensions;
using VkNet.Model.GroupUpdate;
using VkNet.Model.Attachments;
using CybersbrodBot.Resourсes;

namespace CybersbrodBot
{
    class ProcessWall
    {
        static VkApi GroupApi;

        public ProcessWall(VkApi groupApi)
        {
            GroupApi = groupApi;
        }

        public static void WallNew(GroupUpdate message)
        {
            Task.Run(() =>
            {
                TPLConsole.ConsoleWrite("Post process");
                var attachments = new List<MediaAttachment>
                {
                    (Wall)message.WallPost
                };

                GroupApi.Messages.Send(new MessagesSendParams
                {
                    RandomId = new Random().Next(2^32),
                    PeerId = 2000000001,   //с59
                    Attachments = attachments,
                });
            });
        }

    }
}
