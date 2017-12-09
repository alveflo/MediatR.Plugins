using System;
using System.Collections.Generic;
using System.Text;

namespace MediatR.Plugins.Caching
{
    public interface ICachableRequest<TResponse> : IRequest<TResponse>
    {
        string GetCacheKey();
    }
}
