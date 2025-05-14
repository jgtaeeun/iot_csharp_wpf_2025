using MahApps.Metro.Controls.Dialogs;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieFinder2025.Helpers
{
    public class Common
    {
        //NLog 인스턴스
        public static readonly Logger LOGGER = LogManager.GetCurrentClassLogger();

        //DB연결 connectString
        public static readonly string CONNSTR = "Server=localhost;Database=moviefinder;Uid=root;Pwd=12345;Charset=utf8";


        //MahApps.Metro 다이얼로그 코디네이터
        public static IDialogCoordinator DIALOGCOORDINATOR;
    }
}
