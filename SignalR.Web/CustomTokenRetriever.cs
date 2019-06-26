using System;
using IdentityModel.AspNetCore.OAuth2Introspection;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;

namespace SignalR.Web
{
    /// <summary>
    /// Решение проблемы с авторизацией UI в SignalR
    /// Взято отсюда:
    /// https://stackoverflow.com/questions/50640316/aspnet-core-identityserver-4-unauthorized-401-during-websocket-handshake-wit
    /// </summary>
    public class CustomTokenRetriever
    {
        internal const string TokenItemsKey = "idsrv4:tokenvalidation:token";

        // custom token key change it to the one you use for sending the access_token to the server
        // during websocket handshake
        internal const string SignalRTokenKey = "signalr_token";

        static Func<HttpRequest, string> AuthHeaderTokenRetriever { get; set; }
        static Func<HttpRequest, string> QueryStringTokenRetriever { get; set; }

        static CustomTokenRetriever()
        {
            AuthHeaderTokenRetriever = TokenRetrieval.FromAuthorizationHeader();
            QueryStringTokenRetriever = TokenRetrieval.FromQueryString();
        }

        public static string FromHeaderAndQueryString(HttpRequest request)
        {
            var token = AuthHeaderTokenRetriever(request);

            if (string.IsNullOrEmpty(token))
            {
                token = QueryStringTokenRetriever(request);
            }

            if (string.IsNullOrEmpty(token))
            {
                token = request.HttpContext.Items[TokenItemsKey] as string;
            }

            if (string.IsNullOrEmpty(token) && request.Query.TryGetValue(SignalRTokenKey, out StringValues extract))
            {
                token = extract.ToString();
            }

            return token;
        }
    }
}