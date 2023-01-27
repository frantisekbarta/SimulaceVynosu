using System;

namespace SimulaceVynosu
{
    class PomocneMetody
    {
        // Vypočítá vhodné maximum grafu (nejbližší vyšší číslo v řádu zadaného čísla).
        // Např. pro 5865 bude maximum 6000, pro 111 bude maximum 200 atd.
        public static double RozsahGrafuStejnyRad(double cislo)
        {
            double rozsah;
            double rad;
            rad = Math.Pow(10, Math.Floor(Math.Log10(cislo)));
            rozsah = Math.Ceiling(cislo / rad) * rad;
            return rozsah;
        }

        // Vypočítá vhodné maximum grafu (nejbližší vyšší řád zadaného čísla).
        // Např. pro 5865 bude maximum 10000, pro 111 bude maximum 1000 atd.
        public static double RozsahGrafuVyssiRad(double cislo)
        {
            double rozsah;
            double rad;
            rad = Math.Pow(10, Math.Floor(Math.Log10(cislo)));
            rozsah = 10 * rad;
            return rozsah;
        }

        // Vypočítá vhodné maximum grafu (nejbližší vyšší řád zadaného čísla nebo upravená hodnota).
        // Pokud je mezi nejbližším vyšším řádem a zadaným číslem příliš velká vzdálenost (5 x, 2,5 x nebo 2 x), vydělí se rozsah 5, 2,5 resp. 2.
        // Pokud je mezi nejbližším vyšším řádem a zadaným číslem příliš malá vzdálenost (1,5 x), vynásobí se rozsah 2.
        // Je to kompromis mezi metodami výše, takto je přiměřeně využita výška grafu a zároveň jsou popisky na ose pěkně odstupňovány.
        public static double RozsahGrafu(double cislo)
        {
            double rozsah;
            double rad;
            rad = Math.Pow(10, Math.Floor(Math.Log10(cislo)));
            rozsah = 10 * rad;
            if (rozsah / cislo > 5)
                rozsah = rozsah / 5;
            if (rozsah / cislo > 2.5)
                rozsah = rozsah / 2.5;
            if (rozsah / cislo > 2)
                rozsah = rozsah / 2;
            if (rozsah / cislo < 1.5)
                rozsah = rozsah * 2;
            return rozsah;
        }
    }
}
