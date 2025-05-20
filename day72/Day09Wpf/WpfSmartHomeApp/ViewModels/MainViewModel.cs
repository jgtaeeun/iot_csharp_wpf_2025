using Accessibility;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace WpfSmartHomeApp.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {   //온도
        private double _homeTemp;
        public double HomeTemp
        {
            get => _homeTemp;
            set => SetProperty(ref _homeTemp, value);
        }

        //습도
        private double _homeHumid;
      
        public double HomeHumid
        {
            get => _homeHumid;
            set => SetProperty(ref _homeHumid, value);
        }


        //사람감지여부
        private bool _isDetectOn;

        public Boolean  IsDetectOn
        {
            get => _isDetectOn;
            set => SetProperty(ref _isDetectOn, value);
        }
        //사람감지여부 결과
        private string _detectResult;

        public string DetectResult
        {
            get => _detectResult;
            set => SetProperty(ref _detectResult, value);
        }

        //비감지 여부

        private bool _isRainOn;

        public Boolean IsRainOn
        {
            get => _isRainOn;
            set => SetProperty(ref _isRainOn, value);
        }
        //비 감지여부 결과
        private string _rainResult;

        public string RainResult
        {
            get => _rainResult;
            set => SetProperty(ref _rainResult, value);
        }

        //에어컨

        private bool _isConditionerOn;

        public Boolean IsConditionerOn
        {
            get => _isConditionerOn;
            set => SetProperty(ref _isConditionerOn, value);
        }


        private string _conditionerResult;

        public string ConditionerResult
        {
            get => _conditionerResult;
            set => SetProperty(ref _conditionerResult, value);
        }
        //빛

        private bool _isLightOn;

        public Boolean IsLightOn
        {
            get => _isLightOn;
            set => SetProperty(ref _isLightOn, value);
        }
        private string _lightResult;

        public string LightResult
        {
            get => _lightResult;
            set => SetProperty(ref _lightResult, value);
        }


        //현재 시간
        private string _currDatetime;
        public string CurrDatetime
        {
            get => _currDatetime;
            set => SetProperty(ref _currDatetime, value);
        }

        //타이머
        private readonly DispatcherTimer _timer;

        //생성자
        public MainViewModel()
        {
            //상태바 시계
            CurrDatetime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");   //최초 화면 시계
            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromSeconds(1);
            _timer.Tick += (sender, e) =>
            {
                CurrDatetime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            };
            _timer.Start();

        }


        [RelayCommand]
        public void OnLoaded()
        {

            HomeTemp = 30;
            HomeHumid = 43.2;
         
            DetectResult = "Detected Human!";
            IsDetectOn = true;
            RainResult = "Raining";
            IsRainOn = true;
            ConditionerResult = "Aircon On!";
            IsConditionerOn = true;
            LightResult = "Light On!";
            IsLightOn = true;


        }
    }
}
