
using Microsoft.Extensions.DependencyInjection;

using TestClasses;

namespace UnitTests.Tests
{

    public class ServiceInstantiationTests
    {

        [Fact]
        /***
        ***/
        public void Test_ConstructorChoice()
        {
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
                    // the debug output shows : 
                    //[16]UnitTests.Tests.ServiceInstantiationTests].(Test_ConstructorChoice)'(-)'
                    //[16]UnitTests.Tests.ServiceInstantiationTests].(Test_ConstructorChoice)'_serviceProvider.GetService<ClassD>(-)'
                    //[16]ClassD[4]].(.ctor) 'interfaceA=ClassA[2] interfaceB=ClassB[3]'
                    //[16]UnitTests.Tests.ServiceInstantiationTests].(Test_ConstructorChoice)'_serviceProvider.GetService<ClassD>(+) classD=ClassD[4]'
                }

                // no constructor from ClassE can be unambiguously chosen by the container 
                // which then throws an exception
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

            }
            finally
            {
                this.Log("(+)");
                _serviceProvider?.Dispose();
            }
        }

        [Fact]
        /***
         * - Observing the destruction of the disposable instances produced by a 'root' container
         *   when disposed.
        ***/
        public void Test_DisposableImplementations0()
        {
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

            }
            finally
            {
                this.Log("_serviceProvider?.Dispose(-)");
                _serviceProvider?.Dispose();
                this.Log("_serviceProvider?.Dispose(+)");
            }
        }

        [Fact]
        /***
         * - Observing the destruction of the disposable instances produced by a 'scoped' container
         *   when disposed.
        ***/
        public void Test_DisposableImplementations1()
        {
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
        }

        [Fact]
        /***
         * - Observing the destruction of a disposable instance built with a disposable service 
         *   it disposes when disposed itself.
        ***/
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
                // as well as the DisposableClassD instance created by the container
                // to build disposableClassE.
                // disposableClassE will itself dispose the DisposableClassD instance it received
                // when constructed : this DisposableClassD instance will then be disposed twice 
                _serviceProvider?.Dispose();
                this.Log("_serviceProvider?.Dispose(+)");
            }
        }

    }

}
