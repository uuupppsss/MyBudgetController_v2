using MyBudgetController.Model;
using MyBudgetController.View;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MyBudgetController.ViewModel
{
    public class MainWindowVM : BaseVM
    {
        private List<string> _months;
        public List<string> Months
        {
            get => _months;
            set
            {
                if (_months != value)
                {
                    _months = value;
                    Signal();
                }
            }
        }

        private List<int> _years;
        public List<int> Years
        {
            get => _years;
            set
            {
                if (_years != value)
                {
                    _years = value;
                    Signal();
                }
            }
        }

        private int _selectedYear;
        public int SelectedYear
        {
            get => _selectedYear;
            set
            {
                if (_selectedYear != value)
                {
                    _selectedYear = value;
                    Signal();
                    FilterDateChanged();
                }
            }
        }

        private string _selectedMonth;
        public string SelectedMonth
        {
            get => _selectedMonth;
            set
            {
                if (_selectedMonth != value)
                {
                    _selectedMonth = value;
                    Signal();
                    FilterDateChanged();
                }
            }
        }

        public CommandVM AddNewExpence { get; }
        public CommandVM AddNewIncome { get; }
        public CommandVM FilterCommand { get; }
        public CommandVM<Operation> RemoveCommand { get; }
        public CommandVM LogOutCommand { get; } 
        public CommandVM AddNewAccount { get; }
        public CommandVM RemoveAccountCommand { get; }

        public Operation SelectedVal { get; set; }

        private ObservableCollection<Operation> _filteredCollectionE;
        public ObservableCollection<Operation> FilteredCollection_E
        {
            get => _filteredCollectionE;
            set
            {
                _filteredCollectionE = value;
                Signal();
            }
        }
        private ObservableCollection<Operation> _filteredCollectionI;
        public ObservableCollection<Operation> FilteredCollection_I
        {
            get => _filteredCollectionI;
            set
            {
                _filteredCollectionI = value;
                Signal();
            }
        }

        private ObservableCollection<ReportItem> _reportitemse;
        public ObservableCollection<ReportItem> ReportItems_E
        {
            get => _reportitemse;
            set
            {
                _reportitemse = value;
                Signal();
            }
        }

        private ObservableCollection<ReportItem> _reportitemsi;
        public ObservableCollection<ReportItem> ReportItems_I
        {
            get => _reportitemsi;
            set
            {
                _reportitemsi = value;
                Signal();
            }
        }
        private ObservableCollection<Account> accounts;

        public ObservableCollection<Account> Accounts
        {
            get => accounts;
            set
            {
                accounts= value;
                Signal();
            }
        }

        private Account selectedaccount;

        public Account SelectedAccount
        {
            get => selectedaccount;
            set
            {
                selectedaccount = value;
                Signal();
                AccountChanged();
            }

        }


        private double balance;
        public double Balance
        {
            get => balance;
            set
            {
                balance = value;
                Signal();
            }
        }


        OperationManager operationManager = OperationManager.Instance;
        UserManager userManager = UserManager.Instance;
        AccountManager accountManager = AccountManager.Instance;

        public MainWindowVM()
        {

            accountManager.GetAccounts();
            Accounts=accountManager.Accounts;
            SelectedAccount = Accounts[0];
            accountManager.SelectedAccount = SelectedAccount;

            Months = FilterManager.GetMonths();
            Years = FilterManager.GetYears();

            SelectedYear = DateTime.Now.Year;
            SelectedMonth = Months[DateTime.Now.Month];

            AddNewExpence = new CommandVM(() =>
            {
                operationManager.CurrentOperationType = "Expences";
                AddOperationWin addIWin = new AddOperationWin();
                addIWin.ShowDialog();
                Filter();
            });

            AddNewIncome = new CommandVM(() =>
            {
                operationManager.CurrentOperationType = "Incomes";
                AddOperationWin addIWin = new AddOperationWin();
                addIWin.ShowDialog();
                Filter();
            });

            RemoveCommand = new CommandVM<Operation>(s =>
            {
                if (s != null)
                {
                    operationManager.RemoveOperation(s);
                }
            });

            LogOutCommand = new CommandVM(() =>
            {
            var dialogresult = MessageBox.Show("Are you shure you want to log out?", "Log out", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (dialogresult == MessageBoxResult.Yes)
                { 
                    userManager.CurrentUser = null;
                    SignInWin signInWin = new SignInWin();
                    signInWin.Show();
                    Window win = Application.Current.Windows.OfType<Window>().SingleOrDefault(x => x.DataContext == this);
                    win?.Close();

                }
            });

            AddNewAccount = new CommandVM(() =>
            {
                AddNewAccountWin win = new AddNewAccountWin();
                win.ShowDialog();
            });

            RemoveAccountCommand = new CommandVM(() =>
            {
                accountManager.RemoveAccount();
                SelectedAccount = Accounts[0];
                Filter();
            });
        }

        private void ExpencesCollection_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            MessageBox.Show("Expences collection changed");
            ReportItems_E = FilterManager.GetReportItems(FilteredCollection_E);
            Balance = FilterManager.GetBalance();
        }
        private void IncomesCollection_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            MessageBox.Show("Incomes collection changed");
            ReportItems_I = FilterManager.GetReportItems(FilteredCollection_I);
            Balance = FilterManager.GetBalance();
        }

        private void FilterDateChanged()
        {
            if (SelectedMonth == null || SelectedYear == 0)
                return;

            operationManager.selected_month = Array.IndexOf(Months.ToArray(), SelectedMonth);
            operationManager.selected_year = SelectedYear;
            Filter();
        }

        private void Filter()
        {
            operationManager.GetOperations("Expences");
            operationManager.GetOperations("Incomes");

            if (FilteredCollection_E != null)
            {
                FilteredCollection_E.Clear();
                foreach (var c in operationManager.CurrentExpencesCollection)
                    FilteredCollection_E.Add(c);

                FilteredCollection_I.Clear();
                foreach (var c in operationManager.CurrentIncomesCollection)
                    FilteredCollection_I.Add(c);
            }
            else
            {
                FilteredCollection_E = operationManager.CurrentExpencesCollection;
                FilteredCollection_I = operationManager.CurrentIncomesCollection;


                FilteredCollection_E.CollectionChanged += ExpencesCollection_CollectionChanged;
                FilteredCollection_I.CollectionChanged += IncomesCollection_CollectionChanged;
            }
            ReportItems_E = FilterManager.GetReportItems(FilteredCollection_E);
            ReportItems_I = FilterManager.GetReportItems(FilteredCollection_I);

            Balance = FilterManager.GetBalance();
        }

        private void AccountChanged()
        {
            if (SelectedYear==0||SelectedMonth==null)
                return;
            accountManager.SelectedAccount = SelectedAccount;

            Filter();
        }
    }
}
