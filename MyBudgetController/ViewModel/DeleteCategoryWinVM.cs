using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using MyBudgetController.Model;

namespace MyBudgetController.ViewModel
{
    public class DeleteCategoryWinVM:BaseVM
    {
        public ObservableCollection<Category> Categories { get; set; }
        public CommandVM DeleteCommand { get; }
        public Category SelectedCategory { get; set; }
        public DeleteCategoryWinVM()
        {
            OperationManager operationManager = OperationManager.Instance;
            CategoriesManager categoriesManager = CategoriesManager.Instance;
            string type = operationManager.CurrentOperationType;
            categoriesManager.GetCategory(type);

            if (type == "Expences")
                Categories = categoriesManager.CurrentECategoriesCollection;
            if (type == "Incomes")
                Categories = categoriesManager.CurrentICategoriesCollection;

            DeleteCommand = new CommandVM(() =>
            {
                categoriesManager.RemoveCategory(SelectedCategory);
                Window win = Application.Current.Windows.OfType<Window>().SingleOrDefault(x => x.DataContext == this);
                win?.Close();
            });
        }
    }
}
