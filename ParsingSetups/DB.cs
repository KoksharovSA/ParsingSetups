using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace ParsingSetups
{
    class DB
    {
        internal static bool CreateDBSetups(SQLiteConnection sQLiteConnection)
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
        internal static bool UpdateDBSetup(SQLiteConnection sQLiteConnection, Setup setup)
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

                using (SQLiteCommand command2 = sQLiteConnection.CreateCommand())
                {
                    command2.CommandText = "UPDATE Setups SET NameSetup = '" + setup.NameSetup + "', DirSetup = '" + setup.DirSetup + "', MaterialSetup = '" + setup.MaterialSetup + "', SizeListSetup = '" + setup.SizeListSetup + "', TimeSetup = '" + setup.TimeSetup + "', NumberOfRunsSetup = '" + setup.NumberOfRunsSetup + "', WastePercentageSetup = '" + setup.WastePercentageSetup + "', WasteSMSetup = '" + setup.WasteSMSetup + "', BusinessWasteSetup = '" + setup.BusinessWasteSetup + "', DateSpellingSetup = '" + setup.DateSpellingSetup + "', DateRunSetup = '" + setup.DateRunSetup + "', DetailsSetup = '" + lineDetails + "' WHERE NameSetup = '" + setup.NameSetup + "';";
                    //Thread.Sleep(1000);
                    command2.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                sQLiteConnection.Close();
                MessageBox.Show(ex.Message, "Ошибка");
                return false;
            }

            sQLiteConnection.Close();
            return true;
        }

        internal static bool UpdateDBSetup(SQLiteConnection sQLiteConnection, Collection<Setup> tempSetups)
        {
            sQLiteConnection.Open();
            foreach (var itemSetap in tempSetups)
            {
                try
                {
                    string lineDetails = "";
                    foreach (var item in itemSetap.DetailsSetup)
                    {
                        string a = item.Key;
                        string b = item.Value.ToString();
                        lineDetails += a + "&" + b + "|";
                    }
                    itemSetap.BusinessWasteSetup = itemSetap.BusinessWasteSetup == null ? "" : itemSetap.BusinessWasteSetup;
                    itemSetap.DateRunSetup = itemSetap.DateRunSetup == null ? "" : itemSetap.DateRunSetup;

                    using (SQLiteCommand command2 = sQLiteConnection.CreateCommand())
                    {
                        command2.CommandText = "UPDATE Setups SET NameSetup = '" + itemSetap.NameSetup + "', DirSetup = '" + itemSetap.DirSetup + "', MaterialSetup = '" + itemSetap.MaterialSetup + "', SizeListSetup = '" + itemSetap.SizeListSetup + "', TimeSetup = '" + itemSetap.TimeSetup + "', NumberOfRunsSetup = '" + itemSetap.NumberOfRunsSetup + "', WastePercentageSetup = '" + itemSetap.WastePercentageSetup + "', WasteSMSetup = '" + itemSetap.WasteSMSetup + "', BusinessWasteSetup = '" + itemSetap.BusinessWasteSetup + "', DateSpellingSetup = '" + itemSetap.DateSpellingSetup + "', DateRunSetup = '" + itemSetap.DateRunSetup + "', DetailsSetup = '" + lineDetails + "' WHERE NameSetup = '" + itemSetap.NameSetup + "';";
                        //Thread.Sleep(1000);
                        command2.ExecuteNonQuery();
                    }
                }
                catch (Exception ex)
                {
                    sQLiteConnection.Close();
                    MessageBox.Show(ex.Message, "Ошибка");
                    return false;
                }
            }
            sQLiteConnection.Close();
            return true;
        }

        internal static bool AddDBSetup(SQLiteConnection sQLiteConnection, Collection<Setup> tempSetups)
        {
            sQLiteConnection.Open();
            foreach (var itemSetap in tempSetups)
            {
                try
                {
                    string lineDetails = "";
                    foreach (var item in itemSetap.DetailsSetup)
                    {
                        string a = item.Key;
                        string b = item.Value.ToString();
                        lineDetails += a + "&" + b + "|";
                    }
                    itemSetap.BusinessWasteSetup = itemSetap.BusinessWasteSetup == null ? "" : itemSetap.BusinessWasteSetup;
                    itemSetap.DateRunSetup = itemSetap.DateRunSetup == null ? "" : itemSetap.DateRunSetup;
                    SQLiteDataReader sql;
                    using (SQLiteCommand command1 = sQLiteConnection.CreateCommand())
                    {
                        command1.CommandText = "SELECT * FROM Setups WHERE NOT EXISTS (SELECT * FROM Setups WHERE NameSetup = '" + itemSetap.NameSetup + "');";
                        sql = command1.ExecuteReader();
                    }
                    if (sql.Read())
                    {
                        using (SQLiteCommand command2 = sQLiteConnection.CreateCommand())
                        {
                            command2.CommandText = "INSERT INTO Setups(NameSetup, DirSetup, MaterialSetup, SizeListSetup, TimeSetup, NumberOfRunsSetup, WastePercentageSetup, WasteSMSetup, BusinessWasteSetup, DateSpellingSetup, DateRunSetup, DetailsSetup)VALUES('" + itemSetap.NameSetup + "', '" + itemSetap.DirSetup + "', '" + itemSetap.MaterialSetup + "', '" + itemSetap.SizeListSetup + "', '" + itemSetap.TimeSetup + "' , '" + itemSetap.NumberOfRunsSetup + "' , '" + itemSetap.WastePercentageSetup + "' , '" + itemSetap.WasteSMSetup + "', '" + itemSetap.BusinessWasteSetup + "', '" + itemSetap.DateSpellingSetup + "', '" + itemSetap.DateRunSetup + "', '" + lineDetails + "')";
                            command2.ExecuteNonQuery();
                        }
                    }
                    else
                    {
                        string message = "Данный план наладки уже существует, заменить его?";
                        string caption = "Ошибка " + itemSetap.NameSetup;
                        MessageBoxResult result;

                        result = MessageBox.Show(message, caption, MessageBoxButton.YesNo, MessageBoxImage.Question);

                        if (result == MessageBoxResult.Yes)
                        {
                            using (SQLiteCommand command2 = sQLiteConnection.CreateCommand())
                            {
                                command2.CommandText = "UPDATE Setups SET NameSetup = '" + itemSetap.NameSetup + "', DirSetup = '" + itemSetap.DirSetup + "', MaterialSetup = '" + itemSetap.MaterialSetup + "', SizeListSetup = '" + itemSetap.SizeListSetup + "', TimeSetup = '" + itemSetap.TimeSetup + "', NumberOfRunsSetup = '" + itemSetap.NumberOfRunsSetup + "', WastePercentageSetup = '" + itemSetap.WastePercentageSetup + "', WasteSMSetup = '" + itemSetap.WasteSMSetup + "', BusinessWasteSetup = '" + itemSetap.BusinessWasteSetup + "', DateSpellingSetup = '" + itemSetap.DateSpellingSetup + "', DateRunSetup = '" + itemSetap.DateRunSetup + "', DetailsSetup = '" + lineDetails + "' WHERE NameSetup = '" + itemSetap.NameSetup + "';";
                                //Thread.Sleep(1000);
                                command2.ExecuteNonQuery();
                            }

                        }
                    }
                }
                catch (Exception ex)
                {
                    sQLiteConnection.Close();
                    MessageBox.Show(ex.Message, "Ошибка");
                    return false;
                }
            }
            sQLiteConnection.Close();
            return true;
        }

        internal static Collection<Setup> ReadDBTools(string sQLiteConnection)
        {
            using (var connection = new SQLiteConnection(sQLiteConnection))
            {
                connection.Open();
                try
                {
                    Collection<Setup> setups = new Collection<Setup>();
                    SQLiteCommand command = connection.CreateCommand();
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
                        setup.BusinessWasteSetup = Convert.ToString(sql["BusinessWasteSetup"]) ?? "";
                        setup.DateSpellingSetup = Convert.ToString(sql["DateSpellingSetup"]) ?? "";
                        setup.DateRunSetup = Convert.ToString(sql["DateRunSetup"]) ?? "";
                        foreach (var item in Convert.ToString(sql["DetailsSetup"]).Split('|'))
                        {
                            if (item != "")
                            {
                                setup.DetailsSetup.Add(item.Split('&')?[0], Convert.ToInt32(item.Split('&')?[1]));
                            }
                        }
                        setups.Add(setup);
                    }
                    sql.Close();
                    connection.Close();
                    return setups;
                }
                catch (Exception ex)
                {
                    connection.Close();
                    MessageBox.Show(ex.Message, "Ошибка");
                    return null;
                }
            }

        }

    }
}
