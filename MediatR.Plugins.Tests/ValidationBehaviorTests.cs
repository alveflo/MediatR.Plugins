using MediatR.Plugins.Tests.Handlers;
using MediatR.Plugins.Validation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StructureMap;
using System.Threading.Tasks;
using FluentValidation;

namespace MediatR.Plugins.Tests
{
    [TestClass]
    public class ValidationBehaviorTests
    {
        private readonly IMediator _mediator;

        public ValidationBehaviorTests()
        {
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
                
                cfg.For<IMediator>().Use<Mediator>();

                cfg.For(typeof(IPipelineBehavior<,>)).Add(typeof(ValidationBehavior<,>));
            });

            _mediator = container.GetInstance<IMediator>();
        }

        [TestMethod]
        public async Task ValidationBehavior_ShouldThrowValidationException_IfRequestIsNotValid()
        {
            await Assert.ThrowsExceptionAsync<ValidationException>(() =>  _mediator.Send(new ValidatablePing.Request(-1)));
        }

        [TestMethod]
        public async Task ValidationBehavior_ShouldNotThrowValidationException_IfRequestIsValid()
        {
            var response = await _mediator.Send(new ValidatablePing.Request(15));
            Assert.AreEqual(15, response.Value);
        }
    }
}
