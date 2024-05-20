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
    public class InfoWinVM : Base
    {
        private string name;
        public string Name
        {
            get => name;
            set
            {
                name = value;
                Signal();
            }
        }
        private DateTime date;
        public DateTime Date
        { 
            get => date;
            set
            {
                date = value;
                Signal();
            }
        }
        private double sum;
        public double Sum
        {
            get => sum;
            set
            {
                sum= value;
                Signal() ;
            }
        }
        public DateTime InsertDate { get; set; }
        public string Account { get; set; }
        private string type;
        public string Type
        {
            get=>type;
            set
            {
                type = value;
                Signal();
            }
        }

        public CommandVM DeleteCommand { get;}
        public CommandVM UpdateCommand { get; }

        OperationManager operationManager;
        public InfoWinVM()
        {
            operationManager = OperationManager.Instance;
            GetData();

            DeleteCommand = new CommandVM(()=>
            {
                operationManager.RemoveOperation(operationManager.CurrentOperation);
                InfoWin thiswin = (InfoWin)Application.Current.Windows.OfType<Window>().SingleOrDefault(x => x.DataContext == this);
                thiswin?.Close();
            });

            UpdateCommand = new CommandVM(() =>
            {
                AddOperationWin addOperationWin = new AddOperationWin();
                addOperationWin.Closed += DataUpdate;
                addOperationWin.Show();
            });
        }

        private void DataUpdate(object sender, EventArgs e)
        {
            GetData();
        }
        private void GetData()
        {
            if(operationManager.CurrentOperation!=null)
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
}
