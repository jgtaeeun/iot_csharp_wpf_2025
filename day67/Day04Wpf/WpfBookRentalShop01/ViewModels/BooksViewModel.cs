﻿using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
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
        //상세보기

        private bool _isUpdate;

        // 상세보기
        private Book _selectedBook;
        public Book SelectedBook
        {
            get => _selectedBook;
            set
            {
                SetProperty(ref _selectedBook, value);
                _isUpdate = true;
            }
        }

      
        //db에서 읽어온 데이터 저장할 공간
        private ObservableCollection<Book> _bookList;

        public ObservableCollection<Book> BookList
        {
            get => _bookList;
            set => SetProperty(ref _bookList, value);
        }

        //콤보박스 데이터
        private ObservableCollection<KeyValuePair<string, string>> _genresList;
        public ObservableCollection<KeyValuePair<string, string>> GenresList
        {
            get => _genresList;
            set => SetProperty(ref _genresList, value);
        }


        // 메세지박스대신에 다이얼로그로 표현하기 위해서
        private IDialogCoordinator _dialogCoordinator;

        //디자인 타임에서도 사용할 수 있도록 기본 생성자 오버로드를 추가
        public BooksViewModel() : this(DialogCoordinator.Instance) { }

        public BooksViewModel(IDialogCoordinator coordinator)
        {   this._dialogCoordinator = coordinator;
            InitVariable();
            LoadGridFromDb();
            LoadComboFromDb();
        }

        private void InitVariable()
        {
            SelectedBook = new Book();
            SelectedBook.Idx = 0;
            SelectedBook.DNames = string.Empty;
            SelectedBook.BNames = string.Empty;
            SelectedBook.Author = string.Empty;
            SelectedBook.ISBN = string.Empty;
            SelectedBook.ReleaseDate = new DateTime();
            SelectedBook.Price = 0;

            _isUpdate = false;
        }

        private void LoadComboFromDb()
        {
            string query = "SELECT distinct b.Division, d.Names FROM madang.bookstbl b, divtbl d where b.Division = d.Division;";
            ObservableCollection<KeyValuePair<string, string>> temp = new ObservableCollection<KeyValuePair<string, string>>();

            using (MySqlConnection conn = new MySqlConnection(Common.CONNSTR))
            {
                try
                {
                    conn.Open();

                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    MySqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {

                        var d = reader.GetString(reader.GetOrdinal("Division"));
                        var n = reader.GetString(reader.GetOrdinal("Names"));
                        temp.Add(new KeyValuePair<string, string>(d, n));

                    }
                }
                catch (MySqlException ex)
                {
                    MessageBox.Show("DB 오류 발생: " + ex.Message);
                }
                GenresList = temp;
            }

        }

        private void LoadGridFromDb()
        {

            //string connectionString = "Server=localhost;Database=madang;Uid=root;Pwd=12345;Charset=utf8";

            string query = "SELECT Idx,Author,bookstbl.Division as bd,bookstbl.Names as bn,ReleaseDate,ISBN,Price , divtbl.Names as dn FROM bookstbl , divtbl WHERE bookstbl.Division = divtbl.Division ";
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
                        var idx = reader.GetInt32(reader.GetOrdinal("Idx"));
                        var author = reader.GetString(reader.GetOrdinal("Author"));
                        var division = reader.GetString(reader.GetOrdinal("bd"));
                        var names = reader.GetString(reader.GetOrdinal("bn"));
                        var releaseDate = reader.GetDateTime(reader.GetOrdinal("ReleaseDate"));
                        var isbn = reader.GetString(reader.GetOrdinal("ISBN"));
                        var price = reader.IsDBNull(reader.GetOrdinal("Price")) ? 0 : reader.GetInt32(reader.GetOrdinal("Price"));
                        var divisionNames = reader.GetString(reader.GetOrdinal("dn"));


                        books.Add(new Book { Division = division,
                            DNames = divisionNames,
                            Idx = idx,
                            Author = author,
                            ReleaseDate = releaseDate,
                            ISBN = isbn,
                            Price = price,
                            BNames = names,
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

        [RelayCommand]
        public void SetInit()
        {

            InitVariable();
            Common.LOGGER.Info("초기화버튼");
        }

        [RelayCommand]
        public async void DelData()
        {

            if (_isUpdate == false)
            {
                // MessageBox.Show("선택된 데이터가 없습니다.");
                await this._dialogCoordinator.ShowMessageAsync(this, "삭제 관리", "선택된 데이터가 없습니다.");
                return;
            }
            //string connectionString = "Server=localhost;Database=madang;Uid=root;Pwd=12345;Charset=utf8";
            string query = "Delete  FROM bookstbl where Idx =@Idx";
            //MessageBox.Show($"삭제 시도: Division = [{SelectedGenre.Division}]");

            var result = await this._dialogCoordinator.ShowMessageAsync(this, "삭제 전 확인", "삭제하시겠습니까?", MessageDialogStyle.AffirmativeAndNegative);
            if (result == MessageDialogResult.Affirmative) //삭제  ok
            {
                Common.LOGGER.Info("삭제동의완료");
            }
            else
            {
                return;
            }

            using (MySqlConnection conn = new MySqlConnection(Common.CONNSTR))
            {
                try
                {
                    conn.Open();
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@Idx", SelectedBook.Idx);
                    int resultCnt = cmd.ExecuteNonQuery();  //한 건 삭제하면 resultCnt=1, 안 지워지면 resultCnt=0

                    if (resultCnt > 0)
                    {
                        //MessageBox.Show("삭제 성공");
                        await this._dialogCoordinator.ShowMessageAsync(this, "삭제 관리", "삭제 성공");

                        Common.LOGGER.Info("삭제버튼-삭제 성공");
                        LoadGridFromDb(); // 목록 갱신
                        SetInit();        // 선택 초기화
                    }
                    else
                    {
                        //MessageBox.Show("삭제실패");
                        await this._dialogCoordinator.ShowMessageAsync(this, "삭제 관리", "삭제실패");
                        Common.LOGGER.Info("삭제버튼-삭제실패");
                    }

                }
                catch (MySqlException ex)
                {
                    //MessageBox.Show("DB 오류 발생: " + ex.Message);
                    await this._dialogCoordinator.ShowMessageAsync(this, "삭제 관리", $"DB 오류 발생:{ex.Message}");
                    Common.LOGGER.Info($"삭제버튼-DB 오류 발생{ex.Message}");
                }



            }
        }

        [RelayCommand]
        public async void SaveData()
        {

            //string connectionString = "Server=localhost;Database=madang;Uid=root;Pwd=12345;Charset=utf8";
            string query = string.Empty;

            using (MySqlConnection conn = new MySqlConnection(Common.CONNSTR))
            {
                try
                {
                    conn.Open();

                    //신규추가
                    if (_isUpdate == false)
                    {
                        query = "insert into bookstbl( Author ,Division,Names,ReleaseDate,ISBN,Price)" +
                            " values( @Author,@Division,@Names,@ReleaseDate,@ISBN,@Price)";
                    }
                    //기존업데이트
                    else
                    {
                        query = "Update bookstbl set Author = @Author," +
                            "Division = @Division,Names =@Names,ReleaseDate =@ReleaseDate,ISBN =@ISBN,Price=@Price" +
                            " where Idx =@Idx";
                    }

                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    if (_isUpdate)  cmd.Parameters.AddWithValue("@Idx", SelectedBook.Idx);
                    cmd.Parameters.AddWithValue("@Author", SelectedBook.Author);
                    cmd.Parameters.AddWithValue("@Names", SelectedBook.BNames);
                    cmd.Parameters.AddWithValue("@ReleaseDate", SelectedBook.ReleaseDate);
                    cmd.Parameters.AddWithValue("@ISBN", SelectedBook.ISBN);
                    cmd.Parameters.AddWithValue("@Price", SelectedBook.Price);
                    cmd.Parameters.AddWithValue("@Division", SelectedBook.Division);
                    var resultCnt = cmd.ExecuteNonQuery();  //한 건 삭제하면 resultCnt=1, 안 지워지면 resultCnt=0

                    if (resultCnt > 0)
                    {
                        //MessageBox.Show("저장 성공");
                        await this._dialogCoordinator.ShowMessageAsync(this, "저장 관리", "저장 성공");
                        Common.LOGGER.Info("저장버튼-저장 성공");
                        LoadGridFromDb(); // 목록 갱신
                        SetInit();        // 선택 초기화
                    }
                    else
                    {
                        //MessageBox.Show("저장 실패");
                        await this._dialogCoordinator.ShowMessageAsync(this, "저장 관리", "저장 실패");
                        Common.LOGGER.Info("저장버튼-저장 실패");
                    }

                }
                catch (MySqlException ex)
                {
                    // MessageBox.Show("DB 오류 발생: " + ex.Message);
                    await this._dialogCoordinator.ShowMessageAsync(this, "저장 관리", $"DB 오류 발생:{ex.Message}");
                    Common.LOGGER.Info($"저장버튼-DB 오류 발생{ex.Message}");
                }




            }
        }
    }
}
