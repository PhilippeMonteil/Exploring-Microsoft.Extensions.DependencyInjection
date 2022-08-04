
using Microsoft.Extensions.DependencyInjection;

using TestClasses;

namespace UnitTests.Tests
{

    public class IServiceScopeFactoryTests : BaseClass
    {

        #region --- Test0

        // Utilisation d'un IServiceScope existant et de son .ServiceProvider
        // pour en obtenir des services 
        void test0(IServiceScope scope)
        {
            InterfaceA? _pInterfaceA0 = scope.ServiceProvider.GetService<InterfaceA>();
            InterfaceA? _pInterfaceA1 = scope.ServiceProvider.GetService<InterfaceA>();
        }

        // Utilisation d'un IServiceScopeFactory pour produire localement un IServiceScope,
        // obtenir des services de son .ServiceProvider puis détruire ce IServiceScope
        // et les instances qu'il a pu produire.
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

        #endregion

        #region --- Test1

        /**
         * classe dont le constructeur nécéssite un IServiceScopeFactory 
        ***/
        class ClassTest1 : BaseClass
        {
            readonly IServiceScopeFactory serviceScopeFactory;

            public ClassTest1(IServiceScopeFactory serviceScopeFactory)
            {
                this.serviceScopeFactory = serviceScopeFactory;
            }

            public void Method0()
            {
                using (IServiceScope scope = serviceScopeFactory.CreateScope())
                {
                    InterfaceA? _interfaceA0 = scope.ServiceProvider.GetService<InterfaceA>();
                }
            }

        }

        [Fact]
        public void Test1()
        {
            ServiceProvider? services = null;
            try
            {
                this.Log($"(-)");
                services = new ServiceCollection()
                    .AddSingleton<ClassTest1>()
                    .BuildServiceProvider();

                ClassTest1? classTest1 = services.GetService<ClassTest1>();
                classTest1.Method0();

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

        #endregion

    }

}
