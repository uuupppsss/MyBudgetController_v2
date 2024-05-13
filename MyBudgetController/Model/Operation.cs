using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using MySqlConnector;

namespace MyBudgetController.Model
{
    public class Operation
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public double Sum { get; set; }
        public DateTime Date { get; set; }
        public Category Type { get; set; }
        public Account Account { get; set; }
        public DateTime InputDate { get; set; }


        public Category SetType(int id, string type)
        {
            DBConnection dBConnection=new DBConnection();
            string query = $"select Name from Categories where id={id}";
            MySqlCommand command= new MySqlCommand(query, dBConnection.GetConnection());
            object result=command.ExecuteScalar();
            command.Dispose();
            Category category = new Category
            {
                Name = result.ToString(),
                Type = type,
                Id = id
            };
            dBConnection.CloseConnection();
            return category;
        }

        public Account SetAccount(int id)
        {
            Account account = new Account();
            DBConnection dBConnection = new DBConnection();
            string query = $"select Name, Currency from Accounts where id={id}";
            MySqlCommand command = new MySqlCommand(query, dBConnection.GetConnection());
            MySqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                account.Name = reader.GetString(0);
                account.Currency = reader.GetString(1);
            }

            dBConnection.CloseConnection();
            return account;
        }
    }
}
