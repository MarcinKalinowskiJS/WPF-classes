using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace WpfInterfejsGraficzny
{
    public class Commands
    {
        private static RoutedUICommand fixedExpenses;
        private static RoutedUICommand deleteMonth;
        private static RoutedUICommand editMonth;
        private static RoutedUICommand showMonthsSummary;
        private static RoutedUICommand printMonths;

        private static RoutedUICommand deleteProduct;
        private static RoutedUICommand editProduct;
        private static RoutedUICommand showMonthSummary;
        private static RoutedUICommand printMonth;



        static Commands()
        {
            fixedExpenses = new RoutedUICommand("Show fixed expenses", "FIXED_EXPENSES", typeof(Commands));
            deleteMonth = new RoutedUICommand("Delete month", "DELETE_MONTH", typeof(Commands));
            editMonth = new RoutedUICommand("Edit month", "EDIT_MONTH", typeof(Commands));
            showMonthsSummary = new RoutedUICommand("Show months summary", "SHOW_MONTHS_SUMMARY", typeof(Commands));
            printMonths = new RoutedUICommand("Print months summary", "PRINT_MONTHS_SUMMARY", typeof(Commands));

            deleteProduct = new RoutedUICommand("Delete product", "DELETE_PRODUCT", typeof(Commands));
            editProduct = new RoutedUICommand("Edit product", "EDIT_PRODUCT", typeof(Commands));
            showMonthSummary = new RoutedUICommand("Show month summary", "SHOW_MONTH_SUMMARY", typeof(Commands));
            printMonth = new RoutedUICommand("Print month summary", "PRINT_MONTH_SUMMARY", typeof(Commands));
        }



        public static RoutedUICommand DeleteMonth
        {
            get { return deleteMonth; }
        }
        public static RoutedUICommand EditMonth
        {
            get { return editMonth; }
        }
        public static RoutedUICommand ShowMonthsSummary
        {
            get { return showMonthsSummary; }
        }

        public static RoutedUICommand DeleteProduct
        {
            get { return deleteProduct; }
        }
        public static RoutedUICommand EditProduct
        {
            get { return editProduct; }
        }
        public static RoutedUICommand ShowMonthSummary
        {
            get { return showMonthSummary; }
        }
        public static RoutedUICommand PrintMonth
        {
            get { return printMonth; }
        }
        public static RoutedUICommand PrintMonths
        {
            get { return printMonths; }
        }
        public static RoutedUICommand FixedExpenses
        {
            get { return fixedExpenses; }
        }
    }
}
