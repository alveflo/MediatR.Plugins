using MediatR.Plugins.Caching;

namespace MediatR.Plugins.Tests.Mock
{
    public class CacheLogger : ICacheLogger
    {
        public void Debug(string message)
        {
            return;
        }
    }
}
