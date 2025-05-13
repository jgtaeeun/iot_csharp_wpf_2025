using CommunityToolkit.Mvvm.ComponentModel;
using MahApps.Metro.Controls.Dialogs;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using WpfBookRentalShop01.Helpers;
using WpfBookRentalShop01.Models;

namespace WpfBookRentalShop01.ViewModels
{


    public partial class BooksViewModel : ObservableObject
    {
        //db에서 읽어온 데이터 저장할 공간
        private ObservableCollection<Book> _bookList;

        public ObservableCollection<Book> BookList
        {
            get => _bookList;
            set => SetProperty(ref _bookList, value);
        }



        // 메세지박스대신에 다이얼로그로 표현하기 위해서
        private IDialogCoordinator _dialogCoordinator;

        //디자인 타임에서도 사용할 수 있도록 기본 생성자 오버로드를 추가
        public BooksViewModel() : this(DialogCoordinator.Instance) { }

        public BooksViewModel(IDialogCoordinator coordinator)
        {   this._dialogCoordinator = coordinator;
            LoadGridFromDb();
        }

        private void LoadGridFromDb()
        {

            //string connectionString = "Server=localhost;Database=madang;Uid=root;Pwd=12345;Charset=utf8";

            string query = "SELECT Idx,Author,Division,Names,ReleaseDate,ISBN,Price FROM bookstbl";
            ObservableCollection<Book> books = new ObservableCollection<Book>();

            //3. db연결, 명령, 리더
            using (MySqlConnection conn = new MySqlConnection(Common.CONNSTR))
            {
                try
                {
                    conn.Open();
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    MySqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        var idx = reader.GetInt32("Idx");
                        var author = reader.GetString("Author");
                        var division = reader.GetString("Division");
                        var names = reader.GetString("Names"); 
                        var releaseDate = reader.GetDateTime("ReleaseDate");
                        var isbn = reader.GetString("ISBN");
                        var price = reader.GetInt32("Price");

                        books.Add(new Book { Division = division,
                            DNames = names ,
                            Idx = idx,
                            Author = author,
                            ReleaseDate = releaseDate,
                            ISBN = isbn,
                            Price = price,
                        });

                    }
                }
                catch (MySqlException ex)
                {
                    MessageBox.Show("DB 오류 발생: " + ex.Message);
                }

                BookList = books;

            }
        }
    }
}
