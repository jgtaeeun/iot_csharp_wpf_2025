using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MahApps.Metro.Controls.Dialogs;
using MovieFinder2025.Helpers;
using MovieFinder2025.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Security.RightsManagement;
using System.Text;
using System.Threading.Tasks;
using System.Web;

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
                Common.LOGGER.Info($"SelectedMovieItem: {value.Poster_path}");
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

        // 메세지박스대신에 다이얼로그로 표현하기 위해서
        private IDialogCoordinator _dialogCoordinator;
       
        public MoviesViewModel(IDialogCoordinator coordinator) 
        {
            this._dialogCoordinator = coordinator;
            Common.LOGGER.Info("MovieFinder2025 시작");
            PosterUri = new Uri("/nopicture.png" , uriKind: UriKind.RelativeOrAbsolute);
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

            // //db에서 영화검색하는 함수 + 검색시간동안 다이얼로그
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
                    movieItems.Add(movie);
                }
              
            }
            catch (Exception ex)
            {
                await this._dialogCoordinator.ShowMessageAsync(this, "예외", ex.Message);
                Common.LOGGER.Fatal(ex.Message);
            }
            MovieItems = movieItems;
        }

        [RelayCommand]
        public async Task MovieItemDoubleClick() 
        {
            var currentMovie = SelectedMovieItem;

            if (currentMovie != null)
            {
                StringBuilder sb= new StringBuilder();
                sb.Append(currentMovie.Original_title + "(" + currentMovie.Release_date.ToString("yyyy-MM-dd") + ")" + Environment.NewLine + Environment.NewLine);
                sb.Append($"평점 ★ {currentMovie.Vote_average.ToString("F2")}\r\n\r\n");
                sb.Append(currentMovie.Overview);
                await this._dialogCoordinator.ShowMessageAsync(this, currentMovie.Title, sb.ToString());
            }
        }
    }
}
