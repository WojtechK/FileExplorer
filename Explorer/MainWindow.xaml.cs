using System;
using System.Collections.Generic;
using System.IO;
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
using System.Windows.Navigation;


namespace Explorer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Domyślny konstruktor
        /// </summary>
        #region Konstruktor

        public MainWindow()
        {
            InitializeComponent();

        }
        #endregion

        #region W momencie załadowania okna
        /// <summary>
        /// Metoda wywoływana w momencie załadowania okna
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // wybranie wszystkich dysków
            foreach (var dysk in Directory.GetLogicalDrives())
            {
                // stworzenie nowego elementu drzewa
                var item = new TreeViewItem()
                {
                    // ustawienie nagłówka i opisu na nazwę dysku
                    Header = dysk,
                    Tag = dysk
                };
                
                // dodanie durnego elementu
                item.Items.Add(null);
                //nasłuch na element - czy jest on rozwijany
                item.Expanded += Folder_Expanded;

                //dodanie elemntu do drzewa
                FolderView.Items.Add(item);
            }
        }
        #region Rozwinięcie katalogu
        private void Folder_Expanded(object sender, RoutedEventArgs e)
        {
            #region Sprawdzenie i inicjalizacja
            var item = (TreeViewItem) sender;
            // jeśli element zawiera jedynie głupi element
            if (item.Items.Count != 1 || item.Items[0] != null)
               return;
            //czyszczę wszystko - także głupi element
            item.Items.Clear();

            // wyciągnąć nazwę dysku
            var dysk = (string) item.Tag;
            #endregion
            #region Pobranie katalogów
            // tworzenie listy katalogów
            var katalogi = new List<string>();
            //wybieram nazwy katalogów na danym dysku i wrzucam do listy
            try { 
                var kat= Directory.GetDirectories(dysk);
                if (kat.Length > 0)
                    katalogi.AddRange(kat);
                }
            catch { }
            // dla każdego katalogu
            katalogi.ForEach(sciezkaKatalogow =>
            {
                // tworzenie elementu katalogu
                var subItem = new TreeViewItem()
                {
                    // ustawnienie nagłowka i opisu
                    Header = GetFileFolderName(sciezkaKatalogow),
                    Tag = sciezkaKatalogow
                };
                // dodawanie głupiego elementu tak aby móc rozwinąć folder
                subItem.Items.Add(null);
                // wywołanie siebie - pokazanie kolejnych podkatalogów
                subItem.Expanded += Folder_Expanded;

                // dodanie elementu do rodzica
                item.Items.Add(subItem);

            });
            #endregion
            #region Pobranie plików
            // tworzenie listy katalogów
            var pliki = new List<string>();
            //wybieram nazwy plikówz folderu i wrzucam do listy
            try
            {
                var pli = Directory.GetFiles(dysk);
                if (pli.Length > 0)
                    pliki.AddRange(pli);
            }
            catch { }
            // dla każdego pliku
            pliki.ForEach(sciezkaPlikow =>
            {
                // tworzenie elementu katalogu
                var subItem = new TreeViewItem()
                {
                    // ustawnienie nagłowka i opisu
                    Header = GetFileFolderName(sciezkaPlikow),
                    Tag = sciezkaPlikow
                };
                

                // dodanie elementu do rodzica
                item.Items.Add(subItem);

            });
            #endregion



        }
        #endregion
        /// <summary>
        /// Metoda wybierająca nazwę katalogu / pliku
        /// </summary>
        /// <param name="sciezkaKatalogow"></param>
        /// <returns></returns>
        private static string GetFileFolderName(string sciezkaKatalogow)
        {
           // c:\\kat\\kat1\\  katalog
            //c:\\kat\\plik.png  plik

            // jeśli ścieżka katalogów jest pusta, zwróć pustego stringa
            if (string.IsNullOrEmpty(sciezkaKatalogow))
                return string.Empty;
            // sprawdzenie czy są / i zamiana na \\
            var poprawnaSciezka = sciezkaKatalogow.Replace("/", "\\");

            // znalezienie ostatniego \\
            var ostatni = poprawnaSciezka.LastIndexOf("\\");

            //jeżeli nie znaleziono \\ zwróć ścieżkę
            if (ostatni <= 0)
                return sciezkaKatalogow;
            //zwracam nazwę po sotatnim backslashu
            return sciezkaKatalogow.Substring(ostatni + 1);

        }
        #endregion
    }
}
