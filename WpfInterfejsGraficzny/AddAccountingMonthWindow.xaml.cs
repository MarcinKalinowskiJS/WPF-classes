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
using System.Windows.Shapes;

namespace WpfInterfejsGraficzny
{
    /// <summary>
    /// Interaction logic for AddAccountingMonthWindow.xaml
    /// </summary>
    public partial class AddAccountingMonthWindow : Window
    {
        public AddAccountingMonthWindow()
        {
            InitializeComponent();
        }
        private void BT_AcceptAddMonth(object sender, RoutedEventArgs e)
        {
            int rok = 0;
            double przychod = 0;
            if (int.TryParse(TB_Rok.Text, out rok))
            {
                if (double.TryParse(TB_Przychod.Text, out przychod))
                {
                    if (rok > 0)
                    {
                        if (przychod >= 0)
                        {
                            //Dodawanie miesiąca do bazy danych
                            if(MainWindow.db.AddMonthIfNotExists(TB_NazwaMiesiaca.Text, rok, przychod))
                            {
                                //Zamknięcie okna i odświeżenie listy miesięcy w oknie głównym
                                Window.GetWindow(this).DialogResult = true;
                                Window.GetWindow(this).Close();
                            }
                            else
                            {
                                MessageBox.Show("Miesiąc " + TB_NazwaMiesiaca.Text + " " + TB_Rok.Text + " już istnieje w bazie");
                            }

                        }
                        else
                        {
                            MessageBox.Show("Przychód musi być większy od zera");
                        }
                    }
                    else
                    {
                        MessageBox.Show("Rok musi być większy od zera");
                    }
                }
                else
                {
                    MessageBox.Show("Błędna kwota przychodu:" + TB_Przychod);
                }
            }
            else
            {
                MessageBox.Show("Błędny rok:" + TB_Rok.Text);
            }
        }
    }
}
