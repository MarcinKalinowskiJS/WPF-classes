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
    /// Interaction logic for AddFixedExpenseWindow.xaml
    /// </summary>
    public partial class AddFixedExpenseWindow : Window
    {
        public AddFixedExpenseWindow()
        {
            InitializeComponent();
        }

        private void BT_AddFixedExpense(object sender, RoutedEventArgs e)
        {
            double expense = -1;
            int quantity = -1;
            if (int.TryParse(TB_quantity.Text, out quantity))
            {
                if (double.TryParse(TB_Expense.Text, out expense))
                {
                    MainWindow.db.AddFixedExpense(TB_Name.Text, TB_Description.Text, quantity, expense);
                    Window.GetWindow(this).Close();
                }
                else
                {
                    MessageBox.Show("Błędna wartość wydatku");
                }
            }
            else
            {
                MessageBox.Show("Błędna wartość ilości");
            }
        }
        private void BT_Close(object sender, RoutedEventArgs e)
        {
            Window.GetWindow(this).Close();
        }
    }
}
