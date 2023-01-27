namespace SimulaceVynosu
{
    // enum s druhy profilů
    public enum DruhProfilu { Dynamicky, Vyvazeny, Konzervativni }

    class Spolehlivost
    {
        // hodnota kvantilu - např. 99 % hodnot je menší než μ + σ * K99 (1 % je menší než μ - σ * K99)
        public const double K99 = 2.3264;
        public const double K95 = 1.6449;
        public const double K90 = 1.2816;
        public const double K75 = 0.6745;
        public const double K50 = 0;

        // hodnota intervalu - např. 99 % hodnot je mezi μ +/- σ * I99
        public const double I99 = 2.5758;
        public const double I95 = 1.9600;
        public const double I90 = 1.6449;
        public const double I75 = 1.1504;
        public const double I50 = 0.6745;
    }

    class VstupniHodnoty
    {
        // roční míry růstu
        public const double MiraRustuAkcie = 0.07;
        public const double MiraRustuDluhopisy = 0.04;
        public const double MiraRustuSporeni = 0.02;
        public const double MiraRustuInflace = 0.03;

        // roční směrodatné odchylky
        public const double VariabilitaAkcie = 0.15;
        public const double VariabilitaDluhopisy = 0.08;
        public const double VariabilitaSporeni = 0;

        // počet iterací simulace
        public const int PocetIteraci = 10000;

        // podíly jednotlivých tříd aktiv v jednotlivých letech
        public static Profil[] Dynamicky = new Profil[30];
        public static Profil[] Vyvazeny = new Profil[30];
        public static Profil[] Konzervativni = new Profil[30];

        // algoritmus pro naplnění procenty, bylo by možné nahradit přímým zadáním nebo importem z csv
        static VstupniHodnoty()
        {
            // pomocné proměnné
            double akcie;
            double dluhopisy;
            double sporeni;

            // generování dynamického profilu
            for (int i = 0; i < 30; i++)
            {
                if (i > 19) akcie = 1 - (i - 19) * 0.1;
                else akcie = 1;
                if (i > 24) sporeni = (i - 24) * 0.2;
                else sporeni = 0;
                dluhopisy = 1 - akcie - sporeni;
                Dynamicky[i] = new Profil(akcie, dluhopisy, sporeni);
            }

            // generování vyváženého profilu
            for (int i = 0; i < 30; i++)
            {
                if (i > 19) akcie = 0.5 - (i - 19) * 0.05;
                else akcie = 0.5;
                if (i > 24) sporeni = (i - 24) * 0.2;
                else sporeni = 0;
                dluhopisy = 1 - akcie - sporeni;
                Vyvazeny[i] = new Profil(akcie, dluhopisy, sporeni);
            }

            // generování konzervativního profilu
            for (int i = 0; i < 30; i++)
            {
                if (i > 19) akcie = 0.25 - (i - 19) * 0.025;
                else akcie = 0.25;
                if (i > 24) sporeni = 0.5 + (i - 24) * 0.1;
                else sporeni = 0.5;
                dluhopisy = 1 - akcie - sporeni;
                Konzervativni[i] = new Profil(akcie, dluhopisy, sporeni);
            }
        }
    }
}
