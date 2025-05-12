using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using WpfBookRentalShop01.Models;

namespace WpfBookRentalShop01.ViewModels
{
    public partial class BookGenreViewModel : ObservableObject
    {
       
        private bool _isUpdate;

        // 상세보기
        private Genre _selectedGenre;
        public Genre SelectedGenre
        {
            get => _selectedGenre;
            set
            {
                SetProperty(ref _selectedGenre, value);
                _isUpdate = true;
            }
        }

        //db에서 읽어온 데이터 저장할 공간
       private ObservableCollection<Genre> _genreList;

        public ObservableCollection<Genre> GenreList
        {
            get => _genreList;
            set =>SetProperty(ref _genreList, value);
        }

        public BookGenreViewModel()
        {
            _isUpdate = false;
            LoadGridFromDb();
        }

        private void LoadGridFromDb()
        {
         
            string connectionString = "Server=localhost;Database=madang;Uid=root;Pwd=12345;Charset=utf8";
         
            string query = "SELECT Division,Names FROM divtbl";
            ObservableCollection<Genre> genres = new ObservableCollection<Genre>();

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
                        genres.Add(new Genre { Division = division, Name = name });

                    }
                }
                catch (MySqlException ex)
                {

                }

                GenreList = genres;

            }
        }

        [RelayCommand]
        public void SetInit() 
        {
            _isUpdate = false;
            SelectedGenre = null;
        }

        [RelayCommand]
        public void DelData()
        { 
            if (_isUpdate == false)
            {
                MessageBox.Show("선택된 데이터가 없습니다.");
                return;
            }



            string connectionString = "Server=localhost;Database=madang;Uid=root;Pwd=12345;Charset=utf8";

            string query = "Delete  FROM divtbl where Division =@Division";
            MessageBox.Show($"삭제 시도: Division = [{SelectedGenre.Division}]");

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@Division", SelectedGenre.Division);
                    int resultCnt = cmd.ExecuteNonQuery();  //한 건 삭제하면 resultCnt=1, 안 지워지면 resultCnt=0

                    if (resultCnt> 0)
                    {
                        MessageBox.Show("삭제 성공");
                        LoadGridFromDb(); // 목록 갱신
                        SetInit();        // 선택 초기화
                    }
                    else
                        MessageBox.Show("삭제실패");
                }
                catch (MySqlException ex)
                {
                    MessageBox.Show("DB 오류 발생: " + ex.Message);
                }



            }
        }

        [RelayCommand]
        public void SaveData() { }

    }
}
