using LiveCharts;
using LiveCharts.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
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
using System.Windows.Shapes;

namespace WpfInterfejsGraficzny
{
    /// <summary>
    /// Interaction logic for EditMonthWindow.xaml
    /// </summary>
    public partial class EditMonthWindow : Window
    {
        ObservableCollection<Product> productList;
        private string searchField;
        public Month editingMonth { set; get; }
        public Product selectedProduct { set; get; }
        public ChartPointsClass ChartPoints { get; set; }
        double newWindowHeight = 450, newWindowWidth = 450, prevWindowHeight = 450, prevWindowWidth = 450;
        public string TestString { set; get; }

        public string SearchField
        {
            get
            {
                return searchField;
            }
            set
            {
                searchField = value;
                ProductView.Refresh(); //Odświeżenie listy produktów po wpisaniu wyszukiwanej frazy
            }

        }



        public EditMonthWindow(Month editingMonth)
        {
            this.editingMonth = editingMonth;
            productList = new ObservableCollection<Product>();
            RefreshProductList();

            ProductView.Filter = new Predicate<object>(o => Filter(o as Product));

            this.DataContext = this;

            ChartPoints = new ChartPointsClass();
            //ChartPoints.AddPoint(3, "Test");
            RefreshPieChart();
            InitializeComponent();
            this.SizeChanged += OnWindowSizeChanged;
        }


        private void RefreshProductList()
        {
            productList.Clear();
            List<Product> tmpProductList = MainWindow.db.GetProductsForMonth(editingMonth.NazwaMiesiaca, editingMonth.Rok);
            foreach (var product in tmpProductList)
            {
                productList.Add(product);
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

        public void RefreshPieChart()
        {
            ChartPoints.RemoveAllPoints();
            productList.OrderBy(s => (s.Cena * s.Ilosc));

            //Ilość wyświetlanych produktów bazując na rozmiarze okna
            int displayProducts = 3;
            if (newWindowHeight > 440 && newWindowWidth > 440)
            {
                displayProducts = 6;
            }
            if (newWindowHeight > 800 && newWindowWidth > 800)
            {
                displayProducts = 12;
            }

            Product tmp;
            int i;
            for (i = 0; i < displayProducts-1 && i < productList.Count; i++)
            {
                tmp = productList.ElementAt(i);
                ChartPoints.AddPoint(tmp.Cena*tmp.Ilosc, tmp.NazwaProduktu);
            }

            double sum = 0;
            for(; i<productList.Count; i++)
            {
                tmp = productList.ElementAt(i);
                sum += tmp.Cena * tmp.Ilosc;
            }
            if (sum > 0)
            {
                ChartPoints.AddPoint(sum, "Inne");
            }
        }

        protected void OnWindowSizeChanged(object sender, SizeChangedEventArgs e)
        {
            newWindowHeight = e.NewSize.Height;
            newWindowWidth = e.NewSize.Width;
            prevWindowHeight = e.PreviousSize.Height;
            prevWindowWidth = e.PreviousSize.Width;
            //Odświeżanie wykresu w przypadku zmiany rozmiaru okna
            RefreshPieChart();
        }

        public ICollectionView ProductView
        {
            get { return CollectionViewSource.GetDefaultView(productList); }
        }

        private bool Filter(Product product)
        {
            double fromCena=0, toCena=0, cena=0;
            int fromIlosc = 0, toIlosc = 0, ilosc = 0;
            int fromToCenaType = 0, fromToIloscType = 0; //0-dokladna wartosc 1-podana tylko poczatkowa 2-podana tylko koncowa 3-podany zakres min i max wartosci
            bool productCategoryMatch = false;
            bool productNameMatch = false;

            List<string> kategorie = new List<string>();


            if (SearchField != null) 
            {
                string[] searchedStrings = SearchField.Split(' ');//Używane w wyszukiwaniu po nazwach produktów i ich kategoriach

                //Logika wyszukiwania po nazwach
                //Przechowuje nazwe produktu zamiast "nazwa_produktu" jest "nazwa produktu",
                string[] productNameStringArray; //bo spacja rozdziela, więc należy wprowadzić nazwę produktu z podkreśleniem
                string productNameString;
                for (int j = 0; j < searchedStrings.Count(); j++)
                {
                    productNameStringArray = searchedStrings[j].Split('_');

                    productNameString = "";
                    for(int k=0; k<productNameStringArray.Count(); k++)
                    {
                        productNameString += productNameStringArray[k];
                        if(k+1< productNameStringArray.Count())
                        {
                            productNameString += " ";
                        }
                    }

                    if (product.NazwaProduktu.IndexOf(productNameString, StringComparison.OrdinalIgnoreCase) != -1
                        && searchedStrings[j].Length > 0)
                    {
                        productNameMatch = true;
                        break;
                    }
                }
                

                //Logika pobierania przychodu z pola tekstowego
                //Remove(0, 1) ponieważ wyrażenie regularne bierze łącznie ze znakiem >
                if ((Regex.Match(SearchField, @"[C,c]>(\d+\,?\d{1,2}|\d+)").Value.Length > 0) && double.TryParse(Regex.Match(SearchField, @"[C,c]>(\d+\,?\d{1,2}|\d+)").Value.Remove(0, 2), out fromCena))
                {
                    fromToCenaType += 1;
                }
                if (Regex.Match(SearchField, @"[C,c]<(\d+\,?\d{1,2}|\d+)").Value.Length > 0 && double.TryParse(Regex.Match(SearchField, @"[C,c]<(\d+\,?\d{1,2}|\d+)").Value.Remove(0, 2), out toCena))
                {
                    fromToCenaType += 2;
                }

                //Logika pobierania ilosci produktu z pola tekstowego
                if((Regex.Match(SearchField, @"[I,i]>\d+").Value.Length > 0) && int.TryParse(Regex.Match(SearchField, @"[I,i]>\d+").Value.Remove(0, 2), out fromIlosc))
                {
                    fromToIloscType += 1;
                }
                if ((Regex.Match(SearchField, @"[I,i]<\d+").Value.Length > 0) && int.TryParse(Regex.Match(SearchField, @"[I,i]<\d+").Value.Remove(0, 2), out toIlosc))
                {
                    fromToIloscType += 2;
                }

                //Logika sprawdzania kategorii
                for( int i=0; i<product.Kategorie.Count && productCategoryMatch==false; i++)
                {
                    for (int j=0; j<searchedStrings.Count(); j++)
                    {
                        if(product.Kategorie.ElementAt(i).IndexOf(searchedStrings[j], StringComparison.OrdinalIgnoreCase) != -1
                            && searchedStrings[j].Length>0)
                        {
                            productCategoryMatch = true;
                            //Trace.WriteLine(product.NazwaProduktu + " |" + product.Kategorie.ElementAt(i) + "| |" + searchedCategories[j] + "|");
                            break;
                        }
                    }
                }
            }


            return SearchField == null
                || SearchField == ""
                || (productNameMatch == true)
                || ((Regex.Match(SearchField, @"[C,c](\d+\,?\d{1,2}|\d+)").Value.Length > 0)
                    && double.TryParse(Regex.Match(SearchField, @"[C,c](\d+\,?\d{1,2}|\d+)").Value.Remove(0, 1), out cena)
                    && product.Cena == cena)
                || (fromToCenaType == 1 && fromCena <= product.Cena)
                || (fromToCenaType == 2 && toCena >= product.Cena)
                || (fromToCenaType == 3 && toCena >= product.Cena && fromCena <= product.Cena)
                || ((Regex.Match(SearchField, @"[I,i]\d+").Value.Length > 0)
                    && int.TryParse((Regex.Match(SearchField, @"[I,i]\d+").Value.Remove(0, 1)), out ilosc)
                    && product.Ilosc == ilosc)
                || (fromToIloscType == 1 && fromIlosc <= product.Ilosc)
                || (fromToIloscType == 2 && toIlosc >= product.Ilosc)
                || (fromToIloscType == 3 && toIlosc >= product.Ilosc && fromIlosc <= product.Ilosc)
                || (productCategoryMatch == true);
                
                
        }

        private void productView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (productListBox.SelectedIndex < 0)//Zabezpieczenie przy usuwaniu, kiedy ustawiany jest indeks mniejszy od zera
                productListBox.SelectedIndex = 0;

            if (productList.Count != 0)//Kiedy jest jakis miesiac na liscie (także po wyszukiwaniu)
            {
                selectedProduct = (Product)productListBox.SelectedItem;
                //wykres_tb.GetBindingExpression(TextBox.TextProperty).UpdateTarget();
            }

        }

        private void showComparisons(object sender, RoutedEventArgs e)
        {
            ;
        }

        private void addProduct(object sender, RoutedEventArgs e)
        {

            //Otwieranie okna dodawania produktu i odświeżanie listy produktów danego miesiąca
            AddProductWindow window = new AddProductWindow(editingMonth);
            window.Title = "Dodawanie produktu";
            if ((bool)window.ShowDialog() == true)
            {
                RefreshProductList();
                RefreshPieChart();
            }
        }

        private void deleteProduct(object sender, RoutedEventArgs e)
        {
            var curItem = ((ListBoxItem)productListBox.ContainerFromElement((Button)sender)).Content;
            Product clickedProduct = (Product)curItem;
            MainWindow.db.DeleteProductForMonth(clickedProduct.NazwaProduktu, clickedProduct.Cena, clickedProduct.Ilosc, editingMonth.NazwaMiesiaca, editingMonth.Rok);
            
            //Odświeżanie listy produktów i wykresu
            RefreshProductList();
            RefreshPieChart();
        }

        private void editProduct(object sender, RoutedEventArgs e)
        {
            ;
        }

        private void BT_Close(object sender, RoutedEventArgs e)
        {
            Window.GetWindow(this).Close();
        }

        
        //Commandsy
        private void ShowMonthsSummaryExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            //Tworzenie DataTable przesyłanego do zestawienia
            DataTable dt = new DataTable("Example DataTable");
            dt.Columns.Add("Nazwa miesiaca", typeof(string));
            dt.Columns.Add("Rok", typeof(int));
            dt.Columns.Add("Wydatek", typeof(double));
            dt.Columns.Add("Przychód", typeof(double));
            List<Month> monthsToDataTable = MainWindow.db.GetMonths();
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
            if (MainWindow.db == null || MainWindow.db.GetMonths() == null || MainWindow.db.GetMonths().Count <= 0)
            {
                e.CanExecute = false;
            }
            else
            {
                e.CanExecute = true;
            }
        }
        private void PrintMonthExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            PDFCreator pdf = new PDFCreator();

            pdf.PreCreateDocument("Dane miesiąca " + editingMonth.NazwaMiesiaca + " " + editingMonth.Rok);
            string[] columnNames = new string[] { "ID", "Nazwa produktu", "Opis produktu", "Ilosc", "Cena", "Suma" };
            string[][] tableData = new string[productList.Count][];

            Product product;
            for (int i = 0; i < productList.Count; i++)
            {
                product = productList.ElementAt(i);
                tableData[i] = new string[] { i.ToString(), product.NazwaProduktu, product.Opis, product.Ilosc.ToString(), product.Cena.ToString(), (product.Ilosc * product.Cena).ToString() };
            }
            
            MigraDoc.DocumentObjectModel.Color[] colors = {
                new MigraDoc.DocumentObjectModel.Color((byte)119, (byte)221, (byte)119),
                new MigraDoc.DocumentObjectModel.Color((byte)0, (byte)255, (byte)255)
            };
            pdf.NewTable("Podsumowanie miesiąca " + editingMonth.NazwaMiesiaca + " " + editingMonth.Rok, columnNames, tableData, 2, colors);
            pdf.PostCreateDocument("Summary of Month " + editingMonth.NazwaMiesiaca + " " + editingMonth.Rok + " .pdf", 1);

        }
        private void PrintMonthCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (productList != null && productList.Count > 0)
                e.CanExecute = true;
            else
                e.CanExecute = false;
        }

        private void FixedExpensesExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            ;
        }

        private void FixedExpensesCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = false;
        }
    }
}
