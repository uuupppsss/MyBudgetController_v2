using MySqlConnector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MyBudgetController.Model
{
    public class FilterManager
    {
        public List<int> Years { get; set; }
        public List<string> Months { get; set; }

        static FilterManager instance;
        public static FilterManager Instance
        {
            get
            {
                if (instance == null)
                    instance = new FilterManager();
                return instance;
            }
        }

        public static double GetBalance()
        {
            UserManager userManager = UserManager.Instance;
            DBConnection dBConnection = DBConnection.Instance;
            AccountManager accountManager = AccountManager.Instance;

            double ExpencesSum, IncomesSum;
            string q1 = $"select sum(sum) from Operations where account_id={accountManager.SelectedAccount.Id} and type_id in(select id from Categories where Type='Expences' and user_id={userManager.CurrentUser.Id})";
            MySqlCommand command1 = new MySqlCommand(q1, dBConnection.GetConnection());
            object res1 = command1.ExecuteScalar();
            if (res1 != DBNull.Value)
            {
                ExpencesSum = double.Parse(res1.ToString());
            }
            else ExpencesSum = 0;
            command1.Dispose();

            string q2 = $"select sum(sum) from Operations where account_id={accountManager.SelectedAccount.Id} and type_id in(select id from Categories where Type='Incomes' and user_id={userManager.CurrentUser.Id})";
            MySqlCommand command2 = new MySqlCommand(q2, dBConnection.GetConnection());
            object res2 = command2.ExecuteScalar();
            if (res2 != DBNull.Value)
            {
                IncomesSum = double.Parse(res2.ToString());
            }
            else  IncomesSum = 0;
            command2.Dispose();

            return IncomesSum - ExpencesSum;

        }

        public static List<int> GetYears()
        {
            UserManager userManager = UserManager.Instance;
            DBConnection dBConnection = DBConnection.Instance;

            string query = $"select distinct year(Date) as y from Operations where user_id={userManager.CurrentUser.Id} ";

            MySqlCommand command = new MySqlCommand(query, dBConnection.GetConnection());
            MySqlDataReader reader = command.ExecuteReader();

            List<int> Years = new List<int>();

            while (reader.Read())
            {
                Years.Add(reader.GetInt32(0));
            }

            reader.Close();
            command.Dispose();


            if (!Years.Contains(DateTime.Now.Year))
            {
                Years.Add(DateTime.Now.Year);
            }

            Years.Sort((d1, d2) => d1.CompareTo(d2));
            return Years;
        }

        public static List<string> GetMonths()
        {
            List<string> months = new List<string> { "Все", "Январь", "Февраль", "Март", "Апрель", "Май", "Июнь", "Июль", "Август", "Сентябрь", "Октябрь", "Ноябрь", "Декабрь" };
            return months;
        }

        public static ObservableCollection<ReportItem> GetReportItems(ObservableCollection<Operation> collection)
        {
            ObservableCollection<ReportItem> ReportItems = new ObservableCollection<ReportItem>();
            if (collection != null)
            {
                //добавление общей суммы
                ReportItems.Add(new ReportItem { TypeName = "Всего", Value = collection.Sum(o => o.Sum), Percent = 0 });

                //нахождение суммы по каждому типу

                var operationsByType = collection.GroupBy(o => o.Type.Name).Select(g => new
                {
                    TypeName = g.Key,
                    TotalSum = g.Sum(o => o.Sum)
                });

                foreach (var item in operationsByType)
                {
                    ReportItems.Add(new ReportItem
                    {
                        TypeName = item.TypeName,
                        Value = item.TotalSum,
                        Percent = item.TotalSum / ReportItems[0].Value
                    });
                }

                ReportItems.OrderByDescending(o => o.Percent);
            }
            return ReportItems;
        }


    }
}
