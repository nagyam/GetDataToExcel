using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using ClosedXML.Excel;
using System.Text.RegularExpressions;

namespace GetDataToExcel
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            this.Text = Program.ablaknev;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DataTable tab = dataTable(StartTime(), StopTime());
            if (tab != null)
            {
                if (tab.Rows.Count > 0)
                {


                    saveFileDialog1.Title = $"Hová mentsük a lekérdezés ({tab.Rows.Count} sor) eredményét?";
                    saveFileDialog1.Filter = "Excel fájl|*.xlsx";
                    saveFileDialog1.FileName = "Lekérdezés";
                    DataTable st2   = ReplaceColumnNames(tab);
                    var dialogResult = saveFileDialog1.ShowDialog();
                    if (dialogResult == DialogResult.OK)
                    {
                        SaveDataTableToExcel(st2, saveFileDialog1.FileName);
                        var ans = MessageBox.Show("A lekérdezés eredménye sikeresen elmentve az Excel fájlba.\nMegnyitja most a fájlt?", "Siker", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                        {
                            if (ans == DialogResult.Yes)
                            {
                                System.Diagnostics.Process.Start(saveFileDialog1.FileName);
                            }
                        }
                    }
                    
                }
            }
        }

        private DataTable dataTable(DateTime startido, DateTime stopido)
        {
            DataTable _dataTable = new DataTable();
            // Open the database connection
            using (SqlConnection connection = new SqlConnection(Program.conn))
            {
                // Create a new SqlCommand with the query and connection
                using (SqlCommand command = new SqlCommand(Program.comm, connection))
                {
                    // Add the parameters to the command
                    command.Parameters.AddWithValue("@startido", startido);
                    command.Parameters.AddWithValue("@stopido", stopido);

                    
                    // Open the connection
                    connection.Open();

                    // Execute the query and fill the DataTable with the result
                    using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                    {
                        adapter.Fill(_dataTable);
                    }
                    
                }
                return _dataTable;
            }
        }


        private DateTime StartTime()
        {
            string date = dateTimePicker1.Value.ToString("yyyy.MM.dd");
            string time = maskedTextBox1.Text;
            return DateTime.Parse(date + " " + time);
        }
        private DateTime StopTime()
        {
            string date = dateTimePicker2.Value.ToString("yyyy.MM.dd");
            string time = maskedTextBox2.Text;
            return DateTime.Parse(date + " " + time);
        }
        private void SaveDataTableToExcel(DataTable table, string filepath)
        {
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Sheet1");
                

                // Add the DataTable to the worksheet starting from cell A1
                worksheet.Cell(1, 1).InsertTable(table);

                // Format the header row
                var headerRow = worksheet.Row(1);
                headerRow.Style.Font.Bold = true;
                headerRow.Style.Fill.BackgroundColor = XLColor.DimGray;
                headerRow.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                // Format the data rows
                var dataRange = worksheet.RangeUsed().RowsUsed().Skip(1); // Skip header row
                int rowIndex = 1;
                foreach (var row in dataRange)
                {
                    row.Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    row.Style.Border.BottomBorderColor = XLColor.DimGray;

                    // Alternate row colors
                    if (rowIndex % 2 == 0)
                    {
                        row.Style.Fill.BackgroundColor = XLColor.WhiteSmoke;
                    }
                    else
                    {
                        row.Style.Fill.BackgroundColor = XLColor.LightGray;
                    }
                    rowIndex++;
                }

                // Adjust column widths to fit the content
                worksheet.Columns().AdjustToContents();

                // Save the workbook to the specified file path
                workbook.SaveAs(filepath);
            }
        }
        private DataTable ReplaceColumnNames(DataTable dataTable)
        {
            // Extract column names from the comm file
            string[] columnNames = ExtractColumnNamesFromComm();

            // Replace column names in the DataTable
            for (int i = 0; i < dataTable.Columns.Count; i++)
            {
                if (i < columnNames.Length)
                {
                    dataTable.Columns[i].ColumnName = columnNames[i];
                }
            }

            return dataTable;
        }

        private string[] ExtractColumnNamesFromComm()
        {
            // Read the comm file content
            string commContent = System.IO.File.ReadAllText("comm", Encoding.UTF8);

            // Extract column names using regex
            var matches = Regex.Matches(commContent, @"as N'([^']+)'");
            return matches.Cast<Match>().Select(m => m.Groups[1].Value).ToArray();
        }
    }
}
