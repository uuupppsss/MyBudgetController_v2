using MyBudgetController.Model;
using MyBudgetController.View;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Windows;

namespace MyBudgetController.ViewModel
{
    public class MainWindowVM : Base
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
        public CommandVM<Operation> InformCommand { get; }
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


        OperationManager operationManager;
        UserManager userManager;
        AccountManager accountManager;
        CategoriesManager categoriesManager;
        FilterManager filterManager;

        public MainWindowVM()
        {
            operationManager = OperationManager.Instance;
            userManager = UserManager.Instance;
            accountManager = AccountManager.Instance;
            categoriesManager = CategoriesManager.Instance;
            filterManager = FilterManager.Instance;

            categoriesManager.CategoryRemoved += CategoryRemoved;
            filterManager.PropertyChanged += BalanceUpdate;


            accountManager.GetAccounts();
            Accounts=accountManager.Accounts;
            SelectedAccount = Accounts[0];
            accountManager.SelectedAccount = SelectedAccount;

            Accounts.CollectionChanged += SelectedAccountChanged;

            Months = FilterManager.GetMonths();
            Years = FilterManager.GetYears();

            SelectedYear = DateTime.Now.Year;
            SelectedMonth = Months[DateTime.Now.Month];

            AddNewExpence = new CommandVM(() =>
            {
                operationManager.CurrentOperationType = "Expences";
                OpenAddWin();
            });

            AddNewIncome = new CommandVM(() =>
            {
                operationManager.CurrentOperationType = "Incomes";
                OpenAddWin();
            });

            InformCommand = new CommandVM<Operation>(s =>
            {
                if (s != null)
                {
                    operationManager.CurrentOperation = s;
                    operationManager.CurrentOperationType = operationManager.CurrentOperation.Type.Type;
                    InfoWin win = new InfoWin();
                    win.Closed += UnsetCurrentOperation;
                    win.Show();
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

                DataUpdate();
            });
        }

        private void SelectedAccountChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            SelectedAccount = Accounts.Last();
        }

        private void UnsetCurrentOperation(object sender, EventArgs e)
        {
            operationManager.CurrentOperation = null;
        }

        private void CategoryRemoved()
        {
            DataUpdate();
        }

        private void ExpencesCollection_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            ReportItems_E = FilterManager.GetReportItems(FilteredCollection_E);
        }

        private void IncomesCollection_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            ReportItems_I = FilterManager.GetReportItems(FilteredCollection_I);
        }

        private void FilterDateChanged()
        {
            if (SelectedMonth == null || SelectedYear == 0)
                return;

            operationManager.selected_month = Array.IndexOf(Months.ToArray(), SelectedMonth);
            operationManager.selected_year=SelectedYear;

            DataUpdate();

        }

        private void AccountChanged()
        {
            if (SelectedYear==0||SelectedMonth==null|| SelectedAccount==null)
                return;
            accountManager.SelectedAccount = SelectedAccount;
            DataUpdate();
        }

        private void DataUpdate()
        {
            operationManager.GetOperations("Expences");
            operationManager.GetOperations("Incomes");

            FilteredCollection_E = operationManager.CurrentExpencesCollection; 
            FilteredCollection_I = operationManager.CurrentIncomesCollection;

            FilteredCollection_E.CollectionChanged += ExpencesCollection_CollectionChanged;
            FilteredCollection_I.CollectionChanged += IncomesCollection_CollectionChanged;

            ReportItems_E = FilterManager.GetReportItems(FilteredCollection_E);
            ReportItems_I = FilterManager.GetReportItems(FilteredCollection_I);

            Years = FilterManager.GetYears();
            filterManager.GetBalance();
        }

        private void OpenAddWin()
        {
            AddOperationWin addIWin = new AddOperationWin();
            addIWin.ShowDialog();
        }

        private void BalanceUpdate(object sender, EventArgs e)
        {
            Balance = filterManager.Balance;
        }

    }
}
