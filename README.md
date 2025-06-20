# iot_csharp_wpf_2025
iot 개발자 wpf 학습

## 64일차(5/8)

### WPF(Windows Presentation Foundation) 개요
- WinForms의 디자인의 미약한 부분, 속도 개선, 개발과 디자인의 분리 개선하고자 만든 MS  프레임워크
- 화면디자인을 XML기반의 xaml

### WPF DB 바인딩 (with MahApps) [db연동](./day64/Day01Wpf/WpfBasicApp1/MainWindow.xaml)
1. 프로젝트 생성 - 새프로젝트>WPF 애플리케이션
2. Nuget 패키지 관리자 >MahApps.Metro 라이브러리 설치
3. 디자인 영역
    - App.xaml
    ```xml
        <ResourceDictionary>
            <ResourceDictionary.MergedDictionaries>
                    <!-- MahApps.Metro resource dictionaries. Make sure that all file names are Case Sensitive! -->
                    <ResourceDictionary Source="pack://application:,,,/MahApps.Metro;component/Styles/Controls.xaml" />
                    <ResourceDictionary Source="pack://application:,,,/MahApps.Metro;component/Styles/Fonts.xaml" />


                    <!-- Theme setting -->
                    <ResourceDictionary Source="pack://application:,,,/MahApps.Metro;component/Styles/Themes/Light.Yellow.xaml" />

            </ResourceDictionary.MergedDictionaries>
        </ResourceDictionary>
    ```

    - MainWindow.xaml
        ```xml
            <mah:MetroWindow x:Class="WpfBasicApp1.MainWindow"
            xmlns:mah="http://metro.mahapps.com/winfx/xaml/controls"
            xmlns:iconpacks="http://metro.mahapps.com/winfx/xaml/iconpacks"
            Loaded="MetroWindow_Loaded">
        ```
    - MainWindow.xaml.cs

        ```csharp
        using MahApps.Metro.Controls;
        public partial class MainWindow : MetroWindow
        ```

4. UI 구현 [MainWindow.xaml](./day64/Day01Wpf/WpfBasicApp1/MainWindow.xaml)
    - DataGrid + SqlDataAdapter
        - AutoGenerateColumns="True" 는 모든 열
            <img src ='./day64/datagrid속성autogeneratecolumns를true.png'> 
        - AutoGenerateColumns="False"
        ```xml
         <DataGrid x:Name="GrdBooks" Grid.Row="0" Grid.Column="0" Margin="5" AutoGenerateColumns="False">
            <DataGrid.Columns>
                <DataGridTextColumn Binding="{Binding Idx}" Header="순번"/>
                <DataGridTextColumn Binding="{Binding Names}" Header="책제목"/>
                <DataGridTextColumn Binding="{Binding ReleaseDate}" Header="출판일"/>
                <DataGridTextColumn Binding="{Binding Author}" Header="저자"/>
                <DataGridTextColumn Binding="{Binding Division}" Header="장르"/>
                <DataGridTextColumn Binding="{Binding ISBN}" Header="ISBN"/>
                <DataGridTextColumn Binding="{Binding Price}" Header="책가격"/>
            </DataGrid.Columns>
        </DataGrid>
        ``` 
        - <img src ='./day64/datagrid속성autogeneratecolumns를false.png'> 
    - DataGrid 한 행 선택한 것을 DataRowView로 형변환
        ```csharp
        // DATAGRID에서 더블클릭했을 때 이벤트- 선택한 그리드의 레코드값이 오른쪽 GROUPBOX에 출력함
        private void GrdBooks_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            //사용자가 선택한 행이 정확히 한 개인지 확인합니다.
            //DataGrid는 다중 선택이 가능하므로, 1개만 선택됐을 때만 처리하도록 조건을 줍니다.
            if (GrdBooks.SelectedItems.Count ==1) 
            {
                //DataRowView는 DataTable을 바인딩한 경우, 각 행을 나타냅니다.
                //GrdBooks.ItemsSource는 DataTable.DefaultView나 DataView 등으로 바인딩되어 있어야 합니다.
                //즉, DataGrid가 DataRowView 타입의 데이터를 보여주고 있어야 이 코드가 제대로 작동합니다.
                //데이터 그리드에 로드데이터할 때, adapter.Fill(dt)여서 가능함
                var item = GrdBooks.SelectedItems[0] as DataRowView;
                //MessageBox.Show(item.Row["Names"].ToString());

                NUD.Value = Convert.ToDouble(item.Row["Idx"]);
                CboDivisions.SelectedValue = Convert.ToString(item.Row["Division"]);
                TxtBookAuthor.Text = Convert.ToString(item.Row["Author"]);
                TxtBookName.Text= Convert.ToString(item.Row["Names"]);
                TxtISBN.Text = Convert.ToString(item.Row["ISBN"]);
                TxtPrice.Text = Convert.ToString(item.Row["Price"]);
                DP.Text = Convert.ToString(item.Row["ReleaseDate"]);
            }
        }
        ```
    <img src='./day64/datagrid하나의행선택을datarowview로행변환.png'>
5. DB연결 사전준비
    - Nuget 패키지 관리자 > MySQL.Data 라이브러리 설치

6. DB연결 
    1. DB연결문자열(connectionString): DB종류마다 연결문자열 포맷이 다르고 무조건 있어야 함
    2. 쿼리 : 실행할 쿼리(보통 SELECT, INSERT, UPDATE, DELETE)
    3. 데이터를 담을 객체 : 리스트 형식
    4. DB연결객체(SqlConnection) : 연결문자열을 처리하는 객체 . DB연결,끊기, 연결확인...
        - DB연결종류별로 MySqlConnection, SqlConnection, OracleConnection
    5. DB명령객체(SqlDataCommand) : 쿼리 컨트롤하는 객체, 생성시 쿼리와 연결객체
        - ExecuteReader() : SELECT문 실행, 결과 데이터를 담는 메서드
        - ExecuteNonQuery() : INSERT, UPDATE, DELETE문과 같이 트랜젝션이 발생하는 쿼리실행 사용 메서드
        - ExecuteScaler() : SELECT문 중 count() 등 함수로 1row/1column 데이터만 가져오는 메서드
    6. 데이터어댑터(SqlDataAdapter) : 연결이후 데이터처리를 쉽게 도와주는 객체
        - DB명령객체처럼 쿼리를 직접실행할 필요없음
        - DataTable, DataSet객체에 fill()메서드로 자동으로 채워줌
        - 거의 SELECT문에만 사용
        ```csharp
        string connectionString = "Server=localhost;Database=madang;Uid=root;Pwd=12345;Charset=utf8";
        string query = "SELECT Idx,Author,Division,Names,ReleaseDate,ISBN,Price FROM bookstbl";

        using (MySqlConnection conn = new MySqlConnection(connectionString)) 
        {
            try 
            {
                conn.Open();
                MySqlDataAdapter adapter = new MySqlDataAdapter(query, conn);
                DataTable dt = new DataTable();
                adapter.Fill(dt);

                GrdBooks.ItemsSource = dt.DefaultView;
            }
            catch (MySqlException ex)
            {
            
            }
        }
        ```
    7. DB데이터리더
        - DataReader :ExecuteReader()로 가져온 데이터를 조작하는 객체
        ```csharp
            //1. db연결문자열
            string connectionString = "Server=localhost;Database=madang;Uid=root;Pwd=12345;Charset=utf8";
            //2. 사용쿼리
            string query = "SELECT Division,Names FROM divtbl";

            List<KeyValuePair<string, string>> divisions = new List<KeyValuePair<string, string>>();

            //3. db연결, 명령, 리더
            using (MySqlConnection conn = new MySqlConnection(connectionString)) 
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
                        divisions.Add(new KeyValuePair<string, string> (division, name));
                        
                    }
                }
                catch (MySqlException ex)
                {
                
                }
                CboDivisions.ItemsSource = divisions;
            }
        ```
        

7. 실행결과
    - DATAGRID에서 더블클릭했을 때 이벤트- 선택한 그리드의 레코드값이 오른쪽 GROUPBOX에 출력함
    - <img src='./day64/그리드데이터를하나선택시상세정보를우측에.png'>

8. MahApps.Metro 방식 다이얼로그 처리
    ```csharp
     private async void GrdBooks_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
        //사용자가 선택한 행이 정확히 한 개인지 확인합니다.
        //DataGrid는 다중 선택이 가능하므로, 1개만 선택됐을 때만 처리하도록 조건을 줍니다.
        if (GrdBooks.SelectedItems.Count ==1) 
        {
            //DataRowView는 DataTable을 바인딩한 경우, 각 행을 나타냅니다.
            //GrdBooks.ItemsSource는 DataTable.DefaultView나 DataView 등으로 바인딩되어 있어야 합니다.
            //즉, DataGrid가 DataRowView 타입의 데이터를 보여주고 있어야 이 코드가 제대로 작동합니다.
            //데이터 그리드에 로드데이터할 때, adapter.Fill(dt)여서 가능함
            var item = GrdBooks.SelectedItems[0] as DataRowView;
            //MessageBox.Show(item.Row["Names"].ToString());

            NUD.Value = Convert.ToDouble(item.Row["Idx"]);
            CboDivisions.SelectedValue = Convert.ToString(item.Row["Division"]);
            TxtBookAuthor.Text = Convert.ToString(item.Row["Author"]);
            TxtBookName.Text= Convert.ToString(item.Row["Names"]);
            TxtISBN.Text = Convert.ToString(item.Row["ISBN"]);
            TxtPrice.Text = Convert.ToString(item.Row["Price"]);
            DP.Text = Convert.ToString(item.Row["ReleaseDate"]);


        }
        await this.ShowMessageAsync($"상세설명 제공완료!", "성공");
    }
     ```
    - <img src='./day64/다이얼로그.png'>
9. 여기까지 개발한 방식은 전통적인 C# 윈앱개발과 차이가 없음

### WPF MVVM
- [디자인 패턴](https://ko.wikipedia.org/wiki/%EC%86%8C%ED%94%84%ED%8A%B8%EC%9B%A8%EC%96%B4_%EB%94%94%EC%9E%90%EC%9D%B8_%ED%8C%A8%ED%84%B4)
    - 소프트웨어 공학에서 공통적으로 발생하는 문제를 재사용 가능하게 해결한 방식들
    - 반복적으로 되풀이 되는 개발디자인의 문제를 해결하도록 맞춤화시킨 양식(템플릿)
    - 여러 디자인패턴 중 아키텍쳐패턴, 그 중 디자인고 개발을 분리해 개발할 수 있는 패턴을 준비
        - MV* : MVC, MVP, MVVM...

- MVC : Model View Controller 패턴
    - 사용자 인터페이스(View)와 비즈니스 로직(Controller, Model)분리해서 앱을 개발
    - 디자이너에게 최소한의 개발에 참여를 시킴
    - 개발자는 디자인은 고려하지 않고 개발만 할 수 있음
    - 사용자는 Controller에게 요청
    - Controller가 Model에게 Data를 요청
    - Model이 DB에 데이터를 가져와 Controller로 전달
    - Controller는 데이터를 비즈니스로직에 따라서 처리하고 View로 전달
    - View는 데이터를 화면에 뿌려주고, 화면상에 처리할 것을 마친 후 사용자에게 응답
    <img src='./day64/mvc+db.png' width=600>
- MVP : Model-View-Presenter 패턴
    - MVC 패턴에서 파생됨
    - Presenter : Supervising Controller라고 부름
- MVVM : Model-View-ViewModel 패턴
    - MVC 패턴에서 파생됨
    - 마크업언어로 GUI코드를 구현하는 아키텍처
    - 사용자는 View로 접근(MVC와의 차이점)
    - ViewModel이 Controller역할(비즈니스로직 처리)
    - Model은 당연히 DB요청, 응답
    - 연결방식이 MVC와 다름
    - 전통적인 C#방식은 사용자가 이벤트발생시키기 때문에 발생시기를 바로 알 수 있음
    - MVVM방식은 C#이 변화를 주시하고 있어야 함. 상태가 바뀌면 변화를 줘야함
    - <img src='./day64/mvvm.png' width=600>
- MVVM  장단점
    - View <-> ViewModel 간 데이터 자동 연동
    - 로직 분리로 구조가 명확해짐. 자기할일만 하면 됨.
    - 팀으로 개발시 역할분담이 확실. 팀프로젝트에 알맞음
    - 테스트와 유지보수는 쉬움
    - 구조가 복잡. 디버깅이 어려움.
    - 스케일이 커짐

### WPF MVVM 연습 [(MVVM DB연동 디자인)](./day64/Day01Wpf/WpfBasicApp2/View/MainView.xaml) [(MVVM DB연동 소스)](./day64/Day01Wpf/WpfBasicApp2/ViewModel/MainViewModel.cs)
1. 프로젝트 생성 , app.xaml, MainWindow.xaml, MainWindow.axml.cs 수정
2. WPF 바인딩 연습시 사용한 MainWindow.xaml의 UI 복사
3. Model, View, ViewModel 폴더 생성
4. MainWindow.xaml을 View로 이동
5. ** App.xaml에서 StartupUri 수정**
    ```csharp
    StartupUri="/View/MainWindow.xaml"
    ```
6. Model폴더 내 Book.cs 생성 [Book.cs](./day64/Day01Wpf/WpfBasicApp2/Model/Book.cs)
    - **INotifyPropertyChanged 인터페이스 : 객체내의 어떠한 속성값이 변경되면 상태를 C#에게 알려주는 기능**
    - **PropertyChangedEventHandler 이벤트 생성**
    ```csharp
        public class Book : INotifyPropertyChanged
        {
            public int Idx {  get; set; }
            public string Names { get; set; }
            public string Author {  get; set; }
            public string Division {  get; set; }
            public string DNames { get; set; }

            public string ISBN {  get; set; }   
            public int Price {  get; set; }

            public DateTime ReleaseDate {  get; set; }
            
            //위의 여덟개의 값이 기존상태에서 변경이 되면 발생하는 이벤트
            //사용자가 클릭같은 거로 발생하는 이벤트가 아님
            public event PropertyChangedEventHandler? PropertyChanged;
        }
    ```

7. ViewModel폴더 내 MainViewModel.cs 생성
    - INotifyPropertyChanged 인터페이스 : 객체내의 어떠한 속성값이 변경되면 상태를 C#에게 알려주는 기능
    - PropertyChangedEventHandler 이벤트 생성

## 65일차(5/9)
### WPF MVVM 연습 [(MVVM DB연동 디자인)](./day64/Day01Wpf/WpfBasicApp2/View/MainView.xaml) [(MVVM DB연동 소스)](./day64/Day01Wpf/WpfBasicApp2/ViewModel/MainViewModel.cs)
8. View와 ViewModel 연동 준비작업
    - MainViewModel.cs 에 변화알림 이벤트 
    ```csharp

    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged(string propertyName)
    {   //기본적인 이벤트핸들러 파라미터와 동일(Object sender, EventArgs e)
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));        
    }

    ```
    - MainView.xaml에 연동
    ```xml
    <mah:MetroWindow 
    xmlns:vm="clr-namespace:WpfBasicApp2.ViewModel"
    DataContext="{DynamicResource MainVM}" >

    <mah:MetroWindow.Resources>
        <!--MainViewModel을 가져와서 사용하겠다.-->
        <vm:MainViewModel x:Key="MainVM"/>
    </mah:MetroWindow.Resources>


    ```

9. View-MainViewModel 연결(1) - MainViewModel에서 DB연동(divtbl데이터)하고 View(combobox장르 컨트롤)에서 데이터(장르)를 보여주기
    - MainViewModel.cs 코드 
        ```csharp
        //groupbox의 combobox에 넣을 데이터 저장할 리스트
        public ObservableCollection<KeyValuePair<string, string>> Divisions { get; set; }
        ```
       
        ```csharp
         //db연동코드
        private void LoadControlFromDb()
        {
            string connectionString = "Server=localhost;Database=madang;Uid=root;Pwd=12345;Charset=utf8";
            string query = "SELECT Division,Names FROM divtbl";

            ObservableCollection<KeyValuePair<string, string>> divisions = new ObservableCollection<KeyValuePair<string, string>>();

            using (MySqlConnection conn = new MySqlConnection(connectionString))
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
                        divisions.Add(new KeyValuePair<string, string>(division, name));
                    }
                }
                catch (MySqlException ex)
                {
                }

                Divisions = divisions;
                OnPropertyChanged(nameof(Divisions));
            }
        }
        ```
    - View의 MainView.xaml
    ```xml
    <!--combobox에 바인딩-->
    <ComboBox  ItemsSource="{Binding Divisions}">
    ```

10. View-MainViewModel 연결(2) - MainViewModel에서 DB연동(bookstbl데이터)하고 View(데이터그리드 컨트롤)에서 데이터를 보여주기
    - MainViewModel.cs 코드
    ```csharp
    //datagrid에 넣을 데이터 저장할 리스트
    public ObservableCollection<Book> Books { get; set; } 
    ```
    ```csharp
    //db연동
    // DATAGRID 컨트롤에 로드되는 데이터
    private void LoadGridFromDb()
    {
        string connectionString = "Server=localhost;Database=madang;Uid=root;Pwd=12345;Charset=utf8";
        string query = "SELECT b.Idx,b.Author,b.Division ,b.Names, b.ReleaseDate,b.ISBN,b.Price, d.Names as dNames FROM bookstbl b, divtbl d WHERE b.Division = d.Division order by b.Idx";

        ObservableCollection<Book> books = new ObservableCollection<Book>();

        using (MySqlConnection conn = new MySqlConnection(connectionString))
        {
            try
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand(query, conn);
                MySqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    books.Add(new Book
                    {
                        Idx = reader.GetInt32("Idx"),
                        Division = reader.GetString("Division"),
                        Author = reader.GetString("Author"),
                        Names = reader.GetString("Names"),
                        DNames = reader.GetString("DNames"),
                        ISBN = reader.GetString("ISBN"),
                        Price = reader.GetInt32("Price"),
                        ReleaseDate= reader.GetDateTime("ReleaseDate")
                    });
                }
            }
            catch (MySqlException ex)
            {
            }
            Books = books;
            OnPropertyChanged(nameof (Books));
        }
    }

    ```
    - View의 MainView.xaml
    ```xml
    <!--datagrid에 바인딩--> 
    <DataGrid  ItemsSource="{Binding Books}">

    <!-- datagrid 태그 안 각각의 속성 바인딩--> 
    <!--*주의할점: Books 속성에서 정의한 이름 그대로 써야함-->
    <DataGridTextColumn Binding="{Binding DNames}" Header="장르명" />

    ```

11. View-MainViewModel 연결(3) - View(데이터그리드 컨트롤)에서 선택한 행의 데이터를  View(groupbox컨트롤)에 보여주기
    - View의 MainView.xaml
    ```xml
    <!--datagrid에 선택아이템속성추가--> 
    <DataGrid  ItemsSource="{Binding Books}" SelectedItem="{Binding SelectedBook , Mode=TwoWay}">

    <!--groupbox의 각각의 컨트롤의 속성에 코드 추가--> 
    <mah:NumericUpDown     Value="{Binding SelectedBook.Idx}"/>
    <ComboBox SelectedValue="{Binding SelectedBook.Division}"></ComboBox>
    <DatePicker SelectedDate="{Binding SelectedBook.ReleaseDate}"></DatePicker>

    ```

    - MainViewModel.cs 코드
    ```csharp
    //선택한 행 담을 변수

    public Book _selectedBook;

    public Book SelectedBook
    {
        //람다식 표현, get{ return _selectedBook; }와 동일
        get => _selectedBook;
        
        set
        {
            _selectedBook = value;
            //값이 변경된 것을 알아차리도록 해줘야함!!
            OnPropertyChanged(nameof(SelectedBook));
        }
    }

    ```
    - `public Book Book { get; set; }는 멤버변수가 자동으로 정의된다.(public Book _book;)`
12. 실행결과
    

https://github.com/user-attachments/assets/42fa4f15-9cb1-43dc-a6b9-3c699bb0aa89


13. MainView.xmal 컨트롤에 바인딩 작업
    - [전통적인 C# 방식](./day64/Day01Wpf/WpfBasicApp1/MainWindow.xaml) : x:Name사용(MainView.xaml.cs에서 사용필요) , 마우스이벤트 추가 
    ```xml
    <DataGrid x:Name="GrdBooks" Grid.Row="0" Grid.Column="0" Margin="5" AutoGenerateColumns="False" IsReadOnly="True" MouseDoubleClick="GrdBooks_MouseDoubleClick">
    <DataGrid.Columns>
        <DataGridTextColumn Binding="{Binding Idx}" Header="순번"/>
        <DataGridTextColumn Binding="{Binding Names}" Header="책제목"/>
        <DataGridTextColumn Binding="{Binding ReleaseDate, StringFormat='yyyy-MM-dd'}" Header="출판일"/>
        <DataGridTextColumn Binding="{Binding Author}" Header="저자"/>
        <DataGridTextColumn Binding="{Binding Division}" Header="장르" Visibility="Hidden"/>
        <DataGridTextColumn Binding="{Binding dNames}" Header="장르명" />
        <DataGridTextColumn Binding="{Binding ISBN}" Header="ISBN"/>
        <DataGridTextColumn Binding="{Binding Price, StringFormat={}{0:N0}원}" Header="책가격"/>
    </DataGrid.Columns>
    </DataGrid>
    ```
    - [WPF MVVM 바인딩 방식](./day64/Day01Wpf/WpfBasicApp2/View/MainView.xaml) : 전부 Binding 사용
    ```xml
    <DataGrid  Grid.Row="0" Grid.Column="0" Margin="5" AutoGenerateColumns="False" IsReadOnly="True" ItemsSource="{Binding Books}" SelectedItem="{Binding SelectedBook , Mode=TwoWay}">
     <DataGrid.Columns>
         <DataGridTextColumn Binding="{Binding Idx}" Header="순번"/>
         <DataGridTextColumn Binding="{Binding DNames}" Header="장르명" />
         <DataGridTextColumn Binding="{Binding Names}" Header="책제목"/>
         <DataGridTextColumn Binding="{Binding ReleaseDate, StringFormat='yyyy-MM-dd'}" Header="출판일"/>
         <DataGridTextColumn Binding="{Binding Author}" Header="저자" Visibility="Hidden"/>
         <DataGridTextColumn Binding="{Binding Division}" Header="장르" Visibility="Hidden"/>
         <DataGridTextColumn Binding="{Binding ISBN}" Header="ISBN" Visibility="Hidden"/>
         <DataGridTextColumn Binding="{Binding Price, StringFormat={}{0:N0}원}" Header="책가격"/>
     </DataGrid.Columns>
    </DataGrid>
    ```
### MVVM Framework
- MVVM 개발자체가 어려움. 초기 개발시 MVVM 템풀릿을 만드는데 시간이 많이 소요. 난이도 있음
- 조금 쉽게 개발하고자 3rd Party에서 MVVM 프레임워크 사용
- 종류
    - Prism : MS계열에서 직접 개발. 대규모 앱 개발시 사용. 모듈화 잘 되어있음. 커뮤니티 활발. 
        - 진입장벽 높음
    - **Caliburn.Micro** : 경량화된 프레임워크. 쉽게 개발할 수 있음. Xaml바인딩 생략가능. 커뮤니티 주는 추세.
        - MahApps.Metro에서 사용 중
        - 디버깅이 어려움
    - MVVM Light Toolkit : 가장 가벼운 MVVM 입문용. 쉬운 Command 지원. 개발종료
        - 확장성이 떨어짐

    - **CommunityTookit.Mvvm** : MS공식 경량 MVVM. 단순, 빠름. 커뮤니티 등 매우 활발
        - 모듈기능이 없음 
        - NotifyOfPropertyChange를 사용할 필요없음

    - ReactiveUI : Rx기반 MVVM. 비동기. 스트림처리 강력. 커뮤니티가 활발
        - 진입장벽이 높음

### Caliburn.Micro
- [공식사이트](https://caliburnmicro.com/)
- [Github](https://github.com/Caliburn-Micro/Caliburn.Micro)


### Caliburn.Micro 학습 [Caliburn 학습](./day65/Day02Wpf/WpfBasicApp01/MainWindow.xaml)
1. WPF 애플리케이션 프로젝트 생성 , Nuget패키지 관리자에서 Caliburn.Micro 설치
2. App.xaml에서 StartupUri 삭제
3. **Models, Views, ViewModels 폴더** 생성
    - MainView.xaml, MainView.xaml.cs , MainViewModel.cs, Bootstrapper.cs 에서 네임스페이스 수정
4. ViewModel폴더 내에 MainViewModel 클래스 생성
    - MainView의 속하는 ViewModel은 반드시 MainViewModel라는 이름을 써야함
    ```csharp
    using Caliburn.Micro;

    namespace WpfBasicApp01.ViewModel
    {
        public class MainViewModel : Conductor<object>
        {

        }
    }
    ```
5. MainWindow.xaml를 View폴더 내로 이동 
6. MainWindow.xaml를 MainView.xaml로 이름변경(F2)
    ```xml
    <!--MainView.xaml-->
    <Window x:Class="WpfBasicApp01.View.MainView"
        xmlns:local="clr-namespace:WpfBasicApp01.View">
    ```
    ```csharp
    //MainView.xaml.cs
    namespace WpfBasicApp01.View
    {
        
        public partial class MainView : Window
        {
            public MainView()
            {
                InitializeComponent();
            }
        }
    }
    ```
7. Bootstrapper 클래스 생성
    ```csharp
    using Caliburn.Micro;
    using System.Windows;
    using WpfBasicApp01.View;
    using WpfBasicApp01.ViewModel;

    namespace WpfBasicApp01
    {
        public class Bootstrapper : BootstrapperBase
        {
            public Bootstrapper() 
            {
                Initialize();
            }

            protected override void OnStartup(object sender, StartupEventArgs e)
            {
                //base.OnStartup(sender, e);

                //App.xaml의 StartupUri와  동일한 일을 수행
                //MainViewModel과 동일한 이름의 View를 찾아서 바인딩 후 실행
                DisplayRootViewForAsync<MainViewModel>(); 
            }
        }
    }


    ```
8. App.xaml에서 resource 추가
```xml
 <Application.Resources>
     <ResourceDictionary>
         <ResourceDictionary.MergedDictionaries>
             <ResourceDictionary>
                 <local:Bootstrapper x:Key="bootstrapper"/>
             </ResourceDictionary>
         </ResourceDictionary.MergedDictionaries>
     </ResourceDictionary>
 </Application.Resources>
```
9. 컨트롤 연습
    ```xml
    <!--MainView.xaml-->
    <Label Content="{Binding Greeting}"  ></Label>
    <Button Content="클릭" Width="100" Height="30" cal:Message.Attach="SayMyName"></Button>
    ```
    ```csharp
    //MainViewModel.cs
    public string _greeting;
    public string Greeting 
    
    { get => _greeting;
        set 
        {
            _greeting = value;
            NotifyOfPropertyChange(() => Greeting);
        }
    
    }

    public MainViewModel() { Greeting = "Hello Caliburn Micro"; }
    
    public void SayMyName()
    {
        Greeting = "abcdefghijk";
    }
    ```
    - <img src='./day65/caliburn컨트롤실행1.png' width=500>
    - <img src='./day65/caliburn컨트롤실행2.png' width=500>

10. MahApps.Metro UI 적용
    - Nuget패키지관리자에서 MahApps.Metro 설치
    - App.xaml에 리소스 추가
    - MainView.xaml에 코드 추가
    - MainView.xaml.cs에 코드 추가

### Caliburn.Micro + MahApp.Metro + DB연동 [(디자인)](./day65/Day02Wpf/WpfBasicApp02/Views/MainView.xaml) [(소스코드)](./day65/Day02Wpf/WpfBasicApp02/ViewModels/MainViewModel.cs)
1. WPF 애플리케이션 프로젝트 생성
2. Nuget패키지에서 mahapps, mysql.data, caliburn 설치
3. Models, Views, ViewModels 폴더 생성
4. App.xaml 작성
```xml
<Application.Resources>
    <ResourceDictionary>
        <ResourceDictionary.MergedDictionaries>
            <ResourceDictionary>
                <local:Bootstrapper x:Key="bootstrapper"/>
            </ResourceDictionary>
            <!-- MahApps.Metro resource dictionaries. Make sure that all file names are Case Sensitive! -->
            <ResourceDictionary Source="pack://application:,,,/MahApps.Metro;component/Styles/Controls.xaml" />
            <ResourceDictionary Source="pack://application:,,,/MahApps.Metro;component/Styles/Fonts.xaml" />


            <!-- Theme setting -->
            <ResourceDictionary Source="pack://application:,,,/MahApps.Metro;component/Styles/Themes/Light.Pink.xaml" />
        </ResourceDictionary.MergedDictionaries>
    </ResourceDictionary>
</Application.Resources>
```
5. Models 폴더 내에 Book.cs 생성
```csharp
 public class Book 
 {
     public int Idx {  get; set; }
     public string Names { get; set; }
     public string Author {  get; set; }
     public string Division {  get; set; }
     public string DNames { get; set; }

     public string ISBN {  get; set; }   
     public int Price {  get; set; }

     public DateTime ReleaseDate {  get; set; }
   
 }
```
6. Views 폴더 내에 MainView.xaml, MainView.xaml.cs 생성
```xml
<mah:MetroWindow x:Class="WpfBasicApp02.Views.MainView"
        xmlns:mah="http://metro.mahapps.com/winfx/xaml/controls"
        xmlns:iconpacks="http://metro.mahapps.com/winfx/xaml/iconpacks"
        xmlns:cal="http://caliburnmicro.com"
      >
```
```xml
    <DataGrid  Grid.Row="0" Grid.Column="0" Margin="5" AutoGenerateColumns="False" IsReadOnly="True" ItemsSource="{Binding Books}" SelectedItem="{Binding SelectedBook , Mode=TwoWay}" >
        <DataGrid.Columns>
            <DataGridTextColumn Binding="{Binding Idx}" Header="순번"/>
            <DataGridTextColumn Binding="{Binding DNames}" Header="장르명" />
            <DataGridTextColumn Binding="{Binding Names}" Header="책제목"/>
            <DataGridTextColumn Binding="{Binding ReleaseDate, StringFormat='yyyy-MM-dd'}" Header="출판일"/>
            <DataGridTextColumn Binding="{Binding Author}" Header="저자" Visibility="Hidden"/>
            <DataGridTextColumn Binding="{Binding Division}" Header="장르" Visibility="Hidden"/>
            <DataGridTextColumn Binding="{Binding ISBN}" Header="ISBN" Visibility="Hidden"/>
            <DataGridTextColumn Binding="{Binding Price, StringFormat={}{0:N0}원}" Header="책가격"/>
        </DataGrid.Columns>
    </DataGrid>
    <GroupBox Grid.Row="0" Grid.Column="1" Margin="5" Header="상세">
        <Grid>
            <Grid.RowDefinitions>
                <RowDefinition Height="*"></RowDefinition>
                <RowDefinition Height="*"></RowDefinition>
                <RowDefinition Height="*"></RowDefinition>
                <RowDefinition Height="*"></RowDefinition>
                <RowDefinition Height="*"></RowDefinition>
                <RowDefinition Height="*"></RowDefinition>
                <RowDefinition Height="*"></RowDefinition>
            </Grid.RowDefinitions>
            <!--그룹박스 내 컨트롤-->
            <mah:NumericUpDown  Grid.Row="0" Maximum="100" Minimum="0"  Margin="3"
                   mah:TextBoxHelper.Watermark="순번"
                   mah:TextBoxHelper.AutoWatermark="True"
                   IsReadOnly="True"
                   Value="{Binding SelectedBook.Idx}"/>
            <ComboBox  Grid.Row="1" Margin="3"
            mah:TextBoxHelper.Watermark="장르"
            mah:TextBoxHelper.AutoWatermark="True"
            DisplayMemberPath="Value"
            SelectedValuePath="Key"
            ItemsSource="{Binding Divisions}"
            SelectedValue="{Binding SelectedBook.Division}"></ComboBox>
            <TextBox  Grid.Row="2" Margin="3" mah:TextBoxHelper.Watermark="책제목" Text="{Binding SelectedBook.Names}"></TextBox>
            <TextBox  Grid.Row="3" Margin="3" mah:TextBoxHelper.Watermark="책저자" Text="{Binding SelectedBook.Author}"></TextBox>
            <TextBox  Grid.Row="4" Margin="3" mah:TextBoxHelper.Watermark="ISBN" Text="{Binding SelectedBook.ISBN}"></TextBox>
            <DatePicker Grid.Row="5" Margin="3"  mah:TextBoxHelper.Watermark="출간일" SelectedDate="{Binding SelectedBook.ReleaseDate}"></DatePicker>
            <TextBox  Grid.Row="6" Margin="3" mah:TextBoxHelper.Watermark="책가격" Text="{Binding SelectedBook.Price}"></TextBox>

        </Grid>
    </GroupBox>
</Grid>
```
```csharp
using MahApps.Metro.Controls;
public partial class MainView : MetroWindow
```
7. ViewModels 폴더 내에 MainViewModel.cs 생성
- NotifyOfPropertyChange(() => Divisions); 이와 같이 간단하게 변화를 알릴 수 있다.
```csharp
using Caliburn.Micro;
using MahApps.Metro.Controls.Dialogs;
using MySql.Data.MySqlClient;
using System.Collections.ObjectModel;
using System.Windows;
using WpfBasicApp2.Models;

namespace WpfBasicApp02.ViewModels
{
    class MainViewModel : Conductor<object>
    {
       

        public MainViewModel()
        {
            LoadControlFromDb();
            LoadGridFromDb();
            DoAction();
        }

        //그룹박스의 콤보박스에 아이템 넣기 위해서
        private void LoadControlFromDb()
        {

                Divisions = divisions;
                NotifyOfPropertyChange(() => Divisions);
        }
        

        // DATAGRID 컨트롤에 로드되는 데이터
        private void LoadGridFromDb()
        {
            string connectionString = "Server=localhost;Database=madang;Uid=root;Pwd=12345;Charset=utf8";
           
                Books = books;
                NotifyOfPropertyChange(() => Books);

            }
        }

        public  void DoAction()
        {
            MessageBox.Show("테스트");
           
        }
    
}

```
8. Bootstrapper.cs 생성
```csharp
using Caliburn.Micro;
using System.Windows;
using WpfBasicApp02.ViewModels;

namespace WpfBasicApp02
{
    public class Bootstrapper : BootstrapperBase
    {
        public Bootstrapper() 
        {
            Initialize();
        }

        protected override void OnStartup(object sender, StartupEventArgs e)
        {
            DisplayRootViewForAsync<MainViewModel>();
        }
    }
}

```

9. 실행결과
<img src='./day65/caliburn과db연동.png'>

10. MahApps.Metro-DB연동-MVVM 정리
- 2가지 방식 비교 요약 -차이는 변화알림 부분, 나머지는 바인딩이나 마하디자인은 동일

|순서/방식|MahApps.Metro-DB연동-MVVM|MahApps.Metro-DB연동-MVVM+프레임 워크 Caliburn|
|:--:|:--:|:--:|
|1|WPF 애플리케이션 프로젝트 생성|WPF 애플리케이션 프로젝트 생성
|2|Nuget패키지에서 mahapps, mysql.data설치|Nuget패키지에서 mahapps, mysql.data, caliburn 설치
|3|Models, Views, ViewModels 폴더 생성|Models, Views, ViewModels 폴더 생성
|4|App.xaml 리소스 작성, App.xaml에서 StartupUri 수정|App.xaml 리소스 작성, App.xaml에서 StartupUri 삭제
|5|Models폴더 내 Book.cs 작성(INotifyPropertyChanged,PropertyChangedEventHandler)|Models폴더 내 Book.cs 작성
|6|View폴더 내 MainView.xaml작성(MahApps,UI,Binding), MainView.xaml.cs 작성(MahApps.Metro)|View폴더 내 MainView.xaml작성(MahApps,UI,Binding), Caliburn,  MainView.xaml.cs 작성(MahApps.Metro)
|7|ViewModel폴더 내 MainViewModel.cs작성(INotifyPropertyChanged,PropertyChangedEventHandler)|ViewModel폴더 내 MainViewModel.cs작성(Conductor<object>, NotifyOfPropertyChange(() =>속성명 ))
|8||Bootstrap.cs작성
|9|./day64/Day01Wpf/WpfBasicApp2[실습코드](./day64/Day01Wpf/WpfBasicApp2/ViewModel/MainViewModel.cs)|./day65/Day02Wpf/WpfBasicApp02[실습코드](./day65/Day02Wpf/WpfBasicApp02/ViewModels/MainViewModel.cs)|

## 66일차(5/12)
### 프레임워크 CommunityTookit.Mvvm 학습 [MahApps.Metro-DB연동-MVVM + CommunityTookit.Mvvm](./day66/Day03Wpf/WpfBasicApp1/ViewModels/MainViewModel.cs)
-  MahApps.Metro-DB연동-MVVM + CommunityTookit.Mvvm
1. WPF 애플리케이션 프로젝트 생성
2. Nuget패키지에서 mahapps, mysql.data , CommunityTookit.Mvvm 설치
3. Models, Views, ViewModels 폴더 생성
4. App.xaml 리소스 작성, App.xaml에서 StartupUri 삭제, App.xaml.cs에 startUp 이벤트 작성
```csharp
//App.xaml.cs
using System.Windows;
using WpfBasicApp1.ViewModels;
using WpfBasicApp1.Views;

public partial class App : Application
{
    private void Application_Startup(object sender, StartupEventArgs e)
    {
        var viewModel = new MainViewModel();
        var view = new MainView
        {
            DataContext = viewModel,
        };
        view.ShowDialog();
    }
}
```
5. Models폴더 내 Book.cs 작성 
```csharp
 public class Book
```
6. View폴더 내 MainView.xaml작성(MahApps,UI,Binding),  MainView.xaml.cs 작성(MahApps.Metro)
7. ViewModel폴더 내 MainViewModel.cs작성
```csharp
     public partial class MainViewModel : ObservableObject
    {
        #region communityToolkit 학습
        //멤버변수1
        private string _greeting;

        //속성1
        public string Greeting 
        { get => _greeting; 
            
          set => SetProperty(ref _greeting, value); //CommunityToolkit.Mvvm의 핵심
        }
        #endregion
      
        #region 생성자
        public MainViewModel()
        {
            // community프레임 학습-속성호출
            _greeting = "MainViewModel 생성자호출ㅡCommunity Frame 학습";

            //db연동
            LoadControlFromDb();
            LoadGridFromDb();
        }
        #endregion

        #region db연동
        private ObservableCollection<KeyValuePair<string, string>> _divisions;
        public ObservableCollection<KeyValuePair<string, string>> Divisions 
        { get=> _divisions;
            set=>SetProperty(ref _divisions, value);
        }

        private ObservableCollection<Book> _book;
        public ObservableCollection<Book> Books 
        { 
            get=> _book;
            set => SetProperty(ref _book, value);
        }
      
        private Book _selectedBook;
        public Book SelectedBook
        {
            get => _selectedBook;
            set => SetProperty(ref _selectedBook, value);
        }

        //그룹박스의 콤보박스에 아이템 넣기 위해서
        private void LoadControlFromDb()
        {
            string connectionString = "Server=localhost;Database=madang;Uid=root;Pwd=12345;Charset=utf8";
            string query = "SELECT Division,Names FROM divtbl";

            ObservableCollection<KeyValuePair<string, string>> divisions = new ObservableCollection<KeyValuePair<string, string>>();

            //3. db연결, 명령, 리더
            using (MySqlConnection conn = new MySqlConnection(connectionString))
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
                        divisions.Add(new KeyValuePair<string, string>(division, name));

                    }
                }
                catch (MySqlException ex)
                {

                }
                Divisions = divisions;
            }
        }

        // DATAGRID 컨트롤에 로드되는 데이터
        private void LoadGridFromDb()
        {
            string connectionString = "Server=localhost;Database=madang;Uid=root;Pwd=12345;Charset=utf8";
            string query = "SELECT b.Idx,b.Author,b.Division ,b.Names, b.ReleaseDate,b.ISBN,b.Price, d.Names as dNames FROM bookstbl b, divtbl d WHERE b.Division = d.Division order by b.Idx";

            ObservableCollection<Book> books = new ObservableCollection<Book>();

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    MySqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        books.Add(new Book
                        {
                            Idx = reader.GetInt32("Idx"),
                            Division = reader.GetString("Division"),
                            Author = reader.GetString("Author"),
                            Names = reader.GetString("Names"),
                            DNames = reader.GetString("DNames"),
                            ISBN = reader.GetString("ISBN"),
                            Price = reader.GetInt32("Price"),
                            ReleaseDate = reader.GetDateTime("ReleaseDate")
                        });

                    }
                }
                catch (MySqlException ex)
                {
                }
                Books = books;
            }
        }
        #endregion
    }
```
8. 실행결과
    - <img src='./day66/commnunity프레임워크,mahapps,db연동,mvvm.png'>

9. 아이콘 추가
    - 프로젝트명 오른쪽마우스- 속성 - 애플리케이션 - win32리소스- 아이콘 찾아보기- 확장자가 ico인 파일 넣기
    - MainView.xaml
    ```xml
       <mah:MetroWindow.IconTemplate>
       <DataTemplate>
           <iconpacks:PackIconFileIcons Kind="_4d" Margin="10,7,0,0" Background="White"/>
       </DataTemplate>
   </mah:MetroWindow.IconTemplate>
    ```
    - <img src='./day66/아이콘.png'>
    
 ### Log 라이브러리
 - 개발한 앱, 솔루션의 현재상태를 계속 모니터링하는 기능
 - Log 사용법
    - 직접 코딩 방식
    - 로그 라이브러리 사용방식
- Log 라이브러리  
    - **NLog** : 가볍고 쉽다. 빠름. 데스크톱
    - Serilog : 어려운 편. 빠름. 웹쪽
    - Log4net : Java의 로그를 .NET으로 이전. 느림. 웹쪽
    - ZLogger : 제일 최신(2021). 초고속. 게임서버


### NLog 라이브러리 사용 [NLog.config](./day66/Day03Wpf/WpfBasicApp1/NLog.config) [로그호출](./day66/Day03Wpf/WpfBasicApp1/ViewModels/MainViewModel.cs)
1. Nuget패키지관리자에서 NLog, NLog.Schema 설치
2. 새항목>XML파일>NLog.config 생성 > **속성 출력디렉토리로 복사를  항상복사**
3. NLog 로그 레벨 순서 (낮음 → 높음) Info < Debug < Warning < Error
    - MainViewModel.cs에서 로그호출할려고 하니 Info, Warn, Error, Fatal은 출력되는데 Debug, Trace는 출력이 안됨.
4. NLog.config 작성
```xml
<?xml version="1.0" encoding="utf-8" ?>
<nlog xmlns="http://www.nlog-project.org/schemas/NLog.xsd"
	  xmlns:xsi ="http://www.w3.org/XMLSchema-instance">
	<!--로그 저장위치 및 이름-->
	<targets>
		<target name="logfile" xsi:type="File" fileName="logs/app.log" 
				layout="${longdate} ${level:uppercase=true} ${logger} ${message}"></target>
		<target name="logconsole" xsi:type="Console"></target>
	</targets>
	
	<!--어떤 로그를 쓸지-->
	<rules>
		<logger name ="*" minlevel ="Info"   writeTo="logfile,logconsole"/>
	</rules>
</nlog>
```
5. MainViewModel.cs에서 속성만들기
```csharp
 //Nlog 객체 생성
 private readonly Logger _logger = LogManager.GetCurrentClassLogger();

 public MainViewModel()
{
    //로그
    _logger.Info("뷰모델 시작");
}
```

6. C:\Source\iot_csharp_wpf_2025\day66\Day03Wpf\WpfBasicApp1\bin\Debug\net8.0-windows\logs 에 app.log파일 생성
    - <img src='./day66/applog파일.png'>


### MVVM , MahApps, communityToolkit , NLog , DB연동 + CRUD 실습 [BOOKSHOP](./day66/Day03Wpf/WpfBookRentalShop01/ViewModels/MainViewModel.cs)
1. WPF 프로젝트 생성
2. Nuget패키지 - NLog, Mysql, MahApps, CommunityTookit 설치
3. Models, Views, ViewModels 폴더 생성
4. App.xaml 리소스 작성, App.xaml에서 StartupUri 삭제, App.xaml.cs에 startUp 이벤트 작성
5. Models폴더 내 Book.cs 작성 , Genre.cs 작성
    ```csharp
    private string _division;
    private string _names;

    public string Division 
    {  get => _division;
        set=>SetProperty(ref _division, value);
    }
    public string Name
    {
        get => _names;
        set => SetProperty(ref _names, value);
    }
    ```
6. View폴더 내 MainView.xaml작성(MahApps,UI,Binding),  MainView.xaml.cs 작성(MahApps.Metro)-**Binding과정 중요**
```xml
<Menu IsMainMenu="True" Style="{StaticResource MahApps.Styles.Menu}">
    <MenuItem Header="종료" Command="{Binding AppExitCommand}">
        <MenuItem.Icon>
            <iconpacks:PackIconBoxIcons Kind="SolidExit"/>
        </MenuItem.Icon>
    </MenuItem>
     <MenuItem Header="책장르관리" Command="{Binding ShowBookGenreCommand}">
     <MenuItem.Icon>
         <iconpacks:PackIconMaterialDesign Kind="Category"/>
     </MenuItem.Icon>
    </MenuItem>
</Menu>
```
7. ViewModel폴더 내 MainViewModel.cs작성-**[RelayCommand], view, viewModel연결**
    ```csharp
    [RelayCommand]
    public  void AppExit()
    {
        MessageBox.Show("종료합니다.");
    }

    [RelayCommand]
    public void ShowBookGenre()
    {
        //MessageBox.Show("책장르 관리");
        var vm = new BookGenreViewModel();
        var v = new BookGenreView { DataContext = vm };
        CurrentView = v;
        CurrentStatus = "책장르 관리";
    }

    
    ```
8. 하위 사용자 컨트롤 작업(1)BookGenre(View, ViewModel) - DELETE
 ```xml
 <DataGrid Grid.Row="1" Grid.Column="0" Margin="5" AutoGenerateColumns="False" IsReadOnly="True"
          ItemsSource="{Binding GenreList}"
          SelectedItem="{Binding SelectedGenre, Mode=TwoWay}">
    <DataGrid.Columns>
        <DataGridTextColumn Binding="{Binding Division}" Header="장르코드"/>
        <DataGridTextColumn Binding="{Binding Name}" Header="장르명"/>

    </DataGrid.Columns>
</DataGrid>
 ```

 ```csharp
 //db연동
  //db에서 읽어온 데이터 저장할 공간
 private ObservableCollection<Genre> _genreList;

  public ObservableCollection<Genre> GenreList
  {
      get => _genreList;
      set =>SetProperty(ref _genreList, value);
  }

 private void LoadGridFromDb()
 {
  
     string connectionString = "Server=localhost;Database=madang;Uid=root;Pwd=12345;Charset=utf8";
  
     string query = "SELECT Division,Names FROM divtbl";
     ObservableCollection<Genre> genres = new ObservableCollection<Genre>();

     //3. db연결, 명령, 리더
     using (MySqlConnection conn = new MySqlConnection(connectionString))
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
 ```
```csharp
//버튼(초기화, 저장, 삭제)
 [RelayCommand]
 public void SetInit() 
 {
     _isUpdate = false;
     SelectedGenre = null;
 }

 [RelayCommand]
 public void DelData()
 { 
     if (_isUpdate == false)
     {
         MessageBox.Show("선택된 데이터가 없습니다.");
         return;
     }



     string connectionString = "Server=localhost;Database=madang;Uid=root;Pwd=12345;Charset=utf8";

     string query = "Delete  FROM divtbl where Division =@Division";
     MessageBox.Show($"삭제 시도: Division = [{SelectedGenre.Division}]");

     using (MySqlConnection conn = new MySqlConnection(connectionString))
     {
         try
         {
             conn.Open();
             MySqlCommand cmd = new MySqlCommand(query, conn);
             cmd.Parameters.AddWithValue("@Division", SelectedGenre.Division);
             int resultCnt = cmd.ExecuteNonQuery();  //한 건 삭제하면 resultCnt=1, 안 지워지면 resultCnt=0

             if (resultCnt> 0)
             {
                 MessageBox.Show("삭제 성공");
                 LoadGridFromDb(); // 목록 갱신
                 SetInit();        // 선택 초기화
             }
             else
                 MessageBox.Show("삭제실패");
         }
         catch (MySqlException ex)
         {
             MessageBox.Show("DB 오류 발생: " + ex.Message);
         }



     }
 }
```

- 실행결과: 외래키 연결되어있는것은 삭제과정 중 에러남.
  

https://github.com/user-attachments/assets/fe96d571-f566-4859-aba5-e5f767f344ca

## 67일차(5/13)
8. 하위 사용자 컨트롤 작업(1)BookGenre(View, ViewModel) - INSERT, UPDATE [CRUD 추가기능](./day67/Day04Wpf/WpfBookRentalShop01/ViewModels/BookGenreViewModel.cs)
9. DB연결 CRUD 연습시 추가 필요사항
- [X] 여러번 나오는 로직 메서도화 -InitVariable함수 만들어서 SetInit(),생성자에서 호출
- [X] 공통화 작업, 연결문자열 Common으로 이전 - Helps폴더 내에 Common.cs 파일에서 자주사용하는 것들 정의(connectionString, NLog , Dialog)
- [X] NLog로 각 기능 동작시 로그남기기 - NLog.config(xmal, 항상복사설정) , CS파일에서 NLog호출
    ```csharp
        [RelayCommand]
        public void SetInit() 
        {

            InitVariable();
            Common.LOGGER.Info("초기화버튼");
        }
    ```
- [X] `메뉴탭- 종료메뉴 다이얼로그 MahApps.Metro 메시지형태로 변경 - MainView.xaml , App.xaml.cs , MainView.xaml.cs 설정`
```xml
<!--MainView.xaml-->
xmlns:Dialog ="clr-namespace:MahApps.Metro.Controls.Dialogs;assembly=MahApps.Metro"
Dialog:DialogParticipation.Register="{Binding}"
``` 
```csharp
// App.xaml.cs
 Common.DIALOGCOORDINATOR = DialogCoordinator.Instance;
 var viewModel = new MainViewModel(Common.DIALOGCOORDINATOR);
```
```csharp
  // MainView.xaml.cs
  private IDialogCoordinator _dialogCoordinator;

  public MainViewModel(IDialogCoordinator coordinator)
  {
      this._dialogCoordinator = coordinator;
  }

  [RelayCommand]
  public async Task AppExit()
  {
      var result = await this._dialogCoordinator.ShowMessageAsync(this, "종료확인", "종료하시겠습니까?", MessageDialogStyle.AffirmativeAndNegative);
      if (result == MessageDialogResult.Affirmative) //종료 ok
      {
          Common.LOGGER.Info("종료");
          Application.Current.Shutdown();  
      }
      else
      {
          return;
      }
  }
```


https://github.com/user-attachments/assets/b011016f-0826-4100-b496-89a751a4c833


- [X] `메뉴탭- 책장르관리 뷰- 저장버튼 다이얼로그 MahApps.Metro 메시지형태로 변경 - MainView.xaml.cs , BookGenreView.xaml, BookGenreView.xaml.cs (app.xaml.cs와 common.cs는 앞과정에서 한 거 그대로 씀)`
```csharp
//  MainView.xaml.cs
   [RelayCommand]
   public void ShowBookGenre()
   {
       //MessageBox.Show("책장르 관리");
       var vm = new BookGenreViewModel(Common.DIALOGCOORDINATOR);
       var v = new BookGenreView { DataContext = vm };
       CurrentView = v;
       CurrentStatus = "책장르 관리";
       Common.LOGGER.Info("책장르 관리");
   }
```
```xml
<!--BookGenreView.xaml-->
xmlns:Dialog ="clr-namespace:MahApps.Metro.Controls.Dialogs;assembly=MahApps.Metro"
Dialog:DialogParticipation.Register="{Binding}"
```
```csharp
// BookGenreView.xaml.cs
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

[RelayCommand]
public async void SaveData() 
{

    string query = string.Empty;

    using (MySqlConnection conn = new MySqlConnection(Common.CONNSTR))
    {
        try
        {
            conn.Open();

            //빈 데이터이 아닐 때, 새로 데이터 저장
            if (_isUpdate == false && !string.IsNullOrWhiteSpace(SelectedGenre.Division)
             && !string.IsNullOrWhiteSpace(SelectedGenre.Name))
 
            {
                query = "insert into divtbl values(@Division, @Name)";
            }
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
```

- [X] 삭제여부 메시지박스 추가 - BookGenre.xaml.cs에 삭제함수 코드 추가
```csharp
  var result = await this._dialogCoordinator.ShowMessageAsync(this, "삭제 전 확인", "삭제하시겠습니까?", MessageDialogStyle.AffirmativeAndNegative);
  if (result == MessageDialogResult.Affirmative) //삭제  ok
  {
      Common.LOGGER.Info("삭제동의완료");
  }
  else
  {
      return;
  }
```


https://github.com/user-attachments/assets/48b63f18-f0e2-484f-9f82-453c9c20d825



10. 하위 사용자 컨트롤 작업(2)Books(View, ViewModel) - BooksView.xaml, BooksViewModel.cs [CRUD 추가기능](./day67/Day04Wpf/WpfBookRentalShop01/ViewModels/BooksViewModel.cs)
    - 아래 11과 과정 동일
    - 주의할점 - comboBox에서  DisplayMemberPath="Value" SelectedValuePath="Key" 이기에 SelectedValue="{Binding SelectedBook.Division}"이어야 장르명이 화면에 표시됨
    ```xml
     <DataGrid.Columns>
     <DataGridTextColumn Binding="{Binding Idx}" Header="순번"/>
     <DataGridTextColumn Binding="{Binding DNames}" Header="장르명" />
     <DataGridTextColumn Binding="{Binding BNames}" Header="책제목"/>
     <DataGridTextColumn Binding="{Binding ReleaseDate, StringFormat='yyyy-MM-dd'}" Header="출판일"/>
     <DataGridTextColumn Binding="{Binding Author}" Header="저자" Visibility="Hidden"/>
     <DataGridTextColumn Binding="{Binding Division}" Header="장르" Visibility="Hidden"/>
     <DataGridTextColumn Binding="{Binding ISBN}" Header="ISBN" Visibility="Hidden"/>
     <DataGridTextColumn Binding="{Binding Price, StringFormat={}{0:N0}원}" Header="책가격"/>
    </DataGrid.Columns>

    <ComboBox  Grid.Row="1" Margin="3"
    mah:TextBoxHelper.Watermark="장르"
    mah:TextBoxHelper.AutoWatermark="True"
    ItemsSource="{Binding GenresList}"
    DisplayMemberPath="Value"
    SelectedValuePath="Key"
    SelectedValue="{Binding SelectedBook.Division}"></ComboBox>   
    ```
11. 하위 사용자 컨트롤 작업(3)Member(View, ViewModel) -Member.xaml, Member.cs , MemberViewModel.cs , MainViewModel.cs [CRUD 추가기능](./day67/Day04Wpf/WpfBookRentalShop01/ViewModels/MemberViewModel.cs)
     - Member.xaml에 ui, 다이얼로그 관련 코드, 마하앱 코드 
     - Member.xaml.cs에 뷰 관련 코드 
     ```csharp
        public MemberView()
        {
            InitializeComponent();
            this.DataContext = new MemberViewModel();  // 이 줄이 꼭 있어야 함
        }
     ```
     - MemberViewModel.cs에 db저장변수, 콤보박스저장변수, 다이얼로그변수, 선택된 아이템 변수
     - MemberViewModel.cs  생성자 2개 , MainViewModel코드 수정
     ```csharp
     // MemberViewModel.cs 
    //디자인 타임에서도 사용할 수 있도록 기본 생성자 오버로드를 추가
    public MemberViewModel() : this(DialogCoordinator.Instance) { }

    public MemberViewModel(IDialogCoordinator coordinator)
    {
        this._dialogCoordinator = coordinator;
        InitVariable();
        LoadGridFromDb();
        LoadComboFromDb();
    }

     ```
     ```csharp
     //MainViewModel.cs
        [RelayCommand]
        public void ShowMember()
        {
            var vm = new MemberViewModel(Common.DIALOGCOORDINATOR);
            var v = new MemberView { DataContext = vm };
            CurrentView = v;
            CurrentStatus = "회원 관리";
            Common.LOGGER.Info("회원 관리");
        }
     ```
     - 초기화함수, 데이터로드함수, 콤보박스데이터로드 함수
     - Member.cs 클래스
     - Member.xaml에 바인딩
     - 저장, 초기화, 삭제 버튼 함수 (CRUD)
     - 메시지박스 대신 다이얼로그 - async, await


12. 콤보박스 속성 
    - <img src = './day67/콤보박스바인딩속성비교.png'>


13. 실행결과
    

https://github.com/user-attachments/assets/d36d719e-49d9-4977-bfc2-29885e7e8f30


## 68일차(5/14)
14. 하위 사용자 컨트롤 작업(3)Rental(View, ViewModel) [(대여관리View)](./day67/Day04Wpf/WpfBookRentalShop01/Views/RentalView.xaml)  [(대여관리ViewModel)](./day67/Day04Wpf/WpfBookRentalShop01/ViewModels/RentalViewModel.cs)
- 실행결과
  

https://github.com/user-attachments/assets/56c8d09f-460c-4697-952c-00f2c0f35a11


### 영화즐겨찾기 앱 WITH  openAPI + Youtube API  [iot_wpf_2025_api repository-\day69\Day06Wpf\MovieFinder2025\Views\MovieViewModel]
15. 기능 디테일
- 기능
    - TMDB 사이트에서 제공하는 openAPI로 데이터 가져오기
    - 내가 좋아하는 영화리스트 선택, 즐겨찾기 저장
    - 저장한 영화만 리스트업
    - 선택된 영화목록 더블클릭> 영화 상세정보 팝업
    - 선택된 영화 선택 > 예고편 보기 > 유튜브 동영상 팝업

1. TMDB, Youtube API 준비
    - TMDB API 신청 [TMDB공식사이트](https://www.themoviedb.org/)  [참고사이트](https://0lrlokr.tistory.com/16)
    - Youtube Data API 신청  [구글 api](https://console.cloud.google.com/)
        - 프로젝트 생성
        - 목록 -API 및 서비스 - 라이브러리- Youtube Data API v3 - 사용 

2. WPF 애플리케이션 프로젝트 생성 및 Nuget패키지 라이브러리 설치(MahApp. communityToolkit. NLog. MySQL)   
3. 폴더 생성(Helps, Views, ViewModels, Models) 
4. App.xaml에서 startUp넣기, StartupUri 지우기 , 리소스 넣기
5. App.xaml.cs에서 startUp함수 정의
6. NLog- NLog.Config xml파일(항상복사) , Common.cs에 NLog인스턴스 선언
7. 공통화작업- Common.cs에 connectionString, Dialog 선언
8. 다이얼로그- MoviesView.xaml에 다이얼로그관련 코드 2개 추가 + MoviesViewModel.cs에 다이얼로그 관련 변수 및 생성자 추가 + App.xaml.cs에 다이얼로그 관련 코드 추가
9. 아이콘- MoviesView.xaml에 MahApps.iconpacks , 프로젝트명-속성-Window32 아이콘 찾아보기
10. MahApps.Metro-MoviesView.xaml에 마하 관련 코드 2개 추가 , 태그 수정 + MoviesView.xaml.cs에 MetroWindow import
11. 디자인
    - 상태바
    ```xml
    <!--상태바 영역-->
    <StatusBar Grid.Row="3" Grid.Column="0" Grid.ColumnSpan="2" >
        <StatusBarItem Content="TMDB &amp; Youtube API App"  Margin="10,0"/>
        <Separator Style="{StaticResource MahApps.Styles.Separator.StatusBar}"/>
        <StatusBarItem Content="{Binding}"/>
        <StatusBarItem Content="2025-05-14 오후 12:12:24" HorizontalAlignment="Right" Margin="0,0,10,0" />
    </StatusBar>
    ```
    - 이미지 속성-Source에 파일 첨부, stretch를 Fill로
    ```xml
    <Image Margin="10" Source="/Views/nopicture.png" Stretch="Fill" />
    ```
    - <img src='./day68/이미지태그속성.png'>
    
- <img src ='./day68/ui디자인.png' width = 600>

12. API 연동- TMDB API 구현
    - api호출해서 가져올 데이터를 담을 클래스 MovieItem.cs( 베테랑1 영화정보) , MovieSearchResponse.cs(동일한 영화명 여러 시즌일 때, 베테랑1, 베테랑2) 작성
    - MoviesViewModel.cs에서 api호출해서 데이터가져오는 함수 구현
    ```csharp
      //영화데이터 
    private ObservableCollection<MovieItem> _movieItems;
    public ObservableCollection<MovieItem> MovieItems
    {
        get => _movieItems;
        set => SetProperty(ref _movieItems, value);
    }

    private async void SearchMovie(string movieName)
    {
        string tmdb_apikey = "api키 입력하기";
        string encoding_moviename = HttpUtility.UrlEncode(movieName, Encoding.UTF8);  //입력한 한글을 UTF-8로 변경
        string openApiUri = $"https://api.themoviedb.org/3/search/movie?api_key={tmdb_apikey}" +
                            $"&language=ko-KR&page=1&include_adult=false&query={encoding_moviename}";

        string result = string.Empty;

        string result = string.Empty;

   
        HttpClient client = new HttpClient();
    
        ObservableCollection<MovieItem> movieItems = new ObservableCollection<MovieItem>(); 
        try
        {
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
    ```
    - MoviesView.xmal에서 바인딩
    ```xml
     <DataGrid Grid.Row="1" Grid.Column="0" Margin="5" IsReadOnly="True"  AutoGenerateColumns="False"
           ItemsSource="{Binding MovieItems}"
           SelectedItem="{Binding SelectedMovieItem, Mode=TwoWay}"
           Style="{StaticResource MahApps.Styles.DataGrid.Azure}">
     <DataGrid.Columns>
 
         <DataGridTextColumn Header="한글제목" FontWeight="Bold" Binding="{Binding Title}"></DataGridTextColumn>
    ```
    - <img src='./day68/클래스가2개인이유.png'>
12. URI -포스터 
    - 선택한 영화(SelectdMovieItem)이 있을 때, 영화포스터 보이도록
    - nopicture.png는 프로젝트폴더에 넣기
    - MoviesView.xaml 바인딩
    ```xml
    <Image Margin="10" Source="{Binding PosterUri}" Stretch="Fill" />
    ```

    - MoviesViewModel.cs 구현 
        
    ```csharp
    //포스터 변수
    private string _baseurl = "https://image.tmdb.org/t/p/w300_and_h450_bestv2";

    private Uri _posterUri;
    public Uri PosterUri 
    {
        get=> _posterUri;
        set => SetProperty(ref _posterUri, value);
    }

    //기본 포스터 화면
    public MoviesViewModel(IDialogCoordinator coordinator) 
    {
        this._dialogCoordinator = coordinator;
        PosterUri = new Uri("/nopicture.png" , uriKind: UriKind.RelativeOrAbsolute);
    }
    //선택된 영화일 때 포스터 화면
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
    ```
13. 기능 디테일
    - 숫자 오른쪽 정렬
    ```xml
    <DataGridTextColumn Header="평점" Binding="{Binding Vote_average , StringFormat=F2}">
    ```
    - enter키로 영화검색
    ```xml
    <TextBox Grid.Column="0" Margin="5,10" FontSize="14"
          mah:TextBoxHelper.Watermark="검색할 영화이름 입력"
          mah:TextBoxHelper.AutoWatermark="True"
          mah:TextBoxHelper.UseFloatingWatermark="True"
          mah:TextBoxHelper.ClearTextButton="True"
          Text="{Binding MovieName , UpdateSourceTrigger=PropertyChanged}">
     <TextBox.InputBindings>
         <KeyBinding Key="Return" Command="{Binding SearchMovieCommand}"/>
     </TextBox.InputBindings>
    </TextBox>

    ```
    - 영화검색 textbox에 포커스 +영화검색 언어 다양화
    ```xml
    <!--MovieView.xaml-->
    <mah:MetroWindow x:Class="MovieFinder2025.Views.MoviesView"
        FocusManager.FocusedElement="{Binding ElementName=TxtSearchMovie}">

    <!--검색 영역-->
    <Grid Grid.Row="0" Grid.Column="0">
        <Grid.ColumnDefinitions>
            <ColumnDefinition Width="3*"></ColumnDefinition>
            <ColumnDefinition Width="1*"></ColumnDefinition>
        </Grid.ColumnDefinitions>
        <TextBox x:Name="TxtSearchMovie" Grid.Column="0" Margin="5,10" FontSize="14"
          InputMethod.PreferredImeState="On"
          InputMethod.PreferredImeConversionMode="Native" />
    ```
    - <img src='./day68/영어입력검색.png' width=600>

14. 상세보기기능 -데이터그리드 더블클릭해서 영화정보 상세보기
    - communityToolkit에서는 지원하지 않음
    - `Nuget패키지관리자에서 Microsoft.Xaml.Behaviors.Wpf 설치`
    - MovieView.xml에서 datagrid 밑에 코드 추가
    ```xml
    <mah:MetroWindow xmlns:i="http://schemas.microsoft.com/xaml/behaviors" >

    <DataGrid Grid.Row="1" Grid.Column="0" Margin="5" IsReadOnly="True"  AutoGenerateColumns="False"
          ItemsSource="{Binding MovieItems}"
          SelectedItem="{Binding SelectedMovieItem, Mode=TwoWay}"
          Style="{StaticResource MahApps.Styles.DataGrid.Azure}">
    <i:Interaction.Triggers>
        <i:EventTrigger EventName="MouseDoubleClick">
            <i:InvokeCommandAction Command ="{Binding MovieItemDoubleClickCommand}"/>
        </i:EventTrigger>
    </i:Interaction.Triggers>
    ```
    - MoviesViewModel.cs에서 바인딩 정의
    ```csharp
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
    ```
- 실행결과


https://github.com/user-attachments/assets/6d225131-1427-4442-9868-6a37b8ecf123


## 69일차(5/15)
### 영화즐겨찾기 앱 WITH  openAPI + Youtube API  [iot_wpf_2025_api repository-\day69\Day06Wpf\MovieFinder2025\Views\TarilerViewModel]
15. 기능 디테일
    1. 오른쪽 하단 시계
    ```xml
    <StatusBarItem Content="{Binding CurrDateTime}" HorizontalAlignment="Right" Margin="0,0,10,0" />
    ```

    ```csharp
    // 상태바 현재시간
    private readonly DispatcherTimer _timer;
    private string _currDateTime;
    public string CurrDateTime
    {
        get => _currDateTime;
        set => SetProperty(ref _currDateTime, value);
    }

    public MoviesViewModel(IDialogCoordinator coordinator) 
    {
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
    ```
    2. 상태바에 검색결과 건수 표시
    ```xml
    <StatusBarItem Content="{Binding SearchResult}"/>
    ```

    ```csharp
    //검색건수

    private string _searchResult;
    public string SearchResult
    {
        get => _searchResult;
        set => SetProperty(ref _searchResult, value);
    }

    private async void SearchMovie(string movieName)
    {
       try{
            foreach (var movie in response.Results)
            {
                Common.LOGGER.Info(movie.Title);
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
    }

    ```
    3. nlog.config 수정 -날짜별로 로그파일 생성하도록
    ```xml
    <?xml version="1.0" encoding="utf-8" ?>
    <nlog xmlns="http://www.nlog-project.org/schemas/NLog.xsd"
        xmlns:xsi ="http://www.w3.org/XMLSchema-instance">
        <!--로그 저장위치 및 이름-->


        <targets>
            <target name="file" xsi:type="File" fileName="logs/app_${date:format=yyyyMMdd}.log"
                    layout="[${date}] [TID:${threadid}] [${level:uppercase=true}] ${message}"></target>
            <target name="console" xsi:type="ColoredConsole"
                    layout="[${date}] [TID:${threadid}] [${level:uppercase=true}] ${message}"></target>
        </targets>

        <!--어떤 로그를 쓸지-->
        <rules>
            <logger name ="*" minlevel ="Info"   writeTo="file"/>
            <logger name ="*" minlevel ="Info"   writeTo="console"/>
        </rules>
    </nlog>
    ```
16. 즐겨찾기 버튼 [즐겨찾기 버튼의 db연동 코드](./day69/Day06Wpf/MovieFinder2025/ViewModels/MoviesViewModel.cs)
    1. db에서 moviefinder스키마, movieItems테이블(movieItems.cs 속성과 동일한 컬럼) 만들기
    2. 즐겨찾기 추가 버튼 
    ```xml
    <Button Command="{Binding AddMovieInfoCommand}">
    ```
    ```csharp
     // 즐겨찾기 리스트인지 아닌지
    private bool _isFavoriteList = false;

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
        //  -------------db에 넣는 insert 쿼리 과정 
    }
    ```
    2. 즐겨찾기 보기 버튼
    ```xml
    <Button Command="{Binding ViewMovieInfoCommand}">
    ```

    ```csharp
    [RelayCommand]
    public async Task ViewMovieInfo()
    {
        MovieName = "";   //즐겨찾기 보기 버튼 클릭 시 검색창에 입력한 영화제목 지우기

        //검색창에서 즐겨찾기 온 경우
        if (_isFavoriteList ==false)
        {   //데이터 가져오는 동안 로딩화면을 다이얼로그로 
            var controller = await _dialogCoordinator.ShowProgressAsync(this, "즐겨찾기 보기", "데이터 가져오는 중...");
            controller.SetIndeterminate();
            ViewMovieInfoDetail();
            await Task.Delay(1000);
            await controller.CloseAsync();
        }
        //즐겨찾기 창에서 즐겨찾기 온 경우 , 그대로 유지
    }

    private async void ViewMovieInfoDetail()
    {
        // -------------db에서 즐겨찾기 목록 가져오는 select문
        MovieItems = movieList;

        //현재 즐겨찾기 페이지임을 가리키는 변수
        _isFavoriteList = true;

      
        //즐겨찾기가 있을 때, 포스터 이미지는 인덱스 0번째꺼로
        if (movieList.Count > 0)
        {
            SelectedMovieItem = movieList[0];
        }
        else
        {  //즐겨찾기가 없을 때, 포스터 이미지
             PosterUri = new Uri("/nopicture.png", uriKind: UriKind.RelativeOrAbsolute);
        }

        SearchResult = $"즐겨찾기검색 건수 {movieList.Count}건";
        Common.LOGGER.Info(SearchResult + "검색완료!!");

    }
    ```
    3. 즐겨찾기 삭제 버튼
    ```xml
    <Button Command="{Binding DelMovieInfoCommand}">
    ```
    ```csharp
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

    // -------------db에 있는 즐겨찾기 선택한 거 삭제하는 delete문

    //삭제 후 업데이트 된 즐겨찾기 보기위해서  
    ViewMovieInfoDetail();
    ```
17. 검색창에 영화이름 입력시 Release_date가 빈문자열이라서 예외가 발생하는 문제 - 범죄도시, 베테랑 등이 이 예외를 겪음
    - the JSON value could not be converted to system.nullable 에러는 release_date 필드가 JSON에서 빈 문자열 ""로 제공되었을 때 발생하는 문제입니다.
    - 이는 DateTime? (nullable DateTime) 필드에 빈 문자열을 DateTime으로 변환하려 할 때 발생합니다.

    - 커스텀 JsonConverter 작성
    ```csharp
    //SafeDateTimeConverter.cs
    public class SafeDateTimeConverter : JsonConverter<DateTime?>
    {
        public override DateTime? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.String)
            {
                var str = reader.GetString();
                if (string.IsNullOrWhiteSpace(str))
                    return null;  // 빈 문자열은 null로 처리

                if (DateTime.TryParse(str, out var date))
                    return date;  // 유효한 날짜 문자열을 DateTime으로 변환

                return null;  // 날짜 형식이 잘못된 경우 null 반환
            }

            return null;  // 다른 타입의 경우 null 반환
        }

        public override void Write(Utf8JsonWriter writer, DateTime? value, JsonSerializerOptions options)
        {
            if (value.HasValue)
                writer.WriteStringValue(value.Value.ToString("yyyy-MM-dd"));
            else
                writer.WriteNullValue();  // null 값은 "null"로 출력
        }
    }
    ```


    - MovieItem 클래스에서 커스텀 JsonConverter 적용
    ```csharp
    //MovieItem.cs
    [JsonConverter(typeof(SafeDateTimeConverter))]  // 커스텀 변환기 사용
    public DateTime? Release_date { get => _release_date; set => SetProperty(ref _release_date, value); }
    ```

    ```csharp
    //MoviesViewModel.cs
    foreach (var movie in response.Results)
    {
        Common.LOGGER.Info(movie.Title);
        movie.Release_date = movie.Release_date ?? new DateTime(0001, 1, 1);
        movieItems.Add(movie);
    }

    ```
    - <img src='./day69/예외처리-개봉일미정인영화.png'>
- 실행결과
  

https://github.com/user-attachments/assets/68a5e882-fe16-4654-a1ad-bc3c2301b952


18. 예고편 보기 버튼 [(예고편 뷰)](./day69/Day06Wpf/MovieFinder2025/Views/TrailerView.xaml) [(예고편 뷰모델)](./day69/Day06Wpf/MovieFinder2025/ViewModels/TrailerViewModel.cs)
    1. TrailerView.xaml , TrailerViewModel.cs 생성 + 마하, 다이얼로그 코드 작성 + ui디자인
    2. 웹 컨트롤 환경세팅
        - WPF 기본 WebBrower는 HTML5 기술이 표현이 안됨. 오류가 많음
        - Nuget패키지관리자에서 CefSharp.Wpf.NETCore 설치
        - 프로젝트명-오른쪽마우스-속성-빌드-일반-플랫폼대상 x64로
        - 솔루션명-오른쪽마우스-속성-구성관리자-활성솔루션 플랫폼 x64로 저장
    - 웹컨트롤 준비
    ```xml
    xmlns:cefsharp="clr-namespace:CefSharp.Wpf;assembly=CefSharp.Wpf"
    ```
    - 웹컨트롤 테스트
    ```xml
    <cefsharp:ChromiumWebBrowser Address="https://www.naver.com"/>
    ```

    3. 영화제목을 MovieViewModel.cs -> TrailerViewModel.cs로 전달
    ```csharp
    //MoiveViewModel.cs
     [RelayCommand]
    public async Task ViewMovieTrailer()
    {
        var movieTitle = SelectedMovieItem.Title;
       
        //예고편은 새 창에서 보이도록 하기 위해서 
        var viewModel = new TrailerViewModel(Common.DIALOGCOORDINATOR , movieTitle);
        var view = new TrailerView
        { 
            DataContext = viewModel,
        };
        view.ShowDialog();
    }

    ```
    ```xml
    //TrailerView.xaml
     <Label  Content="{Binding MovieTitle}" />
    ```

    ```csharp
    //TarilerViewModel.cs
    private string _movieTitle;
    public string MovieTitle
    {
        get => _movieTitle;
        set => SetProperty(ref _movieTitle, value);
    }

    public TrailerViewModel(IDialogCoordinator coordinator , string mvm )
    {
        this._dialogCoordinator = coordinator;
        MovieTitle = mvm;   
    }
    ```
    4. YouTube api로 받는 데이터 클래스 YoutubeItem.cs 생성
    5. Youtube api 환경 세팅 + youtube api 요청 응답 + 예고편 url
        - Nuget패키지관리자에서 Google.Apis.YouTube.v3 설치
        ```xml
        <!--TrailerView.xaml-->
        <ListView Grid.Row="1" Grid.Column="0" Margin="5" ItemsSource="{Binding YoutubeItems}" SelectedItem="{Binding SelectedYoutube}">
            <i:Interaction.Triggers>
                <i:EventTrigger EventName="MouseDoubleClick">
                    <i:InvokeCommandAction Command ="{Binding TrailerDoubleClickCommand}"/>
                </i:EventTrigger>
            </i:Interaction.Triggers>
                    <ListView.View>
                            <GridView>
                                <GridViewColumn Header="썸네일" Width="100">
                                    <GridViewColumn.CellTemplate>
                                        <DataTemplate>
                                            <Image Stretch="Fill" Source="{Binding Thumbanil}"/>
                                        </DataTemplate>
                                    </GridViewColumn.CellTemplate>
                                </GridViewColumn>
                                <GridViewColumn Header="타이틀" Width="auto" DisplayMemberBinding="{Binding Title}"></GridViewColumn>
                            </GridView>
                    </ListView.View>
        </ListView>

        <Grid Grid.Row="1" Grid.Column="1"  Margin="10">
            <cefsharp:ChromiumWebBrowser Address="{Binding YoutubeUri}"/>
        </Grid>
        ```

        ```csharp
        //TrailerViewModel.cs
        //유튜브API에서 가져온 정보들
        private ObservableCollection<YoutubeItem> _youtubeItems;
        public ObservableCollection<YoutubeItem>  YoutubeItems
        {
            get => _youtubeItems;
            set => SetProperty(ref _youtubeItems, value);
        }


          public TrailerViewModel(IDialogCoordinator coordinator , string mvm )
        {
            
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
                    ApiKey = "{youtube api키}",
                    ApplicationName = this.GetType().ToString()
                });
            var req = servie.Search.List("snippet");
            req.Q = $"{MovieTitle} 예고편 공식";  //영화이름으로 api 검색
            req.Order = SearchResource.ListRequest.OrderEnum.Relevance;
            req.Type = "video";
            req.MaxResults = 10;

            var res = await req.ExecuteAsync(); //api실행결과를 리턴(비동기)

       
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
       ```


       ```csharp
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


        public TrailerViewModel(IDialogCoordinator coordinator , string mvm )
        {
            
            MovieTitle = mvm;

            //초기화면은 유튜브 처음페이지
            YoutubeUri = "https:www.youtube.com";
            
            //YoutubeApi로 예고편 찾는 함수
            SearchYoutubeApi();
            
        }

        [RelayCommand]
        public  async Task TrailerDoubleClick()
        {
            YoutubeUri = SelectedYoutube.URL;
        }
        ```

- 실행결과

https://github.com/user-attachments/assets/923ef789-ebdb-4d49-8fc3-64a147d462c6


  
## 70일차 (5/16) 
### 부산맛집지도 앱 [iot_wpf_2025_api repository (day70/Day07Wpf/BusanRestaurantApp/)]
1. 공공데이터포털 -부산맛집정보서비스 api키
2. wpf애플리케이션 생성 + nuget 패키지
    - mahApps, communitytoolkit, nlog, 구성관리자 x64로->cefsharp.wpf.netcore 설치,Newtonsoft.Json
3. Helpers 폴더 생성 ,NLog.config(속성-항상복사)
4. App.xaml에서 startUp생성 , startUri 제거, 리소스 + BusanMatjipViewModel.cs 생성자
```csharp
//App.xaml.cs
   private void Application_Startup(object sender, StartupEventArgs e)
   {
       Common.DIALOGCOORDINATOR = DialogCoordinator.Instance;
       var viewModel = new BusanMatjipViewModel(Common.DIALOGCOORDINATOR);
       var view = new BusanMatjipView
       {
           DataContext = viewModel,
       };
       view.ShowDialog();
   }

   // BusanMatjipViewModel.cs
    private IDialogCoordinator _dialogCoordinator;

    public BusanMatjipViewModel(IDialogCoordinator coordinator)
    {
        this._dialogCoordinator = coordinator;
        Common.LOGGER.Info("BusanMatjip 시작");


    }
```
5. Views, ViewModels, Models , Helpers 폴더 생성 및 xaml, cs파일 생성
```
BusanRestaurantApp/
├── Views/
│   ├── BusanMatjipView.xaml
│   └── GoogleMapView.xaml
├── ViewModels/
│   ├── BusanMatjipViewModel.cs
│   └── GoogleMapViewModel
├── Models/
│   └── BusanItem.cs
├── Helpers/
│   └── Common.cs
└── NLog.config

```
6. BusanMatjipView.xaml 디자인
    - MahApps 코드 (xaml,cs)
    - Dialog 코드 (xaml)
    - 아이콘 설정
    - UI
    - 바인딩 
    ```xml
    <TextBlock Text="페이지번호" />
    <mah:NumericUpDown Minimum="1" Value="{Binding PageNo}"/>

    <TextBlock Text="결과수"/>
    <mah:NumericUpDown Minimum="10" Value="{Binding NumOfRows}"/>

    <Button Content="검색" Command="{Binding LoadDataCommand}">

    <!--데이터 그리드 영역-->
    <DataGrid  AutoGenerateColumns="True" SelectionMode="Single" SelectionUnit="FullRow" IsReadOnly="True"
               ItemsSource="{Binding BusanItems}">
    </DataGrid>
    ```

7. BusanItem.cs 클래스 
    - api 요청문
    ```
    http://apis.data.go.kr/6260000/FoodService/getFoodKr?serviceKey={api decode 키}&numOfRows={한페이지에 나타나는 행의 수}&pageNo={페이지번호}
    ```
    - api 응답결과
    ```xml
      <item>
        <MAIN_TITLE>만드리곤드레밥</MAIN_TITLE>
        <LNG>128.95245</LNG>
        <UC_SEQ>70</UC_SEQ>
        <CNTCT_TEL>051-941-3669</CNTCT_TEL>
        <MAIN_IMG_NORMAL>https://www.visitbusan.net/uploadImgs/files/cntnts/20191209162810545_ttiel</MAIN_IMG_NORMAL>
        <ITEMCNTNTS>곤드레밥에는 일반적으로 건조 곤드레나물이 사용되는데, 이곳은 생 곤드레나물을 사용하여 돌솥밥을 만든다. 된장찌개와 함께 열 가지가 넘는 반찬이 제공되는 돌솥곤드레정식이 인기있다 </ITEMCNTNTS>
        <PLACE>만드리곤드레밥</PLACE>
        <SUBTITLE/>
        <ADDR2/>
        <USAGE_DAY_WEEK_AND_TIME>10:00-20:00 (19:50 라스트오더)</USAGE_DAY_WEEK_AND_TIME>
        <GUGUN_NM>강서구</GUGUN_NM>
        <ADDR1>강서구 공항앞길 85번길 13</ADDR1>
        <RPRSNTV_MENU>돌솥곤드레정식, 단호박오리훈제</RPRSNTV_MENU>
        <HOMEPAGE_URL/>
        <TITLE>만드리곤드레밥</TITLE>
        <MAIN_IMG_THUMB>https://www.visitbusan.net/uploadImgs/files/cntnts/20191209162810545_thumbL</MAIN_IMG_THUMB>
        <LAT>35.177387</LAT>
        </item>
                
    ```
    - api응답결과를 담을 BusanItem.cs 클래스 속성
    ```csharp
    //콘텐츠id
    public int UC_SEQ { get; set; }
    //콘텐츠명
    public string MAIN_TITLE { get; set; }
    //구군
    public string GUGUN_NM { get; set; }
    //주소
    public string ADDR1 { get; set; }
    //위도
    public double LNG { get; set; }
    //경도
    public double LAT { get; set; }
    //연락처
    public string CNTCT_TEL { get; set; }
    //운영 및 시간
    public string USAGE_DAY_WEEK_AND_TIME { get; set; }
    //대표메뉴
    public string RPRSNTV_MENU { get; set; }

    //썸네일 이미지 
    public string MAIN_IMG_THUMB {  get; set; }

    //상세내용
    public string ITEMCNTNTS {  get; set; }
    ```
8. BusanMatjipView.xaml.cs 코드 
    - 속성 (다이얼로그 _dialogCoordinator, api응답결과 부산맛집 리스트 BusanItems, 페이지번호 PageNo, 결과행수 NumOfRows)
    - 생성자
    ```csharp
    public BusanMatjipViewModel(IDialogCoordinator coordinator)
    {
        this._dialogCoordinator = coordinator;
       
        // 초기값 설정
        PageNo = 1;
        NumOfRows = 10;
    }
    ```
    - api에 요청하고 응답받는 함수 LoadData
    ```csharp
    [RelayCommand]
    private async Task LoadData()
    {
        string myApiKey = "{공공데이터포털 api키}";
        string baseUri = "http://apis.data.go.kr/6260000/FoodService/getFoodKr?";
        StringBuilder strUri = new StringBuilder();
        strUri.Append($"serviceKey={myApiKey}");
        strUri.Append($"&numOfRows={NumOfRows}");
        strUri.Append($"&pageNo={PageNo}");
        string totalOpenApi = $"{baseUri}{strUri}";
            HttpClient client = new HttpClient();
        ObservableCollection<BusanItem> busanItems = new ObservableCollection<BusanItem>();

        try
        {
            var response = await client.GetStringAsync(totalOpenApi);
            //Common.LOGGER.Info(response);

            //Newtonsoft.json으로 json처리 방식
            /*
              API 응답은 XML 형식이고, JObject.Parse()는 JSON 전용 파서이기 때문입니다.
               그래서 XML을 JSON 문자열로 변환한 다음에야 JObject.Parse()를 사용할 수 있습니다.
             */
            var doc = XDocument.Parse(response);
            string jsonText = JsonConvert.SerializeXNode(doc);

            // 이제 jsonText는 JSON 문자열입니다.
            var jsonResult = JObject.Parse(jsonText);
            //Common.LOGGER.Info(jsonResult);

            // ?(null 조건 연산자)를 붙이는 이유:  중간에 하나라도 없으면 null 반환 후 다음 접근을 중단하므로 예외 없이 안전하게 처리됩니다.
            //.ToString()을 붙이는 이유: JToken은 JSON 구조 안에 있는 모든 값의 기본 타입입니다.
            var resultCode = jsonResult["response"]?["header"]?["resultCode"]?.ToString();


            if (resultCode == "00")
            {

                // Check if "body" and "items" exist before proceeding
                var body = jsonResult["response"]?["body"];
                if (body != null)
                {
                    var items = body["items"];
                    if (items != null)
                    {
                        var jsonArray = items["item"] as JArray;
                        if (jsonArray != null)
                        {
                            foreach (var itemDetail in jsonArray)
                            {
                                //Common.LOGGER.Info(itemDetail.ToString());
                                busanItems.Add(
                                    new BusanItem
                                    {
                                        UC_SEQ = Convert.ToInt32(itemDetail["UC_SEQ"]),
                                        MAIN_TITLE = Convert.ToString(itemDetail["MAIN_TITLE"]),
                                        GUGUN_NM = Convert.ToString(itemDetail["GUGUN_NM"]),
                                        ADDR1 = Convert.ToString(itemDetail["ADDR1"]),
                                        LNG = Convert.ToDouble(itemDetail["LNG"]),
                                        LAT = Convert.ToDouble(itemDetail["LAT"]),
                                        CNTCT_TEL = Convert.ToString(itemDetail["CNTCT_TEL"]),
                                        USAGE_DAY_WEEK_AND_TIME = Convert.ToString(itemDetail["USAGE_DAY_WEEK_AND_TIME"]),
                                        RPRSNTV_MENU = Convert.ToString(itemDetail["RPRSNTV_MENU"]),
                                        MAIN_IMG_THUMB = Convert.ToString(itemDetail["MAIN_IMG_THUMB"]),
                                        ITEMCNTNTS = Convert.ToString(itemDetail["ITEMCNTNTS"]),
                                    }
                                    );
                            }
                            BusanItems = busanItems;
                        }
                        else
                        {
                            Common.LOGGER.Warn("Item array is empty or null.");
                        }
                    }
                    else
                    {
                        Common.LOGGER.Warn("'items' is null.");
                    }
                }
                else
                {
                    Common.LOGGER.Warn("'body' is null.");
                }
            }
            else
            {
                Console.WriteLine($"오류 코드: {resultCode}");
            }
        }

        catch (Exception ex)
        {
            await _dialogCoordinator.ShowMessageAsync(this, "api 요청 결과", ex.Message);
            Common.LOGGER.Fatal(ex);
            return;
        }
    }

    ```
- 실행결과



https://github.com/user-attachments/assets/a0ec1f67-d0e3-4acb-8e8e-54e49d2abf61



- 로그결과
    - api응답결과 response (xml형식)
    - <img src='./day70/응답결과xml형식.png'>
    - response가 json으로 변환된 jsonResult
    - <img src='./day70/json으로 변환한 응답결과.png'>

## 71일차 (5/19)
### 부산맛집지도 앱 [iot_wpf_2025_api repository (day71/Day08Wpf/BusanRestaurantApp/)]
- BusanMatjipView.xaml 데이터그리드 컬럼들 디자인
    - DataGridTemplateColumn. DataGridTextColumn
    ```xml
    <DataGridTemplateColumn Header="대표이미지" Width="auto">
        <DataGridTemplateColumn.CellTemplate>
            <DataTemplate>
                <Image Source="{Binding MAIN_IMG_THUMB}" Stretch="Uniform" Width="65" Height="50"/>
            </DataTemplate>
        </DataGridTemplateColumn.CellTemplate>
    </DataGridTemplateColumn>

    <DataGridTemplateColumn Header="주소" Width="220">
        <DataGridTemplateColumn.CellTemplate>
            <DataTemplate>
                <TextBlock Text="{Binding ADDR1}" TextWrapping="Wrap" MaxWidth="220"/>
            </DataTemplate>
        </DataGridTemplateColumn.CellTemplate>
    </DataGridTemplateColumn>
    ```
- 구글지도 뷰, 뷰모델
    1. 부산맛집 뷰,뷰모델에서 데이터그리드 더블클릭시 뷰이동
    ```xml
    <DataGrid  ItemsSource="{Binding BusanItems}"  SelectedItem="{Binding SelectedMatjipItem}">
    <i:Interaction.Triggers>
        <i:EventTrigger EventName="MouseDoubleClick">
            <i:InvokeCommandAction Command="{Binding MatgibItemDoubleClickCommand}"></i:InvokeCommandAction>
        </i:EventTrigger>
    </i:Interaction.Triggers>
    ```
    ```csharp
     [RelayCommand]
    private async Task MatgibItemDoubleClick()
    {
        
        var viewModel = new GoogleMapViewModel();
        viewModel.SelectedMatjipItem = SelectedMatjipItem;
        var view = new GoogleMapView { 
            DataContext = viewModel,
        };
        view.Owner=Application.Current.MainWindow;
        view.WindowStartupLocation = WindowStartupLocation.CenterScreen;
        Common.LOGGER.Info("현재 선택한 맛집에 대한 구글맵 뷰 시작");
        Common.LOGGER.Info($"위도:{SelectedMatjipItem.LAT}, 경도:{SelectedMatjipItem.LNG}");
        view.ShowDialog();
    }
    ```
    2. 구글맵 xml
    ```xml
    xmlns:cefsharp="clr-namespace:CefSharp.Wpf;assembly=CefSharp.Wpf"

    <Label Grid.Row="0" FontSize="20" FontWeight="Bold" Margin="5" Content="{Binding SelectedMatjipItem.MAIN_TITLE }"></Label>
    <cefsharp:ChromiumWebBrowser Grid.Row="1" Address="{Binding MatjibLocation}" Margin="5"></cefsharp:ChromiumWebBrowser>
    ```
    3. 구글맵 csharp
    ```csharp
    //선택한 맛집
    private BusanItem _selectedMatjipItem;
    public BusanItem SelectedMatjipItem
    {
        get => _selectedMatjipItem;
        set
        {
            SetProperty(ref _selectedMatjipItem, value);
            MatjibLocation = $"https://google.com/maps/place/{SelectedMatjipItem.LAT},{SelectedMatjipItem.LNG}";
        }
    }


    // MatjibLocation
    private String _matjibLocation;

    
    public String MatjibLocation
    {
        get => _matjibLocation;
        set => SetProperty(ref _matjibLocation, value);
        
        
    }

    public GoogleMapViewModel()
    {
        MatjibLocation = "";
    }
    ```
- 실행결과
  

https://github.com/user-attachments/assets/4882d33e-aeba-4006-a69e-41b23e1778e7


### ==================================================
### mvvm , mvvm + caliburn, mvvm + commnunity프레임워크 비교 
- mvvm 
    - xaml파일 : DynamicResource ,x:Class="WpfBasicApp2.View.MainView" 
    - cs파일 : PropertyChangedEventHandler ,OnPropertyChanged
    - app.xaml의 startUri 재정의(/View/MainView.xaml)
    - <img src='./day70/mvvm.png'>
- mvvm + caliburn 
    - Conductor, NotifyOfPropertyChange, bootstrapper 
    - app.xaml의 startUri삭제 및 startUp 정의
    - Views, ViewModels, Models
    - <img src ='./day70/mvvm caliburn 1.png'>
    - <img src ='./day70/mvvm caliburn2.png'>
- mvvm + communityToolkit 
    - SetProperty, [RelayCommand]  
    - app.xaml의 startUri삭제 및 startUp 정의
    - Views, ViewModels, Models
    - <img src ='./day70/mvvm community.tool.kit.png'>
### ==================================================
### db연동, api연동 
- db연동 - 데이터베이스, 클래스 필요
    - select
        - <img src='./day70/select db.png'>
    - insert
        - <img src='./day70/insert db.png'>
    - delete
        - <img src='./day70/delete db.png'>
    - update

- api연동 - api키, 클래스 필요
    - TMDB api호출
        - <img src='./day70/TMDB api호출.png'>
    - Youtube API 호출
        - <img src='./day70/Youtube API 호출.png'>
    - 공공데이터포털 API호출
        - <img src='./day70/공공데이터포털 API호출.png'>
### ==================================================
### 스마트홈 연동 모니터링 앱
##### 구성
- 전면부 
    - <img src='./day71/전면부.jpg' width =500>
- 후면부
    - <img src='./day71/후면부.jpg'  width =500>

- 수치
    - <img src='./day71/수치.jpg'  width =500>
##### MQTT 
- Message Queueing Telemetry Transport : 기계간 통신용 경량 메시징 프로토콜
- Publish/Subscribe 라는 출판쪽 용어로 사용
    - publish (출간) : 메시지를 만들어서 전달
    - subscribe(구독) : 필요한 메시지를 수신받아서 사용
- Server/Client 프로그램으로 동작
- 데이터는 휘발성: 받는 사람이 없으면 데이터는 사라짐. DB에 저장하는 구성을 해줘야 함
- <img src='./day71/MQTT.png'>

#### MQTT 시뮬레이션 프로젝트 시작
1. MQTT 브로커 설치
    - https://mosquitto.org/download/에서 Binary Installation- Windows- mosquitto-2.0.21a-install-windows-x64.exe 설치
    - 설치 후 window+R에 services.msc 입력해서 Mosquitto Broker 찾아서 서비스 중지
2. 설정파일 수정
    - notdpad++ 관리자모드로 열기 - 설치된 경로 C:\Dev\tool\mosquitto에 있는  mosquitto.conf를 파일 열기
    - 212줄에 있는 #listeners 수정 => Listeners 1883 
    ```
    # MQTT 디폴트 포트번호 1883
    Listeners 1883
    ```
    - 534줄에 있는 allow _anonymous false 수정 =>  allow_anonymous true
    - 파일저장 후 , 서비스 재시작
3. Window보안
    - 방화벽 및 네트워크 보호- 고급설정 또는 제어판-windows defender방화벽-고급설정
    - 인바운드 규칙- 새규칙- 포트 - TCP , 특정로컬포트 1883 - 연결허용- 3항목 다 체크- 이름, 설명 작성 - 마침

4. MQTT Explorer 설치
    - https://mqtt-explorer.com/ 에서 download - window platform - installer
    - <img src='./day71/MQTT EXPLORER 설정.png'>
    - window+r의 services.msc에서 mosquitto.service 시작하기
    - 위의 방법이 안 될 경우, powershell에서 아래의 코드 실행 후 explorer에서 connect 시도하기
    ```shell
    PS C:\Users\Admin> cd C:\Dev\tool\mosquitto
    PS C:\Dev\tool\mosquitto> .\mosquitto.exe -v -c C:\Dev\tool\mosquitto\mosquitto.conf
    ```
    - <img src='./day71/실행화면.png' width =500>
5. vs code에서 Mqtt.py파일 생성
    - 모듈설치
    ```shell
    PS C:\Source\iot_csharp_wpf_2025> python --version
    Python 3.11.9
    PS C:\Source\iot_csharp_wpf_2025> python
    >>> import sys
    >>> sys.executable
    'C:\\Users\\Admin\\.pyenv\\pyenv-win\\versions\\3.11.9\\python.exe'
    >>> exit()

    PS C:\Source\iot_csharp_wpf_2025> pip install paho-mqtt
    ```
    - 코드작성[paho-mqtt 라이브러리를 이용해 MQTT 브로커에 메시지를 발행(Publish)](./day71/Pythons/MqttPub.py)
    ```python
    #IOT110 클라이언트가 "무한루프로 MQTT 메시지를 Publish만 하는" 구조
    # pip install paho-mqtt

    import paho.mqtt.client as mqtt
    import json
    import datetime as dt
    import uuid
    from collections import OrderedDict
    import random
    import time   #스레드 위한 timer

    #power shell- ipconfig -  IPv4 주소 . . . . . . . . . : 210.119.12.110
    PUB_ID = 'IOT110'                     # 클라이언트 ID (e.g., "IOT110")
    BROKER = '210.119.12.110'            # 브로커 IP (자기 컴퓨터)
    PORT = 1883                          # MQTT 기본 포트
    TOPIC = 'smarthome/110/topic'        # MQTT 토픽
    COLORS = ['RED', 'ORANGE', 'YELLOW', 'GREEN', 'BLUE', 'NAVY', 'PUPPLE']
    COUNT = 0                            # 메시지 카운터

    #브로커 연결 성공 시 실행됨
    def on_connect(client, userdata, flags, reason_code, properties = None):
        print(f'Connectedc with reason code : {reason_code}')

    #publish완료후 발생 콜백 /메시지 발행 완료 시 실행됨
    def on_publish(client, userdata,mid) :
        print(f'Message published mid : {mid}')


    try :
        
        #MQTT 클라이언트 생성 후 콜백 등록
        client = mqtt.Client(client_id=PUB_ID , protocol=mqtt.MQTTv5 )
        client.on_connect = on_connect
        client.on_publish = on_publish

        client.connect(BROKER ,PORT)
        client.loop_start()

        while True :
            #publish실행
            #매초마다 랜덤 색상을 선택하고, 타임스탬프와 함께 메시지 발행
            #qos=1 : 적어도 한 번은 전송됨 (보장성 있는 전달)

            currTime = dt.datetime.now()
            selected = random.choice(COLORS)
            COUNT += 1
            client.publish(TOPIC, payload = f'Emulate from {PUB_ID}[{COUNT}] :  {selected} /{currTime}', qos = 1)
            time.sleep(1)

    except Exception as ex:
        print(f"Error raised: {ex}")

    #Ctrl + C 누르면 안전하게 전송을 중단하고 연결 종료
    except KeyboardInterrupt :
        print('MQTT 전송중단')
        client.loop_stop()
        client.disconnect()

    ```
6. 실행결과


https://github.com/user-attachments/assets/ef4500c0-f1fe-41b9-ac38-ee087f1b54dc


#####  프로젝트 준비 [개발코드 강사깃허브](https://github.com/hugoMGSung/hungout-with-arduino/tree/main/SmartHomeDIY)
1. day08폴더 내에 다운받은 코드 압축 풀기 
2. day08에서 프로젝트 생성
3. 솔루션 탐색기에서 솔루션 이름 우클릭 → "기존 프로젝트 추가(Add Existing Project)" →압축 해제한 폴더 내 .csproj 또는 .vbproj 선택
4. 프로젝트가 솔루션에 추가됨 확인 및 솔루션 빌드

##### 프로젝트 시작
1. 화면 UI 변경
2. Nuget패키지 - CommunityTookit 설치
3. Views, ViewModels, Models폴더 생성
4. Views에 MainWindow 넣기 + ViewModels에 MainViewModel.cs 생성
5. namespace수정(WpfSmartHomeApp.Views.MainWindow , WpfSmartHomeApp.ViewModels.MainViewModel)
6. App.xaml의 StartupUri제거 및 startUp함수 cs파일에서 정의 
7. MianWindow.xaml에 Loal생성 및 MianWindow.xaml.cs에서 함수정의 +MianWindow에서 윈도우_로드함수 호출시, MainViewModel의 load함수 호출되도록 연결
```xml
<!--MianWindow.xaml-->
<Window 
xmlns:vm="clr-namespace:WpfSmartHomeApp.ViewModels"
Loaded="window_Loaded">
```
```csharp
//MianWindow.xaml.cs
private void window_Loaded(object sender, RoutedEventArgs e)
{
    if (DataContext is MainViewModel vm)
    {
        vm.LoadedCommand.Execute(null); 
    }
}
```

```csharp
//MainViewModel.cs
[RelayCommand]
public void OnLoaded()
{
    HomeTemp = 30;
    HomeHumid = 43.2;

    DetectResult = "Detected Human!";
    IsDetectOn = true;
    RainResult = "Raining";
    IsRainOn = true;
    ConditionerResult = "Aircon On!";
    IsConditionerOn = true;
    LightResult = "Light On!";
    IsLightOn = true;
}
```

8. MainWindow 바인딩 처리, MainViewModel.cs에서 바인딩 값들 초기화
- <img src='./day71/스마트홈 ui.png' width =500>


## 72일차(5/20)
### 스마트홈 연동 모니터링 앱 [스마트홈 연동 모니터링 앱 view](./day72/Day09Wpf/WpfSmartHomeApp/Views/MainWindow.xaml)



#### MQTT 시뮬레이션 프로젝트 (계속) [mqtt 코드](./day72/Pythons/MqttPub.py)
1. MQTTPub.py소스코드에 임의의 더미 Iot 센서값 정의 + json형태로 변환해서 publish
    - <img src='./day72/json형태로 mqtt실행.png' width=500>

##### 모니터링 시뮬레이션 프로젝트 시작
1. 프로젝트 생성 및 모듈 설치(mahapps, communitytoolkit, newtonsoft.json, mqttnet )
2. 폴더 구조
```
WpfMqttSubApp/
├── Views/
│   └── MainView.xaml
├── ViewModels/
│   └──  MainViewModel.cs
├── Models/
│   └── SensorInfo.cs
└──  Helpers/
    └── RichTextBoxHelper.cs
```
3. App.xaml(리소스), App.xaml.cs(뷰연결), MainView.xaml(마하) , MainView.xaml.cs(마하) 
4. MainView.xaml ui 디자인
5. MainView.xaml, MainViewModel.cs 바인딩 
6. telnet 명령어로 서버서비스가 동작중인지 확인
    - window 기능켜기/끄기 - telnet client 체크 후 확인
    - cmd에 telnet 입력
    ```shell
    C:\Users\Admin>telnet ipv4주소 포트번호
    ```
7. RichTextBox를 MVVM에서 데이터를 바인딩하려면 RichTextBoxHelper클래스 생성 및 BindableDocument속성을 추가적으로 만들어야 함.
    ```csharp
    public static class RichTextBoxHelper
    {
        // 사용자가 만든 바인딩할 문자열 프로퍼티  BindableDocument
        public static readonly DependencyProperty BindableDocumentProperty =
            DependencyProperty.RegisterAttached(
                "BindableDocument",
                typeof(string),
                typeof(RichTextBoxHelper),
                new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnBindableDocumentChanged));

        // 속성 BindableDocument의 게터함수
        public static string GetBindableDocument(DependencyObject obj)
        {
            return (string)obj.GetValue(BindableDocumentProperty);
        }

        // 속성 BindableDocument의 세터함수
        public static void SetBindableDocument(DependencyObject obj, string value)
        {
            obj.SetValue(BindableDocumentProperty, value);
        }

        // 속성값 변경되었을 때 이벤트처리
        private static void OnBindableDocumentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is RichTextBox richTextBox)
            {
                // 기존 문서 클리어
                richTextBox.Document.Blocks.Clear();
                // 새 문자열을 포함하는 Paragraph 추가
                richTextBox.Document.Blocks.Add(new Paragraph(new Run(e.NewValue as string ?? string.Empty)));
            }
        }
    }
    ```
8. RichTextBox 바인딩
    ```xml
    <mah:MetroWindow
        xmlns:helper ="clr-namespace:WpfMqttSubApp.Helpers"
        xmlns:vm ="clr-namespace:WpfMqttSubApp.ViewModels">
    <RichTextBox x:Name ="LogBox" Grid.Row="1" Margin="5" VerticalScrollBarVisibility="Visible" 
             IsReadOnly="True"
             helper:RichTextBoxHelper.BindableDocument="{Binding LogText, UpdateSourceTrigger=PropertyChanged}">
    ```
    ```csharp
    private string _logText;
    public string LogText
    {
        get => _logText;
        set => SetProperty(ref _logText, value);    
    }
    ```
9. RichTextBox에 바인딩할 값 LogText(1) - mqtt
    - MqttPub.py 실행 - 임의의 센서 데이터를 JSON 형식으로 smarthome/110/topic이라는 토픽에 주기적으로 Publish ,  MQTT 브로커에 전송합니다.
    - mqtt explorer 실행  - 클라이언트들이 주고받는 메시지를 중개하는 역할
    - MainView.xaml 실행 - MQTT 클라이언트를 생성하고 smarthome/110/topic 토픽에 구독하여 메시지를 수신 , 이썬에서 전송하는 메시지를 수신하고, 수신된 메시지를 LogText로 출력
    ```xml
    <!--mqtt 연결 버튼-->
    <Button   Command="{Binding ConnectMqttCommand}">
        <TextBlock Text="CONNECT"  Margin="5,0"></TextBlock>
    </Button>
    ```
    ```csharp
    [RelayCommand]
    public async Task ConnectMqtt ()
    {
        if (string.IsNullOrEmpty(BrokerHost))
        {
            await this.dialogCoordinator.ShowMessageAsync(this, "브로커연결", "브로커연결합니다.");
            return;
        }

        //mqtt 브로커에 접속해서 데이터를 가져오기
        ConnectMqttBroker();
    }

    private async Task ConnectMqttBroker()
    {   //mqtt 클라이언트 생성
        var mqttFactory = new MqttClientFactory();
        _mqttClient = mqttFactory.CreateMqttClient();

        //matt 클라이언트 접속 설정
        var mqttClientOptions = new MqttClientOptionsBuilder()
            .WithTcpServer(BrokerHost)
            .WithCleanSession(true)
            .Build();

        //matt 접속 후 이벤트 처리
        _mqttClient.ConnectedAsync += async e =>
        {
            LogText += "MQTT Broker 연결성공\n";

            //연결이후 구독(subscribe)
            // 파이썬에서 MQTT 메시지를 publish할 때 사용하는 topic이 "smarthome/110/topic"이라면, 
            //그 데이터를 받기 위해서는 subscribe할 때 동일한 topic인 "smarthome/110/topic"을 사용해야 합니다.
            await _mqttClient.SubscribeAsync("smarthome/110/topic");
        };

        //MQTT 클라이언트에서 메시지 수신 시 이벤트 처리
        _mqttClient.ApplicationMessageReceivedAsync += e =>
        {
            var topic = e.ApplicationMessage.Topic;
            var payload = e.ApplicationMessage.ConvertPayloadToString(); //byte데이터를 utf-8문자열로 변환
            LogText += $"{payload}\n";          //payload는 MqttPub.py에서 만든 임의의 센서값  json데이터이다.

            return Task.CompletedTask;
        };

        await _mqttClient.ConnectAsync(mqttClientOptions);
    }

    ```
10. RichTextBox에 바인딩할 값 LogText(2) - db
    1. mysql에서 smarthome 스키마 생성 + 테이블생성
    ```sql
    CREATE TABLE `smarthome`.`fakedatas` (
    `sensing_dt` DATETIME NOT NULL,
    `pub_id` VARCHAR(10) NOT NULL,
    `count` DECIMAL NOT NULL,
    `temp` DECIMAL(5,1) NOT NULL,
    `humid` DECIMAL(5,1) NOT NULL,
    `light` CHAR(1) NOT NULL,
    `human` CHAR(1) NOT NULL,
    PRIMARY KEY (`sensing_dt`, `pub_id`));
    ```
    2. db연결, 해제 
  
    ```csharp
    private string _connString = string.Empty;
    private MySqlConnection connection;


    [RelayCommand]
    public async Task ConnectDB()
    {
        if (string.IsNullOrEmpty(DBHost))
        {
            await this.dialogCoordinator.ShowMessageAsync(this, "db연결합니다.", "db연결실패");
            return;
        }
        _connString = $"Server={DBHost};Database=smarthome;Uid=root;Pwd=12345;Charset=utf8";
        await ConnectDatabaseServer();
    
    }

    private async Task ConnectDatabaseServer()
    {
        try
        {
            connection = new MySqlConnection(_connString);
            connection.Open();
            LogText += $"{DBHost} DB서버 접속 성공 ! {connection.State}\n";
        }
        catch (Exception ex)
        {
            LogText += $"{DBHost} DB서버 접속 실패 :{ex.Message}\n";
        }
    }

    public void Dispose()
    {   //리소스 해제 
        connection?.Dispose();
    }
    ```
    3. db서버에서 접속자 정보 확인
    ```sql
    select * from information_schema.processlist limit 10;
    ```
    - <img src ='./day72/sql로 접속확인.png'>
    4. 구독한 데이터를 받아올 저장공간인 SensorInfo 클래스 작성 [SensorInfo.cs](./day72/Day09Wpf/WpfMqttSubApp/Models/SensorInfo.cs)
    5. mqtt브로커주소 버튼 클릭 시, 구독한 데이터를 db에 저장
        - db주소 버튼 클릭해서 connection, _connstring 초기화
        - mqtt브로커주소 버튼 클릭해서 데이터를 db에 저장 
    ```csharp
    private async Task ConnectMqttBroker()
    {
        _mqttClient.ApplicationMessageReceivedAsync += e =>
        {
            var topic = e.ApplicationMessage.Topic;
            var payload = e.ApplicationMessage.ConvertPayloadToString(); //byte데이터를 utf-8문자열로 변환
        
            //json으로 변경하여 db에저장하기 위한 과정
            var data = JsonConvert.DeserializeObject<SensorInfo>(payload);
            // Debug.WriteLine($"{data.COUNT} /{data.SENSING_DT}/{data.HUMID}/ {data.LIGHT}");
            SaveSensingData(data);
        };
    }


    private async Task SaveSensingData(SensorInfo data)
    {
        string query = "INSERT INTO fake_datas VALUES (@SENSING_DT, @PUB_ID,@COUNT,@TEMP ,@HUMID,@RAIN,@PERSON,@LIGHT)";
        //Debug.WriteLine(connection.State);
        //Debug.WriteLine(System.Data.ConnectionState.Open);
        if (connection.State == System.Data.ConnectionState.Open)
            {
                using var cmd = new MySqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@SENSING_DT", data.SENSING_DT);
                cmd.Parameters.AddWithValue("@PUB_ID", data.PUB_ID);
                cmd.Parameters.AddWithValue("@COUNT", data.COUNT);
                cmd.Parameters.AddWithValue("@TEMP", data.TEMP);
                cmd.Parameters.AddWithValue("@HUMID", data.HUMID);
                cmd.Parameters.AddWithValue("@RAIN", data.RAIN);
                cmd.Parameters.AddWithValue("@PERSON", data.PERSON);
                cmd.Parameters.AddWithValue("@LIGHT", data.LIGHT);

                await cmd.ExecuteNonQueryAsync();
            }
    }
    ```

- MqttPub.py, mqtt.explorer, MainView.xaml 실행결과 : db버튼 누르면 db연결성공메시지, mqtt버튼 누르면 데이터 구독해옴과 동시에 mysql데이터베이스에 저장됨


https://github.com/user-attachments/assets/bbe29cda-5f82-4cec-89f7-cabecff99a56


## 73일차(5/21)
### 스마트홈 연동 모니터링 앱 

#### 스마트홈 기기 실제 Iot센서데이터 모니터링앱 [iot센서데이터 구독해오기](./day73/Day10Wpf/WpfMqttSubApp/ViewModels/MainViewModel.cs)
1. 72일차 작성한 MQTT Subscribe앱에 Iot센서데이터 넣음 ->강사ip로 BrokerHost변경 + topic을 "pknu/sh01/data"로 변경 + MainView.xaml파일만 실행
    - SmartHome MQTT Json Key값
        - L : Light (전등)
        - R : Rain  
        - T : Temp
        - H : Humid
        - F : Fan (선풍기, 에어컨)
        - V : Vulernability (침입 감지)
        - RL : Real Light  (실제 전등 켜짐 여부)
        - CB : ChaimBell (현관벨)
    - <img src ='./day73/구독데이터 확인.png'>
2. db 테이블 생성 + 클래스 생성
    - 클래스 생성 시, json데이터의 키값 그대로 작성
    ```csharp
    public class SensingInfo
    {
        public int L{  get; set; } 
        public int R { get; set; }   
        public float T {  get; set; }  
        public float H {  get; set; } 
        public string F { get; set; }   
        public string V{ get; set; }
        public string RL { get; set; }    
        public string CB { get; set; } 

    }
    ```
3. db연결, 해제 ,구독한 데이터를 db에 저장
    ```csharp
    private async Task ConnectMqttBroker()
    {
        _mqttClient.ApplicationMessageReceivedAsync += e =>
        {
            var topic = e.ApplicationMessage.Topic;
            var payload = e.ApplicationMessage.ConvertPayloadToString(); //byte데이터를 utf-8문자열로 변환
        
            //json으로 변경하여 db에저장하기 위한 과정
            var data = JsonConvert.DeserializeObject<SensingInfo>(payload);
            SaveSensingData(data);
        };
    }


    private async Task SaveSensingData(SensingInfo data)
    {
        sstring query = " SET time_zone = 'Asia/Seoul';INSERT INTO sensing_datas(Light,Rain,Temp,Humid,Fan,Vulernability,Real_Light,ChaimBell,Sensing_date) VALUES (@Light, @Rain,@Temp ,@Humid,@Fan,@Vulernability,@Real_Light,@ChaimBell,now())";
     
        try
        {
            if (connection.State == System.Data.ConnectionState.Open)
            {
                using var cmd = new MySqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@Light", data.L);
                cmd.Parameters.AddWithValue("@Rain", data.R);
                cmd.Parameters.AddWithValue("@Temp", data.T);
                cmd.Parameters.AddWithValue("@Humid", data.H);
                cmd.Parameters.AddWithValue("@Fan", data.F);
                cmd.Parameters.AddWithValue("@Vulernability", data.V);
                cmd.Parameters.AddWithValue("@Real_Light", data.RL);
                cmd.Parameters.AddWithValue("@ChaimBell", data.CB);
        
                await cmd.ExecuteNonQueryAsync();
            }
        }
        catch (MySqlException ex)
        {
            Debug.WriteLine($"MySQL 오류: {ex.Message}");
        }
    }
    ```
    - <img src='./day73/db에 구독 데이터 넣기.png'>

#### 스마트홈 WPF 실시간 시각화 [스마트홈 모니터링 시각화](./day73/Day10Wpf/WpfSmartHomeApp/ViewModels/MainViewModel.cs)
- **readonly는 생성자에서만 값 할당할 경우, 적음**
0. WpfSmartHomeApp프로젝트 UI는 이전꺼 그대로[UI](./day71/Day08Wpf/WpfSmartHomeApp/Views/MainWindow.xaml)
1. WpfSmartHomeApp프로젝트에 패키지 설치(json, mqttnet , mahapps, communitytoolkit)
2. 데이터 담을 클래스 작성 
    - 클래스 복사해올 때, namespace를 현재 프로젝트명으로 
    ```csharp
    namespace WpfSmartHomeApp.Models
    {
        public class SensingInfo
        {}
    }
    ```
3. MainViewModel.cs에서 속성 선언
    ```csharp
    // 실시간 데이터 값 구독해오기 위한 준비
    private MySqlConnection connection;

    //TOPIC 
    private  string TOPIC;

    private IMqttClient mqttClient;

    private string BROKERHOST;
    ```
4. MainViewModel.cs에서 iot센서 데이터 구독 함수 선언
    ```csharp
     public partial class MainViewModel : ObservableObject , IDisposable
     {
        [RelayCommand]
        public async Task OnLoaded()
        {
            BROKERHOST = "210.119.12.52";
            connection = new MySqlConnection();
            TOPIC = "pknu/sh01/data";

             //mqtt 클라이언트 생성
            var mqttFactory = new MqttClientFactory();
            mqttClient = mqttFactory.CreateMqttClient();

            //matt 클라이언트 접속 설정
            var mqttClientOptions = new MqttClientOptionsBuilder()
                .WithTcpServer(BROKERHOST)
                .WithCleanSession(true)
                .Build();

            //mqtt 접속 후 이벤트 처리 메서드 선언 
            mqttClient.ConnectedAsync += MqttClient_ConnectedAsync;

            //mqtt 구독 메시지 확인 메서드 선언
            mqttClient.ApplicationMessageReceivedAsync += MqttClient_ApplicationMessageReceivedAsync;

            await mqttClient.ConnectAsync(mqttClientOptions);

        }
        public void Dispose()
        {
            connection?.Close();
        }
     }
    ```
5. MainViewModel.cs에서 iot센서 데이터 구독 함수 선언- 세부
    ```csharp
    //MqttClient_ConnectedAsync
    private async Task MqttClient_ConnectedAsync(MqttClientConnectedEventArgs arg)
    {
        Common.LOGGER.Info("MQTT Borker 접속 성공!!");
        //연결이후 구독(subscribe)
        await mqttClient.SubscribeAsync(TOPIC);
        
    }
    ```
     ```csharp
    //MqttClient_ApplicationMessageReceivedAsync
    private  Task MqttClient_ApplicationMessageReceivedAsync(MqttApplicationMessageReceivedEventArgs arg)
    {

        var topic = arg.ApplicationMessage.Topic;
        var payload = arg.ApplicationMessage.ConvertPayloadToString(); //byte데이터를 utf-8문자열로 변환

        // json으로 변경하여 db에저장하기 위한 과정
        var data = JsonConvert.DeserializeObject<SensingInfo>(payload);
        //Common.LOGGER.Info($"|Light:{data.L}|Rain:{data.R}|Temp:{data.T}|Humid:{data.H}|Fan:{data.F}|Detect:{data.V}|{data.RL}|{data.CB}|");

        HomeTemp = data.T;
        HomeHumid = data.H;

        IsDetectOn = data.V == "ON" ? true : false;
        DetectResult = IsDetectOn ? "Dectection State!!" : "Normal State";

        IsConditionerOn = data.F == "ON" ? true : false;
        ConditionerResult = IsConditionerOn ? "AirCon On!!" : "AirCon Off";

        IsLightOn = data.RL ==  "ON" ? true : false;
        LightResult = IsLightOn ? "Light On!!" : "Light Off";

        IsRainOn = data.R <= 350 ? true : false;
        RainResult = IsRainOn ? "Rain!!" : "No Rain";

        // 구독 종료 알림
        return Task.CompletedTask;
    }
    ```
- 실행결과


https://github.com/user-attachments/assets/35ca5b77-1594-4a81-855f-fb3ecfb9728c

- 센서 조작 영상
  


https://github.com/user-attachments/assets/a931903b-927d-4903-a947-ea02cb3398fe

# 미니프로젝트
- 주제 : 공공데이터의 문화재 현황 api를 통해 데이터를 읽어와 문화재 찾기 앱 구현
- 기능 
    - MainView.xaml : 앱 실행시 뜨는 첫 화면. 기본값은 오늘의 문화재인데 버튼을 통해 뷰를 바꿀 수 있음.
    - TodayView.xaml :오늘의 문화재를 보여줌(랜덤으로 할당)
    - LocationView.xaml : 사용자가 지역(시,도,군,구)을 입력해서 필터링해서 데이터리스트를 보여줌
    - DetailView.xaml : 사용자가 선택한 문화재에 대한 더 상세한 설명은 국가유산 공식포털 페이지를 띄우도록 함.

## 74일차(5/22)
- 진행상황 
    - 패키지 설치 - MahApps, CommunityTookit, CefSharp , Newtonsoft.Json
    - Models, Views, ViewModels 폴더 및 뷰, 뷰모델, 클래스 파일 생성
    ```csharp
    public partial class MainView : MetroWindow{}
    public partial class LocationView : UserControl{}
    public partial class TodayView : UserControl{}
    ```
    - App.xaml 리소스 및 뷰 변환, App.xaml.cs startUp
    - UI디자인 - MainView.xaml ,TodayView.xaml,LocationView.xaml
    - 버튼 클릭을 통한 화면 전환 - App.xaml에서 뷰 리소스 , MainView.xaml , MainViewModel.cs
    ```xml
    <Application x:Class="HeritageApp.App"
             xmlns:views ="clr-namespace:HeritageApp.Views"
             xmlns:vm ="clr-namespace:HeritageApp.ViewModels">
        <Application.Resources>
            <ResourceDictionary>
                <DataTemplate DataType="{x:Type vm:LocationViewModel}">
                    <views:LocationView />
                </DataTemplate>
                <DataTemplate DataType="{x:Type vm:TodayViewModel}">
                    <views:TodayView />
                </DataTemplate>
            </ResourceDictionary>
        </Application.Resources>
    </Application>
     
    ```
    ```csharp
    <mah:MetroWindow  xmlns:vm ="clr-namespace:HeritageApp.ViewModels">
        <Window.DataContext>
            <vm:MainViewModel />
        </Window.DataContext>
        <Grid>
            <Button Content="오늘의 문화재 알아보기" Command ="{Binding ShowView1Command}"></Button>
            <Button Content="우리동네 문화재 알아보기" Command ="{Binding ShowView2Command}"></Button>
            <ContentControl Grid.Row="1" Grid.Column="0" Grid.ColumnSpan="2" Content="{Binding CurrentViewModel}" />
        </Grid>
    </mah:MetroWindow>
    ```
    ```csharp
        private object currentViewModel;
        public object CurrentViewModel
        {
            get => currentViewModel;
            set => SetProperty(ref currentViewModel, value);    
        }
      

        public MainViewModel()
        {
            ShowView1();
        }

        [RelayCommand]
        private void ShowView1()
        {
            CurrentViewModel = new TodayViewModel();
        }

        [RelayCommand]
        private void ShowView2()
        {
            CurrentViewModel = new LocationViewModel();
        }
    ```

- 실행결과


https://github.com/user-attachments/assets/42d78017-2705-4b4a-8d47-e4ec55f92b02



## 75일차(5/23)
- 국가유산 https://www.heritage.go.kr/heri/cul/culSelectViewList.do?gbn=2&pageNo=1_1_1_1&culPageNo=14&ccbaCndt=&searchCondition=&region=1&s_kdcdArr=00&s_ctcdArr=21&ccbaPcd1Arr=99&stCcbaAsdt=&endCcbaAsdt=&ccbaGcodeArr=00&sortType=&sortOrd=
- api호출 http://apis.data.go.kr/6260000/BusanTblClthrtStusService/getTblClthrtStusInfo?serviceKey=&numOfRows=&pageNo=
- 진행상황
    - DetailView.xaml  - 디자인 (O)
    - item.cs 모델 클래스 (O) 
    - api로 데이터 읽어와서 오늘의 문화재 찾아보기 뷰에 랜덤 할당(O)
    - 오늘의 문화재 찾아보기에 오늘날짜, 달력시간 컨트롤 넣기 (O)
    - dates(지정일) /dataday (데이터업로드일)(O)

## 6/2    
- 진행상황
    - today ui디자인
    - today  이미지 cefsharp
    - today  오늘날짜 기본값, 오늘날짜 기준 몇년전 유산인지 계산기
    - <img src='./today뷰.png' width=500>

## 6/4 
- today 영어버전으로 읽기, 한국어버전으로 읽기 
    - today뷰에서 버튼 command 속성 , today뷰모델에서 command정의 
    - 번역기 돌린 화면의 개발자도구-요소-html코드를 불러와서 필요한부분인 span태그 내용 추출
    - NuGet 패키지 매니저를 통해 설치- Selenium.WebDriver, Selenium.WebDriver.ChromeDriver
    ```cs
    public async Task<string> GetTranslatedTextAsync(string input)
    {
        try
        {
            
            var encodedInput = Uri.EscapeDataString(input);
            var url = $"https://translate.google.co.kr/?sl=auto&tl=en&text={encodedInput}&op=translate";
            var options = new ChromeOptions();
            options.AddArgument("--headless");
            options.AddArgument("--disable-gpu");
            options.AddArgument("--no-sandbox");
            options.AddArgument("--disable-dev-shm-usage");

            using (var driver = new ChromeDriver(options))
            {
                driver.Navigate().GoToUrl(url);

                var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(100));

                // 더 안정적인 jsname 속성으로 요소 찾기
                var translatedElement = wait.Until(d =>
                {
                    var elements = d.FindElements(By.CssSelector("span.ryNqvb[jsname='W297wb']"));
                    return elements.FirstOrDefault(e => e.Displayed);
                });

                return translatedElement?.Text ?? "번역된 텍스트를 찾을 수 없습니다.";
            }
        }
        catch (NoSuchElementException ex)
        {
            Common.LOGGER.Error($"[Selenium] Element not found: {ex.Message}");
            return "번역된 텍스트를 찾을 수 없습니다.";
        }
        catch (Exception ex)
        {
            Common.LOGGER.Fatal($"[Selenium Fatal Error] {ex}");
            return "번역 중 오류가 발생했습니다.";
        }
    }
    ```
    - 한번에 로드될까=>번역기가 길어지니 빠뜨리는게 생김으로 예외계속 발생 =>개별로 하자!!
    - <img src='./today뷰 영어.png' width=500>
## 6/5
- location 콤보박스 값 할당(시도, 구군)
- location 지역 입력값 + api로 데이터 읽어와서 문화재 리스트
- <img src='./location뷰.png' width=500>
## 6/9
- location 지역별 문화재 뷰에 바인딩 - true로 하면 나오는데 false로 DataGridTextColumn을 직접 하니 뜨지 않음  =>해결함
    - 디버깅해서 필드첫번째가 대문자인 것을 확인함.
    - Binding문법 오류였음
    ```cs
    <DataGrid
    ItemsSource="{Binding TodayItems}">
    <DataGrid.Columns>
        <DataGridTextColumn Header="문화재명" FontWeight="Bold" Binding="{Binding CultHeritNm}" />
        <DataGridTextColumn Header="지정번호" FontWeight="Bold" Binding="{Binding Number}" />
        <DataGridTextColumn Header="지정일자" FontWeight="Bold" Binding="{Binding Dates}" />
        <DataGridTextColumn Header="시대" FontWeight="Bold" Binding="{Binding Era}" />
        <DataGridTextColumn Header="종별" FontWeight="Bold" Binding="{Binding Kind}" />
    </DataGrid.Columns>
    </DataGrid>

    ```
    
- detail 더블클릭했을 때, 디테일뷰가 나오고 이 뷰에 국가유산 포털 연결되도록
- <img src='./detail뷰.png' width=500>


# 미니 프로젝트
### 국가문화유산 랜덤 탐색 시스템 개발 보고서
1. 개요 및 목적
    - 사용자가 매일 하나의 국가문화유산을 랜덤하게 탐색하고, 그 유산의 상세정보, 지정일자, 시대, 지역 등의 정보를 확인할 수 있도록 하는 데 목적이 있습니다.
    - 또한, 지역 필터링 기능, 언어 번역 기능, 국가유산 포털 연동 등 다양한 부가 기능을 통해 문화유산 정보의 접근성과 활용성을 높이고자 합니다.
    - 이 시스템은 교육적 목적, 박물관 안내 앱, 관광지 연계 콘텐츠 등에 응용될 수 있는 프로토타입 형태로 구현되었습니다.

2. 주요 기능

|view|기능|설명|
|:--:|:--:|:--:|
|TodayView|오늘의 문화재 랜덤 배정|앱 실행 시, API를 통해 랜덤으로 문화재 1건을 보여줌|
|TodayView|오늘날짜 기반 계산기|문화재 지정일과 오늘 날짜를 기준으로 얼마나 오래되었는지 자동 계산|
|TodayView|다국어 번역 기능|Google Translate 웹 번역기 + Selenium을 활용해 한국어 → 영어로 자동 번역|
|LocationView|지역별 검색 기능|시도, 구군 등 지역 정보를 입력하면 해당 지역의 문화재 리스트를 바인딩|
|DetailView|상세보기 및 외부 링크 연동|목록에서 문화재를 더블클릭하면 DetailView로 이동하며, 국가유산 포털 웹페이지를 연동 표시|

3. 시스템 구조

```yaml
MainView
├── TodayView
└── LocationView
    └── DetailView
```

4. 실행결과



https://github.com/user-attachments/assets/8d7ef4c1-2955-4b38-8b23-8be4f12dcf88



5. 프로젝트 수행 중 새롭게 학습한 내용

    1. WPF + MVVM 구조에서 동적으로 View를 전환 - Button 클릭 시 CurrentViewModel을 바꾸고, ContentControl이 이를 감지해서 해당 ViewModel에 맞는 View를 보여주는 구조
        - ContentControl + DataTemplate
        - MainView.xaml
            ```xml
            <StackPanel Grid.Row="0" Grid.Column="1" Orientation="Horizontal" Margin="5" HorizontalAlignment="Right">
                <Button Content="오늘의 문화재 알아보기" Style="{StaticResource MahApps.Styles.Button.MetroSquare.Accent}"
                        Margin="5,0,5,0" Height="30" Command ="{Binding ShowView1Command}"></Button>
                <Button Content="우리동네 문화재 알아보기" Style="{StaticResource MahApps.Styles.Button.MetroSquare.Accent}" 
                        Margin="0,0,5,0" Height="30" Command ="{Binding ShowView2Command}"></Button>
            </StackPanel>
            <ContentControl Grid.Row="1" Grid.Column="0" Grid.ColumnSpan="2" Content="{Binding CurrentViewModel}" />
            ```
        - MainView.cs
            ```cs

            private object currentViewModel;
            public object CurrentViewModel
            {
                get => currentViewModel;
                set => SetProperty(ref currentViewModel, value);    
            }
                

            public MainViewModel()
            {
                ShowView1();
            }

            [RelayCommand]
            private void ShowView1()
            {
                CurrentViewModel = new TodayViewModel();
            }

            [RelayCommand]
            private void ShowView2()
            {
                CurrentViewModel = new LocationViewModel();
            }
            ```
        - App.xaml
            ```xml
            <DataTemplate DataType="{x:Type vm:LocationViewModel}">
                <views:LocationView />
            </DataTemplate>
            <DataTemplate DataType="{x:Type vm:TodayViewModel}">
                <views:TodayView />
            </DataTemplate>
            ```
    2. WPF에서 <DataGridTextColumn>의 Binding 속성
        - 기본 개념: Binding="{Binding PropertyName}"
        - Binding="{Binding CultHeritNm}" 은 현재 행(Row)의 데이터 객체의 CultHeritNm 속성에 바인딩한다는 뜻입니다.
        - 바인딩 경로에서 대소문자 구분이 중요합니다.
        - 정확한 속성 이름 사용해야 합니다.
    3. Google 번역 사이트에 자동으로 접속하여, 지정한 텍스트를 영어로 번역하고, 번역 결과(span 요소)에 있는 텍스트를 추출
        - Selenium - 웹 데이터 수집 (스크래핑)
        - Selenium WebDriver: 작동 원리
            - WebDriver 객체 생성
            - URL 열기
            - HTML 요소 찾기 및 제어 - FindElement()로 버튼, 텍스트박스 등 찾기
            - 자동화 동작 수행
            - 종료


6. 출처
    - 공공테이터포털 https://www.data.go.kr/
    - 구글번역  https://translate.google.co.kr/?sl=auto&tl=en&op=translate
    - 국가유산포털 https://www.heritage.go.kr/main/?v=1749440296009