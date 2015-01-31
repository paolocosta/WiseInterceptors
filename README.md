# WiseInterceptors

This project implements some C# interceptors.

Currently we have:

 1. A circuit breaker interceptor. 
 
 2. A cache interceptor.

 3. A method validation interceptor.

The goal is to have a complete suite of easily configurable interceptors as a starter kit for enterprise projects.

Short term goal: in order to make the code independent from the WiseInterceptor library, 
attributes will be removed and they will be substiututed by injectable interfaces. Client code will be free 
to define its own attributes and only the application's composition root will be dependent from this library