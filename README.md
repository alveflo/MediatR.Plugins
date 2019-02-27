# MediatR.Plugins
[![Build status](https://ci.appveyor.com/api/projects/status/wiaw36sbfycn9jw6?svg=true)](https://ci.appveyor.com/project/alveflo/mediatr-plugins)

Super simple, handy plugins for [MediatR](https://github.com/jbogard/MediatR).

## Install
```
PM> Install-Package MediatR.Plugins
```
## Behaviors
- Logging behavior
  - [Log4net](https://logging.apache.org/log4net/)
  - [Serilog](https://serilog.net)
- Validation behavior (with [FluentValidation](https://github.com/JeremySkinner/FluentValidation))
- Caching behavior

## Logging
The logging behavior will log:
- Error severity:
  - If an exception was thrown in handler
- Debug severity:
  - Pre processing: `Handling {typeof(TRequest)}, traceId: {GUID} {timestamp}`
  - Post processing: `Handling {typeof(TRequest)}, traceId: {GUID} {timestamp}`

### Log4net
#### Setup
`Log4netLoggingBehavior` obviously depends on Log4net, so an `ILog` needs to be
configured in your IoC/DI.

Then simply register the `Log4netLogginBehavior` in your IoC/DI container as well.
Structuremap example:
```csharp
var container = new Container(cfg =>
{
    ...
    For<ILog>().Use(Log4netInstance);
    cfg.For(typeof(IPipelineBehavior<,>)).Add(typeof(Log4netLoggingBehavior<,>));
});
```

### Serilog
#### Setup
`SerilogLoggingBehavior` obviously depends on Serilog, so an `ILogger` needs to be
configured in your IoC/DI.

Then simply register the `SerilogLoggingBehavior` in your IoC/DI container as well.
Structuremap example:
```csharp
var container = new Container(cfg =>
{
    ...
    For<ILogger>().Use(SerilogInstance);
    cfg.For(typeof(IPipelineBehavior<,>)).Add(typeof(SerilogLoggingBehavior<,>));
});
```
## Validation
#### Setup
No additional setup is required to use the validation behavior, even though it relies on
`FluentValidation`. I choosed to design this as decoupled as posible from the users
IoC/DI framework as possible. So the only setup needed is to register the pipeline behavior:

```csharp
var container = new Container(cfg =>
{
    ...
    cfg.For(typeof(IPipelineBehavior<,>)).Add(typeof(ValidationBehavior<,>));
});
```
#### Example
A validatable request needs to implement `IValidatableRequest`:
```csharp
public class Request : IValidatableRequest<Result>
{
  public Request(int value)
  {
    Value = value;
  }

  public int Value { get; set; }
  public ValidationResult Validate() => new Validator().Validate(this);
}

public class Validator : AbstractValidator<Request>
{
    public Validator()
    {
        RuleFor(x => x.Value)
            .GreaterThan(10)
            .LessThan(20);
    }
}
```

The `IValidatableRequest` is an `IRequest` with a `Validate()` method on top of it that is
used by the pipeline behavior to do the validation, without having to do IoC/DI framework
specific scanning to figure out which validator to use.

## Caching behavior
#### Why caching responses
The reason why I felt the need of caching responses is that I was writing an application
that needs to be extremely fast performing and that my handlers that I needed to cache
was of pure I/O calculators. No external dependencies (e.g. lookups to database).

I did know for sure that given a set of parameters, I knew that the response would be
exactly the same every time it was invoked. Hence caching is safe.

#### Before you start caching responses
Before starting using a caching behavior of your request, make sure that you
know exactly *why* you want to cache, *what* you want to cache and most importantly
*how* to figure out unique caching keys. If caching keys are not generated properly,
the caching behavior is a very dangerous feature to rely on.

That being said the caching behavior is safely implemented and you are fully responsible
and in control of cache validation checks, which caching provider to use etc.

#### Setup
The caching behavior pipeline depends on an `ICacheProvider` and an `ICacheLogger`.
The caching itself is not done itself in the pipeline, but are left to the user to
implement using the `ICacheProvider`. So whether you want to cache using a `Dictionary`,
Redis, distributed cache, runtime cache or whatever - its up to you.

Same goes for the ICacheLogger, just implement the `Debug` method to (preferrably) decorate
your favorite logging framework of choice.

```csharp
var container = new Container(cfg =>
{
    ...
    For<ICacheLogger>().Use(CacheLoggerInstance);
    For<ICacheProvider>().Use(CacheProviderInstance);
    cfg.For(typeof(IPipelineBehavior<,>)).Add(typeof(SerilogLoggingBehavior<,>));
});
```
#### Logging
The caching behavior pipeline will log with `Debug` severity when:
- Retrieve from cache
- Adding to cache

#### Example
A cachable request needs to be of type `ICachableRequest` which is an `IRequest`
with a `GetCacheKey()` method on top of it. This is where you should implement your
request-unique cache key generator.

```csharp
public class CachablePing
{
    public class Handler : IRequestHandler<Request, Result>
    {
        public Task<Result> Handle(Request request, CancellationToken cancellationToken)
        {
            return Task.FromResult(new Result
            {
                Value = Math.Pow(request.Value, 2),
                ProcessedTicks = DateTime.UtcNow.Ticks
            });
        }
    }

    public class Request : ICachableRequest<Result>
    {
        public double Value { get; set; }

        public string GetCacheKey()
        {
            return Value.ToString();
        }
    }

    public class Result
    {
        public double Value { get; set; }
        public long ProcessedTicks { get; set; }
    }
}
```

## License
MIT License

Copyright (c) 2017 Victor Alveflo

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
