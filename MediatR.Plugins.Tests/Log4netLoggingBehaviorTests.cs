using log4net;
using MediatR.Plugins.Log4net;
using MediatR.Plugins.Tests.Handlers;
using MediatR.Plugins.Tests.Mock;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StructureMap;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace MediatR.Plugins.Tests
{
    [TestClass]
    public class Log4netLoggingBehaviorTests
    {
        private readonly IMediator _mediator;
        private readonly Log4netMock log4netMock;

        public Log4netLoggingBehaviorTests()
        {
            log4netMock = new Log4netMock();
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

                cfg.For<ILog>().Use(log4netMock);
                cfg.For<IMediator>().Use<Mediator>();

                cfg.For(typeof(IPipelineBehavior<,>)).Add(typeof(Log4netLoggingBehavior<,>));
            });

            _mediator = container.GetInstance<IMediator>();
        }

        [TestMethod]
        public async Task Log4netLoggingBehavior_ShouldOnlyContainInfo_IfNoExceptionsWasThrown()
        {
            var test = await _mediator.Send(new Ping.Request());
            var logEntry = log4netMock.Entries.FirstOrDefault();
            Assert.AreEqual(logEntry.Severity, LogSeverity.DEBUG);
        }

        [TestMethod]
        public async Task Log4netLoggingBehavior_ShouldContainError_IfExceptionsWasThrown()
        {
            try
            {
                var test = await _mediator.Send(new Ping.Request { Value = 200 });
            }
            catch (Exception)
            {
                var logEntry = log4netMock.Entries.FirstOrDefault(x => x.Severity == LogSeverity.ERROR);
                Assert.AreEqual(logEntry.Severity, LogSeverity.ERROR);
            }
        }

    }
}
