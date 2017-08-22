using System.Windows;
using BluetoothSPPServer;

namespace BluetoothServerGUI
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            BTServer server = new BTServer();
            this.DataContext = server;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

            BluetoothServerGUI.kasseAndreasDataSet kasseAndreasDataSet = ((BluetoothServerGUI.kasseAndreasDataSet)(this.FindResource("kasseAndreasDataSet")));
            // Lädt Daten in Tabelle "stock_articles". Sie können diesen Code nach Bedarf ändern.
            BluetoothServerGUI.kasseAndreasDataSetTableAdapters.stock_articlesTableAdapter kasseAndreasDataSetstock_articlesTableAdapter = new BluetoothServerGUI.kasseAndreasDataSetTableAdapters.stock_articlesTableAdapter();
            kasseAndreasDataSetstock_articlesTableAdapter.Fill(kasseAndreasDataSet.stock_articles);
            System.Windows.Data.CollectionViewSource stock_articlesViewSource = ((System.Windows.Data.CollectionViewSource)(this.FindResource("stock_articlesViewSource")));
            stock_articlesViewSource.View.MoveCurrentToFirst();
        }
    }
}
