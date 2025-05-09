using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfBasicApp2.Model;

namespace WpfBasicApp2.ViewModel
{
    public class MainViewModel : INotifyPropertyChanged
    {
        //ObservableCollection ->리스트의 변형(변화를 감지할 수 있도록 처리된 클래스>
        // DataAdapter로 받은 DataTable 역할, DataGrid에 넣을 데이터
        public ObservableCollection<Book> Books { get; set; }   

        //List<KeyValuePair<string, string>> divisions 의 변형, GroupBox의 콤보박스에 넣을 데이터
        public ObservableCollection<KeyValuePair<string, string>> Divisions { get; set; }


        //선택된 값에 대한 멤버변수, 멤버변수는 _를 붙이거나, 소문자로 변수명을 시작
        public Book _selectedBook;

        public Book SelectedBook
        {
            //람다식 표현, get{ return _selectedBook; }와 동일
            get => _selectedBook;
            
            set
            {
                _selectedBook = value;
                //값이 변경된 것을 알아차리도록 해줘야함!!
                OnPropertyChanged(nameof(SelectedBook));
            }
        }

        public MainViewModel()
        {
            LoadControlFromDb();
            LoadGridFromDb();
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
                OnPropertyChanged(nameof(Divisions));
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
                            ReleaseDate= reader.GetDateTime("ReleaseDate")
                        });

                    }


                }
                catch (MySqlException ex)
                {

                }

                Books = books;
                OnPropertyChanged(nameof (Books));

            }
        }

        // PropertyChanged 이벤트는.NET의 INotifyPropertyChanged 인터페이스에서 사용되는 이벤트입니다.
        public event PropertyChangedEventHandler? PropertyChanged;

        //OnPropertyChanged는 이 이벤트를 발생시키는 도우미 메서드입니다.
        protected void OnPropertyChanged(string propertyName)
        {   //기본적인 이벤트핸들러 파라미터와 동일(Object sender, EventArgs e)
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));        
        }
    }
}
