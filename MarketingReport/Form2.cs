using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MarketingReport
{
    public partial class Form2 : Form
    {

        public  DataTable dt; 
        public Form2()
        {
            InitializeComponent();
           
        }

        private void Form2_Load(object sender, EventArgs e)
        {

        }

        //Use DataGrid to view the datatable that was passed from form 1
        //
        public DataTable dgv
        {
            
            set { dataGridView1.DataSource = value;
                dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            }
            
        }

        
        public TextBox title
        {
            set { textBox1 = value; }

            
        }

        // Create CSV File from the datatable.
        public static void csvFile(DataTable dataTable, string Title)
        {
            try
            {
                string name = Title;

                var lines = new List<string>();

                string[] columnNames = dataTable.Columns.Cast<DataColumn>().
                                                  Select(column => column.ColumnName).
                                                  ToArray();

                var header = string.Join(",", columnNames);
                lines.Add(header);

                var valueLines = dataTable.AsEnumerable()
                                   .Select(row => string.Join(",", row.ItemArray));
                lines.AddRange(valueLines);

                //Path for CSV File
                String folderPath = @"\\aafs1\I\MarketingReport\SavedCSVFiles\";
            
                
                
                File.WriteAllLines(folderPath + name + ".csv", lines);
                MessageBox.Show("File was succesfully saved to " + folderPath);
            }

            //Throw an exception if saving the CSV File failed. 
            catch (Exception e)
            {
                MessageBox.Show("Save to the I drive was unsuccesful"); 
            }
            
        }


        //Save CSV file, if this button is pressed
        public void button1_Click(object sender, EventArgs e)
        {
            string temp = textBox1.Text;
            DataTable tempTable;
            tempTable = dt;

            //Pass current datatable to CSV method
            csvFile(tempTable, temp);


        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}
