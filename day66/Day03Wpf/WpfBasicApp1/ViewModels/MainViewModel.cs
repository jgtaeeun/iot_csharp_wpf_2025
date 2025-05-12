using CommunityToolkit.Mvvm.ComponentModel;
using Google.Protobuf.WellKnownTypes;
using MySql.Data.MySqlClient;
using NLog;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using WpfBasicApp1.Models;
using ZstdSharp.Unsafe;

namespace WpfBasicApp1.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        #region communityToolkit 학습
        //멤버변수1
        private string _greeting;

        //속성1
        public string Greeting 
        { get => _greeting; 
            
          set => SetProperty(ref _greeting, value); //CommunityToolkit.Mvvm의 핵심
        
        }

        //현재시간 멤버변수2
        private string _currentTime;
       

        //속성2
        public string CurrentTime 
        {   get => _currentTime; 
            set => SetProperty(ref _currentTime, value); 
        }

        //타이머 멤버변수3
        private readonly DispatcherTimer _timer;

        private void _timer_Tick(object? sender, EventArgs e)
        {
            CurrentTime = DateTime.Now.ToString();
            Debug.WriteLine(CurrentTime);
            _logger.Info(CurrentTime);
        }

        //Nlog 객체 생성
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();
        #endregion

        #region 생성자
        public MainViewModel()
        {
            //로그
            _logger.Info("뷰모델 시작");

            // community프레임 학습-속성호출
            _greeting = "MainViewModel 생성자호출ㅡCommunity Frame 학습";

            CurrentTime = DateTime.Now.ToString();
            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromSeconds(1);  //1초마다 변경
            _timer.Tick += _timer_Tick;
            _timer.Start();

            //db연동
            LoadControlFromDb();
            LoadGridFromDb();

           
        }


        #endregion

        #region db연동
        private ObservableCollection<KeyValuePair<string, string>> _divisions;
        public ObservableCollection<KeyValuePair<string, string>> Divisions 
        { get=> _divisions;
            set=>SetProperty(ref _divisions, value);
        }

        private ObservableCollection<Book> _book;
        public ObservableCollection<Book> Books 
        { 
            get=> _book;
            set => SetProperty(ref _book, value);
        }
      
        
        private Book _selectedBook;
        public Book SelectedBook
        {

            get => _selectedBook;
            set => SetProperty(ref _selectedBook, value);
            

        }


        //그룹박스의 콤보박스에 아이템 넣기 위해서
        private void LoadControlFromDb()
        {
            //1. db연결문자열
            string connectionString = "Server=localhost;Database=madang;Uid=root;Pwd=12345;Charset=utf8";
            //2. 사용쿼리
            string query = "SELECT Division,Names FROM divtbl";

            ObservableCollection<KeyValuePair<string, string>> divisions = new ObservableCollection<KeyValuePair<string, string>>();

            //3. db연결, 명령, 리더
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    MySqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        var division = reader.GetString("Division");
                        var name = reader.GetString("Names");
                        divisions.Add(new KeyValuePair<string, string>(division, name));

                    }
                }
                catch (MySqlException ex)
                {

                }

                Divisions = divisions;
                
            }
        }

        // DATAGRID 컨트롤에 로드되는 데이터
        private void LoadGridFromDb()
        {
            string connectionString = "Server=localhost;Database=madang;Uid=root;Pwd=12345;Charset=utf8";
            string query = "SELECT b.Idx,b.Author,b.Division ,b.Names, b.ReleaseDate,b.ISBN,b.Price, d.Names as dNames FROM bookstbl b, divtbl d WHERE b.Division = d.Division order by b.Idx";

            ObservableCollection<Book> books = new ObservableCollection<Book>();


            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    MySqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        books.Add(new Book
                        {
                            Idx = reader.GetInt32("Idx"),
                            Division = reader.GetString("Division"),
                            Author = reader.GetString("Author"),
                            Names = reader.GetString("Names"),
                            DNames = reader.GetString("DNames"),
                            ISBN = reader.GetString("ISBN"),
                            Price = reader.GetInt32("Price"),
                            ReleaseDate = reader.GetDateTime("ReleaseDate")
                        });

                    }


                }
                catch (MySqlException ex)
                {

                }

                Books = books;
               

            }
        }

        #endregion
    }
}
