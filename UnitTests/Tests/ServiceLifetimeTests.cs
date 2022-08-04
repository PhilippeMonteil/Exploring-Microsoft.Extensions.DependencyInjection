
using Microsoft.Extensions.DependencyInjection;

using TestClasses;

namespace UnitTests.Tests
{

    public class ServiceLifetimeTests
    {

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        /***
         * Testing Singleton, Transient, Scoped services
        ***/
        public void Test0(bool validateScopes)
        {
            ServiceProvider? _serviceProvider = null;
            try
            {
                ServiceProviderOptions serviceProviderOptions = new ServiceProviderOptions()
                {
                    ValidateScopes = validateScopes,
                };

                ClassD _classD = new ClassD();

                // ServiceProvider
                _serviceProvider = new ServiceCollection()
                    .AddSingleton<InterfaceA, ClassA>()
                    .AddTransient<InterfaceB, ClassB>()
                    .AddScoped<ClassD>()
                    .AddSingleton<ClassD>(_classD)
                    .BuildServiceProvider(options: serviceProviderOptions);

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

                // Transient
                {
                    InterfaceB? _interface0 = _serviceProvider.GetService<InterfaceB>();
                    Assert.NotNull(_interface0);
                    //
                    InterfaceB? _interface1 = _serviceProvider.GetService<InterfaceB>();
                    Assert.NotEqual(_interface1, _interface0);
                }

                // Scoped, out of a scope : should not be done
                // If _serviceProvider was built (BuildServiceProvider) with 'options.ValidateScopes=true' 
                // an exception is thrown
                {
                    bool _thrown = false;
                    try
                    {
                        ClassD? _class0 = _serviceProvider.GetService<ClassD>();
                        ClassD? _class1 = _serviceProvider.GetService<ClassD>();
                        Assert.Equal(_class0, _class1);
                    }
                    catch (Exception E)
                    {
                        _thrown = true;
                        this.Log(E);
                    }
                    Assert.Equal(_thrown, validateScopes);
                }

            }
            finally
            {
                _serviceProvider?.Dispose();
            }
        }

        [Fact]
        /***
         * - Registering and resolving a Scoped service 
         * - Resolving Singleton and Transient services from a 'scoped container'
        ***/
        public void Test1()
        {
            ServiceProvider? _serviceProvider = null;
            try
            {
                // ServiceProvider
                _serviceProvider = new ServiceCollection()
                    .AddSingleton<InterfaceA, ClassA>()
                    .AddTransient<InterfaceB, ClassB>()
                    .AddScoped<ClassD>()
                    .BuildServiceProvider();

                // a Singleton and a Transient service resolved by the 'root container' 
                InterfaceA? _interfaceA0 = _serviceProvider.GetService<InterfaceA>();
                Assert.NotNull(_interfaceA0);
                InterfaceB? _interfaceB0 = _serviceProvider.GetService<InterfaceB>();
                Assert.NotNull(_interfaceB0);

                // Creating a IServiceScope, making sure it is disposed when used
                using (IServiceScope scope = _serviceProvider.CreateScope())
                {
                    // using the scope.ServiceProvider 'scoped container', and not _serviceProvider
                    // to resolve services

                    // a Singleton and a Transient service resolved by the 'scoped container' 
                    InterfaceA? _interfaceA1 = _serviceProvider.GetService<InterfaceA>();
                    Assert.Equal(_interfaceA1, _interfaceA0);

                    InterfaceB? _interfaceB1 = _serviceProvider.GetService<InterfaceB>();
                    Assert.NotNull(_interfaceB1);
                    Assert.NotEqual(_interfaceB1, _interfaceB0);

                    ClassD? _class0 = scope.ServiceProvider.GetService<ClassD>();
                    Assert.NotNull(_class0);
                    ClassD? _class1 = scope.ServiceProvider.GetService<ClassD>();
                    // the resolution of a 'scoped' service always provides the same answer 
                    Assert.Equal(_class0, _class1);
                }

            }
            finally
            {
                _serviceProvider?.Dispose();
            }
        }

    }

}
