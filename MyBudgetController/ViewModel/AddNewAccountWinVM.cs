using MyBudgetController.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

            AddCommand = new CommandVM(()=>
            {
                accountManager.SetNewAccount(new Account { Currency = SelectedCurrency, Name = Name });
            });
        }
    }
}
