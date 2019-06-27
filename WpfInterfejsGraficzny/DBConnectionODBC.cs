using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Odbc;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfInterfejsGraficzny
{
    public class DBConnectionODBC
    {
        public const string ConnectionString = "DRIVER={MySQL ODBC 5.1 Driver};" + "SERVER=localhost;" +
            "DATABASE=projektwpf;" + "UID=root;" + "PASSWORD=;" + "PORT=3306" + "OPTION=3";
        string[] table_names = { "miesiace", "wydatkistale", "opcje", "produkty", "miesiace_produkty", "kategorie", "kategorie_produkty" };

        public void AddFixedExpense(string name, string description, int quantity, double expense)
        {
            OdbcConnection connection = new OdbcConnection(ConnectionString);
            connection.Open();

            //Znajdź kolejne wolne id dla wydatku stałego
            int nextExpenseId = findNextFreeFixedExpenseId();

            //Dodaj wydatek o danym id do bazy
            OdbcCommand command = new OdbcCommand("Insert into " + table_names[1] + " Values(" + nextExpenseId + ", '"
                + expense + "', " + quantity + ", '" + name + "', '" + description + "')", connection);

            command.ExecuteNonQuery();

            connection.Close();
        }

        public int findNextFreeFixedExpenseId()
        {
            OdbcConnection connection = new OdbcConnection(ConnectionString);
            connection.Open();

            int nextFreeId = 1;
            OdbcCommand command = new OdbcCommand("Select * From " + table_names[1], connection);
            OdbcDataReader dataReader = command.ExecuteReader();

            while(dataReader.Read())
            {
                if(dataReader.GetInt32(0) >= nextFreeId)
                {
                    nextFreeId = dataReader.GetInt32(0) + 1;
                }
            }

            return nextFreeId;
        }
        public void deleteFixedExpense(int id)
        {
            OdbcConnection connection = new OdbcConnection(ConnectionString);
            connection.Open();

            OdbcCommand command = new OdbcCommand("Delete from " + table_names[1] + " Where id_wydatkustalego=" + id, connection);
            command.ExecuteNonQuery();

            connection.Close();
        }
        public void UpdateFixedExpense(int id, string nazwa, string opis, int ilosc, double wydatek)
        {
            OdbcConnection connection = new OdbcConnection(ConnectionString);
            connection.Open();

            OdbcCommand command = new OdbcCommand("Update " + table_names[1] + " Set nazwa='" + nazwa + "', opis='" + opis
                + "',  ilosc=" + ilosc + ", wydatek='" + wydatek + "' Where id_wydatkustalego=" + id, connection);
            Trace.WriteLine("RowsAffected:" + command.ExecuteNonQuery());

            connection.Close();

        }

        public List<FixedExpense> GetFixedExpenses()
        {
            List<FixedExpense> fixedExpenseList = new List<FixedExpense>();

            OdbcConnection connection = new OdbcConnection(ConnectionString);
            connection.Open();

            OdbcCommand command = new OdbcCommand("Select * from " + table_names[1], connection);
            OdbcDataReader dataReader = command.ExecuteReader();

            while(dataReader.Read())
            {
                Decimal wydatek_decimal = dataReader.GetDecimal(1);
                double wydatek_double = 0;
                if (double.TryParse(wydatek_decimal.ToString(), out wydatek_double))
                {
                    fixedExpenseList.Add(new FixedExpense(dataReader.GetInt32(0), dataReader.GetString(3), dataReader.GetString(4), wydatek_double, dataReader.GetInt32(2)));
                }
            }

            dataReader.Close();
            connection.Close();

            return fixedExpenseList;
        }

        public int DeleteProductForMonth(string nazwaProduktu, double cena, int ilosc, string nazwaMiesiaca, int rok)
        {
            int monthId = findMonthId(nazwaMiesiaca, rok);
            int productId = findProductId(nazwaProduktu);
            int affectedRows;

            OdbcConnection connection = new OdbcConnection(ConnectionString);
            connection.Open();

            //Usuwanie danego produktu dla danego miesiaca (zamiana przecinków na kropki dla bazy danych)
            OdbcCommand command = new OdbcCommand("Delete from " + table_names[4] + " Where id_miesiaca=" + monthId
                + " AND id_produktu=" + productId + " AND ilosc=" + ilosc + " AND cena_jednostkowa='"
                + cena.ToString().Replace(",", ".") + "'", connection);

            Trace.WriteLine("Przed execute");
            affectedRows = command.ExecuteNonQuery();
            Trace.WriteLine("Afrow" + affectedRows);
            connection.Close();

            return affectedRows;
        }

        private int findProductId(string nazwaProduktu)
        {
            int productId = -1;

            OdbcConnection connection = new OdbcConnection(ConnectionString);
            connection.Open();

            OdbcCommand command = new OdbcCommand("Select id_produktu from " + table_names[3] + " Where nazwa='" + nazwaProduktu + "'", connection);
            OdbcDataReader dataReader = command.ExecuteReader();
            
            if(dataReader.Read())
            {
                productId = dataReader.GetInt32(0);
            }

            dataReader.Close();
            connection.Close();

            return productId;
        }

        public List<Product> GetProducts()
        {
            OdbcConnection connection = new OdbcConnection(ConnectionString);
            connection.Open();

            OdbcCommand command = new OdbcCommand("Select p.nazwa, p.opis, mp.cena_jednostkowa, p.opis, mp.ilosc, p.id_produktu from "
                + table_names[3] + " p, " + table_names[4] + " mp Where p.id_produktu=mp.id_produktu", connection);
            OdbcDataReader dataReader = command.ExecuteReader();

            List<Product> productList = new List<Product>();
            List<string> categories_s = null;
            ObservableCollection<string> categories_oc = null;
            while (dataReader.Read())
            {
                categories_s = GetCategoriesForProductId(dataReader.GetInt32(5));
                categories_oc = new ObservableCollection<string>();
                foreach (var category in categories_s)
                {
                    categories_oc.Add(category);
                }
                productList.Add(new Product(dataReader.GetString(0), dataReader.GetString(1), categories_oc, dataReader.GetInt32(4), (double)dataReader.GetDecimal(2)));
            }
            dataReader.Close();

            return productList;
        }


        public List<string> GetCategoriesForProductId(int id)
        {
            List<string> categories = new List<string>();
            OdbcConnection connection = new OdbcConnection(ConnectionString);
            connection.Open();

            OdbcCommand command = new OdbcCommand("Select k.nazwa, k.opis from " + table_names[5] + " k, " + table_names[6] +
                " kp Where k.id_kategorii=kp.id_kategorii AND kp.id_produktu=" + id, connection);
            OdbcDataReader dataReader = command.ExecuteReader();

            while (dataReader.Read())
            {
                categories.Add(dataReader.GetString(0));
            }
            dataReader.Close();

            return categories;
        }



        public List<Product> GetProductsForMonth(string nazwaMiesiaca, int rok)
        {
            List<Product> productListFromDB = new List<Product>();
            OdbcConnection connection = new OdbcConnection(ConnectionString);
            connection.Open();

            //Pobranie id miesiąca
            int monthId = findMonthId(nazwaMiesiaca, rok);

            //Pobranie danych z bazy MySQL na temat produktów według id w tabeli miesiace_produkty dla danego miesiaca
            OdbcCommand command = new OdbcCommand("Select * from " + table_names[4] + " Where id_miesiaca=" + monthId, connection);
            OdbcDataReader dataReader = command.ExecuteReader();
            while (dataReader.Read())
            {
                productListFromDB.Add( findProductById(dataReader.GetInt32(1)) );
                productListFromDB.Last().Ilosc = dataReader.GetInt32(2);
                productListFromDB.Last().Cena = (double)dataReader.GetDecimal(3);
            }

            dataReader.Close();
            connection.Close();

            return productListFromDB;
        }


        public int UpdateMonthsExpenses()
        {
            int rowsAffected = 0;
            OdbcConnection connection = new OdbcConnection(ConnectionString);
            connection.Open();

            //Pobranie miesięcy
            List<Month> monthListToUpdate = GetMonths();

            double fixedExpensesSum = 0;
            OdbcCommand command = new OdbcCommand("Select * from " + table_names[1], connection);
            OdbcDataReader dataReader = command.ExecuteReader();
            while(dataReader.Read())
            {
                fixedExpensesSum += dataReader.GetDouble(1) * dataReader.GetInt32(2);
            }
            dataReader.Close();

            double sum = 0;
            foreach (Month m in monthListToUpdate)
            {
                //Sumowanie wartości produktów dla danego miesiąca
                List<Product> productListForMonth = GetProductsForMonth(m.NazwaMiesiaca, m.Rok);
                sum = 0;
                foreach(Product p in productListForMonth)
                {
                    sum += p.Cena * p.Ilosc;
                }
                sum += fixedExpensesSum;

                //Update wydatków w danym miesiącu
                decimal d = decimal.Parse(sum.ToString());
                command.CommandText = "Update " + table_names[0] + " Set wydatki='" + decimal.Round(d, 2) +
                    "' Where id_miesiaca=" + findMonthId(m.NazwaMiesiaca, m.Rok);
                rowsAffected += command.ExecuteNonQuery();
            }
            connection.Close();

            return rowsAffected;
        }



        private Product findProductById(int id)
        {
            OdbcConnection connection = new OdbcConnection(ConnectionString);
            connection.Open();


            //Pobranie id kategorii danego produktu
            List<int> kategorieProduktuInt = new List<int>();
            OdbcCommand command = new OdbcCommand("Select * from " + table_names[6] + " Where id_produktu=" + id, connection);
            OdbcDataReader dataReader = command.ExecuteReader();
            while (dataReader.Read())
            {
                kategorieProduktuInt.Add(dataReader.GetInt32(1));
            }
            dataReader.Close();


            //Pobranie nazw kategorii o danym id dla danego produktu
            ObservableCollection<string> kategorieProduktuString = new ObservableCollection<string>();
            if (kategorieProduktuInt.Count > 0)
            {
                command.CommandText = "Select * From " + table_names[5] + " Where ";
                for (int i = 0; i < kategorieProduktuInt.Count; i++)
                {
                    command.CommandText += "id_kategorii =" + kategorieProduktuInt[i];
                    if (i + 1 < kategorieProduktuInt.Count)
                        command.CommandText += " OR ";
                }
                dataReader = command.ExecuteReader();
                while (dataReader.Read())    //Dodawanie nazw kategorii produktu do listy
                {
                    kategorieProduktuString.Add(dataReader.GetString(1));
                    //dataReader.GetString(2) opis kategorii
                }
                dataReader.Close();
            }

            //Pobranie nazwy produktu o danym id
            Product tmpProduct = null;
            command.CommandText = "Select * from " + table_names[3] + " Where id_produktu="+id;
            dataReader = command.ExecuteReader();
            if(dataReader.Read())
            {
                tmpProduct = new Product(dataReader.GetString(1), dataReader.GetString(2), kategorieProduktuString, 0, 0);
            }
            dataReader.Close();


            connection.Close();

            return tmpProduct;
        }

        private int findMonthId(string nazwaMiesiaca, int rok)
        {
            OdbcConnection connection = new OdbcConnection(ConnectionString);
            connection.Open();

            //Pobranie id miesiąca
            int monthId = -1;
            OdbcCommand command = new OdbcCommand("Select * from " + table_names[0] + " Where nazwa_miesiaca='" +
                nazwaMiesiaca + "' AND rok=" + rok, connection);
            OdbcDataReader dataReader = command.ExecuteReader();
            if (dataReader.Read())
            {
                monthId = dataReader.GetInt32(0);
            }

            dataReader.Close();
            connection.Close();

            return monthId;
        }


        public bool CheckConnection()
        {
            OdbcConnection connection = new OdbcConnection(ConnectionString);
            try {
                connection.Open();
                connection.Close();
            }catch (Exception e)
            {
                return false;
            }
            return true;
        }



        public void AddProductToMonthAndUpdateIfExists(string nazwa, string opis, List<string>kategorie, double cena, int ilosc, string nazwaMiesiaca, int rok)
        {
            OdbcConnection connection = new OdbcConnection(ConnectionString);
            connection.Open();

            //Sprawdź czy produkt istnieje
            bool productExistsInDatabase = false;//Wpis w produkty o danym produkcie
            int productId = -1;
            OdbcCommand command = new OdbcCommand("Select * from " + table_names[3] + " Where nazwa='" + nazwa + "'", connection);
            OdbcDataReader dataReader = command.ExecuteReader();
            if (dataReader.Read())
            {
                productExistsInDatabase = true;
                productId = dataReader.GetInt32(0);
            }
            dataReader.Close();

            bool productBinded = false; //Wpis w produkty_miesiace o cenie i ilosci zakupionej w danym miesiacu
            command.CommandText = "Select * from " + table_names[3] + " p," + table_names[4] + " mp Where p.nazwa='" + nazwa
            + "' AND mp.id_produktu=p.id_produktu AND mp.cena_jednostkowa='" + cena + "'" + " AND mp.id_miesiaca=" +
            findMonthId(nazwaMiesiaca, rok);
            dataReader = command.ExecuteReader();
            if (dataReader.Read())
            {
                productBinded = true;
            }
            dataReader.Close();


            //Jeśli istnieje to update
            if (productExistsInDatabase)
            {
                command.CommandText = "Update " + table_names[3] + " Set opis='" + opis + "' Where id_produktu=" + productId;
                command.ExecuteNonQuery();
            }
            else//Jeśli nie istnieje to insert na kolejnej wolnej pozycji
            {
                productId = FindNextFreeProductId();
                command.CommandText = "Insert into " + table_names[3] + " VALUES(" + productId + ", '" + nazwa + "', '" +
                    opis + "'";
                command.ExecuteNonQuery();
            }

            if (productBinded)//Jeśli istnieje wpis o ilosci i cenia w danym miesiącu to update
            {
                command.CommandText = "Update " + table_names[4] + " Set ilosc=" + ilosc + ", cena_jednostkowa='" + cena +
                    "' Where id_miesiaca=" + findMonthId(nazwaMiesiaca, rok) + " AND id_produktu=" + productId;
                command.ExecuteNonQuery();
            }
            else//Jeśli nie istnieje wpis o ilosci i cenie w danym miesiacu to insert
            {
                command.CommandText = "Insert into " + table_names[4] + " Values(" + findMonthId(nazwaMiesiaca, rok) +
                    ", " + productId + ", " + ilosc + ", '" + cena + "')";
                command.ExecuteNonQuery();
            }

            connection.Close();
        }


        private int FindNextFreeProductId()
        {
            OdbcConnection connection = new OdbcConnection(ConnectionString);
            connection.Open();

            //Wyszukaj kolejne wolne id
            int nextFreeId = -1;
            OdbcCommand command = new OdbcCommand("Select * from " + table_names[3], connection);
            OdbcDataReader dataReader = command.ExecuteReader();
            while(dataReader.Read())
            {
                if (dataReader.GetInt32(0) > nextFreeId)
                {
                    nextFreeId = dataReader.GetInt32(0);
                }
            }
            connection.Close();

            return nextFreeId;
        }



        public bool AddMonthIfNotExists(string NazwaMiesiaca, int Rok, double Przychod)
        {
            OdbcConnection connection = new OdbcConnection(ConnectionString);
            connection.Open();

            //Sprawdź czy miesiąc istnieje
            OdbcCommand command = new OdbcCommand("Select * from " + table_names[0] + 
                " Where nazwa_miesiaca='" + NazwaMiesiaca + "' AND rok=" + Rok, connection);
            OdbcDataReader dataReader = command.ExecuteReader();
            if(dataReader.Read())
            {
                return false;
            }
            dataReader.Close();

            //Wyszukaj kolejne id miesiąca
            int id = 0;
            command.CommandText = "Select * From " + table_names[0];
            dataReader = command.ExecuteReader();
            while(dataReader.Read())
            {
                if (id < dataReader.GetInt32(0))
                {
                    id = dataReader.GetInt32(0);
                }
            }
            dataReader.Close();

            //Dodaj miesiąc z znalezionym id
            command.CommandText = "Insert into " + table_names[0] + " VALUES(" + (++id) + ", '" + NazwaMiesiaca +
                "', " + 0.00 + ", '" + Convert.ToDecimal(Przychod) + "', " + Rok + ")";
            command.ExecuteNonQuery();

            return true;
        }


        public List<Month> GetMonths()
        {
            List<Month> monthListFromDB = new List<Month>();
            OdbcConnection connection = new OdbcConnection(ConnectionString);
            connection.Open();

            //Pobranie danych z bazy MySQL
            OdbcCommand command = new OdbcCommand("Select * from " + table_names[0], connection);
            OdbcDataReader dataReader = command.ExecuteReader();
            Month tmpMonth = null;
            while(dataReader.Read())
            {
                //W bazie:
                //id_miesiaca INT, nazwa_miesiaca VARCHAR(12), wydatki DECIMAL(10,2), przychody DECIMAL(10,2), rok DECIMAL(4,0)
                //Konstruktor w kodzie:
                //string nazwa, int rok, double przychod, double wydatek
                tmpMonth = new Month(dataReader.GetString(1), (int)dataReader.GetDecimal(4), 
                    (double)dataReader.GetDecimal(3), (double)dataReader.GetDecimal(2));
                monthListFromDB.Add(tmpMonth);
            }
            dataReader.Close();
            connection.Close();

            return monthListFromDB;
        }



        public int removeMonthFromDB(string nazwaMiesiaca, int rok)
        {
            OdbcConnection connection = new OdbcConnection(ConnectionString);
            connection.Open();

            //Pobranie id miesiaca
            int monthId = -1;
            monthId = findMonthId(nazwaMiesiaca, rok);

            //Usuwanie z tabeli miesiace_produkty danego miesiaca
            OdbcCommand command = new OdbcCommand("Delete From " + table_names[4] + " Where id_miesiaca=" + monthId, connection);
            command.ExecuteNonQuery();

            //Usuwanie z tabeli miesiace danego miesiaca
            int deletedRows = 0;
            command.CommandText = "Delete From " + table_names[0] + " Where id_miesiaca=" + monthId;
            deletedRows = command.ExecuteNonQuery();

            connection.Close();

            return deletedRows;
        }



        public void RecreateDatabaseWithValues()
        {
            OdbcConnection connection = new OdbcConnection(ConnectionString);
            connection.Open();


            //Usuwanie tabel
            OdbcCommand command = new OdbcCommand("Command text", connection);
            for (int i = 0; i < table_names.Count(); i++)
            {
                command.CommandText = "DROP TABLE IF EXISTS " + table_names[i];
                command.ExecuteNonQuery();
            }



            //Dodawanie tabel
            command.CommandText = "CREATE TABLE " + table_names[0] + "(id_miesiaca INT, nazwa_miesiaca VARCHAR(12), wydatki DECIMAL(10,2), przychody DECIMAL(10,2), rok DECIMAL(4,0)) ";
            command.ExecuteNonQuery();
            command.CommandText = "CREATE TABLE " + table_names[1] + "(id_wydatkustalego INT, wydatek DECIMAL(10,2), ilosc INT, nazwa VARCHAR(25), opis VARCHAR(64)) ";
            command.ExecuteNonQuery();
            command.CommandText = "CREATE TABLE " + table_names[2] + "(id_opcji INT, nazwa VARCHAR(45), wartosc BIT(1))";
            command.ExecuteNonQuery();
            command.CommandText = "CREATE TABLE " + table_names[3] + "(id_produktu INT, nazwa VARCHAR(45), opis VARCHAR(64))";
            command.ExecuteNonQuery();
            command.CommandText = "CREATE TABLE " + table_names[4] + "(id_miesiaca INT, id_produktu INT, ilosc INT, cena_jednostkowa DECIMAL(10,2))";
            command.ExecuteNonQuery();
            command.CommandText = "CREATE TABLE " + table_names[5] + "(id_kategorii INT, nazwa VARCHAR(25), opis VARCHAR(64))";
            command.ExecuteNonQuery();
            command.CommandText = "CREATE TABLE " + table_names[6] + "(id_produktu INT, id_kategorii INT)";
            command.ExecuteNonQuery();



            //Dodawanie danych do tabel
            //Tabela[1] miesiące
            command.CommandText = "INSERT INTO " + table_names[0] + " VALUES(1, 'Styczeń', 2500.20, 2700.00, 2017)";
            command.ExecuteNonQuery();
            command.CommandText = "INSERT INTO " + table_names[0] + " VALUES(2, 'Luty', 2200.99, 2700.00, 2017)";
            command.ExecuteNonQuery();
            command.CommandText = "INSERT INTO " + table_names[0] + " VALUES(3, 'Marzec', 2100.00, 2700.00, 2017)";
            command.ExecuteNonQuery();
            command.CommandText = "INSERT INTO " + table_names[0] + " VALUES(4, 'Kwiecień', 2750.45, 2700.00, 2017)";
            command.ExecuteNonQuery();
            command.CommandText = "INSERT INTO " + table_names[0] + " VALUES(5, 'Maj', 2700.20, 3000.00, 2017)";
            command.ExecuteNonQuery();
            command.CommandText = "INSERT INTO " + table_names[0] + " VALUES(6, 'Czerwiec', 2200.00, 2700.00, 2017)";
            command.ExecuteNonQuery();
            command.CommandText = "INSERT INTO " + table_names[0] + " VALUES(7, 'Lipiec', 2100.99, 2700.00, 2017)";
            command.ExecuteNonQuery();
            command.CommandText = "INSERT INTO " + table_names[0] + " VALUES(8, 'Sierpień', 2550.78, 2700.00, 2017)";
            command.ExecuteNonQuery();

            //Tabela[2]
            //wydatkistale
            //id_wydatkustalego INT, wydatek DECIMAL(10,2), ilosc INT, nazwa VARCHAR(25), opis VARCHAR(64)
            command.CommandText = "INSERT INTO " + table_names[1] + " VALUES(1, 120.00, 1, 'Podatek RTV', 'Podatek Radiowo-Telewizyjny')";
            command.ExecuteNonQuery();
            command.CommandText = "INSERT INTO " + table_names[1] + " VALUES(2, 90.00, 1, 'Netflix', 'Miesięczny abonament serwisu Netflix')";
            command.ExecuteNonQuery();
            command.CommandText = "INSERT INTO " + table_names[1] + " VALUES(3, 50.00, 1, 'Gaz', 'Gaz propan butan do kuchenki gazowej')";
            command.ExecuteNonQuery();

            //Tabela[3]
            //opcje

            //Tabela[4]
            //produkty - cena i ilość jest w tabeli mesiace_produkty, bo cena może się zmieniać w różnych miesiącach
            //id_produktu INT, nazwa VARCHAR(45), opis VARCHAR(64)
            command.CommandText = "INSERT INTO " + table_names[3] + " VALUES(1, 'Dosia', 'Proszek do prania')";
            command.ExecuteNonQuery();
            command.CommandText = "INSERT INTO " + table_names[3] + " VALUES(2, 'Chleb', 'Chleb białostocki')";
            command.ExecuteNonQuery();
            command.CommandText = "INSERT INTO " + table_names[3] + " VALUES(3, 'Pizza', 'Pizza od Rafalskiego')";
            command.ExecuteNonQuery();
            command.CommandText = "INSERT INTO " + table_names[3] + " VALUES(4, 'Ketchup', 'Ketchup Pudliszki')";
            command.ExecuteNonQuery();
            command.CommandText = "INSERT INTO " + table_names[3] + " VALUES(5, 'Kino', 'Bilet do kina Helios')";
            command.ExecuteNonQuery();
            command.CommandText = "INSERT INTO " + table_names[3] + " VALUES(6, 'Kebab', 'Kebab przy dworcu')";
            command.ExecuteNonQuery();
            command.CommandText = "INSERT INTO " + table_names[3] + " VALUES(7, 'Bryza', 'Proszek do prania')";
            command.ExecuteNonQuery();
            command.CommandText = "INSERT INTO " + table_names[3] + " VALUES(8, 'E-automat', 'Proszek do prania')";
            command.ExecuteNonQuery();
            command.CommandText = "INSERT INTO " + table_names[3] + " VALUES(9, 'Koszulka H&M', 'Oryginalna koszulka')";
            command.ExecuteNonQuery();
            command.CommandText = "INSERT INTO " + table_names[3] + " VALUES(10, 'Odkurzacz', 'Do sprzątania')";
            command.ExecuteNonQuery();


            //Tabela[5]
            //miesiace_produkty
            //id_miesiaca INT, id_produktu INT, ilosc INT, cena_jednostkowa DECIMAL(10,2)
            //w styczniu 2017
            command.CommandText = "INSERT INTO " + table_names[4] + " VALUES(1, 1, 2, 39.99)";
            command.ExecuteNonQuery();
            command.CommandText = "INSERT INTO " + table_names[4] + " VALUES(1, 2, 3, 1.99)";
            command.ExecuteNonQuery();
            command.CommandText = "INSERT INTO " + table_names[4] + " VALUES(1, 3, 1, 40.99)";
            command.ExecuteNonQuery();
            command.CommandText = "INSERT INTO " + table_names[4] + " VALUES(1, 4, 1, 9.99)";
            command.ExecuteNonQuery();
            command.CommandText = "INSERT INTO " + table_names[4] + " VALUES(1, 6, 1, 9.99)";
            command.ExecuteNonQuery();
            //W lutym 2017
            command.CommandText = "INSERT INTO " + table_names[4] + " VALUES(2, 5, 3, 14.99)";
            command.ExecuteNonQuery();
            command.CommandText = "INSERT INTO " + table_names[4] + " VALUES(2, 6, 1, 9.99)";
            command.ExecuteNonQuery();
            command.CommandText = "INSERT INTO " + table_names[4] + " VALUES(2, 7, 1, 39.99)";
            command.ExecuteNonQuery();
            command.CommandText = "INSERT INTO " + table_names[4] + " VALUES(2, 9, 2, 49.99)";
            command.ExecuteNonQuery();
            //W lipcu 2017
            command.CommandText = "INSERT INTO " + table_names[4] + " VALUES(7, 5, 3, 19.99)";
            command.ExecuteNonQuery();
            command.CommandText = "INSERT INTO " + table_names[4] + " VALUES(7, 4, 3, 9.99)";
            command.ExecuteNonQuery();
            //W siepniu 2017
            command.CommandText = "INSERT INTO " + table_names[4] + " VALUES(8, 3, 2, 39.99)";
            command.ExecuteNonQuery();
            command.CommandText = "INSERT INTO " + table_names[4] + " VALUES(8, 9, 2, 99.99)";
            command.ExecuteNonQuery();
            command.CommandText = "INSERT INTO " + table_names[4] + " VALUES(8, 10, 1, 199.99)";
            command.ExecuteNonQuery();

            //Tabela[6]
            //kategorie
            //id_kategorii INT, nazwa VARCHAR(25), opis VARCHAR(64)
            command.CommandText = "INSERT INTO " + table_names[5] + " VALUES(1, 'Środki czystości', 'Do czyszczenia różnego rodzaju powierzchni')";
            command.ExecuteNonQuery();
            command.CommandText = "INSERT INTO " + table_names[5] + " VALUES(2, 'Jedzenie', 'Do spożycia')";
            command.ExecuteNonQuery();
            command.CommandText = "INSERT INTO " + table_names[5] + " VALUES(3, 'Rozrywka', 'Żeby mieć coś z życia')";
            command.ExecuteNonQuery();
            command.CommandText = "INSERT INTO " + table_names[5] + " VALUES(4, 'Fast-food', 'Trochę niezdrowego jedzenia')";
            command.ExecuteNonQuery();
            command.CommandText = "INSERT INTO " + table_names[5] + " VALUES(5, 'Markowe Ubrania', '')";
            command.ExecuteNonQuery();
            command.CommandText = "INSERT INTO " + table_names[5] + " VALUES(6, 'Proszek do prania', '')";
            command.ExecuteNonQuery();


            //Tabela[7]
            //kategorie_produkty
            //id_produktu INT, id_kategorii INT
            command.CommandText = "INSERT INTO " + table_names[6] + " VALUES(1, 1)";
            command.ExecuteNonQuery();
            command.CommandText = "INSERT INTO " + table_names[6] + " VALUES(1, 6)";
            command.ExecuteNonQuery();
            command.CommandText = "INSERT INTO " + table_names[6] + " VALUES(2, 2)";
            command.ExecuteNonQuery();
            command.CommandText = "INSERT INTO " + table_names[6] + " VALUES(3, 2)";
            command.ExecuteNonQuery();
            command.CommandText = "INSERT INTO " + table_names[6] + " VALUES(3, 4)";
            command.ExecuteNonQuery();
            command.CommandText = "INSERT INTO " + table_names[6] + " VALUES(4, 2)";
            command.ExecuteNonQuery();
            command.CommandText = "INSERT INTO " + table_names[6] + " VALUES(5, 3)";
            command.ExecuteNonQuery();
            command.CommandText = "INSERT INTO " + table_names[6] + " VALUES(6, 2)";
            command.ExecuteNonQuery();
            command.CommandText = "INSERT INTO " + table_names[6] + " VALUES(6, 4)";
            command.ExecuteNonQuery();
            command.CommandText = "INSERT INTO " + table_names[6] + " VALUES(7, 1)";
            command.ExecuteNonQuery();
            command.CommandText = "INSERT INTO " + table_names[6] + " VALUES(7, 6)";
            command.ExecuteNonQuery();
            command.CommandText = "INSERT INTO " + table_names[6] + " VALUES(8, 1)";
            command.ExecuteNonQuery();
            command.CommandText = "INSERT INTO " + table_names[6] + " VALUES(8, 6)";
            command.ExecuteNonQuery();
            command.CommandText = "INSERT INTO " + table_names[6] + " VALUES(9, 5)";
            command.ExecuteNonQuery();

            connection.Close();
        }
    }
}
