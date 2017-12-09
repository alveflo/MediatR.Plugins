namespace MediatR.Plugins.Caching
{
    public interface ICacheProvider
    {
        void Add(string key, object obj);
        object Get(string key);
    }
}