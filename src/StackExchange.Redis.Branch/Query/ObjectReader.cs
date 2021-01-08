using StackExchange.Redis.Branch.Entity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace StackExchange.Redis.Branch.Query
{
    internal class ObjectReader<T> : IEnumerable<T>, IEnumerable where T : RedisEntity, new()
    {
        private Enumerator _enumerator;

        internal ObjectReader(List<HashEntry[]> reader)
        {
            _enumerator = new Enumerator(reader);
        }

        public IEnumerator<T> GetEnumerator()
        {
            Enumerator e = _enumerator;

            if (e == null)
            {
                throw new InvalidOperationException("Cannot enumerate more than once");
            }

            _enumerator = null;
            return e;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        class Enumerator : IEnumerator<T>, IEnumerator
        {
            private List<HashEntry[]> _reader;
            private T _current;
            private int _index;

            internal Enumerator(List<HashEntry[]> reader)
            {
                _reader = reader;
                _index = 0;
            }

            public T Current
            {
                get { return _current; }
            }

            object IEnumerator.Current
            {
                get { return _current; }
            }

            public void Dispose()
            {
            }

            public bool MoveNext()
            {
                if (_reader.Count > _index)
                {
                    T instance = _reader.ElementAt(_index++).ConvertFromHashEntryList<T>();

                    _current = instance;

                    return true;
                }

                return false;
            }

            public void Reset()
            {
            }
        }
    }
}
