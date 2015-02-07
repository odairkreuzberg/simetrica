using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text;

namespace RP.Util.Exception
{
    [SerializableAttribute]
    public class NotFoundException : System.Exception 
    {
        public NotFoundException(string message) : base(message) 
        {
        }
    }

    [SerializableAttribute]
    public class ForeingKeyException : System.Exception
    {
        public ForeingKeyException(string message)
            : base(message)
        {
        }
    }

    [SerializableAttribute]
    public class RPException : System.Exception
    {
        public RPException(string message)
            : base(message)
        {
        }
    }



    public class Message
    {
        private static System.Collections.Hashtable __tableNames;

        public static string Get(System.Exception ex)
        {
            if (ex != null)
            {
                if (ex is DbEntityValidationException)
                {
                    var _e = (DbEntityValidationException)ex;
                    StringBuilder sb = new StringBuilder();

                    foreach (var failure in _e.EntityValidationErrors)
                    {
                        sb.AppendFormat("{0} failed validation\n", failure.Entry.Entity.GetType());

                        foreach (var error in failure.ValidationErrors)
                        {
                            sb.AppendFormat("- {0} : {1}", error.PropertyName, error.ErrorMessage);
                            sb.AppendLine();
                        }
                    }

                    return sb.ToString();
                }
                else if (ex is OutOfMemoryException)
                {
                    return "O relatório solicitado excedeu os recursos disponíveis, adicione um filtro e tente novamente.";
                }

                if (ex.InnerException != null)
                    return RP.Util.Exception.Message.Get(ex.InnerException);

                if (ex is Npgsql.NpgsqlException)
                {
                    return ConflictMessagePOSTGRES(ex.Message);
                }
                else
                {
                    return ConflictMessageSQLSERVER(ex.Message);
                }
            }
            return string.Empty;
        }

        private static string ConflictMessageSQLSERVER(string msg)
        {
            if (msg.IndexOf("The DELETE statement conflicted") == 0)
            {
                return "O registro não pode ser removido, existem vinculos com " + GetTableNameSQLSERVER(msg);
            }
            return msg;
        }

        private static string ConflictMessagePOSTGRES(string msg)
        {
            if (msg.IndexOf("ERRO: 23503") == 0)
            {
                return "O registro não pode ser removido, existem vinculos com " + GetTableNamePOSTGRES(msg);
            }
            return msg;
        }

        private static string GetTableNameSQLSERVER(string msg)
        {
            string _result = msg.Substring(msg.IndexOf(", table") + 7);
            int _index = _result.IndexOf(", column");
            if (_index > 0)
            {
                _result = _result.Substring(0, _index);
            }

            string _name = string.Empty;
            foreach (char item in _result.ToLower())
            {
                if ("abcdefghijklmnopqrstuvxzwy.1234567890".IndexOf(item) >= 0)
                {
                    _name += item;
                }
            }

            return GetTableAlias(_name);
        }

        private static string GetTableNamePOSTGRES(string msg)
        {
            string[] _temp = msg.Split('"');
            if (_temp.Length >= 5)
            {
                string _result = _temp[5];

                string _name = string.Empty;
                foreach (char item in _result.ToLower())
                {
                    if ("abcdefghijklmnopqrstuvxzwy.1234567890_".IndexOf(item) >= 0)
                    {
                        _name += item;
                    }
                }

                return GetTableAlias(_name);
            }
            return msg;
        }

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
