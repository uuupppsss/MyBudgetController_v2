using MyBudgetController.Model;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;
using MyBudgetController.View;

namespace MyBudgetController.ViewModel
{
    public class SignUpWinVM : BaseVM
    {
        public CommandVM SignUp { get; }

        public bool IsUsernameMessageVisible { get; set; }
        public string Message_username => IsUsernameMessageVisible ? "This username is already exist" : "";

        private string _username;
        public string Username
        {
            get => _username;
            set
            {
                if (_username != value)
                {
                    _username = value;
                    Signal(nameof(Username));
                    IfUserExist();
                }
            }
        }

        PasswordBox pwd_box;
        PasswordBox repeatpwd_box;
        internal void SetPassBox(PasswordBox pwd_box)
        {
            this.pwd_box = pwd_box;
        }

        internal void SetRepeatPassBox(PasswordBox pwd_box)
        {
            this.repeatpwd_box = pwd_box;
        }

        private string _password;
        public string Password
        {
            get => _password;
            set
            {
                if (_password != value)
                {
                    _password = value;
                    Signal(nameof(Password));
              
                }
            }
        }

        private string _repeatpassword;
        public string RepeatPassword
        {
            get => _repeatpassword;
            set
            {
                if (_repeatpassword != value)
                {
                    _repeatpassword = value;
                    Signal(nameof(RepeatPassword));

                }
            }
        }

        public SignUpWinVM()
        {

            SignUp = new CommandVM(() =>
            {
                Password = pwd_box.Password;
                RepeatPassword = repeatpwd_box.Password;

                if (Password != RepeatPassword)
                    MessageBox.Show("Password mismatching", "Error", MessageBoxButton.OK);
                else
                {
                    if (UserManager.SignUpMethod(Password, Username))
                    {
                        MainWindow mainwin = new MainWindow();
                        mainwin.Show();
                        Window win = Application.Current.Windows.OfType<Window>().SingleOrDefault(x => x.DataContext == this);
                        win?.Close();
                    }
                }
            });
        }


        public void IfUserExist()
        {
            DBConnection dbConnection = DBConnection.Instance;

            string query = $"select id from Users where username='{Username}'";
            MySqlCommand command = new MySqlCommand(query, dbConnection.GetConnection());
            object account = command.ExecuteScalar();
            if (account == null)
                IsUsernameMessageVisible = false;

            else
                IsUsernameMessageVisible = true;
            Signal(nameof(IsUsernameMessageVisible));
            Signal(nameof(Message_username));

        }

    }
}
