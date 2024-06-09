using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
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
using System.Windows.Shapes;

namespace LeadsTracker_FinalsProject1
{
    /// <summary>
    /// Interaction logic for Menutable.xaml
    /// </summary>
    public partial class Menutable : Window
    {
        public Menutable()
        {
            InitializeComponent();
            LoadData();

        }
        private void LoadData()
        {
            try
            {
                // Define your connection string (update it with your actual database connection string)
                string connectionString = "Data Source=DESKTOP-F726TKR\\SQLEXPRESS;Initial Catalog=Lead Tracker;Integrated Security=True;Connect Timeout=30;Encrypt=True;TrustServerCertificate=True";

                // Define your query
                string query = "SELECT * FROM Leads;";

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    SqlCommand command = new SqlCommand(query, connection);
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();

                    List<Lead> leads = new List<Lead>();

                    while (reader.Read())
                    {
                        Lead lead = new Lead
                        {
                            Lead_ID = reader["Lead_ID"].ToString(),
                            Date = reader["Date"] != DBNull.Value ? (DateTime)reader["Date"] : (DateTime?)null,
                            Lead_Source = reader["Lead_Source"].ToString(),
                            Lead_Name = reader["Lead_Name"].ToString(),
                            Lead_Email = reader["Lead_Email"].ToString(),
                            Phone_Number = reader["Phone_Number"].ToString(),
                            Notes = reader["Notes"].ToString(),
                            Documents_ID = reader["Documents_ID"].ToString(),
                            Lead_Status = reader["Lead_Status"].ToString(),
                            Interview_Date = reader["Interview_Date"] != DBNull.Value ? reader["Interview_Date"].ToString() : null
                        };

                        leads.Add(lead);
                    }
                    DataGridXAML.ItemsSource = leads;
                }
            }
            catch (Exception ex)
            {
                // Handle any exceptions that occur during data retrieval
                MessageBox.Show("An error occurred: " + ex.Message);
            }
        }
        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void viewButton_Click(object sender, RoutedEventArgs e)
        {
            Menu w1 = new Menu();
            w1.Show();
            this.Close();
        }
        public class Lead
        {
            public string Lead_ID { get; set; }
            public DateTime? Date { get; set; }
            public string Lead_Source { get; set; }
            public string Lead_Name { get; set; }
            public string Lead_Email { get; set; }
            public string Phone_Number { get; set; }
            public string Notes { get; set; }
            public string Documents_ID { get; set; }
            public string Lead_Status { get; set; }
            public string Interview_Date { get; set; }
        }
    }
}
