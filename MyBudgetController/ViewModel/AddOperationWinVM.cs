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
        public Category SelectedType { get; set; }
        public ObservableCollection<Category> OperationTypes { get; set; }
        public CommandVM AddNewType { get; }
        public CommandVM AddOperation { get; }

        public CommandVM DeleteCategory { get; }


        public AddOperationWinVM()
        {
            string type = OperationManager.Instance.CurrentOperationType;
            OperationManager operationManager = OperationManager.Instance;
            CategoriesManager categoriesManager = CategoriesManager.Instance;
            AccountManager accountManager = AccountManager.Instance;
            categoriesManager.GetCategory(type);
            if (type == "Expences")
                OperationTypes = categoriesManager.CurrentECategoriesCollection;

            if (type == "Incomes")
                OperationTypes = categoriesManager.CurrentICategoriesCollection;

            if (operationManager.CurrentOperation == null)
            {
                SelectedType = OperationTypes[0];
                Date = DateTime.Now;

                AddOperation = new CommandVM(() =>
                {
                    operationManager.AddNewOperation(new Operation() { Name = Name, Sum = Sum, Date = Date, Type = SelectedType, Account = accountManager.SelectedAccount });
                });
            }
            else
            {
                Name=operationManager.CurrentOperation.Name;
                Category category = OperationTypes.FirstOrDefault(c => c.Id == operationManager.CurrentOperation.Type.Id);
                SelectedType = category;
                Sum = operationManager.CurrentOperation.Sum;
                Date = operationManager.CurrentOperation.Date;
                AddOperation = new CommandVM(() =>
                {
                    Operation operation = new Operation() { ID = operationManager.CurrentOperation.ID, Name = Name, Sum = Sum, Date = Date, Type = SelectedType, Account = accountManager.SelectedAccount };
                    operationManager.UpdateOperation(operation);
                    if (type == "Expences")
                        operationManager.CurrentExpencesCollection.Remove(operationManager.CurrentOperation);

                    if (type == "Incomes")
                        operationManager.CurrentIncomesCollection.Remove(operationManager.CurrentOperation);

                    operationManager.CurrentOperation = operation;
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

    }
}
