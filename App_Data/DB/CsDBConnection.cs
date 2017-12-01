using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Web;


    public static class CsDBConnection
    {
        //To allow us access tables in  different DB 
        /*
        private static string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["SCOREDB"].ToString();
        private static SqlConnection batchConn = new SqlConnection(connectionString);
         */ 
        private static Dictionary<string, SqlTransaction> batchTransation = new Dictionary<string, SqlTransaction>();

        public static int ExecuteNonQuery(string sql,string DBName)
        {
            try
            {
                //To allow us access tables in  different DB 
                string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings[DBName].ToString();
                
                SqlConnection conn = new SqlConnection(connectionString);
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                int connInt = cmd.ExecuteNonQuery();
                conn.Close();
                conn.Dispose();
                return connInt;
            }

            catch (Exception e){
                //今日日期
                DateTime Date = DateTime.Now;
                string TodyMillisecond = Date.ToString("yyyy-MM-dd HH:mm:ss");
                string Tody = Date.ToString("yyyy-MM-dd");

                //如果此路徑沒有資料夾
                if (!Directory.Exists(HttpContext.Current.Server.MapPath("~") + "\\SQLLog"))
                {
                    //新增資料夾
                    Directory.CreateDirectory(HttpContext.Current.Server.MapPath("~") + "\\SQLLog");
                }

                //把例外狀況寫到目的檔案，若檔案存在則附加在原本內容之後(換行)

                File.AppendAllText(string.Format(HttpContext.Current.Server.MapPath("~") + "\\SQLLog\\{0}.txt", Tody), string.Format("\r\n{0} ： 查詢函式 「{1}」\r\n\t\t\t錯誤內容「{2}」\r\n\r\n", TodyMillisecond, sql, e ));
                //File.AppendAllText("J:\\Thisway_Log\\" + Tody + ".txt", "\r\n" + TodyMillisecond + "：" +  e);
                throw e;
            }

            return 0;
            
            
        }

        /// <summary>
        /// Get a DataSet for the select query string.
        /// (ex: "SELECT * FROM table1 WHERE ...")
        /// The returned DataSet will contain one table only which has no table name.
        /// </summary>
        /// <param name="strSQL"></param>
        /// <returns></returns>
        public static DataSet GetDataSet(string strSQL,string DBName)
        {

            //To allow us access tables in  different DB 
            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings[DBName].ToString();
                
            System.Data.SqlClient.SqlDataAdapter sda = new SqlDataAdapter(strSQL, connectionString);
            DataSet dsResult = new DataSet();
            try
            {
                //cmd.Connection.Open();
                sda.Fill(dsResult);

            }
            catch (Exception e)
            {
                //今日日期
                DateTime Date = DateTime.Now;
                string TodyMillisecond = Date.ToString("yyyy-MM-dd HH:mm:ss");
                string Tody = Date.ToString("yyyy-MM-dd");

                //如果此路徑沒有資料夾
                if (!Directory.Exists(HttpContext.Current.Server.MapPath("~") + "\\SQLLog"))
                {
                    //新增資料夾
                    Directory.CreateDirectory(HttpContext.Current.Server.MapPath("~") + "\\SQLLog");
                }

                //把例外狀況寫到目的檔案，若檔案存在則附加在原本內容之後(換行)

                File.AppendAllText(string.Format(HttpContext.Current.Server.MapPath("~") + "\\SQLLog\\{0}.txt", Tody), string.Format("\r\n{0} ： 查詢函式 「{1}」\r\n\t\t\t錯誤內容「{2}」\r\n\r\n", TodyMillisecond, strSQL, e));
                //File.AppendAllText("J:\\Thisway_Log\\" + Tody + ".txt", "\r\n" + TodyMillisecond + "：" +  e);
                throw e;
            }
            finally
            {
                //cmd.Connection.Close();
                sda.Dispose();
            }
            return dsResult;
        }



    #region transaction
    /*
    public static void BegineTransaction(string transactionName) {
            if (batchTransation.Count == 0)
            {
                batchConn.Open();
            }
            if (!batchTransation.ContainsKey(transactionName)) {
                SqlTransaction transaction;
                if (!batchTransation.TryGetValue(transactionName, out transaction)) {
                    batchTransation.Add(transactionName, batchConn.BeginTransaction(transactionName));
                }
            }
        }

        public static void EndTransaction(string transactionName)
        {
            SqlTransaction transaction;
            if (batchTransation.TryGetValue(transactionName, out transaction))
            {
                transaction.Dispose();
                batchTransation.Remove(transactionName);
                if (batchTransation.Count == 0) {
                    batchConn.Close();
                }
            }
        }

        public static bool CommitTransaction(string transactionName) {
            SqlTransaction transaction;
            bool ret = false;
            if (batchTransation.TryGetValue(transactionName, out transaction))
            {
                try
                {
                    transaction.Commit();
                    ret = true;
                }
                catch (Exception e) {
                    ret = false;
                    try
                    {
                        transaction.Rollback();
                    }
                    catch (SqlException ex)
                    {
                    }
                }
            }

            return ret;
        }

        public static void RollbackTransaction(string transactionName) {
            SqlTransaction transaction;
            if (batchTransation.TryGetValue(transactionName, out transaction))
            {
                transaction.Rollback();
            }
        }

        public static int BatchExecuteNonQuery(string transactionName, string sql)
        {
            int connInt = 0;
            SqlTransaction transaction;
            if (batchTransation.TryGetValue(transactionName, out transaction))
            {
                try
                {
                    SqlCommand cmd = batchConn.CreateCommand();
                    cmd.Transaction = transaction;
                    cmd.CommandText = sql;
                    connInt = cmd.ExecuteNonQuery();
                }
                catch (Exception e)
                {
                    //今日日期
                    DateTime Date = DateTime.Now;
                    string TodyMillisecond = Date.ToString("yyyy-MM-dd HH:mm:ss");
                    string Tody = Date.ToString("yyyy-MM-dd");

                    //如果此路徑沒有資料夾
                    if (!Directory.Exists(HttpContext.Current.Server.MapPath("~") + "\\SQLLog"))
                    {
                        //新增資料夾
                        Directory.CreateDirectory(HttpContext.Current.Server.MapPath("~") + "\\SQLLog");
                    }

                    //把例外狀況寫到目的檔案，若檔案存在則附加在原本內容之後(換行)

                    File.AppendAllText(string.Format(HttpContext.Current.Server.MapPath("~") + "\\SQLLog\\{0}.txt", Tody), string.Format("\r\n{0} ： 查詢函式 「{1}」\r\n\t\t\t錯誤內容「{2}」\r\n\r\n", TodyMillisecond, sql, e));
                    //File.AppendAllText("J:\\Thisway_Log\\" + Tody + ".txt", "\r\n" + TodyMillisecond + "：" +  e);
                    throw e;
                }
                
            }
            return connInt;
        }
        */
    #endregion

}

