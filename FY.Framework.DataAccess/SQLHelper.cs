/*
 *作者：Frank Fan
 *创建时间：2011-5-23 16:52:18
 *再次编辑时间：2012-8-18 23:40:24
 *说明：数据库助手类
 *版权所有：fanyong@Gmail.com
 */

using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;

namespace DAL
{
    public class SQLHelper
    {
        #region 变量
        private SqlConnection conn = null;
        private SqlCommand cmd = null;
        private SqlDataReader sdr = null;
        #endregion

        public SQLHelper()
        {
            string connStr = ConfigurationManager.ConnectionStrings["connStr"].ConnectionString;
            conn = new SqlConnection(connStr);
        }

        #region 获取数据库的连接
        /// <summary>
        /// 获取数据库的连接
        /// </summary>
        /// <returns>SqlConnection</returns>
        private SqlConnection GetConn()
        {
            //判断数据库状态
            if (conn.State == ConnectionState.Closed)
            {
                conn.Open();
            }
            return conn;
        }
        #endregion

        #region 执行不带参数的增删改SQL语句或存储过程
        /// <summary>
        /// 执行不带参数的增删改SQL语句或存储过程
        /// </summary>
        /// <param name="cmdText">SQL语句或存储过程</param>
        /// <param name="ct">命令类型</param>
        /// <returns>受影响的行数</returns>
        public int ExecuteNonQuery(string cmdText, CommandType ct)
        {
            int res;
            try
            {
                SqlCommand cmd = new SqlCommand(cmdText, GetConn());
                cmd.CommandType = ct;
                res = cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }
            }

            return res;
        }
        #endregion

        #region 执行带参数的增删改SQL语句或存储过程
        /// <summary>
        /// 执行带参数的增删改SQL语句或存储过程
        /// </summary>
        /// <param name="cmdText">SQL语句或存储过程</param>
        /// <param name="paras">参数</param>
        /// <param name="ct">命令类型</param>
        /// <returns>受影响的行数</returns>
        public int ExecuteNonQuery(string cmdText, SqlParameter[] paras, CommandType ct)
        {
            int res;
            using (cmd = new SqlCommand(cmdText, GetConn()))
            {
                cmd.CommandType = ct;
                //不用Add方法，用多个参数同时添加方法AddRange
                //cmd.Parameters.Add(new SqlParameter("@catName","SQL注入')delete category where id=1--"));
                cmd.Parameters.AddRange(paras);
                res = cmd.ExecuteNonQuery();
            }
            return res;
        }
        #endregion

        #region 执行不带参数的SQL语句或存储过程
        /// <summary>
        /// 执行SQL语句或存储过程
        /// </summary>
        /// <param name="cmdText">SQL语句或存储过程</param>
        /// <param name="ct">命令类型</param>
        /// <returns>DataTable</returns>
        public DataTable ExecuteQuery(string cmdText, CommandType ct)
        {
            DataTable dt = new DataTable();
            cmd = new SqlCommand(cmdText, GetConn());
            cmd.CommandType = ct;
            using (sdr = cmd.ExecuteReader(CommandBehavior.CloseConnection))
            {
                //把数据装入到DataTable中
                dt.Load(sdr);
            }
            //记的关闭
            conn.Close();
            return dt;
        }
        #endregion

        #region 执行带参数的SQL语句或存储过程
        /// <summary>
        /// 执行带参数的SQL语句或存储过程
        /// </summary>
        /// <param name="cmdText">SQL语句或存储过程</param>
        /// <param name="paras">集合</param>
        /// <param name="ct">命令类型</param>
        /// <returns>DataTable</returns>
        public DataTable ExecuteQuery(string cmdText, SqlParameter[] paras, CommandType ct)
        {
            DataTable dt = new DataTable();
            cmd = new SqlCommand(cmdText, GetConn());
            cmd.CommandType = ct;
            cmd.Parameters.AddRange(paras);
            using (sdr = cmd.ExecuteReader(CommandBehavior.CloseConnection))
            {
                //把数据装入到DataTable中
                dt.Load(sdr);
            }
            //记的关闭
            conn.Close();
            return dt;
        }
        #endregion

        #region 执行sql语句，返回第一行第一列的值
        /// <summary>
        /// 执行sql语句，返回第一行第一列的值
        /// </summary>
        /// <param name="sql">sql语句</param>
        /// <returns>返回第一行第一列</returns>
        public string ExecuteScalar(string sql)
        {
            try
            {
                conn.Open();
                cmd = new SqlCommand(sql, conn);
                object obj = cmd.ExecuteScalar();
                if (obj != null)
                {
                    return obj.ToString();
                }
                return "";
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }
            }
        }
        #endregion

    }
}