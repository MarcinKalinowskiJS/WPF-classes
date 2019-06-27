using LiveCharts;
using LiveCharts.Wpf;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfInterfejsGraficzny
{
    public class ChartPointsClass
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged(string propName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propName));
        }


        private SeriesCollection _values;
        private string[] _title;

        public SeriesCollection Values
        {
            get
            {
                return _values;
            }
            set
            {
                _values = value;
                this.NotifyPropertyChanged("Values");
            }
        }

        public string[] Title
        {
            get
            {
                return _title;
            }
            set
            {
                _title = value;
                this.NotifyPropertyChanged("Title");
            }
        }

        public void AddPoint(double value, string title)
        {
            if (_values == null)
            {
                _values = new SeriesCollection();
            }
            _values.Add(new PieSeries { Values = new ChartValues<double> { value }, Title=title, DataLabels=true });
            _values.Last().LabelPoint = point => point.Y.ToString() + " " + point.Participation.ToString("P");
            this.NotifyPropertyChanged("Values");
            this.NotifyPropertyChanged("Title");
        }

        public void AddPointList(List<double> values, List<string> titles)
        {
            if (_values == null)
            {
                _values = new SeriesCollection();
            }
            for(int i=0; i<values.Count && i<titles.Count; i++)
            {
                _values.Add(new PieSeries { Values = new ChartValues<double> { values.ElementAt(i) }, Title = titles.ElementAt(i), DataLabels = true });
                _values.Last().LabelPoint = point => point.Y.ToString() + " " + point.Participation.ToString("P");
            }
            this.NotifyPropertyChanged("Values");
            this.NotifyPropertyChanged("Title");
        }

        public void RemoveAllPoints()
        {
            if (_values == null)
            {
                _values = new SeriesCollection();
            }

            _values.Clear();
        }

        public void LoadDefault()
        {
            _values = new SeriesCollection
            {
                new PieSeries
                {
                    Values = new ChartValues<double> { 21 },
                    Title = "TestLabel1",
                    DataLabels = true
                },
                new PieSeries
                {
                    Values = new ChartValues<double> { 4 },
                    Title = "TestLabel2",
                    DataLabels = true
                }
            };
            foreach(var element in _values)
            {
                element.LabelPoint = point => point.Y.ToString()  + " " + point.Participation.ToString("P");
            }
        }
    }
}
