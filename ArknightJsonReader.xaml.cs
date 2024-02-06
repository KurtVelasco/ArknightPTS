using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ArknightPTS
{
    /// <summary>
    /// Interaction logic for ArknightJsonReader.xaml
    /// </summary>
    public partial class ArknightJsonReader : Window
    {
        private Dictionary<string, Dictionary<string, object>> OPERATOR_DICT = new Dictionary<string, Dictionary<string, object>>();
        private dynamic UNEDITED_JSON = null;
        private Dictionary<string, string> CONFIG_FILE = new Dictionary<string, string>();
        private string DEFAULT_URL = "https://raw.githubusercontent.com/Kengxxiao/ArknightsGameData_YoStar/main/en_US/gamedata/excel/character_table.json";
        string DEFAULT_FILEPATH = "arknightChar" + ".json";
        public ArknightJsonReader()
        {
            InitializeComponent();
            LoadConfig();
            Textbox_DownloadURL.Text = DEFAULT_URL; 
        }
        private void LoadConfig()
        {
            try
            {
                string[] lines = File.ReadAllLines("dataConfig.txt");
                foreach (string line in lines)
                {
                    string[] parts = line.Split(':');
                    CONFIG_FILE[parts[0]] = parts[1];
                }
            }
            catch (FileNotFoundException)
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
                using (StreamWriter writer = new StreamWriter("dataConfig.txt"))
                {
                    foreach (var kv in defaultConfig)
                    {
                        writer.WriteLine($"{kv.Key}:{kv.Value}");
                    }
                }
                CONFIG_FILE = defaultConfig;
                ListBox_Logs.Items.Add("dataConfig.txt was not found, genrating a new config file");
            }
        }
        private void ExtractJson()
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
                    else
                    {
                        ListBox_Logs.Items.Add(entry.Key + " Removed from pool");
                    }
                }
                catch
                {
                    MessageBox.Show("The Json File is invalid", "Invalid Json File", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            DumpJson();
        }
        private void DumpJson()
        {
  
            if (Textbox_FileName.Text.Length > 0)
            {
                string json = JsonConvert.SerializeObject(OPERATOR_DICT, Formatting.Indented);
                File.WriteAllText(Textbox_FileName.Text + ".json", json);
            }           
        }
        private void Button_Download_Click(object sender, RoutedEventArgs e)
        {
            bool isSuccess = false;
            using (WebClient client = new WebClient())
            {
                ListBox_Logs.Items.Add("Downloaded from the following file");
                try
                {
                    client.DownloadFile(DEFAULT_URL, DEFAULT_FILEPATH);
                    ListBox_Logs.Items.Add("File Downloaded");
                    string json = File.ReadAllText(DEFAULT_FILEPATH);
                    UNEDITED_JSON = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, object>>>(json);
                    isSuccess = true;
                }
                catch (Exception err)
                {
                    ListBox_Logs.Items.Add(err);
                }
            }
            Button_ExtractJSON.IsEnabled = true;
        }
        private void GetFileLocation()
        {

            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Json File|*.json";
            if (saveFileDialog.ShowDialog() == true)
            {
                Textbox_FileName.Text = saveFileDialog.FileName;
            }
            
        }
        private void Border_Drop(object sender, DragEventArgs e)
        {
            string json = "";
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

                if(files.Count() > 1)
                {
                    MessageBox.Show("Only one File is allowed", "Multiple Files Inserted", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                // Process dropped files
                foreach (string file in files)
                {
                    if (System.IO.Path.GetExtension(file).Equals(".json", StringComparison.OrdinalIgnoreCase))
                    {
                        json = File.ReadAllText(file);
                        UNEDITED_JSON = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, object>>>(json);
                        Button_ExtractJSON.IsEnabled = true;
                    }
                    else
                    {
                        MessageBox.Show($"Unsupported file format: {file}");
                        break;
                    }
                }
            }
        }

        private void Button_FileLocation_Click(object sender, RoutedEventArgs e)
        {
            GetFileLocation();
        }

        private void Button_ExtractJSON_Click(object sender, RoutedEventArgs e)
        {
            if(Textbox_FileName.Text.Length == 0)
            {
                MessageBox.Show("Define the filepath for the Json File", "Invalid FilePath", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            ExtractJson();
        }
    }
}
