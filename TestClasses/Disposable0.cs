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
            int _disposed = Interlocked.CompareExchange(ref this._disposed, 1, 0);

            if (_disposed == 0)
            {
                this.Log($"disposing={disposing}");
            }
            else
            {
                this.Log($"disposing={disposing} ALREADY DISPOSED");
            }

            if (_disposed != 0) return;

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
            if (!IsDisposed)
            {
                return $"{GetType().Name}[{ID}]";
            }
            else
            {
                if (IsExplicitelyDisposed)
                {
                    return $"{GetType().Name}[{ID}] : DISPOSED";
                }
                else
                {
                    return $"{GetType().Name}[{ID}] : GARBAGE COLLECTED";
                }
            }
        }

        public bool IsDisposed => _disposed != 0;
        public bool IsExplicitelyDisposed => _disposed != 0 && _explicitely_disposed;

    }

}
