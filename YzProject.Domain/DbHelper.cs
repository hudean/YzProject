using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YzProject.Domain
{
    public sealed class DbHelper
    {
        private readonly string _connectionString;
        public readonly string _providerName;
        public DbProviderFactory Factory { get; set; }

        public DbHelper()
        {
            _connectionString = "";
            _providerName = "MySql.Data.MySqlClient";
            //"System.Data.SqlClient"
            //"System.Data.OleDb";
            //"MySql.Data.MySqlClient";
            //"System.Data.OracleClient";
            CreateFactory();
        }

        private void CreateFactory()
        {
            Factory = DbProviderFactories.GetFactory(_providerName);
        }


        public int ExecuteNonQuery(string sqlStr, CommandType commandType = CommandType.Text,params DbParameter[] dbParameters)
        {
            try
            {
                using (var conn = Factory.CreateConnection())
                {
                    using (var cmd = conn.CreateCommand())
                    {
                        conn.Open();
                        cmd.CommandText = sqlStr;
                        cmd.CommandType = commandType;
                        cmd.CommandTimeout = 180;
                        cmd.Parameters.Clear();
                        if (dbParameters != null && dbParameters.Count() > 0)
                        {
                            cmd.Parameters.AddRange(dbParameters);
                        }
                        return cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                return -1;
            }
        }

        public object ExecuteScalar(string sqlStr, CommandType commandType = CommandType.Text, params DbParameter[] dbParameters)
        {
            try
            {
                using (var conn = Factory.CreateConnection())
                {
                    using (var cmd = conn.CreateCommand())
                    {
                        conn.Open();
                        cmd.CommandText = sqlStr;
                        cmd.CommandType = commandType;
                        cmd.CommandTimeout = 180;
                        cmd.Parameters.Clear();
                        if (dbParameters != null && dbParameters.Count() > 0)
                        {
                            cmd.Parameters.AddRange(dbParameters);
                        }
                        return cmd.ExecuteScalar();
                    }
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }


        public DataTable ExecuteGetDataTable(string sqlStr, CommandType commandType = CommandType.Text, params DbParameter[] dbParameters)
        {
            DataTable dt = new DataTable();
            try
            {
                using (var conn = Factory.CreateConnection())
                {
                    using (var command = conn.CreateCommand())
                    {
                        conn.Open();
                        command.CommandText = sqlStr;
                        command.CommandType = commandType;
                        command.CommandTimeout = 180;
                        command.Parameters.Clear();
                        if (dbParameters != null && dbParameters.Count() > 0)
                        {
                            command.Parameters.AddRange(dbParameters);
                        }

                        using (var reader = command.ExecuteReader())
                        {

                            //if (reader.HasRows)
                            //{
                            //    while (reader.Read())
                            //    {
                            //        Console.WriteLine("{0}: {1:C}", reader[0], reader[1]);
                            //    }

                            //}
                            //else
                            //{
                            //    Console.WriteLine("No rows found.");
                            //}
                            //reader.Close();

                            //dt = reader.GetSchemaTable();
                            dt.Load(reader);
                            
                            return dt;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return dt;
            }
        }
    }
}
