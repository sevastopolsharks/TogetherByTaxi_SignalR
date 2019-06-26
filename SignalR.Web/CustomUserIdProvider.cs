using Microsoft.AspNetCore.SignalR;

namespace SignalR.Web
{
    /// <summary>
    /// Реализация IUserIdProvider для работы SignalR
    /// </summary>
    public class CustomUserIdProvider : IUserIdProvider
    {
        public string GetUserId(HubConnectionContext connection)
        {
            // According to OpenId specification
            // Subject - Identifier for the End-User at the Issuer.
            var subjectClaimName = "sub";
            return connection.User?.FindFirst(subjectClaimName)?.Value;
        }
    }
}