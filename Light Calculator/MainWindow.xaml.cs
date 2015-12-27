using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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

namespace Light_Calculator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static List<string> buttonSequences = new List<string>();        

        public MainWindow()
        {
            string version = "v" + (Assembly.GetExecutingAssembly().GetName().Version).ToString();
            string appName = "Light Calculator";
            string titleText = appName + "     " + version;
            
            InitializeComponent();
            this.Title = titleText;                     
            btnRun.IsEnabled = false;            
           
        }


        private void Button_Click(object sender, RoutedEventArgs e)
        {
            List<int> defaultConfig = getLights(tbDefault.Text);
            List<int> button1 = getLights(tbTemple.Text);
            List<int> button2 = getLights(tbTree.Text);
            List<int> button3 = getLights(tbPagota.Text);
            List<int> button4 = getLights(tbFurnace.Text);
            List<int> button5 = getLights(tbPier.Text);
            List<int> button6 = getLights(tbWaterFall.Text);
            List<int> button7 = getLights(tbRock.Text);

            Dictionary<int, int> buttonsPerLight = new Dictionary<int, int>();
            Dictionary<int, bool> lights = new Dictionary<int, bool>();
            List<List<int>> buttonsList = new List<List<int>>();
            Dictionary<int, string> buttonDescriptions = new Dictionary<int, string>();
            
            buttonsList.Add(button1);
            buttonsList.Add(button2);
            buttonsList.Add(button3);
            buttonsList.Add(button4);
            buttonsList.Add(button5);
            buttonsList.Add(button6);
            buttonsList.Add(button7);

            buttonDescriptions.Add(1, "H: Temple");
            buttonDescriptions.Add(2, "A: Tree");
            buttonDescriptions.Add(3, "B: Pagota");
            buttonDescriptions.Add(4, "C: Furnace");
            buttonDescriptions.Add(5, "G: Pier");
            buttonDescriptions.Add(6, "D: WaterFall");
            buttonDescriptions.Add(7, "E: Rock");
            
            // Clear out results text box
            if (!string.IsNullOrEmpty(tbSequence.Text))
                tbSequence.Text = "";
            
            // Determine how many buttons control each light
            buttonsPerLight = getButtonsPerLight(buttonsList);

            // Remove influence of default lights
            for (int i = 0; i < buttonsList.Count; i++)
                buttonsList[i] = flipDefaults(buttonsList[i], defaultConfig);

            // Populate list of 20 lights in off condition
            for (int i = 0; i < 20; i++)
                lights.Add(i + 1, false);

            // Turn on default config lights
            foreach (var light in defaultConfig)
                lights[light] = !lights[light];
                    
            GetCombination(new List<int> { 1, 2, 3, 4, 5, 6, 7 });

            bool foundSequence = false;
            foreach (var sequence in buttonSequences)
            {
                Dictionary<int, bool> tempLights = new Dictionary<int, bool>(lights);

                // Loop through all buttons in button sequence to toggle lights for each button
                foreach (var item in sequence.ToCharArray())
                {
                    int button = (int)Char.GetNumericValue(item);
                    foreach (var p in buttonsList[button - 1])
                        tempLights[p] = !tempLights[p];
                }

                // Check if all lights are on or at least all lights with more than 1 button attached.  If so claim success.
                bool success = true;
                foreach (var item in tempLights)
                {
                    if (item.Value == false)
                        if (buttonsPerLight[item.Key] != 1)
                            success = false;
                }

                if (success)
                {
                    var arr = sequence.ToCharArray();
                    StringBuilder str = new StringBuilder();

                    foreach (var item in arr)
                    {
                        int buttonKey = (int)Char.GetNumericValue(item);
                        str.Append(buttonDescriptions[buttonKey] + " + ");
                    }

                    if(str.Length >= 3)
                        str.Length = str.Length - 2;

                    if (string.IsNullOrEmpty(tbSequence.Text))
                        tbSequence.Text = str.ToString();
                    else
                        tbSequence.Text = tbSequence.Text + "\n" + str.ToString();
                                        
                    foundSequence = true;                 
                }
            }

            if (!foundSequence)
            {
                tbSequence.Text = "Could not find solution...";
            }

            var lightsWithOneButton = getLightsWithOneButton(buttonsPerLight);
            StringBuilder str2 = new StringBuilder();
            foreach (var item in lightsWithOneButton)
                str2.Append(item + ", ");

            if (str2.Length >= 3)
                str2.Length = str2.Length - 2;

            tbLightsWithOneButton.Text = str2.ToString();
        }


        static void GetCombination(List<int> list)
        {
            // Clear button sequences to prevent duplication on multiple runs
            buttonSequences.Clear();

            string temp = "";
            double count = Math.Pow(2, list.Count);
            for (int i = 1; i <= count - 1; i++)
            {
                string str = Convert.ToString(i, 2).PadLeft(list.Count, '0');
                for (int j = 0; j < str.Length; j++)
                {
                    if (str[j] == '1')
                    {
                        temp += list[j];
                    }
                }
                buttonSequences.Add(temp);
                temp = "";
            }
        }

        public List<int> getLights(string lights)
        {
            List<int> results = new List<int>();

            var arr = lights.Split(',');
            foreach (var item in arr)
            {
                int value;
                int.TryParse(item, out value);
                results.Add(value);
            }

            return results;
        }

        static List<int> flipDefaults(List<int> buttonList, List<int> defaultList)
        {
            List<int> results = new List<int>(buttonList);

            foreach (var item in defaultList)
            {
                if (results.Contains(item))
                    results.Remove(item);
                else
                    results.Add(item);
            }

            return results;
        }

        static Dictionary<int, int> getButtonsPerLight(List<List<int>> buttons)
        {
            Dictionary<int, int> results = new Dictionary<int, int>();

            for (int i = 1; i < 21; i++)
                results.Add(i, 0);

            foreach (var button in buttons)
            {
                foreach (var light in button)
                {
                    if (!results.ContainsKey(light))
                        results.Add(light, 1);
                    else
                        results[light] += 1;
                }
            }

            return results;
        }

        static List<int> getLightsWithOneButton(Dictionary<int, int> buttonsPerLight)
        {
            var results = buttonsPerLight.Where(x => x.Value == 1).Select(x => x.Key).ToList();

            return results;
        }

        private void populateTestData()
        {
            tbDefault.Text = "11,17";
            tbTemple.Text = "3,4,8,11,15,17,19";
            tbTree.Text = "4,8,9,11,12,14,15,17";
            tbPagota.Text = "1,2,3,10,11,16,17,19";
            tbFurnace.Text = "1,6,7,9,10,11,12,14,17,20";
            tbPier.Text = "9,12,14";
            tbWaterFall.Text = "5,13,18";
            tbRock.Text = "5,6,7,11,13,17,18,20";

            // 2nd test case from commenter on reddit.  Not sure of data quality

            //tbDefault.Text = "1,9,14";
            //tbTemple.Text = "2,5,6,8,16,20";
            //tbTree.Text = "2,3,4,6,10,20";
            //tbPagota.Text = "2,6,17,18,19,20";
            //tbFurnace.Text = "1,3,4,9,12,14,15";
            //tbPier.Text = "5,7,8,11,13,16";
            //tbWaterFall.Text = "7,10,11,13";
            //tbRock.Text = "1,9,14,17,18,19";
        }

        static bool verifyAllData(List<string> lightData)
        {
            bool success = true;

            foreach (var item in lightData)
            {
                if (!verifyData(item))
                    success = false;
            }

            return success;
        }

        static bool verifyData(string data)
        {
            bool success = true;

            if (string.IsNullOrEmpty(data))
                success = false;

            var arr = data.Split(',');
            foreach (var str in arr)
            {
                int value;
                if (!int.TryParse(str, out value))
                    success = false;
                if (value <= 0 || value > 20)
                    success = false;
            }

            return success;
        }


        private void btnPopulateTest_Click(object sender, RoutedEventArgs e)
        {
            populateTestData();
            btnRun.IsEnabled = true;
            tbSequence.Text = "";
            tbLightsWithOneButton.Text = "";
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            bool enable = true;
            var tb = (TextBox)sender;

            List<string> lightData = new List<string>();
            lightData.Add(tbDefault.Text);
            lightData.Add(tbTemple.Text);
            lightData.Add(tbTree.Text);
            lightData.Add(tbPagota.Text);
            lightData.Add(tbFurnace.Text);
            lightData.Add(tbPier.Text);
            lightData.Add(tbWaterFall.Text);
            lightData.Add(tbRock.Text);

            if (!verifyData(tb.Text))
                tb.Foreground = new SolidColorBrush(Colors.Red);
            else
                tb.Foreground = new SolidColorBrush(Colors.Black);

            if (!verifyAllData(lightData))
                enable = false;

            if (enable == false)
                btnRun.IsEnabled = false;

            if (!btnRun.IsEnabled && enable == true)
                btnRun.IsEnabled = true;
        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            tbDefault.Text = "";
            tbTemple.Text = "";
            tbTree.Text = "";
            tbFurnace.Text = "";
            tbPagota.Text = "";
            tbPier.Text = "";
            tbWaterFall.Text = "";
            tbRock.Text = "";
            tbSequence.Text = "";
            tbLightsWithOneButton.Text = "";

            btnRun.IsEnabled = false;
        }


    }


}
