﻿using LeadsTracker_FinalsProject1;
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

        public event PropertyChangedEventHandler PropertyChanged;

        public Menu()
        {
            InitializeComponent();
            LoadData(); // Load data into the ListBox
            DataContext = this; // Set DataContext for data binding
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
                string connectionString = "Data Source=DESKTOP-F726TKR\\SQLEXPRESS;Initial Catalog=Lead Tracker;Integrated Security=True;";
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

                        Leads.Add(lead);
                    }
                    leadList.ItemsSource = leads;
                    originalLeads = leads;
                }
                UpdateCounters();
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

                // Optionally, save changes back to the database if needed
                SaveChangesToDatabase(SelectedLead);
            }
        }

        private void SaveChangesToDatabase(Lead leadToUpdate)
        {
            try
            {
                string connectionString = "Data Source=DESKTOP-F726TKR\\SQLEXPRESS;Initial Catalog=Lead Tracker;Integrated Security=True;";
                string query = "UPDATE Leads SET Lead_Name=@LeadName, Lead_Status=@LeadStatus, Lead_Email=@LeadEmail, Date=@Date, Lead_Source=@LeadSource, Phone_Number=@PhoneNumber, Notes=@Notes, Documents_ID=@DocumentsID, Interview_Date=@InterviewDate WHERE Lead_ID=@LeadID";

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@LeadName", leadToUpdate.Lead_Name);
                    command.Parameters.AddWithValue("@LeadStatus", leadToUpdate.Lead_Status);
                    command.Parameters.AddWithValue("@LeadEmail", leadToUpdate.Lead_Email);
                    command.Parameters.AddWithValue("@LeadSource", leadToUpdate.Lead_Source);
                    command.Parameters.AddWithValue("@PhoneNumber", leadToUpdate.Phone_Number);
                    command.Parameters.AddWithValue("@Notes", leadToUpdate.Notes);
                    command.Parameters.AddWithValue("@Date", leadToUpdate.Date);
                    command.Parameters.AddWithValue("@DocumentsID", leadToUpdate.Documents_ID);
                    command.Parameters.AddWithValue("@InterviewDate", leadToUpdate.Interview_Date);
                    command.Parameters.AddWithValue("@LeadID", leadToUpdate.Lead_ID);

                    connection.Open();
                    int rowsAffected = command.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Lead details updated successfully!");
                    }
                    else
                    {
                        MessageBox.Show("No rows affected. Update failed.");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred while updating lead details: " + ex.Message);
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

            // Create a new Lead object
            Lead newLead = new Lead
            {
                Lead_Name = nameBox.Text,
                Lead_Status = statusBox.Text,
                Lead_Email = emailBox.Text,
                Phone_Number = numberBox.Text,
                Lead_Source = sourceBox.Text,
                Notes = notesBox.Text,
                Date = DateTime.Now.ToString(), // Set to current date
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
                string connectionString = "Data Source=DESKTOP-F726TKR\\SQLEXPRESS;Initial Catalog=Lead Tracker;Integrated Security=True;";

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

                    MessageBox.Show("New lead added successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }

                LoadData(); // Refresh the leads list after adding a new lead
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred while adding a new lead: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private void clear_Click(object sender, RoutedEventArgs e)
        {
            // Clear textboxes
            ClearTextBoxes();
        }
    }
}
