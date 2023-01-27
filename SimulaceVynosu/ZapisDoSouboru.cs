using Microsoft.Win32;
using System;
using System.IO;

namespace SimulaceVynosu
{
    /// <summary>
    /// Zápis výsledků a podílů profilů na disk.
    /// </summary>
    class ZapisDoSouboru
    {
        public static void ZapisVysledku(VysledkySimulace vysledkySimulace)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.FileName = "vysledky.txt";
            saveFileDialog.DefaultExt = ".txt";
            saveFileDialog.Filter = "Textové dokumenty (*.txt)|*.txt";
            bool? result = saveFileDialog.ShowDialog();
            if (result == true)
            {
                using (StreamWriter streamWriter = new StreamWriter(saveFileDialog.FileName))
                {
                    streamWriter.WriteLine("rok; střední hodnota; minimum; maximum; vložená částka; nárůst inflace; reálná střední hodnota; reálné minimum; reálné maximum; reálná vložená částka");
                    streamWriter.Write(0 + ";");
                    // zápis výchozích hodnot
                    for (int i = 0; i < 8; i++) streamWriter.Write(vysledkySimulace.JednorazovaInvestice + vysledkySimulace.PravidelnaInvestice + ";");
                    streamWriter.WriteLine(vysledkySimulace.JednorazovaInvestice + vysledkySimulace.PravidelnaInvestice);
                    for (int i = 0; i < vysledkySimulace.StredniHodnota.Length; i++)
                    {
                        streamWriter.WriteLine(
                            1 + Math.Floor((double)i / 12) + ";" +
                            Math.Round(vysledkySimulace.StredniHodnota[i], 2) + ";" +
                            Math.Round(vysledkySimulace.Minimum[i], 2) + ";" +
                            Math.Round(vysledkySimulace.Maximum[i], 2) + ";" +
                            Math.Round(vysledkySimulace.VlozenaCastka[i], 2) + ";" +
                            Math.Round(vysledkySimulace.NarustInflace[i], 2) + ";" +
                            Math.Round(vysledkySimulace.StredniHodnotaRealne[i], 2) + ";" +
                            Math.Round(vysledkySimulace.MinimumRealne[i], 2) + ";" +
                            Math.Round(vysledkySimulace.MaximumRealne[i], 2) + ";" +
                            Math.Round(vysledkySimulace.VlozenaCastkaRealne[i], 2));
                    }
                    streamWriter.WriteLine("Očekávaný výnos: " + 100 * Math.Round(vysledkySimulace.OcekavanyVynos, 3));
                    streamWriter.WriteLine("Procento nad vloženou částkou: " + 100 * Math.Round(vysledkySimulace.ProcentoNadVlozenouCastkou, 3));
                    streamWriter.WriteLine("Procento nad inflací: " + 100 * Math.Round(vysledkySimulace.ProcentoNadInflaci, 3));
                    streamWriter.WriteLine("Profil: " + vysledkySimulace.Profil);
                    streamWriter.WriteLine("Kvantil: " + vysledkySimulace.Kvantil);
                    streamWriter.Close();
                }
            }
        }

        public static void ZapisPodiluAktiv()
        {
            using (StreamWriter streamWriter = new StreamWriter("podily.txt"))
            {
                streamWriter.WriteLine("rok; akcie (dynamický); dluhopisy (dynamický); spoření (dynamický); akcie (vyvážený); dluhopisy (vyvážený); spoření (vyvážený); akcie (konzervativní); dluhopisy (konzervativní); spoření (konzervativní)");
                for (int i = 0; i < 30; i++)
                {
                    streamWriter.WriteLine(
                        i + 1 + ";" +
                        Math.Round(VstupniHodnoty.Dynamicky[i].Akcie, 4) + ";" +
                        Math.Round(VstupniHodnoty.Dynamicky[i].Dluhopisy, 4) + ";" +
                        Math.Round(VstupniHodnoty.Dynamicky[i].Sporeni, 4) + ";" +
                        Math.Round(VstupniHodnoty.Vyvazeny[i].Akcie, 4) + ";" +
                        Math.Round(VstupniHodnoty.Vyvazeny[i].Dluhopisy, 4) + ";" +
                        Math.Round(VstupniHodnoty.Vyvazeny[i].Sporeni, 4) + ";" +
                        Math.Round(VstupniHodnoty.Konzervativni[i].Akcie, 4) + ";" +
                        Math.Round(VstupniHodnoty.Konzervativni[i].Dluhopisy, 4) + ";" +
                        Math.Round(VstupniHodnoty.Konzervativni[i].Sporeni, 4));
                }
                streamWriter.Close();
            }
        }
    }
}
