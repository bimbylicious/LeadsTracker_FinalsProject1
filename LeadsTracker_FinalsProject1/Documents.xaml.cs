using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
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
using System.Xml.Linq;

namespace LeadsTracker_FinalsProject1
{
	/// <summary>
	/// Interaction logic for Documents.xaml
	/// </summary>
	public partial class Documents : Window
	{
		private const string FolderName = "UploadedDocuments";
		private Document document;
		private string imagesFolderPath;

		public Documents(Document document)
		{
			InitializeComponent();
			this.document = document ?? new Document { Documents_ID = GetNextDocumentId() };

			InitializeImageFolder();

			if (document != null)
			{
				LoadDocumentImages(document);
			}
		}

		private void InitializeImageFolder()
		{
			imagesFolderPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, FolderName);

			if (!Directory.Exists(imagesFolderPath))
			{
				Directory.CreateDirectory(imagesFolderPath);
			}
		}

		private void LoadDocumentImages(Document document)
		{
			Picture.Source = LoadImageFromPath(document.Picture);
			Birth_Certificate.Source = LoadImageFromPath(document.Birth_Certificate);
			Picture.Source = LoadImageFromPath(document.Picture);
			Birth_Certificate.Source = LoadImageFromPath(document.Birth_Certificate);
			Good_Moral.Source = LoadImageFromPath(document.Good_Moral);
			TOR.Source = LoadImageFromPath(document.TOR);
			Medical_Clearance.Source = LoadImageFromPath(document.Medical_Clearance);
			Report_Card.Source = LoadImageFromPath(document.Report_Card);
		}

		private BitmapImage LoadImageFromPath(string path)
		{
			return !string.IsNullOrEmpty(path) ? new BitmapImage(new Uri(path, UriKind.Absolute)) : null;
		}

		private string GetNextDocumentId()
		{
			string nextId = "1";
			string connectionString = "Data Source=DESKTOP-F726TKR\\SQLEXPRESS;Initial Catalog=\"Lead Tracker\";Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False";

			using (SqlConnection connection = new SqlConnection(connectionString))
			{
				string query = "SELECT TOP 1 CAST(Documents_ID AS INT) FROM Documents ORDER BY CAST(Documents_ID AS INT) DESC";
				SqlCommand command = new SqlCommand(query, connection);
				connection.Open();
				object result = command.ExecuteScalar();
				if (result != null)
				{
					int lastId = (int)result;
					nextId = (lastId + 1).ToString();
				}
			}
			return nextId;
		}

		private void Upload_Click(object sender, RoutedEventArgs e)
		{
			Button button = sender as Button;
			string documentType = button.Tag.ToString();

			OpenFileDialog openFileDialog = new OpenFileDialog
			{
				Filter = "Image Files (*.png;*.jpg;*.jpeg)|*.png;*.jpg;*.jpeg;"
			};

			if (openFileDialog.ShowDialog() == true)
			{
				string sourceFilePath = openFileDialog.FileName;
				string targetFilePath = System.IO.Path.Combine(imagesFolderPath, System.IO.Path.GetFileName(sourceFilePath));

				try
				{
					File.Copy(sourceFilePath, targetFilePath, true);

					UpdateDocumentImage(documentType, targetFilePath);

					// Update the Image control in XAML
					UpdateImageControl(documentType, targetFilePath);
				}
				catch (Exception ex)
				{
					MessageBox.Show($"Error uploading file: {ex.Message}");
				}
			}
		}

		private void UpdateDocumentImage(string documentType, string filePath)
		{
			switch (documentType)
			{
				case "LeadPicture":
					Picture.Source = new BitmapImage(new Uri(filePath, UriKind.Absolute));
					document.Picture = filePath;
					break;
				case "BirthCert":
					Birth_Certificate.Source = new BitmapImage(new Uri(filePath, UriKind.Absolute));
					document.Birth_Certificate = filePath;
					break;
				case "GoodMoral":
					Good_Moral.Source = new BitmapImage(new Uri(filePath, UriKind.Absolute));
					document.Good_Moral = filePath;
					break;
				case "TOR":
					TOR.Source = new BitmapImage(new Uri(filePath, UriKind.Absolute));
					document.TOR = filePath;
					break;
				case "MedClear":
					Medical_Clearance.Source = new BitmapImage(new Uri(filePath, UriKind.Absolute));
					document.Medical_Clearance = filePath;
					break;
				case "RepCard":
					Report_Card.Source = new BitmapImage(new Uri(filePath, UriKind.Absolute));
					document.Report_Card = filePath;
					break;
			}
		}

		private void UpdateImageControl(string documentType, string filePath)
		{
			switch (documentType)
			{
				case "LeadPicture":
					Picture.Source = new BitmapImage(new Uri(filePath, UriKind.Absolute));
					break;
				case "BirthCert":
					Birth_Certificate.Source = new BitmapImage(new Uri(filePath, UriKind.Absolute));
					break;
				case "GoodMoral":
					Good_Moral.Source = new BitmapImage(new Uri(filePath, UriKind.Absolute));
					break;
				case "TOR":
					TOR.Source = new BitmapImage(new Uri(filePath, UriKind.Absolute));
					break;
				case "MedClear":
					Medical_Clearance.Source = new BitmapImage(new Uri(filePath, UriKind.Absolute));
					break;
				case "RepCard":
					Report_Card.Source = new BitmapImage(new Uri(filePath, UriKind.Absolute));
					break;
				default:
					// Handle unknown document type here
					break;
			}
		}

		private void SaveBtn_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				string picturePath = Picture.Source != null ? SaveDocument(Picture.Source) : null;
				string birthCertificatePath = Birth_Certificate.Source != null ? SaveDocument(Birth_Certificate.Source) : null;
				string goodMoralPath = Good_Moral.Source != null ? SaveDocument(Good_Moral.Source) : null;
				string torPath = TOR.Source != null ? SaveDocument(TOR.Source) : null;
				string medicalClearancePath = Medical_Clearance.Source != null ? SaveDocument(Medical_Clearance.Source) : null;
				string reportCardPath = Report_Card.Source != null ? SaveDocument(Report_Card.Source) : null;

				if (!string.IsNullOrEmpty(picturePath) ||
					!string.IsNullOrEmpty(birthCertificatePath) ||
					!string.IsNullOrEmpty(goodMoralPath) ||
					!string.IsNullOrEmpty(torPath) ||
					!string.IsNullOrEmpty(medicalClearancePath) ||
					!string.IsNullOrEmpty(reportCardPath))
				{
					UpdateDatabase(document.Documents_ID, picturePath, birthCertificatePath, goodMoralPath, torPath, medicalClearancePath, reportCardPath);
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show("An error occurred while saving changes: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}
		private string SaveDocument(ImageSource imageSource)
		{
			if (imageSource is BitmapImage bitmapImage)
			{
				string targetFolderPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, FolderName);
				if (!Directory.Exists(targetFolderPath))
				{
					Directory.CreateDirectory(targetFolderPath);
				}

				string targetFilePath = System.IO.Path.Combine(targetFolderPath, $"{Guid.NewGuid()}.png");
				using (var fileStream = new FileStream(targetFilePath, FileMode.Create))
				{
					BitmapEncoder encoder = new PngBitmapEncoder();
					encoder.Frames.Add(BitmapFrame.Create(bitmapImage));
					encoder.Save(fileStream);
				}

				return targetFilePath;
			}

			return null;
		}
	private void UpdateDatabase(string documentsId, string picturePath, string birthCertificatePath, string goodMoralPath, string torPath, string medicalClearancePath, string reportCardPath)
	{
			try
			{
				string connectionString = "Data Source=DESKTOP-F726TKR\\SQLEXPRESS;Initial Catalog=\"Lead Tracker\";Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False";

				string query = "UPDATE Documents SET " +
							   "Picture = ISNULL(@Picture, Picture), " +
							   "Birth_Certificate = ISNULL(@Birth_Certificate, Birth_Certificate), " +
							   "Good_Moral = ISNULL(@Good_Moral, Good_Moral), " +
							   "TOR = ISNULL(@TOR, TOR), " +
							   "Medical_Clearance = ISNULL(@Medical_Clearance, Medical_Clearance), " +
							   "Report_Card = ISNULL(@Report_Card, Report_Card) " +
							   "WHERE Documents_ID = @Documents_ID;";

				using (SqlConnection connection = new SqlConnection(connectionString))
				{
					SqlCommand command = new SqlCommand(query, connection);
					command.Parameters.AddWithValue("@Documents_ID", documentsId);
					command.Parameters.AddWithValue("@Picture", string.IsNullOrEmpty(picturePath) ? (object)DBNull.Value : picturePath);
					command.Parameters.AddWithValue("@Birth_Certificate", string.IsNullOrEmpty(birthCertificatePath) ? (object)DBNull.Value : birthCertificatePath);
					command.Parameters.AddWithValue("@Good_Moral", string.IsNullOrEmpty(goodMoralPath) ? (object)DBNull.Value : goodMoralPath);
					command.Parameters.AddWithValue("@TOR", string.IsNullOrEmpty(torPath) ? (object)DBNull.Value : torPath);
					command.Parameters.AddWithValue("@Medical_Clearance", string.IsNullOrEmpty(medicalClearancePath) ? (object)DBNull.Value : medicalClearancePath);
					command.Parameters.AddWithValue("@Report_Card", string.IsNullOrEmpty(reportCardPath) ? (object)DBNull.Value : reportCardPath);

					connection.Open();
					command.ExecuteNonQuery();
				}

				MessageBox.Show("Changes saved successfully.", "Save Successful", MessageBoxButton.OK, MessageBoxImage.Information);
			}
			catch (Exception ex)
			{
				MessageBox.Show("An error occurred while updating the database: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
			}
	}
		private void Camera_Click(object sender, RoutedEventArgs e)
		{
			Button button = sender as Button;
			string documentType = button.Tag.ToString();

			Camera cam = new Camera();
			if (cam.ShowDialog() == true)
			{
				string capturedImagePath = cam.CapturedImagePath;
				if (!string.IsNullOrEmpty(capturedImagePath))
				{
					UpdateDocumentImage(documentType, capturedImagePath);
				}
			}
		}

        private void backButton_Click(object sender, RoutedEventArgs e)
        {
			this.Close();
        }
    }
}
