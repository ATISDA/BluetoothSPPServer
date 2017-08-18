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
            DataContext = server;
        }
    }
}
