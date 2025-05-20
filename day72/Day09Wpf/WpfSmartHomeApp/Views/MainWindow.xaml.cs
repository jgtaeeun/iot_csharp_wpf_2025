using System.Windows;
using System.Windows.Input;
using WpfSmartHomeApp.ViewModels;

namespace WpfSmartHomeApp.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

     

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown(); //종료버튼
        }

        private void window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            //e.Cancel = true;로 설정하면, 닫기 이벤트가 취소되어 창이 닫히지 않습니다.
            //주로 사용자에게 "정말 종료하시겠습니까?" 같은 확인 메시지를 보여줄 때 사용됩니다.
            e.Cancel = true;
        }

        private void window_Loaded(object sender, RoutedEventArgs e)
        {
            if (DataContext is MainViewModel vm)
            {
                vm.LoadedCommand.Execute(null); 
            }
        }

     
    }
}