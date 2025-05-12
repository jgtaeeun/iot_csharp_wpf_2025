using System.Configuration;
using System.Data;
using System.Runtime.Serialization.DataContracts;
using System.Windows;
using WpfBasicApp1.ViewModels;
using WpfBasicApp1.Views;

namespace WpfBasicApp1
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            var viewModel = new MainViewModel();
            var view = new MainView
            {
                DataContext = viewModel,
            };
            view.ShowDialog();
        }
    }

}
