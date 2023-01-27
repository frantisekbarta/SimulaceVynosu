using System.Windows;

namespace SimulaceVynosu
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class SimulaceVynosuView : Window
    {
        public SimulaceVynosuView()
        {
            InitializeComponent();
            SimulaceVynosuViewModel simulaceVynosuViewModel = new SimulaceVynosuViewModel();
            DataContext = simulaceVynosuViewModel;
        }
    }
}
