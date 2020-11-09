using System.Net.Http;
using System.ServiceModel.Channels;
using System.Web;

namespace DeltaWebApi.Util
{
    public static class DeltaWebApiUtils
    {
        public static bool IsIpAddressAllowed(string ipAddr , string[] _ApiAllowedIpAddresLists)
        {
            foreach (string item in _ApiAllowedIpAddresLists)
            {
                if (item.Equals(ipAddr))
                {
                    return true;
                }
                else if (ipAddr.StartsWith(item))
                {
                    return true;
                }
            }

            return false;
        }



    }
}