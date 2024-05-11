using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MyBudgetController.Model
{
    public class UserManager
    {
        public User CurrentUser { get; set; }

        static UserManager instance;

        public static UserManager Instance
        {
            get
            {
                if (instance == null)
                    instance = new UserManager();
                return instance;
            }
        }

        public void GetCurrentUser(string username, int id)
        {
            CurrentUser = new User()
            {
                Id = id,
                Username = username
            };
        }

        public bool SignInMethod(string _pwd, string username)
        {
            DBConnection dbConnection = DBConnection.Instance;
            if (_pwd == null || username == null)
            {
                MessageBox.Show("Password and username can't be null", "Error", MessageBoxButton.OK);

                return false;
            }
            string pwd = HashClass.HashMethod(_pwd);
            //string username=HashClass.HashMethod(_username);

            string query = $"select id from Users where username='{username}' and password='{pwd}'";
            MySqlCommand command = new MySqlCommand(query, dbConnection.GetConnection());
            object account = command.ExecuteScalar();
            command.Dispose();

            if (account != null)
            {
                //определение текущего юзера
                //почемуто тут ошибка => int user_id = Convert.ToInt32(account);
                int.TryParse(account.ToString(), out int user_id);
                GetCurrentUser(username, user_id);

                MessageBox.Show($"Hello {CurrentUser.Username}! You have successfully logged in", "Success", MessageBoxButton.OK);

                return true;

            }
            else
            {
                string query1 = $"select id from Users where username='{username}' ";
                MySqlCommand command1 = new MySqlCommand(query1, dbConnection.GetConnection());
                object account1 = command1.ExecuteScalar();
                command1.Dispose();

                if (account1 != null)
                {
                    MessageBox.Show("Wrong password", "Error", MessageBoxButton.OK);
                    return false;
                }
                else
                {
                    MessageBox.Show("Such account do not exist", "Error", MessageBoxButton.OK);
                    return false;
                }

            }

        }

        public static bool SignUpMethod(string _pwd, string username)
        {
            DBConnection dbConnection = DBConnection.Instance;

            if (_pwd == null || username == null)
            {
                MessageBox.Show("Password and username can't be null", "Error", MessageBoxButton.OK);
                return false;
            }

            string pwd = HashClass.HashMethod(_pwd);
            //string username = HashClass.HashMethod(_username);

            string query_1 = $"select id from Users where username='{username}'";

            MySqlCommand command1 = new MySqlCommand(query_1, dbConnection.GetConnection());
            object account = command1.ExecuteScalar();
            command1.Dispose();
            int result = 0;

            if (account == null)
            {
                string query_2 = $"insert into Users (username,password) values ('{username}','{pwd}') ";
                MySqlCommand command2 = new MySqlCommand(query_2, dbConnection.GetConnection());
                result = command2.ExecuteNonQuery();
                command2.Dispose();

            }
            else MessageBox.Show("Check your username", "Error", MessageBoxButton.OK);

            if (result == 1)
            {
                MessageBox.Show("You have successfully signed up", "Success", MessageBoxButton.OK);
                return true;
            }
            else
            {
                MessageBox.Show("Sign up failed", "Error", MessageBoxButton.OK);
                return false;
            }

        }

    }
}
