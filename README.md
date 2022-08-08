
# Exploring the Microsoft.Extensions.DependencyInjection machinery

This project proposes an exploration of the basic concepts and mechanisms of the __Microsoft.Extensions.DependencyInjection__ Dependency Injection machinery. 

This exploration is meant to be progressive, orderly, specifying the terms used, providing in the form of unit tests some as concise as possible examples illustrating the described mechanisms.

The documents used to write this document were mainly:

- [Dependency inversion](https://docs.microsoft.com/en-us/dotnet/architecture/modern-web-apps-azure/architectural-principles#dependency-inversion)

- [Dependency injection in .NET](https://docs.microsoft.com/en-us/dotnet/core/extensions/dependency-injection)

- [Tutorial: Use dependency injection in .NET](https://docs.microsoft.com/en-us/dotnet/core/extensions/dependency-injection-usage)

- [Dependency injection guidelines](https://docs.microsoft.com/en-us/dotnet/core/extensions/dependency-injection-guidelines)

## 1) Implementation of a 'Dependency Injection Container

### 1.1) ServiceDescriptor class, IServiceCollection and IServiceProvider interfaces

The implementation of a '__Dependency Injection Container__' (aka '__DI container__', '__container__', '__ServiceProvider__') requires

- the definition of a __ServiceCollection__, a list of [__ServiceDescriptors__](https://docs.microsoft.com/en-us/dotnet/api/microsoft.extensions.dependencyinjection.servicedescriptor?view=dotnet-plat-ext-6.0) where each element defines the characteristics 
  of one of the services making the container to be produced:
	- its type
	- the type implementing it
	- the lifetime of the instances implementing it (Singleton, Transient, Scoped)

- the production of a __ServiceProvider__ from this list.

A __ServiceCollection__ takes the form of an object instance exposing the 
[IServiceCollection](https://docs.microsoft.com/en-us/dotnet/api/microsoft.extensions.dependencyinjection.iservicecollection?view=dotnet-plat-ext-6.0)
interface. 

    public interface IServiceCollection: ICollection<ServiceDescriptor>, 
                                        IEnumerable<ServiceDescriptor>, 
                                        IList<ServiceDescriptor>

A __ServiceProvider__ takes the form of an object instance exposing the [IServiceProvider](https://docs.microsoft.com/en-us/dotnet/api/system.iserviceprovider?view=net-6.0)
interface.
 
    public interface IServiceProvider
    {
        public object? GetService (Type serviceType);
    }

The Microsoft.Extensions.DependencyInjection package provides the [ServiceCollection](https://docs.microsoft.com/en-us/dotnet/api/system.web.services.description.servicecollection?view=netframework-4.8) 
and [ServiceProvider](https://docs.microsoft.com/en-us/dotnet/api/microsoft.extensions.dependencyinjection.serviceprovider?view=dotnet-plat-ext-6.0)
classes as default implementations of these interfaces. 

### 1.2) Producing a ServiceProvider from this ServiceCollection list: IServiceCollection.BuildServiceProvider

The [ServiceCollectionContainerBuilderExtensions](https://docs.microsoft.com/fr-fr/dotnet/api/microsoft.extensions.dependencyinjection.servicecollectioncontainerbuilderextensions?view=dotnet-plat-ext-6.0) 
class provides a list of __BuildServiceProvider__ extensions methods for the IServiceCollection interface:

    namespace Microsoft.Extensions.DependencyInjection
    {
        public static class ServiceCollectionContainerBuilderExtensions
        {
            public static ServiceProvider BuildServiceProvider(this IServiceCollection services);
		    ...

### 1.3) Examples: class UnitTests.ServiceProviderCreationTests

The [__UnitTests.ServiceProviderCreationTests__](https://github.com/PhilippeMonteil/Exploring-Microsoft.Extensions.DependencyInjection/blob/Temp_0608/UnitTests/Tests/ServiceProviderCreationTests.cs) 
unit test class provides examples of various ways to create a ServiceCollection and then transform it into a ServiceProvider.

These examples make use of some of the extensions provided by the [ServiceCollectionServiceExtensions](https://docs.microsoft.com/en-us/dotnet/api/microsoft.extensions.dependencyinjection.servicecollectionserviceextensions?view=dotnet-plat-ext-6.0) 
class described below:

#### Example: ServiceProviderCreationTests.Test0: 

        /***
         * - creating a ServiceCollection and then a ServiceProvider from it
         * - registering Singleton services in 2 different ways
         * - testing their resolution
         * - testing the resolution of a non registered service
        ***/
        public void Test0()
        {
            ServiceProvider? _serviceProvider = null;
            try
            {
                this.Log("(-)");

                // ServiceCollection
                IServiceCollection _collection = new ServiceCollection();

                _collection.Add(new ServiceDescriptor(typeof(InterfaceA), typeof(ClassA), ServiceLifetime.Singleton));
                _collection.AddSingleton<InterfaceB, ClassB>();

                // ServiceProvider
                _serviceProvider = _collection.BuildServiceProvider();

                // GetService / Singleton
                {
                    InterfaceA? _interfaceA0 = _serviceProvider.GetService(typeof(InterfaceA)) as InterfaceA;
                    InterfaceA? _interfaceA1 = _serviceProvider.GetService(typeof(InterfaceA)) as InterfaceA;
                    Assert.NotNull(_interfaceA0);
                    Assert.Equal(_interfaceA0, _interfaceA1);
                }
                {
                    InterfaceB? _interfaceB = _serviceProvider.GetService<InterfaceB>();
                    Assert.NotNull(_interfaceB);
                }

                // GetService / unregistered service
                {
                    ClassD? _classC = _serviceProvider.GetService<ClassD>();
                    Assert.Null(_classC);
                }

            }
            finally
            {
                this.Log("_serviceProvider?.Dispose(-)");
                _serviceProvider?.Dispose();
                this.Log("_serviceProvider?.Dispose(+)");
            }

        }

#### Example: ServiceProviderCreationTests.Test1

        /***
         * A more straightforward way of creating a ServiceProvider 
        ***/
        [Fact]
        public void Test1()
        {
            ServiceProvider? _serviceProvider = null;
            try
            {
                // ServiceProvider
                _serviceProvider = new ServiceCollection()
                    .AddSingleton<InterfaceA, ClassA>()
                    .BuildServiceProvider();

                // GetService
                {
                    InterfaceA? _interfaceA = _serviceProvider.GetService<InterfaceA>();
                }

            }
            finally
            {
                _serviceProvider?.Dispose();
            }

        }

### 1.4) Organization and content of the IServiceCollection extensions offered by the ServiceCollectionDescriptorExtensions and ServiceCollectionServiceExtensions classes

The [__ServiceCollectionDescriptorExtensions__](https://docs.microsoft.com/en-us/dotnet/api/microsoft.extensions.dependencyinjection.extensions.servicecollectiondescriptorextensions?view=dotnet-plat-ext-6.0)
and [ServiceCollectionServiceExtensions](https://docs.microsoft.com/en-us/dotnet/api/microsoft.extensions.dependencyinjection.servicecollectionserviceextensions?view=dotnet-plat-ext-6.0) 
classes each expose a set of extensions to the __IServiceCollection__ interface intended to make the creation
of the content of an IServiceCollection more readable and productive.

The __ServiceCollectionDescriptorExtensions__ class exposes a set of extension methods of the __IServiceCollection__
interface concerning its list capabilities: Add, Remove, RemoveAll, Replace, TryAdd, TryAddEnumerable, TryAddScoped/Singleton/Transient ...
 
The __ServiceCollectionServiceExtensions__ class exposes a set of extension methods of the __IServiceCollection__ interface
interface, grouped by lifetime (Scope, Singleton, Transient) and by the way in which the types of services and their implementation are specified:
generic, types passed as parameters.

    public static class ServiceCollectionServiceExtensions
    {
        //
        // Scoped
        //

        // Type parameters
        public static IServiceCollection AddScoped (this IServiceCollection services, 
											Type serviceType);

        public static IServiceCollection AddScoped (this IServiceCollection services, 
											Type serviceType, 
											Func<IServiceProvider,object> implementationFactory);

        public static IServiceCollection AddScoped (this IServiceCollection services, 
											Type serviceType, 
											Type implementationType);

        // Generic methods
        public static IServiceCollection AddScoped<TService,TImplementation> (this IServiceCollection services) 
											where TService: class 
											where TImplementation: class, TService;

        public static IServiceCollection AddScoped<TService,TImplementation> (this IServiceCollection services, 
											Func<IServiceProvider,TImplementation> implementationFactory) 
											where TService: class 
											where TImplementation: class, TService;

        public static IServiceCollection AddScoped<TService> (this IServiceCollection services) 
											where TService: class;

        public static IServiceCollection AddScoped<TService> (this IServiceCollection services, 
											Func<IServiceProvider,TService> implementationFactory) 
											where TService: class;

        //
        // Singleton, Transient
        //
        // ...

## 2) Service resolution using a IServiceProvider: GetService, GetRequiredService, ...

### 2.1) The IServiceProvider interface and its ServiceProviderServiceExtensions extension class

- The __IServiceProvider__ interface exposes a single method:

    public object? GetService (Type serviceType);

- The __[ServiceProviderServiceExtensions](https://docs.microsoft.com/en-us/dotnet/api/microsoft.extensions.dependencyinjection.serviceproviderserviceextensions.getservice?view=dotnet-plat-ext-6.0)__ 
class provides many extensions to this interface as variations of GetService, GetRequiredService, GetServices, CreateScope, CreateAsyncScope:

    public static T? GetService<T>(this IServiceProvider provider);

    public static object GetRequiredService(this IServiceProvider provider, Type serviceType);

    public static T GetRequiredService<T>(this IServiceProvider provider) where T: notnull;

    public static IEnumerable<T> GetServices<T>(this IServiceProvider provider);

    public static IEnumerable<object?> GetServices(this IServiceProvider provider, Type serviceType);

    public static IServiceScope CreateScope(this IServiceProvider provider);

    public static AsyncServiceScope CreateAsyncScope(this IServiceProvider provider);

    public static AsyncServiceScope CreateAsyncScope(this IServiceScopeFactory serviceScopeFactory);

### 2.2) GetService vs GetRequiredService

The difference between a GetService method and its GetRequiredService counterpart is that:
- GetService returns null if the requested service cannot be resolved by the IServiceProvider interface.
- GetRequiredService triggers an exception in this case.

It is illustrated by the [__ServiceProviderServiceExtensionsTests.Test_GetService__](https://github.com/PhilippeMonteil/Exploring-Microsoft.Extensions.DependencyInjection/blob/Temp_0608/UnitTests/Tests/ServiceProviderServiceExtensionsTests.cs) 
unit test.

### 2.3) GetServices

#### Example: ServiceProviderServiceExtensionsTests.Test_GetServices

The following code:

        public void Test_GetServices()
        {
            ServiceProvider? _serviceProvider = null;
            try
            {
                this.Log("(-)");

                _serviceProvider = new ServiceCollection()
                    .AddSingleton<ClassA>(new ClassA())
                    .AddSingleton<ClassA>(new ClassA())
                    .AddSingleton<ClassA>(new ClassA()) // <<<<
                    .BuildServiceProvider();

                ClassA classA = _serviceProvider.GetService<ClassA>(); // <<<<
                this.Log($"classA={classA}");

                foreach (ClassA _service in _serviceProvider.GetServices<ClassA>())
                {
                    this.Log($"_service={_service}");
                }

            }
            finally
            {
                this.Log("(+)");
                _serviceProvider?.Dispose();
            }
        }

produces the following debug output:

    [16]UnitTests.Tests.ServiceProviderServiceExtensionsTests].(Test_GetServices) '(-)' 
    [16]UnitTests.Tests.ServiceProviderServiceExtensionsTests].(Test_GetServices) 'classA=ClassA[4]' 
    [16]UnitTests.Tests.ServiceProviderServiceExtensionsTests].(Test_GetServices) '_service=ClassA[2]' 
    [16]UnitTests.Tests.ServiceProviderServiceExtensionsTests].(Test_GetServices) '_service=ClassA[3]' 
    [16]UnitTests.Tests.ServiceProviderServiceExtensionsTests].(Test_GetServices) '_service=ClassA[4]' 
    [16]UnitTests.Tests.ServiceProviderServiceExtensionsTests].(Test_GetServices) '(+)' 

## 3) Service implementation lifetime: Singleton, Transient, Scope

The rules presented in this paragraph are illustrated by the [ServiceLifetimeTests](https://github.com/PhilippeMonteil/Exploring-Microsoft.Extensions.DependencyInjection/blob/Temp_0608/UnitTests/Tests/ServiceLifetimeTests.cs)
class.

### 3.1) Terminology

- resolution of a service by a (DI) container:

  Calling the .GetService method of an IServiceProvider instance
  specifying the type of the service for which you wish to obtain an implementation 

- resolution of a Singleton/Transient/Scope service by a (DI) container:

  resolution of a service that has been registered as Singleton/Transient/Scope
  by the ServiceCollection from which the implemented DI Container (IServiceProvider)
  was produced (IServiceCollection.BuildServiceProvider).

- 'root' container:

  An IServiceProvider instance produced from an instance of IServiceCollection,
  by a call to BuildServiceProvider.

- 'scoped' container:

  The IServiceProvider instance exposed by an IServiceScope instance:

    public interface IServiceScope: IDisposable
    {
        // The System.IServiceProvider used to resolve dependencies from the scope.
        IServiceProvider ServiceProvider { get; }
    }

  see below.

### 3.2) Singleton

A single instance of the type implementing a 'Singleton' service is created by a ServiceProvider on the first
resolution request (GetService).

A 'Singleton' service could also be associated to an implementation instance when the ServiceCollection
from which the ServiceProvider originates was created. 
This implementation instance will then be returned by the ServiceProvider as a resolution of the 'Singleton' service.

In both cases the resolution of a 'Singleton' service always provides the same answer.

#### Example

    ServiceProvider? _serviceProvider = null;
    try
    {
        ClassD _classD = new ClassD();

        // ServiceProvider
        _serviceProvider = new ServiceCollection()
            .AddSingleton<InterfaceA, ClassA>()
            .AddSingleton<ClassD>(_classD)
            .BuildServiceProvider();

        // Singleton
        {
            InterfaceA? _interface0 = _serviceProvider.GetService<InterfaceA>();
            Assert.NotNull(_interface0);
            //
            InterfaceA? _interface1 = _serviceProvider.GetService<InterfaceA>();
            Assert.Equal(_interface1, _interface0);
            //
            ClassD? _class0 = _serviceProvider.GetService<ClassD>();
            Assert.Equal(_class0, _classD);
        }

### 3.3) Transient

The resolution of a 'Transient' service provides each time a new instance.

#### Example

    ServiceProvider? _serviceProvider = null;
    try
    {
        // ServiceProvider
        _serviceProvider = new ServiceCollection()
            .AddTransient<InterfaceB, ClassB>()
            .BuildServiceProvider();

        // Transient
        {
            InterfaceB? _interface0 = _serviceProvider.GetService<InterfaceB>();
            Assert.NotNull(_interface0);
            //
            InterfaceB? _interface1 = _serviceProvider.GetService<InterfaceB>();
            Assert.NotEqual(_interface1, _interface0);
        }

### 3.4) Scope

#### Rule: the resolution of a 'Scope' service must not be requested from a 'root' container but from a 'scoped' container.

This rule can be checked at runtime or not by a ServiceProvider depending on how it was produced (see below).

#### Example

    bool validateScopes = true; // <<<

    ServiceProvider? _serviceProvider = null;
    try
    {
        ServiceProviderOptions serviceProviderOptions = new ServiceProviderOptions()
        {
            ValidateScopes = validateScopes, // <<<
        };

        ClassD _classD = new ClassD();

        // ServiceProvider
        _serviceProvider = new ServiceCollection()
            .AddScoped<ClassC>()
            .BuildServiceProvider(options: serviceProviderOptions);

        // Scoped, out of a scope: should not be done
        // If _serviceProvider was built (BuildServiceProvider) with 'options.ValidateScopes=true' 
        // an exception is thrown
        {
            bool _thrown = false;
            try
            {
                ClassC? _class0 = _serviceProvider.GetService<ClassC>();
                ClassC? _class1 = _serviceProvider.GetService<ClassC>();
                Assert.Equal(_class0, _class1);
            }
            catch (Exception E)
            {
                _thrown = true; // <<<
                this.Log(E);
            }
            Assert.Equal(_thrown, validateScopes); // <<<
        }

## 4) Choice by a container of the constructor of the type implementing a service

The resolution of a service by a container can imply the creation of an instance of the type implementing this service.
This is the case, among others, during the first resolution of a 'Singleton' service or during each resolution of a 'Transient' service. 

It is possible that the class to be instantiated exposes several constructors: which one does a container choose when it
instantiates the implementing class?

A container chooses the constructor whose parameter list contains the largest number of types resolved by itself. 
It is possible that several constructors quote the same number of resolved types: 
in this case, the container does not know how to choose a constructor and therefore does not instantiate the class and throws an exception.

The [__ServiceInstantiationTests.Test_ConstructorChoice__](https://github.com/PhilippeMonteil/Exploring-Microsoft.Extensions.DependencyInjection/blob/Temp_0608/UnitTests/Tests/ServiceInstantiationTests.cs) test illustrates these mechanisms.

### Example

    class ClassD: BaseClass
    {

        public ClassD()
        {
            this.Log("");
        }

        public ClassD(InterfaceA interfaceA)
        {
            this.Log($"interfaceA={interfaceA}");
        }

        public ClassD(InterfaceA interfaceA, InterfaceB interfaceB)
        {
            this.Log($"interfaceA={interfaceA} interfaceB={interfaceB}");
        }

    }

    class ClassE: BaseClass
    {

        public ClassE(InterfaceA interfaceA, InterfaceB interfaceB)
        {
            this.Log("");
        }

        public ClassE(InterfaceA interfaceA, InterfaceC interfaceC)
        {
            this.Log("");
        }

    }

    ServiceProvider? _serviceProvider = null;
    try
    {
        this.Log("(-)");

        // ServiceProvider
        _serviceProvider = new ServiceCollection()
            .AddTransient<InterfaceA, ClassA>()
            .AddTransient<InterfaceB, ClassB>()
            .AddTransient<InterfaceC, ClassC>()
            .AddTransient<ClassD>()
            .AddTransient<ClassE>()
            .BuildServiceProvider();

        {
            this.Log("_serviceProvider.GetService<ClassD>(-)");
            ClassD? classD = _serviceProvider.GetService<ClassD>();
            this.Log($"_serviceProvider.GetService<ClassD>(+) classD={classD}");
            // the debug output shows: 
            //[16]UnitTests.Tests.ServiceInstantiationTests].(Test_ConstructorChoice)'(-)'
            //[16]UnitTests.Tests.ServiceInstantiationTests].(Test_ConstructorChoice)'_serviceProvider.GetService<ClassD>(-)'
            //[16]ClassD[4]].(.ctor) 'interfaceA=ClassA[2] interfaceB=ClassB[3]'
            //[16]UnitTests.Tests.ServiceInstantiationTests].(Test_ConstructorChoice)'_serviceProvider.GetService<ClassD>(+) classD=ClassD[4]'
        }

        // no constructor from ClassE can be unambiguously chosen by the container which then throws an exception
        {
            bool _thrown = false;
            try
            {
                this.Log("_serviceProvider.GetService<ClassE>(-)");
                ClassE? classE = _serviceProvider.GetService<ClassE>();
                this.Log($"_serviceProvider.GetService<ClassE>(+) classE={classE}");
            }
            catch (Exception E)
            {
                _thrown = true;
                this.Log(E);
            }
            Assert.True(_thrown);
        }

## 5) Registering and destroying disposable instances generated by a container

As mentioned before, the resolution of a service by a container can imply the creation of an instance 
of the type implementing this service.

These instances may be registered by the container in an internal list to ensure
their Singleton or Scope character, but also to explicitly destroy the 'disposable' instances 
(exposing the IDisposable interface) it creates, regardless of their Singleton/Transient/Scoped lifetime,
when it is destroyed.

The ServiceProvider class is disposable: 

    public sealed class ServiceProvider: IServiceProvider, IDisposable, IAsyncDisposable

The IServiceScope interface is disposable: 

    public interface IServiceScope: IDisposable

A 'root' container is explicitly 'disposable'. 

A 'scoped' container is destroyed when its scope is destroyed.

The explicit destruction of the 'disposable' instances produced and listed by a container occurs 
when the container is 'disposed'.

### Examples

#### 'root' container

    ServiceProvider? _serviceProvider = null;
    try
    {
        this.Log("(-)");

        // ServiceProvider
        _serviceProvider = new ServiceCollection()
            .AddTransient<DisposableClassA>()
            .BuildServiceProvider();

        DisposableClassA disposableClassA = _serviceProvider.GetService<DisposableClassA>();

        this.Log($"disposableClassA={disposableClassA}");
        // produces the debug output:
        //[16]UnitTests.Tests.ServiceInstantiationTests].(Test_DisposableImplementations0) 'disposableClassA=DisposableClassA[2]' 

    }
    finally
    {
        this.Log("_serviceProvider?.Dispose(-)");
        _serviceProvider?.Dispose(); // destruction de disposableClassA
        this.Log("_serviceProvider?.Dispose(+)");
        // produces the debug output:
        //[16]UnitTests.Tests.ServiceInstantiationTests].(Test_DisposableImplementations0) '_serviceProvider?.Dispose(-)' 
        //[16]DisposableClassA[2]].(dispose) 'disposing=True' 
        //[16]UnitTests.Tests.ServiceInstantiationTests].(Test_DisposableImplementations0) '_serviceProvider?.Dispose(+)' 
    }

#### 'scoped' container 

The following code:

    ServiceProvider? _serviceProvider = null;
    try
    {
        this.Log("(-)");

        // ServiceProvider
        _serviceProvider = new ServiceCollection()
            .AddSingleton<DisposableClassA>()
            .AddTransient<DisposableClassB>()
            .AddScoped<DisposableClassC>()
            .BuildServiceProvider();

        // Creating a IServiceScope, making sure it is disposed when used
        using (IServiceScope scope = _serviceProvider.CreateScope())
        {
            DisposableClassA? _disposableClassA = _serviceProvider.GetService<DisposableClassA>();
            this.Log($"_disposableClassA={_disposableClassA}");

            DisposableClassB? _disposableClassB = _serviceProvider.GetService<DisposableClassB>();
            this.Log($"_disposableClassB={_disposableClassB}");

            DisposableClassC? _disposableClassC = _serviceProvider.GetService<DisposableClassC>();
            this.Log($"_disposableClassC={_disposableClassC}");
        }

    }
    finally
    {
        this.Log("_serviceProvider?.Dispose(-)");
        _serviceProvider?.Dispose();
        this.Log("_serviceProvider?.Dispose(+)");
    }

produces the following debug output:

    [16]UnitTests.Tests.ServiceInstantiationTests].(Test_DisposableImplementations1) '(-)' 
    [16]UnitTests.Tests.ServiceInstantiationTests].(Test_DisposableImplementations1) '_disposableClassA=DisposableClassA[2]' 
    [16]UnitTests.Tests.ServiceInstantiationTests].(Test_DisposableImplementations1) '_disposableClassB=DisposableClassB[3]' 
    [16]UnitTests.Tests.ServiceInstantiationTests].(Test_DisposableImplementations1) '_disposableClassC=DisposableClassC[4]' 
    [16]UnitTests.Tests.ServiceInstantiationTests].(Test_DisposableImplementations1) '_serviceProvider?.Dispose(-)' 
    [16]DisposableClassC[4]].(dispose) 'disposing=True' 
    [16]DisposableClassB[3]].(dispose) 'disposing=True' 
    [16]DisposableClassA[2]].(dispose) 'disposing=True' 
    [16]UnitTests.Tests.ServiceInstantiationTests].(Test_DisposableImplementations1) '_serviceProvider?.Dispose(+)' 

#### Note: The 'non disposable' Transient instances produced by a container are not listed, they are released by the Garbage Collector.

#### Some important consequences of this operation:

- a 'root' container resolving Singleton or Transient services as 'disposable' instances can be a source of memory leakage, 
  especially for Transients since they won't be disposed until the container is itself disposed.

- this is also true of Transient and Scope services resolved in the form of 'disposable' instances
  by a 'scoped' container, but this container will be destroyed at the same time as and by its parent scope, 
  which is supposed to happen quickly. 

- services resolved as disposable instances should not be disposed by the client of the container that issued them:
  it will be done by the container itself.

This last point should encourage to avoid storing as class members the references of resolved services exposing the IDisposable interface.

#### Example

    class DisposableClassA: Disposable0, IDisposableA
    {
    }

    /***
     * - an instance of DisposableClassE is built with an IDisposableA reference its stores in its _disposableA member.
     * - holding disposable members, DisposableClassE should be disposable itself and handle the destruction of its members when disposed.
    ***/
    class DisposableClassE: Disposable0
    {
        IDisposableA? _disposableA;

        public DisposableClassE(IDisposableA disposableA)
        {
            this._disposableA = disposableA;
        }

        protected override void doDispose(bool disposing)
        {
            try
            {
                this.Log($"(-) disposing={disposing} _disposableA={_disposableA}");
                if (disposing)
                {
                    _disposableA?.Dispose();
                }
                this.Log($"(+) disposableA={_disposableA}");
            }
            finally
            {
                base.doDispose(disposing);
            }
        }

    }

    public void Test_DisposableService0()
    {
        ServiceProvider? _serviceProvider = null;
        try
        {
            this.Log("(-)");

            // ServiceProvider
            _serviceProvider = new ServiceCollection()
                .AddTransient<IDisposableA, DisposableClassD>()
                .AddTransient<DisposableClassE>()
                .BuildServiceProvider();

            DisposableClassE disposableClassE = _serviceProvider.GetService<DisposableClassE>();
            this.Log($"disposableClassE={disposableClassE}");

        }
        finally
        {
            this.Log("_serviceProvider?.Dispose(-)");
            // This call will dispose the DisposableClassE disposableClassE instance 
            // as well as the DisposableClassD instance created by the container to build disposableClassE.
            // disposableClassE will itself dispose the DisposableClassD instance it received when constructed: 
            // this DisposableClassD instance will then be disposed twice ...
            _serviceProvider?.Dispose();
            this.Log("_serviceProvider?.Dispose(+)");
        }
    }

Test_DisposableService0 produces the following debug output:

    [16]UnitTests.Tests.ServiceInstantiationTests].(Test_DisposableService0) '(-)' 
    [16]UnitTests.Tests.ServiceInstantiationTests].(Test_DisposableService0) 'disposableClassE=DisposableClassE[3]' 
    [16]UnitTests.Tests.ServiceInstantiationTests].(Test_DisposableService0) '_serviceProvider?.Dispose(-)' 
    [16]DisposableClassE[3]].(dispose) 'disposing=True' 
    [16]DisposableClassE[3]].(doDispose) '(-) disposing=True _disposableA=DisposableClassD[2]' 
    [16]DisposableClassA[2]].(dispose) 'disposing=True' 
    [16]DisposableClassE[3]].(doDispose) '(+) disposableA=DisposableClassD[2]' 
    [16]DisposableClassA[2]].(dispose) 'disposing=True ALREADY DISPOSED' <<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<
    [16]UnitTests.Tests.ServiceInstantiationTests].(Test_DisposableService0) '_serviceProvider?.Dispose(+)' 
s
## 6) Open generics services

A container can register and resolve 'open generics services'.

### Example

    class UnitTests.Tests.GenericServicesTests
    {
    
        public void Test_GenericService0()
        {
            ServiceProvider services = null;
            try
            {
                services = new ServiceCollection()
                    .AddScoped(typeof(InterfaceF<>), typeof(ClassF<>))
                    .BuildServiceProvider();

                InterfaceF<int>? _instance0 = services.GetService<InterfaceF<int>>();
                Assert.NotNull(_instance0);

                InterfaceF<string>? _instance1 = services.GetService<InterfaceF<string>>();
                Assert.NotNull(_instance1);

            }
            finally
            {
                services?.Dispose();
            }
        }

## 7) Validation capabilities of a container

### 7.1) Preventing the resolution of scoped services out of a scope: 

#### Example: ServiceProviderValidationTests.TestScopeValidation0

      public class ServiceProviderValidationTests
      {

        /***
         * - Testing the ability of a correctly configured container to validate scopes, that is to prevent the resolution
         *   of scoped services out of a scope.
        ***/
        [Fact]
        public void TestScopeValidation0()
        {
            ServiceProvider? _serviceProvider = null;
            try
            {
                this.Log("(-)");

                // ServiceProvider: built to validate scopes
                _serviceProvider = new ServiceCollection()
                    .AddScoped<ClassA>()
                    .BuildServiceProvider(validateScopes: true);

                // Trying to resolve a Scope service out of a scope
                {
                    bool _exception = false;
                    try
                    {
                        ClassA classA = _serviceProvider.GetService<ClassA>();
                    }
                    catch (Exception e)
                    {
                        _exception = true;
                        this.Log(e);
                    }
                    Assert.True(_exception);
                }

                // Correctly resolving a Scope service using an IServiceScope scope
                using (IServiceScope scope = _serviceProvider.CreateScope())
                {
                    bool _exception = false;
                    try
                    {
                        ClassA classA = scope.ServiceProvider.GetService<ClassA>();
                        Assert.NotNull(classA);
                    }
                    catch (Exception e)
                    {
                        _exception = true;
                        this.Log(e);
                    }
                    Assert.False(_exception);
                }

            }
            finally
            {
                this.Log("(+)");
                _serviceProvider?.Dispose();
            }
        }


### 7.2) Preventing the resolution of a Singleton service depending on Scope services.

#### Example: ServiceProviderValidationTests.TestScopeValidation1

    public class ServiceProviderValidationTests
    {

        /***
         * - Testing the ability of a correctly configured container to prevent the resolution of 
         *   a singleton service depending on scoped services.
        ***/
        [Fact]
        public void TestScopeValidation1()
        {
            ServiceProvider? _serviceProvider = null;
            try
            {
                this.Log("(-)");

                /**
                 * The ClassE constructor to be invoked to resolve it:
                 *      public ClassE(InterfaceA interfaceA)
                 * mentions a scoped service: InterfaceA     
                **/
                // ServiceProvider
                _serviceProvider = new ServiceCollection()
                    .AddSingleton<ClassE>()
                    .AddScoped<InterfaceA, ClassA>()
                    .BuildServiceProvider(validateScopes: true);

                {
                    bool _exception = false;
                    try
                    {
                        ClassE classE = _serviceProvider.GetService<ClassE>();
                    }
                    catch (Exception e)
                    {
                        _exception = true;
                        this.Log(e);
                    }
                    Assert.True(_exception);
                }

            }
            finally
            {
                this.Log("(+)");
                _serviceProvider?.Dispose();
            }
        }

### 7.3) Checking the completeness of the description of the services meant to build a container

#### Example: ServiceProviderValidationTests.TestScopeValidation2

    public class ServiceProviderValidationTests
    {

        /***
         * - Checking the completeness of the description of the services meant 
         *   to build a container. 
        ***/
        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void TestScopeValidation2(bool validateOnBuild)
        {
            ServiceProvider? _serviceProvider = null;
            try
            {
                this.Log($"(-) validateOnBuild={validateOnBuild}");

                /**
                 * The ClassE constructor to be invoked to resolve it is:
                 *      public ClassE(InterfaceA interfaceA)
                 * it requires the resolution of the InterfaceA service    
                **/
                {
                    bool _exception = false;
                    try
                    {
                        // ServiceProvider
                        _serviceProvider = new ServiceCollection()
                            .AddSingleton<ClassE>()
                            //.AddTransient<InterfaceA, ClassA>() << missing service declaration
                            .BuildServiceProvider(new ServiceProviderOptions()
                            {
                                ValidateOnBuild = validateOnBuild
                            });

                        // _serviceProvider was built, but it is not able to resolve InterfaceA
                        // and therefore ClassE
                        bool __exception = false;
                        try
                        {
                            ClassE classE = _serviceProvider.GetService<ClassE>();
                        }
                        catch (Exception e)
                        {
                            __exception = true;
                            this.Log(e);
                        }
                        Assert.True(__exception);

                    }
                    catch (Exception e)
                    {
                        _exception = true;
                    }
                    finally
                    {
                        Assert.Equal(_exception, validateOnBuild);
                    }
                }

            }
            finally
            {
                this.Log("(+)");
                _serviceProvider?.Dispose();
            }
        }

## 8) the ActivatorUtilities class 

The [ActivatorUtilities](https://docs.microsoft.com/en-us/dotnet/api/microsoft.extensions.dependencyinjection.activatorutilities?view=dotnet-plat-ext-5.0) class allows to create instances of classes not resolved by a container
but whose constructor requires arguments that can be resolved by this container.  

This excellent article explains how: [Activator utilities: activate anything!](https://docs.microsoft.com/en-us/dotnet/api/microsoft.extensions.dependencyinjection.activatorutilities?view=dotnet-plat-ext-5.0)

## 9) The IServiceScopeFactory interface

The [IServiceScopeFactory](https://docs.microsoft.com/en-us/dotnet/api/microsoft.extensions.dependencyinjection.iservicescopefactory?view=dotnet-plat-ext-6.0) 
interface exposes only one method:

    public Microsoft.Extensions.DependencyInjection.IServiceScope CreateScope ();

It is registered as a Singleton service by a container and can be resolved and used to produce IServiceScope scopes.

### Example: IServiceScopeFactoryTests.Test0

        // Using an existing IServiceScope and its .ServiceProvider to resolve services
        void test0(IServiceScope scope)
        {
            InterfaceA? _pInterfaceA0 = scope.ServiceProvider.GetService<InterfaceA>();
            InterfaceA? _pInterfaceA1 = scope.ServiceProvider.GetService<InterfaceA>();
        }

        // Using an IServiceScopeFactory to produce a local IServiceScope, 
        // use its .ServiceProvider to resolve services and finally destroy that IServiceScope
        // and therefore the instances it has produced
        void test1(IServiceScopeFactory serviceScopeFactory)
        {
            using (IServiceScope scope = serviceScopeFactory.CreateScope())
            {
                InterfaceA? _interfaceA0 = scope.ServiceProvider.GetService<InterfaceA>();
            }
        }

        [Fact]
        public void Test0()
        {
            ServiceProvider services = null;
            try
            {
                this.Log($"(-)");
                services = new ServiceCollection()
                    .AddScoped<InterfaceA, ClassA>()
                    .BuildServiceProvider(validateScopes: true);

                using (IServiceScope scope = services.CreateScope())
                {
                    test0(scope);
                }

                test1(services.GetService<IServiceScopeFactory>());

            }
            catch (Exception E)
            {
                this.Log(E);
            }
            finally
            {
                this.Log("services?.Dispose(-)");
                services?.Dispose();
                this.Log("services?.Dispose(+)");
            }
        }

