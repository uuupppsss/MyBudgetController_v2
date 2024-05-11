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
    }
}
