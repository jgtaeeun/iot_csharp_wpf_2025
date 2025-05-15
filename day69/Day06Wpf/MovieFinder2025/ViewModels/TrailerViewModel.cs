using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Google.Apis.Services;
using Google.Apis.YouTube.v3;
using MahApps.Metro.Controls.Dialogs;
using MovieFinder2025.Helpers;
using MovieFinder2025.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace MovieFinder2025.ViewModels
{
    public partial class TrailerViewModel : ObservableObject
    {
        // MovieBView에서 가져온 영화제목
        private string _movieTitle;
        public string MovieTitle
        {
            get => _movieTitle;
            set => SetProperty(ref _movieTitle, value);
        }

        //유튜브API에서 가져온 정보들
        private ObservableCollection<YoutubeItem> _youtubeItems;
        public ObservableCollection<YoutubeItem>  YoutubeItems
        {
            get => _youtubeItems;
            set => SetProperty(ref _youtubeItems, value);
        }


        //유튜브 예고편 목록 중 선택한 것
        private YoutubeItem _selectedYoutube;
        public YoutubeItem SelectedYoutube
        {
            get => _selectedYoutube;
            set => SetProperty(ref _selectedYoutube, value);
        }

        // 선택한 영화 uri 예고편을 보여줌
        private string _youtubeUri;
        public string YoutubeUri
        {
            get => _youtubeUri;
            set => SetProperty(ref _youtubeUri, value);
        }

        // 메세지박스대신에 다이얼로그로 표현하기 위해서
        private IDialogCoordinator _dialogCoordinator;

    
        public TrailerViewModel(IDialogCoordinator coordinator , string mvm )
        {
            this._dialogCoordinator = coordinator;
            Common.LOGGER.Info(" TrailerViewModel 시작");
            MovieTitle = mvm;

            //초기화면은 유튜브 처음페이지
            YoutubeUri = "https:www.youtube.com";
            
            //YoutubeApi로 예고편 찾는 함수
            SearchYoutubeApi();
          
        }

  

        private async void SearchYoutubeApi()
        {
            await LoadDataCollection();
        }

        private async Task LoadDataCollection()
        {
            var servie = new YouTubeService(
                new BaseClientService.Initializer()
                {
                    ApiKey = "AIzaSyC-ry_xG-vRtUqqP7PRWFk5HsJeYA0yrhw",
                    ApplicationName = this.GetType().ToString()
                });
            var req = servie.Search.List("snippet");
            req.Q = $"{MovieTitle} 예고편 공식";  //영화이름으로 api 검색
            req.Order = SearchResource.ListRequest.OrderEnum.Relevance;
            req.Type = "video";
            req.MaxResults = 10;

            var res = await req.ExecuteAsync(); //api실행결과를 리턴(비동기)

           // MessageBox.Show(res.Items[0].Snippet.Title, "api응답결과");

            //임시저장변수
            ObservableCollection<YoutubeItem> temp = new ObservableCollection<YoutubeItem>();
            foreach ( var item in res.Items )
            {
                temp.Add( new YoutubeItem
                {
                    Title = item.Snippet.Title,
                    ChannelTitle = item.Snippet.ChannelTitle,
                    URL = $"https://www.youtube.com/watch?v={item.Id.VideoId}",
                    Author = item.Snippet.ChannelId,
                    Thumbanil = new BitmapImage(new Uri(item.Snippet.Thumbnails.Default__.Url, UriKind.RelativeOrAbsolute))  
                }
                );
            }

            YoutubeItems = temp;
            
        }

        [RelayCommand]
        public  async Task TrailerDoubleClick()
        {
            YoutubeUri = SelectedYoutube.URL;
        }
    }
}

