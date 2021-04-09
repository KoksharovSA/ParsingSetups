using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SQLite;
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
            openFile.Filter = "Setup files (*.html)|*.html";
            openFile.ShowDialog();
            foreach (var dir in openFile.FileNames)
            {
                Setup setup = ParseSetup(dir);
                DB.AddDBSetup(new SQLiteConnection("Data source=C:\\Users\\User\\Desktop\\Setups.db;Version=3"), setup);
            }
        }

        internal bool SetupInToBase(Setup setup)
        {


            return true;
        }



        internal Setup ParseSetup(string dir)
        {
            Setup setup = new Setup();
            using (StreamReader sr = new StreamReader(dir, Encoding.Default))
            {
                string line = "";
                bool flag1 = false;//Дирректория программы
                bool flag2 = false;//Название программы
                bool flag3 = false;//Материал
                bool flag4 = false;//Размер листа
                bool flag5 = false;//Время
                bool flag6 = false;//Количество прогонов
                bool flag7 = false;//Процент отхода

                while ((line = sr.ReadLine()) != null)
                {
                    setup.DateSpellingSetup = DateTime.Now.ToString();
                    if (flag1)
                    {
                        string[] c = line.Split('>');
                        setup.DirSetup = c[3].Split('&')[0];
                        flag1 = false;
                    }
                    if (line.Contains("МАРШРУТ УПР. ПРОГРАММЫ"))
                    {
                        flag1 = true;
                    }

                    if (flag2 && line.Contains("nbsp"))
                    {
                        string[] c = line.Split('>');
                        setup.NameSetup = c[1].Split('&')[0];
                        flag2 = false;
                    }
                    if (line.Contains("ИМЯ ПРОГРАММЫ"))
                    {
                        flag2 = true;
                    }

                    if (flag3)
                    {
                        string[] c = line.Split('>');
                        setup.MaterialSetup = c[3].Split('&')[0];
                        flag3 = false;
                    }
                    if (line.Contains("МАТЕРИАЛ (ЛИСТ)"))
                    {
                        flag3 = true;
                    }

                    if (flag4)
                    {
                        if (line.Contains("."))
                        {
                            setup.SizeListSetup += " " + line.Trim();
                        }

                        if (line.Contains("nbsp"))
                        {
                            flag4 = false;
                        }
                    }
                    if (line.Contains("ЗАГОТОВКА") && (line.Contains("МИНИМАЛЬНАЯ") != true))
                    {
                        flag4 = true;
                    }

                    if (flag5 && line.Contains("nobr"))
                    {
                        string[] c = line.Split('>');
                        setup.TimeSetup = c[1].Split('[')[0];
                        flag5 = false;
                    }
                    if (line.Contains("МАШИННОЕ ВРЕМЯ"))
                    {
                        flag5 = true;
                    }

                    if (flag6)
                    {
                        string[] c = line.Split('>');
                        setup.NumberOfRunsSetup = c[2].Split('&')[0];
                        flag6 = false;
                    }
                    if (line.Contains("КОЛ-ВО ПРОГОНОВ ПРОГРАММЫ"))
                    {
                        flag6 = true;
                    }

                    if (flag7)
                    {
                        string[] c = line.Split('>');
                        setup.WastePercentageSetup = c[2].Split('&')[0];
                        flag7 = false;
                    }
                    if (line.Contains("ОТХОДЫ"))
                    {
                        flag7 = true;
                    }

                    string name;
                    string count;
                    if (line.Contains("ИМЯ ЛИСТА"))
                    {
                        break;
                    }
                    if (line.Contains("NOID"))
                    {
                        string[] a = line.Split('>');

                        if (a[11].Contains("GEO"))
                        {
                            name = new FileInfo(a[11].Split('&')[0]).Name;
                            count = a[15].Split('&')[0];
                            setup.DetailsSetup.Add(name, Convert.ToInt32(count) * Convert.ToInt32(setup?.NumberOfRunsSetup));
                        }
                    }
                }
            }
            return setup;
        }
    }
}
