using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace HotCoreUtils.DB
{
    /// <summary>
    ///数据库操作类
    ///修改日期：2016.07.07
    ///修改作者：郭孟稳
    /// </summary>
    public abstract class DbHelperSQLP
    {
        // Hashtable to store cached parameters
        /// <summary>
        /// 
        /// </summary>
        private static Hashtable parmCache = Hashtable.Synchronized(new Hashtable());

        /// <summary>
        /// Execute a SqlCommand (that returns no resultset) against the database specified in the connection string 
        /// using the provided parameters.
        /// </summary>
        /// <remarks>
        /// e.g.:  
        ///  int result = ExecuteNonQuery(connString, CommandType.StoredProcedure, "PublishOrders", new SqlParameter("@prodid", 24));
        /// </remarks>
        /// <param name="connectionString">a valid connection string for a SqlConnection</param>
        /// <param name="cmdType">the CommandType (stored procedure, text, etc.)</param>
        /// <param name="cmdText">the stored procedure name or T-SQL command</param>
        /// <param name="commandParameters">an array of SqlParamters used to execute the command</param>
        /// <returns>an int representing the number of rows affected by the command</returns>
        public static int ExecuteNonQuery(string connectionString, CommandType cmdType, string cmdText, params SqlParameter[] commandParameters)
        {

            SqlCommand cmd = new SqlCommand();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                PrepareCommand(cmd, conn, null, cmdType, cmdText, commandParameters);
                int val = cmd.ExecuteNonQuery();
                cmd.Parameters.Clear();
                return val;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="cmdType"></param>
        /// <param name="cmdText"></param>
        /// <param name="commandTimeout"></param>
        /// <param name="commandParameters"></param>
        /// <returns></returns>
        public static int ExecuteNonQuery(string connectionString, CommandType cmdType, string cmdText, int commandTimeout, params SqlParameter[] commandParameters)
        {

            SqlCommand cmd = new SqlCommand();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                cmd.CommandTimeout = commandTimeout;
                PrepareCommand(cmd, conn, null, cmdType, cmdText, commandParameters);
                int val = cmd.ExecuteNonQuery();
                cmd.Parameters.Clear();
                return val;
            }
        }

        /// <summary>
        /// 加了输出项
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="cmdType"></param>
        /// <param name="cmdText"></param>
        /// <param name="_outParaName"></param>
        /// <param name="_out"></param>
        /// <param name="commandParameters"></param>
        /// <returns></returns>
        public static int ExecuteNonQuery(string connectionString, CommandType cmdType, string cmdText, string _outParaName, ref int _out, params SqlParameter[] commandParameters)
        {

            SqlCommand cmd = new SqlCommand();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                PrepareCommand(cmd, conn, null, cmdType, cmdText, commandParameters);
                int val = cmd.ExecuteNonQuery();
                _out = Convert.ToInt32(cmd.Parameters[_outParaName].Value);
                cmd.Parameters.Clear();
                return val;
            }
        }

        /// <summary>
        /// Execute a SqlCommand (that returns no resultset) against an existing database connection 
        /// using the provided parameters.
        /// </summary>
        /// <remarks>
        /// e.g.:  
        ///  int result = ExecuteNonQuery(connString, CommandType.StoredProcedure, "PublishOrders", new SqlParameter("@prodid", 24));
        /// </remarks>
        /// <param name="connection">an existing database connection</param>
        /// <param name="cmdType">the CommandType (stored procedure, text, etc.)</param>
        /// <param name="cmdText">the stored procedure name or T-SQL command</param>
        /// <param name="commandParameters">an array of SqlParamters used to execute the command</param>
        /// <returns>an int representing the number of rows affected by the command</returns>
        public static int ExecuteNonQuery(SqlConnection connection, CommandType cmdType, string cmdText, params SqlParameter[] commandParameters)
        {

            SqlCommand cmd = new SqlCommand();

            PrepareCommand(cmd, connection, null, cmdType, cmdText, commandParameters);
            int val = cmd.ExecuteNonQuery();
            cmd.Parameters.Clear();
            return val;
        }

        /// <summary>
        /// Execute a SqlCommand (that returns no resultset) using an existing SQL Transaction 
        /// using the provided parameters.
        /// </summary>
        /// <remarks>
        /// e.g.:  
        ///  int result = ExecuteNonQuery(connString, CommandType.StoredProcedure, "PublishOrders", new SqlParameter("@prodid", 24));
        /// </remarks>
        /// <param name="trans">an existing sql transaction</param>
        /// <param name="cmdType">the CommandType (stored procedure, text, etc.)</param>
        /// <param name="cmdText">the stored procedure name or T-SQL command</param>
        /// <param name="commandParameters">an array of SqlParamters used to execute the command</param>
        /// <returns>an int representing the number of rows affected by the command</returns>
        public static int ExecuteNonQuery(SqlTransaction trans, CommandType cmdType, string cmdText, params SqlParameter[] commandParameters)
        {
            SqlCommand cmd = new SqlCommand();
            PrepareCommand(cmd, trans.Connection, trans, cmdType, cmdText, commandParameters);
            int val = cmd.ExecuteNonQuery();
            cmd.Parameters.Clear();
            return val;
        }

        /// <summary>
        /// Execute a SqlCommand that returns a resultset against the database specified in the connection string 
        /// using the provided parameters.
        /// </summary>
        /// <remarks>
        /// e.g.:  
        ///  SqlDataReader r = ExecuteReader(connString, CommandType.StoredProcedure, "PublishOrders", new SqlParameter("@prodid", 24));
        /// </remarks>
        /// <param name="connectionString">a valid connection string for a SqlConnection</param>
        /// <param name="cmdType">the CommandType (stored procedure, text, etc.)</param>
        /// <param name="cmdText">the stored procedure name or T-SQL command</param>
        /// <param name="commandParameters">an array of SqlParamters used to execute the command</param>
        /// <returns>A SqlDataReader containing the results</returns>
        public static SqlDataReader ExecuteReader(string connectionString, CommandType cmdType, string cmdText, params SqlParameter[] commandParameters)
        {
            SqlCommand cmd = new SqlCommand();
            SqlConnection conn = new SqlConnection(connectionString);

            // we use a try/catch here because if the method throws an exception we want to 
            // close the connection throw code, because no datareader will exist, hence the 
            // commandBehaviour.CloseConnection will not work
            try
            {
                PrepareCommand(cmd, conn, null, cmdType, cmdText, commandParameters);
                SqlDataReader rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                cmd.Parameters.Clear();
                return rdr;
            }
            catch
            {
                conn.Close();
                throw;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="trans"></param>
        /// <param name="cmdType"></param>
        /// <param name="cmdText"></param>
        /// <param name="commandParameters"></param>
        /// <returns></returns>
        public static SqlDataReader ExecuteReader(SqlTransaction trans, CommandType cmdType, string cmdText, params SqlParameter[] commandParameters)
        {
            SqlCommand cmd = new SqlCommand();
            PrepareCommand(cmd, trans.Connection, trans, cmdType, cmdText, commandParameters);
            SqlDataReader rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
            cmd.Parameters.Clear();
            return rdr;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="cmdType"></param>
        /// <param name="cmdText"></param>
        /// <param name="commandTimeout"></param>
        /// <param name="commandParameters"></param>
        /// <returns></returns>
        public static SqlDataReader ExecuteReader(string connectionString, CommandType cmdType, string cmdText, int commandTimeout, params SqlParameter[] commandParameters)
        {
            SqlCommand cmd = new SqlCommand();
            SqlConnection conn = new SqlConnection(connectionString);
            cmd.CommandTimeout = commandTimeout;
            // we use a try/catch here because if the method throws an exception we want to 
            // close the connection throw code, because no datareader will exist, hence the 
            // commandBehaviour.CloseConnection will not work
            try
            {

                PrepareCommand(cmd, conn, null, cmdType, cmdText, commandParameters);
                SqlDataReader rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                cmd.Parameters.Clear();
                return rdr;
            }
            catch
            {
                conn.Close();
                throw;
            }
        }

        /// <summary>
        /// Execute a SqlCommand that returns the first column of the first record against the database specified in the connection string 
        /// using the provided parameters.
        /// </summary>
        /// <remarks>
        /// e.g.:  
        ///  Object obj = ExecuteScalar(connString, CommandType.StoredProcedure, "PublishOrders", new SqlParameter("@prodid", 24));
        /// </remarks>
        /// <param name="connectionString">a valid connection string for a SqlConnection</param>
        /// <param name="cmdType">the CommandType (stored procedure, text, etc.)</param>
        /// <param name="cmdText">the stored procedure name or T-SQL command</param>
        /// <param name="commandParameters">an array of SqlParamters used to execute the command</param>
        /// <returns>An object that should be converted to the expected type using Convert.To{Type}</returns>
        public static object ExecuteScalar(string connectionString, CommandType cmdType, string cmdText, params SqlParameter[] commandParameters)
        {
            SqlCommand cmd = new SqlCommand();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                PrepareCommand(cmd, connection, null, cmdType, cmdText, commandParameters);
                object val = cmd.ExecuteScalar();
                cmd.Parameters.Clear();
                return val;
            }
        }

        /// <summary>
        /// Execute a SqlCommand that returns the first column of the first record against an existing database connection 
        /// using the provided parameters.
        /// </summary>
        /// <remarks>
        /// e.g.:  
        ///  Object obj = ExecuteScalar(connString, CommandType.StoredProcedure, "PublishOrders", new SqlParameter("@prodid", 24));
        /// </remarks>
        /// <param name="conn">an existing database connection</param>
        /// <param name="cmdType">the CommandType (stored procedure, text, etc.)</param>
        /// <param name="cmdText">the stored procedure name or T-SQL command</param>
        /// <param name="commandParameters">an array of SqlParamters used to execute the command</param>
        /// <returns>An object that should be converted to the expected type using Convert.To{Type}</returns>
        public static object ExecuteScalar(SqlConnection connection, CommandType cmdType, string cmdText, params SqlParameter[] commandParameters)
        {

            SqlCommand cmd = new SqlCommand();

            PrepareCommand(cmd, connection, null, cmdType, cmdText, commandParameters);
            object val = cmd.ExecuteScalar();
            cmd.Parameters.Clear();
            return val;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="trans"></param>
        /// <param name="cmdType"></param>
        /// <param name="cmdText"></param>
        /// <param name="commandParameters"></param>
        /// <returns></returns>
        public static object ExecuteScalar(SqlTransaction trans, CommandType cmdType, string cmdText, params SqlParameter[] commandParameters)
        {
            SqlCommand cmd = new SqlCommand();

            PrepareCommand(cmd, trans.Connection, trans, cmdType, cmdText, commandParameters);
            object val = cmd.ExecuteScalar();
            cmd.Parameters.Clear();
            return val;

        }


        /// <summary>
        /// add parameter array to the cache
        /// </summary>
        /// <param name="cacheKey">Key to the parameter cache</param>
        /// <param name="commandParameters">an array of SqlParamters to be cached</param>
        public static void CacheParameters(string cacheKey, params SqlParameter[] commandParameters)
        {
            parmCache[cacheKey] = commandParameters;
        }

        /// <summary>
        /// Retrieve cached parameters
        /// </summary>
        /// <param name="cacheKey">key used to lookup parameters</param>
        /// <returns>Cached SqlParamters array</returns>
        public static SqlParameter[] GetCachedParameters(string cacheKey)
        {
            SqlParameter[] cachedParms = (SqlParameter[])parmCache[cacheKey];

            if (cachedParms == null)
                return null;

            SqlParameter[] clonedParms = new SqlParameter[cachedParms.Length];

            for (int i = 0, j = cachedParms.Length; i < j; i++)
                clonedParms[i] = (SqlParameter)((ICloneable)cachedParms[i]).Clone();

            return clonedParms;
        }

        /// <summary>
        /// 执行Command
        /// </summary>
        /// <param name="cmd">SqlCommand object</param>
        /// <param name="conn">SqlConnection object</param>
        /// <param name="trans">SqlTransaction object</param>
        /// <param name="cmdType">Cmd type e.g. stored procedure or text</param>
        /// <param name="cmdText">Command text, e.g. Select * from Products</param>
        /// <param name="cmdParms">SqlParameters to use in the command</param>
        private static void PrepareCommand(SqlCommand cmd, SqlConnection conn, SqlTransaction trans, CommandType cmdType, string cmdText, SqlParameter[] cmdParms)
        {

            if (conn.State != ConnectionState.Open)
                conn.Open();

            cmd.Connection = conn;
            cmd.CommandText = cmdText;

            if (trans != null)
                cmd.Transaction = trans;

            cmd.CommandType = cmdType;

            if (cmdParms != null)
            {
                foreach (SqlParameter parameter in cmdParms)
                {
                    if ((parameter.Direction == ParameterDirection.InputOutput || parameter.Direction == ParameterDirection.Input) &&
                        (parameter.Value == null))
                    {
                        parameter.Value = DBNull.Value;
                    }
                    cmd.Parameters.Add(parameter);

                }
            }
        }


        /// <summary>
        /// 执行多条SQL语句，实现数据库事务。
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="SQLStringList">多条SQL语句</param>		
        /// <param name="commandParametersList">SqlParameters to use in the command</param>
        public static int ExecuteSqlTran(string connectionString, List<String> SQLStringList, List<SqlParameter[]> commandParametersList = null)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = conn;
                SqlTransaction tx = conn.BeginTransaction();
                cmd.Transaction = tx;
                try
                {
                    int count = 0;
                    for (int n = 0; n < SQLStringList.Count; n++)
                    {
                        string strsql = SQLStringList[n];
                        if (strsql.Trim().Length > 1)
                        {
                            cmd.CommandText = strsql;
                            if (commandParametersList != null)
                            {
                                foreach (SqlParameter parm in commandParametersList[n])
                                    cmd.Parameters.Add(parm);
                            }
                            count += cmd.ExecuteNonQuery();
                        }
                    }
                    tx.Commit();
                    return count;
                }
                catch (Exception)
                {                   
                    tx.Rollback();
                    return 0;
                }
            }
        }

        /// <summary>
        /// 批量添加数据
        /// 作者：郭孟稳 
        /// 时间： 2016-07-15
        /// </summary>
        /// <param name="connectionString">连接字符串</param>
        /// <param name="dataTable">数据源</param>
        /// <param name="TableName">表名</param>
        /// <param name="batchSize">每批数量，默认10000</param>
        public static void BulkInsert(string connectionString, DataTable dataTable, string TableName, int batchSize = 10000)
        {
            using (SqlConnection sqlcon = new SqlConnection(connectionString))
            {
                sqlcon.Open();

                using (SqlBulkCopy bulk = new SqlBulkCopy(sqlcon, SqlBulkCopyOptions.UseInternalTransaction, null))
                {
                    bulk.BatchSize = batchSize;
                    bulk.DestinationTableName = TableName;
                    //循环所有列，为bulk添加映射
                    for (int i = 0; i < dataTable.Columns.Count; i++)
                    {
                        bulk.ColumnMappings.Add(dataTable.Columns[i].ColumnName, dataTable.Columns[i].ColumnName);
                    }
                    bulk.WriteToServer(dataTable);

                }

                sqlcon.Close();
                sqlcon.Dispose();
            }
        }


        /// <summary>
        /// 返回DataView数据 
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="cmdType"></param>
        /// <param name="cmdText"></param>
        /// <param name="commandParameters"></param>
        /// <returns></returns>
        public static DataView GetDataView(string connectionString, CommandType cmdType, string cmdText, params SqlParameter[] commandParameters)
        {
            SqlCommand cmd = new SqlCommand();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                PrepareCommand(cmd, connection, null, cmdType, cmdText, commandParameters);
                SqlDataAdapter da = new SqlDataAdapter();
                DataTable dt = new DataTable();
                da.SelectCommand = cmd;
                da.Fill(dt);
                DataView val = dt.DefaultView;
                cmd.Parameters.Clear();
                return val;
            }
        }

        /// <summary>
        /// 返回DataTable数据
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="cmdType"></param>
        /// <param name="cmdText"></param>
        /// <param name="commandParameters"></param>
        /// <returns></returns>
        public static DataTable GetDataTable(string connectionString, CommandType cmdType, string cmdText, params SqlParameter[] commandParameters)
        {
            SqlCommand cmd = new SqlCommand();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                PrepareCommand(cmd, connection, null, cmdType, cmdText, commandParameters);
                SqlDataAdapter da = new SqlDataAdapter();
                DataTable dt = new DataTable();
                da.SelectCommand = cmd;
                da.Fill(dt);
                cmd.Parameters.Clear();
                return dt;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="cmdType"></param>
        /// <param name="cmdText"></param>
        /// <param name="commandParameters"></param>
        /// <returns></returns>
        public static DataSet GetDataSet(string connectionString, CommandType cmdType, string cmdText, params SqlParameter[] commandParameters)
        {
            SqlCommand cmd = new SqlCommand();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                PrepareCommand(cmd, connection, null, cmdType, cmdText, commandParameters);
                SqlDataAdapter da = new SqlDataAdapter();
                DataSet dt = new DataSet();
                da.SelectCommand = cmd;
                da.Fill(dt);
                cmd.Parameters.Clear();
                return dt;
            }
        }

        /// <summary>
        /// 执行查询语句，返回DataSet
        /// 修改者：郭孟稳
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="SQLString">查询语句</param>
        /// <returns>DataSet</returns>
        public static DataSet Query(string connectionString, string SQLString)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                DataSet ds = new DataSet();
                try
                {
                    connection.Open();
                    SqlDataAdapter command = new SqlDataAdapter(SQLString, connection);
                    command.Fill(ds, "ds");
                }
                catch (System.Data.SqlClient.SqlException ex)
                {
                    throw new Exception(ex.Message);
                }
                return ds;
            }
        }


        /// <summary>
        /// 执行分页存储过程
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="parameters"></param>
        /// <param name="recordCount"></param>
        /// <returns></returns>
        private static DataTable RunDoSplitPage(string connectionString, IDataParameter[] parameters, out int recordCount)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                DataSet dataSet = new DataSet();
                connection.Open();

                SqlDataAdapter sqlDA = new SqlDataAdapter();
                sqlDA.SelectCommand = BuildQueryCommand(connection, "DoSplitPage", parameters);
                sqlDA.Fill(dataSet);

                recordCount = Convert.ToInt32(sqlDA.SelectCommand.Parameters["@RecordCount"].Value);

                connection.Close();

                DataTable dt = dataSet.Tables[1];
                if (dt.Columns.Contains("ROWSTAT"))//游标多出的一列
                    dt.Columns.Remove("ROWSTAT");
                return dt;
            }
        }
        /// <summary>
        /// 构建 SqlCommand 对象(用来返回一个结果集，而不是一个整数值)
        /// </summary>
        /// <param name="connection">数据库连接</param>
        /// <param name="storedProcName">存储过程名</param>
        /// <param name="parameters">存储过程参数</param>
        /// <returns>SqlCommand</returns>
        private static SqlCommand BuildQueryCommand(SqlConnection connection, string storedProcName, IDataParameter[] parameters)
        {
            SqlCommand command = new SqlCommand(storedProcName, connection);
            command.CommandType = CommandType.StoredProcedure;
            foreach (SqlParameter parameter in parameters)
            {
                if (parameter != null)
                {
                    // 检查未分配值的输出参数,将其分配以DBNull.Value.
                    if ((parameter.Direction == ParameterDirection.InputOutput || parameter.Direction == ParameterDirection.Input) &&
                        (parameter.Value == null))
                    {
                        parameter.Value = DBNull.Value;
                    }
                    command.Parameters.Add(parameter);
                }
            }

            return command;
        }
        /// <summary>
        /// 获取分页DataTable
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="strSql"></param>
        /// <param name="iPageSize">分页大小</param>
        /// <param name="iPageIndex">当前页</param>
        /// <param name="iRecordCount">总记录数</param>
        /// <param name="pageCount"></param>
        /// <returns></returns>
        public static List<T> GetSplitDataTable<T>(string connectionString, string strSql, int iPageSize, int iPageIndex, out int iRecordCount, out int pageCount) where T : new()
        {
            SqlParameter[] parameters = {
                                            new SqlParameter("@Sql", SqlDbType.NVarChar, 4000),
                                            new SqlParameter("@PageNumber", SqlDbType.Int, 4),
                                            new SqlParameter("@PageSize", SqlDbType.Int, 4),
                                            new SqlParameter("@RecordCount", SqlDbType.Int, 4)
                                        };
            parameters[0].Value = strSql;
            parameters[1].Value = iPageIndex;
            parameters[2].Value = iPageSize;
            parameters[3].Direction = ParameterDirection.Output;

            DataTable dt = RunDoSplitPage(connectionString, parameters, out iRecordCount);
            List<T> ent = null;
            if (dt != null)
            {
                ent = GetEntityList<T>(dt.CreateDataReader());
            }
            pageCount = iRecordCount / iPageSize;
            if (iRecordCount % iPageSize != 0)
            {
                ++pageCount;
            }
            return ent;
        }
        /// <summary>
        /// 获取分页DataTable
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="strSql"></param>
        /// <param name="iPageSize">分页大小</param>
        /// <param name="iPageIndex">当前页</param>
        /// <param name="iRecordCount">总记录数</param>
        /// <param name="pageCount"></param>
        public static DataTable GetSplitDataTable(string connectionString, string strSql, int iPageSize, int iPageIndex, out int iRecordCount, out int pageCount)
        {
            SqlParameter[] parameters = {
                                            new SqlParameter("@Sql", SqlDbType.NVarChar, 4000),
                                            new SqlParameter("@PageNumber", SqlDbType.Int, 4),
                                            new SqlParameter("@PageSize", SqlDbType.Int, 4),
                                            new SqlParameter("@RecordCount", SqlDbType.Int, 4)
                                        };
            parameters[0].Value = strSql;
            parameters[1].Value = iPageIndex;
            parameters[2].Value = iPageSize;
            parameters[3].Direction = ParameterDirection.Output;
            DataTable dt = RunDoSplitPage(connectionString, parameters, out iRecordCount);
            pageCount = iRecordCount / iPageSize;
            if (iRecordCount % iPageSize != 0)
            {
                ++pageCount;
            }
            return dt;
        }


        /// <summary>
        /// 生成完整分页语句--无总数返回
        /// </summary>
        /// <param name="pageIndex">当前页码</param>
        /// <param name="pageSize">每页数量</param>
        /// <param name="querySql">原语句</param>
        /// <param name="orderbyField">排序字段</param>
        /// <param name="orderby">排序类型，true:默认排序，false:desc排序</param>
        /// <returns>分页语句</returns>
        public static string buildPageSql(int pageIndex, int pageSize, string querySql, string orderbyField, bool orderby = false)
        {

            var fields = orderbyField.Split(',');
            string _orderbyfield = "";
            foreach (var item in fields)
            {
                if (string.IsNullOrEmpty(_orderbyfield))
                    _orderbyfield = string.Format("{0} {1}", item, orderby ? "asc" : "desc");
                else
                    _orderbyfield += string.Format(",{0} {1}", item, orderby ? "asc" : "desc");
            }

            //添加行号
            int startIndex = querySql.IndexOf("select", StringComparison.CurrentCultureIgnoreCase);
            querySql = querySql.Insert(startIndex + 6, string.Format(" ROW_NUMBER() OVER ( ORDER BY {0} ) AS rowIndex, ", _orderbyfield));
            //页码不能小于或等于0
            if (pageIndex <= 0) pageIndex = 1;
            int beginNum = (pageIndex - 1) * pageSize;
            int endNum = pageIndex * pageSize;
            string sql = string.Format(@"   WITH    _table
                                                  AS ( {2}
                                                     )
                                            SELECT *
                                            FROM    _table
                                            WHERE   rowIndex > {0}
                                                    AND rowIndex <= {1}",
                                        beginNum, endNum, querySql);
            return sql;
        }

        /// <summary>
        /// 外部传排序，生成完整分页语句--无总数返回
        /// </summary>
        /// <param name="pageIndex">当前页码</param>
        /// <param name="pageSize">每页数量</param>
        /// <param name="querySql">原语句</param>
        /// <param name="orderbyField">排序字段</param>        
        /// <returns>分页语句</returns>
        public static string buildPageOrderBySql(int pageIndex, int pageSize, string querySql, string orderbyField)
        {

            var fields = orderbyField.Split(',');
            string _orderbyfield = "";
            foreach (var item in fields)
            {
                if (string.IsNullOrEmpty(_orderbyfield))
                    _orderbyfield = string.Format("{0}", item);
                else
                    _orderbyfield += string.Format(",{0} ", item);
            }

            //添加行号
            int startIndex = querySql.IndexOf("select", StringComparison.CurrentCultureIgnoreCase);
            querySql = querySql.Insert(startIndex + 6, string.Format(" ROW_NUMBER() OVER ( ORDER BY {0} ) AS rowIndex, ", _orderbyfield));
            //页码不能小于或等于0
            if (pageIndex <= 0) pageIndex = 1;
            int beginNum = (pageIndex - 1) * pageSize;
            int endNum = pageIndex * pageSize;
            string sql = string.Format(@"   WITH    _table
                                                  AS ( {2}
                                                     )
                                            SELECT *
                                            FROM    _table
                                            WHERE   rowIndex > {0}
                                                    AND rowIndex <= {1}",
                                        beginNum, endNum, querySql);
            return sql;
        }


        /// <summary>
        /// 生成获取总数语句
        /// </summary>
        /// <param name="querySql">原语句</param>
        /// <returns></returns>
        public static string buildRecordCountSql(string querySql)
        {

            string sql = string.Format(@"set nocount on
                                        select COUNT(1) from(
                                            {0}
                                        ) as temp
                                        set nocount off", querySql);
            return sql;
        }



        /// <summary>   
        /// 根据需要得实体类信息   
        /// </summary>   
        /// <typeparam name="T">需要一个对象有一个无参数的实例化方法</typeparam>   
        /// <param name="dr">table数据源</param>   
        /// <returns>返回整理好了集合</returns>           
        public static List<T> GetEntityList<T>(IDataReader dr) where T : new()
        {
            List<T> entityList = new List<T>();

            int fieldCount = -1;

            while (dr.Read())
            {
                if (-1 == fieldCount)
                    fieldCount = dr.FieldCount;

                // 得到实体类对象   
                T t = (T)Activator.CreateInstance(typeof(T));

                for (int i = 0; i < fieldCount; i++)
                {
                    PropertyInfo prop = t.GetType().GetProperty(dr.GetName(i),
                        BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Public);

                    if (null != prop)
                    {
                        // 为了能用在默认为null的值上   
                        // 如 DateTime? tt = null;
                        if (null == dr[i] || Convert.IsDBNull(dr[i]))
                        {
                            if (prop.PropertyType.Name == "DateTime" || prop.PropertyType.Name == "Date")
                                prop.SetValue(t, DateTime.Parse("1975-1-1"), null);
                            else if (prop.PropertyType.Name == "String")
                                prop.SetValue(t, "", null);
                            else
                                prop.SetValue(t, null, null);
                        }
                        else
                            prop.SetValue(t, dr[i], null);
                    }
                }

                entityList.Add(t);
            }
            dr.Close();
            return entityList;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dr"></param>
        /// <returns></returns>
        public static T GetEntity<T>(IDataReader dr) where T : new()
        {
            T entity = default(T);

            int fieldCount = -1;

            if (dr.Read())
            {
                if (-1 == fieldCount)
                    fieldCount = dr.FieldCount;

                // 得到实体类对象   
                T t = (T)Activator.CreateInstance(typeof(T));

                for (int i = 0; i < fieldCount; i++)
                {
                    PropertyInfo prop = t.GetType().GetProperty(dr.GetName(i),
                        BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Public);

                    if (null != prop)
                    {
                        // 为了能用在默认为null的值上   
                        // 如 DateTime? tt = null;   
                        if (null == dr[i] || Convert.IsDBNull(dr[i]))
                            if (prop.PropertyType.Name == "DateTime" || prop.PropertyType.Name == "Date")
                                prop.SetValue(t, DateTime.Parse("1975-1-1"), null);
                            else if (prop.PropertyType.Name == "String")
                                prop.SetValue(t, "", null);
                            else
                                prop.SetValue(t, null, null);
                        else
                            prop.SetValue(t, dr[i], null);
                    }
                }
                entity = t;
            }
            dr.Close();
            return entity;
        }

    }
}