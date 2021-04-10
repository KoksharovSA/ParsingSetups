using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ParsingSetups
{
    class DB
    {
        internal static bool CreateDBTools(SQLiteConnection sQLiteConnection)
        {
            sQLiteConnection.Open();
            try
            {
                SQLiteCommand command = sQLiteConnection.CreateCommand();
                command.CommandText = "CREATE TABLE Setups(NumberSetup INTEGER NOT NULL UNIQUE, NameSetup TEXT NOT NULL, DirSetup TEXT, MaterialSetup TEXT, SizeListSetup TEXT, TimeSetup TEXT, NumberOfRunsSetup TEXT, WastePercentageSetup TEXT, WasteSMSetup TEXT, BusinessWasteSetup TEXT, DateSpellingSetup TEXT, DateRunSetup TEXT, DetailsSetup TEXT(3000), PRIMARY KEY(NumberSetup AUTOINCREMENT));";
                command.ExecuteNonQuery();
                sQLiteConnection.Close();
                return true;
            }
            catch (Exception ex)
            {
                sQLiteConnection.Close();
                MessageBox.Show(ex.Message, "Ошибка");
                return false;
            }
        }

        internal static bool AddDBSetup(SQLiteConnection sQLiteConnection, Setup setup)
        {
            sQLiteConnection.Open();
            try
            {
                string lineDetails = "";
                foreach (var item in setup.DetailsSetup)
                {
                    string a = item.Key;
                    string b = item.Value.ToString();
                    lineDetails += a + "&" + b + "|";
                }
                setup.BusinessWasteSetup = setup.BusinessWasteSetup == null ? "" : setup.BusinessWasteSetup;
                setup.DateRunSetup = setup.DateRunSetup == null ? "" : setup.DateRunSetup;
                SQLiteCommand command1 = sQLiteConnection.CreateCommand();
                SQLiteCommand command2 = sQLiteConnection.CreateCommand();
                SQLiteCommand command3 = sQLiteConnection.CreateCommand();
                command1.CommandText = "SELECT * FROM Setups WHERE NameSetup = '" + setup.NameSetup + "';";
                var sql = command1.ExecuteReader().StepCount;
                if (sql == 0)
                {
                    command2.CommandText = "INSERT OR IGNORE INTO Setups(NameSetup, DirSetup, MaterialSetup, SizeListSetup, TimeSetup, NumberOfRunsSetup, WastePercentageSetup, WasteSMSetup, BusinessWasteSetup, DateSpellingSetup, DateRunSetup, DetailsSetup)VALUES('" + setup.NameSetup + "', '" + setup.DirSetup + "', '" + setup.MaterialSetup + "', '" + setup.SizeListSetup + "', '" + setup.TimeSetup + "' , '" + setup.NumberOfRunsSetup + "' , '" + setup.WastePercentageSetup + "' , '" + setup.WasteSMSetup + "', '" + setup.BusinessWasteSetup + "', '" + setup.DateSpellingSetup + "', '" + setup.DateRunSetup + "', '" + lineDetails + "')";
                    command2.ExecuteNonQuery();
                }
                else
                {
                    string message = "Даннsq план наладки уже существует, заменить его?";
                    string caption = "Ошибка";
                    MessageBoxResult result;

                    result = MessageBox.Show(message, caption, MessageBoxButton.YesNo, MessageBoxImage.Question);

                    if (result == MessageBoxResult.Yes)
                    {
                        command3.CommandText = "DELETE FROM Setups WHERE NameSetup = '" + setup.NameSetup + "';";
                        command3.ExecuteNonQuery();
                        command2.CommandText = "INSERT OR IGNORE INTO Setups(NameSetup, DirSetup, MaterialSetup, SizeListSetup, TimeSetup, NumberOfRunsSetup, WastePercentageSetup, WasteSMSetup, BusinessWasteSetup, DateSpellingSetup, DateRunSetup, DetailsSetup)VALUES('" + setup.NameSetup + "', '" + setup.DirSetup + "', '" + setup.MaterialSetup + "', '" + setup.SizeListSetup + "', '" + setup.TimeSetup + "' , '" + setup.NumberOfRunsSetup + "' , '" + setup.WastePercentageSetup + "' , '" + setup.WasteSMSetup + "', '" + setup.BusinessWasteSetup + "', '" + setup.DateSpellingSetup + "', '" + setup.DateRunSetup + "', '" + lineDetails + "')";
                        command2.ExecuteNonQuery();
                    }
                }
                sQLiteConnection.Close();
                return true;
            }
            catch (Exception ex)
            {
                sQLiteConnection.Close();
                MessageBox.Show(ex.Message, "Ошибка");
                return false;
            }
        }

        internal static Collection<Setup> ReadDBTools(SQLiteConnection sQLiteConnection)
        {
            sQLiteConnection.Open();
            try
            {
                Collection<Setup> setups = new Collection<Setup>();
                SQLiteCommand command = sQLiteConnection.CreateCommand();
                command.CommandText = "SELECT * FROM Setups";
                SQLiteDataReader sql = command.ExecuteReader();
                while (sql.Read())
                {
                    Setup setup = new Setup();
                    setup.NameSetup = Convert.ToString(sql["NameSetup"]) ?? "";
                    setup.DirSetup = Convert.ToString(sql["DirSetup"]) ?? "";
                    setup.MaterialSetup = Convert.ToString(sql["MaterialSetup"]) ?? "";
                    setup.SizeListSetup = Convert.ToString(sql["SizeListSetup"]) ?? "";
                    setup.TimeSetup = Convert.ToString(sql["TimeSetup"]) ?? "";
                    setup.NumberOfRunsSetup = Convert.ToString(sql["NumberOfRunsSetup"]) ?? "";
                    setup.WastePercentageSetup = Convert.ToString(sql["WastePercentageSetup"]) ?? "";
                    setup.DateSpellingSetup = Convert.ToString(sql["DateSpellingSetup"]) ?? "";
                    setup.DateRunSetup = Convert.ToString(sql["DateRunSetup"]) ?? "";
                    foreach (var item in Convert.ToString(sql["DetailsSetup"]).Split('|'))
                    {
                        if (item!="")
                        {
                            setup.DetailsSetup.Add(item.Split('&')?[0], Convert.ToInt32(item.Split('&')?[1]));
                        }
                    }
                    setups.Add(setup);
                }
                sql.Close();
                sQLiteConnection.Close();
                return setups;
            }
            catch (Exception ex)
            {
                sQLiteConnection.Close();
                MessageBox.Show(ex.Message, "Ошибка");
                return null;
            }
        }

    }
}
