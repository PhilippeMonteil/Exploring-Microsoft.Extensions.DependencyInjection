
using TestClasses;

namespace UnitTests.Tools
{

    class ClassA : BaseClass, InterfaceA
    {
    }

    class ClassB : BaseClass, InterfaceB
    {
    }

    class ClassC : BaseClass, InterfaceC
    {

        public ClassC()
        {
        }

    }

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

        public ClassE(InterfaceA interfaceA)
        {
            this.Log("");
        }

        public ClassE(InterfaceA interfaceA, InterfaceB interfaceB)
        {
            this.Log("");
        }

        public ClassE(InterfaceA interfaceA, InterfaceC interfaceC)
        {
            this.Log("");
        }

    }

    class ClassF<T> : BaseClass, InterfaceF<T>
    {
    }

    class DisposableClassA : Disposable0, InterfaceA
    {
    }

    class DisposableClassB : Disposable0, InterfaceB
    {
    }

    class DisposableClassC : Disposable0
    {
    }

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

    class NotDisposableClassA : NotDisposable0, InterfaceA
    {
    }

    class NotDisposableClassB : NotDisposable0, InterfaceB
    {
    }

    class NotDisposableClassC : NotDisposable0
    {
    }

}
