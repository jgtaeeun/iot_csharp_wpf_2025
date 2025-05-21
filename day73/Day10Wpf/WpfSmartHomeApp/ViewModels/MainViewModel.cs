using Accessibility;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using K4os.Compression.LZ4.Streams.Adapters;
using MQTTnet;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Common;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices.JavaScript;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using WpfSmartHomeApp.Helpers;
using WpfSmartHomeApp.Models;

namespace WpfSmartHomeApp.ViewModels
{
    public partial class MainViewModel : ObservableObject , IDisposable
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


        // 실시간 데이터 값 구독해오기 위한 준비
        //readonly는 생성자에서만 값 할당할 경우, 적음
        private MySqlConnection connection;

        //TOPIC 
        private  string TOPIC;
        
        private IMqttClient mqttClient;

        private string BROKERHOST;

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
        public async Task OnLoaded()
        {
            BROKERHOST = "210.119.12.52";
            connection = new MySqlConnection();
            TOPIC = "pknu/sh01/data";


            //mqtt 클라이언트 생성
            var mqttFactory = new MqttClientFactory();
            mqttClient = mqttFactory.CreateMqttClient();

            //matt 클라이언트 접속 설정
            var mqttClientOptions = new MqttClientOptionsBuilder()
                .WithTcpServer(BROKERHOST)
                .WithCleanSession(true)
                .Build();

            //mqtt 접속 후 이벤트 처리 메서드 선언 
            mqttClient.ConnectedAsync += MqttClient_ConnectedAsync;

            //mqtt 구독 메시지 확인 메서드 선언
            mqttClient.ApplicationMessageReceivedAsync += MqttClient_ApplicationMessageReceivedAsync;

            await mqttClient.ConnectAsync(mqttClientOptions);

        }
        private async Task MqttClient_ConnectedAsync(MqttClientConnectedEventArgs arg)
        {
            Common.LOGGER.Info($"{arg}");
            Common.LOGGER.Info("MQTT Borker 접속 성공!!");
            //연결이후 구독(subscribe)
            await mqttClient.SubscribeAsync(TOPIC);
            
        }


        private  Task MqttClient_ApplicationMessageReceivedAsync(MqttApplicationMessageReceivedEventArgs arg)
        {

            var topic = arg.ApplicationMessage.Topic;
            var payload = arg.ApplicationMessage.ConvertPayloadToString(); //byte데이터를 utf-8문자열로 변환

            // json으로 변경하여 db에저장하기 위한 과정
            var data = JsonConvert.DeserializeObject<SensingInfo>(payload);
            //Common.LOGGER.Info($"|Light:{data.L}|Rain:{data.R}|Temp:{data.T}|Humid:{data.H}|Fan:{data.F}|Detect:{data.V}|{data.RL}|{data.CB}|");



            HomeTemp = data.T;
            HomeHumid = data.H;

            IsDetectOn = data.V == "ON" ? true : false;
            DetectResult = IsDetectOn ? "Dectection State!!" : "Normal State";

            IsConditionerOn = data.F == "ON" ? true : false;
            ConditionerResult = IsConditionerOn ? "AirCon On!!" : "AirCon Off";

            IsLightOn = data.RL ==  "ON" ? true : false;
            LightResult = IsLightOn ? "Light On!!" : "Light Off";

            IsRainOn = data.R <= 350 ? true : false;
            RainResult = IsRainOn ? "Rain!!" : "No Rain";

            // 구독 종료 알림
            return Task.CompletedTask;
        }


        public void Dispose()
        {
            connection?.Close();
        }
    }
}
