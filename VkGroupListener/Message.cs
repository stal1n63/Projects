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
using Microsoft.Extensions.DependencyInjection;
using CybersbrodBot.Resourсes;
using VkNet.Model.GroupUpdate;
using VkNet.Model.Attachments;

namespace CybersbrodBot
{
    class Message
    {        static VkApi GroupApi;

        public Message(VkApi groupApi)
        {
            GroupApi = groupApi;
        }

        public static void SendHelp(GroupUpdate message)
        {
            Task.Run(() => {
                GroupApi.Messages.Send(new MessagesSendParams
                {
                    RandomId = new Random().Next(999999),
                    UserId = message.MessageNew.Message.FromId,
                    Message = Localization.HelpText
                });
            });
        }

        public static void SendMessage(GroupUpdate message, string text)
        {
            Task.Run(() =>
            {
                if (message.MessageNew.Message.Attachments.Count != 0)
                {
                    var attachments = new List<MediaAttachment>
                    {
                    message.MessageNew.Message.Attachments[0].Instance
                    };
                    text = message.MessageNew.Message.Attachments[0].Instance.AccessKey + " | " + message.MessageNew.Message.Attachments[0].Instance.Id + " | " + message.MessageNew.Message.Attachments[0].Instance.OwnerId;

                    GroupApi.Messages.Send(new MessagesSendParams
                    {
                        RandomId = new Random().Next(999999),
                        UserId = message.MessageNew.Message.FromId,
                        Message = text,
                        Attachments = attachments
                    });
                }
                else
                {
                    GroupApi.Messages.Send(new MessagesSendParams
                    {
                        RandomId = new Random().Next(999999),
                        UserId = message.MessageNew.Message.FromId,
                        Message = text,
                    });
                }
            });
        }
    }
}
