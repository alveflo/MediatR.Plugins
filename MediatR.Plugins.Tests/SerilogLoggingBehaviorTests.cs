using MediatR.Plugins.Serilog;
using MediatR.Plugins.Tests.Handlers;
using MediatR.Plugins.Tests.Mock;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Serilog;
using StructureMap;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace MediatR.Plugins.Tests
{
    [TestClass]
    public class SerilogLoggingBehaviorTests
    {
        private readonly IMediator _mediator;
        private readonly SerilogMock serilogMock;

        public SerilogLoggingBehaviorTests()
        {
            serilogMock = new SerilogMock();
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

                cfg.For<ILogger>().Use(serilogMock);
                cfg.For<IMediator>().Use<Mediator>();

                cfg.For(typeof(IPipelineBehavior<,>)).Add(typeof(SerilogLoggingBehavior<,>));
            });

            _mediator = container.GetInstance<IMediator>();
        }

        [TestMethod]
        public async Task SerilogLoggingBehavior_ShouldOnlyContainInfo_IfNoExceptionsWasThrown()
        {
            var test = await _mediator.Send(new Ping.Request());
            var logEntry = serilogMock.Entries.FirstOrDefault();
            Assert.AreEqual(logEntry.Severity, LogSeverity.DEBUG);
        }

        [TestMethod]
        public async Task SerilogLoggingBehavior_ShouldContainError_IfExceptionsWasThrown()
        {
            try
            {
                var test = await _mediator.Send(new Ping.Request { Value = 200 });
            }
            catch (Exception)
            {
                var logEntry = serilogMock.Entries.FirstOrDefault(x => x.Severity == LogSeverity.ERROR);
                Assert.AreEqual(logEntry.Severity, LogSeverity.ERROR);
            }
        }
    }
}
