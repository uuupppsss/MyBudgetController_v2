using MyBudgetController.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyBudgetController.View;
using System.Collections.ObjectModel;
using System.Windows;

namespace MyBudgetController.ViewModel
{
    public class AddOperationWinVM : BaseVM
    {
        public string Name { get; set; }
        public double Sum { get; set; }
        public DateTime Date { get; set; }
        private Category selectedtype;
        public Category SelectedType 
        {
            get => selectedtype;
            set
            {
                selectedtype = value;
                Signal();
            }
        }
        public ObservableCollection<Category> OperationTypes { get; set; }
        public CommandVM AddNewType { get; }
        public CommandVM AddOperation { get; }

        public CommandVM DeleteCategory { get; }

        OperationManager operationManager;
        CategoriesManager categoriesManager;
        AccountManager accountManager;
        string type;
        public AddOperationWinVM()
        {
             type = OperationManager.Instance.CurrentOperationType;
             operationManager = OperationManager.Instance;
             categoriesManager = CategoriesManager.Instance;
             accountManager = AccountManager.Instance;
            categoriesManager.GetCategory(type);

            if (type == "Expences")
                OperationTypes = categoriesManager.CurrentECategoriesCollection;

            if (type == "Incomes")
                OperationTypes = categoriesManager.CurrentICategoriesCollection;
            OperationTypes.CollectionChanged += ChangeDefaultCategory;

            if (operationManager.CurrentOperation == null)
            {
                //добавление новой операции
                SelectedType = OperationTypes[0];
                Date = DateTime.Now;

                AddOperation = new CommandVM(() =>
                {
                    Operation operation = new Operation() { Name = Name, Sum = Sum, Date = Date, Type = SelectedType, Account = accountManager.SelectedAccount, InputDate = DateTime.Now };
                    operationManager.AddNewOperation(operation);
                });
            }
            else
            {
                //перезаписываем выбранную операцию
                Name=operationManager.CurrentOperation.Name;
                Category category = OperationTypes.FirstOrDefault(c => c.Id == operationManager.CurrentOperation.Type.Id);
                SelectedType = category;
                Sum = operationManager.CurrentOperation.Sum;
                Date = operationManager.CurrentOperation.Date;
                AddOperation = new CommandVM(() =>
                {
                    Operation operation = new Operation() { ID = operationManager.CurrentOperation.ID, Name = Name, Sum = Sum, Date = Date, Type = SelectedType, Account = accountManager.SelectedAccount, InputDate= operationManager.CurrentOperation.InputDate };
                    operationManager.UpdateOperation(operation);
                    
                    Window win = Application.Current.Windows.OfType<Window>().SingleOrDefault(x => x.DataContext == this);
                    win?.Close();

                });

            }


            AddNewType = new CommandVM(() =>
            {
                AddCategoryWin addTypeWin = new AddCategoryWin();
                addTypeWin.ShowDialog();
            });

            

            DeleteCategory = new CommandVM(() =>
            {
                DeleteCategoryWin win = new DeleteCategoryWin();
                win.ShowDialog();
            });

        }

        private void ChangeDefaultCategory(object sender, EventArgs e)
        {
            SelectedType=OperationTypes.Last();
        }
    }
}
