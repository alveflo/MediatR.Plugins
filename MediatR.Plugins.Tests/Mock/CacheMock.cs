using MediatR.Plugins.Caching;
using System;
using System.Collections.Generic;
using System.Text;

namespace MediatR.Plugins.Tests.Mock
{
    public class CacheMock : ICacheProvider
    {
        private readonly Dictionary<string, object> _dictionary;

        public CacheMock(Dictionary<string, object> dictionary)
        {
            _dictionary = dictionary;
        }
        public void Add(string key, object obj)
        {
            _dictionary.Add(key, obj);
        }

        public object Get(string key)
        {
            return _dictionary.GetValueOrDefault(key);
        }

        public bool IsValid(string key)
        {
            return true;
        }
    }
}
