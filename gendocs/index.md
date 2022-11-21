# NDF
The Net Delivery Framework provides common delivery framework implementations for the following.

## Logging
Provides standard logging support and directly injects the hosting container and the memeber name automatically.
Also provides standard enter and exit messaging support. 

## Managed Exception Pattern Implementation
Provides a common implementation pattern for handling exceptions in the system. This is a standard set of **Managed Exceptions**. The goal of this standard pattern is the following.
 - Raise well known exceptions that do not have any compromising data in the exception.
 - Raise exceptions of a common category so the consumer understands the type of exception that has occurred and can handle it.
 - Known a exception has been managed and can raise it without handling it. 
 - Known set of exceptions that can be transformed on technology boundries and raised to other technologies. 


 ## Library Based Dependency Injection Management
 Be able to use a **Leaf** to **Trunk** pattern for adding services to ceneral depedency injection containers. Standardized base class approach that does the following.

 - Provides parent to child library registeratin of DI registration with a central container. 
 - Provides for manual entry registeration in the container registration.
 - Provides methods for automation based registration of services. 


