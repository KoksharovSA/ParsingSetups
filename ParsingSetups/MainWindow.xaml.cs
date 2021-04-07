using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ParsingSetups
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFile = new OpenFileDialog();
            openFile.Multiselect = true;
            openFile.ShowDialog();
            foreach (var dir in openFile.FileNames)
            {
                int CountProgs = 1;
                string nameSetup = "";
                Dictionary<string, int> setup = new Dictionary<string, int>();
                using (StreamReader sr = new StreamReader(dir, Encoding.Default))
                {                 
                    string line = "";
                    bool flag1 = false;
                    bool flag2 = false;
                    while ((line = sr.ReadLine()) != null)
                    {
                        string name;
                        string count;
                        if (flag1)
                        {
                            string[] c = line.Split('>');
                            CountProgs = Convert.ToInt32(c[2].Split('&')[0]);
                            flag1 = false;
                        }
                        if (line.Contains("КОЛ-ВО ПРОГОНОВ ПРОГРАММЫ"))
                        {
                            flag1 = true;
                        }
                        if (flag2)
                        {
                            nameSetup = line.Split('\\')[line.Split('\\').Length - 1].Split('&')[0].Substring(0, line.Split('\\')[line.Split('\\').Length - 1].Split('&')[0].Length-4);
                            break;
                        }
                        if (line.Contains("ИМЯ ЛИСТА"))
                        {
                            flag2 = true;
                        }                        
                        if (line.Contains("NOID"))
                        {
                            string[] a = line.Split('>');

                            if (a[11].Contains("GEO"))
                            {                                
                                name = new FileInfo(a[11].Split('&')[0]).Name;
                                count = a[15].Split('&')[0];
                                setup.Add(name, Convert.ToInt32(count) * CountProgs);
                            }                            
                        }                        
                    }
                }
                TextBoxDetail.Text += TextBoxDetail.Text==""? nameSetup + " (" + CountProgs + " п/п)\n": "\n" + nameSetup + " (" + CountProgs + " п/п)\n";
                foreach (var item in setup)
                {
                    TextBoxDetail.Text += item.Key + " - " + item.Value + "шт.\n";
                }
            }
        }
    }
}
