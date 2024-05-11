using MyBudgetController.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MyBudgetController.ViewModel
{
    public class AddCategoryWinVM : BaseVM
    {

        public CommandVM AddNewType { get; set; }
        public string Type { get; set; }

        public AddCategoryWinVM()
        {
            OperationManager operationManager=OperationManager.Instance;
            CategoriesManager categoriesManager = CategoriesManager.Instance;

            string type = operationManager.CurrentOperationType;

            AddNewType = new CommandVM(() =>
            {
                categoriesManager.AddNewCategory(new Category() { Name = Type, Type=type });
            });
        }

    }
}
