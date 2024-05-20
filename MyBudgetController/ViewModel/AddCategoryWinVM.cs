using MyBudgetController.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Linq;

namespace MyBudgetController.ViewModel
{
    public class AddCategoryWinVM : Base
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
                if (Type != ""&&Type!=null)
                {
                    categoriesManager.AddNewCategory(new Category() { Name = Type, Type = type });

                }
                else MessageBox.Show("Please enter a name of category");
            });
        }

    }
}
