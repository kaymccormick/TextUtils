using Castle.DynamicProxy;

namespace TextUtils
{
    public class ProxyHelper
    {
        private static readonly ProxyGenerator InternalProxyGenerator = new ProxyGenerator();

        public static ProxyGenerator ProxyGenerator => InternalProxyGenerator;

        public static bool IsProxyEnabled => false;
    }
}