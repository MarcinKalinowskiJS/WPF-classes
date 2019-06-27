using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfInterfejsGraficzny
{
    public class Product : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged(string propName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propName));
        }



        private string nazwaProduktu;
        private string opis;
        private ObservableCollection<string> kategorie; //https://stackoverflow.com/questions/8470101/liststring-inotifypropertychanged-event
        private int ilosc;
        private double cena;



        public Product(string nazwaProduktu, string opis, ObservableCollection<string> kategorie, int ilosc, double cena)
        {
            this.nazwaProduktu = nazwaProduktu;
            this.opis = opis;
            this.kategorie = kategorie;
            this.ilosc = ilosc;
            this.cena = cena;
        }



        public string NazwaProduktu
        {
            get
            {
                return nazwaProduktu;
            }
            set
            {
                nazwaProduktu = value;
                this.NotifyPropertyChanged("NazwaProduktu");
            }
        }

        public string Opis
        {
            get
            {
                return opis;
            }
            set
            {
                opis = value;
                this.NotifyPropertyChanged("Opis");
            }
        }

        public ObservableCollection<string> Kategorie
        {
            get
            {
                return kategorie;
            }
            set
            {
                kategorie = value;
                this.NotifyPropertyChanged("Kategorie");
            }
        }

        public int Ilosc
        {
            get
            {
                return ilosc;
            }
            set
            {
                ilosc = value;
                NotifyPropertyChanged("Ilosc");
            }
        }

        public double Cena
        {
            get
            {
                return cena;
            }
            set
            {
                cena = value;
                NotifyPropertyChanged("Cena");
            }
        }
    }
}
