using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Media;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace LawlerBallisticsDesk.Views.Cartridges
{
    public class Converters
    {

    }

    /// <summary>
    /// Used in recipe data grid to highlight outlier cells
    /// </summary>
    public class BBTOclrConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targettype, object Parameter, CultureInfo culture)
        {
            double lSig;
            double lAvg;
            double lval;
            double lmulti;
            Brush lRTN = Brushes.Transparent;

            try
            {
                lval = (double)values[0];
                lAvg = (double)values[1];
                lSig = (double)values[2];
                lmulti = (double)values[3];
                if (lval > (lAvg + lSig * lmulti))
                {
                    lRTN = Brushes.Orange;
                }
                else if (lval < (lAvg - lSig * lmulti))
                {
                    lRTN = Brushes.Orange;
                }
            }
            catch
            {
                lRTN = Brushes.Transparent;
            }
            return lRTN;
        }
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    /// <summary>
    /// Used in recipe data grid to highlight outlier cells
    /// </summary>
    public class BLclrConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targettype, object Parameter, CultureInfo culture)
        {
            double lSig;
            double lAvg;
            double lval;
            double lmulti;
            Brush lRTN = Brushes.Transparent;

            try
            {
                lval = (double)values[0];
                lAvg = (double)values[1];
                lSig = (double)values[2];
                lmulti = (double)values[3];
                if (lval > (lAvg + lSig * lmulti))
                {
                    lRTN = Brushes.Orange;
                }
                else if (lval < (lAvg - lSig * lmulti))
                {
                    lRTN = Brushes.Orange;
                }
            }
            catch
            {
                lRTN = Brushes.Transparent;
            }
            return lRTN;
        }
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    /// <summary>
    /// Used in recipe data grid to highlight outlier cells
    /// </summary>
    public class BWclrConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targettype, object Parameter, CultureInfo culture)
        {
            double lSig;
            double lAvg;
            double lval;
            double lmulti;
            Brush lRTN = Brushes.Transparent;

            try
            {
                lval = (double)values[0];
                lAvg = (double)values[1];
                lSig = (double)values[2];
                lmulti = (double)values[3];
                if (lval > (lAvg + lSig * lmulti))
                {
                    lRTN = Brushes.Orange;
                }
                else if (lval < (lAvg - lSig * lmulti))
                {
                    lRTN = Brushes.Orange;
                }
            }
            catch
            {
                lRTN = Brushes.Transparent;
            }
            return lRTN;
        }
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    /// <summary>
    /// Used in recipe data grid to highlight outlier cells
    /// </summary>
    public class GDVrowClrConverter : IMultiValueConverter
    {
        public object Convert(object[] value, Type targettype, object Parameter, CultureInfo culture)
        {
            double lval, lnval;
            Brush lRTN = Brushes.Transparent;

            try
            {
                lval = (double)value[0];
                lnval = (int)value[1];
                if (lval < 0)
                {
                    lRTN = Brushes.Red;
                }
                else
                {
                    if((lnval % 2) == 0)
                    {
                        lRTN = Brushes.Cyan;
                    }
                }
            }
            catch
            {
                lRTN = Brushes.Transparent;
            }
            return lRTN;
        }
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
