using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ArknightPTS
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string PICK_ME = "";
        public enum Menuoptions
        {
            PullSimulator,
            JsonReader,
            DamageCalculator
        }

        public MainWindow()
        {
            InitializeComponent();
            LoadMenu();
        }
        public void LoadMenu()
        {
            List<string> enumValues = new List<string>();
            foreach (var value in Enum.GetValues(typeof(Menuoptions)))
            {
                enumValues.Add(value.ToString());
            }
            Listview_Menu.ItemsSource = enumValues;
        }


        private void Listview_Menu_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string updateDate = "";
            string link = "";
            string paragraph = "";
            string name = "";
            switch(Listview_Menu.SelectedItem.ToString())
            {
                case "PullSimulator" :
                    PICK_ME = "Pull";
                    updateDate = "13/03/2023";
                    link = "https://github.com/KurtVelasco/gacha-simulator";
                    name = "PullSimulator";
                    paragraph = "A simple gacha simulator for arknights (Register-Trademark). Currently in work in progress, but the basic function is working including pulling for units that are part of the headhunt." +
                        "Planning to add headhunting system, including auto-pull.";
                    AddFormattedParagraph(updateDate, link, paragraph, name);
                    break;
                case "JsonReader":
                    PICK_ME = "Json";
                    updateDate = "23/01/2023";
                    link = "https://github.com/KurtVelasco/ArknightsJsonReader";
                    name = "JsonParser";
                    paragraph = "A jsonParse specifically for the extraction of relevant data from the Json file from @Kengxxio's" +
                        " Arknights Character table or something similar. The data can be configured through the config.txt, run the program once to create a standard config.txt.";
                    AddFormattedParagraph(updateDate, link, paragraph, name);
                    break;
                case "DamageCalculator":
                    PICK_ME = "Calc";
                    link = "https://github.com/KurtVelasco/RhodeCalculator";
                    name = "Damage Calculator";
                    updateDate = "13/03/2023";
                    paragraph = "A simple gacha simulator for arknights (Register-Trademark). Currently in work in progress, but the basic function is working including pulling for units that are part of the headhunt." +
                        "Planning to add headhunting system, including auto-pull.";
                    AddFormattedParagraph(updateDate, link, paragraph, name);
                    break;
            }

        }




        private void AddFormattedParagraph(string updateDate, string link, string paragraph , string name)
        {
            Rich_MoD.Document.Blocks.Clear();
            Paragraph formattedParagraph = new Paragraph();
            Run lastUpdatedRun = new Run("Last Updated: ");
            lastUpdatedRun.Foreground = System.Windows.Media.Brushes.Red;
            formattedParagraph.Inlines.Add(lastUpdatedRun);
            formattedParagraph.Inlines.Add(updateDate);

            formattedParagraph.Inlines.Add(new LineBreak());
            Run githubRun = new Run("Github: ");
            formattedParagraph.Inlines.Add(githubRun);

            Hyperlink githubLink = new Hyperlink(new Run(name));
            githubLink.NavigateUri = new Uri(link);
            formattedParagraph.Inlines.Add(githubLink);

            formattedParagraph.Inlines.Add(new LineBreak());
            Run descriptionRun = new Run(paragraph);
            formattedParagraph.Inlines.Add(descriptionRun);
            FlowDocument flowDocument = Rich_MoD.Document;

            flowDocument.Blocks.Add(formattedParagraph);
        }

        private void Button_Enter_Click(object sender, RoutedEventArgs e)
        {
            switch (PICK_ME)
            {
                case "Pull":
                    PullSimulator ps = new PullSimulator();
                    ps.Show();
                    this.Close();
                    break;
                case "Json":
                    ArknightJsonReader aj = new ArknightJsonReader();
                    aj.Show();
                    this.Close();
                    break;

            }
        }
    }
}
