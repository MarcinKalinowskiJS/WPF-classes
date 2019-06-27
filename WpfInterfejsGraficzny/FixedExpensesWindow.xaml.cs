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
    /// Interaction logic for FixedExpensesWindow.xaml
    /// </summary>
    public partial class FixedExpensesWindow : Window
    {
        ObservableCollection<FixedExpense> fixedExpensesList = new ObservableCollection<FixedExpense>();
        private string searchField;
        private int lastSelectedIndex;
        private FixedExpense selectedFixedExpense
        {
            get;
            set;
        }

        public FixedExpensesWindow()
        {
            RefreshFixedExpensesList();

            FixedExpensesView.Filter = new Predicate<object>(o => Filter(o as FixedExpense));
            RefreshFixedExpensesList();

            this.DataContext = this;

            InitializeComponent();
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
                FixedExpensesView.Refresh(); //Odświeżenie listy miesięcy po wpisaniu wyszukiwanej frazy
            }

        }

        public ICollectionView FixedExpensesView
        {
            get { return CollectionViewSource.GetDefaultView(fixedExpensesList); }
        }

        private bool Filter(FixedExpense fixedExpense)
        {


            /*if (SearchField != null)
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
                }*/

            return SearchField == null
                || SearchField == ""
                || fixedExpense.Nazwa.IndexOf(SearchField, StringComparison.OrdinalIgnoreCase) != -1
                || fixedExpense.Opis.IndexOf(SearchField, StringComparison.OrdinalIgnoreCase) != -1;
        }

        public void RefreshFixedExpensesList()
        {
            fixedExpensesList.Clear();
            List<FixedExpense> tmpFixedExpensesList = null;
            tmpFixedExpensesList = MainWindow.db.GetFixedExpenses();
            foreach(var fixedExpense in tmpFixedExpensesList)
            {
                fixedExpensesList.Add(fixedExpense);
            }
        }

        private void fixedExpensesView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int selectedIndex = fixedExpensesListBox.SelectedIndex;
            if (selectedIndex > 0)
                lastSelectedIndex = selectedIndex;
            if(selectedIndex<0)
            {
                selectedIndex = 0;
            }
            if(fixedExpensesList.Count>0)
            {
                selectedFixedExpense = fixedExpensesList[selectedIndex];
                TB_Nazwa.Text = selectedFixedExpense.Nazwa;
                TB_Opis.Text = selectedFixedExpense.Opis;
                TB_Ilosc.Text = selectedFixedExpense.Ilosc.ToString();
                TB_Wydatek.Text = selectedFixedExpense.Wydatek.ToString();
            }
        }

        private void BT_ApplyChanges(object sender, RoutedEventArgs e)
        {
            //Zmiana wartości w bazie dancyh jeśli jest wybrany index
            if(lastSelectedIndex>=0)
            { 
                int ilosc = -1;
                if( int.TryParse(TB_Ilosc.Text, out ilosc) )
                {
                    double wydatek = -1;
                    if ( double.TryParse(TB_Wydatek.Text, out wydatek))
                    {
                        MainWindow.db.UpdateFixedExpense(selectedFixedExpense.ID, TB_Nazwa.Text, TB_Opis.Text, ilosc, wydatek);
                        RefreshFixedExpensesList();
                    }
                }
            }
        }
        private void BT_AddFixedExpense(object sender, RoutedEventArgs e)
        {
            AddFixedExpenseWindow window = new AddFixedExpenseWindow();

            window.Title = "Dodawanie wydatku stałego";
            if ((bool)window.ShowDialog() == true)
            {

            }

            //Update wartości po zmianie wydatku w danym miesiącu poprzez dodanie produktu
            RefreshFixedExpensesList();
        }
        private void BT_DeleteFixedExpense(object sender, RoutedEventArgs e)
        {
            var curItem = ((ListBoxItem)fixedExpensesListBox.ContainerFromElement((Button)sender)).Content;
            FixedExpense clickedFixedExpense = (FixedExpense)curItem;
            MainWindow.db.deleteFixedExpense(clickedFixedExpense.ID);
            
            //Odświeżanie listy po usunięciu
            RefreshFixedExpensesList();
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
