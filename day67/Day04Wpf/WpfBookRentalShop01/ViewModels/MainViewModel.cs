using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MahApps.Metro.Controls.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using WpfBookRentalShop01.Helpers;
using WpfBookRentalShop01.Views;

namespace WpfBookRentalShop01.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {

        private string _message;
        public string Message
        {
            get => _message;
            set => SetProperty(ref _message, value);
        }

        //현재 뷰
        private UserControl _currentView;
        public UserControl CurrentView
        {
            get => _currentView;
            set => SetProperty(ref _currentView, value);
        
        }

        //상태바
        private string _currentStatus;
        public string CurrentStatus
        {
            get => _currentStatus;
            set =>SetProperty(ref _currentStatus, value);
        }


        // 메세지박스대신에 다이얼로그로 표현하기 위해서
        private IDialogCoordinator _dialogCoordinator;

        public MainViewModel(IDialogCoordinator coordinator)
        {
            this._dialogCoordinator = coordinator;
            Message = "책랜탈샵 화면입니다.";
            Common.LOGGER.Info("책랜탈샵 화면-MainViewModel");
        }

        [RelayCommand]
        public async Task AppExit()
        {
            //MessageBox.Show("종료합니다.");
            //await this._dialogCoordinator.ShowMessageAsync(this, "종료합니다.", "메시지");
            var result = await this._dialogCoordinator.ShowMessageAsync(this, "종료확인", "종료하시겠습니까?", MessageDialogStyle.AffirmativeAndNegative);
            if (result == MessageDialogResult.Affirmative) //종료 ok
            {
                Common.LOGGER.Info("종료");
                Application.Current.Shutdown();  
                
            }
            else
            {
                return;
            }
            
        }

        [RelayCommand]
        public void ShowBookGenre()
        {
            //MessageBox.Show("책장르 관리");
            var vm = new BookGenreViewModel(Common.DIALOGCOORDINATOR);
            var v = new BookGenreView { DataContext = vm };
            CurrentView = v;
            CurrentStatus = "책장르 관리";
            Common.LOGGER.Info("책장르 관리");
        }

        [RelayCommand]
        public void ShowBooks()
        {
            //MessageBox.Show("책 관리");
            var vm = new BooksViewModel(Common.DIALOGCOORDINATOR);
            var v = new BooksView { DataContext = vm };
            CurrentView = v;
            CurrentStatus = "책 관리";
            Common.LOGGER.Info("책 관리");
        }
    }

}

