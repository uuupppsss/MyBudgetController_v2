using MyBudgetController.View;
using MyBudgetController.ViewModel;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MyBudgetController.Model
{
    public class CategoriesManager:INotifyPropertyChanged
    {
        static CategoriesManager instance;
        public static CategoriesManager Instance
        {
            get
            {
                if (instance == null)
                    instance = new CategoriesManager();
                return instance;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private ObservableCollection<Category> currentECategoriesCollection;

        public ObservableCollection<Category> CurrentECategoriesCollection
        {
            get => currentECategoriesCollection; 
            set 
            { 
                currentECategoriesCollection = value;
            }
        }

        private ObservableCollection<Category> currentICategoriesCollection;

        public ObservableCollection<Category> CurrentICategoriesCollection
        {
            get => currentICategoriesCollection;
            set
            {
                currentICategoriesCollection = value;
            }
        }

        private bool isRemoved;

        public bool  IsRemoved
        {
            get => isRemoved; 
            set 
            { 
                isRemoved = value;
                OnPropertyChanged(nameof(IsRemoved));
            }
        }


        public void GetCategory(string type)
        {
            DBConnection dbConnection = DBConnection.Instance;
            UserManager userManager = UserManager.Instance;
            ObservableCollection<Category> categories = new ObservableCollection<Category>();
            MySqlConnection connection = dbConnection.GetConnection();

            string scmd = $"SELECT Name, id FROM Categories WHERE Type='{type}' and user_id={userManager.CurrentUser.Id}";
            if (connection != null)
            {
                MySqlCommand command = new MySqlCommand(scmd, connection);
                MySqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    categories.Add(new Category()
                    {
                        Name = reader.GetString(0),
                        Id = reader.GetInt32(1),
                        Type=type
                    });
                }
                reader.Close();
                command.Dispose();
            }
            switch (type)
            {
                case "Expences": CurrentECategoriesCollection=categories; break;
                case "Incomes": CurrentICategoriesCollection=categories; break;
                    default: MessageBox.Show($"Error", "Error", MessageBoxButton.OK); return;
            }
        }

        public void AddNewCategory(Category category)
        {
            UserManager userManager = UserManager.Instance;
            int user_id = userManager.CurrentUser.Id;
            DBConnection dbConnection = DBConnection.Instance;
            var connection=dbConnection.GetConnection();

            if (connection != null)
            {
                string query = $"select id from Categories where Name='{category.Name}' and user_id={user_id} and Type='{category.Type}'";
                MySqlCommand command = new MySqlCommand(query, connection);
                object result = command.ExecuteScalar();
                command.Dispose();

                if (result != null)
                {
                    MessageBox.Show($"This type is already exist", "Error", MessageBoxButton.OK);
                    return;
                }

                if (category.Name == null)
                {
                    MessageBox.Show($"Input category", "Error", MessageBoxButton.OK);
                    return;
                }

                query = $"INSERT INTO Categories ( Name, Type, user_id) VALUES ( '{category.Name}','{category.Type}', {user_id})";
                command = new MySqlCommand(query, connection);
                result = command.ExecuteNonQuery();
                command.Dispose();

                if (result != null)
                {
                    string query1=$"select id from Categories where id=(select max(id) from Categories where user_id={user_id})";
                    MySqlCommand command1 = new MySqlCommand(query1, connection);
                    object result1 = command1.ExecuteScalar();
                    command.Dispose ();
                    category.Id=int.Parse(result1.ToString());
                    switch (category.Type)
                    {
                        case "Expences": CurrentECategoriesCollection.Add(category); break;
                        case "Incomes": CurrentICategoriesCollection.Add(category); break;
                    }
                    MessageBox.Show($"Success", "Success", MessageBoxButton.OK);
                }

                else MessageBox.Show($"Error", "Error", MessageBoxButton.OK);
            }
        }

        public void RemoveCategory(Category category)
        {
            MySqlConnection connection = DBConnection.Instance.GetConnection();
            UserManager userManager = UserManager.Instance;
                string check_q = $"select id from Categories where Name= 'Другое' and Type='{category.Type}' and user_id={userManager.CurrentUser.Id}";
                MySqlCommand cmd =new MySqlCommand(check_q, connection);
                object result= cmd.ExecuteScalar();
                int id=int.Parse(result.ToString());

                // замена удаляемой категории на дефолтную категорию "Другое"
                string query0 = $"update Operations set type_id={id} where type_id={category.Id} ";
                MySqlCommand command0 = new MySqlCommand(query0, connection);
                command0.ExecuteNonQuery();
                command0.Dispose();

                //удаление категории
                string query = $"delete from Categories where id={category.Id}";
                MySqlCommand command = new MySqlCommand(query, connection);
                object result1 = command.ExecuteNonQuery();
                command.Dispose();
                if (result1 != null)
                {
                    switch (category.Type)
                    {
                        case "Expences":
                            CurrentECategoriesCollection.Remove(category);
                            break;
                        case "Incomes":
                            CurrentICategoriesCollection.Remove(category);
                            break;
                        default:
                            MessageBox.Show("Error", "Error", MessageBoxButton.OK); return;
                    }
                IsRemoved = true;
                }
                else MessageBox.Show("Error", "Error", MessageBoxButton.OK);
            IsRemoved = false;
        }

    }
}
