using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Web;

namespace RP.Util.Class
{
    public class APIKeyAuthorizationManager
    {
        public const string APIKEY = "APIKey";
        public const string APIKEYLIST = "APIKeyList";
        private static List<Guid> _APIKeys;

        public static bool CheckAccessCore(OperationContext operationContext, List<Guid> keys)
        {
            _APIKeys = keys;
            return IsValidAPIKey(operationContext);
        }

        private static bool IsValidAPIKey(OperationContext operationContext)
        {
            string key = GetAPIKey(operationContext);
            Guid apiKey;

            // Convert the string into a Guid and validate it
            if (Guid.TryParse(key, out apiKey) && _APIKeys.Contains(apiKey))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static string GetAPIKey(OperationContext operationContext)
        {
            // Get the request message
            var request = operationContext.RequestContext.RequestMessage;

            // Get the HTTP Request
            var requestProp = (HttpRequestMessageProperty)request.Properties[HttpRequestMessageProperty.Name];

            // Get the query string
            NameValueCollection queryParams = HttpUtility.ParseQueryString(requestProp.QueryString);

            // Return the API key (if present, null if not)
            return queryParams[APIKEY];
        }
    }
}
