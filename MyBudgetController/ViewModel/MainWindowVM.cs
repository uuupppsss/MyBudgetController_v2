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

        private int [] filter_date;

        public int [] Filter_Date
        {
            get => filter_date;
            set
            {
                filter_date = value;
                Signal();
            }
        }

        OperationManager operationManager = OperationManager.Instance;

        public MainWindowVM()
        {

            Months = FilterManager.GetMonths();
            Years = FilterManager.GetYears();
            SelectedYear = DateTime.Now.Year;
            SelectedMonth = Months[DateTime.Now.Month];
 
            FilteredCollection_E.CollectionChanged+= ExpencesCollection_CollectionChanged;
            FilteredCollection_I.CollectionChanged += IncomesCollection_CollectionChanged;

            AddNewExpence = new CommandVM(() =>
            {
                operationManager.CurrentOperationType = "Expences";
                MessageBox.Show($"{operationManager.CurrentOperationType}", "You are going to add expence", MessageBoxButton.OK);
                AddOperationWin addIWin = new AddOperationWin();
                addIWin.ShowDialog();
            });

            AddNewIncome = new CommandVM(() =>
            {
                operationManager.CurrentOperationType = "Incomes";
                MessageBox.Show($"{operationManager.CurrentOperationType}", "You are going to add an income", MessageBoxButton.OK);
                AddOperationWin addIWin = new AddOperationWin();
                addIWin.ShowDialog();
            });

            RemoveCommand = new CommandVM<Operation>(s =>
            {
                if (s != null)
                {
                    operationManager.RemoveOperation(s);
                }
            });
        }

        private void ExpencesCollection_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            ReportItems_E = FilterManager.GetReportItems(FilteredCollection_E);
            Balance = ReportItems_I[0].Value - ReportItems_E[0].Value;
        }
        private void IncomesCollection_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            ReportItems_I = FilterManager.GetReportItems(FilteredCollection_I);
            Balance = ReportItems_I[0].Value - ReportItems_E[0].Value;
        }

        private void FilterDateChanged()
        {
            Filter_Date = new[] { SelectedYear, Array.IndexOf(Months.ToArray(), SelectedMonth) };
            operationManager.GetOperations(Filter_Date, "Expences");
            operationManager.GetOperations(Filter_Date, "Incomes");

            FilteredCollection_E = operationManager.CurrentExpencesCollection;
            FilteredCollection_I = operationManager.CurrentIncomesCollection;

            ReportItems_E = FilterManager.GetReportItems(FilteredCollection_E);
            ReportItems_I = FilterManager.GetReportItems(FilteredCollection_I);

            Balance = ReportItems_I[0].Value - ReportItems_E[0].Value;
        }
    }
}
