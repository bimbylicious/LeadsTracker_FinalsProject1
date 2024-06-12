using System;
using System.Collections.Generic;
using System.Data.SqlClient;
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
    /// Interaction logic for Documents.xaml
    /// </summary>
    public partial class Documents : Window
    {
        public Documents(Document document)
        {
            InitializeComponent();
            LoadDocumentImages(document);
        }

        private void LoadDocumentImages(Document document)
        {
            if (!string.IsNullOrEmpty(document.Picture))
            {
                Picture.Source = new BitmapImage(new Uri(document.Picture, UriKind.Absolute));
            }
            if (!string.IsNullOrEmpty(document.Birth_Certificate))
            {
                Birth_Certificate.Source = new BitmapImage(new Uri(document.Birth_Certificate, UriKind.Absolute));
            }
            if (!string.IsNullOrEmpty(document.Good_Moral))
            {
                Good_Moral.Source = new BitmapImage(new Uri(document.Good_Moral, UriKind.Absolute));
            }
            if (!string.IsNullOrEmpty(document.TOR))
            {
                TOR.Source = new BitmapImage(new Uri(document.TOR, UriKind.Absolute));
            }
            if (!string.IsNullOrEmpty(document.Medical_Clearance))
            {
                Medical_Clearance.Source = new BitmapImage(new Uri(document.Medical_Clearance, UriKind.Absolute));
            }
            if (!string.IsNullOrEmpty(document.Report_Card))
            {
                Report_Card.Source = new BitmapImage(new Uri(document.Report_Card, UriKind.Absolute));
            }
        }
    }
}
