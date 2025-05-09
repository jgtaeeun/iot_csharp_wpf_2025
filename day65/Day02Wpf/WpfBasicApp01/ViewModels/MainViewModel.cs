using Caliburn.Micro;
using MahApps.Metro.Controls.Dialogs;
using System.Windows;

namespace WpfBasicApp01.ViewModels
{
    public class MainViewModel : Conductor<object>
    {
        public string _greeting;

        //다이얼로그를 위한 변수
        private readonly IDialogCoordinator _dialogCoordinator;

        public string Greeting 
        
        { get => _greeting;
            set 
            {
                _greeting = value;
                NotifyOfPropertyChange(() => Greeting);
            }
        
        }

        public MainViewModel(IDialogCoordinator dialogCoordinator) 
        {
            _dialogCoordinator = dialogCoordinator;
            Greeting = "Hello Caliburn Micro";
        }
        
        public async void SayMyName()
        {
            Greeting = "abcdefghijk";
            await _dialogCoordinator.ShowMessageAsync(this, "Greeting", "adcdefghijk");
        }
    }
}
