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
using System.ComponentModel;
using System.Windows.Controls.Primitives;

namespace LeadsTracker_FinalsProject1
{
    /// <summary>
    /// Interaction logic for Menutable.xaml
    /// </summary>
    public partial class Menutable : Window
    {
        private List<Lead> originalLeads; // Variable to store the original data source
        private bool isAscendingOrder = true; // Variable to track sorting order
        public Menutable()
        {
            InitializeComponent();
            search.GotFocus += (s, ev) => { search.Text = ""; };
            LoadData();

            //if (MainWindow.isAdmin)
            //{
            //    remove.Visibility = Visibility.Collapsed;
            //}
            //else
            //{
            //    remove.Visibility = Visibility.Visible;
            //}
        }
        private void LoadData()
        {
            try
            {
				string connectionString = "Data Source=DESKTOPMIGUEL;Initial Catalog=\"Lead Tracker\";Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False";

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
                        if (MainWindow.isAdmin == true)
                        {
                            if (lead.Lead_Status != "Dead")
                            {
                                leads.Add(lead);
                            }
                        }
                        else
                        { leads.Add(lead); }
                    }
                    DataGridXAML.ItemsSource = leads;
                    originalLeads = leads;
                }
            }
            catch (Exception ex)
            {
                // Handle any exceptions that occur during data retrieval
                MessageBox.Show("An error occurred: " + ex.Message);
            }
            SortLeadsByLeadID();
        }

        private void FilterChanged(object sender, SelectionChangedEventArgs e)
        {
            ApplyFilters();
        }

        private void search_TextChanged(object sender, TextChangedEventArgs e)
        {
            ApplyFilters();
        }

        private void ApplyFilters()
        {
            if (originalLeads == null)
            {
                return;
            }

            string searchText = search.Text.Trim().ToLower();
            string selectedSource = sourceFilter.SelectedItem is ComboBoxItem sourceItem ? sourceItem.Content.ToString() : "";
            string selectedStatus = statusFilter.SelectedItem is ComboBoxItem statusItem ? statusItem.Content.ToString() : "";

            var filteredLeads = originalLeads.Where(lead =>
                (string.IsNullOrEmpty(searchText) ||
                    lead.Lead_Name.ToLower().Contains(searchText) ||
                    lead.Lead_Email.ToLower().Contains(searchText) ||
                    lead.Phone_Number.ToLower().Contains(searchText) ||
                    lead.Lead_Source.ToLower().Contains(searchText) ||
                    lead.Notes.ToLower().Contains(searchText) ||
                    lead.Lead_Status.ToLower().Contains(searchText)) &&
                (string.IsNullOrEmpty(selectedSource) || lead.Lead_Source == selectedSource) &&
                (string.IsNullOrEmpty(selectedStatus) || lead.Lead_Status == selectedStatus)
            ).ToList();

            DataGridXAML.ItemsSource = null;
            DataGridXAML.ItemsSource = filteredLeads;
        }


        private void removeButton_Click(object sender, RoutedEventArgs e)
        {
            if (DataGridXAML.SelectedItem is Lead selectedLead)
            {
                string message = $"Do you want to mark the following lead as dead?\n\n" +
                                 $"Lead ID: {selectedLead.Lead_ID}\n" +
                                 $"Lead Name: {selectedLead.Lead_Name}\n" +
                                 $"Lead Email: {selectedLead.Lead_Email}\n" +
                                 $"Phone Number: {selectedLead.Phone_Number}";

                MessageBoxResult result = MessageBox.Show(message, "Confirm Mark as Dead", MessageBoxButton.YesNo, MessageBoxImage.Warning);

                if (result == MessageBoxResult.Yes)
                {
                    MarkLeadAsDead(selectedLead.Lead_ID);
                    LoadData(); // Refresh the DataGrid
                }
            }
            else
            {
                MessageBox.Show("Please select a lead to mark as dead.", "No Lead Selected", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void MarkLeadAsDead(string leadID)
        {
            try
            {
                string connectionString = "Data Source=DESKTOPMIGUEL;Initial Catalog=\"Lead Tracker\";Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False";
                string query = "UPDATE Leads SET Lead_Status = 'Dead' WHERE Lead_ID = @Lead_ID;";

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@Lead_ID", leadID);

                    connection.Open();
                    command.ExecuteNonQuery();
                }

                MessageBox.Show("Lead marked as dead successfully.", "Lead Updated", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred: " + ex.Message);
            }
        }

        private void saveButton_Click(object sender, RoutedEventArgs e)
        {
            // Filter out non-Lead items and check for new lead entry
            var newLead = DataGridXAML.Items
                .OfType<Lead>()  // Filter only items that are of type Lead
                .FirstOrDefault(l => string.IsNullOrEmpty(l.Lead_ID));

            Lead selectedLead = DataGridXAML.SelectedItem as Lead;

            if (newLead != null)
            {
                // Validate the new lead
                if (!IsValidLead(newLead))
                {
                    return; // Validation failed, exit without saving
                }

                // Confirmation message for saving a new lead
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
            else if (selectedLead != null)
            {
                // Validate the existing lead
                if (!IsValidLead(selectedLead))
                {
                    return; // Validation failed, exit without saving
                }

                // Confirmation message for updating an existing lead
                string message = $"Do you want to save changes for the following lead?\n\n" +
                                 $"Lead ID: {selectedLead.Lead_ID}\n" +
                                 $"Lead Name: {selectedLead.Lead_Name}\n" +
                                 $"Lead Email: {selectedLead.Lead_Email}\n" +
                                 $"Phone Number: {selectedLead.Phone_Number}\n" +
                                 $"Lead Source: {selectedLead.Lead_Source}\n" +
                                 $"Notes: {selectedLead.Notes}\n" +
                                 $"Lead Status: {selectedLead.Lead_Status}\n" +
                                 $"Interview Date: {selectedLead.Interview_Date}";

                MessageBoxResult result = MessageBox.Show(message, "Confirm Update", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    UpdateLead(selectedLead);
                    LoadData(); // Refresh the DataGrid
                }
            }
            else
            {
                MessageBox.Show("Please select a lead to update or enter details for a new lead in the blank row.", "No Lead Selected", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private bool IsValidLead(Lead lead)
        {
            // Check Lead_Name
            if (string.IsNullOrEmpty(lead.Lead_Name))
            {
                MessageBox.Show("Please enter the lead's name.", "Missing Lead Name", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            // Check Lead_Source
            List<string> validSources = new List<string> { "Facebook", "Instagram", "Twitter", "Physical ads", "Referral" };
            if (!validSources.Contains(lead.Lead_Source, StringComparer.OrdinalIgnoreCase))
            {
                MessageBox.Show("Invalid lead source. Choose from: Facebook, Instagram, Twitter, Physical ads, and Referral", "Invalid Lead Source", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            // Check Lead_Status
            List<string> validStatuses = new List<string> { "Cold", "Warm", "Hot", "Dead" };
            if (!validStatuses.Contains(lead.Lead_Status, StringComparer.OrdinalIgnoreCase))
            {
                MessageBox.Show("Invalid lead status. Choose from: Cold, Warm, Hot, and Dead", "Invalid Lead Status", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            // Check at least one of Email or Phone Number
            if (string.IsNullOrEmpty(lead.Lead_Email) && string.IsNullOrEmpty(lead.Phone_Number))
            {
                MessageBoxResult noContactResult = MessageBox.Show("Do you want to save the lead without email nor phone number?", "Missing Contact Information", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (noContactResult != MessageBoxResult.Yes)
                {
                    return false; // User opted not to save without contact info
                }
            }

            return true; // Lead is valid
        }

        private void UpdateLead(Lead lead)
        {
            try
            {
                string connectionString = "Data Source=DESKTOPMIGUEL;Initial Catalog=\"Lead Tracker\";Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False";
				string query = "UPDATE Leads SET Lead_Name = @Lead_Name, Lead_Email = @Lead_Email, " +
                               "Phone_Number = @Phone_Number, Lead_Source = @Lead_Source, " +
                               "Notes = @Notes, Lead_Status = @Lead_Status, Interview_Date = @Interview_Date " +
                               "WHERE Lead_ID = @Lead_ID;";

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@Lead_ID", lead.Lead_ID);
                    command.Parameters.AddWithValue("@Lead_Name", lead.Lead_Name ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@Lead_Email", lead.Lead_Email ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@Phone_Number", lead.Phone_Number ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@Lead_Source", lead.Lead_Source ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@Notes", lead.Notes ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@Lead_Status", lead.Lead_Status ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@Interview_Date", lead.Interview_Date ?? (object)DBNull.Value);

                    connection.Open();
                    command.ExecuteNonQuery();
                }

                MessageBox.Show("Lead updated successfully.", "Lead Updated", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private void SaveNewLead(Lead newLead)
        {
            try
            {
                string connectionString = "Data Source=DESKTOPMIGUEL;Initial Catalog=\"Lead Tracker\";Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False";

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

                    string queryInsertDocument = "INSERT INTO Documents (Documents_ID, Picture, Birth_Certificate, Good_Moral, TOR, Medical_Clearance, Report_Card) " +
                             "VALUES (@Documents_ID, @Picture, @Birth_Certificate, @Good_Moral, @TOR, @Medical_Clearance, @Report_Card);";

                    SqlCommand commandInsertDocument = new SqlCommand(queryInsertDocument, connection);
                    commandInsertDocument.Parameters.AddWithValue("@Documents_ID", nextLeadID);
                    commandInsertDocument.Parameters.AddWithValue("@Picture", $@"C:\Users\PC\OneDrive\Pictures\documents\Picture\{nextLeadID}");
                    commandInsertDocument.Parameters.AddWithValue("@Birth_Certificate", $@"C:\Users\PC\OneDrive\Pictures\documents\Birth_Certificate\{nextLeadID}");
                    commandInsertDocument.Parameters.AddWithValue("@Good_Moral", $@"C:\Users\PC\OneDrive\Pictures\documents\Good_Moral\{nextLeadID}");
                    commandInsertDocument.Parameters.AddWithValue("@TOR", $@"C:\Users\PC\OneDrive\Pictures\documents\TOR\{nextLeadID}");
                    commandInsertDocument.Parameters.AddWithValue("@Medical_Clearance", $@"C:\Users\PC\OneDrive\Pictures\documents\Medical_Clearance\{nextLeadID}");
                    commandInsertDocument.Parameters.AddWithValue("@Report_Card", $@"C:\Users\PC\OneDrive\Pictures\documents\Report_Card\{nextLeadID}");

                    commandInsertDocument.ExecuteNonQuery();
                }

                MessageBox.Show("New lead saved successfully.", "Lead Saved", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
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

        private void LeadDocuments_Click(object sender, RoutedEventArgs e)
        {
            if (DataGridXAML.SelectedItem is Lead selectedLead)
            {
                var document = GetDocumentForLead(selectedLead.Documents_ID);
                Documents docu = new Documents(document);
                docu.Show();
            }
            else
            {
                MessageBox.Show("Please select a lead to view documents.", "No Lead Selected", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
		private Document GetDocumentForLead(string documentsId)
		{
			Document document = null;

            try
            {
				string connectionString = "Data Source=DESKTOPMIGUEL;Initial Catalog=\"Lead Tracker\";Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False";
				string query = "SELECT * FROM Documents WHERE Documents_ID = @Documents_ID";

				using (SqlConnection connection = new SqlConnection(connectionString))
				{
					SqlCommand command = new SqlCommand(query, connection);
					command.Parameters.AddWithValue("@Documents_ID", documentsId);
					connection.Open();
					SqlDataReader reader = command.ExecuteReader();

					if (reader.Read())
					{
						document = new Document
						{
							Documents_ID = reader["Documents_ID"].ToString(),
							Picture = reader["Picture"] != DBNull.Value ? reader["Picture"].ToString() : null,
							Birth_Certificate = reader["Birth_Certificate"] != DBNull.Value ? reader["Birth_Certificate"].ToString() : null,
							Good_Moral = reader["Good_Moral"] != DBNull.Value ? reader["Good_Moral"].ToString() : null,
							TOR = reader["TOR"] != DBNull.Value ? reader["TOR"].ToString() : null,
							Medical_Clearance = reader["Medical_Clearance"] != DBNull.Value ? reader["Medical_Clearance"].ToString() : null,
							Report_Card = reader["Report_Card"] != DBNull.Value ? reader["Report_Card"].ToString() : null
						};
					}
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show("An error occurred: " + ex.Message);
			}

            return document;
        }

        private void DataGridXAML_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void leadSort_Click(object sender, RoutedEventArgs e)
        {
            SortLeadsByLeadID();
        }

        private void SortLeadsByLeadID()
        {
            if (originalLeads == null)
            {
                return;
            }

            var sortedLeads = isAscendingOrder
                ? originalLeads.OrderBy(lead => int.TryParse(lead.Lead_ID, out int id) ? id : int.MaxValue).ToList()
                : originalLeads.OrderByDescending(lead => int.TryParse(lead.Lead_ID, out int id) ? id : int.MinValue).ToList();

            isAscendingOrder = !isAscendingOrder;

            DataGridXAML.ItemsSource = null;
            DataGridXAML.ItemsSource = sortedLeads;
        }
    }
}
