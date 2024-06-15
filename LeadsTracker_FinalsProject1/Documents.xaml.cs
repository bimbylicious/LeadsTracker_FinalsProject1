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
		public Documents(Document document)
		{
			InitializeComponent();
			if (document != null)
			{
				LoadDocumentImages(document);
			}
		}

		private void LoadDocumentImages(Document document)
		{
			if (document != null)
			{
				Picture.Source = !string.IsNullOrEmpty(document.Picture)
					? new BitmapImage(new Uri(document.Picture, UriKind.Absolute))
					: null;

				Birth_Certificate.Source = !string.IsNullOrEmpty(document.Birth_Certificate)
					? new BitmapImage(new Uri(document.Birth_Certificate, UriKind.Absolute))
					: null;

				Good_Moral.Source = !string.IsNullOrEmpty(document.Good_Moral)
					? new BitmapImage(new Uri(document.Good_Moral, UriKind.Absolute))
					: null;

				TOR.Source = !string.IsNullOrEmpty(document.TOR)
					? new BitmapImage(new Uri(document.TOR, UriKind.Absolute))
					: null;

				Medical_Clearance.Source = !string.IsNullOrEmpty(document.Medical_Clearance)
					? new BitmapImage(new Uri(document.Medical_Clearance, UriKind.Absolute))
					: null;

				Report_Card.Source = !string.IsNullOrEmpty(document.Report_Card)
					? new BitmapImage(new Uri(document.Report_Card, UriKind.Absolute))
					: null;
			}
		}

		private void Upload_Click(object sender, RoutedEventArgs e)
		{
			Button button = sender as Button;
			string documentType = button.Tag.ToString();

			OpenFileDialog openFileDialog = new OpenFileDialog
			{
				Filter = "Image Files (*.png;*.jpg;*.jpeg;*.gif;*.bmp)|*.png;*.jpg;*.jpeg;*.gif;*.bmp"
			};

			if (openFileDialog.ShowDialog() == true)
			{
				string sourceFilePath = openFileDialog.FileName;
				string targetFolderPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, FolderName);

				// Create the target folder if it doesn't exist
				if (!Directory.Exists(targetFolderPath))
				{
					Directory.CreateDirectory(targetFolderPath);
				}

				string targetFilePath = System.IO.Path.Combine(targetFolderPath, System.IO.Path.GetFileName(sourceFilePath));

				File.Copy(sourceFilePath, targetFilePath, true);

				// Update the corresponding Image control with the new file
				UpdateDocumentImage(documentType, targetFilePath);
			}
		}

		private void Camera_Click(object sender, RoutedEventArgs e)
		{
			//Button button = sender as Button;
			//string documentType = button.Tag.ToString();

			//var dialog = new Microsoft.Win32.OpenFileDialog();
			//dialog.Filter = "Image Files (*.png;*.jpg;*.jpeg;*.gif;*.bmp)|*.png;*.jpg;*.jpeg;*.gif;*.bmp";

			//if (dialog.ShowDialog() == true)
			//{
			//	string sourceFilePath = dialog.FileName;
			//	string targetFolderPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, FolderName);

			//	if (!Directory.Exists(targetFolderPath))
			//	{
			//		Directory.CreateDirectory(targetFolderPath);
			//	}

			//	string targetFilePath = System.IO.Path.Combine(targetFolderPath, $"{documentType}_captured_image.png");
			//	File.Copy(sourceFilePath, targetFilePath, true);

			//	UpdateDocumentImage(documentType, targetFilePath);
			//}
		}
		private void UpdateDocumentImage(string documentType, string filePath)
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
			}
		}

		private void SaveBtn_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				// Save the images and get their file paths
				string picturePath = Picture.Source != null ? SaveDocument(Picture.Source) : null;
				string birthCertificatePath = Birth_Certificate.Source != null ? SaveDocument(Birth_Certificate.Source) : null;
				string goodMoralPath = Good_Moral.Source != null ? SaveDocument(Good_Moral.Source) : null;
				string torPath = TOR.Source != null ? SaveDocument(TOR.Source) : null;
				string medicalClearancePath = Medical_Clearance.Source != null ? SaveDocument(Medical_Clearance.Source) : null;
				string reportCardPath = Report_Card.Source != null ? SaveDocument(Report_Card.Source) : null;

				// Update the database with the file paths for non-null documents
				if (!string.IsNullOrEmpty(picturePath) ||
					!string.IsNullOrEmpty(birthCertificatePath) ||
					!string.IsNullOrEmpty(goodMoralPath) ||
					!string.IsNullOrEmpty(torPath) ||
					!string.IsNullOrEmpty(medicalClearancePath) ||
					!string.IsNullOrEmpty(reportCardPath))
				{
					UpdateDatabase(document.Documents_ID, picturePath, birthCertificatePath, goodMoralPath, torPath, medicalClearancePath, reportCardPath);
					MessageBox.Show("Changes saved successfully.", "Save Successful", MessageBoxButton.OK, MessageBoxImage.Information);
				}
				else
				{
					MessageBox.Show("No changes to save.", "Save Information", MessageBoxButton.OK, MessageBoxImage.Information);
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
					BitmapEncoder encoder = new PngBitmapEncoder(); // You can change the encoder based on your image format needs
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
				string connectionString = "Your_Connection_String_Here";

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
	}
}
