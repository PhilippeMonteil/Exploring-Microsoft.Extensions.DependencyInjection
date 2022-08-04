
using Microsoft.Extensions.DependencyInjection;

using TestClasses;

namespace UnitTests.Tests
{

    public class ServiceProviderCreationTests
    {

        [Fact]
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

    }

}
