using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace WpfInterfejsGraficzny
{
    public class StringCollectionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            
            ObservableCollection<string> kategorie = (ObservableCollection<string>)value;

            string kategorie_string = "";
            for (int i=0; kategorie!=null && i<kategorie.Count; i++)
            {
                kategorie_string += kategorie.ElementAt(i).ToString();
                if (i + 1 < kategorie.Count)
                    kategorie_string += ",";
            }

            return kategorie_string;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            ObservableCollection<string> kategorie = new ObservableCollection<string>();
            string kategorieString = value as string;
            kategorieString.Replace(" ", string.Empty);

            foreach (var s in kategorieString.Split(',').ToList())
            {
                kategorie.Add(s);
            }

            return kategorie;
        }
    }
}
