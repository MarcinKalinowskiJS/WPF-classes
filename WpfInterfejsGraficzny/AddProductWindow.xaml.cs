using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    /// Interaction logic for AddProductWindow.xaml
    /// </summary>
    public partial class AddProductWindow : Window
    {
        Month selectedMonth = null;
        List<Product> productList = MainWindow.db.GetProducts();
        public ObservableCollection<string> KategorieBind { get; set; }

        public AddProductWindow(Month month)
        {
            KategorieBind = new ObservableCollection<string>();

            selectedMonth = month;

            this.DataContext = this;
            InitializeComponent();

            //Dodawanie istniejących produktów do wyboru w ComboBox
            foreach (var product in productList)
            {
                CB_AvailableProducts.Items.Add(product.NazwaProduktu + " " + product.Cena);
            }
        }

        private void AcceptAddProduct(object sender, RoutedEventArgs e)
        {
            double cena;
            int ilosc;
            if (double.TryParse(TB_Cena.Text, out cena))
            {
                if (int.TryParse(TB_Ilosc.Text, out ilosc))
                {
                    MainWindow.db.AddProductToMonthAndUpdateIfExists(TB_Nazwa.Text, TB_Opis.Text, new List <string>() /*KategorieBind*/, cena, 
                        ilosc, selectedMonth.NazwaMiesiaca, selectedMonth.Rok);
                    Window.GetWindow(this).DialogResult = true;
                    Window.GetWindow(this).Close();
                }
                else
                {
                    MessageBox.Show("Błędna wartość pola ilosc");
                }
            }
            else
            {
                MessageBox.Show("Błędna wartość pola cena");
            }
        }


        private void CB_AvailableProductsSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //Pobieranie danych o produkcie
            string selectedText = (sender as ComboBox).SelectedItem as string;
            string selectedProduct = Regex.Match(selectedText, "^[a-zA-ZóżźćąęÓŻŹĆĄĘ& ]+").Value;
            selectedProduct = selectedProduct.Remove(selectedProduct.Length - 1);//Usuwanie spacji z końca stringa
            string selectedPrice = Regex.Match(selectedText, "[0-9]*[,]?[0-9]*$").Value;
            double cena = 0;
            if (double.TryParse(selectedPrice, out cena))
            {

                //Przeszukiwanie listy produktów w celu zmiany zawartości TextBoxów
                Product product = null;
                for (int i = 0; i < productList.Count; i++)
                {
                    if (productList.ElementAt(i).Cena == cena && productList.ElementAt(i).NazwaProduktu.Equals(selectedProduct))
                    {
                        product = productList.ElementAt(i);
                        break;
                    }
                }

                if (product != null)
                {
                    //Zmiana zawartości TextBoxów
                    TB_Nazwa.Text = product.NazwaProduktu;
                    TB_Opis.Text = product.Opis;
                    string kategoriestring = "";
                    for (int i=0; i<product.Kategorie.Count; i++)
                    {
                        kategoriestring += product.Kategorie.ElementAt(i);
                        if(i + 1 < product.Kategorie.Count)
                        {
                            kategoriestring += ",";
                        }
                    }
                    TB_Kategorie.Text = kategoriestring;
                    TB_Cena.Text = product.Cena.ToString();
                    TB_Ilosc.Text = product.Ilosc.ToString();
                }
            }
        }

    }
}
