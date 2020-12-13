using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace CybersbrodBot.Resourсes
{
    class Localization
    {
        public static string HelpText;

        public Localization(string json)
        {
            var jsondes = JsonConvert.DeserializeObject<LocalizationDeserialized>(json);

            HelpText = jsondes.helpText;
        }
    }

    [Serializable]
    class LocalizationDeserialized
    {
        [JsonProperty("help_text")]
        public string helpText;
    }
}
