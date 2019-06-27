using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
using LiveCharts;
using LiveCharts.Wpf;
using MySql.Data.MySqlClient;
using System.Data;
using System.Data.Odbc;

namespace WpfInterfejsGraficzny
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static DBConnectionODBC db = new DBConnectionODBC();
        ObservableCollection<Month> monthList;
        private string searchField;
        //public Func<ChartPoint, string> PointLabel { get; set; }
        public ChartPointsClass ChartPoints { get; set; }
        double newWindowHeight=450, newWindowWidth=450, prevWindowHeight=450, prevWindowWidth=450;
        


        public MainWindow()
        {
            if (!CheckConnection())
            {
                MessageBox.Show("BŁĄD ŁĄCZENIA Z BAZĄ DANYCH 'projektwpf'");
            }
            //Zresetowanie bazy danych
            //db.RecreateDatabaseWithValues();
            monthList = new ObservableCollection<Month>();
            RefreshMonthList();           
            
            MonthView.Filter = new Predicate<object>(o => Filter(o as Month));

            ChartPoints = new ChartPointsClass();

            this.DataContext = this;

            RefreshPieChart();
            InitializeComponent();
            this.SizeChanged += OnWindowSizeChanged;
        }

        public bool CheckConnection()
        {
            if( !db.CheckConnection() )
            {
                Trace.WriteLine("BŁĄD ŁĄCZENIA Z BAZĄ DANYCH 'projektwpf'");
                return false;
            }
            else
            {
                Trace.WriteLine("POŁĄCZONO Z BAZĄ DANYCH 'projektwpf'");
                return true;
            }

        }

        public void RefreshMonthList()
        {
            monthList.Clear();
            List<Month> tmpMonthList = null;
            tmpMonthList = db.GetMonths();
            foreach (var month in tmpMonthList)
            {
                monthList.Add(month);
            }
        }

        public string SearchField
        {
            get
            {
                return searchField;
            }
            set
            {
                searchField = value;
                MonthView.Refresh(); //Odświeżenie listy miesięcy po wpisaniu wyszukiwanej frazy
            }

        }

        private void Chart_OnDataClick(object sender, ChartPoint chartpoint)
        {
            var chart = (LiveCharts.Wpf.PieChart)chartpoint.ChartView;


            //clear selected slice.
            foreach (PieSeries series in chart.Series)
                series.PushOut = 0;

            var selectedSeries = (PieSeries)chartpoint.SeriesView;
            selectedSeries.PushOut = 8;
        }

        public ICollectionView MonthView
        {
            get { return CollectionViewSource.GetDefaultView(monthList); }
        }

        private bool Filter(Month month)
        {
            int fromRok=0, toRok=0, rok=0;
            double fromPrzychod=0, toPrzychod=0, przychod = 0;
            double fromWydatek = 0, toWydatek = 0, wydatek = 0;
            int fromToRokType = 0, fromToPrzychodType = 0, fromToWydatekType = 0; //0-dokladny rok 1-podana tylko poczatkowa 2-podana tylko koncowa 3-podane obie daty
            bool monthNameMatch = false;

            if (SearchField != null)
            {
                string[] searchedStrings = SearchField.Split(' ');//Używane w wyszukiwaniu po nazwach miesięcy

                //Logika wyszukiwania po nazwach
                for (int j = 0; j < searchedStrings.Count(); j++)
                {
                    if (month.NazwaMiesiaca.IndexOf(searchedStrings[j], StringComparison.OrdinalIgnoreCase) != -1
                        && searchedStrings[j].Length > 0)
                    {
                        monthNameMatch = true;
                        break;
                    }
                }

                //Logika wyszukiwania po roku
                //Remove(0, 2) ponieważ wyrażenie regularne bierze łącznie ze znakami R>
                if ((Regex.Match(SearchField, @"[R,r]>\d+").Value.Length > 0) && int.TryParse(Regex.Match(SearchField, @"[R,r]>\d+").Value.Remove(0, 2), out fromRok))
                {
                    fromToRokType++;
                }
                if (Regex.Match(SearchField, @"[R,r]<\d+").Value.Length > 0 && int.TryParse(Regex.Match(SearchField, @"[R,r]<\d+").Value.Remove(0, 2), out toRok))
                {
                    fromToRokType += 2;
                }

                //Logika wyszukiwania po miesięcznym przychodzie
                if ((Regex.Match(SearchField, @"[P,p]>(\d+\,?\d{1,2}|\d+)").Value.Length > 0) && double.TryParse(Regex.Match(SearchField, @"[P,p]>(\d+\,?\d{1,2}|\d+)").Value.Remove(0, 2), out fromPrzychod))
                {
                    fromToPrzychodType++;
                }
                if (Regex.Match(SearchField, @"[P,p]<(\d+\,?\d{1,2}|\d+)").Value.Length > 0 && double.TryParse(Regex.Match(SearchField, @"[P,p]<(\d+\,?\d{1,2}|\d+)").Value.Remove(0, 2), out toPrzychod))
                {
                    fromToPrzychodType += 2;
                }

                //Logika wyszukiwania po miesięcznym wydatku
                if ((Regex.Match(SearchField, @"[W,w]>(\d+\,?\d{1,2}|\d+)").Value.Length > 0) && double.TryParse(Regex.Match(SearchField, @"[W,w]>(\d+\,?\d{1,2}|\d+)").Value.Remove(0, 2), out fromWydatek))
                {
                    fromToWydatekType++;
                }
                if (Regex.Match(SearchField, @"[W,w]<(\d+\,?\d{1,2}|\d+)").Value.Length > 0 && double.TryParse(Regex.Match(SearchField, @"[W,w]<(\d+\,?\d{1,2}|\d+)").Value.Remove(0, 2), out toWydatek))
                {
                    fromToWydatekType += 2;
                }
            }
            return SearchField == null
                || SearchField == ""
                || monthNameMatch == true
                || (Regex.Match(SearchField, @"[R,r]\d+").Value.Length > 0
                    && int.TryParse(Regex.Match(SearchField, @"[R,r]\d+").Value.Remove(0, 1), out rok)
                    && month.Przychod == przychod)
                || (fromToRokType == 1 && fromRok <= month.Rok)
                || (fromToRokType == 2 && toRok >= month.Rok)
                || (fromToRokType == 3 && toRok >= month.Rok && fromRok <= month.Rok)
                || (Regex.Match(SearchField, @"[P,p](\d+\,?\d{1,2}|\d+)").Value.Length > 0
                    && double.TryParse(Regex.Match(SearchField, @"[P,p](\d+\,?\d{1,2}|\d+)").Value.Remove(0, 1), out przychod)
                    && month.Przychod == przychod)
                || (fromToPrzychodType == 1 && fromPrzychod <= month.Przychod)
                || (fromToPrzychodType == 2 && toPrzychod >= month.Przychod)
                || (fromToPrzychodType == 3 && toPrzychod >= month.Przychod && fromPrzychod <= month.Przychod)
                || (Regex.Match(SearchField, @"[W,w](\d+\,?\d{1,2}|\d+)").Value.Length > 0
                    && double.TryParse(Regex.Match(SearchField, @"[W,w](\d+\,?\d{1,2}|\d+)").Value.Remove(0, 1), out wydatek)
                    && month.Wydatek == wydatek)
                || (fromToWydatekType == 1 && fromWydatek <= month.Wydatek)
                || (fromToWydatekType == 2 && toWydatek >= month.Wydatek)
                || (fromToWydatekType == 3 && toWydatek >= month.Wydatek && fromWydatek <= month.Wydatek);
        }

        private void monthView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (monthListBox.SelectedIndex < 0)//Zabezpieczenie przy usuwaniu, kiedy ustawiany jest indeks mniejszy od zera
                monthListBox.SelectedIndex = 0;

            if (monthList.Count != 0)//Kiedy jest jakis miesiac na liscie (także po wyszukiwaniu)
            {
                ;// selectedMonth = (Month)monthListBox.SelectedItem;
                //wykres_tb.GetBindingExpression(TextBox.TextProperty).UpdateTarget();
            }

        }

        protected void OnWindowSizeChanged(object sender, SizeChangedEventArgs e)
        {
            newWindowHeight = e.NewSize.Height;
            newWindowWidth = e.NewSize.Width;
            prevWindowHeight = e.PreviousSize.Height;
            prevWindowWidth = e.PreviousSize.Width;
            RefreshPieChart();
        }

        public void RefreshPieChart()
        {
            ChartPoints.RemoveAllPoints();
            monthList.OrderBy(s => s.Rok).ThenBy(s => s.NumerMiesiaca);
            
            //Ilość wyświetlanych miesięcy bazując na rozmiarze okna
            int displayMonth = 3;
            if(newWindowHeight>440 && newWindowWidth > 440)
            {
                displayMonth = 6;
            }
            if (newWindowHeight > 800 && newWindowWidth > 800)
            {
                displayMonth = 12;
            }

            Month tmp;
            for (int i=0; i<displayMonth && i<monthList.Count; i++)
            {
                tmp = monthList.ElementAt(i);
                ChartPoints.AddPoint(tmp.Wydatek, tmp.Rok + " " + tmp.NazwaMiesiaca);
            }
        }

        private void BT_AddAccountingMonth(object sender, RoutedEventArgs e)
        {
            //Otwieranie okna dodawania miesiąca, blokowanie innych działań i odświeżanie wartości okna po dodaniu miesiąca
            AddAccountingMonthWindow window = new AddAccountingMonthWindow();
            window.Title = "Dodawanie miesiąca rozliczeniowego";
            if( (bool)window.ShowDialog() == true)
            {
                RefreshMonthList();
                RefreshPieChart();
            }
        }

        private void deleteMonth(object sender, RoutedEventArgs e)
        {
            var curItem = ((ListBoxItem)monthListBox.ContainerFromElement((Button)sender)).Content;
            Month clickedMonth = (Month)curItem;

            //Zapytanie czy na pewno usunąć
            if (GetConfirmation("Czy na prawno chcesz usunąć miesiąc " + clickedMonth.NazwaMiesiaca + " " + clickedMonth.Rok, "Potwierdzenie usunięcia"))
            {
                //Usuwanie miesiąca
                db.removeMonthFromDB(clickedMonth.NazwaMiesiaca, clickedMonth.Rok);
                //Odświeżanie listy
                RefreshMonthList();
                //Odświeżanie wykresu kołowego
                RefreshPieChart();
            }
        }

        public bool GetConfirmation(string Message, string Caption)
        {
            return MessageBox.Show(Message,
                                   Caption,
                                   System.Windows.MessageBoxButton.OKCancel,
                                   System.Windows.MessageBoxImage.Question,
                                   System.Windows.MessageBoxResult.Cancel) == System.Windows.MessageBoxResult.OK;
        }

        private void BT_Close(object sender, RoutedEventArgs e)
        {
            Window.GetWindow(this).Close();
        }

        private void editMonth(object sender, RoutedEventArgs e)
        {
            var curItem = ((ListBoxItem)monthListBox.ContainerFromElement((Button)sender)).Content;
            Month clickedMonth = (Month)curItem;
            EditMonthWindow window = new EditMonthWindow(clickedMonth);

            window.Title = clickedMonth.NazwaMiesiaca + " " + clickedMonth.Rok;
            if ((bool)window.ShowDialog() == true)
            {
                
            }

            //Update wartości po zmianie wydatku w danym miesiącu poprzez dodanie produktu
            db.UpdateMonthsExpenses();
            RefreshMonthList();
            RefreshPieChart();
        }

        private void BT_ShowMonthSummary(object sender, RoutedEventArgs e)
        {
            var curItem = ((ListBoxItem)monthListBox.ContainerFromElement((Button)sender)).Content;
            Month clickedMonth = (Month)curItem;

            //Tworzenie DataTable przesyłanego do zestawienia
            DataTable dt = new DataTable("Month summary DataTable");
            dt.Columns.Add("Nazwa", typeof(string));
            //dt.Columns.Add("Opis", typeof(string));
            dt.Columns.Add("ilosc", typeof(int));
            dt.Columns.Add("Cena", typeof(double));
            dt.Columns.Add("Suma", typeof(double));
            dt.Columns.Add("Kategorie", typeof(string));
            List<Product> productsToDataTable = db.GetProductsForMonth(clickedMonth.NazwaMiesiaca, clickedMonth.Rok);
            foreach (var product in productsToDataTable)
            {
                string categories = "";
                foreach(var category in product.Kategorie)
                {
                    categories += category + ", ";
                } 
                dt.Rows.Add(product.NazwaProduktu, product.Ilosc, product.Cena, product.Ilosc * product.Cena, categories);
            }



            //Przesyłanie do okna od wyświetlania zestawienia obiektu DataTable
            MonthSummaryWindow window = new MonthSummaryWindow(dt);
            window.Title = "Podsumowanie miesiąca " + clickedMonth.NazwaMiesiaca + " " + clickedMonth.Rok;
            window.Show();
        }

        private void BT_ShowFixedExpenses(object sender, RoutedEventArgs e)
        {
            FixedExpensesWindow window = new FixedExpensesWindow();

            window.Title = "Wydatki stałe";
            if ((bool)window.ShowDialog() == true)
            {
            }
        }

        //Commandsy
        private void ShowMonthsSummaryExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            //Tworzenie DataTable przesyłanego do zestawienia
            DataTable dt = new DataTable("Months summary DataTable");
            dt.Columns.Add("Nazwa miesiąca", typeof(string));
            dt.Columns.Add("Rok", typeof(int));
            dt.Columns.Add("Wydatek", typeof(double));
            dt.Columns.Add("Przychód", typeof(double));
            List<Month> monthsToDataTable = db.GetMonths();
            foreach (var month in monthsToDataTable)
            {
                dt.Rows.Add(month.NazwaMiesiaca, month.Rok, month.Wydatek, month.Przychod);
            }



            //Przesyłanie do okna od wyświetlania zestawienia obiektu DataTable
            MonthsSummaryWindow window = new MonthsSummaryWindow(dt);
            window.Title = "Podsumowanie ostatnich miesięcy";
            window.Show();
        }
        private void ShowMonthsSummaryCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            //if (db == null || db.GetMonths() == null || db.GetMonths().Count <= 0)
            if(monthList == null || monthList.Count==0)
            {
                e.CanExecute = false;
            }
            else
            {
                e.CanExecute = true;
            }
        }

        private void PrintMonthsExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            PDFCreator pdf = new PDFCreator();

            pdf.PreCreateDocument("Podsumowanie miesięcy");
            string[] columnNames = new string[] { "ID", "Nazwa miesiąca", "Rok", "Wydatek", "Przychód" };
            string[][] tableData = new string[monthList.Count][];

            Month month;
            for (int i = 0; i < monthList.Count; i++)
            {
                month = monthList.ElementAt(i);
                tableData[i] = new string[] { i.ToString(), month.NazwaMiesiaca, month.Rok.ToString(), month.Wydatek.ToString(), month.Przychod.ToString()};
            }

            MigraDoc.DocumentObjectModel.Color[] colors = {
                new MigraDoc.DocumentObjectModel.Color((byte)119, (byte)221, (byte)119),
                new MigraDoc.DocumentObjectModel.Color((byte)0, (byte)255, (byte)255)
            };
            pdf.NewTable("Podsumowanie wszystkich miesięcy", columnNames, tableData, 2, colors);
            pdf.PostCreateDocument("MonthsSummary.pdf", 1);

        }
        private void PrintMonthsCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (monthList != null && monthList.Count > 0)
                e.CanExecute = true;
            else
                e.CanExecute = false;
        }
    }
}
