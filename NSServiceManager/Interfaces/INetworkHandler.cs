using System;
using System.Net.Http;

namespace ServiceManager
{
    public interface INetworkHandler
    {
        HttpClientHandler GetNetworkHandler();
    }
}
