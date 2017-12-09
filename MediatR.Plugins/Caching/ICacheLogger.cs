using System;
using System.Collections.Generic;
using System.Text;

namespace MediatR.Plugins.Caching
{
    public interface ICacheLogger
    {
        void Debug(string message);
    }
}
