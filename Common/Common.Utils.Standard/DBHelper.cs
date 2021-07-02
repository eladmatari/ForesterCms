using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common.Utils.Standard
{
    public static class DBHelper
    {
        #region Members
        private static MySqlDatabase m_Main = null;
        private static MySqlDatabase m_Log = null;
        private static object m_SyncRoot = new object();
        #endregion

        #region Properties

        /// <summary>
        /// The following routine creates and returns an instance of SQL Database
        /// </summary>
        public static MySqlDatabase Database
        {
            get
            {
                if (m_Main == null)
                {
                    lock (m_SyncRoot)
                    {
                        if (m_Main == null)
                        {
                            // Get connection string
                            string connectionString = Config.GetAppSettings("Connection_Main");

                            m_Main = new MySqlDatabase(connectionString);
                        }
                    }
                }

                return m_Main;
            }
        }

        public static MySqlDatabase LogDatabase
        {
            get
            {
                if (m_Log == null)
                {
                    lock (m_SyncRoot)
                    {
                        if (m_Log == null)
                        {
                            // Get connection string
                            string connectionString = Config.GetAppSettings("Connection_Log");

                            m_Log = new MySqlDatabase(connectionString);
                        }
                    }
                }

                return m_Log;
            }
        }

        #endregion
    }
}
