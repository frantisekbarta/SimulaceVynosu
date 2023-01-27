namespace SimulaceVynosu
{
    /// <summary>
    /// Slouží pro uložení výsledků a následné zobrazení, nebo zápis na disk.
    /// </summary>
    class VysledkySimulace
    {
        // nominální vyjádření
        public double[] StredniHodnota;
        // dolní a horní hranice
        public double[] Minimum;
        public double[] Maximum;
        public double[] VlozenaCastka;
        // "zhodnocení" investice o inflaci
        public double[] NarustInflace;

        // reálné vyjádření
        public double[] StredniHodnotaRealne;
        public double[] MinimumRealne;
        public double[] MaximumRealne;
        public double[] VlozenaCastkaRealne;

        public double ProcentoNadVlozenouCastkou;
        public double ProcentoNadInflaci;
        public double OcekavanyVynos;
        public string Profil;
        public string Kvantil;

        // pro výchozí pozici výstupu
        public double JednorazovaInvestice;
        public double PravidelnaInvestice;
    }
}
