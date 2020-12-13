using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using VkNet;
using VkNet.Enums.SafetyEnums;
using VkNet.Model.RequestParams;
using VkNet.Model;
using VkNet.Model.GroupUpdate;
using CybersbrodBot.Resourсes;

namespace CybersbrodBot
{
    class PostInWall
    {
        static VkApi _api;

        public PostInWall(VkApi api)
        {
            _api = api;
        }

        public static void RePost(GroupUpdate message)
        {
            Task.Run(() =>
            {
                _api.Wall.Post(new WallPostParams
                {
                    OwnerId = -188415343,
                    FromGroup = true,
                    Message = message.MessageNew.Message.Text.Remove(0, 5) + " (C) [id" + message.MessageNew.Message.FromId + "|Цитата]"
                });
            });
        }

        public static void RePostImage(GroupUpdate message)
        {/*
            _api.Wall.Post(new WallPostParams
            {
                OwnerId = Tokens.GroupId,
                FromGroup = true,
                Message = ""
            });*/
        }
    }
}
