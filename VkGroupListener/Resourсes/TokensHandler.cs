using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.IO;
using Newtonsoft.Json;

namespace CybersbrodBot.Resourсes
{
    class Tokens
    {
        public static long? GroupId;
        public static string GroupToken;
        public static long? UserId;
        public static string UserToken;
        public static long? AppId;
        public static string AppToken;
        public static string Login;
        public static string Password;

        public static long? Group2Id;
        public static string Group2Token;
        public Tokens(string json)
        {
            var tokensObject = JsonConvert.DeserializeObject<TokensDeserialized>(json);
            GroupId = tokensObject.groupId;
            GroupToken = tokensObject.groupToken;
            AppId = tokensObject.appId;
            AppToken = tokensObject.appToken;
            Group2Id = tokensObject.group2Id;
            Group2Token = tokensObject.group2Token;
        }
    }

    [Serializable]
    class TokensDeserialized
    {
        [JsonProperty("GroupId")]
        public long? groupId { get; private set; }

        [JsonProperty("GroupToken")]
        public string groupToken { get; private set; }
        [JsonProperty("ApplicationId")]
        public long? appId { get; private set; }
        [JsonProperty("ApplicationSecret")]
        public string appToken { get; private set; }
        [JsonProperty("Group2Id")]
        public long? group2Id { get; private set; }

        [JsonProperty("Group2Token")]
        public string group2Token { get; private set; }
    }
}
