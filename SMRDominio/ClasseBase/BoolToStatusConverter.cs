using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMRDominio.ClasseBase
{
    public class BoolToStatusConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool ativo && ativo)
                return "ATIVO";

            return "INATIVO";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string texto && texto.ToUpper() == "ATIVO")
                return true;

            return false;
        }
    }
}
