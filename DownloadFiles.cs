using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.IO;
using System.Runtime.InteropServices.WindowsRuntime;

namespace ArknightPTS
{
    internal class DownloadFiles
    {
        public Dictionary<string, Dictionary<string, object>> OPERATOR_DICT = new Dictionary<string, Dictionary<string, object>>();
        private dynamic UNEDITED_JSON = null;
        private Dictionary<string, string> CONFIG_FILE = new Dictionary<string, string>();
        private string DEFAULT_URL = "https://raw.githubusercontent.com/Kengxxiao/ArknightsGameData_YoStar/main/en_US/gamedata/excel/character_table.json";
        string DEFAULT_FILEPATH = "Jason/testCharTable.json";
        public string JSON_STRING = "";

        public DownloadFiles()
        {
            LoadConfig();
        }
        public void LoadConfig()
        {
            Dictionary<string, string> defaultConfig = new Dictionary<string, string>()
            {
                { "name", "name" },
                { "nationID", "nationId" },
                { "class", "profession" },
                { "subclass", "subProfessionId" },
                { "recruitment", "itemObtainApproach" },
                { "rarity", "rarity" }
            };
            CONFIG_FILE = defaultConfig;
        }
        public bool PullSimJson()
        {
            bool isSuccess = false;
            using (WebClient client = new WebClient())
            {
                try
                {
                    client.DownloadFile(DEFAULT_URL, DEFAULT_FILEPATH);
                    string json = File.ReadAllText(DEFAULT_FILEPATH);
                    UNEDITED_JSON = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, object>>>(json);

                    return true;
                }
                catch (Exception err)
                {
                    return false;
                }
            }
            return false;
        }
        public void ExtractJson()
        {
            foreach (var entry in UNEDITED_JSON)
            {
                try
                {
                    var key = entry.Key;
                    var value = entry.Value;
                    if (value.ContainsKey("subProfessionId") && value["subProfessionId"].ToString() != "notchar2" &&
                        value.ContainsKey("subProfessionId") && value["subProfessionId"].ToString() != "notchar2")
                    {
                        OPERATOR_DICT[key] = new Dictionary<string, object>();
                        foreach (var kvp in CONFIG_FILE)
                        {
                            OPERATOR_DICT[key][kvp.Key] = value.ContainsKey(kvp.Value) ? value[kvp.Value] : null;
                        }
                    }
                }
                catch
                {
                }
            }
            string json = JsonConvert.SerializeObject(OPERATOR_DICT, Formatting.Indented);
            File.WriteAllText(DEFAULT_FILEPATH, json);
        }
    }
}
