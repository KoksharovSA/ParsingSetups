﻿using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;


namespace ParsingSetups
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        IEnumerable<Setup> setups = new Collection<Setup>();
        IEnumerable<Detail> details = new Collection<Detail>();
        string QLiteConnection = Properties.Settings.Default.sQLiteConnectionString;
        public MainWindow()
        {
            InitializeComponent();
            //DB.CreateDBTools(QLiteConnection));
            setups = DB.ReadDBTools(QLiteConnection);
            details = DB.ReadDBDetails(QLiteConnection);
            LoadTreeView(setups, details);

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFile = new OpenFileDialog();
            openFile.Multiselect = true;
            openFile.Filter = "Setup files (*.html)|*.html";
            openFile.ShowDialog();
            Collection<Setup> tempSetups = new Collection<Setup>();
            foreach (var dir in openFile.FileNames)
            {
                var setup = ParseSetup(dir);
                if (setup != null)
                {
                    tempSetups.Add(setup);
                }
            }
            DB.AddDBSetup(new SQLiteConnection(QLiteConnection), tempSetups);
            setups = DB.ReadDBTools(QLiteConnection);
            LoadTreeView(setups, details);
        }

        internal bool LoadTreeView(IEnumerable<Setup> setups, IEnumerable<Detail> details)
        {
            TreeViewSetups.Items.Clear();
            foreach (var item in setups)
            {
                TreeViewSetups.Items.Add(new TextBlock() { Text = item.NameSetup });
            }            
            TreeViewDetails.Items.Clear();
            if (details !=null)
            {
                foreach (var item in details)
                {
                    TreeViewDetails.Items.Add(new TextBlock() { Text = item.NameDetail , Foreground = item.BendLength != "" ? Brushes.Black : Brushes.Red });
                }
            }

            return true;

        }

        internal Setup ParseSetup(string dir)
        {
            try
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
                    bool flag8 = false;//isPunch

                    bool flag9 = false;//Блок информация об отдельной детали
                    bool flag10 = false;//
                    bool flag11 = false;//

                    while ((line = sr.ReadLine()) != null)
                    {
                        if (line.Contains("TC2000")) { flag8 = true; }
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
                            if (flag8)
                            {
                                string[] c = line.Split('>');
                                setup.NameSetup = c[3].Split('&')[0];
                                flag2 = false;
                            }
                            else
                            {
                                string[] c = line.Split('>');
                                setup.NameSetup = c[1].Split('&')[0];
                                flag2 = false;
                            }
                            
                        }
                        if (line.Contains("ИМЯ ПРОГРАММЫ"))
                        {
                            flag2 = true;
                        }

                        if (flag3)
                        {
                            string[] c = line.Split('>');
                            if (c[3].Split('&')[0].Contains("St37"))
                            {
                                setup.MaterialSetup = Convert.ToString(Convert.ToDouble(c[3].Split('&')[0].Split('-')[1]) / 10) + " Ст3";
                            }
                            else
                            {
                                if (!setup.NameSetup.ToLower().Contains("ner"))
                                {
                                    if (Convert.ToDouble(c[3].Split('&')[0].Split('-')[1]) == 15)
                                    {
                                        setup.MaterialSetup = Convert.ToString(1.2) + "Оц";
                                    }
                                    else
                                    {
                                        setup.MaterialSetup = Convert.ToString(Convert.ToDouble(c[3].Split('&')[0].Split('-')[1]) / 10) + " Оц";
                                    }

                                }
                                else
                                {
                                    setup.MaterialSetup = Convert.ToString(Convert.ToDouble(c[3].Split('&')[0].Split('-')[1]) / 10) + " Нерж";
                                }
                            }
                            flag3 = false;
                        }
                        if (line.Contains("МАТЕРИАЛ (ЛИСТ)")|| line.Contains("ID МАТЕРИАЛА"))
                        {
                            flag3 = true;
                        }

                        if (flag4)
                        {
                            if (line.Contains(".") && !line.Contains("!") || (line.Contains(" ")&&!line.Contains("nbsp") && !line.Contains("size") && !line.Contains("!")))
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

                        if (flag5 && line.Contains("nobr")||(flag5 && line.Contains("nbsp") && !line.Contains("МАШИННОЕ ВРЕМЯ")))
                        {
                            if (flag8)
                            {
                                setup.TimeSetup = line.Split('[')[0].Trim();
                                flag5 = false;
                            }
                            else
                            {
                                string[] c = line.Split('>');
                                setup.TimeSetup = c[1].Split('[')[0].Trim();
                                flag5 = false;
                            }
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
                        
                        if (line.Contains("NOID"))
                        {
                            string[] a = line.Split('>');

                            if (a[11].Contains("GEO")|| a[11].Contains("GMT"))
                            {
                                name = new FileInfo(a[11].Split('&')[0]).Name;
                                count = a[15].Split('&')[0];
                                setup.DetailsSetup.Add(name, Convert.ToInt32(count) * Convert.ToInt32(setup?.NumberOfRunsSetup));
                            }
                        }


                        Detail detail = new Detail();
                        if (line.Contains("РАЗМЕРЫ"))
                        {
                            string[] a = line.Split('>');
                            string filename = new FileInfo(a.FirstOrDefault(x => x.ToLower().Contains("geo") || x.ToLower().Contains("gmt")).Split('&')[0]).Name;
                            detail.NameDetail = filename.Substring(0, filename.Length-4);
                            detail.SizesDetail = a.FirstOrDefault(x => x.Contains("x&nbsp")).Split('&')[0] + "x" + a.FirstOrDefault(x => x.Contains("x&nbsp")).Split('&')[2].Substring(5);
                            detail.SurfaceDetail = a.FirstOrDefault(x => x.Contains("mm2")).Split('&')[0];
                            detail.TimeOfProcessing = a.FirstOrDefault(x => x.Contains("min")).Split(' ')[0];
                            if (!flag8)
                            {
                                detail.CuttingLength = a.Where(x => x.Contains("mm&")).ToArray()[1].Split('&')[0];
                            }
                            detail.WeightDetail = a.FirstOrDefault(x => x.Contains("kg")).Split('&')[0];
                            setup.Details.Add(detail);

                        }
                        



                        if (line.Contains("Tafellayout"))
                        {
                            break;
                        }
                    }
                }
                return setup;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка");
                return null;
            }

        }

        private void TreeViewSetups_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (TreeViewSetups.SelectedItem != null)
            {
                GroupBoxSetup.Header = (TreeViewSetups.SelectedItem as TextBlock)?.Text;
                TBDirSetup.Text = setups.FirstOrDefault(x => x.NameSetup == (TreeViewSetups.SelectedItem as TextBlock)?.Text)?.DirSetup;
                TBMaterialSetup.Text = setups.FirstOrDefault(x => x.NameSetup == (TreeViewSetups.SelectedItem as TextBlock)?.Text)?.MaterialSetup;
                TBSizeSetup.Text = setups.FirstOrDefault(x => x.NameSetup == (TreeViewSetups.SelectedItem as TextBlock)?.Text)?.SizeListSetup.Trim();
                TBTimeSetup.Text = setups.FirstOrDefault(x => x.NameSetup == (TreeViewSetups.SelectedItem as TextBlock)?.Text)?.TimeSetup;
                TBNumberOfRunsSetup.Text = setups.FirstOrDefault(x => x.NameSetup == (TreeViewSetups.SelectedItem as TextBlock)?.Text)?.NumberOfRunsSetup.Trim();
                TBWastePercentageSetup.Text = setups.FirstOrDefault(x => x.NameSetup == (TreeViewSetups.SelectedItem as TextBlock)?.Text)?.WastePercentageSetup;
                TBWasteSMSetup.Text = setups.FirstOrDefault(x => x.NameSetup == (TreeViewSetups.SelectedItem as TextBlock)?.Text)?.WasteSMSetup + " см2";
                TBBusinessWasteSetup.Text = setups.FirstOrDefault(x => x.NameSetup == (TreeViewSetups.SelectedItem as TextBlock)?.Text)?.BusinessWasteSetup;
                DateSpellingSetup.Text = DateTime.Parse(setups.FirstOrDefault(x => x.NameSetup == (TreeViewSetups.SelectedItem as TextBlock)?.Text)?.DateSpellingSetup).ToShortDateString();
                if (setups.FirstOrDefault(x => x.NameSetup == (TreeViewSetups.SelectedItem as TextBlock).Text)?.DateRunSetup != "")
                {
                    DateRunSetupp.Text = DateTime.Parse(setups.FirstOrDefault(x => x.NameSetup == (TreeViewSetups.SelectedItem as TextBlock)?.Text).DateRunSetup).ToShortDateString();
                }
                DetailListBoxSetup.Items.Clear();
                foreach (var item in setups.FirstOrDefault(x => x.NameSetup == (TreeViewSetups.SelectedItem as TextBlock)?.Text).DetailsSetup)
                {
                    DetailListBoxSetup.Items.Add(new TextBlock() { TextWrapping = TextWrapping.Wrap, Text = item.Key + " (" + item.Value / Convert.ToInt32(setups.FirstOrDefault(x => x.NameSetup == (TreeViewSetups.SelectedItem as TextBlock).Text).NumberOfRunsSetup) + " / " + item.Value + ")" });
                }
            }
        }

        private void TBSearch_KeyUp(object sender, KeyEventArgs e)
        {
            if (TBSearch.Text != "" && TBSearch.Text != " " && TBSearch.Text != null)
            {
                LoadTreeView(setups.Where(x => x.NameSetup.ToLower().Contains(TBSearch.Text.ToLower())).OrderBy(x=>x.NameSetup), details);
            }
            else
            {
                LoadTreeView(setups, details);
            }
        }

        private void TreeViewSetups_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if ((TreeViewSetups.SelectedItem as TextBlock)?.Text!="" && TreeViewSetups.SelectedItem!=null)
            {
                StackPanel stackPanel = new StackPanel();
                stackPanel.Orientation = Orientation.Horizontal;
                TextBox textBox = new TextBox() { Text = setups.FirstOrDefault(x => x.NameSetup == (TreeViewSetups.SelectedItem as TextBlock)?.Text)?.NumberOfRunsSetup, Width = 30 };
                TextBlock textBlock = new TextBlock() { Text = (TreeViewSetups.SelectedItem as TextBlock)?.Text };
                stackPanel.Children.Add(textBox);
                stackPanel.Children.Add(textBlock);
                ContextMenu contextMenu = new ContextMenu();
                MenuItem menuItemDelit = new MenuItem();
                menuItemDelit.Header = "Удалить";
                menuItemDelit.Click += (object sender2, RoutedEventArgs e2) =>
                {
                    foreach (var item in AllDetailListBoxSetup.Items)
                    {
                        if (((item as StackPanel).Children[1] as TextBlock).Text == textBlock.Text)
                        {
                            AllDetailListBoxSetup.Items.Remove(item);
                            break;
                        }
                    }
                };
                MenuItem menuItemDelitAll = new MenuItem();
                menuItemDelitAll.Header = "Удалить всё";
                menuItemDelitAll.Click += (object sender2, RoutedEventArgs e2) =>
                {
                    AllDetailListBoxSetup.Items.Clear();
                };
                contextMenu.Items.Add(menuItemDelit);
                contextMenu.Items.Add(menuItemDelitAll);
                stackPanel.ContextMenu = contextMenu;
                bool flag = true;
                foreach (var item in AllDetailListBoxSetup.Items)
                {
                    if (((item as StackPanel).Children[1] as TextBlock).Text == (TreeViewSetups.SelectedItem as TextBlock)?.Text)
                    {
                        flag = false;
                    }
                }
                if (flag)
                {
                    AllDetailListBoxSetup.Items.Add(stackPanel);
                }
                else
                {
                    foreach (var item in AllDetailListBoxSetup.Items)
                    {
                        if (((item as StackPanel).Children[1] as TextBlock).Text == (TreeViewSetups.SelectedItem as TextBlock)?.Text)
                        {
                            ((item as StackPanel).Children[0] as TextBox).Text = Convert.ToString(Convert.ToInt32(((item as StackPanel).Children[0] as TextBox).Text) + Convert.ToInt32((stackPanel.Children[0] as TextBox).Text));
                        }
                    }
                }
            }


            
        }

        /// <summary>
        /// Кнопка формирования списка деталей
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            string result = CBFirstName.Text + "\n(" + DateMakingDetails.Text + ")\n";
            Dictionary<string, double> dictAllDetails = new Dictionary<string, double>();
            Dictionary<string, string[]> dictAllLists = new Dictionary<string, string[]>();
            foreach (var item1 in AllDetailListBoxSetup.Items)
            {
                string size = setups.FirstOrDefault(x => x.NameSetup == ((item1 as StackPanel).Children[1] as TextBlock).Text).SizeListSetup.Trim();
                string material = setups.FirstOrDefault(x => x.NameSetup == ((item1 as StackPanel).Children[1] as TextBlock).Text).MaterialSetup.Trim();
                size = size + " (" + material + ")";
                double waste = Convert.ToDouble(setups.FirstOrDefault(x => x.NameSetup == ((item1 as StackPanel).Children[1] as TextBlock).Text).WasteSMSetup) * Convert.ToDouble(((item1 as StackPanel).Children[0] as TextBox).Text);

                if (dictAllLists.ContainsKey(size) && dictAllLists[size][0] == material)
                {
                    dictAllLists[size][1] = (Convert.ToInt32(dictAllLists[size][1]) + Convert.ToInt32(((item1 as StackPanel).Children[0] as TextBox).Text)).ToString();
                    dictAllLists[size][2] = (Convert.ToInt32(dictAllLists[size][2]) + Convert.ToInt32(waste)).ToString();
                }
                else
                {
                    dictAllLists.Add(size, new string[3] { material, Convert.ToInt32(((item1 as StackPanel).Children[0] as TextBox).Text).ToString(), Convert.ToInt32(waste).ToString() });
                }
                foreach (var item2 in setups.FirstOrDefault(x => x.NameSetup == ((item1 as StackPanel).Children[1] as TextBlock).Text).DetailsSetup)
                {

                    if (dictAllDetails.ContainsKey(item2.Key))
                    {
                        dictAllDetails[item2.Key] = dictAllDetails[item2.Key] + (item2.Value / Convert.ToInt32(setups.FirstOrDefault(x => x.NameSetup == ((item1 as StackPanel).Children[1] as TextBlock).Text).NumberOfRunsSetup) * Convert.ToInt32(((item1 as StackPanel).Children[0] as TextBox).Text));
                    }
                    else
                    {
                        dictAllDetails.Add(item2.Key, item2.Value / Convert.ToInt32(setups.FirstOrDefault(x => x.NameSetup == ((item1 as StackPanel).Children[1] as TextBlock).Text).NumberOfRunsSetup) * Convert.ToInt32(((item1 as StackPanel).Children[0] as TextBox).Text));
                    }
                }
            }
            foreach (var item3 in dictAllDetails)
            {
                result += item3.Key + " - " + item3.Value + "шт.\n";
            }
            result += "\n";
            foreach (var item in dictAllLists)
            {
                result += item.Key + " - " + item.Value[1] + " шт. (отход " + item.Value[2] + " см2)\n";
            }
            Clipboard.SetText(result);
            if (DateMakingDetails.Text != "" && DateMakingDetails.Text != null)
            {
                Collection<Setup> colset = new Collection<Setup>();
                foreach (var item in AllDetailListBoxSetup.Items)
                {
                    var tempSetup = setups.FirstOrDefault(x => x.NameSetup == ((item as StackPanel).Children[1] as TextBlock).Text);
                    tempSetup.DateRunSetup = DateMakingDetails.Text;
                    colset.Add(tempSetup);
                }
                DB.UpdateDBSetup(new SQLiteConnection(QLiteConnection), colset);
            }
        }

        private void UpdateTask_Click(object sender, RoutedEventArgs e)
        {
            setups = DB.ReadDBTools(QLiteConnection);
            details = DB.ReadDBDetails(QLiteConnection);
            LoadTreeView(setups, details);
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Вы точно хотите выйти из программы?", "Окно для Ксении", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                this.Close();
            }
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            if (TBBusinessWasteSetup.Text != null)
            {
                var tempSetup = setups.FirstOrDefault(x => x.NameSetup == GroupBoxSetup.Header.ToString());
                tempSetup.BusinessWasteSetup = TBBusinessWasteSetup.Text;
                DB.UpdateDBSetup(new SQLiteConnection(QLiteConnection), tempSetup);
                setups = DB.ReadDBTools(QLiteConnection);
                LoadTreeView(setups, details);
            }
        }

        private void TreeViewDetails_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (TreeViewDetails.SelectedItem != null)
            {
                TBNameDetail.Text = details.FirstOrDefault(x => x.NameDetail == (TreeViewDetails.SelectedItem as TextBlock)?.Text)?.NameDetail;
                TBMaterialDetail.Text = details.FirstOrDefault(x => x.NameDetail == (TreeViewDetails.SelectedItem as TextBlock)?.Text)?.MaterialDetail;
                TBSizesDetail.Text = details.FirstOrDefault(x => x.NameDetail == (TreeViewDetails.SelectedItem as TextBlock)?.Text)?.SizesDetail;
                TBSurfaceDetail.Text = details.FirstOrDefault(x => x.NameDetail == (TreeViewDetails.SelectedItem as TextBlock)?.Text)?.SurfaceDetail;
                TBTimeOfProcessing.Text = details.FirstOrDefault(x => x.NameDetail == (TreeViewDetails.SelectedItem as TextBlock)?.Text)?.TimeOfProcessing;
                TBCuttingLength.Text = details.FirstOrDefault(x => x.NameDetail == (TreeViewDetails.SelectedItem as TextBlock)?.Text)?.CuttingLength;
                TBWeightDetail.Text = details.FirstOrDefault(x => x.NameDetail == (TreeViewDetails.SelectedItem as TextBlock)?.Text)?.WeightDetail;
                TBBendLength.Text = details.FirstOrDefault(x => x.NameDetail == (TreeViewDetails.SelectedItem as TextBlock)?.Text)?.BendLength;

            }
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            Detail detail = new Detail();
            detail.NameDetail = TBNameDetail.Text;
            detail.BendLength = TBBendLength.Text;
            DB.UpdateDBDetail(new SQLiteConnection(QLiteConnection), detail);
            details = DB.ReadDBDetails(QLiteConnection);
            LoadTreeView(setups, details);
        }

        private void UpdateDBD_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Excel files (*.xlsx;*.xls)|*.xlsx;*.xls";
            openFileDialog.Multiselect = true;
            openFileDialog.ShowDialog();
            foreach (var item in openFileDialog.FileNames)
            {
                var colDet = Excel.ExcelDataLoad(item,2);
                DB.AddDBDetails(new SQLiteConnection(QLiteConnection),colDet);
            }
        }

        private void MenuItem_Click_1(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(TBNameDetail.Text);
        }

        private void MenuItem_Click_2(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Excel files (*.xlsx;*.xls)|*.xlsx;*.xls";
            saveFileDialog.ShowDialog();
            Excel.ExcelDataWrite(details, saveFileDialog.FileName);
            var result = MessageBox.Show("Переместить элементы в основной список?", "Внимание", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                DB.DBExecuteNonQuery(QLiteConnection, "INSERT OR IGNORE INTO AllDetails(NameDetail, SizesDetail, SurfaceDetail, TimeOfProcessing, CuttingLength, WeightDetail, Material, BendLength) SELECT Details.NameDetail, Details.SizesDetail, Details.SurfaceDetail, Details.TimeOfProcessing, Details.CuttingLength, Details.WeightDetail, Details.Material, Details.BendLength FROM Details LEFT OUTER JOIN AllDetails ON Details.NameDetail = AllDetails.NameDetail WHERE AllDetails.NameDetail IS NULL");
                details = DB.ReadDBDetails(QLiteConnection);
                LoadTreeView(setups, details);
            }

        }

        private void ClickCreateTable(object sender, RoutedEventArgs e)
        {
            string employeeName = CBFirstName.Text;
            string dateRun = DateMakingDetails.Text;
            Dictionary<string, double> dictAllDetails = new Dictionary<string, double>();
            Dictionary<string, string[]> dictAllLists = new Dictionary<string, string[]>();
            foreach (var item1 in AllDetailListBoxSetup.Items)
            {
                string size = setups.FirstOrDefault(x => x.NameSetup == ((item1 as StackPanel).Children[1] as TextBlock).Text).SizeListSetup.Trim();
                string material = setups.FirstOrDefault(x => x.NameSetup == ((item1 as StackPanel).Children[1] as TextBlock).Text).MaterialSetup.Trim();
                size = size + " (" + material + ")";
                double waste = Convert.ToDouble(setups.FirstOrDefault(x => x.NameSetup == ((item1 as StackPanel).Children[1] as TextBlock).Text).WasteSMSetup) * Convert.ToDouble(((item1 as StackPanel).Children[0] as TextBox).Text);

                if (dictAllLists.ContainsKey(size) && dictAllLists[size][0] == material)
                {
                    dictAllLists[size][1] = (Convert.ToInt32(dictAllLists[size][1]) + Convert.ToInt32(((item1 as StackPanel).Children[0] as TextBox).Text)).ToString();
                    dictAllLists[size][2] = (Convert.ToInt32(dictAllLists[size][2]) + Convert.ToInt32(waste)).ToString();
                }
                else
                {
                    dictAllLists.Add(size, new string[3] { material, Convert.ToInt32(((item1 as StackPanel).Children[0] as TextBox).Text).ToString(), Convert.ToInt32(waste).ToString() });
                }
                foreach (var item2 in setups.FirstOrDefault(x => x.NameSetup == ((item1 as StackPanel).Children[1] as TextBlock).Text).DetailsSetup)
                {
                    if (dictAllDetails.ContainsKey(item2.Key))
                    {
                        dictAllDetails[item2.Key] = dictAllDetails[item2.Key] + (item2.Value / Convert.ToInt32(setups.FirstOrDefault(x => x.NameSetup == ((item1 as StackPanel).Children[1] as TextBlock).Text).NumberOfRunsSetup) * Convert.ToInt32(((item1 as StackPanel).Children[0] as TextBox).Text));
                    }
                    else
                    {
                        dictAllDetails.Add(item2.Key, item2.Value / Convert.ToInt32(setups.FirstOrDefault(x => x.NameSetup == ((item1 as StackPanel).Children[1] as TextBlock).Text).NumberOfRunsSetup) * Convert.ToInt32(((item1 as StackPanel).Children[0] as TextBox).Text));
                    }
                }
            }

            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Excel files (*.xlsx;*.xls)|*.xlsx;*.xls";
            saveFileDialog.FileName = employeeName + "_" + dateRun;
            saveFileDialog.ShowDialog();
            Excel.ExcelResultWrite(dictAllDetails, dictAllLists, employeeName, dateRun, saveFileDialog.FileName);

            if (DateMakingDetails.Text != "" && DateMakingDetails.Text != null)
            {
                Collection<Setup> colset = new Collection<Setup>();
                foreach (var item in AllDetailListBoxSetup.Items)
                {
                    var tempSetup = setups.FirstOrDefault(x => x.NameSetup == ((item as StackPanel).Children[1] as TextBlock).Text);
                    tempSetup.DateRunSetup = DateMakingDetails.Text;
                    colset.Add(tempSetup);
                }
                DB.UpdateDBSetup(new SQLiteConnection(QLiteConnection), colset);
            }
        }
    }
}
