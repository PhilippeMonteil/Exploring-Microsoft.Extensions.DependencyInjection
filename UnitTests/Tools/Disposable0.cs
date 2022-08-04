namespace TestClasses
{

    public class Disposable0 : BaseClass, IDisposable
    {
        private int _disposed;
        private bool _explicitely_disposed;

        #region --- IDisposable

        protected virtual void doDispose(bool disposing)
        {
        }

        void dispose(bool disposing)
        {
            int __disposed = Interlocked.CompareExchange(ref this._disposed, 1, 0);

            if (__disposed == 0)
            {
                this.Log($"disposing={disposing}");
            }
            else
            {
                this.Log($"disposing={disposing} ALREADY DISPOSED");
            }

            if (__disposed != 0) return;

            _explicitely_disposed = disposing;

            doDispose(disposing);
        }

        ~Disposable0()
        {
            dispose(disposing: false);
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
            dispose(disposing: true);
        }

        #endregion

        public override string ToString()
        {
            return $"{GetType().Name}[{ID}]";
        }

        public bool IsDisposed => _disposed != 0;
        public bool IsExplicitelyDisposed => _disposed != 0 && _explicitely_disposed;

    }

}
