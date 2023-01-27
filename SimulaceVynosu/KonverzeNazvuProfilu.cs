using System;
using System.Globalization;
using System.Windows.Data;

namespace SimulaceVynosu
{
    /// <summary>
    /// Převod hodnot enumu s druhy investičních profilů na názvy s diakritikou.
    /// </summary>
    class KonverzeNazvuProfilu : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((DruhProfilu)value == DruhProfilu.Dynamicky)
                return "Dynamický";
            else if ((DruhProfilu)value == DruhProfilu.Konzervativni)
                return "Konzervativní";
            else
                return "Vyvážený";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((string)value == "Dynamický")
                return DruhProfilu.Dynamicky;
            else if ((string)value == "Konzervativní")
                return DruhProfilu.Konzervativni;
            else
                return DruhProfilu.Vyvazeny;
        }
    }
}
