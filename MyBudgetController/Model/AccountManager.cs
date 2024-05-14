using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MyBudgetController.Model
{
    public class AccountManager
    {
        static AccountManager instance;

        public static AccountManager Instance
        {
            get
            {
                if (instance == null)
                    instance = new AccountManager();
                return instance;
            }
        }
        
        public static List<string> GetCurrencies()
        {
            return new List<string> { "₽", "₩", "₴", "$", "€", "¥", "£", "¥", "₸" };
        }

        public Account SelectedAccount { get; set; }
        public ObservableCollection<Account> Accounts { get; set; }
        

        UserManager userManager=UserManager.Instance;
        DBConnection dbConnection=DBConnection.Instance;
        OperationManager operationManager=OperationManager.Instance;    

        public void GetAccounts()
        {
            Accounts=new ObservableCollection<Account>();
            string q = $"select id, Name, Currency from Accounts where user_id={userManager.CurrentUser.Id} order by id";
            MySqlCommand cmd = new MySqlCommand(q,dbConnection.GetConnection());
            MySqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                Account account = new Account();
                account.Id = reader.GetInt32(0);
                account.Name = reader.GetString(1);
                account.Currency = reader.GetString(2);

                Accounts.Add(account);
            }
            reader.Close();
            cmd.Dispose();
        }

        public void RemoveAccount()
        {
            if (Accounts.Count==1)
            {
                MessageBox.Show("You can't delete your only account");
                return;
            }
            int id = SelectedAccount.Id;
            var dialogresult = MessageBox.Show("Delete this account? All transactions on this account will also be deleted", "Delete", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (dialogresult == MessageBoxResult.Yes)
            {
                Account account=SelectedAccount;
                SelectedAccount = Accounts[0];
                Accounts.Remove(account);
  
                string q0 = $"delete from Operations where account_id={id}";
                MySqlCommand cmd0 = new MySqlCommand(q0, dbConnection.GetConnection());
                cmd0.ExecuteNonQuery();
                cmd0.Dispose();
                operationManager.GetOperations("Expences");
                operationManager.GetOperations("Incomes");

                string q=$"delete from Accounts where id={id}";
                MySqlCommand cmd = new MySqlCommand(q, dbConnection.GetConnection());
                int result = cmd.ExecuteNonQuery();
                cmd.Dispose();
                if (result != 0)
                {

                    MessageBox.Show("Success");
                }

            }
        }

        public void SetNewAccount(Account account)
        {
            string q = $"insert into Accounts(Name,user_id,Currency) values('{account.Name}',{userManager.CurrentUser.Id},'{account.Currency}')";
            MySqlCommand cmd = new MySqlCommand(q, dbConnection.GetConnection());
            int res = cmd.ExecuteNonQuery();
            cmd.Dispose();
            if (res != 0)
            {
                string query = $"select id from Accounts where id=(select max(id) from Accounts where user_id={userManager.CurrentUser.Id})";
                MySqlCommand command = new MySqlCommand(query, dbConnection.GetConnection());
                object result = command.ExecuteScalar();
                command.Dispose();
                account.Id = int.Parse(result.ToString());
                Accounts.Add(account);
                MessageBox.Show("Success");
            }

        }
    }
}
