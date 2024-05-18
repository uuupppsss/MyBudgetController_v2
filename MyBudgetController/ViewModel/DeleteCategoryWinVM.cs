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

            if (type == "Expences")
                Categories = new ObservableCollection<Category>( categoriesManager.CurrentECategoriesCollection);
            if (type == "Incomes")
                Categories = new ObservableCollection<Category>( categoriesManager.CurrentICategoriesCollection);

            Categories.RemoveAt(0);

            DeleteCommand = new CommandVM(() =>
            {
                var result = MessageBox.Show( "Are you shure you want to delete this category? All transactions in this category wil be updated with the default category  ", "Delete", MessageBoxButton.YesNo);
                if (result == MessageBoxResult.Yes)
                {
                    categoriesManager.RemoveCategory(SelectedCategory);
                    Window win = Application.Current.Windows.OfType<Window>().SingleOrDefault(x => x.DataContext == this);
                    win?.Close();
                }

            });
        }
    }
}
