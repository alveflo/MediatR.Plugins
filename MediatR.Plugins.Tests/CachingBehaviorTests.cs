using MediatR.Plugins.Caching;
using MediatR.Plugins.Tests.Handlers;
using MediatR.Plugins.Tests.Mock;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StructureMap;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Serilog;

namespace MediatR.Plugins.Tests
{
    [TestClass]
    public class CachingBehaviorTests
    {
        private readonly IMediator _mediator;
        private readonly CacheMock _cacheMock;
        private readonly Dictionary<string, object> _dictonary;

        public CachingBehaviorTests()
        {
            _dictonary = new Dictionary<string, object>();
            _cacheMock = new CacheMock(_dictonary);
            var container = new Container(cfg =>
            {
                cfg.Scan(scanner =>
                {
                    scanner.AssemblyContainingType(typeof(Ping));
                    scanner.IncludeNamespaceContainingType<Ping>();
                    scanner.WithDefaultConventions();
                    scanner.AddAllTypesOf(typeof(IRequestHandler<,>));
                });
                cfg.For<SingleInstanceFactory>().Use<SingleInstanceFactory>(ctx => t => ctx.GetInstance(t));
                cfg.For<MultiInstanceFactory>().Use<MultiInstanceFactory>(ctx => t => ctx.GetAllInstances(t));

                cfg.For<ICacheLogger>().Use(new CacheLogger());
                cfg.For<ICacheProvider>().Use(_cacheMock);
                cfg.For<IMediator>().Use<Mediator>();

                cfg.For(typeof(IPipelineBehavior<,>)).Add(typeof(CachingBehavior<,>));
            });

            _mediator = container.GetInstance<IMediator>();
        }

        [TestMethod]
        public async Task CachingBehavior_ShouldCacheRequest_IfRequestIsCachable()
        {
            var test = await _mediator.Send(new CachablePing.Request());
            Assert.AreEqual(_dictonary.FirstOrDefault().Value, test);
        }

        [TestMethod]
        public async Task CachingBehavior_ShouldOnlyProcessRequestOnce_IfRequestIsCachable()
        {
            var test = await _mediator.Send(new CachablePing.Request { Value = 10 });
            var ticks = test.ProcessedTicks;

            var responses = new List<CachablePing.Result>();
            for (int i = 100 - 1; i >= 0; i--)
            {
                responses.Add(
                    await _mediator.Send(new CachablePing.Request { Value = 10 }));
            }
            Assert.AreEqual(_dictonary.Count, 1);

            foreach (var entry in responses)
            {
                Assert.AreEqual(ticks, entry.ProcessedTicks);
            }
        }
    }
}
