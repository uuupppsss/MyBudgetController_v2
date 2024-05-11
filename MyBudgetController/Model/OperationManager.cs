using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Linq;

namespace MyBudgetController.Model
{
    public class OperationManager:INotifyPropertyChanged
    {
        static OperationManager instance;

        public event PropertyChangedEventHandler PropertyChanged;

        public static OperationManager Instance
        {
            get
            {
                if (instance == null)
                    instance = new OperationManager();
                return instance;
            }
        }
        //FilterManager filterManager;
        public ObservableCollection<Operation> CurrentExpencesCollection { get; set; }  
        public ObservableCollection<Operation> CurrentIncomesCollection { get; set; }
        public string CurrentOperationType {  get; set; }

        //public OperationManager()
        //{
        //    filterManager=FilterManager.Instance;

        //}

        public void GetOperations(int[] filter_date, string type)
        {
            DBConnection dbConnection = DBConnection.Instance;
            UserManager userManager = UserManager.Instance;

            string query;
            if (filter_date[1] == 0) query = $"SELECT id, Name, Sum, Date, type_id " +
                    $"FROM Operations WHERE user_id={userManager.CurrentUser.Id} and year(Date)={filter_date[0]} and " +
                    $"Type_id in (select id from Categories where Type='{type}') order by Date desc";

            else query = $"SELECT id, Name, Sum, Date, type_id " +
                    $"FROM Operations " +
                    $"WHERE user_id = {userManager.CurrentUser.Id} and year(Date)={filter_date[0]} and month(Date)= {filter_date[1]} and " +
                    $"Type_id in (select id from Categories where Type='{type}') order by Date desc ";

            MySqlCommand command = new MySqlCommand(query, dbConnection.GetConnection());
            MySqlDataReader reader = command.ExecuteReader();
            ObservableCollection<Operation> OperationCollection = new ObservableCollection<Operation>();
            while (reader.Read())
            {
                Operation operation=new Operation();
                operation.ID = reader.GetInt32(0);
                operation.Name = reader.IsDBNull(1) ? "" : reader.GetString(1);
                operation.Sum = reader.GetDouble(2);
                operation.Date = reader.GetDateTime(3);
                operation.Type = operation.SetType(reader.GetInt32(4),type);

                OperationCollection.Add(operation);
            }
            reader.Close();
            command.Dispose();

            switch (type)
            {
                case "Expences": CurrentExpencesCollection=OperationCollection; break;
                case "Incomes": CurrentIncomesCollection=OperationCollection; break;
            }
        }

        public void AddNewOperation(Operation operation)
        {
            if (operation.Type == null)
            {
                MessageBox.Show("Select type", "Error", MessageBoxButton.OK);
                return;
            }
            if (operation.Sum == 0)
            {
                MessageBox.Show("Input sum", "Error", MessageBoxButton.OK);
                return ;
            }
            
            DBConnection dbConnection = DBConnection.Instance;
            UserManager userManager = UserManager.Instance;

            MySqlConnection connection = dbConnection.GetConnection();

                string query = $"INSERT INTO Operations (Date, Name, Type_id, Sum, user_id) VALUES (@Date, @Name, @Type_id, @Sum, @user_id)";
                MySqlCommand command = new MySqlCommand(query, connection);
                command.Parameters.AddWithValue("@Date", operation.Date);
                command.Parameters.AddWithValue("@Name", operation.Name);
                command.Parameters.AddWithValue("@Type_id", operation.Type.Id);
                command.Parameters.AddWithValue("@Sum", operation.Sum);
                command.Parameters.AddWithValue("@user_id", userManager.CurrentUser.Id);
                var result=command.ExecuteNonQuery();
                command.Dispose();
            if (result == 0)
            {
                MessageBox.Show("Insert failed, try again", "Error", MessageBoxButton.OK);
                return;
            }

            string query1 = $"select id from Operations where id=(select max(id) from Operations where user_id={userManager.CurrentUser.Id})";
            MySqlCommand command1 = new MySqlCommand(query1, connection);
            object result1 = command1.ExecuteScalar();
            command.Dispose();
            operation.ID = int.Parse(result1.ToString());
            switch (operation.Type.Type)
            {
                case "Expences":
                    CurrentExpencesCollection.Add(operation);

                    break;
                case "Incomes":
                    CurrentIncomesCollection.Add(operation);
                    break;
            }
            MessageBox.Show("Success", "Success", MessageBoxButton.OK);
        }

        public void RemoveOperation(Operation operation)
        {
            DBConnection dbConnection = DBConnection.Instance;

            var dialogresult = MessageBox.Show("Delete this operation?", "Delete", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (dialogresult == MessageBoxResult.Yes)
            {
                string query = $"delete from Operations where id={operation.ID}";
                MySqlCommand command = new MySqlCommand(query, dbConnection.GetConnection());
                object result = command.ExecuteNonQuery();
                command.Dispose();
                switch (operation.Type.Type)
                {
                    case "Expences": CurrentExpencesCollection.Remove(operation); break;
                    case "Incomes": CurrentIncomesCollection.Remove(operation); break;
                }
                if (result != null)
                    MessageBox.Show("Success", "Success", MessageBoxButton.OK);
                else
                    MessageBox.Show("Error", "Error", MessageBoxButton.OK);
            }

        }

    }
}
