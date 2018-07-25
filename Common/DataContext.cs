using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;

namespace Common
{
    /// <summary>
    /// 数据上下文
    /// </summary>
    public class DataContext<Connection> : IDbConnection where Connection : DbConnection
    {
        public DataContext(string connectionString)
        {
            conn = Activator.CreateInstance<Connection>();
            conn.ConnectionString = connectionString;
        }
        private Connection conn;
 
        public string ConnectionString{
            get;
            set;
        }

        public int ConnectionTimeout
        {
            get
            {
                return conn.ConnectionTimeout;
            }
        }

        public string Database
        {
            get
            {
                return conn.Database;
            }
        }

        public ConnectionState State
        {
            get
            {
                return conn.State;
            }
        }

        public IDbTransaction BeginTransaction()
        {
            return conn.BeginTransaction();
        }

        public IDbTransaction BeginTransaction(IsolationLevel il)
        {
            return conn.BeginTransaction(il);
        }

        public void ChangeDatabase(string databaseName)
        {
            conn.ChangeDatabase(databaseName);
        }

        public void Close()
        {
            conn.Close();
        }

        public IDbCommand CreateCommand()
        {
            return conn.CreateCommand();
        }

        public void Dispose()
        {
            conn.Dispose();
        }

        public void Open()
        {
            conn.Open();
        }
    }
}
