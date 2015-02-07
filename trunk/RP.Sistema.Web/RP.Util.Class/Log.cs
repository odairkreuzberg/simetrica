using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Data.Objects.DataClasses;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace RP.Util.Entity
{
    public class Log : ILog
    {
        [NotMapped]
        [ScriptIgnore]
        public string logData
        {
	        get { return MakeLogData(); }
        }

        [NotMapped]
        [ScriptIgnore]
        public string tableName
        {
            get
            {
                if (this.GetType().Name.IndexOf('_') > 0)
                {
                    return this.GetType().Name.Substring(0, this.GetType().Name.IndexOf('_'));
                }
                return this.GetType().Name;
            }
        }

        [NotMapped]
        [ScriptIgnore]
        public string keyName
        {
            get { return MakeKey(); }
        }

        private string MakeLogData()
        {
            string _result = @"<table name='{0}'><fields>{1}</fields></table>";
            string _field = @"<field name='{0}'>{1}</field>";

            Type myType = this.GetType();
            IList<PropertyInfo> props = new List<PropertyInfo>(myType.GetProperties());

            string _fields = string.Empty;
            foreach (PropertyInfo prop in props)
            {
                Type propType = prop.PropertyType;
                if (prop.PropertyType.IsGenericType &&  prop.PropertyType.GetGenericTypeDefinition() != typeof(Nullable<>))
                {
                    propType = prop.PropertyType.GetGenericArguments()[0];
                }

                if (prop.Name.ToLower() != "logdata" && prop.Name.ToLower() != "tablename" && prop.Name.ToLower() != "hashcode")
                {
                    if (propType == typeof(System.String) ||
                        propType == typeof(System.Int32) ||
                        propType == typeof(System.DateTime) ||
                        propType == typeof(System.Decimal))
                    {
                        object propValue = prop.GetValue(this, null);
                        _fields += string.Format(_field, prop.Name, propValue);
                    }
                }
            }

            return string.Format(_result, this.tableName, _fields );
        }

        private string MakeKey() 
        {
            return "123";
        }

    }
}