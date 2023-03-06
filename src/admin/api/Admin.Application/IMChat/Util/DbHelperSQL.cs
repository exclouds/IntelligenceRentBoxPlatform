namespace Web.UI.Util
{
    using System;
    using System.Collections;
    using System.Configuration;
    using System.Data;
    using System.Data.SqlClient;
    using System.IO;
    using System.Security.Cryptography;
    using System.Text;

    public abstract class DbHelperSQL
    {
        protected static string ConnectionString =
            "Data Source=192.168.10.80;Initial Catalog=RentBox; Persist Security Info=True;User ID=CMOS;Password=xgz@jZ2021#$%";
        protected static SqlConnection Connection;

        protected static void Close()
        {
            if (Connection != null)
            {
                Connection.Close();
            }
        }

        public static int ExecuteSql(string SQLString, params SqlParameter[] cmdParms)
        {
            int num2;
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                SqlCommand cmd = new SqlCommand();
                try
                {
                    PrepareCommand(cmd, connection, null, SQLString, cmdParms);
                    int num = cmd.ExecuteNonQuery();
                    cmd.Parameters.Clear();
                    connection.Close();
                    return num;
                }
                catch (SqlException)
                {
                    connection.Close();
                    return 0;
                }
                finally
                {
                    if (cmd != null)
                    {
                        cmd.Dispose();
                    }
                }
            }
            return num2;
        }

        public static bool ExecuteSQL(string[] SqlStrings)
        {
            bool flag = true;
            SqlConnection connection = new SqlConnection(ConnectionString);
            connection.Open();
            SqlCommand command = new SqlCommand();
            SqlTransaction transaction = Connection.BeginTransaction();
            command.Connection = connection;
            command.Transaction = transaction;
            try
            {
                foreach (string str in SqlStrings)
                {
                    command.CommandText = str;
                    command.ExecuteNonQuery();
                }
                transaction.Commit();
            }
            catch
            {
                flag = false;
                transaction.Rollback();
            }
            finally
            {
                connection.Close();
            }
            return flag;
        }

        public static int ExecuteSQL(string SqlString)
        {
            int num = -1;
            SqlConnection connection = new SqlConnection(ConnectionString);
            connection.Open();
            try
            {
                num = new SqlCommand(SqlString, connection).ExecuteNonQuery();
            }
            catch
            {
                num = -1;
            }
            finally
            {
                connection.Close();
            }
            return num;
        }

        public static int ExecuteSQL(string SqlString, Hashtable MyHashTb)
        {
            int num = -1;
            SqlConnection connection = new SqlConnection(ConnectionString);
            connection.Open();
            try
            {
                SqlCommand command = new SqlCommand(SqlString, connection);
                foreach (DictionaryEntry entry in MyHashTb)
                {
                    char[] separator = new char[] { '|' };
                    string[] strArray = entry.Key.ToString().Split(separator);
                    if (strArray[1].ToString().Trim() == "string")
                    {
                        command.Parameters.Add(strArray[0], SqlDbType.VarChar);
                    }
                    else if (strArray[1].ToString().Trim() == "int")
                    {
                        command.Parameters.Add(strArray[0], SqlDbType.Int);
                    }
                    else if (strArray[1].ToString().Trim() == "text")
                    {
                        command.Parameters.Add(strArray[0], SqlDbType.Text);
                    }
                    else if (strArray[1].ToString().Trim() == "datetime")
                    {
                        command.Parameters.Add(strArray[0], SqlDbType.DateTime);
                    }
                    else
                    {
                        command.Parameters.Add(strArray[0], SqlDbType.VarChar);
                    }
                    command.Parameters[strArray[0]].Value = entry.Value.ToString();
                }
                num = command.ExecuteNonQuery();
            }
            catch
            {
                num = -1;
            }
            finally
            {
                connection.Close();
            }
            return num;
        }

        public static bool Exists(string strSql)
        {
            int num;
            object single = GetSingle(strSql);
            if (Equals(single, null) || Equals(single, DBNull.Value))
            {
                num = 0;
            }
            else
            {
                num = int.Parse(single.ToString());
            }
            if (num == 0)
            {
                return false;
            }
            return true;
        }

        public static bool Exists(string strSql, params SqlParameter[] cmdParms)
        {
            int num;
            object single = GetSingle(strSql, cmdParms);
            if (Equals(single, null) || Equals(single, DBNull.Value))
            {
                num = 0;
            }
            else
            {
                num = int.Parse(single.ToString());
            }
            if (num == 0)
            {
                return false;
            }
            return true;
        }

        public static DataRow GetDataRow(string SqlString)
        {
            DataSet dataSet = GetDataSet(SqlString);
            if (dataSet.Tables[0].Rows.Count > 0)
            {
                return dataSet.Tables[0].Rows[0];
            }
            return null;
        }

        public static DataSet GetDataSet(string SqlString)
        {
            DataSet set2;
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(SqlString, connection))
                {
                    using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                    {
                        DataSet dataSet = new DataSet();
                        try
                        {
                            adapter.Fill(dataSet, "ds");
                            command.Parameters.Clear();
                        }
                        catch (SqlException exception)
                        {
                            throw new Exception(exception.Message);
                        }
                        connection.Close();
                        set2 = dataSet;
                    }
                }
            }
            return set2;
        }

        public static DataTable GetDataTable(string SqlString) => 
            GetDataSet(SqlString).Tables[0];

        public static int GetMaxID(string FieldName, string TableName)
        {
            int num = 0;
            DataSet dataSet = GetDataSet("select max(" + FieldName + ") from " + TableName);
            if (dataSet.Tables[0].Rows.Count > 0)
            {
                num = int.Parse(dataSet.Tables[0].Rows[0][0].ToString());
            }
            return num;
        }

        public static string GetSHSL(string SqlString)
        {
            DataSet dataSet = GetDataSet(SqlString);
            if (dataSet.Tables[0].Rows.Count > 0)
            {
                return Convert.ToString(dataSet.Tables[0].Rows[0][0].ToString());
            }
            return "";
        }

        public static string GetSHSLInt(string SqlString)
        {
            DataSet dataSet = GetDataSet(SqlString);
            if (dataSet.Tables[0].Rows.Count > 0)
            {
                return Convert.ToString(dataSet.Tables[0].Rows[0][0].ToString());
            }
            return "0";
        }

        public static object GetSingle(string SQLString)
        {
            object obj3;
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                SqlCommand command = new SqlCommand(SQLString, connection);
                try
                {
                    connection.Open();
                    object objA = command.ExecuteScalar();
                    if (Equals(objA, null) || Equals(objA, DBNull.Value))
                    {
                        connection.Close();
                        return null;
                    }
                    connection.Close();
                    return objA;
                }
                catch (SqlException)
                {
                    connection.Close();
                    return null;
                }
                finally
                {
                    if (command != null)
                    {
                        command.Dispose();
                    }
                }
            }
            return obj3;
        }

        public static object GetSingle(string SQLString, int Times)
        {
            object obj3;
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                SqlCommand command = new SqlCommand(SQLString, connection);
                try
                {
                    connection.Open();
                    command.CommandTimeout = Times;
                    object objA = command.ExecuteScalar();
                    if (Equals(objA, null) || Equals(objA, DBNull.Value))
                    {
                        connection.Close();
                        return null;
                    }
                    connection.Close();
                    return objA;
                }
                catch (SqlException)
                {
                    connection.Close();
                    return null;
                }
                finally
                {
                    if (command != null)
                    {
                        command.Dispose();
                    }
                }
            }
            return obj3;
        }

        public static object GetSingle(string SQLString, params SqlParameter[] cmdParms)
        {
            object obj3;
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                SqlCommand cmd = new SqlCommand();
                try
                {
                    PrepareCommand(cmd, connection, null, SQLString, cmdParms);
                    object objA = cmd.ExecuteScalar();
                    cmd.Parameters.Clear();
                    if (Equals(objA, null) || Equals(objA, DBNull.Value))
                    {
                        connection.Close();
                        return null;
                    }
                    connection.Close();
                    return objA;
                }
                catch (Exception)
                {
                    connection.Close();
                    return null;
                }
                finally
                {
                    if (cmd != null)
                    {
                        cmd.Dispose();
                    }
                }
            }
            return obj3;
        }

        public static string GetStringList(string SqlString)
        {
            string str = string.Empty;
            DataSet dataSet = GetDataSet(SqlString);
            for (int i = 0; i < dataSet.Tables[0].Rows.Count; i++)
            {
                if (str.Length > 0)
                {
                    str = str + "|" + dataSet.Tables[0].Rows[i][0].ToString();
                }
                else
                {
                    str = dataSet.Tables[0].Rows[i][0].ToString();
                }
            }
            return str;
        }

        protected static void Open()
        {
            if (Connection == null)
            {
                Connection = new SqlConnection(ConnectionString);
            }
            if (Connection.State.Equals(ConnectionState.Closed))
            {
                Connection.Open();
            }
        }

        private static void PrepareCommand(SqlCommand cmd, SqlConnection conn, SqlTransaction trans, string cmdText, SqlParameter[] cmdParms)
        {
            try
            {
                if (conn.State != ConnectionState.Open)
                {
                    conn.Open();
                }
                cmd.Connection = conn;
                cmd.CommandText = cmdText;
                if (trans != null)
                {
                    cmd.Transaction = trans;
                }
                cmd.CommandType = CommandType.Text;
                if (cmdParms != null)
                {
                    foreach (SqlParameter parameter in cmdParms)
                    {
                        if (((parameter.Direction == ParameterDirection.InputOutput) || (parameter.Direction == ParameterDirection.Input)) && (parameter.Value == null))
                        {
                            parameter.Value = DBNull.Value;
                        }
                        cmd.Parameters.Add(parameter);
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static DataSet Query(string SQLString, params SqlParameter[] cmdParms)
        {
            DataSet set2;
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                SqlCommand cmd = new SqlCommand();
                PrepareCommand(cmd, connection, null, SQLString, cmdParms);
                using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                {
                    DataSet dataSet = new DataSet();
                    try
                    {
                        adapter.Fill(dataSet, "ds");
                        cmd.Parameters.Clear();
                    }
                    catch (SqlException)
                    {
                    }
                    connection.Close();
                    set2 = dataSet;
                }
            }
            return set2;
        }
    }
}

