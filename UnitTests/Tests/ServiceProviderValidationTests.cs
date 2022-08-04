
using Microsoft.Extensions.DependencyInjection;

using TestClasses;

namespace UnitTests.Tests
{

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

                // ServiceProvider : built to validate scopes
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
                 * mentions a scoped service : InterfaceA     
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

    }

}
