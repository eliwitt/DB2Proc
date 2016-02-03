#define ObjReport
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IBM.Data.DB2.iSeries;
using System.Diagnostics;

namespace DB2Proc
{
    class Program
    {
        static void Main(string[] args)
        {
            ShippingReport();
            DisReport();
            ObjReport();
        }
        /// <summary>
        /// Get a configuration string
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static String Get(string key)
        {
            return System.Configuration.ConfigurationManager.AppSettings.Get(key);
        }
        /// <summary>
        /// Call the DB2 database using the DB2 objects
        /// </summary>
        [Conditional("ShippingReport")]
        public static void ShippingReport()
        {
            using (iDB2Connection conn = new iDB2Connection(Get("iSeries")))
            {
                using (iDB2Command cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "VT2662AP.ShippingReport";
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    iDB2Parameter parm = cmd.Parameters.Add("@SHIPDATE", iDB2DbType.iDB2Decimal);
                    cmd.Parameters["@SHIPDATE"].Value = 20151219;
                    conn.Open();
                    iDB2DataReader myshipment = cmd.ExecuteReader();

                    if (myshipment.HasRows)
                    {
                        while (myshipment.Read())
                        {
                            Console.WriteLine("{0} has {1} - {2} pick {3} {4}", myshipment.GetString(0),
                                    myshipment.GetString(1), myshipment.GetString(2), myshipment.GetString(3),
                                    myshipment.GetString(8));
                        }
                    }
                    myshipment.Close();
                    conn.Close();
                }
            }
        }
        /// <summary>
        /// Call the DB2 database using the disposable object
        /// </summary>
        [Conditional("DisReport")]
        public static void DisReport()
        {
            using (DisposableFoo conn = new DisposableFoo(Get("iSeries")))
            {
                using (iDB2Command cmd = conn._conn.CreateCommand())
                {
                    cmd.CommandText = "VT2662AP.ShippingReport";
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    iDB2Parameter parm = cmd.Parameters.Add("@SHIPDATE", iDB2DbType.iDB2Decimal);
                    cmd.Parameters["@SHIPDATE"].Value = 20151219;
                    iDB2DataReader myshipment = cmd.ExecuteReader();

                    if (myshipment.HasRows)
                    {
                        while (myshipment.Read())
                        {
                            Console.WriteLine("{0} has {1} - {2} pick {3} {4}", myshipment.GetString(0),
                                    myshipment.GetString(1), myshipment.GetString(2), myshipment.GetString(3),
                                    myshipment.GetString(8));
                        }
                    }
                    myshipment.Close();
                }
            }
        }
        /// <summary>
        /// Call the DB2 database using the disposable object
        /// </summary>
        [Conditional("ObjReport")]
        public static void ObjReport()
        {
            DisposableFoo conn = new DisposableFoo(Get("iSeries"));
            using (iDB2Command cmd = conn._conn.CreateCommand())
                {
                    cmd.CommandText = "VT2662AP.ShippingReport";
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    iDB2Parameter parm = cmd.Parameters.Add("@SHIPDATE", iDB2DbType.iDB2Decimal);
                    cmd.Parameters["@SHIPDATE"].Value = 20151219;
                    iDB2DataReader myshipment = cmd.ExecuteReader();

                    if (myshipment.HasRows)
                    {
                        while (myshipment.Read())
                        {
                            Console.WriteLine("{0} has {1} - {2} pick {3} {4}", myshipment.GetString(0),
                                    myshipment.GetString(1), myshipment.GetString(2), myshipment.GetString(3),
                                    myshipment.GetString(8));
                        }
                    }
                    myshipment.Close();
                }
            //conn.Dispose();
            Console.WriteLine("This is the end");
           }
    }
    /// <summary>
    ///  DB2 Configuration Class
    /// </summary>
    [Serializable]
    public class DisposableFoo : IDisposable
    {
        public iDB2Connection _conn;

        public DisposableFoo(String connMachine)
        {
            Console.WriteLine("In the constructor");
            _conn = new iDB2Connection(connMachine);
            this._conn.Open();
        }

        private void CleanUp()
        {
            Console.WriteLine("In CleanUp");
            this._conn.Close();
            this._conn.Dispose();
        }

        ~DisposableFoo()
        {
            Console.WriteLine("In the destructor.");
            CleanUp();
            Console.WriteLine("Client did not call Dispose().");
        }

        #region IDisposable Members

        public void Dispose()
        {
            Console.WriteLine("In Dispose");
            CleanUp();
            GC.SuppressFinalize(this);
            Console.WriteLine("Finished with object");
        }

        #endregion
    }
}