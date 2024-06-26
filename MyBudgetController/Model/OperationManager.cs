﻿using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;


namespace MyBudgetController.Model
{
    public class OperationManager

    {
        static OperationManager instance;

        public static OperationManager Instance
        {
            get
            {
                if (instance == null)
                    instance = new OperationManager();
                return instance;
            }
        }
        public ObservableCollection<Operation> CurrentExpencesCollection { get; set; }  
        public ObservableCollection<Operation> CurrentIncomesCollection { get; set; }

        public event Action ExpencesCollectionChanged;
        public event Action IncomesCollectionChanged;
        public string CurrentOperationType {  get; set; }

        public int selected_year { get; set; }
        public int selected_month {  get; set; }

        public Operation CurrentOperation {  get; set; }

        private DBConnection dbConnection;
        private UserManager userManager;
        private AccountManager accountManager;
        private Account account;



        public void GetOperations(string type)
        {
            dbConnection = DBConnection.Instance;
            userManager = UserManager.Instance;
            accountManager = AccountManager.Instance;
            account = accountManager.SelectedAccount;

            if (account==null)
                 account = accountManager.Accounts[0];

            string query;
            if (selected_month == 0) query = $"SELECT id, Name, Sum, Date, type_id, account_id, InputDate " +
                    $"FROM Operations " +
                    $"WHERE year(Date)={selected_year} and account_id={account.Id} and " +
                    $"Type_id in (select id from Categories where Type='{type}' and user_id={userManager.CurrentUser.Id}) order by Date desc";

            else query = $"SELECT id, Name, Sum, Date, type_id, account_id, InputDate " +
                    $"FROM Operations " +
                    $"WHERE year(Date)={selected_year} and month(Date)= {selected_month} and account_id={account.Id} and " +
                    $"Type_id in (select id from Categories where Type='{type}' and user_id={userManager.CurrentUser.Id}) order by Date desc ";

            switch (type)
            {
                case "Expences": 
                    CurrentExpencesCollection = new ObservableCollection<Operation>();
                    MySqlCommand command = new MySqlCommand(query, dbConnection.GetConnection());
                    MySqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        Operation operation = new Operation();
                        operation.ID = reader.GetInt32(0);
                        operation.Name = reader.IsDBNull(1) ? "" : reader.GetString(1);
                        operation.Sum = reader.GetDouble(2);
                        operation.Date = reader.GetDateTime(3);
                        operation.Type = operation.SetType(reader.GetInt32(4), type);
                        operation.Account = operation.SetAccount(reader.GetInt32(5));
                        operation.InputDate = reader.GetDateTime(6);

                        CurrentExpencesCollection.Add(operation);
                    }
                    reader.Close();
                    command.Dispose();
                    break;
                case "Incomes":
                    CurrentIncomesCollection = new ObservableCollection<Operation>();
                    MySqlCommand command1 = new MySqlCommand(query, dbConnection.GetConnection());
                    MySqlDataReader reader1 = command1.ExecuteReader();
                    while (reader1.Read())
                    {
                        Operation operation = new Operation();
                        operation.ID = reader1.GetInt32(0);
                        operation.Name = reader1.IsDBNull(1) ? "" : reader1.GetString(1);
                        operation.Sum = reader1.GetDouble(2);
                        operation.Date = reader1.GetDateTime(3);
                        operation.Type = operation.SetType(reader1.GetInt32(4), type);
                        operation.Account = operation.SetAccount(reader1.GetInt32(5));
                        operation.InputDate = reader1.GetDateTime(6);

                        CurrentIncomesCollection.Add(operation);
                    }
                    reader1.Close();
                    command1.Dispose();
                    break;

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
            AccountManager accountManager = AccountManager.Instance;
            FilterManager filterManager = FilterManager.Instance;

            MySqlConnection connection = dbConnection.GetConnection();

                string query = $"INSERT INTO Operations (Date, Name, Type_id, Sum, account_id) VALUES (@Date, @Name, @Type_id, @Sum,@account_id)";
                MySqlCommand command = new MySqlCommand(query, connection);
                command.Parameters.AddWithValue("@Date", operation.Date);
                command.Parameters.AddWithValue("@Name", operation.Name);
                command.Parameters.AddWithValue("@Type_id", operation.Type.Id);
                command.Parameters.AddWithValue("@Sum", operation.Sum);
                command.Parameters.AddWithValue("@account_id", accountManager.SelectedAccount.Id);
                var result=command.ExecuteNonQuery();
                command.Dispose();

            if (result != 0)
            {
                string query1 = $"select id from Operations where id=(select max(id) from Operations where account_id={account.Id})";
                MySqlCommand command1 = new MySqlCommand(query1, connection);
                object result1 = command1.ExecuteScalar();
                command.Dispose();
                operation.ID = int.Parse(result1.ToString());
                filterManager.GetBalance();
                if (!filterManager.Years.Contains(operation.Date.Year))
                {
                    filterManager.Years.Add(operation.Date.Year);
                    List<int> y = new List<int>(filterManager.Years.OrderByDescending(d=>d));
                    filterManager.Years = y;
                }


                bool ifDateMatch = operation.Date.Year == selected_year && (operation.Date.Month == selected_month || selected_month == 0);
                if (ifDateMatch)
                {
                    switch (operation.Type.Type)
                    {
                        case "Expences":
                            CurrentExpencesCollection.Add(operation);
                            CurrentExpencesCollection = new ObservableCollection<Operation>(CurrentExpencesCollection.OrderByDescending(o => o.Date));
                            ExpencesCollectionChanged.Invoke();
                            break;
                        case "Incomes":
                            CurrentIncomesCollection.Add(operation);
                            CurrentIncomesCollection = new ObservableCollection<Operation>(CurrentIncomesCollection.OrderByDescending(o => o.Date));
                            IncomesCollectionChanged.Invoke();
                            break;               
                    }
                }

                MessageBox.Show("Success", "Success", MessageBoxButton.OK);
            }
            else
                MessageBox.Show("Insert failed, try again", "Error", MessageBoxButton.OK);

        }

        public void RemoveOperation(Operation operation)
        {
            DBConnection dbConnection = DBConnection.Instance;
            FilterManager filterManager = FilterManager.Instance;

            var dialogresult = MessageBox.Show("Delete this operation?", "Delete", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (dialogresult == MessageBoxResult.Yes)
            {
                string query = $"delete from Operations where id={operation.ID}";
                MySqlCommand command = new MySqlCommand(query, dbConnection.GetConnection());
                var result = command.ExecuteNonQuery();
                command.Dispose();

                if (result != 0)
                {
                    filterManager.GetBalance();
                    switch (operation.Type.Type)
                    {
                        case "Expences": CurrentExpencesCollection.Remove(operation);
                            ExpencesCollectionChanged.Invoke();
                            break;
                        case "Incomes": CurrentIncomesCollection.Remove(operation);
                            IncomesCollectionChanged.Invoke();
                            break;
                    }
                    MessageBox.Show("Success", "Success", MessageBoxButton.OK);
                }
                else
                    MessageBox.Show("Error", "Error", MessageBoxButton.OK);
            }

        }
        public void UpdateOperation(Operation operation)
        {
            if (operation.Type == null)
            {
                MessageBox.Show("Select type", "Error", MessageBoxButton.OK);
                return;
            }
            if (operation.Sum == 0)
            {
                MessageBox.Show("Input sum", "Error", MessageBoxButton.OK);
                return;
            }
            DBConnection dbConnection = DBConnection.Instance;
            FilterManager filterManager= FilterManager.Instance;
            string query0 = $"update Operations set Name=@Name, Date=@Date, type_id=@Type_id, Sum=@Sum where id={operation.ID} ";
            MySqlCommand command0 = new MySqlCommand(query0, dbConnection.GetConnection());
            command0.Parameters.AddWithValue("@Date", operation.Date);
            command0.Parameters.AddWithValue("@Name", operation.Name);
            command0.Parameters.AddWithValue("@Type_id", operation.Type.Id);
            command0.Parameters.AddWithValue("@Sum", operation.Sum);
            int res=command0.ExecuteNonQuery();
            command0.Dispose();
            if (res != 0)
            {
                filterManager.GetBalance();
                string type= operation.Type.Type;

                bool ifDateMatch = operation.Date.Year == selected_year && (operation.Date.Month == selected_month || selected_month == 0);

                    switch(type)
                    {
                        case "Expences":
                        CurrentExpencesCollection.Remove(CurrentOperation);
                        if (ifDateMatch)
                        {
                            CurrentExpencesCollection.Add(operation);
                            CurrentExpencesCollection = new ObservableCollection<Operation>(CurrentExpencesCollection.OrderByDescending(o => o.Date));
                        }
                        ExpencesCollectionChanged.Invoke();
                            break;


                        case "Incomes":
                        CurrentIncomesCollection.Remove(CurrentOperation);
                        if (ifDateMatch)
                        {
                            CurrentIncomesCollection.Add(operation);
                            CurrentIncomesCollection = new ObservableCollection<Operation>(CurrentIncomesCollection.OrderByDescending(o => o.Date));
                        }
                        IncomesCollectionChanged.Invoke();
                            break;  
                    }
                CurrentOperation = operation;
                MessageBox.Show("Success", "Success", MessageBoxButton.OK);
            }
                
        }

    }
}
