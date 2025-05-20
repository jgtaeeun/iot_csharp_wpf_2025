using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MahApps.Metro.Controls.Dialogs;
using MQTTnet;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using Org.BouncyCastle.Tls;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;
using System.Windows.Threading;
using WpfMqttSubApp.Models;
using ZstdSharp.Unsafe;

namespace WpfMqttSubApp.ViewModels
{
    public partial class MainViewModel : ObservableObject , IDisposable
    {
        //타이머
        private readonly DispatcherTimer _timer;
       
        //디버깅 확인 위한 테스트값
        private int LineCounter = 1; 

        //mqtt
        private IMqttClient _mqttClient;

        //MahApps.Metro 다이얼로그 코디네이터
        private readonly IDialogCoordinator dialogCoordinator;

        //db주소
        private string _dbHost;
        public string DBHost
        {
            get => _dbHost;
            set => SetProperty(ref _dbHost, value);
        }

        //mqtt 주소
        private string _brokerHost;
        public string BrokerHost
        {
            get => _brokerHost;
            set => SetProperty(ref _brokerHost, value);
        }

        //richboxtext에 값
        private string _logText;
        public string LogText
        {
            get => _logText;
            set => SetProperty(ref _logText, value);    
        }

        //db연결 conString
        private string _connString = string.Empty;
        private MySqlConnection connection;


        public MainViewModel(IDialogCoordinator coordiantor)
        {
            this.dialogCoordinator = coordiantor;
            BrokerHost = "210.119.12.110";
            DBHost = "210.119.12.110";
            connection = new MySqlConnection();
            ////RichTextBox 테스트용 코드
            //_timer = new DispatcherTimer();
            //_timer.Interval = TimeSpan.FromSeconds(1);
            //_timer.Tick += (sender, e) =>
            //{
            //    //richTextBox 추가내용
            //    LogText += $"Log[{DateTime.Now:HH:mm:ss}]-[{LineCounter++}]\n";

            //};
            //_timer.Start();

        }

        [RelayCommand]
        public async Task ConnectMqtt ()
        {
            if (string.IsNullOrEmpty(BrokerHost))
            {
                await this.dialogCoordinator.ShowMessageAsync(this, "브로커연결합니다.", "브로커연결주소없음");
                return;
            }

            //mqtt 브로커에 접속해서 데이터를 가져오기
            ConnectMqttBroker();
        }

        private async Task ConnectMqttBroker()
        {   //mqtt 클라이언트 생성
            var mqttFactory = new MqttClientFactory();
            _mqttClient = mqttFactory.CreateMqttClient();

            //matt 클라이언트 접속 설정
            var mqttClientOptions = new MqttClientOptionsBuilder()
                .WithTcpServer(BrokerHost)
                .WithCleanSession(true)
                .Build();

            //matt 접속 후 이벤트 처리
            _mqttClient.ConnectedAsync += async e =>
            {
                LogText += "MQTT Broker 연결성공\n";

                //연결이후 구독(subscribe)
                await _mqttClient.SubscribeAsync("smarthome/110/topic");
            };

            _mqttClient.ApplicationMessageReceivedAsync += e =>
            {
                var topic = e.ApplicationMessage.Topic;
                var payload = e.ApplicationMessage.ConvertPayloadToString(); //byte데이터를 utf-8문자열로 변환
            
                //json으로 변경하여 db에저장하기 위한 과정
                var data = JsonConvert.DeserializeObject<SensorInfo>(payload);
                // Debug.WriteLine($"{data.COUNT} /{data.SENSING_DT}/{data.HUMID}/ {data.LIGHT}");
                SaveSensingData(data);

                //richtextbox에 데이터 띄우기
                LogText += $"LineNumber:{LineCounter++}\n";
                LogText += $"{payload}\n";

                return Task.CompletedTask;
            };

            await _mqttClient.ConnectAsync(mqttClientOptions);
        }


        private async Task SaveSensingData(SensorInfo data)
        {
            string query = "INSERT INTO fake_datas VALUES (@SENSING_DT, @PUB_ID,@COUNT,@TEMP ,@HUMID,@RAIN,@PERSON,@LIGHT)";
            //Debug.WriteLine(connection.State);
            //Debug.WriteLine(System.Data.ConnectionState.Open);
            if (connection.State == System.Data.ConnectionState.Open)
                {
                    using var cmd = new MySqlCommand(query, connection);
                    cmd.Parameters.AddWithValue("@SENSING_DT", data.SENSING_DT);
                    cmd.Parameters.AddWithValue("@PUB_ID", data.PUB_ID);
                    cmd.Parameters.AddWithValue("@COUNT", data.COUNT);
                    cmd.Parameters.AddWithValue("@TEMP", data.TEMP);
                    cmd.Parameters.AddWithValue("@HUMID", data.HUMID);
                    cmd.Parameters.AddWithValue("@RAIN", data.RAIN);
                    cmd.Parameters.AddWithValue("@PERSON", data.PERSON);
                    cmd.Parameters.AddWithValue("@LIGHT", data.LIGHT);

                    await cmd.ExecuteNonQueryAsync();
                }
            
            
            
        }

        [RelayCommand]
        public async Task ConnectDB()
        {
            if (string.IsNullOrEmpty(DBHost))
            {
                await this.dialogCoordinator.ShowMessageAsync(this, "db연결합니다.", "db연결실패");
                return;
            }
            _connString = $"Server={DBHost};Database=smarthome;Uid=root;Pwd=12345;Charset=utf8";
            await ConnectDatabaseServer();
          
        }

        private async Task ConnectDatabaseServer()
        {
            try
            {
                connection = new MySqlConnection(_connString);
                connection.Open();
                LogText += $"{DBHost} DB서버 접속 성공 ! {connection.State}\n";

               //db에서 데이터 불러오기
            }
            catch (Exception ex)
            {
                LogText += $"{DBHost} DB서버 접속 실패 :{ex.Message}\n";
            }
        }


        public void Dispose()
        {   //리소스 해제 
            connection?.Dispose();
        }
    }
}
