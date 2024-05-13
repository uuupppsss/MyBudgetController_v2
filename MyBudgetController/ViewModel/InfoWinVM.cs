using MyBudgetController.Model;
using MyBudgetController.View;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MyBudgetController.ViewModel
{
    public class InfoWinVM:BaseVM
    {
        public string Name { get; set; }
        public DateTime Date{ get; set; }
        public double Sum { get; set; }
        public DateTime InsertDate { get; set; }
        public string Account { get; set; }
        public string Type { get; set; }

        public CommandVM DeleteCommand { get;}
        public CommandVM UpdateCommand { get; }

        OperationManager operationManager;
        public InfoWinVM()
        {
            operationManager = OperationManager.Instance;
            operationManager.PropertyChanged += OnOperationManagerPropertyChanged;
            DataUpdate();

            DeleteCommand = new CommandVM(()=>
            {
                operationManager.RemoveOperation(operationManager.CurrentOperation);
                
            });

            UpdateCommand = new CommandVM(() =>
            {
                AddOperationWin addOperationWin = new AddOperationWin();
                addOperationWin.Show();

            });
        }
        private void OnOperationManagerPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            MessageBox.Show("Data updating");
                DataUpdate();
        }

        private void DataUpdate()
        {
            Name = operationManager.CurrentOperation.Name;
            Date = operationManager.CurrentOperation.Date;
            Sum = operationManager.CurrentOperation.Sum;
            InsertDate = operationManager.CurrentOperation.InputDate;
            Account = operationManager.CurrentOperation.Account.ToString();
            Type = operationManager.CurrentOperation.Type.Name.ToString();
        }

    }
}
