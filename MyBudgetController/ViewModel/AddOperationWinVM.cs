using MyBudgetController.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyBudgetController.View;
using System.Collections.ObjectModel;

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
            string type=OperationManager.Instance.CurrentOperationType;
            OperationManager operationManager=OperationManager.Instance;
            CategoriesManager categoriesManager=CategoriesManager.Instance;
            Date = DateTime.Now;
            categoriesManager.GetCategory(type);
            if(type=="Expences")
                OperationTypes=categoriesManager.CurrentECategoriesCollection;
            if(type== "Incomes")
                OperationTypes=categoriesManager.CurrentICategoriesCollection;

            AddNewType = new CommandVM(() =>
            {
                AddCategoryWin addTypeWin = new AddCategoryWin();
                addTypeWin.ShowDialog();
            });

            AddOperation = new CommandVM(() =>
            {
                operationManager.AddNewOperation(new Operation() { Name = Name, Sum = Sum, Date = Date, Type = SelectedType});
            });

            DeleteCategory = new CommandVM(() =>
            {
                DeleteCategoryWin win = new DeleteCategoryWin();
                win.ShowDialog();
            });
        }

    }
}
