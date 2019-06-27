using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfInterfejsGraficzny
{
    public class FixedExpense : INotifyPropertyChanged
    { 
        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged(string propName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propName));
        }


        private int id;
        private double wydatek;
        private int ilosc;
        private string nazwa;
        private string opis;



        public int ID
        {
            get
            {
                return id;
            }
            set
            {
                id = value;
                this.NotifyPropertyChanged("ID");
            }
        }
        public double Wydatek
        {
            get
            {
                return wydatek;
            }
            set
            {
                wydatek = value;
                this.NotifyPropertyChanged("Wydatek");
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
                this.NotifyPropertyChanged("Ilosc");
            }
        }
        public string Nazwa
        {
            get
            {
                return nazwa;
            }
            set
            {
                nazwa = value;
                this.NotifyPropertyChanged("Nazwa");
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



        public FixedExpense()
        {
            this.wydatek = 0;
            this.ilosc = 1;
            this.nazwa = "-";
            this.opis = "Brak opisu";
        }
        public FixedExpense(int id, string nazwa, string opis, double wydatek, int ilosc)
        {
            this.id = id;
            this.nazwa = nazwa;
            this.ilosc = ilosc;
            this.opis = opis;
            this.wydatek = wydatek;
        }
    }
}
