using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Media;

namespace SimulaceVynosu
{
    /// <summary>
    /// Viewmodel hlavní obrazovky.
    /// </summary>
    class SimulaceVynosuViewModel : INotifyPropertyChanged
    {
        private int jednorazovaInvestice;
        public int JednorazovaInvestice
        {
            get { return jednorazovaInvestice; }
            set
            {
                jednorazovaInvestice = value;
                NotifyPropertyChanged("JednorazovaInvestice");
            }
        }

        private int pravidelnaInvestice;
        public int PravidelnaInvestice
        {
            get { return pravidelnaInvestice; }
            set
            {
                pravidelnaInvestice = value;
                NotifyPropertyChanged("PravidelnaInvestice");
            }
        }

        private int dobaInvestice;
        public int DobaInvestice
        {
            get { return dobaInvestice; }
            set
            {
                dobaInvestice = value;
                NotifyPropertyChanged("DobaInvestice");
            }
        }

        // názvy jsou využity v comboboxu
        public ObservableCollection<string> NazvyRizikovychProfilu { get; set; }

        private DruhProfilu vybranyProfil;

        public DruhProfilu VybranyProfil
        {
            get { return vybranyProfil; }
            set
            {
                vybranyProfil = value;
                NotifyPropertyChanged("VybranyProfil");
            }
        }

        private PointCollection bodyStredniHodnota = new PointCollection();
        public PointCollection BodyStredniHodnota
        {
            get { return bodyStredniHodnota; }
            set
            {
                bodyStredniHodnota = value;
                NotifyPropertyChanged("BodyStredniHodnota");
            }
        }

        private PointCollection bodyMinimum = new PointCollection();
        public PointCollection BodyMinimum
        {
            get { return bodyMinimum; }
            set
            {
                bodyMinimum = value;
                NotifyPropertyChanged("BodyMinimum");
            }
        }

        private PointCollection bodyMaximum = new PointCollection();
        public PointCollection BodyMaximum
        {
            get { return bodyMaximum; }
            set
            {
                bodyMaximum = value;
                NotifyPropertyChanged("BodyMaximum");
            }
        }

        private PointCollection bodyVlozenaCastka = new PointCollection();
        public PointCollection BodyVlozenaCastka
        {
            get { return bodyVlozenaCastka; }
            set
            {
                bodyVlozenaCastka = value;
                NotifyPropertyChanged("BodyVlozenaCastka");
            }
        }

        private PointCollection bodyNarustInflace = new PointCollection();
        public PointCollection BodyNarustInflace
        {
            get { return bodyNarustInflace; }
            set
            {
                bodyNarustInflace = value;
                NotifyPropertyChanged("BodyNarustInflace");
            }
        }

        private string ocekavanyVynos;
        public string OcekavanyVynos
        {
            get { return ocekavanyVynos; }
            set
            {
                ocekavanyVynos = value;
                NotifyPropertyChanged("OcekavanyVynos");
            }
        }

        private string vlozenaCastka;
        public string VlozenaCastka
        {
            get { return vlozenaCastka; }
            set
            {
                vlozenaCastka = value;
                NotifyPropertyChanged("VlozenaCastka");
            }
        }

        private string stredniHodnota;
        public string StredniHodnota
        {
            get { return stredniHodnota; }
            set
            {
                stredniHodnota = value;
                NotifyPropertyChanged("StredniHodnota");
            }
        }

        private string stredniHodnotaRealne;
        public string StredniHodnotaRealne
        {
            get { return stredniHodnotaRealne; }
            set
            {
                stredniHodnotaRealne = value;
                NotifyPropertyChanged("StredniHodnotaRealne");
            }
        }

        private string minimum;
        public string Minimum
        {
            get { return minimum; }
            set
            {
                minimum = value;
                NotifyPropertyChanged("Minimum");
            }
        }

        private string procentoNadVlozenouCastkou;
        public string ProcentoNadVlozenouCastkou
        {
            get { return procentoNadVlozenouCastkou; }
            set
            {
                procentoNadVlozenouCastkou = value;
                NotifyPropertyChanged("ProcentoNadVlozenouCastkou");
            }
        }

        private string procentoNadInflaci;
        public string ProcentoNadInflaci
        {
            get { return procentoNadInflaci; }
            set
            {
                procentoNadInflaci = value;
                NotifyPropertyChanged("ProcentoNadInflaci");
            }
        }

        private ObservableCollection<PopiskyOsy> popiskyOsyX;
        public ObservableCollection<PopiskyOsy> PopiskyOsyX
        {
            get { return popiskyOsyX; }
            set
            {
                popiskyOsyX = value;
                NotifyPropertyChanged("PopiskyOsyX");
            }
        }

        private ObservableCollection<PopiskyOsy> popiskyOsyY;
        public ObservableCollection<PopiskyOsy> PopiskyOsyY
        {
            get { return popiskyOsyY; }
            set
            {
                popiskyOsyY = value;
                NotifyPropertyChanged("PopiskyOsyY");
            }
        }

        private VysledkySimulace vysledkySimulace;

        private SimulaceVynosuService simulaceVynosuService;

        public RelayCommand PrepocitatCommand { get; private set; }
        public RelayCommand UlozitVysledkyCommand { get; private set; }

        public SimulaceVynosuViewModel()
        {
            JednorazovaInvestice = 1000000;
            PravidelnaInvestice = 5000;
            DobaInvestice = 20;
            NazvyRizikovychProfilu = new ObservableCollection<string> { "Dynamický", "Vyvážený", "Konzervativní" };
            VybranyProfil = DruhProfilu.Vyvazeny;
            simulaceVynosuService = new SimulaceVynosuService();
            PrepocitatCommand = new RelayCommand(Prepocitat, lzePrepocitat);
            UlozitVysledkyCommand = new RelayCommand(UlozitVysledky, lzeUlozitVysledky);
            Prepocitat(null);
            // zápis podílů aktiv do souboru
            // ZapisDoSouboru.ZapisPodiluAktiv();
        }

        public void Prepocitat(object parametr)
        {
            // zadání parametrů simulace
            vysledkySimulace = simulaceVynosuService.Simulace(JednorazovaInvestice, PravidelnaInvestice, VybranyProfil, DobaInvestice, Spolehlivost.K95);

            // zobrazení výsledků a vykreslení grafu
            OcekavanyVynos = (vysledkySimulace.OcekavanyVynos * 100).ToString("F1") + " %";
            VlozenaCastka = vysledkySimulace.VlozenaCastka[12 * DobaInvestice - 1].ToString("N0");
            StredniHodnota = vysledkySimulace.StredniHodnota[12 * DobaInvestice - 1].ToString("N0");
            StredniHodnotaRealne = vysledkySimulace.StredniHodnotaRealne[12 * DobaInvestice - 1].ToString("N0");
            Minimum = vysledkySimulace.Minimum[12 * DobaInvestice - 1].ToString("N0");
            ProcentoNadVlozenouCastkou = (vysledkySimulace.ProcentoNadVlozenouCastkou * 100).ToString("F0") + " %";
            ProcentoNadInflaci = (vysledkySimulace.ProcentoNadInflaci * 100).ToString("F0") + " %";

            PopiskyOsyX = new ObservableCollection<PopiskyOsy>();
            PopiskyOsyY = new ObservableCollection<PopiskyOsy>();

            double krokOsyX = Math.Round(500d / DobaInvestice);

            for (int i = 0; i < DobaInvestice; i++)
            {
                PopiskyOsyX.Add(new PopiskyOsy(-10 + (i + 1) * krokOsyX, 10, i + 1));
            }

            // určení rozsahu osy y na základě dynamického profilu rychlým deterministickým výpočtem, aby se při výběru dalších profilů případně neměnilo měřítko (což je pro srovnání profilů nežádoucí)
            double rozsahOsyY = PomocneMetody.RozsahGrafu(simulaceVynosuService.StredniHodnota(JednorazovaInvestice, PravidelnaInvestice, DruhProfilu.Dynamicky, DobaInvestice));
            double meritko = 500d / rozsahOsyY;
            double krokOsyY = rozsahOsyY / 4;

            PopiskyOsyY.Add(new PopiskyOsy(10, 0, 0));
            for (int i = 0; i < 4; i++)
            {
                PopiskyOsyY.Add(new PopiskyOsy(10, (i + 1) * krokOsyY * meritko, (i + 1) * krokOsyY));
            }

            BodyStredniHodnota = new PointCollection();
            BodyMaximum = new PointCollection();
            BodyMinimum = new PointCollection();
            BodyVlozenaCastka = new PointCollection();
            BodyNarustInflace = new PointCollection();

            double x;
            double yStredniHodnota;
            double yMaximum;
            double yMinimum;
            double yVlozenaCastka;
            double yNarustInflace;

            for (int i = 0; i < 12 * DobaInvestice; i++)
            {
                x = i * (500d / (12 * DobaInvestice));
                yStredniHodnota = 500 - vysledkySimulace.StredniHodnota[i] * meritko;
                yMaximum = 500 - vysledkySimulace.Maximum[i] * meritko;
                yMinimum = 500 - vysledkySimulace.Minimum[i] * meritko;
                yVlozenaCastka = 500 - vysledkySimulace.VlozenaCastka[i] * meritko;
                yNarustInflace = 500 - vysledkySimulace.NarustInflace[i] * meritko;
                BodyStredniHodnota.Add(new Point(x, yStredniHodnota));
                BodyMaximum.Add(new Point(x, yMaximum));
                BodyMinimum.Add(new Point(x, yMinimum));
                BodyVlozenaCastka.Add(new Point(x, yVlozenaCastka));
                BodyNarustInflace.Add(new Point(x, yNarustInflace));
            }
        }

        public bool lzePrepocitat(object parametr)
        {
            return true;
        }

        public void UlozitVysledky(object parametr)
        {
            // uložení výsledků do souboru
            ZapisDoSouboru.ZapisVysledku(vysledkySimulace);
        }

        public bool lzeUlozitVysledky(object parametr)
        {
            return true;
        }

        // převzato z https://docs.microsoft.com/cs-cz/dotnet/api/system.componentmodel.inotifypropertychanged.propertychanged?view=netcore-3.1
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
