using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class SqlBuilderUtils
    {
        private static object objlocker = new object();
        static Dictionary<string, string> cache = new Dictionary<string, string>();
        static List<string> GetList(Type tp, bool IncludeReadOnly)
        {
            var properties = tp.GetProperties(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);
            List<string> list = new List<string>();

            foreach (var p in properties)
            {
                if (!IncludeReadOnly)
                {

                    var attrs = p.GetCustomAttributes(typeof(ReadOnlyAttribute), false);
                    if (attrs.Length > 0)
                        continue;
                }
                list.Add(p.Name);
            }
            return list;
        }

        internal static string CreateQuerySql(Type type)
        {
            lock (objlocker)
            {
                string sql = "";
                string key = $"{type.FullName}[SelectSql]";
                if (cache.TryGetValue(key, out sql))
                {
                    return sql;
                }
                var list = GetList(type, true);

                sql = $"SELECT a.* FROM {type.Name} as a  Where 1=1 ";
                cache.Add(key, sql);
                return sql;
            }
        }



        public static string CreateInsertSql(Type type)
        {
            lock (objlocker)
            {
                string sql = "";
                string key = $"{type.FullName}[InsertSql]";
                if (cache.TryGetValue(key, out sql))
                {
                    return sql;
                }
                var list = GetList(type, false);
                sql = $"INSERT INTO {type.Name} ({string.Join(",", list.ToArray())}) Values({string.Join(",", list.Select(fn => "@" + fn).ToArray())})";
                cache.Add(key, sql);
                return sql;
            }
        }
        public static string CreateInsertSql<T>()
        {
            return CreateInsertSql(typeof(T));
        }

        internal static string CreateInsertDeclareSql(Type type)
        {
            var list = GetList(type, false);
            var sql = $"INSERT INTO {type.Name} ({string.Join(",", list.ToArray())})";
            return sql;
        }

        internal static string CreateInsertGetValueForDeclareSql<TSource>(TSource item) 
        {
            List<string> values = new List<string>();
            var type = item.GetType();
            var list = GetList(type, false);
            foreach (var p in list)
            {
                var property = type.GetProperty(p);
                object value = property.GetValue(item, null);
                values.Add(GetSqlValue(value, property.PropertyType));
            }
            return $"({string.Join(",", values.ToArray())})";
        }
        static string GetSqlValue(object val, Type type)
        {
            if (val == null)
                return "NULL";
            if (type == typeof(int)
                || type == typeof(double)
                || type == typeof(decimal)
               || type == typeof(long)
               || type == typeof(short)
               || type == typeof(int?)
                || type == typeof(double?)
                || type == typeof(decimal?)
               || type == typeof(long?)
               || type == typeof(short?)
               )
            {
                return $"{val}";
            }
            else if (type == typeof(bool))
            {
                return $"{((bool)val ? 1 : 0)}";
            }
            else if (type == typeof(DateTime))
            {
                DateTime v;
                if (!DateTime.TryParse(val.ToString(), out v))
                {
                    v = DateTime.Parse("1901-1-1");
                }
                return $"'{(DateTime.Parse(v.ToString("yyyy-MM-dd HH:mm:ss")))}'";
            }
            return $"'{val}'";
        }

        internal static void SqlBulkCopy<TSource>(DataContext<SqlConnection> context, List<TSource> dataSet) 
        {
            var type = typeof(TSource);
            DataTable dt = GetTableSchema(context, type.Name);
            var list = GetList(type, true);

            foreach (var val in dataSet)
            {
                var dr = dt.NewRow();
                foreach (DataColumn c in dt.Columns)
                {
                    var property = type.GetProperty(c.ColumnName, System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public |
                        System.Reflection.BindingFlags.IgnoreCase);
                    if (property != null)
                        dr[c] = property.GetValue(val, null);
                }
                dt.Rows.Add(dr);
            }

            using (SqlBulkCopy bulkCopy = new SqlBulkCopy(context.ConnectionString, SqlBulkCopyOptions.KeepIdentity))
            {
                //每一批次中的行数
                bulkCopy.BatchSize = 100000;
                //超时之前操作完成所允许的秒数
                bulkCopy.BulkCopyTimeout = 1800;

                //将DataTable表名作为待导入库中的目标表名 
                bulkCopy.DestinationTableName = dt.TableName;

                //将数据集合和目标服务器库表中的字段对应  
                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    //列映射定义数据源中的列和目标表中的列之间的关系
                    bulkCopy.ColumnMappings.Add(dt.Columns[i].ColumnName, dt.Columns[i].ColumnName);
                }
                //将DataTable数据上传到数据表中
                bulkCopy.WriteToServer(dt);
            }
        }

        private static DataTable GetTableSchema(DataContext<SqlConnection> context, string name)
        {
            DataTable dt = new DataTable(name);
            using (SqlConnection cnn = new SqlConnection(context.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand($"select * from [{name}] where  1=2", cnn);
                SqlDataAdapter dpt = new SqlDataAdapter(cmd);
                dpt.Fill(dt);
            }
            return dt;
        }

        internal static void MySqlBulkCopy<TSource>(DataContext<MySqlConnection> context, List<TSource> dataSet)
        {
            if (dataSet.Count == 0) return;
            var type = typeof(TSource);
            DataTable table = GetMyTableSchema(context, type.Name);
            var list = GetList(type, true);

            foreach (var val in dataSet)
            {
                var dr = table.NewRow();
                foreach (DataColumn c in table.Columns)
                {
                    var property = type.GetProperty(c.ColumnName, System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public |
                        System.Reflection.BindingFlags.IgnoreCase);
                    if (property != null)
                        dr[c] = property.GetValue(val, null);
                }
                table.Rows.Add(dr);
            }

            if (string.IsNullOrEmpty(table.TableName))
                throw new Exception("请给DataTable的TableName属性附上表名称");

            int insertCount = 0;
            string tmpPath = Path.GetTempFileName();
            string csv = DataTableToCsv(table);
            File.WriteAllText(tmpPath, csv);

            using (MySqlConnection conn = new MySqlConnection(context.ConnectionString))
            {
                try
                {
                    conn.Open();
                    MySqlBulkLoader bulk = new MySqlBulkLoader(conn)
                    {
                        FieldTerminator = ",",
                        FieldQuotationCharacter = '"',
                        EscapeCharacter = '"',
                        LineTerminator = "\r\n",
                        FileName = tmpPath,
                        NumberOfLinesToSkip = 0,
                        TableName = table.TableName,
                    };
                    insertCount = bulk.Load();
                }
                catch (MySqlException ex)
                {
                    throw ex;
                }
            }
            File.Delete(tmpPath);
        }

        private static string DataTableToCsv(DataTable table)
        {
            //以半角逗号（即,）作分隔符，列为空也要表达其存在。  
            //列内容如存在半角逗号（即,）则用半角引号（即""）将该字段值包含起来。  
            //列内容如存在半角引号（即"）则应替换成半角双引号（""）转义，并用半角引号（即""）将该字段值包含起来。  
            StringBuilder sb = new StringBuilder();
            DataColumn colum;
            foreach (DataRow row in table.Rows)
            {
                for (int i = 0; i < table.Columns.Count; i++)
                {
                    colum = table.Columns[i];
                    if (i != 0) sb.Append(",");
                    if (colum.DataType == typeof(string) && row[colum].ToString().Contains(","))
                    {
                        sb.Append("\"" + row[colum].ToString().Replace("\"", "\"\"") + "\"");
                    }
                    else sb.Append(row[colum].ToString());
                }
                sb.AppendLine();
            }


            return sb.ToString();
        }

        private static DataTable GetMyTableSchema(DataContext<MySqlConnection> context, string name)
        {
            DataTable dt = new DataTable(name);

            string sql = $@"select * from { name} where 1=2";
            using (MySqlConnection cn = new MySqlConnection(context.ConnectionString))
            {
                MySqlCommand cmd = new MySqlCommand(sql, cn);
                MySqlDataAdapter dpt = new MySqlDataAdapter(cmd);
                dpt.Fill(dt);
            }
            return dt;
        }
    }
}
