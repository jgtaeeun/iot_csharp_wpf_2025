using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeritageApp.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
      
        private object currentViewModel;
        public object CurrentViewModel
        {
            get => currentViewModel;
            set => SetProperty(ref currentViewModel, value);    
        }
      

        public MainViewModel()
        {
            ShowView1();
        }

        [RelayCommand]
        private void ShowView1()
        {
            CurrentViewModel = new TodayViewModel();
        }

        [RelayCommand]
        private void ShowView2()
        {
            CurrentViewModel = new LocationViewModel();
        }

    }
}
