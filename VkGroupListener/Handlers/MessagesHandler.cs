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

namespace CybersbrodBot
{
    class MessagesHandler
    {
        public static void ReadMessage(GroupUpdate message)
        {
            Task.Run(() =>         
            {
                switch (message.Type.ToString())
                {
                    default:
                        TPLConsole.ConsoleWrite(message.Type.ToString(), ConsoleColor.Red);
                        break;
                    case "message_new":
                        CheckMessage.ProcessMessage(message);
                        break;
                    case "wall_post_new":
                        TPLConsole.ConsoleWrite(message.Type.ToString(), ConsoleColor.Blue);
                        ProcessWall.WallNew(message);
                        break;
                }
            });
        }
    }
}
