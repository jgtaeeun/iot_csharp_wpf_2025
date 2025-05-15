using CommunityToolkit.Mvvm.ComponentModel;
using MovieFinder2025.Helpers;
using Org.BouncyCastle.Ocsp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MovieFinder2025.Models
{
    public class MovieItem : ObservableObject
    {
        /*
         {"adult":false,
        "backdrop_path":"/bKCpRjjTKcr3KAITmwjVMobbBYg.jpg",
        "genre_ids":[18],
        "id":615643,
        "original_language":"en",
        "original_title":"Minari",
        "overview":"낯선 미국에서 병아리를 감별하며 생계를 이어가던 제이콥과 모니카. 딸 앤과 아들 데이빗에게 아버지로서 뭔가 해내는 모습을 보여주고 싶은 제이콥은 아칸소로 이주해 자신의 농장을 가꾼다. 모니카는 낡은 컨테이너에서 생활하며 농장 일에만 몰두하는 제이콥이 못마땅하지만 그저 그의 결정을 지켜볼 뿐이다. 아칸소에서의 적적하고 고된 삶에 지친 모니카는 엄마 순자를 미국으로 모신다. 한약, 멸치, 미나리 씨 등을 잔뜩 챙겨온 순자는 여느 할머니와 달리 요리도 하지 않고 프로레슬링을 즐겨 본다. 앤과 데이빗은 그런 할머니가 낯설지만, 못된 장난까지 사랑으로 포용하는 할머니와 점점 가까워진다.",
        "popularity":4.1018,
        "poster_path":"/ltS2iKKvvBi7ZXWPRZSZBGmC9Gr.jpg",
        "release_date":"2021-02-12",
        "title":"미나리",
        "video":false,
        "vote_average":7.336,
        "vote_count":1678}
         */
        private bool _adult;
        private string _backdrop_path;
        private List<int> _genre_ids;
        private int _id;
        private string _original_language;
        private string _original_title;
        private string _overview;
        private double _popularity;
        private string _poster_path;
        private DateTime? _release_date;
        private string _title;
        private bool _video;
        private double  _vote_average;
        private int _vote_count;

        public bool Adult { get => _adult; set => SetProperty(ref _adult, value); }
        public string Backdrop_path { get => _backdrop_path; set => SetProperty(ref _backdrop_path, value); }
        public List<int> Genre_ids { get => _genre_ids; set => SetProperty(ref _genre_ids , value); }
        public int Id { get => _id; set => SetProperty(ref _id, value); }
        public string Original_language { get => _original_language; set => SetProperty(ref _original_language, value); }
        public string Original_title { get => _original_title; set => SetProperty(ref _original_title, value); }
        public string Overview { get => _overview; set => SetProperty(ref _overview, value); }
        public double Popularity { get => _popularity; set => SetProperty(ref  _popularity, value); }
        public string Poster_path { get => _poster_path; set => SetProperty(ref _poster_path, value); }
        
       
        [JsonConverter(typeof(SafeDateTimeConverter))]  // 커스텀 변환기 사용
        public DateTime? Release_date { get => _release_date; set => SetProperty(ref _release_date, value); }
        
        public string Title { get => _title; set => SetProperty(ref _title, value); }
        public bool Video { get => _video; set => _video = value; }
        public double Vote_average { get => _vote_average; set => _vote_average = value; }
        public int Vote_count { get => _vote_count; set => _vote_count = value; }
    }
}
