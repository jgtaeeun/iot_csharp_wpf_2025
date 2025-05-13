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
using static Mysqlx.Notice.Warning.Types;

namespace WpfBookRentalShop01.ViewModels
{


    public partial class MemberViewModel : ObservableObject
    {


        //상세보기

        private bool _isUpdate;

        // 상세보기
        private Member _selectedMember;
        public Member SelectedMember
        {
            get => _selectedMember;
            set
            {
                SetProperty(ref _selectedMember, value);
                _isUpdate = true;
            }
        }

        //콤보박스 리스트
        private ObservableCollection<string> _levelsList;
        public ObservableCollection<string> LevelsList 
        {
            get => _levelsList;
            set =>SetProperty(ref _levelsList, value);
        }

        //db에서 읽어온 데이터 저장할 공간
        private ObservableCollection<Member> _memberList;

        public ObservableCollection<Member> MemberList
        {
            get => _memberList;
            set => SetProperty(ref _memberList, value);
        }



        // 메세지박스대신에 다이얼로그로 표현하기 위해서
        private IDialogCoordinator _dialogCoordinator;

        //디자인 타임에서도 사용할 수 있도록 기본 생성자 오버로드를 추가
        public MemberViewModel() : this(DialogCoordinator.Instance) { }

        public MemberViewModel(IDialogCoordinator coordinator)
        {
            this._dialogCoordinator = coordinator;
            InitVariable();
            LoadGridFromDb();
            LoadComboFromDb();
        }

     

        private void InitVariable()
        {
            SelectedMember = new Member()
            {
                Idx = 0,
                MNames = string.Empty,
                Levels = string.Empty,
                Addr = string.Empty,
                Mobile  = string.Empty,
                Email = string.Empty,
            };

            _isUpdate = false;
        }


        private void LoadComboFromDb()
        {
            string query = "SELECT Distinct Levels FROM membertbl order by Levels";
            ObservableCollection<string> levels = new ObservableCollection<string>();

            using (MySqlConnection conn = new MySqlConnection(Common.CONNSTR))
            {
                try
                {
                    conn.Open();

                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    MySqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                      
                        var level = reader.GetString(reader.GetOrdinal("Levels"));

                        levels.Add(level);

                    }
                }
                catch (MySqlException ex)
                {
                    MessageBox.Show("DB 오류 발생: " + ex.Message);
                }
                LevelsList = levels;
            }

        }

        private void LoadGridFromDb()
        {

            //string connectionString = "Server=localhost;Database=madang;Uid=root;Pwd=12345;Charset=utf8";

            string query = "SELECT Idx,Names,Levels,Addr,Mobile,Email FROM membertbl";
            ObservableCollection<Member> members = new ObservableCollection<Member>();

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
                        var names = reader.GetString(reader.GetOrdinal("Names"));
                        var levels = reader.GetString(reader.GetOrdinal("Levels"));
                        var addr = reader.GetString(reader.GetOrdinal("Addr"));
                        var mobile = reader.GetString(reader.GetOrdinal("Mobile"));
                        var email = reader.GetString(reader.GetOrdinal("Email"));


                        members.Add(new Member
                        {
                            Idx = idx,
                            MNames = names,
                            Levels = levels,
                            Addr = addr,
                            Mobile = mobile,
                            Email = email
                        });

                    }
                }
                catch (MySqlException ex)
                {
                    MessageBox.Show("DB 오류 발생: " + ex.Message);
                }

               MemberList = members;

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
            string query = "Delete  FROM membertbl where Idx =@Idx";
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
                    cmd.Parameters.AddWithValue("@Idx", SelectedMember.Idx);
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
                        query = "insert into membertbl( Names,Levels,Addr,Mobile,Email)" +
                            " values( @Names,@Levels,@Addr,@Mobile,@Email)";
                    }
                    //기존업데이트
                    else
                    {
                        query = "Update membertbl set Names = @Names," +
                            "Levels = @Levels,Addr =@Addr,Mobile =@Mobile,Email =@Email" +
                            " where Idx =@Idx";
                    }

                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    if (_isUpdate) cmd.Parameters.AddWithValue("@Idx", SelectedMember.Idx);
                    cmd.Parameters.AddWithValue("@Names", SelectedMember.MNames);
                    cmd.Parameters.AddWithValue("@Levels", SelectedMember.Levels);
                    cmd.Parameters.AddWithValue("@Addr", SelectedMember.Addr);
                    cmd.Parameters.AddWithValue("@Mobile", SelectedMember.Mobile);
                    cmd.Parameters.AddWithValue("@Email", SelectedMember.Email);

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
