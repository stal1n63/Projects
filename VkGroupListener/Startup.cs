using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VkNet;
using VkNet.Model;
using VkNet.AudioBypassService.Extensions;
using Microsoft.Extensions.DependencyInjection;
using CybersbrodBot.Resourсes;
using System.Reflection;
using System.IO;

/*
Прослушиватель VkLongApi на основе библиотеки VkNet
Функционал - прослушка одной группы на наличие новых постов, и перессылка данной новости от имени другой группы
иному пользователю, беседе и прочее.
*/
namespace CybersbrodBot
{
    class Startup
    {
        static void Main()
        {
            new Tokens(OpenTokens());
            new Localization(OpenLocalization());

            var services = new ServiceCollection();
            services.AddAudioBypass();

            var GroupApi = new VkApi(services);
            GroupApi.Authorize(new ApiAuthParams() { AccessToken = Tokens.GroupToken });

            var Group2Api = new VkApi(services);
            Group2Api.Authorize(new ApiAuthParams() { AccessToken = Tokens.Group2Token });

            new Message(GroupApi);
            //new PostInWall(UserApi);
            new LongPoolHandler(GroupApi, GroupApi.Groups.GetLongPollServer((ulong)Tokens.GroupId));
            new LongPoolHandler(Group2Api, Group2Api.Groups.GetLongPollServer((ulong)Tokens.Group2Id));
            new ProcessWall(GroupApi);
        }
        public static string OpenTokens()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = "CybersbrodBot.Resourсes.Tokens.json";

            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            using (StreamReader reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }
        public static string OpenLocalization()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = "CybersbrodBot.Resourсes.Localization_RU.json";

             using (Stream stream = assembly.GetManifestResourceStream(resourceName))
             using (StreamReader reader = new StreamReader(stream))
             {
                 return reader.ReadToEnd();
             }
        }
    }
}
