//Code written by Raul Sanchez. 
//2/6/2019

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Data.SqlClient;

namespace MarketingReport
{
    public partial class Form1 : Form
    {
        //For the DataGridView in Form 2
        public DataGridView DGV { get; set; }
        private void Form1_Load(object sender, EventArgs e)
        {



        }
        public Form1()
        {

            try
            {
                // Fill Both Blog and People drop downs. 
                InitializeComponent();
                Fillcombo();
                FillBlog();
               
            }

            //Throw an exception in case the application fails to upload. 
            catch (Exception e)
            {
                MessageBox.Show("You do not have permissions to use this application. Please contact an Administrator for help");
            }
        }
        void Fillcombo()
        {

            //Fill the dropbox with names of people who viewed the blogs.
            //Connect to DataBase
            SqlConnection conn = new SqlConnection(@"Insert Connection String Here");

            conn.Open();
            //insert email as name if Person does not have a first and last name. 
            string query = "Select DISTINCT ISNULL (LastName + ', ' + FirstName, email) as FullName FROM dbo.Users ";
            SqlCommand cmd = new SqlCommand(query, conn);
            SqlDataReader reader;
            DataTable dt = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(dt);



            //Add each name to the dropbox
            foreach (DataRow dr in dt.Rows)
            {
                peopleComboBox.Items.Add(dr["FullName"]).ToString();

            }

            conn.Close();
        }

        void FillBlog()
        {
            //Fill drop box with blog titles. 
           
            SqlConnection conn = new SqlConnection(@"Insert Connection String Here");
            conn.Open();
            string query = "Select Title + ' ' + InteractionType AS Name FROM dbo.TitleOfBlog ";
           
            SqlCommand cmd = new SqlCommand(query, conn);
            SqlDataReader reader;
            DataTable dt = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(dt);



            //Add each blog to the dropbox 
            foreach (DataRow dr in dt.Rows)
            {
                blogDropDown.Items.Add(dr["Name"]).ToString();

            }

            conn.Close();

        }

        



       



        //Button
        private void peopleButton_SelectedIndexChanged(object sender, EventArgs e)
        {


        }

        //Button
        private void peopleQuery(object sender, EventArgs e)
        {
            
        }

        private void peopleSearch_Click(object sender, EventArgs e)
        {
            //Set up Connection 
           
            SqlConnection conn = new SqlConnection(@"Insert Connection String Here");
          
            conn.Open();

            if (peopleComboBox.SelectedItem != null)
            {

                string temp = peopleComboBox.SelectedItem.ToString();

                //For this condition, the string must be a name
                //Fires 3 queries, one to get total views, one for the email, one for the count of blog interactions. 
                if (!temp.Contains('@'))
                {
                    string query = "Select tab.Title, tab.Category, tab.InteractionType, c.Count, (SElECT sum(Count) from count c where c.UserId = u.userID) AS \"Total Views\" from Users u Join Count c on c.UserId = u.UserId" +
                                        " Join TitleOfBlog tab on tab.BlogNumber = c.BlogId where LastName + ', ' + FirstName = " + " '" + temp + "'";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    String TotalQuery = "select SUM(Count) from Count c join dbo.Users u on u.UserId = c.UserID where u.LastName + ', ' + u.FirstName = " + " '" + temp + "'";
                    SqlCommand cmd2 = new SqlCommand(TotalQuery, conn);

                    string emailQuery = "select email from dbo.Users u where u.LastName + ', ' + u.FirstName = " + " '" + temp + "'";
                    SqlCommand cmd3 = new SqlCommand(emailQuery, conn);
                    Int32 count = (Int32)cmd2.ExecuteScalar();
                    DataTable dt = new DataTable();
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(dt);
                    SqlDataAdapter da2 = new SqlDataAdapter(cmd3);
                    string email = (string)cmd3.ExecuteScalar();
                    
                    
                    //Set values for Form 2
                    
                    Form2 fm = new Form2();
                    fm.totalViews.Text = count.ToString();
                    fm.textBox1.Text = temp;
                    fm.textBox3.Text = email;
                    fm.dgv = dt;
                    fm.dt = dt;
                    fm.Show();


                    //close connection 
                    conn.Close();
                }

                //Takes care of the condition, if the name was not available, but the email was for an individual
                else if (temp.Contains('@'))
                {

                    string query = "Select tab.Title, tab.Category, tab.InteractionType, c.Count, (SElECT sum(Count) from count c where c.UserId = u.userID) AS \"Total Views\" from Users u Join Count c on c.UserId = u.UserId" +
                                        " Join TitleOfBlog tab on tab.BlogNumber = c.BlogId where Email = " + " '" + temp + "'";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    String TotalQuery = "select SUM(Count) from Count c join dbo.Users u on u.UserId = c.UserID where u.Email = " + " '" + temp + "'";
                    SqlCommand cmd2 = new SqlCommand(TotalQuery, conn);
                    Int32 count = (Int32)cmd2.ExecuteScalar();
                    DataTable dt = new DataTable();
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(dt);

                    //Set values for Form 2
                    //Set the datagrid view to the datatable. 
                    Form2 fm = new Form2();
                    fm.totalViews.Text = count.ToString();
                    fm.textBox1.Text = temp;
                    fm.dgv = dt;
                    fm.dt = dt;
                    fm.Show();


                    //close connection 
                    conn.Close();
                }
            }

            //Ensures a selection from the dropbox. 
            else
            {
                MessageBox.Show("Please select a person you want a report on"); 
            }
        }

        private void peopleComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            
        }




        private void blogQuery_Click(object sender, EventArgs e)
        {
            SqlConnection conn = new SqlConnection(@"Insert Connection String Here");

            conn.Open();
            string temp = null;
           
            //Make sure both blog and blog order is selected. 
            if (blogDropDown.SelectedItem != null && IndividulOrder.SelectedItem != null)
            {

                temp = blogDropDown.SelectedItem.ToString();

                string choice = IndividulOrder.SelectedItem.ToString();
                temp = blogDropDown.SelectedItem.ToString();

                //Report in order of last name.
                if (choice == "Last Name:")
                {
                    String query = "select u.LastName, u.FirstName, u.Email," +
                        " c.Count from dbo.Users u Join dbo.Count c on c.UserId = u.UserId " +
                        "Join dbo.TitleOfBlog tab on tab.BlogNumber = c.BlogId where Title + ' ' + InteractionType = " + "'" + temp + "' Order By u.LastName";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    DataTable dt = new DataTable();
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(dt);

                    string totalQuery = "select SUM(count) from count c " +
                        "join dbo.TitleOfBlog tab on tab.BlogNumber = c.BlogId where tab.title + ' ' + tab.InteractionType = " + "'" + temp + "'";

                    SqlCommand cmd2 = new SqlCommand(totalQuery, conn);
                    Int32 count = (Int32)cmd2.ExecuteScalar();



                    //Set values for Form 2
                    //Set the datagrid view to the datatable.
                    Form2 fm = new Form2();
                    fm.textBox1.Text = temp;
                    fm.totalViews.Text = count.ToString();
                    fm.dgv = dt;
                    fm.dt = dt;
                    fm.Show();
                    conn.Close();
                }

                //Report is done in order of most views, to least views. 
                else if (choice == "Ascending: ")
                {
                    String query = "select u.LastName, u.FirstName, u.Email, c.Count from dbo.Users u Join dbo.Count c on c.UserId = u.UserId " +
              "Join dbo.TitleOfBlog tab on tab.BlogNumber = c.BlogId where Title + ' ' + InteractionType = " + "'" + temp + "' order by c.Count Desc";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    DataTable dt = new DataTable();
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(dt);

                    string totalQuery = "select SUM(count) from count c " +
                        "join dbo.TitleOfBlog tab on tab.BlogNumber = c.BlogId where tab.title + ' ' + tab.InteractionType = " + "'" + temp + "'";

                    SqlCommand cmd2 = new SqlCommand(totalQuery, conn);
                    Int32 count = (Int32)cmd2.ExecuteScalar();



                    //Set values for Form 2
                    //Set the datagrid view to the datatable.
                    Form2 fm = new Form2();
                    fm.textBox1.Text = temp;
                    fm.totalViews.Text = count.ToString();
                    fm.dgv = dt;
                    fm.dt = dt;
                    fm.Show();
                    conn.Close();
                }
            }


            //Ensure both blog and order are selected, otherwise throw a message box.
            else
            {

                MessageBox.Show("Please make sure Blog and Order are selected"); 
            }

            
        }


        //Gives a report with every person and their blog interaction count. 
        private void finalReview_Click(object sender, EventArgs e)
        {
             
            
            //Set up Connection 
            SqlConnection conn = new SqlConnection(@"Insert Connection String Here");

            conn.Open();

            //Final Report ordered by last name.
            string name = "Final Report";
            if (finalReportOrder.SelectedItem != null)
            {
                string temp = finalReportOrder.SelectedItem.ToString();
                if (temp == "Last Name:")
                {

                    SqlCommand cmd = new SqlCommand("finalReport", conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    DataTable dt = new DataTable();
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(dt);

                    //Pass to checkVoid Method  
                    checkVoid(dt);

                    Form2 fm = new Form2();
                    fm.textBox1.Text = name;
                    fm.dgv = dt;
                    fm.dt = dt;
                    fm.Show();



                    conn.Close();
                }



            //Final Report ordered by most to least interactions. 
            else
             {


                    SqlCommand cmd = new SqlCommand("finalReportDesc", conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    SqlDataReader reader;
                    DataTable dt = new DataTable();
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(dt);

                    //Check for missing values.
                    checkVoid(dt);


                    //Initialize Form 2. 
                    Form2 fm = new Form2();
                    fm.textBox1.Text = name;
                    fm.dgv = dt;
                    fm.dt = dt;
                    fm.Show();



                    conn.Close();
            }
            }

            else
            {
                MessageBox.Show("Please enter an Order for your Final Report"); 
            }
  
        }

        //This Method adds 0's to empty values to remove empty spaces.
        //It takes the datatables as an argument 
        public static void checkVoid(DataTable table)
        {

            
            foreach (DataRow row in table.Rows)
            {
                foreach (DataColumn col in table.Columns)
                {
                    if (row[col] == DBNull.Value)
                    {
                        row[col] = 0; 
                    }
                }
            }

            return; 
        }
    }

}


