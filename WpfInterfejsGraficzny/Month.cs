using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfInterfejsGraficzny
{
    public class Month : INotifyPropertyChanged
    { 
        public event PropertyChangedEventHandler PropertyChanged; 

        public void NotifyPropertyChanged(string propName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propName));
        }



        private string nazwaMiesiaca;
        private int rok;
        private double wydatek;
        private double przychod;



        public string NazwaMiesiaca
        {
            get
            {
                return nazwaMiesiaca;
            }
            set
            {
                nazwaMiesiaca = value;
                this.NotifyPropertyChanged("NazwaMiesiaca");
            }
        }
        public int Rok
        {
            get
            {
                return rok;
            }
            set
            {
                rok = value;
                this.NotifyPropertyChanged("Rok");
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
        public double Przychod
        {
            get
            {
                return przychod;
            }
            set
            {
                przychod = value;
                this.NotifyPropertyChanged("Przychod");
            }
        }
        public int NumerMiesiaca
        {
            get
            {
                switch (NazwaMiesiaca.ToLower())//Bez break, bo return
                {
                    case "styczeń": return 1;
                    case "luty": return 2;
                    case "marzec": return 3;
                    case "kwiecień": return 4;
                    case "maj": return 5;
                    case "czerwiec": return 6;
                    case "lipiec": return 7;
                    case "sierpień": return 8;
                    case "wrzesień": return 9;
                    case "październik": return 10;
                    case "listopad": return 11;
                    case "grudzień": return 12;
                    default: return 0;
                }
            }
            set
            {
                switch (value)
                {
                    case 1: NazwaMiesiaca = "Styczeń"; break;
                    case 2: NazwaMiesiaca = "Luty"; break;
                    case 3: NazwaMiesiaca = "Marzec"; break;
                    case 4: NazwaMiesiaca = "Kwiecień"; break;
                    case 5: NazwaMiesiaca = "Maj"; break;
                    case 6: NazwaMiesiaca = "Czerwiec"; break;
                    case 7: NazwaMiesiaca = "Lipiec"; break;
                    case 8: NazwaMiesiaca = "Sierpień"; break;
                    case 9: NazwaMiesiaca = "Wrzesień"; break;
                    case 10: NazwaMiesiaca = "Październik"; break;
                    case 11: NazwaMiesiaca = "Listopad"; break;
                    case 12: NazwaMiesiaca = "Grudzień"; break;
                    default: NazwaMiesiaca = "Styczen"; break;
                }
            }
        }



        public Month()
        {
            this.nazwaMiesiaca = "-";
            this.rok = 2018;
            this.przychod = 0;
            this.wydatek = 0;
        }
        public Month(string nazwa, int rok, double przychod, double wydatek)
        {
            this.nazwaMiesiaca = nazwa;
            this.rok = rok;
            this.przychod = przychod;
            this.wydatek = wydatek;
        }
    }
}
