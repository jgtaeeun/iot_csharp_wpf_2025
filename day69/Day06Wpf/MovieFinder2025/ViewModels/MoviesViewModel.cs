using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MahApps.Metro.Controls.Dialogs;
using MovieFinder2025.Helpers;
using MovieFinder2025.Models;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.DirectoryServices;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Reflection.PortableExecutable;
using System.Security.RightsManagement;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Threading;

namespace MovieFinder2025.ViewModels
{
    public partial class MoviesViewModel : ObservableObject
    {

        //영화검색
        private string _movieName;

        public string MovieName 
        {
            get => _movieName;
            set => SetProperty(ref _movieName, value);
        }


        //영화데이터 
        private ObservableCollection<MovieItem> _movieItems;
        public ObservableCollection<MovieItem> MovieItems
        {
            get => _movieItems;
            set => SetProperty(ref _movieItems, value);
        }

        //선택한 것
        private MovieItem _selectedMovieItem;
        public MovieItem SelectedMovieItem
        {
            get => _selectedMovieItem;
            set 
            {
                SetProperty(ref _selectedMovieItem, value);
                Common.LOGGER.Info($"SelectedMovieItem: {_baseurl}{value.Poster_path}");
                PosterUri = new Uri($"{_baseurl}{value.Poster_path}", uriKind: UriKind.RelativeOrAbsolute);
            }

        }

        // 선택한 것->포스터uri
        private string _baseurl = "https://image.tmdb.org/t/p/w300_and_h450_bestv2";

        private Uri _posterUri;
        public Uri PosterUri 
        {
            get=> _posterUri;
            set => SetProperty(ref _posterUri, value);
        }

        // 상태바 현재시간
        private readonly DispatcherTimer _timer;
        private string _currDateTime;
        public string CurrDateTime
        {
            get => _currDateTime;
            set => SetProperty(ref _currDateTime, value);
        }

        //검색건수

        private string _searchResult;
        public string SearchResult
        {
            get => _searchResult;
            set => SetProperty(ref _searchResult, value);
        }

        // 즐겨찾기 리스트인지 아닌지
        private bool _isFavoriteList = false;

        // 메세지박스대신에 다이얼로그로 표현하기 위해서
        private IDialogCoordinator _dialogCoordinator;
       
        public MoviesViewModel(IDialogCoordinator coordinator) 
        {
            this._dialogCoordinator = coordinator;
            Common.LOGGER.Info("MovieFinder2025 시작");
            PosterUri = new Uri("/nopicture.png" , uriKind: UriKind.RelativeOrAbsolute);

            //상태바 시계
            CurrDateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");   //최초 화면 시계
            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromSeconds(1);
            _timer.Tick += (sender, e) =>
            {
                CurrDateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"); 
            };
            _timer.Start();
        }


        // 사용자입력컨트롤
        [RelayCommand]
        public async Task SearchMovie()
        {
            if (string.IsNullOrEmpty(MovieName))
            {
                await this._dialogCoordinator.ShowMessageAsync(this, "영화검색", "영화명을 입력하세요!");
                return;
            }


            
            //db에서 영화검색하는 함수 + 검색시간동안 다이얼로그
            var controller = await _dialogCoordinator.ShowProgressAsync(this, "대기중", "검색중...");
            controller.SetIndeterminate();
            Common.LOGGER.Info($"영화 검색 시작[영화명:{MovieName}]");
            SearchMovie(MovieName);
            await Task.Delay(1000);
            await controller.CloseAsync();

        }

        //db에서 영화검색하는 함수
        private async void SearchMovie(string movieName)
        {
            string tmdb_apikey = "79a9d0d1e4e23aff7e47352831d128d2";
            string encoding_moviename = HttpUtility.UrlEncode(movieName, Encoding.UTF8);  //입력한 한글을 UTF-8로 변경
            string openApiUri = $"https://api.themoviedb.org/3/search/movie?api_key={tmdb_apikey}" +
                                $"&language=ko-KR&page=1&include_adult=false&query={encoding_moviename}";

            string result = string.Empty;

            //OpenAPI 실행할 웹 객체
            HttpClient client = new HttpClient();
            //HttpResponseMessage response;
            

            ObservableCollection<MovieItem> movieItems = new ObservableCollection<MovieItem>(); 
            try
            {
                //response = await client.GetAsync(openApiUri);
                var response = await client.GetFromJsonAsync<MovieSearchResponse>(openApiUri);

                foreach (var movie in response.Results)
                {
                    Common.LOGGER.Info(movie.Title);
                    movie.Release_date = movie.Release_date ?? new DateTime(0001, 1, 1);
                    movieItems.Add(movie);
                }
                SearchResult = $"영화검색 건수 {response.Total_results}건" ;
                Common.LOGGER.Info(SearchResult + "검색완료!!");
            }
            catch (Exception ex)
            {
                await this._dialogCoordinator.ShowMessageAsync(this, "예외", ex.Message);
                Common.LOGGER.Fatal(ex.Message);
                SearchResult = $"오류발생";
            }
            MovieItems = movieItems;
            _isFavoriteList = false;
        }

        [RelayCommand]
        public async Task MovieItemDoubleClick() 
        {
            var currentMovie = SelectedMovieItem;

            if (currentMovie != null)
            {
                StringBuilder sb= new StringBuilder();
                string releaseDateText = currentMovie.Release_date.HasValue
                ? currentMovie.Release_date.Value.ToString("yyyy-MM-dd")
                : "날짜없음";
                sb.Append(currentMovie.Original_title + "(" + releaseDateText + ")" + Environment.NewLine + Environment.NewLine);
                sb.Append($"평점 ★ {currentMovie.Vote_average.ToString("F2")}\r\n\r\n");
                sb.Append(currentMovie.Overview);
                await this._dialogCoordinator.ShowMessageAsync(this, currentMovie.Title, sb.ToString());
            }
        }
        [RelayCommand]
        public async Task AddMovieInfo()
        {
            if (SelectedMovieItem == null|| _isFavoriteList == true)
            {
                if (_isFavoriteList)
                {
                    await this._dialogCoordinator.ShowMessageAsync(this, "즐겨찾기 추가", "현재 즐겨찾기 리스트를 보고 있습니다.");
                }
                else
                {
                    await this._dialogCoordinator.ShowMessageAsync(this, "즐겨찾기 추가", "선택한 영화가 없습니다");
                }
                    
                return;
            }

       
            try 
            {

            var query = "Insert into movieItem values (@Id,@Adult,@Backdrop_path,@Original_language," +
            "@Original_title,@Overview,@Popularity,@Poster_path,@Release_date,@Title,@Vote_average,@Vote_count)";
                using (MySqlConnection conn = new MySqlConnection(Common.CONNSTR))
                {
                    conn.Open();
                    MySqlCommand cmd = new MySqlCommand(query, conn);

                    cmd.Parameters.AddWithValue("@Id", SelectedMovieItem.Id);
                    cmd.Parameters.AddWithValue("@Adult", SelectedMovieItem.Adult);
                    cmd.Parameters.AddWithValue("@Backdrop_path", SelectedMovieItem.Backdrop_path);
                    cmd.Parameters.AddWithValue("@Original_language", SelectedMovieItem.Original_language);
                    cmd.Parameters.AddWithValue("@Original_title", SelectedMovieItem.Original_title);
                    cmd.Parameters.AddWithValue("@Overview", SelectedMovieItem.Overview);
                    cmd.Parameters.AddWithValue("@Popularity", SelectedMovieItem.Popularity);
                    cmd.Parameters.AddWithValue("@Poster_path", SelectedMovieItem.Poster_path);
                    cmd.Parameters.AddWithValue("@Release_date", SelectedMovieItem.Release_date);
                    cmd.Parameters.AddWithValue("@Title", SelectedMovieItem.Title);
                    cmd.Parameters.AddWithValue("@Vote_average", SelectedMovieItem.Vote_average);
                    cmd.Parameters.AddWithValue("@Vote_count", SelectedMovieItem.Vote_count);

                    var resultCnt = cmd.ExecuteNonQuery();
                    if (resultCnt > 0)
                    {
                        //MessageBox.Show("저장 성공");
                        await this._dialogCoordinator.ShowMessageAsync(this, "즐겨찾기 추가", "즐겨찾기 추가 성공");
                        Common.LOGGER.Info("즐겨찾기 추가버튼-즐겨찾기 추가성공");
                    }
                    else
                    {
                        //MessageBox.Show("저장 실패");
                        await this._dialogCoordinator.ShowMessageAsync(this, "즐겨찾기 추가", "즐겨찾기 추가 실패");
                        Common.LOGGER.Info("즐겨찾기 추가버튼-즐겨찾기 추가 실패");
                    }

                }
            }
            catch (Exception e)
            {
                if (e.Message == "Duplicate entry '64931' for key 'movieItem.PRIMARY'")
                {
                    await this._dialogCoordinator.ShowMessageAsync(this, "즐겨찾기 추가", "이미 즐겨찾기에 등록되어있습니다.");
                }
                else
                {
                    await this._dialogCoordinator.ShowMessageAsync(this, "즐겨찾기 추가", e.Message);
                }
                Common.LOGGER.Fatal(e.Message);
                return;

            }



        }

        [RelayCommand]
        public async Task ViewMovieInfo()
        {
            MovieName = "";
            if (_isFavoriteList ==false)
            {
                var controller = await _dialogCoordinator.ShowProgressAsync(this, "즐겨찾기 보기", "데이터 가져오는 중...");
                controller.SetIndeterminate();
                ViewMovieInfoDetail();
                await Task.Delay(1000);
                await controller.CloseAsync();
            }
         
        }

        private async void ViewMovieInfoDetail()
        {
            string query = "Select Id,Adult ,Backdrop_path,Original_language ,Original_title," +
               " Overview,Popularity, Poster_path,Release_date, Title,Vote_average, Vote_count from movieItem";
            ObservableCollection<MovieItem> movieList = new ObservableCollection<MovieItem>();

            using (MySqlConnection conn = new MySqlConnection(Common.CONNSTR))
                try
                {
                    conn.Open();
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    MySqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        var id = reader.GetInt32(reader.GetOrdinal("Id"));
                        var adult = reader.GetBoolean(reader.GetOrdinal("Adult"));
                        var backdrop_path = reader.IsDBNull(2)?string.Empty: reader.GetString(reader.GetOrdinal("Backdrop_path"));
                        var original_language = reader.GetString(reader.GetOrdinal("Original_language"));
                        var original_title = reader.GetString(reader.GetOrdinal("Original_title"));
                        var overview = reader.GetString(reader.GetOrdinal("Overview"));
                        var popularity = reader.GetDouble(reader.GetOrdinal("Popularity"));
                        var poster_path = reader.GetString(reader.GetOrdinal("Poster_path"));
                        var release_date = reader.GetDateTime(reader.GetOrdinal("Release_date"));
                        var title = reader.GetString(reader.GetOrdinal("Title"));
                        var vote_average = reader.GetDouble(reader.GetOrdinal("Vote_average"));
                        var vote_count = reader.GetInt32(reader.GetOrdinal("Vote_count"));

                        movieList.Add(new MovieItem
                        {
                            Id = id,
                            Adult = adult,
                            Backdrop_path = backdrop_path,
                            Original_language = original_language,
                            Original_title = original_title,
                            Overview = overview,
                            Popularity = popularity,
                            Release_date = release_date,
                            Poster_path = poster_path,
                            Title = title,
                            Vote_average = vote_average,
                            Vote_count = vote_count,


                        });
                    }
                    MovieItems = movieList;
                    _isFavoriteList = true;


                }
                catch (Exception e)
                {
                    await this._dialogCoordinator.ShowMessageAsync(this, "즐겨찾기 보기", e.Message);
                    Common.LOGGER.Fatal(e.Message);
                    return;
                }
            
                //즐겨찾기가 없을 때, 포스터 이미지
                //즐겨찾기가 있을 때, 포스터 이미지는 인덱스 0번째꺼로
                if (movieList.Count > 0)
                {
                    SelectedMovieItem = movieList[0];
                }
                else
                {
                     PosterUri = new Uri("/nopicture.png", uriKind: UriKind.RelativeOrAbsolute);
                }
                SearchResult = $"즐겨찾기검색 건수 {movieList.Count}건";
                Common.LOGGER.Info(SearchResult + "검색완료!!");
                

        }
        [RelayCommand]
        public async Task DelMovieInfo()
        {
           if (SelectedMovieItem == null )
           {
                await this._dialogCoordinator.ShowMessageAsync(this, "즐겨찾기 삭제", "선택한 영화가 없습니다");
                return;
           }

           if ( _isFavoriteList == false)
           {
                await this._dialogCoordinator.ShowMessageAsync(this, "즐겨찾기 삭제", "현재 즐겨찾기 리스트가 아닙니다.");
                return;
           }
            string query = "DELETE FROM  movieItem WHERE Id = @Id";
            using (MySqlConnection conn = new MySqlConnection(Common.CONNSTR))
                try
                {
                  
                    conn.Open();
                    MySqlCommand cmd = new MySqlCommand(query, conn);

                    cmd.Parameters.AddWithValue("@Id", SelectedMovieItem.Id);

                    var resultCnt = cmd.ExecuteNonQuery();
                    if (resultCnt > 0)
                    {
                        await this._dialogCoordinator.ShowMessageAsync(this, "즐겨찾기 삭제", "즐겨찾기 삭제 성공");
                        Common.LOGGER.Info("즐겨찾기 삭제버튼-즐겨찾기 삭제성공");
                    }
                    else
                    {   await this._dialogCoordinator.ShowMessageAsync(this, "즐겨찾기 삭제", "즐겨찾기 삭제 실패");
                        Common.LOGGER.Info("즐겨찾기 삭제버튼-즐겨찾기 삭제 실패");
                    }

                }
                catch (Exception e)
                {
                    await this._dialogCoordinator.ShowMessageAsync(this, "즐겨찾기 삭제", e.Message);
                    Common.LOGGER.Fatal(e.Message);
                    return;
                }
        
                //삭제 후 업데이트 된 즐겨찾기 보기위해서  
                ViewMovieInfoDetail();

        }

        [RelayCommand]
        public async Task ViewMovieTrailer()
        {
            if (SelectedMovieItem == null)
            {
                await this._dialogCoordinator.ShowMessageAsync(this, "예고편 보기", "선택한 영화가 없습니다");
                return;
            }

            await this._dialogCoordinator.ShowMessageAsync(this, "예고편 보기", "유튜브 api 연결준비");

        }


        
    }

}
