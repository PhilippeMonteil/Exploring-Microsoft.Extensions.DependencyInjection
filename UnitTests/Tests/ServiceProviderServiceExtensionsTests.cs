
using Microsoft.Extensions.DependencyInjection;

using TestClasses;

namespace UnitTests.Tests
{

    public class ServiceProviderServiceExtensionsTests
    {

        [Fact]
        public void Test_GetService()
        {
            ServiceProvider? _serviceProvider = null;
            try
            {
                this.Log("(-)");

                // ServiceProvider
                _serviceProvider = new ServiceCollection()
                    .AddSingleton<ClassA>()
                    .BuildServiceProvider();

                {
                    ClassA classA = _serviceProvider.GetService<ClassA>();
                    Assert.NotNull(classA);
                }

                {
                    ClassB classB = _serviceProvider.GetService<ClassB>();
                    Assert.Null(classB);
                }

                {
                    bool _exception = false;
                    try
                    {
                        ClassB classB = _serviceProvider.GetRequiredService<ClassB>();
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

        [Fact]
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

    }

}
