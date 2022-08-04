
using Microsoft.Extensions.DependencyInjection;

using TestClasses;

namespace UnitTests.Tests
{

    public class GenericServicesTests
    {

        [Fact]
        /***
         * - declaring a generic service and then resolving concrete services from it 
        ***/
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

    }

}
