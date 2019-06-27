using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
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
    /// Interaction logic for MonthsSummaryWindow.xaml
    /// </summary>
    public partial class MonthsSummaryWindow : Window
    {
        private string searchField;
        DataView dv;

        public string SearchField
        {
            get
            {
                return searchField;
            }
            set
            {
                searchField = value;
                dv.RowFilter = SearchField;
                MonthsDataGrid.Items.Refresh();
            }

        }

        public MonthsSummaryWindow(DataTable dt)
        {
            InitializeComponent();
            dv = new DataView(dt);
            this.DataContext = this;
            MonthsDataGrid.ItemsSource = dv;
        }
    }
}
