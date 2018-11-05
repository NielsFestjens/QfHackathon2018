using Microsoft.AspNetCore.SignalR;

namespace Server.WebApi.SignalR
{
    public static class ClientProxyExtensions
    {
        public static ClientProxyGroup And(this IClientProxy proxy1, IClientProxy proxy2)
        {
            return new ClientProxyGroup(proxy1, proxy2);
        }
    }
}