using System;

namespace SimulaceVynosu
{
    /// <summary>
    /// Algoritmy pro výpočet vývoje investic.
    /// </summary>
    class SimulaceVynosuService
    {
        // průměrné měsíční výnosové míry vypočítané z efektivních ročních výnosových měr
        private double stredniHodnotaAkcie;
        private double stredniHodnotaDluhopisy;
        private double stredniHodnotaSporeni;
        private double stredniHodnotaInflace;

        // měsíční směrodatné odchylky (roční vydělené odmocninou z 12) 
        private double smerodatnaOdchylkaAkcie;
        private double smerodatnaOdchylkaDluhopisy;
        private double smerodatnaOdchylkaSporeni;

        // vývoj tříd aktiv po měsících
        private double[] hodnotyAkcie;
        private double[] hodnotyDluhopisy;
        private double[] hodnotySporeni;

        // násobek vývoje tříd aktiv a jejich podílů v profilech
        private double[] hodnotyDynamicky;
        private double[] hodnotyVyvazeny;
        private double[] hodnotyKonzervativni;

        // matice se simulacemi investice
        private double[,] hodnotySimulace;
        // střední hodnota všech simulací
        private double[] stredniHodnotaSimulace;
        // směrodatná odchylka všech simulací
        private double[] smerodatnaOdchylkaSimulace;

        private Random nahodnaRada = new Random();

        private VysledkySimulace vysledky = new VysledkySimulace();

        /// <summary>
        /// Vytvoří matici se simulovanými průběhy investice na základě jednorázové částky, měsíční pravidelné částky, druhu rizikového profilu, doby trvání investice v letech a hodnoty kvantilu pro zvolenou spolehlivost).
        /// </summary>
        public VysledkySimulace Simulace(int jednorazovaInvestice, int pravidelnaInvestice, DruhProfilu profilInvestice, int dobaInvestice, double kvantil)
        {
            hodnotyAkcie = new double[12 * dobaInvestice];
            hodnotyDluhopisy = new double[12 * dobaInvestice];
            hodnotySporeni = new double[12 * dobaInvestice];

            hodnotyDynamicky = new double[12 * dobaInvestice];
            hodnotyVyvazeny = new double[12 * dobaInvestice];
            hodnotyKonzervativni = new double[12 * dobaInvestice];

            hodnotySimulace = new double[VstupniHodnoty.PocetIteraci, 12 * dobaInvestice];
            stredniHodnotaSimulace = new double[12 * dobaInvestice];
            smerodatnaOdchylkaSimulace = new double[12 * dobaInvestice];

            vysledky.StredniHodnota = new double[12 * dobaInvestice];
            vysledky.Minimum = new double[12 * dobaInvestice];
            vysledky.Maximum = new double[12 * dobaInvestice];
            vysledky.VlozenaCastka = new double[12 * dobaInvestice];
            vysledky.NarustInflace = new double[12 * dobaInvestice];
            vysledky.StredniHodnotaRealne = new double[12 * dobaInvestice];
            vysledky.MinimumRealne = new double[12 * dobaInvestice];
            vysledky.MaximumRealne = new double[12 * dobaInvestice];
            vysledky.VlozenaCastkaRealne = new double[12 * dobaInvestice];

            // převod na měsíční úrokovou míru
            stredniHodnotaAkcie = Math.Pow(1 + VstupniHodnoty.MiraRustuAkcie, 1d / 12) - 1;
            stredniHodnotaDluhopisy = Math.Pow(1 + VstupniHodnoty.MiraRustuDluhopisy, 1d / 12) - 1;
            stredniHodnotaSporeni = Math.Pow(1 + VstupniHodnoty.MiraRustuSporeni, 1d / 12) - 1;
            stredniHodnotaInflace = Math.Pow(1 + VstupniHodnoty.MiraRustuInflace, 1d / 12) - 1;

            // převod na měsíční směrodatnou odchylku
            smerodatnaOdchylkaAkcie = VstupniHodnoty.VariabilitaAkcie / Math.Sqrt(12);
            smerodatnaOdchylkaDluhopisy = VstupniHodnoty.VariabilitaDluhopisy / Math.Sqrt(12);
            smerodatnaOdchylkaSporeni = VstupniHodnoty.VariabilitaSporeni / Math.Sqrt(12);

            // pomocná proměnná pro jeden simulovaný průběh investice
            double hodnotaInvestice = jednorazovaInvestice;

            // generování hodnot pro první průběh simulace
            GenerovaniHodnot(profilInvestice, dobaInvestice);

            for (int j = 0; j < VstupniHodnoty.PocetIteraci; j++)
            {
                for (int i = 0; i < 12 * dobaInvestice; i++)
                {
                    if (profilInvestice == DruhProfilu.Dynamicky)
                        hodnotaInvestice = (hodnotaInvestice + pravidelnaInvestice) * (1 + hodnotyDynamicky[i]);
                    if (profilInvestice == DruhProfilu.Vyvazeny)
                        hodnotaInvestice = (hodnotaInvestice + pravidelnaInvestice) * (1 + hodnotyVyvazeny[i]);
                    if (profilInvestice == DruhProfilu.Konzervativni)
                        hodnotaInvestice = (hodnotaInvestice + pravidelnaInvestice) * (1 + hodnotyKonzervativni[i]);
                    hodnotySimulace[j, i] = hodnotaInvestice;
                    // načítání logaritmů pro získání logaritmicko normalního rozdělení
                    stredniHodnotaSimulace[i] = stredniHodnotaSimulace[i] + Math.Log(hodnotySimulace[j, i]);
                }
                hodnotaInvestice = jednorazovaInvestice;
                // generování hodnot pro další průběh simulace
                GenerovaniHodnot(profilInvestice, dobaInvestice);
            }
            for (int i = 0; i < 12 * dobaInvestice; i++)
            {
                // zprůměrování načtených logaritmů
                stredniHodnotaSimulace[i] = stredniHodnotaSimulace[i] / VstupniHodnoty.PocetIteraci;
            }
            for (int j = 0; j < VstupniHodnoty.PocetIteraci; j++)
            {
                for (int i = 0; i < 12 * dobaInvestice; i++)
                {
                    // načítání čtverců odchylek
                    smerodatnaOdchylkaSimulace[i] = smerodatnaOdchylkaSimulace[i] + (Math.Log(hodnotySimulace[j, i]) - stredniHodnotaSimulace[i]) * (Math.Log(hodnotySimulace[j, i]) - stredniHodnotaSimulace[i]);
                }
            }
            for (int i = 0; i < 12 * dobaInvestice; i++)
            {
                // zprůměrování načtených čtverců odchylek
                smerodatnaOdchylkaSimulace[i] = smerodatnaOdchylkaSimulace[i] / VstupniHodnoty.PocetIteraci;
                // finální výpočet směrodatné odchylky
                smerodatnaOdchylkaSimulace[i] = Math.Sqrt(smerodatnaOdchylkaSimulace[i]);
            }

            // tady je pomocná proměnná použita pro výpočet nárůstu inflace
            hodnotaInvestice = jednorazovaInvestice;

            for (int i = 0; i < 12 * dobaInvestice; i++)
            {
                // vzorce pro logaritmicko normální rozdělení
                vysledky.StredniHodnota[i] = Math.Exp(stredniHodnotaSimulace[i] + smerodatnaOdchylkaSimulace[i] * smerodatnaOdchylkaSimulace[i] / 2);
                vysledky.Minimum[i] = Math.Exp(stredniHodnotaSimulace[i] - kvantil * smerodatnaOdchylkaSimulace[i]);
                vysledky.Maximum[i] = Math.Exp(stredniHodnotaSimulace[i] + kvantil * smerodatnaOdchylkaSimulace[i]);
                vysledky.VlozenaCastka[i] = jednorazovaInvestice + pravidelnaInvestice * (1 + i);
                hodnotaInvestice = (hodnotaInvestice + pravidelnaInvestice) * (1 + stredniHodnotaInflace);
                vysledky.NarustInflace[i] = hodnotaInvestice;

                vysledky.StredniHodnotaRealne[i] = vysledky.StredniHodnota[i] * vysledky.VlozenaCastka[i] / vysledky.NarustInflace[i];
                vysledky.MinimumRealne[i] = vysledky.Minimum[i] * vysledky.VlozenaCastka[i] / vysledky.NarustInflace[i];
                vysledky.MaximumRealne[i] = vysledky.Maximum[i] * vysledky.VlozenaCastka[i] / vysledky.NarustInflace[i];
                vysledky.VlozenaCastkaRealne[i] = vysledky.VlozenaCastka[i] * vysledky.VlozenaCastka[i] / vysledky.NarustInflace[i];
            }

            // výpočet procenta případů, kdy je hodnota investice nad vloženou částkou resp. inflací
            for (int j = 0; j < VstupniHodnoty.PocetIteraci; j++)
            {
                if (hodnotySimulace[j, 12 * dobaInvestice - 1] >= vysledky.VlozenaCastka[12 * dobaInvestice - 1])
                    vysledky.ProcentoNadVlozenouCastkou++;
                if (hodnotySimulace[j, 12 * dobaInvestice - 1] >= vysledky.NarustInflace[12 * dobaInvestice - 1])
                    vysledky.ProcentoNadInflaci++;
            }

            vysledky.ProcentoNadVlozenouCastkou = vysledky.ProcentoNadVlozenouCastkou / VstupniHodnoty.PocetIteraci;
            vysledky.ProcentoNadInflaci = vysledky.ProcentoNadInflaci / VstupniHodnoty.PocetIteraci;
            vysledky.Profil = profilInvestice.ToString();
            vysledky.Kvantil = kvantil.ToString();

            // první prvek pro výpočet geometrického průměru
            if (profilInvestice == DruhProfilu.Dynamicky)
                vysledky.OcekavanyVynos = 1 + VstupniHodnoty.Dynamicky[30 - dobaInvestice].Akcie * VstupniHodnoty.MiraRustuAkcie + VstupniHodnoty.Dynamicky[30 - dobaInvestice].Dluhopisy * VstupniHodnoty.MiraRustuDluhopisy + VstupniHodnoty.Dynamicky[30 - dobaInvestice].Sporeni * VstupniHodnoty.MiraRustuSporeni;
            if (profilInvestice == DruhProfilu.Vyvazeny)
                vysledky.OcekavanyVynos = 1 + VstupniHodnoty.Vyvazeny[30 - dobaInvestice].Akcie * VstupniHodnoty.MiraRustuAkcie + VstupniHodnoty.Vyvazeny[30 - dobaInvestice].Dluhopisy * VstupniHodnoty.MiraRustuDluhopisy + VstupniHodnoty.Vyvazeny[30 - dobaInvestice].Sporeni * VstupniHodnoty.MiraRustuSporeni;
            if (profilInvestice == DruhProfilu.Konzervativni)
                vysledky.OcekavanyVynos = 1 + VstupniHodnoty.Konzervativni[30 - dobaInvestice].Akcie * VstupniHodnoty.MiraRustuAkcie + VstupniHodnoty.Konzervativni[30 - dobaInvestice].Dluhopisy * VstupniHodnoty.MiraRustuDluhopisy + VstupniHodnoty.Konzervativni[30 - dobaInvestice].Sporeni * VstupniHodnoty.MiraRustuSporeni;

            // donásobení ostatních prvků
            for (int i = 1 + 30 - dobaInvestice; i < 30; i++)
            {
                if (profilInvestice == DruhProfilu.Dynamicky)
                    vysledky.OcekavanyVynos = vysledky.OcekavanyVynos * (1 + VstupniHodnoty.Dynamicky[i].Akcie * VstupniHodnoty.MiraRustuAkcie + VstupniHodnoty.Dynamicky[i].Dluhopisy * VstupniHodnoty.MiraRustuDluhopisy + VstupniHodnoty.Dynamicky[i].Sporeni * VstupniHodnoty.MiraRustuSporeni);
                if (profilInvestice == DruhProfilu.Vyvazeny)
                    vysledky.OcekavanyVynos = vysledky.OcekavanyVynos * (1 + VstupniHodnoty.Vyvazeny[i].Akcie * VstupniHodnoty.MiraRustuAkcie + VstupniHodnoty.Vyvazeny[i].Dluhopisy * VstupniHodnoty.MiraRustuDluhopisy + VstupniHodnoty.Vyvazeny[i].Sporeni * VstupniHodnoty.MiraRustuSporeni);
                if (profilInvestice == DruhProfilu.Konzervativni)
                    vysledky.OcekavanyVynos = vysledky.OcekavanyVynos * (1 + VstupniHodnoty.Konzervativni[i].Akcie * VstupniHodnoty.MiraRustuAkcie + VstupniHodnoty.Konzervativni[i].Dluhopisy * VstupniHodnoty.MiraRustuDluhopisy + VstupniHodnoty.Konzervativni[i].Sporeni * VstupniHodnoty.MiraRustuSporeni);
            }

            // dopočítání geometrického průměru
            vysledky.OcekavanyVynos = Math.Pow(vysledky.OcekavanyVynos, 1d / dobaInvestice) - 1;

            vysledky.JednorazovaInvestice = jednorazovaInvestice;
            vysledky.PravidelnaInvestice = pravidelnaInvestice;

            return vysledky;
        }

        // generování hodnot jednotlivých tříd aktiv a hodnot v jednotlivých profilech
        void GenerovaniHodnot(DruhProfilu profilInvestice, int dobaInvestice)
        {
            // pomocné proměnné pro načítání měsíců a roků
            int mesic = 0;
            int rok = 30 - dobaInvestice;

            // experimentální implementace návratu ke střední hodnotě, použitý parametr rychlosti reverze je intuitivní a nevychází z žádného odhadu
            // viz https://www.investopedia.com/terms/v/vasicek-model.asp
            double rychlostReverze = 0.1;
            double minulaHodnotaAkcie = stredniHodnotaAkcie;
            double minulaHodnotaDluhopisy = stredniHodnotaDluhopisy;
            double minulaHodnotaSporeni = stredniHodnotaSporeni;
            // defaultně vypnuto
            bool pouzitiReverze = false;

            for (int i = 0; i < 12 * dobaInvestice; i++)
            {
                hodnotyAkcie[i] = MiraRustuNorm(stredniHodnotaAkcie, smerodatnaOdchylkaAkcie);
                hodnotyDluhopisy[i] = MiraRustuNorm(stredniHodnotaDluhopisy, smerodatnaOdchylkaDluhopisy);
                hodnotySporeni[i] = MiraRustuNorm(stredniHodnotaSporeni, smerodatnaOdchylkaSporeni);
                if (pouzitiReverze == true)
                {
                    hodnotyAkcie[i] = hodnotyAkcie[i] + rychlostReverze * (stredniHodnotaAkcie - minulaHodnotaAkcie);
                    minulaHodnotaAkcie = hodnotyAkcie[i];
                    hodnotyDluhopisy[i] = hodnotyDluhopisy[i] + rychlostReverze * (stredniHodnotaDluhopisy - minulaHodnotaDluhopisy);
                    minulaHodnotaDluhopisy = hodnotyDluhopisy[i];
                    hodnotySporeni[i] = hodnotySporeni[i] + rychlostReverze * (stredniHodnotaSporeni - minulaHodnotaSporeni);
                    minulaHodnotaSporeni = hodnotySporeni[i];
                }
            }

            for (int i = 0; i < 12 * dobaInvestice; i++)
            {
                if (profilInvestice == DruhProfilu.Dynamicky)
                    hodnotyDynamicky[i] = VstupniHodnoty.Dynamicky[rok].Akcie * hodnotyAkcie[i] + VstupniHodnoty.Dynamicky[rok].Dluhopisy * hodnotyDluhopisy[i] + VstupniHodnoty.Dynamicky[rok].Sporeni * hodnotySporeni[i];
                if (profilInvestice == DruhProfilu.Vyvazeny)
                    hodnotyVyvazeny[i] = VstupniHodnoty.Vyvazeny[rok].Akcie * hodnotyAkcie[i] + VstupniHodnoty.Vyvazeny[rok].Dluhopisy * hodnotyDluhopisy[i] + VstupniHodnoty.Vyvazeny[rok].Sporeni * hodnotySporeni[i];
                if (profilInvestice == DruhProfilu.Konzervativni)
                    hodnotyKonzervativni[i] = VstupniHodnoty.Konzervativni[rok].Akcie * hodnotyAkcie[i] + VstupniHodnoty.Konzervativni[rok].Dluhopisy * hodnotyDluhopisy[i] + VstupniHodnoty.Konzervativni[rok].Sporeni * hodnotySporeni[i];
                mesic++;
                if (mesic == 12)
                {
                    rok++;
                    mesic = 0;
                }
            }
        }

        // generování náhodných čísel s normálním rozdělením
        double MiraRustuNorm(double stredniHodnota, double smerodatnaOdchylka)
        {
            double prvni;
            double druhe;
            double nahodneCislo;
            prvni = nahodnaRada.NextDouble();
            druhe = nahodnaRada.NextDouble();
            // Box-Muller transformace
            nahodneCislo = Math.Sqrt(-2 * Math.Log(prvni)) * Math.Cos(2 * Math.PI * druhe) * smerodatnaOdchylka + stredniHodnota;
            return nahodneCislo;
        }

        // pomocná metoda pro rychlý deterministický výpočet střední hodnoty, využitá pro určení rozsahu osy y grafu
        public double StredniHodnota(int jednorazovaInvestice, int pravidelnaInvestice, DruhProfilu profilInvestice, int dobaInvestice)
        {
            double stredniHodnota;
            int mesic = 0;
            int rok = 30 - dobaInvestice;

            // první prvek
            if (profilInvestice == DruhProfilu.Dynamicky)
                stredniHodnota = (jednorazovaInvestice + pravidelnaInvestice) * (1 + VstupniHodnoty.Dynamicky[rok].Akcie * stredniHodnotaAkcie + VstupniHodnoty.Dynamicky[rok].Dluhopisy * stredniHodnotaDluhopisy + VstupniHodnoty.Dynamicky[rok].Sporeni * stredniHodnotaSporeni);
            else if (profilInvestice == DruhProfilu.Vyvazeny)
                stredniHodnota = (jednorazovaInvestice + pravidelnaInvestice) * (1 + VstupniHodnoty.Vyvazeny[rok].Akcie * stredniHodnotaAkcie + VstupniHodnoty.Vyvazeny[rok].Dluhopisy * stredniHodnotaDluhopisy + VstupniHodnoty.Vyvazeny[rok].Sporeni * stredniHodnotaSporeni);
            else
                stredniHodnota = (jednorazovaInvestice + pravidelnaInvestice) * (1 + VstupniHodnoty.Konzervativni[rok].Akcie * stredniHodnotaAkcie + VstupniHodnoty.Konzervativni[rok].Dluhopisy * stredniHodnotaDluhopisy + VstupniHodnoty.Konzervativni[rok].Sporeni * stredniHodnotaSporeni);
            mesic++;

            // donásobení ostatních prvků
            for (int i = 1; i < 12 * dobaInvestice; i++)
            {
                stredniHodnota = stredniHodnota + pravidelnaInvestice;
                if (profilInvestice == DruhProfilu.Dynamicky)
                    stredniHodnota = stredniHodnota * (1 + VstupniHodnoty.Dynamicky[rok].Akcie * stredniHodnotaAkcie + VstupniHodnoty.Dynamicky[rok].Dluhopisy * stredniHodnotaDluhopisy + VstupniHodnoty.Dynamicky[rok].Sporeni * stredniHodnotaSporeni);
                else if (profilInvestice == DruhProfilu.Vyvazeny)
                    stredniHodnota = stredniHodnota * (1 + VstupniHodnoty.Vyvazeny[rok].Akcie * stredniHodnotaAkcie + VstupniHodnoty.Vyvazeny[rok].Dluhopisy * stredniHodnotaDluhopisy + VstupniHodnoty.Vyvazeny[rok].Sporeni * stredniHodnotaSporeni);
                else
                    stredniHodnota = stredniHodnota * (1 + VstupniHodnoty.Konzervativni[rok].Akcie * stredniHodnotaAkcie + VstupniHodnoty.Konzervativni[rok].Dluhopisy * stredniHodnotaDluhopisy + VstupniHodnoty.Konzervativni[rok].Sporeni * stredniHodnotaSporeni);
                mesic++;
                if (mesic == 12)
                {
                    rok++;
                    mesic = 0;
                }
            }
            return stredniHodnota;
        }
    }
}
