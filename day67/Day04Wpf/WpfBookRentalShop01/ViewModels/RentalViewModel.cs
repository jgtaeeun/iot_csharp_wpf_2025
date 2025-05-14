using CommunityToolkit.Mvvm.ComponentModel;
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


    public partial class RentalViewModel : ObservableObject
    {

        
        // 콤보박스
        private ObservableCollection<KeyValuePair<int, string>> _memberList;

        public ObservableCollection<KeyValuePair<int, string>> MemberList
        {
            get => _memberList;
            set => SetProperty(ref _memberList, value);
        }

        private ObservableCollection<KeyValuePair<int, string>> _bookList;

        public ObservableCollection<KeyValuePair<int, string>>  BookList
        {
            get => _bookList;
            set => SetProperty(ref _bookList, value);
        }

        // 데이터 그리드 

        private ObservableCollection<Rental> _rentalList;
        public ObservableCollection<Rental> RentalList
        {
            get => _rentalList;
            set => SetProperty(ref _rentalList, value);
        }
        //선택 항목-상세보기

        private bool _isUpdate;

        // 상세보기
        private Rental _selectedRental;
        public Rental SelectedRental
        {
            get => _selectedRental;
            set
            {
                SetProperty(ref _selectedRental, value);
                _isUpdate = true;
            }
        }
        // 메세지박스대신에 다이얼로그로 표현하기 위해서
        private IDialogCoordinator _dialogCoordinator;


        public RentalViewModel() : this(DialogCoordinator.Instance) { }

        public RentalViewModel(IDialogCoordinator coordinator)
        {
            this._dialogCoordinator = coordinator;
            InitVariable();
            LoadGridFromDb();
            LoadComboFromDb();
            LoadComboFromDb2();
        }

       

        private void InitVariable()
        {
            SelectedRental = new Rental()
            {
                Idx = 0,
                MNames = string.Empty,
                BookIdx= 0,
                MemberIdx = 0,
                BNames = string.Empty,
                RentalDate = new DateTime(),
                ReturnDate = new DateTime()

            };

            _isUpdate = false;
        }


        private void LoadComboFromDb()
        {
            string query = "SELECT Idx,Names FROM membertbl";
            ObservableCollection<KeyValuePair<int, string>> members = new ObservableCollection<KeyValuePair<int, string>>();


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
                        var names = reader.GetString(reader.GetOrdinal("Names"));
                    
                       

                        members.Add(new KeyValuePair<int, string>(idx, names));


                    }
                }
                catch (MySqlException ex)
                {
                    MessageBox.Show("DB 오류 발생: " + ex.Message);
                }

                MemberList = members;

            }
        }

        private void LoadComboFromDb2()
        {
            string query = "SELECT bookid,bookname FROM Book";
            ObservableCollection<KeyValuePair<int, string>> books = new ObservableCollection<KeyValuePair<int, string>>();


            using (MySqlConnection conn = new MySqlConnection(Common.CONNSTR))
            {
                try
                {
                    conn.Open();

                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    MySqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        var idx = reader.GetInt32(reader.GetOrdinal("bookid"));
                        var names = reader.GetString(reader.GetOrdinal("bookname"));



                        books.Add(new KeyValuePair<int, string>(idx, names));


                    }
                }
                catch (MySqlException ex)
                {
                    MessageBox.Show("DB 오류 발생: " + ex.Message);
                }

                BookList = books;

            }
        }

        private void LoadGridFromDb()
        {

            //string connectionString = "Server=localhost;Database=madang;Uid=root;Pwd=12345;Charset=utf8";

            string query = "select r.Idx,memberIdx,r.bookIdx,rentalDate,returnDate , m.Names , b.bookname from rentaltbl r, membertbl m , Book b \r\nwhere r.memberIdx = m.Idx and r.bookIdx= b.bookid;\r\n";
            ObservableCollection<Rental> rentals = new ObservableCollection<Rental>();

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
                        var memberIdx = reader.GetInt32(reader.GetOrdinal("memberIdx"));
                        var bookIdx = reader.GetInt32(reader.GetOrdinal("bookIdx"));
                        var rentalDate = reader.GetDateTime(reader.GetOrdinal("rentalDate"));

                        //DB에 returnDate값이 NULL인 경우가 있어 예외가 일어나서 아래와 같이 작성
                        var returnDate = !reader.IsDBNull(reader.GetOrdinal("returnDate")) ? reader.GetDateTime(reader.GetOrdinal("returnDate")) : new DateTime();

                        var names = reader.GetString(reader.GetOrdinal("Names"));
                        var bookname = reader.GetString(reader.GetOrdinal("bookname"));



                        rentals.Add(new Rental
                        {
                            Idx = idx,
                            MemberIdx = memberIdx,
                            BookIdx = bookIdx,
                            RentalDate = rentalDate,
                            ReturnDate = returnDate,
                            MNames = names,
                            BNames = bookname
                        });

                    }
                }
                catch (MySqlException ex)
                {
                    MessageBox.Show("DB 오류 발생: " + ex.Message);
                }

                RentalList = rentals ;

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
            string query = "Delete  FROM rentaltbl where Idx =@Idx";
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
                    cmd.Parameters.AddWithValue("@Idx", SelectedRental.Idx);
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

            //string connectionSt
            //ring = "Server=localhost;Database=madang;Uid=root;Pwd=12345;Charset=utf8";
            string query = string.Empty;

            using (MySqlConnection conn = new MySqlConnection(Common.CONNSTR))
            {
                try
                {
                    conn.Open();

                    //신규추가
                    if (_isUpdate == false)
                    {
                        query = "insert into rentaltbl( memberIdx,bookIdx,rentalDate,returnDate)" +
                            " values( @memberIdx,@bookIdx,@rentalDate,@returnDate)";
                    }
                    //기존업데이트
                    else
                    {
                        query = "Update rentaltbl set memberIdx= @memberIdx," +
                            "bookIdx = @bookIdx,rentalDate =@rentalDate,returnDate=@returnDate" +
                            " where Idx =@Idx";
                    }

                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    if (_isUpdate) cmd.Parameters.AddWithValue("@Idx", SelectedRental.Idx);
                    cmd.Parameters.AddWithValue("@memberIdx", SelectedRental.MemberIdx);
                    cmd.Parameters.AddWithValue("@bookIdx", SelectedRental.BookIdx);
                    cmd.Parameters.AddWithValue("@rentalDate", SelectedRental.RentalDate);
                    cmd.Parameters.AddWithValue("@returnDate", SelectedRental.ReturnDate);



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

