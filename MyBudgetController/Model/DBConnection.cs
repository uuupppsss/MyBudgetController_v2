using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using MySqlConnector;

namespace MyBudgetController.Model
{
    public class DBConnection
    {
        string dbConnect = "server=192.168.200.13;user=student;password=student;database=Operations;Character Set=utf8mb4";
       // string dbConnect = "server=localhost;user=root;database=operationsdb;port=3306;password=root;";

        MySqlConnection connection;

        public DBConnection()
        {
            connection = new MySqlConnection(dbConnect);
            OpenConnection();
        }

        private bool OpenConnection()
        {
            try
            {

                connection.Open();

                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
        }

        public void CloseConnection()
        {
            try
            {
                connection.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        internal MySqlConnection GetConnection()
        {

            if (connection.State != System.Data.ConnectionState.Open)
                if (!OpenConnection())
                    return null;


            return connection;
        }

        static DBConnection instance;
        public static DBConnection Instance
        {
            get
            {
                if (instance == null)
                    instance = new DBConnection();
                return instance;
            }
        }




    }
}
