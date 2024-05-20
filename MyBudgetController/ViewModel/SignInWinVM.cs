using MyBudgetController.Model;
using MyBudgetController.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;

namespace MyBudgetController.ViewModel
{
    public class SignInWinVM : Base
    {
        UserManager umanager;

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
                }
            }
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

        public CommandVM OpenSignUp { get; }
        public CommandVM SignIn { get; }

        public SignInWinVM()
        {
            umanager = UserManager.Instance;

            OpenSignUp = new CommandVM(() =>
            {
                SignUpWin win = new SignUpWin();
                win.Show();
                Window thiswin = Application.Current.Windows.OfType<Window>().SingleOrDefault(x => x.DataContext == this);
                thiswin?.Close();

            });

            SignIn = new CommandVM(() =>
            {
                Password = pwd_box.Password;
                if (umanager.SignInMethod(Password, Username))
                {
                    MainWindow win = new MainWindow();
                    win.Show();
                    Window thiswin = Application.Current.Windows.OfType<Window>().SingleOrDefault(x => x.DataContext == this);
                    thiswin?.Close();
                }

            });
        }


        PasswordBox pwd_box;

        internal void SetPassBox(PasswordBox pwd_box)
        {
            this.pwd_box = pwd_box;
        }
    }
}
