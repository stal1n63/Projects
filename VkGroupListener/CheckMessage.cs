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
    class CheckMessage
    {

        public static void ProcessMessage(GroupUpdate message)
        {
            Task.Run(() => 
            {
                if (!CheckForCommand(message)) { /*Message.SendMessage(message, message.MessageNew.Message.Text.Length != 0 ? message.MessageNew.Message.Text : "Empty");*/ }
            });
        }

        static bool CheckForCommand(GroupUpdate message)
        {
            var text = message.MessageNew.Message.Text;
            if (text != null & text.Length != 0)
            {
                if (text.StartsWith("-"))
                {
                    string[] words = text.Split(' ');

                    foreach(var commandWord in MessagesEnums.ChatCommandsEnum)
                    {
                        if(words[0] == commandWord)
                        {
                            TPLConsole.ConsoleWrite(message.MessageNew.Message.Date.Value.ToString("MM/dd/yyyy HH:mm:ss") + " " + commandWord + " invoked by " + message.MessageNew.Message.FromId.ToString());
                            switch (commandWord) { 
                                default:
                                    break;
                                case "-post":
                                    if (message.MessageNew.Message.FromId == 234131638)
                                        PostInWall.RePost(message);
                                    break;
                                case "-help":
                                    Message.SendHelp(message);
                                    break;
                            }
                            return true;
                        }
                    }
                }
            }
            return false;
        }

    }
}
