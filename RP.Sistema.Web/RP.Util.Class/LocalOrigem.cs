using System.Collections.Generic;
using System.Configuration;
using System.Xml;
using System.Linq;
using System;
using System.Web.Mvc;

namespace RP.Util
{
    public class LocalOrigem
    {
        public int Id { get; set; }
        public string Nome { get; set; }
    }


    public static class LocalHelper
    {
        private static List<LocalOrigem> _list;

        public static List<LocalOrigem> Carregar(string path)
        {

            if (_list == null)
            {
                _list = new List<LocalOrigem>();

                XmlDocument document = new XmlDocument();
                document.Load(path);

                foreach (XmlElement element in document["locais"].ChildNodes)
                {
                    _list.Add(new LocalOrigem
                    {
                        Id = Convert.ToInt32(element.Attributes["id"].Value),
                        Nome = element.InnerText
                    });
                }
            }

            return _list;
        }

        public static LocalOrigem CarregarPorId(int idLocal, string path)
        {
            return Carregar(path).FirstOrDefault(e => e.Id == idLocal);
        }
    }

}
