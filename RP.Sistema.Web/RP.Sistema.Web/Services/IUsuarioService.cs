using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Web;
using RP.Sistema.Model.Entities;
using RP.Sistema.Web.Models.Usuario;

namespace RP.Sistema.Web.Services
{
    [ServiceContract]
    public interface IUsuarioService
    {
        [OperationContract]
        //[AspNetCacheProfile("ServiceCacheServer")]
        [WebGet(ResponseFormat = WebMessageFormat.Json, UriTemplate = "/Usuarios.json")]
        List<UsuarioModel> ListarUsuariosJson();

        [OperationContract]
        //[AspNetCacheProfile("ServiceCacheServer")]
        [WebGet(ResponseFormat = WebMessageFormat.Xml, UriTemplate = "/Usuarios.xml")]
        List<UsuarioModel> ListarUsuariosXml();

        [OperationContract]
        [WebGet(ResponseFormat = WebMessageFormat.Json, UriTemplate = "/Usuario/{idUsuario}.json")]
        UsuarioModel BuscarUsuarioJson(string idUsuario);

        [OperationContract]
        [WebGet(ResponseFormat = WebMessageFormat.Xml, UriTemplate = "/Usuario/{idUsuario}.xml")]
        UsuarioModel BuscarUsuarioXml(string idUsuario);

        [OperationContract]
        [WebInvoke(Method = "PUT", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, UriTemplate = "/AtualizarListaPerfil.json")]
        void AtualizarPerfisJson(Usuario usuario);

        [OperationContract]
        [WebInvoke(Method = "PUT", RequestFormat = WebMessageFormat.Xml, ResponseFormat = WebMessageFormat.Xml, UriTemplate = "/AtualizarListaPerfil.xml")]
        void AtualizarPerfisXml(Usuario usuario);
    }
}
