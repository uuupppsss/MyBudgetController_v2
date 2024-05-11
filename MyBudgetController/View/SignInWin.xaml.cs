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
    /// Логика взаимодействия для SignInWin.xaml
    /// </summary>
    public partial class SignInWin : Window
    {
        public SignInWin()
        {
            InitializeComponent();
            pwd_box.PasswordChar = '*';
            pwd_box.MaxLength = 50;
            username_box.MaxLength = 50;
            ((SignInVM)DataContext).SetPassBox(pwd_box);
        }
    }
}
