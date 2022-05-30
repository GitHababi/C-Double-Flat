using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace C_Double_Flat.Core.Utilities
{
    internal class VariableEnumerator : IEnumerator<IVariable>
    {
        public IVariable[] _variables;

        // Enumerators are positioned before the first element
        // until the first MoveNext() call.
        int position = -1;
        private bool disposedValue;

        public VariableEnumerator(IVariable[] list)
        {
            _variables = list;
        }

        public bool MoveNext()
        {
            position++;
            return (position < _variables.Length);
        }

        public void Reset()
        {
            position = -1;
        }

        public IVariable Current
        {
            get
            {
                try
                {
                    return _variables[position];
                }
                catch (IndexOutOfRangeException)
                {
                    throw new InvalidOperationException();
                }
            }
        }

        object IEnumerator.Current => Current;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~VariableEnumerator()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
