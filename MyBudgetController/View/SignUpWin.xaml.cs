using MyBudgetController.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace MyBudgetController.View
{
    /// <summary>
    /// Логика взаимодействия для SignUpWin.xaml
    /// </summary>
    public partial class SignUpWin : Window
    {
        public SignUpWin()
        {
            InitializeComponent();
            pwd_box.PasswordChar = '*';
            repeatpwd_box.PasswordChar = '*';
            pwd_box.MaxLength = 50;
            username_box.MaxLength = 50;
            repeatpwd_box.MaxLength = 50;
            ((SignUpWinVM)DataContext).SetPassBox(pwd_box);
            ((SignUpWinVM)DataContext).SetRepeatPassBox(repeatpwd_box);
        }
    }
}
