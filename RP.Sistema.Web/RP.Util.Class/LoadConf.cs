using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace RP.Util.Class
{
    public class LoadConf
    {
        private static System.Collections.Hashtable __tableNames;

        public static string GetTableAlias(string tableName)
        {
            if (__tableNames == null)
            {
                DataSet _data = new DataSet();
                _data.ReadXml(System.Configuration.ConfigurationManager.AppSettings["PathXMLAlias"]);

                __tableNames = new System.Collections.Hashtable();
                foreach (DataRow row in _data.Tables[0].Rows)
                {
                    if (!__tableNames.ContainsKey(row["name"].ToString().ToLower().Trim()))
                    {
                        __tableNames.Add(row["name"].ToString().ToLower().Trim(), row["alias"].ToString());
                    }
                }
            }


            if (__tableNames.ContainsKey(tableName))
            {
                return __tableNames[tableName].ToString();
            }

            return tableName;
        }


    }
}
