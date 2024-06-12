﻿using System;
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

        private void removeButton_Click(object sender, RoutedEventArgs e)
        {
            if (DataGridXAML.SelectedItem is Lead selectedLead)
            {
                string message = $"Do you want to remove the following lead?\n\n" +
                                 $"Lead ID: {selectedLead.Lead_ID}\n" +
                                 $"Lead Name: {selectedLead.Lead_Name}\n" +
                                 $"Lead Email: {selectedLead.Lead_Email}\n" +
                                 $"Phone Number: {selectedLead.Phone_Number}";

                MessageBoxResult result = MessageBox.Show(message, "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Warning);

                if (result == MessageBoxResult.Yes)
                {
                    DeleteLead(selectedLead.Lead_ID);
                    LoadData(); // Refresh the DataGrid
                }
            }
            else
            {
                MessageBox.Show("Please select a lead to remove.", "No Lead Selected", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void DeleteLead(string leadID)
        {
            try
            {
                string connectionString = "Data Source=DESKTOP-F726TKR\\SQLEXPRESS;Initial Catalog=Lead Tracker;Integrated Security=True;Connect Timeout=30;Encrypt=True;TrustServerCertificate=True";
                string query = "DELETE FROM Leads WHERE Lead_ID = @Lead_ID;";

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@Lead_ID", leadID);

                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred: " + ex.Message);
            }
        }

        private void saveButton_Click(object sender, RoutedEventArgs e)
        {
            var newLead = DataGridXAML.Items.Cast<Lead>().FirstOrDefault(l => string.IsNullOrEmpty(l.Lead_ID));
            if (newLead != null)
            {
                string message = $"Do you want to save the following new lead?\n\n" +
                                 $"Lead Name: {newLead.Lead_Name}\n" +
                                 $"Lead Email: {newLead.Lead_Email}\n" +
                                 $"Phone Number: {newLead.Phone_Number}\n" +
                                 $"Lead Source: {newLead.Lead_Source}\n" +
                                 $"Notes: {newLead.Notes}\n" +
                                 $"Lead Status: {newLead.Lead_Status}\n" +
                                 $"Interview Date: {newLead.Interview_Date}";

                MessageBoxResult result = MessageBox.Show(message, "Confirm Save", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    SaveNewLead(newLead);
                    LoadData(); // Refresh the DataGrid
                }
            }
            else
            {
                MessageBox.Show("Please enter details for a new lead in the blank row.", "No New Lead", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void SaveNewLead(Lead newLead)
        {
            try
            {
                string connectionString = "Data Source=DESKTOP-F726TKR\\SQLEXPRESS;Initial Catalog=Lead Tracker;Integrated Security=True;Connect Timeout=30;Encrypt=True;TrustServerCertificate=True";

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    // Find the next Lead_ID
                    string queryGetMaxID = "SELECT ISNULL(MAX(CAST(Lead_ID AS INT)), 0) + 1 FROM Leads;";
                    SqlCommand commandGetMaxID = new SqlCommand(queryGetMaxID, connection);
                    int nextLeadID = (int)commandGetMaxID.ExecuteScalar();

                    // Insert the new lead
                    string queryInsert = "INSERT INTO Leads (Lead_ID, Date, Lead_Source, Lead_Name, Lead_Email, Phone_Number, Notes, Documents_ID, Lead_Status, Interview_Date) " +
                                         "VALUES (@Lead_ID, @Date, @Lead_Source, @Lead_Name, @Lead_Email, @Phone_Number, @Notes, @Documents_ID, @Lead_Status, @Interview_Date);";

                    SqlCommand commandInsert = new SqlCommand(queryInsert, connection);
                    commandInsert.Parameters.AddWithValue("@Lead_ID", nextLeadID);
                    commandInsert.Parameters.AddWithValue("@Date", DateTime.Now); // Set to current date
                    commandInsert.Parameters.AddWithValue("@Lead_Source", newLead.Lead_Source ?? (object)DBNull.Value);
                    commandInsert.Parameters.AddWithValue("@Lead_Name", newLead.Lead_Name ?? (object)DBNull.Value);
                    commandInsert.Parameters.AddWithValue("@Lead_Email", newLead.Lead_Email ?? (object)DBNull.Value);
                    commandInsert.Parameters.AddWithValue("@Phone_Number", newLead.Phone_Number ?? (object)DBNull.Value);
                    commandInsert.Parameters.AddWithValue("@Notes", newLead.Notes ?? (object)DBNull.Value);
                    commandInsert.Parameters.AddWithValue("@Documents_ID", nextLeadID.ToString()); // Documents_ID is the same as Lead_ID
                    commandInsert.Parameters.AddWithValue("@Lead_Status", newLead.Lead_Status ?? (object)DBNull.Value);
                    commandInsert.Parameters.AddWithValue("@Interview_Date", newLead.Interview_Date ?? (object)DBNull.Value);

                    commandInsert.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
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
