

.) Pr�sentation de Exploring.Microsoft.Extensions.DependencyInjection

Ce projet propose une exploration des concepts et m�canismes de base de la machinerie d'Injection de D�pendances
de .Net Core. Cette exploration se veut progressive, ordonn�e, en pr�cisant les termes employ�s, fournissant
sous forme de tests unitaires des exemples aussi concis que possible illustrant les m�canismes pr�sent�s.

Ce document et ses exemples se trouvent dans le r�pertoire 
https://github.com/PhilippeMonteil/Exploring-Microsoft.Extensions.DependencyInjection

.) ServiceDescriptor, IServiceCollection, IServiceProvider

La mise en place d'un 'Dependency Injection Container' (aka 'DI container', 'container', 'ServiceProvider') passe par:

- la d�finition d'une liste de ServiceDescriptors, ServiceCollection, pr�cisant chacun :
	- le type du service
	- le type l'impl�mentant
	- la dur�e de vie des instances l'impl�mentant (singleton, transient, scoped)
	- ...
  https://docs.microsoft.com/en-us/dotnet/api/microsoft.extensions.dependencyinjection.servicedescriptor?view=dotnet-plat-ext-6.0

- la production d'un ServiceProvider � partir de cet ServiceCollection : BuildServiceProvider

Un ServiceCollection est une instance exposant l'interface IServiceCollection :

  public interface IServiceCollection : ICollection<ServiceDescriptor>, IEnumerable<ServiceDescriptor>, IList<ServiceDescriptor>

  https://docs.microsoft.com/en-us/dotnet/api/microsoft.extensions.dependencyinjection.iservicecollection?view=dotnet-plat-ext-6.0

Un ServiceProvider est une instance exposant l'interface IServiceProvider :

	public interface IServiceProvider
	{
		public object? GetService (Type serviceType);
	}

  https://docs.microsoft.com/en-us/dotnet/api/system.iserviceprovider?view=net-6.0

Production d'un ServiceProvider � partir de cette liste ServiceCollection : m�thode BuildServiceProvider

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionContainerBuilderExtensions
    {
        public static ServiceProvider BuildServiceProvider(this IServiceCollection services);
		...

.) classes ServiceCollection et ServiceProvider

  Microsoft.Extensions.DependencyInjection fournit les classes ServiceCollection et ServiceProvider impl�mentant respectivement
  les interfaces IServiceCollection et IServiceProvider.

  https://docs.microsoft.com/en-us/dotnet/api/system.web.services.description.servicecollection?view=netframework-4.8
  https://docs.microsoft.com/en-us/dotnet/api/microsoft.extensions.dependencyinjection.serviceprovider?view=dotnet-plat-ext-6.0

.) production d'un ServiceCollection puis d'un ServiceProvider

- La classe UnitTests.ServiceProviderCreationTests fournit des exemples de divers modes de cr�ation d'un ServiceCollection
  puis de sa transformation en ServiceProvider.

  Ces modes de cr�ation mettent en oeuvre certaines des extensions fournies par la classe ServiceCollectionServiceExtensions

    https://docs.microsoft.com/en-us/dotnet/api/microsoft.extensions.dependencyinjection.servicecollectionserviceextensions?view=dotnet-plat-ext-6.0

- ServiceProviderCreationTests.Test0 : 

        public static void Test0()
        {
            ServiceProvider? _serviceProvider = null;
            try
            {
                // ServiceCollection
                IServiceCollection _collection = new ServiceCollection();

                _collection.Add(new ServiceDescriptor(typeof(InterfaceA), typeof(ClassA), ServiceLifetime.Singleton));
                _collection.AddScoped<InterfaceB, ClassB>();

                // BuildServiceProvider
                _serviceProvider = _collection.BuildServiceProvider();

                // GetService
                {
                    InterfaceA? _interfaceA = _serviceProvider.GetService(typeof(InterfaceA)) as InterfaceA;
                }
                {
                    InterfaceB? _interfaceB = _serviceProvider.GetService<InterfaceB>();
                }

            }
            finally
            {
                _serviceProvider?.Dispose();
            }
        }

- ServiceProviderCreationTests.Test1

        public static void Test1()
        {
            ServiceProvider? _serviceProvider = null;
            try
            {
                // ServiceProvider
                _serviceProvider = new ServiceCollection()
                    .AddSingleton<InterfaceB, ClassB>()
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

.) organisation et contenu des extensions IServiceCollection des classes ServiceCollectionDescriptorExtensions 
   et ServiceCollectionServiceExtensions

Les classes ServiceCollectionDescriptorExtensions et ServiceCollectionServiceExtensions exposent chacune un ensemble
d'extensions de l'interface IServiceCollection destin�es � rendre plus lisible et productive la cr�ation du contenu
d'une instance de IServiceCollection.

La classe ServiceCollectionServiceExtensions expose un ensemble de m�thodes d'extension de l'interface IServiceCollection
portant sur ses capacit�s de liste : Add, Remove, RemoveAll,  Replace, TryAdd, TryAddEnumerable, TryAddScoped/Singleton/Transient ...

https://docs.microsoft.com/en-us/dotnet/api/microsoft.extensions.dependencyinjection.extensions.servicecollectiondescriptorextensions?view=dotnet-plat-ext-6.0
 
La classe ServiceCollectionServiceExtensions expose un ensemble de m�thodes d'extension de l'interface IServiceCollection
regroup�es par 'scope' : Scope, Singleton, Transient et par fa�on de pr�ciser les types des services et de leur impl�mentation:
g�n�rique, types pass�s en param�tre. 

https://docs.microsoft.com/en-us/dotnet/api/microsoft.extensions.dependencyinjection.servicecollectionserviceextensions?view=dotnet-plat-ext-6.0

public static class ServiceCollectionServiceExtensions
{
    //
    // Scoped
    //

    // types pass�s en param�tre
    public static IServiceCollection AddScoped (this IServiceCollection services, 
											Type serviceType);

    public static IServiceCollection AddScoped (this IServiceCollection services, 
											Type serviceType, 
											Func<IServiceProvider,object> implementationFactory);

    public static IServiceCollection AddScoped (this IServiceCollection services, 
											Type serviceType, 
											Type implementationType);

    // m�thodes g�n�riques
    public static IServiceCollection AddScoped<TService,TImplementation> (this IServiceCollection services) 
											where TService : class 
											where TImplementation : class, TService;

    public static IServiceCollection AddScoped<TService,TImplementation> (this IServiceCollection services, 
											Func<IServiceProvider,TImplementation> implementationFactory) 
											where TService : class 
											where TImplementation : class, TService;

    public static IServiceCollection AddScoped<TService> (this IServiceCollection services) 
											where TService : class;

    public static IServiceCollection AddScoped<TService> (this IServiceCollection services, 
											Func<IServiceProvider,TService> implementationFactory) 
											where TService : class;

    //
    // Singleton, Transient
    //
    // ...

.) R�solution d'un service par un IServiceProvider : GetService, GetRequiredService, ...

- L'interface IServiceProvider expose une m�thode unique :

    public object? GetService (Type serviceType);

- La classe ServiceProviderServiceExtensions fournit nombre d'extensions � cette interface,
  variations de GetService, GetRequiredService, GetServices, CreateScope, CreateAsyncScope :

    https://docs.microsoft.com/en-us/dotnet/api/microsoft.extensions.dependencyinjection.serviceproviderserviceextensions.getservice?view=dotnet-plat-ext-6.0

    public static T? GetService<T>(this IServiceProvider provider);
    public static object GetRequiredService(this IServiceProvider provider, Type serviceType);
    public static T GetRequiredService<T>(this IServiceProvider provider) where T : notnull
    public static IEnumerable<T> GetServices<T>(this IServiceProvider provider)
    public static IEnumerable<object?> GetServices(this IServiceProvider provider, Type serviceType)
    public static IServiceScope CreateScope(this IServiceProvider provider)
    public static AsyncServiceScope CreateAsyncScope(this IServiceProvider provider)
    public static AsyncServiceScope CreateAsyncScope(this IServiceScopeFactory serviceScopeFactory)

- GetService vs GetRequiredService :

La diff�rence entre une m�thode GetService et sa contrepartie GetRequiredService est que :
- GetService retourne null si le service demand� ne peut pas �tre r�solu par l'interface IServiceProvider
- GetRequiredService d�clenche une exception dans ce cas

Elle est illustr�e par le test ServiceProviderServiceExtensionsTests.Test_GetService

- GetServices :

Exemple (ServiceProviderServiceExtensionsTests.Test_GetServices)

Le code :

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

g�n�re le debug :

[16]UnitTests.Tests.ServiceProviderServiceExtensionsTests].(Test_GetServices) '(-)' 
[16]UnitTests.Tests.ServiceProviderServiceExtensionsTests].(Test_GetServices) 'classA=ClassA[4]' 
[16]UnitTests.Tests.ServiceProviderServiceExtensionsTests].(Test_GetServices) '_service=ClassA[2]' 
[16]UnitTests.Tests.ServiceProviderServiceExtensionsTests].(Test_GetServices) '_service=ClassA[3]' 
[16]UnitTests.Tests.ServiceProviderServiceExtensionsTests].(Test_GetServices) '_service=ClassA[4]' 
[16]UnitTests.Tests.ServiceProviderServiceExtensionsTests].(Test_GetServices) '(+)' 

.) Dur�e de vie de l'impl�mentation d'un service : Singleton, Transient, Scope

Les r�gles pr�sent�es dans ce paragraphe sont illustr�es par la classe de test ServiceLifetimeTests.

Terminologie: 

- r�solution d'un service par un (DI) container :

  Fait d'appeler la m�thode .GetService d'une instance de IServiceProvider
  en pr�cisant le type du service dont on souhaite obtenir une impl�mentation 

- r�solution d'un service Singleton/Transient/Scope par un (DI) container :

  r�solution d'un service ayant �t� enregistr� comme Singleton/Transient/Scope
  par le ServiceCollection � partir duquel le DI Container (IServiceProvider)
  mis en oeuvre a �t� produit (IServiceCollection.BuildServiceProvider).

- container 'racine'' :

  Instance de IServiceProvider produit � partir d'une instance de IServiceCollection,
  par un appel � BuildServiceProvider.

- container 'scoped' :

  Instance de IServiceProvider expos�e par une instance de IServiceScope:

    public interface IServiceScope : IDisposable
    {
        //
        // Summary:
        //     The System.IServiceProvider used to resolve dependencies from the scope.
        IServiceProvider ServiceProvider { get; }
    }

  voir plus bas.

-) Singleton

Une instance unique du type impl�mentant un service 'Singleton' est cr��e par un ServiceProvider lors de la premi�re
requ�te de r�solution recue (GetService).

Un service 'Singleton' a aussi pu �tre associ� � une instance d'impl�mentation lors de la constitution du ServiceCollection
� partir duquel sera cr�� un ServiceProvider. Cette instance d'impl�mentation sera alors retourn�e par le ServiceProvider
en tant que r�solution du service 'Singleton'.

Dans les deux cas, la r�solution d'un service 'Singleton' fournit tourjours la m�me r�ponse.

exemple :

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

-) Transient

La r�solution d'un service 'Transient' fournit � chaque fois une r�ponse diff�rente.

exemple:

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

-) Scope

R�gle : la r�solution d'un service 'Scope' ne doit pas �tre demand�e � un container 'racine' 
        mais � un container 'scoped' (voir d�finitions plus haut).

        Cette r�gle peut �tre v�rifi�e � l'ex�cution ou non par un ServiceProvider 
        selon la fa�on dont il a �t� produit (voir exemple ci-dessous).

Exemple:

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
                    .BuildServiceProvider(options : serviceProviderOptions);

                // Scoped, out of a scope : should not be done
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

.) Choix du constructeur d'instance par un container

La r�solution d'un service par un container peut impliquer la cr�ation d'une instance du type impl�mentant ce service.
C'est le cas, entre autres, lors de la premi�re r�solution d'un service 'singleton' ou � chaque r�solution d'un service 'transient'. 

Il se peut que la classe � instantier expose plusieurs constructeurs : lequel un container choisit-il lorsqu'il
doit instantier ladite classe ?

Un container choisit le constructeur dont la liste de param�tres contient le plus grand nombre de types r�solus
par lui m�me. Il se peut que plusieurs constructeurs citent le m�me nombre de types r�solus : le container ne
sait alors pas choisir donc pas instantier la classe et il d�clenche une exception.

Le test ServiceInstantiationTests.Test_ConstructorChoice illustre ces m�canismes.

Exemple:

    class ClassD : BaseClass
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

    class ClassE : BaseClass
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

.) enregistrement et destruction des instances 'disposables' g�n�r�es par un container

Comme �voqu� plus haut, la r�solution d'un service par un container peut impliquer la cr�ation d'une instance 
du type impl�mentant ce service.

Ces instances peuvent �tre enregistr�es ou non par le container dans une liste interne et ce pour assurer
leur caract�re Singleton ou Scope, mais aussi pour se charger de d�truire explicitement les instances 'disposables' 
(exposant l'interface IDisposable), ind�pendamment de leur dur�e de vie Singleton/Transient/Scoped.

La classe ServiceProvider est 'disposable' : 
    public sealed class ServiceProvider : IServiceProvider, IDisposable, IAsyncDisposable

L'interface IServiceScope est 'disposable' : 
    public interface IServiceScope : IDisposable

Un container 'racine' est 'disposable' explicitement. 
Un container 'scoped' est d�truit lorsque son scope l'est, par lui.

La destruction explicite des instances 'disposables' produites par un container intervient 
lorsque le container lui-m�me est 'dispos�'.

Exemples:

-) container 'racine'

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
                // produit le debug suivant :
                //[16]UnitTests.Tests.ServiceInstantiationTests].(Test_DisposableImplementations0) 'disposableClassA=DisposableClassA[2]' 

            }
            finally
            {
                this.Log("_serviceProvider?.Dispose(-)");
                _serviceProvider?.Dispose(); // destruction de disposableClassA
                this.Log("_serviceProvider?.Dispose(+)");
// produit le debug suivant :
//[16]UnitTests.Tests.ServiceInstantiationTests].(Test_DisposableImplementations0) '_serviceProvider?.Dispose(-)' 
//[16]DisposableClassA[2]].(dispose) 'disposing=True' 
//[16]UnitTests.Tests.ServiceInstantiationTests].(Test_DisposableImplementations0) '_serviceProvider?.Dispose(+)' 
            }

-) container 'scoped'

Le code:

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

produit le debug:

[16]UnitTests.Tests.ServiceInstantiationTests].(Test_DisposableImplementations1) '(-)' 
[16]UnitTests.Tests.ServiceInstantiationTests].(Test_DisposableImplementations1) '_disposableClassA=DisposableClassA[2]' 
[16]UnitTests.Tests.ServiceInstantiationTests].(Test_DisposableImplementations1) '_disposableClassB=DisposableClassB[3]' 
[16]UnitTests.Tests.ServiceInstantiationTests].(Test_DisposableImplementations1) '_disposableClassC=DisposableClassC[4]' 
[16]UnitTests.Tests.ServiceInstantiationTests].(Test_DisposableImplementations1) '_serviceProvider?.Dispose(-)' 
[16]DisposableClassC[4]].(dispose) 'disposing=True' 
[16]DisposableClassB[3]].(dispose) 'disposing=True' 
[16]DisposableClassA[2]].(dispose) 'disposing=True' 
[16]UnitTests.Tests.ServiceInstantiationTests].(Test_DisposableImplementations1) '_serviceProvider?.Dispose(+)' 

Remarque: les instances 'non disposables' Transient produites par un container ne sont pas m�moris�es, elles
sont lib�r�es par le Garbage Collector

Quelques cons�quences importantes de ce fonctionnement :

- un container 'racine' produisant des services Singleton ou Transient sous forme d'instances 'disposables'
  en conserve la liste jusqu'� sa destruction : ceci peut �tre une source de fuite m�moire, particuli�rement
  pour ce qui est des Transients.

- ceci est �galement vrai des services Transient et Scope r�solus sous forme d'instances 'disposables'
  par un container 'scoped' mais ce container sera d�truit en m�me temps que et par son 'scope' parent, 
  ce qui est sens� se produire rapidement. 

- les services r�solus sous forme d'instances 'disposables' ne doivent pas �tre 'dispos�s' par le client du
  container les ayant �mis : lui-m�me s'en chargera.

Ce dernier point devrait inciter � �viter que les services exposent l'interface IDisposable voient les r�f�rences
produites par leur r�solution par un container soient stock�es en tant que membres de classe.

Exemple:

    class DisposableClassD : Disposable0, IDisposableA
    {
    }

    /***
     * - an instance of DisposableClassE is built with an IDisposableA reference its stores in its
     *   _disposableA member
     * - holding disposable members, DisposableClassE should be disposable itself and handle the
     *   destruction of its members when disposed itself
    ***/
    class DisposableClassE : Disposable0
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
                // as well as the DisposableClassD instance created by the container
                // to build disposableClassE.
                // disposableClassE will itself dispose the DisposableClassD instance it received
                // when constructed : this DisposableClassD instance will then be disposed twice 
                _serviceProvider?.Dispose();
                this.Log("_serviceProvider?.Dispose(+)");
            }
        }

        Test_DisposableService0 produces the following debug:

[16]UnitTests.Tests.ServiceInstantiationTests].(Test_DisposableService0) '(-)' 
[16]UnitTests.Tests.ServiceInstantiationTests].(Test_DisposableService0) 'disposableClassE=DisposableClassE[3]' 
[16]UnitTests.Tests.ServiceInstantiationTests].(Test_DisposableService0) '_serviceProvider?.Dispose(-)' 
[16]DisposableClassE[3]].(dispose) 'disposing=True' 
[16]DisposableClassE[3]].(doDispose) '(-) disposing=True _disposableA=DisposableClassD[2]' 
[16]DisposableClassD[2]].(dispose) 'disposing=True' 
[16]DisposableClassE[3]].(doDispose) '(+) disposableA=DisposableClassD[2]' 
[16]DisposableClassD[2]].(dispose) 'disposing=True ALREADY DISPOSED' <<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<
[16]UnitTests.Tests.ServiceInstantiationTests].(Test_DisposableService0) '_serviceProvider?.Dispose(+)' 

.) Services g�n�riques : 'open generics services'

Un container sait enregistrer et r�soudre des 'open generics services'.

Exemple :

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

.) validation capabilities of a ServiceProvider

-) Preventing the resolution of scoped services out of a scope : 

Exemple : ServiceProviderValidationTests.TestScopeValidation0

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

-) Preventing the resolution of a Singleton service depending on Scope services.

Exemple : ServiceProviderValidationTests.TestScopeValidation1

namespace UnitTests.Tests
{

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

-) Checking the completeness of the description of the services meant to build a container

Exemple : ServiceProviderValidationTests.TestScopeValidation2

namespace UnitTests.Tests
{

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

.) la classe ActivatorUtilities 

La classe ActivatorUtilities permet de cr�er des instances de classes non r�solues par un container
mais dont le constructeur requiert des arguments pouvant eux �tre r�solus par ce container.  

    https://docs.microsoft.com/en-us/dotnet/api/microsoft.extensions.dependencyinjection.activatorutilities?view=dotnet-plat-ext-5.0

Cet excellent article explique comment:

    - Activator utilities: activate anything!
    https://onthedrift.com/posts/activator-utilities/

.) IServiceScopeFactory

L'interface IServiceScopeFactory

https://docs.microsoft.com/en-us/dotnet/api/microsoft.extensions.dependencyinjection.iservicescopefactory?view=dotnet-plat-ext-6.0

expose la seule m�thode :

    public Microsoft.Extensions.DependencyInjection.IServiceScope CreateScope ();

Elle est enregistr�e en tant que service Singleton par un container et peut dont �tre r�solue,
voir exemple ci-dessous, et utilis�e pour produire des IServiceScopes.

Exemple : IServiceScopeFactoryTests.Test0

        // Utilisation d'un IServiceScope existant et de son .ServiceProvider
        // pour en obtenir des services 
        void test0(IServiceScope scope)
        {
            InterfaceA? _pInterfaceA0 = scope.ServiceProvider.GetService<InterfaceA>();
            InterfaceA? _pInterfaceA1 = scope.ServiceProvider.GetService<InterfaceA>();
        }

        // Utilisation d'un IServiceScopeFactory pour produire localement un IServiceScope,
        // obtenir des services de son .ServiceProvider puis d�truire ce IServiceScope
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

.) URLs

	- Dependency inversion
	https://docs.microsoft.com/en-us/dotnet/architecture/modern-web-apps-azure/architectural-principles#dependency-inversion

	- Dependency injection in .NET
	https://docs.microsoft.com/en-us/dotnet/core/extensions/dependency-injection

    - Tutorial: Use dependency injection in .NET 
    https://docs.microsoft.com/en-us/dotnet/core/extensions/dependency-injection-usage

    - Dependency injection guidelines
    https://docs.microsoft.com/en-us/dotnet/core/extensions/dependency-injection-guidelines

