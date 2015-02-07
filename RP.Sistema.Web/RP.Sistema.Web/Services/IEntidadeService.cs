using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using RP.Sistema.Model.Entities;

namespace RP.Sistema.Web.Services
{
    [ServiceContract]
    public interface IEntidadeService
    {
        [OperationContract]
        //[AspNetCacheProfile("ServiceCacheServer")]
        [WebGet(ResponseFormat = WebMessageFormat.Json, UriTemplate = "/Entidade/{idEntidade}.json")]
        Entidade BuscarEntidadeJson(string idEntidade);

        [OperationContract]
        //[AspNetCacheProfile("ServiceCacheServer")]
        [WebGet(ResponseFormat = WebMessageFormat.Xml, UriTemplate = "/Entidade/{idEntidade}.xml")]
        Entidade BuscarEntidadeXml(string idEntidade);

        [OperationContract]
        //[AspNetCacheProfile("ServiceCacheServer")]
        [WebGet(ResponseFormat = WebMessageFormat.Json, UriTemplate = "/Entidades.json")]
        List<Entidade> ListarEntidadesJson();

        [OperationContract]
        //[AspNetCacheProfile("ServiceCacheServer")]
        [WebGet(ResponseFormat = WebMessageFormat.Xml, UriTemplate = "/Entidades.xml")]
        List<Entidade> ListarEntidadesXml();
    }
}
