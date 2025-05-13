using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WpfBookRentalShop01.ViewModels;

namespace WpfBookRentalShop01.Views
{
    /// <summary>
    /// BooksView.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class RentalView : UserControl
    {
        public RentalView()
        {
            InitializeComponent();
            this.DataContext = new RentalViewModel();  // 이 줄이 꼭 있어야 함
        }
    }
}
