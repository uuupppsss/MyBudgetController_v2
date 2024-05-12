using MyBudgetController.Model;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace MyBudgetController.Convert
{
    public class ValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string currency = "";
            AccountManager accountManager = AccountManager.Instance;
            double result = double.Parse(value.ToString());
            Account account = accountManager.SelectedAccount;
            if (account == null)
                currency = accountManager.Accounts[0].Currency;
            else
                currency = account.Currency;
            
            return $"{result.ToString("#,#", CultureInfo.InvariantCulture)} {currency}";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
