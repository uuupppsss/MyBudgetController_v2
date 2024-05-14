using MyBudgetController.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MyBudgetController.ViewModel
{
    public class AddNewAccountWinVM:BaseVM
    {
        public List<string> Currencies { get; set; }
        public string SelectedCurrency { get; set; }
        public string Name { get; set; }

        public CommandVM AddCommand { get;}

        AccountManager accountManager=AccountManager.Instance;
        public AddNewAccountWinVM()
        {
            Currencies = AccountManager.GetCurrencies();
            SelectedCurrency = Currencies[0];

            AddCommand = new CommandVM(()=>
            {
                if (Name != null)
                {
                    accountManager.SetNewAccount(new Account { Currency = SelectedCurrency, Name = Name });
                }
                else MessageBox.Show("Please enter a name of account");
            });
        }
    }
}
