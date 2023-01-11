using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace YzProject.Domain
{
    /// <summary>
    /// 上下文扩展类
    /// </summary>
    public static class DbContextExtension
    {
        // ado.net 文档：https://docs.microsoft.com/zh-cn/dotnet/framework/data/adonet/distributed-transactions

        /// <summary>
        /// 执行非查询
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="context">数据库连接上下文</param>
        /// <param name="sql">sql语句</param>
        /// <param name="sqlParams"></param>
        /// <returns>返回受影响的行数</returns>
        public static int ExecuteNoQuery<T>(YzProjectContext context, string sql, DbParameter[] sqlParams) where T : new()
        {
            DbConnection connection = context.Database.GetDbConnection();
            DbCommand cmd = connection.CreateCommand();
            //int result = 0;
            context.Database.OpenConnection();
            cmd.CommandText = sql;
            cmd.CommandType = CommandType.Text;
            if (sqlParams != null)
            {
                cmd.Parameters.AddRange(sqlParams);
            }
            int result = cmd.ExecuteNonQuery();
            context.Database.CloseConnection();
            return result;
        }


        /// <summary>
        /// 执行非查询SQL
        /// </summary>
        /// <param name="context">数据库连接上下文</param>
        /// <param name="strSql">sql语句</param>
        /// <param name="cmdType">命令类型默认语句</param>
        /// <param name="dbParameters">参数</param>
        /// <returns>返回受影响的行数</returns>
        public static int ExecuteNonQuery(this YzProjectContext context, string strSql, CommandType cmdType = CommandType.Text, params DbParameter[] dbParameters)
        {
            try
            {
                using (var conn = context.Database.GetDbConnection())
                {
                    using var cmd = conn.CreateCommand();
                    conn.Open();
                    cmd.CommandText = strSql;
                    cmd.CommandType = cmdType;
                    cmd.Parameters.Clear();
                    if (dbParameters != null && dbParameters.Length > 0)
                    {
                        cmd.Parameters.AddRange(dbParameters);
                    }
                    return cmd.ExecuteNonQuery();
                    //using (var cmd = conn.CreateCommand())
                    //{
                    //    conn.Open();
                    //    cmd.CommandText = strSql;
                    //    cmd.CommandType = cmdType;
                    //    cmd.Parameters.Clear();
                    //    if (dbParameters != null && dbParameters.Length > 0)
                    //    {
                    //        cmd.Parameters.AddRange(dbParameters);
                    //    }
                    //    return cmd.ExecuteNonQuery();
                    //}
                }
            }
            catch (Exception ex)
            {
                return -1;
            }
        }

        /// <summary>
        /// 异步执行SQL返回受影响的行数
        /// </summary>
        /// <param name="context">数据库连接上下文</param>
        /// <param name="strSql">sql语句</param>
        /// <param name="cmdType">命令类型默认语句</param>
        /// <param name="dbParameters">参数</param>
        /// <returns></returns>
        public static async Task<int> ExecuteNonQueryAsync(this YzProjectContext context, string strSql, CommandType cmdType = CommandType.Text, params DbParameter[] dbParameters)
        {
            try
            {
                using (var conn = context.Database.GetDbConnection())
                {
                    //conn.ConnectionString = "";
                    using (var cmd = conn.CreateCommand())
                    {
                        conn.Open();
                        cmd.CommandText = strSql;
                        cmd.CommandType = cmdType;
                        cmd.Parameters.Clear();
                        if (dbParameters != null && dbParameters.Length > 0)
                        {
                            cmd.Parameters.AddRange(dbParameters);
                        }
                        return await cmd.ExecuteNonQueryAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                return -1;
            }
        }

        /// <summary>
        /// 获取DataTable
        /// </summary>
        /// <param name="context">数据库连接上下文</param>
        /// <param name="strSql">sql语句</param>
        /// <param name="cmdType">命令类型默认语句</param>
        /// <param name="dbParameters">参数</param>
        /// <returns></returns>
        public static DataTable ExecuteGetDataTable(this YzProjectContext context, string strSql, CommandType cmdType = CommandType.Text, params DbParameter[] dbParameters)
        {
            DataTable dt = new DataTable();
            try
            {
                using (var conn = context.Database.GetDbConnection())
                {
                    using (var da = DbProviderFactories.GetFactory(conn).CreateDataAdapter())
                    {
                        // context.Database.OpenConnection();
                        conn.Open();
                        da.SelectCommand.CommandText = strSql;
                        da.SelectCommand.CommandType = cmdType;
                        da.SelectCommand.Parameters.Clear();
                        if (dbParameters != null && dbParameters.Length > 0)
                        {
                            da.SelectCommand.Parameters.AddRange(dbParameters);
                        }
                        da.Fill(dt);
                        return dt;
                    }
                }
            }
            catch (Exception ex)
            {
                return dt;
            }
        }

        public static DataTable ExecuteGetDataTable2(this YzProjectContext context, string sql, CommandType type = CommandType.Text, params DbParameter[] dbParameters)
        {
            DbConnection connection = context.Database.GetDbConnection();
            DbCommand cmd = connection.CreateCommand();
            context.Database.OpenConnection();
            cmd.CommandText = sql;
            //3分钟过期时间
            cmd.CommandTimeout = 180;
            cmd.CommandType = type;
            cmd.Parameters.Clear();
            if (dbParameters != null && dbParameters.Length > 0)
            {
                cmd.Parameters.AddRange(dbParameters);
            }
            DataTable dt = new DataTable();
            using (DbDataReader reader =  cmd.ExecuteReader())
            {
                dt.Load(reader);
            }
            context.Database.CloseConnection();
            return dt;
        }

        public static async Task<DataTable> ExecuteGetDataTableAsync(this YzProjectContext context, string sql, CommandType type = CommandType.Text, params DbParameter[] dbParameters)
        {
            DbConnection connection = context.Database.GetDbConnection();
            DbCommand cmd = connection.CreateCommand();
            context.Database.OpenConnection();
            cmd.CommandText = sql;
            //3分钟过期时间
            cmd.CommandTimeout = 180;
            cmd.CommandType = type;
            cmd.Parameters.Clear();
            if (dbParameters != null && dbParameters.Length > 0)
            {
                cmd.Parameters.AddRange(dbParameters);
            }
            DataTable dt = new DataTable();
            using (DbDataReader reader = await cmd.ExecuteReaderAsync())
            {
                dt.Load(reader);
            }
            context.Database.CloseConnection();
            return dt;
        }
    }
}
