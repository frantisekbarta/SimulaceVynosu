namespace SimulaceVynosu
{
    /// <summary>
    /// Podíly zastoupení akcií, dluhopisů a spoření (např. spořící účet nebo peněžní trh).
    /// </summary>
    class Profil
    {
        public double Akcie;
        public double Dluhopisy;
        public double Sporeni;

        public Profil(double akcie, double dluhopisy, double sporeni)
        {
            Akcie = akcie;
            Dluhopisy = dluhopisy;
            Sporeni = sporeni;
        }
    }
}
