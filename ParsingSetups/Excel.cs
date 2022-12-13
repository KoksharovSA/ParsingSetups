using System;
using Microsoft.Office.Interop.Excel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Windows;
using System.IO;

namespace ParsingSetups
{
    class Excel
    {
        public static Collection<Detail> ExcelDataLoad(string dir, int startRow)
        {

            Collection<Detail> details = new Collection<Detail>();

            Microsoft.Office.Interop.Excel.Application excel = new Microsoft.Office.Interop.Excel.Application();
            try
            {
                Workbook wb = excel.Workbooks.Add(Type.Missing);
                Worksheet excelSheet = (Microsoft.Office.Interop.Excel.Worksheet)excel.ActiveSheet;

                int currentRow = startRow;
                bool isNotEmptyField = true;
                while (isNotEmptyField)
                {
                    var b = excelSheet.Cells[currentRow, 1].Value;

                    if (b != null
                        && Convert.ToString(b) != ""
                        && Convert.ToString(b) != " ")
                    {
                        Detail detail = new Detail();
                        if (Convert.ToString((excelSheet.Cells[currentRow, 1] as Microsoft.Office.Interop.Excel.Range)?.Value) != null)
                        {
                            detail.NameDetail = Convert.ToString((excelSheet.Cells[currentRow, 1] as Microsoft.Office.Interop.Excel.Range)?.Value).Trim();
                        }
                        if (Convert.ToString((excelSheet.Cells[currentRow, 2] as Microsoft.Office.Interop.Excel.Range)?.Value) != null)
                        {
                            detail.MaterialDetail = Convert.ToString((excelSheet.Cells[currentRow, 2] as Microsoft.Office.Interop.Excel.Range)?.Value).Trim();
                        }
                        if (Convert.ToString((excelSheet.Cells[currentRow, 3] as Microsoft.Office.Interop.Excel.Range)?.Value) != null && Convert.ToString((excelSheet.Cells[currentRow, 4] as Microsoft.Office.Interop.Excel.Range)?.Value) != null)
                        {
                            detail.SizesDetail = Convert.ToString((excelSheet.Cells[currentRow, 3] as Microsoft.Office.Interop.Excel.Range)?.Value).Trim() + "х" + Convert.ToString((excelSheet.Cells[currentRow, 4] as Microsoft.Office.Interop.Excel.Range)?.Value).Trim();
                        }
                        if (Convert.ToString((excelSheet.Cells[currentRow, 5] as Microsoft.Office.Interop.Excel.Range)?.Value) != null)
                        {
                            detail.SurfaceDetail = Convert.ToString((excelSheet.Cells[currentRow, 5] as Microsoft.Office.Interop.Excel.Range)?.Value).Trim();
                        }
                        if (Convert.ToString((excelSheet.Cells[currentRow, 6] as Microsoft.Office.Interop.Excel.Range)?.Value) != null)
                        {
                            detail.TimeOfProcessing = Convert.ToString((excelSheet.Cells[currentRow, 6] as Microsoft.Office.Interop.Excel.Range)?.Value).Trim();
                        }
                        if (Convert.ToString((excelSheet.Cells[currentRow, 7] as Microsoft.Office.Interop.Excel.Range)?.Value) != null)
                        {
                            detail.BendLength = Convert.ToString((excelSheet.Cells[currentRow, 7] as Microsoft.Office.Interop.Excel.Range)?.Value).Trim();
                        }
                        details.Add(detail);
                        currentRow += 1;
                    }
                    else
                    {
                        isNotEmptyField = false;
                    }
                }
                wb.Close(0);
                excel.Quit();
                return details;
            }
            catch (Exception ex)
            {
                excel.Quit();
                MessageBox.Show(Convert.ToString(ex));
                return details;
            }
        }

        public static void ExcelDataWrite(IEnumerable<Detail> details, string dir)
        {
            Microsoft.Office.Interop.Excel.Application excel = new Microsoft.Office.Interop.Excel.Application();
            try
            {
                Workbook wb = excel.Workbooks.Add(Type.Missing);
                wb.SaveAs(dir);

                wb = excel.Workbooks.Open(dir);
                Worksheet excelSheet = (Microsoft.Office.Interop.Excel.Worksheet)excel.ActiveSheet;
                excel.Columns.WrapText = true;
                excelSheet.Cells[1, 1] = "Наименование";
                excelSheet.Cells[1, 1].EntireRow.Font.Bold = true;
                excelSheet.Cells[1, 2] = "Длина, мм";
                excelSheet.Cells[1, 3] = "Ширина, мм";
                excelSheet.Cells[1, 4] = "Площадь поверхности, мм2";
                excelSheet.Cells[1, 5] = "Площадь поверхности, cм2";
                excelSheet.Cells[1, 6] = "Время обработки, мин";
                excelSheet.Cells[1, 7] = "Длина реза, мм";
                excelSheet.Cells[1, 8] = "Вес детали, кг";
                excelSheet.Cells[1, 9] = "Материал";
                excelSheet.Cells[1, 10] = "Длина гибов, мм";

                excel.Columns[1].ColumnWidth = 40;
                excel.Columns[2].ColumnWidth = 15;
                excel.Columns[3].ColumnWidth = 15;
                excel.Columns[4].ColumnWidth = 15;
                excel.Columns[5].ColumnWidth = 15;
                excel.Columns[6].ColumnWidth = 15;
                excel.Columns[7].ColumnWidth = 15;
                excel.Columns[8].ColumnWidth = 15;
                excel.Columns[9].ColumnWidth = 15;
                excel.Columns[10].ColumnWidth = 15;

                excel.Columns.VerticalAlignment = XlHAlign.xlHAlignCenter;
                excel.Cells[1, 1].HorizontalAlignment = XlHAlign.xlHAlignCenter;
                excel.Columns[2].HorizontalAlignment = XlHAlign.xlHAlignCenter;
                excel.Columns[3].HorizontalAlignment = XlHAlign.xlHAlignCenter;
                excel.Columns[4].HorizontalAlignment = XlHAlign.xlHAlignCenter;
                excel.Columns[5].HorizontalAlignment = XlHAlign.xlHAlignCenter;
                excel.Columns[6].HorizontalAlignment = XlHAlign.xlHAlignCenter;
                excel.Columns[7].HorizontalAlignment = XlHAlign.xlHAlignCenter;
                excel.Columns[8].HorizontalAlignment = XlHAlign.xlHAlignCenter;
                excel.Columns[9].HorizontalAlignment = XlHAlign.xlHAlignCenter;
                excel.Columns[10].HorizontalAlignment = XlHAlign.xlHAlignCenter;

                bool isEmptyField = true;
                int currentRow = 1;
                while (isEmptyField)
                {
                    var b = excelSheet.Cells[currentRow, 1].Value;

                    if (b == null
                        || Convert.ToString(b) == ""
                        || Convert.ToString(b) == " ")
                    {
                        foreach (var item in details)
                        {
                            excel.Cells[currentRow, 1].EntireColumn.NumberFormat = "@";
                            excel.Cells[currentRow, 2].EntireColumn.NumberFormat = "@";
                            excel.Cells[currentRow, 3].EntireColumn.NumberFormat = "@";
                            excel.Cells[currentRow, 4].EntireColumn.NumberFormat = "@";
                            excel.Cells[currentRow, 5].EntireColumn.NumberFormat = "@";
                            excel.Cells[currentRow, 6].EntireColumn.NumberFormat = "@";
                            excel.Cells[currentRow, 7].EntireColumn.NumberFormat = "@";
                            excel.Cells[currentRow, 8].EntireColumn.NumberFormat = "@";
                            excel.Cells[currentRow, 9].EntireColumn.NumberFormat = "@";
                            excel.Cells[currentRow, 10].EntireColumn.NumberFormat = "@";

                            excelSheet.Cells[currentRow, 1] = item.NameDetail;
                            excelSheet.Cells[currentRow, 2] = item.SizesDetail != "" && item.SizesDetail != " " && item.SizesDetail != null ? item.SizesDetail?.Split('x')[0] : "0";
                            excelSheet.Cells[currentRow, 3] = item.SizesDetail != "" && item.SizesDetail != " " && item.SizesDetail != null ? item.SizesDetail?.Split('x')[1] : "0";
                            excelSheet.Cells[currentRow, 4] = item.SurfaceDetail?.Replace('.', ',').ToString();
                            excelSheet.Cells[currentRow, 5] = Convert.ToString(Convert.ToDouble(item.SurfaceDetail?.Replace('.', ',')) / 100);
                            excelSheet.Cells[currentRow, 6] = item.TimeOfProcessing?.Replace('.', ',');
                            excelSheet.Cells[currentRow, 7] = item.CuttingLength?.Replace('.', ',');
                            excelSheet.Cells[currentRow, 8] = item.WeightDetail?.Replace('.', ',');
                            excelSheet.Cells[currentRow, 9] = item.MaterialDetail;
                            excelSheet.Cells[currentRow, 10] = item.BendLength?.Replace('.', ',');
                            currentRow += 1;
                        }
                        isEmptyField = false;
                    }
                    else
                    {
                        currentRow += 1;
                    }
                }
                wb.Save();
                wb.Close(0);
                excel.Quit();
            }
            catch (Exception ex)
            {

                excel.Quit();
                MessageBox.Show(Convert.ToString(ex));
            }
        }

        public static void ExcelResultWrite(Dictionary<string, double> details, Dictionary<string, string[]> lists, string employyName, string dateRun, string dir)
        {
            Microsoft.Office.Interop.Excel.Application excel = new Microsoft.Office.Interop.Excel.Application();
            try
            {
                Workbook wb = excel.Workbooks.Add(Type.Missing);
                wb.SaveAs(dir);

                wb = excel.Workbooks.Open(dir);
                Worksheet excelSheet = (Microsoft.Office.Interop.Excel.Worksheet)excel.ActiveSheet;
                excel.Columns.WrapText = true;
                excelSheet.Cells[1, 1] = employyName;
                excelSheet.Cells[1, 2] = dateRun;
                excelSheet.Cells[2, 1] = "Наименование";
                excelSheet.Cells[1, 1].EntireRow.Font.Bold = true;
                excelSheet.Cells[2, 1].EntireRow.Font.Bold = true;
                excelSheet.Cells[2, 2] = "Количество";

                excel.Columns[1].ColumnWidth = 40;
                excel.Columns[2].ColumnWidth = 15;
                excel.Columns[3].ColumnWidth = 15;

                excel.Columns.VerticalAlignment = XlHAlign.xlHAlignCenter;
                excel.Cells[1, 1].HorizontalAlignment = XlHAlign.xlHAlignCenter;
                excel.Columns[2].HorizontalAlignment = XlHAlign.xlHAlignCenter;
                excel.Columns[3].HorizontalAlignment = XlHAlign.xlHAlignCenter;

                bool isEmptyField = true;
                int currentRow = 2;
                while (isEmptyField)
                {
                    var b = excelSheet.Cells[currentRow, 1].Value;

                    if (b == null
                        || Convert.ToString(b) == ""
                        || Convert.ToString(b) == " ")
                    {
                        foreach (var detailItem in details)
                        {
                            excel.Cells[currentRow, 1].EntireColumn.NumberFormat = "@";
                            excel.Cells[currentRow, 2].EntireColumn.NumberFormat = "@";
                            if (detailItem.Key.ToLower().Contains("geo") || detailItem.Key.ToLower().Contains("gmt"))
                            {
                                excelSheet.Cells[currentRow, 1] = detailItem.Key.Substring(0, detailItem.Key.Length - 4);
                            }
                            else
                            {
                                excelSheet.Cells[currentRow, 1] = detailItem.Key;
                            }
                            excelSheet.Cells[currentRow, 2] = detailItem.Value;

                            currentRow += 1;
                        }
                        currentRow += 1;
                        excelSheet.Cells[currentRow, 1] = "Материал";
                        excelSheet.Cells[currentRow, 1].EntireRow.Font.Bold = true;
                        excelSheet.Cells[currentRow, 2] = "Количество";
                        excelSheet.Cells[currentRow, 3] = "Отход(см2)";
                        currentRow += 1;
                        foreach (var materialItem in lists)
                        {
                            excel.Cells[currentRow, 1].EntireColumn.NumberFormat = "@";
                            excel.Cells[currentRow, 2].EntireColumn.NumberFormat = "@";
                            excel.Cells[currentRow, 3].EntireColumn.NumberFormat = "@";
                            excelSheet.Cells[currentRow, 1] = materialItem.Key;
                            excelSheet.Cells[currentRow, 2] = materialItem.Value[1];
                            excelSheet.Cells[currentRow, 3] = materialItem.Value[2];
                            currentRow += 1;
                        }
                        isEmptyField = false;
                    }
                    else
                    {
                        currentRow += 1;
                    }
                }
                wb.Save();
                wb.Close(0);
                excel.Quit();
            }
            catch (Exception ex)
            {
                excel.Quit();
                MessageBox.Show(Convert.ToString(ex));
            }
        }
    }

}

