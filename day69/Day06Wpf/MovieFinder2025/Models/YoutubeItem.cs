using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace MovieFinder2025.Models
{
    public class YoutubeItem :ObservableObject
    {
        public string Title { get; set; }   //영상 제목
        public string Author { get; set; } //영상 작성자
        public string ChannelTitle {  get; set; } //영상 채널명
        public string URL { get; set; }  //영상 url
        public BitmapImage Thumbanil { get; set; }  //영상 썸네일 

    }
}
