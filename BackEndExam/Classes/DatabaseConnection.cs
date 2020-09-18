using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace BackEndExam.Classes
{
    public class DatabaseConnection
    {
        private string server;
        private string user;
        private string database;
        private string port;
        private string password;
        private MySqlConnection connection;
        private static DatabaseConnection instance;
        private static readonly object padlock = new object();

        public string Server
        {
            get { return this.server; }
        }
        public string User
        {
            get { return this.user; }
        }
        public string Database
        {
            get { return this.database; }
        }
        public string Port
        {
            get { return this.port; }
        }

        public static DatabaseConnection GetInstance
        {
            get
            {
                if (instance == null)
                {
                    lock (padlock)
                    {
                        if (instance == null)
                            instance = new DatabaseConnection();
                    }
                }
                return instance;
            }
        }

        private DatabaseConnection()
        {
            this.server = "localhost";
            this.user = "root";
            this.database = "backendexam";
            this.port = "3306";
            this.password = "admin";
        }

        private MySqlConnection OpenConnection()
        {
            string connStr = "server=" + server + ";user=" + user + ";database=" + database + ";port=" + port + ";password=" + password;
            this.connection = new MySqlConnection(connStr);
            this.connection.Open();
            return connection;
        }

        private void CloseConnection()
        {
            this.connection.Close();
        }

        public JObject DeleteData(string tablename, int id, string id_searcher = "id")
        {
            JObject data = new JObject();
            data.Add("isError", false);
            try
            {
                JObject temp = new JObject();
                OpenConnection();
                string query = "DELETE FROM " + tablename + " WHERE " + id_searcher + " = " + id;
                MySqlCommand command = new MySqlCommand(query, connection);
                MySqlDataReader reader;
                reader = command.ExecuteReader();
                temp.Add("id", id);
                data.Add("data", temp);
            }
            catch (Exception ex)
            {
                JObject temp = new JObject();
                temp.Add("message", ex.ToString());
                data["isError"] = true;
                data.Add("data", temp);
            }
            CloseConnection();
            return data;
        }

        public JObject UpdateData(string tablename, int id, JObject value)
        {
            JObject data = new JObject();
            data.Add("isError", false);
            try
            {
                JObject temp = new JObject();
                OpenConnection();
                string query = "UPDATE " + tablename + " SET ";
                int i = 0;
                foreach (JProperty property in value.Properties())
                {
                    string celltoinsert = property.Name + "='" + property.Value + "'";
                    celltoinsert += i < value.Count - 1 ? ", " : " ";
                    query += celltoinsert;
                    i++;
                }
                query += "WHERE id = " + id;
                MySqlCommand command = new MySqlCommand(query, connection);
                MySqlDataReader reader;
                reader = command.ExecuteReader();
                temp.Add("id", id);
                data.Add("data", temp);
            }
            catch (Exception ex)
            {
                JObject temp = new JObject();
                temp.Add("message", ex.ToString());
                data["isError"] = true;
                data.Add("data", temp);
            }
            CloseConnection();
            return data;
        }

        public JObject InsertData(string tablename, string[] columnname, List<object> value)
        {
            JObject data = new JObject();
            data.Add("isError", false);
            try
            {
                JObject temp = new JObject();
                OpenConnection();
                string query = "INSERT INTO " + tablename + " (" + String.Join(",", columnname) + ") VALUES";
                int val_i = 0;
                foreach (object obj in value)
                {
                    JObject row = JObject.FromObject(obj);
                    string rowtoinsert = "(";
                    for (int i = 0; i < columnname.Length; i++)
                    {
                        string column = columnname[i];
                        string tempstr = columnname.Length - 1 == i ? "'" + row[column] + "')" : "'" + row[column] + "',";
                        rowtoinsert += tempstr;
                    }
                    query += rowtoinsert;
                    if (!(val_i == value.Count - 1)) query += ", ";
                    val_i++;
                }
                MySqlCommand command = new MySqlCommand(query, connection);
                MySqlDataReader reader;
                reader = command.ExecuteReader();
                temp.Add("id", command.LastInsertedId);
                data.Add("data", temp);
            }
            catch (Exception ex)
            {
                data.Add("message", ex.ToString());
                data["isError"] = true;
            }
            CloseConnection();
            return data;
        }

        public JObject SelectData(string tablename, int id = 0, string id_searcher = "id", JObject order_by = null)
        {
            JObject data = new JObject();
            data.Add("isError", false);
            try
            {
                JObject temp = new JObject();
                OpenConnection();
                string query = "SELECT * FROM " + tablename + " WHERE " + id_searcher + " = " + id;
                if (id < 1) query = "SELECT * FROM " + tablename;
                if (order_by != null) query += " ORDER BY " + order_by["column"] + " " + order_by["mode"];
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    MySqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        JObject row = new JObject();
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            row.Add(reader.GetName(i), reader[i].ToString());
                        }
                        temp.Add(reader["id"].ToString(), row);
                    }
                    reader.Close();
                }
                data.Add("data", temp);
            }
            catch (Exception ex)
            {
                JObject temp = new JObject();
                temp.Add("message", ex.ToString());
                data["isError"] = true;
                data.Add("data", temp);
            }
            CloseConnection();
            return data;
        }
    }
}
