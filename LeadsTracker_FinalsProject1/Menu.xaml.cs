using LeadsTracker_FinalsProject1;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Globalization;
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
    /// Interaction logic for Menu.xaml
    /// </summary>
    public partial class Menu : Window, INotifyPropertyChanged
    {
        private ObservableCollection<Lead> leads;
        private Lead selectedLead;
        private ObservableCollection<Lead> originalLeads;
        private bool isAscendingOrder = true; // Variable to track sorting order

        public event PropertyChangedEventHandler PropertyChanged;

        public Menu()
        {
            InitializeComponent();
            LoadData(); // Load data into the ListBox
            DataContext = this; // Set DataContext for data binding
            //if (MainWindow.isAdmin)
            //{
            //    removeButton.Visibility = Visibility.Collapsed;
            //}
            //else
            //{
            //    removeButton.Visibility = Visibility.Visible;
            //}
        }

        public ObservableCollection<Lead> Leads
        {
            get { return leads; }
            set
            {
                leads = value;
                OnPropertyChanged("Leads");
            }
        }

        public Lead SelectedLead
        {
            get { return selectedLead; }
            set
            {
                selectedLead = value;
                OnPropertyChanged("SelectedLead");
                UpdateTextBoxes();
            }
        }

        private void LoadData()
        {
            try
            {
				string connectionString = "Data Source=DESKTOP-F726TKR\\SQLEXPRESS;Initial Catalog=\"Lead Tracker\";Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False";
				string query = "SELECT Lead_ID, Date, Lead_Name, Lead_Status, Lead_Email, Phone_Number, Lead_Source, Notes, Documents_ID, Interview_Date FROM Leads;";

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    SqlCommand command = new SqlCommand(query, connection);
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    Leads = new ObservableCollection<Lead>();

                    while (reader.Read())
                    {
                        Lead lead = new Lead
                        {
                            Lead_ID = reader["Lead_ID"].ToString(),
                            Date = reader["Date"].ToString(),
                            Lead_Name = reader["Lead_Name"].ToString(),
                            Lead_Status = reader["Lead_Status"].ToString(),
                            Lead_Email = reader["Lead_Email"].ToString(),
                            Phone_Number = reader["Phone_Number"].ToString(),
                            Lead_Source = reader["Lead_Source"].ToString(),
                            Notes = reader["Notes"].ToString(),
                            Documents_ID = reader["Documents_ID"].ToString(),
                            Interview_Date = reader["Interview_Date"].ToString()
                        };
                        if (MainWindow.isAdmin == true)
                        {
                            if (lead.Lead_Status != "Dead")
                            {
                                leads.Add(lead);
                            }
                        }
                        else
                        {
                            leads.Add(lead);
                        }
                    }
                    leadList.ItemsSource = leads;
                    originalLeads = leads;
                }
                UpdateCounters();
                SortLeadsByLeadID();
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred: " + ex.Message);
            }
        }
        private void UpdateCounters()
        {
            if (Leads != null && Leads.Count > 0)
            {
                // Total Leads
                totalCounter.Text = Leads.Count.ToString();

                // New Leads (1 week old)
                DateTime oneWeekAgo = DateTime.Today.AddDays(-7);
                int newLeadsCount = Leads.Count(lead => DateTime.Parse(lead.Date) >= oneWeekAgo);
                newCounter.Text = newLeadsCount.ToString();

                // Closed Leads (Lead_Status = Dead)
                int closedLeadsCount = Leads.Count(lead => lead.Lead_Status == "Dead");
                closedCounter.Text = closedLeadsCount.ToString();
            }
            else
            {
                // Reset counters if Leads collection is null or empty
                totalCounter.Text = "0";
                newCounter.Text = "0";
                closedCounter.Text = "0";
            }
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
                    lead.Phone_Number.ToLower().Contains(searchText)) &&
                (string.IsNullOrEmpty(selectedSource) || lead.Lead_Source == selectedSource) &&
                (string.IsNullOrEmpty(selectedStatus) || lead.Lead_Status == selectedStatus)
            ).ToList();

            leadList.ItemsSource = null;
            leadList.ItemsSource = filteredLeads;
        }

        private void removeButton_Click(object sender, RoutedEventArgs e)
        {
            if (leadList.SelectedItem is Lead selectedLead)
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
                string connectionString = "Data Source=DESKTOP-F726TKR\\SQLEXPRESS;Initial Catalog=\"Lead Tracker\";Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False";
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

        private void UpdateTextBoxes()
        {
            if (SelectedLead != null)
            {
                displayID.Text = SelectedLead.Lead_ID;
                nameBox.Text = SelectedLead.Lead_Name;
                statusBox.Text = SelectedLead.Lead_Status;
                emailBox.Text = SelectedLead.Lead_Email;
                sourceBox.Text = SelectedLead.Lead_Source;
                numberBox.Text = SelectedLead.Phone_Number;
                notesBox.Text = SelectedLead.Notes;
                dateAddedBox.Text = SelectedLead.Date;
                documentsBox.Text = SelectedLead.Documents_ID;
                interviewDateBox.Text = SelectedLead.Interview_Date;
            }
            else
            {
                ClearTextBoxes();
            }
        }

        private void ClearTextBoxes()
        {
            displayID.Text = string.Empty;
            nameBox.Text = string.Empty;
            statusBox.Text = string.Empty;
            sourceBox.Text = string.Empty;
            emailBox.Text = string.Empty;
            numberBox.Text = string.Empty;
            notesBox.Text = string.Empty;
            dateAddedBox.Text = string.Empty;
            documentsBox.Text = string.Empty;
            interviewDateBox.Text = string.Empty;
        }

        private void viewButton_Click(object sender, RoutedEventArgs e)
        {
            Menutable w1 = new Menutable();
            w1.Show();
            this.Close();
        }

        private void signoutButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow w2 = new MainWindow();
            w2.Show();
            this.Close();
        }

        private void save_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedLead != null)
            {
                // Check if Lead_ID is null or empty
                if (string.IsNullOrEmpty(SelectedLead.Lead_ID))
                {
                    MessageBox.Show("This is not an existing lead, how can you possibly save changes to it?", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Update the selected lead with the current TextBox values
                SelectedLead.Lead_Name = nameBox.Text;
                SelectedLead.Lead_Status = statusBox.Text;
                SelectedLead.Lead_Email = emailBox.Text;
                SelectedLead.Phone_Number = numberBox.Text;
                SelectedLead.Lead_Source = sourceBox.Text;
                SelectedLead.Notes = notesBox.Text;
                SelectedLead.Date = dateAddedBox.Text;
                SelectedLead.Documents_ID = documentsBox.Text;
                SelectedLead.Interview_Date = interviewDateBox.Text;

                // Save changes back to the database
                SaveChangesToDatabase(SelectedLead);
            }
            else
            {
                MessageBox.Show("No lead is selected to save changes.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SaveChangesToDatabase(Lead leadToUpdate)
        {
            try
            {
				string connectionString = "Data Source=DESKTOP-F726TKR\\SQLEXPRESS;Initial Catalog=\"Lead Tracker\";Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False";
				string query = "UPDATE Leads SET Lead_Name=@LeadName, Lead_Status=@LeadStatus, Lead_Email=@LeadEmail, Date=@Date, Lead_Source=@LeadSource, Phone_Number=@PhoneNumber, Notes=@Notes, Documents_ID=@DocumentsID, Interview_Date=@InterviewDate WHERE Lead_ID=@LeadID";

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@LeadName", leadToUpdate.Lead_Name ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@LeadStatus", leadToUpdate.Lead_Status ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@LeadEmail", leadToUpdate.Lead_Email ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@LeadSource", leadToUpdate.Lead_Source ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@PhoneNumber", leadToUpdate.Phone_Number ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@Notes", leadToUpdate.Notes ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@Date", leadToUpdate.Date ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@DocumentsID", leadToUpdate.Documents_ID ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@InterviewDate", string.IsNullOrEmpty(leadToUpdate.Interview_Date) ? (object)DBNull.Value : leadToUpdate.Interview_Date);
                    command.Parameters.AddWithValue("@LeadID", leadToUpdate.Lead_ID);

                    connection.Open();
                    int rowsAffected = command.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Changes saved successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        MessageBox.Show("No rows affected. Please check the lead ID and try again.", "No Changes", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred while saving changes to the database: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public class Lead : INotifyPropertyChanged
        {
            private string lead_ID;
            private string lead_Name;
            private string lead_Status;
            private string lead_Email;
            private string phone_Number;
            private string lead_Source;
            private string notes;
            private string date;
            private string documents_ID;
            private string interview_Date;

            public event PropertyChangedEventHandler PropertyChanged;

            public string Lead_ID
            {
                get { return lead_ID; }
                set
                {
                    lead_ID = value;
                    OnPropertyChanged("Lead_ID");
                }
            }

            public string Lead_Name
            {
                get { return lead_Name; }
                set
                {
                    lead_Name = value;
                    OnPropertyChanged("Lead_Name");
                }
            }
            public string Date
            {
                get { return date; }
                set
                {
                    date = value;
                    OnPropertyChanged("Date");
                }
            }

            public string Lead_Status
            {
                get { return lead_Status; }
                set
                {
                    lead_Status = value;
                    OnPropertyChanged("Lead_Status");
                }
            }
            public string Lead_Source
            {
                get { return lead_Source; }
                set
                {
                    lead_Source = value;
                    OnPropertyChanged("Lead_Source");
                }
            }

            public string Lead_Email
            {
                get { return lead_Email; }
                set
                {
                    lead_Email = value;
                    OnPropertyChanged("Lead_Email");
                }
            }

            public string Phone_Number
            {
                get { return phone_Number; }
                set
                {
                    phone_Number = value;
                    OnPropertyChanged("Phone_Number");
                }
            }

            public string Notes
            {
                get { return notes; }
                set
                {
                    notes = value;
                    OnPropertyChanged("Notes");
                }
            }

            public string Documents_ID
            {
                get { return documents_ID; }
                set
                {
                    documents_ID = value;
                    OnPropertyChanged("Documents_ID");
                }
            }

            public string Interview_Date
            {
                get { return interview_Date; }
                set
                {
                    interview_Date = value;
                    OnPropertyChanged("Interview_Date");
                }
            }

            protected void OnPropertyChanged(string propertyName)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }


        private void addLead_Click(object sender, RoutedEventArgs e)
        {
            // Check if displayID is not empty, indicating an existing lead is displayed
            if (!string.IsNullOrEmpty(displayID.Text))
            {
                MessageBoxResult result = MessageBox.Show("This is an existing lead. If you want to add a new lead, do you want to clear the form?", "Existing Lead", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    // Clear textboxes
                    ClearTextBoxes();
                    return;
                }
                else
                {
                    return; // User chose not to clear the form, exit method
                }
            }

            // Validate Lead_Source and Lead_Status
            if (string.IsNullOrEmpty(sourceBox.Text))
            {
                MessageBox.Show("Please choose a Lead Source.", "Missing Lead Source", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrEmpty(statusBox.Text))
            {
                MessageBox.Show("Please choose a Lead Status.", "Missing Lead Status", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrEmpty(nameBox.Text))
            {
                MessageBox.Show("Please enter the lead's name.", "Missing Lead Name", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Check if both email and phone number fields are empty
            if (string.IsNullOrEmpty(emailBox.Text) && string.IsNullOrEmpty(numberBox.Text))
            {
                MessageBoxResult result = MessageBox.Show("You have not entered an email or phone number for this lead. Do you still want to save it?", "Confirm Save Without Contact Information", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result != MessageBoxResult.Yes)
                {
                    return; // User chose not to save the lead without contact information, exit method
                }
            }

            // Create a new Lead object
            Lead newLead = new Lead
            {
                Lead_Name = nameBox.Text,
                Lead_Status = statusBox.Text,
                Lead_Email = emailBox.Text,
                Phone_Number = numberBox.Text,
                Lead_Source = sourceBox.Text,
                Notes = notesBox.Text,
                Date = DateTime.Now.ToString(), // Set to current date as DateTime
                Documents_ID = string.Empty, // You may set this as needed
                Interview_Date = interviewDateBox.Text // You may set this as needed
            };

            // Save the new lead to the database
            SaveNewLead(newLead);
        }



        private void SaveNewLead(Lead newLead)
        {
            try
            {
				string connectionString = "Data Source=DESKTOP-F726TKR\\SQLEXPRESS  ;Initial Catalog=\"Lead Tracker\";Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False";

				using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    // Find the next Lead_ID
                    string queryGetMaxID = "SELECT ISNULL(MAX(CAST(Lead_ID AS INT)), 0) + 1 FROM Leads;";
                    SqlCommand commandGetMaxID = new SqlCommand(queryGetMaxID, connection);
                    int nextLeadID = (int)commandGetMaxID.ExecuteScalar();

                    // Insert the new lead
                    string queryInsert = "INSERT INTO Leads (Lead_ID, Date, Lead_Name, Lead_Status, Lead_Email, Phone_Number, Lead_Source, Notes, Documents_ID, Interview_Date) " +
                                         "VALUES (@Lead_ID, @Date, @Lead_Name, @Lead_Status, @Lead_Email, @Phone_Number, @Lead_Source, @Notes, @Documents_ID, @Interview_Date);";

                    SqlCommand commandInsert = new SqlCommand(queryInsert, connection);
                    commandInsert.Parameters.AddWithValue("@Lead_ID", nextLeadID);
                    commandInsert.Parameters.AddWithValue("@Date", DateTime.Now); // Set to current date
                    commandInsert.Parameters.AddWithValue("@Lead_Name", newLead.Lead_Name ?? (object)DBNull.Value);
                    commandInsert.Parameters.AddWithValue("@Lead_Status", newLead.Lead_Status ?? (object)DBNull.Value);
                    commandInsert.Parameters.AddWithValue("@Lead_Email", newLead.Lead_Email ?? (object)DBNull.Value);
                    commandInsert.Parameters.AddWithValue("@Phone_Number", newLead.Phone_Number ?? (object)DBNull.Value);
                    commandInsert.Parameters.AddWithValue("@Lead_Source", newLead.Lead_Source ?? (object)DBNull.Value);
                    commandInsert.Parameters.AddWithValue("@Notes", newLead.Notes ?? (object)DBNull.Value);
                    commandInsert.Parameters.AddWithValue("@Documents_ID", nextLeadID.ToString()); // Documents_ID is the same as Lead_ID
                    commandInsert.Parameters.AddWithValue("@Interview_Date", string.IsNullOrEmpty(newLead.Interview_Date) ? (object)DBNull.Value : newLead.Interview_Date);

                    commandInsert.ExecuteNonQuery();

                    MessageBox.Show("New lead added successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }

                LoadData(); // Refresh the leads list after adding a new lead
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred while adding a new lead: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
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

            leadList.ItemsSource = null;
            leadList.ItemsSource = sortedLeads;
        }

        private void clear_Click(object sender, RoutedEventArgs e)
        {
            // Clear textboxes
            ClearTextBoxes();
        }
    }
}
