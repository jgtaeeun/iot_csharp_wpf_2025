using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
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

        public MainViewModel()
        {
            Message = "책랜탈샵 화면입니다.";
        }

        [RelayCommand]
        public  void AppExit()
        {
            MessageBox.Show("종료합니다.");
        }

        [RelayCommand]
        public void ShowBookGenre()
        {
            //MessageBox.Show("책장르 관리");
            var vm = new BookGenreViewModel();
            var v = new BookGenreView { DataContext = vm };
            CurrentView = v;
            CurrentStatus = "책장르 관리";
        }

        [RelayCommand]
        public void ShowBooks()
        {
            //MessageBox.Show("책 관리");
            var vm = new BooksViewModel();
            var v = new BooksView { DataContext = vm };
            CurrentView = v;
            CurrentStatus = "책 관리";
        }
    }

}

