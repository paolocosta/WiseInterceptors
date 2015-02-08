# WiseInterceptors

This project implements some C# interceptors using Castle Dynamic Proxy and AutoFac

Currently we have:

 1. A circuit breaker interceptor. 
 
 2. A cache interceptor.

 3. A method validation interceptor.

The goal is to have a complete suite of easily configurable interceptors as a starter kit for enterprise projects.

Cache interceptor  (Work in progress)
-----------------

The cache interceptor applies the cache algorithm proposed here: https://happinessdd.wordpress.com/2014/11/06/a-new-solution-for-the-cache-stampede-problem/.

It's also possible to save cached data in a persistent storage in order to be fault tolerant with database failures.

To apply a cache interceptor to a method follow this rule:

Your application must reference the Autofac library.

Inside your composition root you should call the WiseInterceptors Autofac module:

	var builder = new ContainerBuilder();
    builder.RegisterModule<InterceptorModule>();
 
 and then indicate your concrete implementation of the ICache interface
	
	builder.RegisterType<Cache>().As<ICache>();


