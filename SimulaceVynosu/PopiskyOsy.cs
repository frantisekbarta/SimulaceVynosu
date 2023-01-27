namespace SimulaceVynosu
{
    /// <summary>
    /// Využito pro binding popisků os.
    /// </summary>
    class PopiskyOsy
    {
        public double PoziceX { get; set; }
        public double PoziceY { get; set; }
        public double Popis { get; set; }

        public PopiskyOsy(double poziceX, double poziceY, double popis)
        {
            PoziceX = poziceX;
            PoziceY = poziceY;
            Popis = popis;
        }
    }
}
