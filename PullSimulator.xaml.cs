using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.IO;
using System.Windows.Media.Animation;
using System.Windows.Media.TextFormatting;
using System.IO.Ports;
using Newtonsoft.Json.Linq;
using System.Collections;
using System.Threading;
using System.Net;

namespace ArknightPTS
{
    /// <summary>
    /// Interaction logic for PullSimulator.xaml
    /// </summary>
    public partial class PullSimulator : Window
    {
       
        public Dictionary<string, Character> CHARACTERS_LOADED;    
        private List<Character> TIER_3 = new List<Character>();
        private List<Character> TIER_4 = new List<Character>();
        private List<Character> TIER_5 = new List<Character>();
        private List<Character> TIER_6 = new List<Character>();
        private static Random random = new Random();
        private int TOTAL_PULLS = 0;
        private int PULLS_6STAR = 0;
        private int LAST_6STAR = 0;
        private int SIX_STAR = 0;
        private int FIVE_STAR = 0;
        private int FOUR_STAR = 0;
        private double SPENDING = 0;

        public class Character
        {
            public string Key {  get; set; }
            public string Name { get; set; }
            public string Class { get; set; }
            public string Rarity { get; set; }
            public string Recruitment { get; set; }
        }
        public PullSimulator()
        {
            InitializeComponent();
            LoadCharacters();
            LoadCharactersByRarity();

        }
     
        //This codes looks ugly as hell and may cause an infinite loop
        // Will fix later maybe prob 
        private void LoadCharacters()
        {
            try
            {
                string json = File.ReadAllText("Jason/testCharTable.json");
                CHARACTERS_LOADED = JsonConvert.DeserializeObject<Dictionary<string, Character>>(json);
                foreach (var kvp in CHARACTERS_LOADED)
                {
                    kvp.Value.Key = kvp.Key;
                }
            }
            catch
            {
                MessageBoxResult ms = MessageBox.Show("No Character Table available. Do you want to Download a dummy Char Json?", "No Json File"
                    , MessageBoxButton.YesNo, MessageBoxImage.Information);
                if(ms != MessageBoxResult.Yes)
                {
                    Button_X10Pull.IsEnabled = false;
                    MessageBox.Show("Failed to Download/Json File, Check for Internet Connection or manually add the Json File", "No Json File");
                }
                else
                {
                    DownloadFiles downloadFiles = new DownloadFiles();
                    if(downloadFiles.PullSimJson()){
                        downloadFiles.ExtractJson();
                        LoadCharacters();
                    }
                }
            }
        }
        private void LoadCharactersByRarity()
        {
            foreach (var character in CHARACTERS_LOADED)
            {

                var key = character.Key;
                var value = character.Value;
                string rarity = character.Value.Rarity;
                switch (rarity)
                {
                    case "TIER_3":
                        if (character.Value.Recruitment == "Recruitment & Headhunting")
                        {
                            TIER_3.Add(character.Value);
                        }
                        break;
                    case "TIER_4":
                        if (character.Value.Recruitment == "Recruitment & Headhunting")
                        {
                            TIER_4.Add(character.Value);
                        }
                        break;
                    case "TIER_5":
                        if (character.Value.Recruitment == "Recruitment & Headhunting")
                        {
                            TIER_5.Add(character.Value);
                        }
                        break;
                    case "TIER_6":
                        if (character.Value.Recruitment == "Recruitment & Headhunting")
                        {
                            TIER_6.Add(character.Value);
                        }
                        break;
                }
            }
        }
        private Character GetRandomCharacterFromRarity(string rarity)
        {
            int randomIndex = 0;
            Character result = null;
            List<Character> characterList = new List<Character>();
            switch (rarity)
            {               
                case "TIER_3":
                    randomIndex = random.Next(TIER_3.Count);
                    result = TIER_3[randomIndex];
                    break;
                case "TIER_4":
                    randomIndex = random.Next(TIER_4.Count);
                    result = TIER_4[randomIndex];
                    break;
                case "TIER_5":
                    randomIndex = random.Next(TIER_5.Count);
                    result = TIER_5[randomIndex];
                    break;
                case "TIER_6":
                    randomIndex = random.Next(TIER_6.Count);
                    result = TIER_6[randomIndex];
                    break;
            }
            return result;
        }
        private Character SimulatePull(double tierRoll)
        {
            if (tierRoll <= 0.4)
            {
                LAST_6STAR += 1;
                return GetRandomCharacterFromRarity("TIER_3");
            }
            else if (tierRoll <= 0.9)
            {
                LAST_6STAR += 1;
                return GetRandomCharacterFromRarity("TIER_4");
            }
            else if (tierRoll <= 0.98)
            {
                LAST_6STAR += 1;
                return GetRandomCharacterFromRarity("TIER_5");
            }
            else
            {
                LAST_6STAR = 0;
                PULLS_6STAR += 1;
                return GetRandomCharacterFromRarity("TIER_6");
            }
        }
        private void SimulatePulls(int numPulls)
        {
            TOTAL_PULLS += numPulls;

            List<Character> results = new List<Character>();
            for (int i = 0; i < numPulls; i++)
            {
                double tierRoll = random.NextDouble();
                Character result = SimulatePull(tierRoll);

                results.Add(result);
            }
            FillListView(results);

        }

        private void FillListView(List<Character> output)
        {
            ListView_PullWindow.Items.Clear();
            foreach(Character ch in output ){
                ListView_PullWindow.Items.Add(new Character
                {
                    Name = ch.Name, 
                    Rarity = ch.Rarity, 
                    Key = ch.Key
                });
            }
        }


        private async void DownloadAndDisplayImage(string id)
        {
            try
            {
                string imageUrl = "https://raw.githubusercontent.com/Aceship/Arknight-Images/main/avatars/" + id + ".png";
                byte[] imageData = await DownloadImageAsync(imageUrl);
                DisplayImage(imageData);
            }
            catch (Exception ex)
            {
            }
        }

        private async Task<byte[]> DownloadImageAsync(string imageUrl)
        {
            using (var httpClient = new WebClient())
            {
                return await httpClient.DownloadDataTaskAsync(imageUrl);
            }
        }

        private void DisplayImage(byte[] imageData)
        {
            try
            { 
                using (MemoryStream ms = new MemoryStream(imageData))
                {
                    BitmapImage bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.StreamSource = ms;
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.EndInit();
                    Image_Ops.Source = bitmap;
                }
            }
            catch (Exception ex)
            {
            }
        }

        private void ListView_PullWindow_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
                string Id = "";
            foreach (Character selectedItem in ListView_PullWindow.SelectedItems)
            {
                Id = selectedItem.Key;
            }
            DownloadAndDisplayImage(Id);
        }

        private void Button_Reset_Click(object sender, RoutedEventArgs e)
        {
            PULLS_6STAR = 0;
            TOTAL_PULLS = 0;
            LAST_6STAR = 0;
            SIX_STAR = 0;   
            FIVE_STAR = 0;
            FOUR_STAR = 0;
            SPENDING = 0;
            ResetCounter();
        }

        private void Button_1XPull_Click(object sender, RoutedEventArgs e)
        {
            SimulatePulls(1);
            ResetCounter();
        }

        private void Button_X10Pull_Click(object sender, RoutedEventArgs e)
        {
            SimulatePulls(10);
            ResetCounter();
        }
        private void ResetCounter()
        {
            TextBox_TOTALPULLS.Text = TOTAL_PULLS.ToString();
            TextBox_6STARPULLS.Text = PULLS_6STAR.ToString();
            TextBox_LAST6STAR.Text = LAST_6STAR.ToString();
            Textbox_6Stars.Text = SIX_STAR.ToString();
            Textbox_5Stars.Text = FIVE_STAR.ToString();
            Textbox_4Stars.Text = FOUR_STAR.ToString();
        }

    }
}
