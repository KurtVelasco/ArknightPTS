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
using static ArknightPTS.PullSimulator;
using System.Windows.Media.Animation;
using System.Windows.Media.TextFormatting;
using System.IO.Ports;
using Newtonsoft.Json.Linq;
using System.Collections;
using System.Threading;

namespace ArknightPTS
{
    /// <summary>
    /// Interaction logic for PullSimulator.xaml
    /// </summary>
    public partial class PullSimulator : Window
    {
        private Dictionary<string, Dictionary<string, object>> OPERATOR_DICT = new Dictionary<string, Dictionary<string, object>>();
        private Dictionary<string, Character> CHARACTERS_LOADED;
        private Dictionary<string, Character> charactersByRarity = new Dictionary<string, Character>();
        private List<Character> TIER_3 = new List<Character>();
        private List<Character> TIER_4 = new List<Character>();
        private List<Character> TIER_5 = new List<Character>();
        private List<Character> TIER_6 = new List<Character>();
        private static Random random = new Random();
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
     

        private void LoadCharacters()
        {
            string json = File.ReadAllText("meme.json.json");
            CHARACTERS_LOADED = JsonConvert.DeserializeObject<Dictionary<string, Character>>(json);
            foreach (var kvp in CHARACTERS_LOADED)
            {
                kvp.Value.Key = kvp.Key;
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
                return GetRandomCharacterFromRarity("TIER_3");
            }
            else if (tierRoll <= 0.9)
            {
                return GetRandomCharacterFromRarity("TIER_4");
            }
            else if (tierRoll <= 0.98)
            {
                return GetRandomCharacterFromRarity("TIER_5");
            }
            else
            {
                return GetRandomCharacterFromRarity("TIER_6");
            }
        }
        private void SimulatePulls(int numPulls)
        {
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
                });
            }
        }
        private void Button_10XPull_Click(object sender, RoutedEventArgs e)
        {
            SimulatePulls(10);
        }
    }
}
