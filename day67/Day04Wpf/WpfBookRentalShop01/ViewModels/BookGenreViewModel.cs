using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MahApps.Metro.Controls.Dialogs;
using MySql.Data.MySqlClient;
using Org.BouncyCastle.Crypto.Modes.Gcm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using WpfBookRentalShop01.Helpers;
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


        // 메세지박스대신에 다이얼로그로 표현하기 위해서
        private IDialogCoordinator _dialogCoordinator;

        //디자인 타임에서도 사용할 수 있도록 기본 생성자 오버로드를 추가
        public BookGenreViewModel() : this(DialogCoordinator.Instance) { }

        public BookGenreViewModel(IDialogCoordinator coordinator)
        {
            
            this._dialogCoordinator = coordinator;
            InitVariable();
            LoadGridFromDb();
        }

        private void InitVariable()
        {
            SelectedGenre = new Genre();
            SelectedGenre.Division = string.Empty;
            SelectedGenre.Name = string.Empty;

            _isUpdate = false;
        }
        private void LoadGridFromDb()
        {
         
            //string connectionString = "Server=localhost;Database=madang;Uid=root;Pwd=12345;Charset=utf8";
         
            string query = "SELECT Division,Names FROM divtbl";
            ObservableCollection<Genre> genres = new ObservableCollection<Genre>();

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
            string query = "Delete  FROM divtbl where Division =@Division";
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
                    cmd.Parameters.AddWithValue("@Division", SelectedGenre.Division);
                    int resultCnt = cmd.ExecuteNonQuery();  //한 건 삭제하면 resultCnt=1, 안 지워지면 resultCnt=0

                    if (resultCnt> 0)
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
                    if (_isUpdate == false && !string.IsNullOrWhiteSpace(SelectedGenre.Division)
                    && !string.IsNullOrWhiteSpace(SelectedGenre.Name))
                    {
                        query = "insert into divtbl values(@Division, @Name)";
                    }
                    //기존업데이트
                    else
                    {
                        query = "Update divtbl set Names = @Name where Division =@Division";
                    }
                    
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@Division", SelectedGenre.Division);
                    cmd.Parameters.AddWithValue("@Name", SelectedGenre.Name);
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
