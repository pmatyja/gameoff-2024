using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace DeBroglie.Wfc
{
    internal class Deque<T> : IEnumerable<T>
    {
        private T[] _data;
        private int _dataLength;

        // Data is in range lo to hi, exclusive of hi
        // hi == lo if the Deque is empty
        // You may have hi < lo if we've wrapped the end of data
        private int _lo;
        private int _hi;

        public Deque(int capacity = 4)
        {
            this._data = new T[capacity];
            this._dataLength = capacity;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Push(T t)
        {
            var hi = this._hi;
            var lo = this._lo;
            this._data[hi] = t;
            hi++;
            if (hi == this._dataLength) hi = 0;
            this._hi = hi;
            if (hi == lo)
            {
                this.ResizeFromFull();
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T Pop()
        {
            var lo = this._lo;
            var hi = this._hi;
            if (lo == hi)
                throw new Exception("Deque is empty");
            if (hi == 0) hi = this._dataLength;
            hi--;
            this._hi = hi;
            return this._data[hi];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Shift(T t)
        {
            var lo = this._lo;
            var hi = this._hi;
            if (lo == 0) lo = this._dataLength;
            lo--;
            this._data[lo] = t;
            this._lo = lo;
            if (hi == lo)
            {
                this.ResizeFromFull();
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T Unshift()
        {
            var lo = this._lo;
            var hi = this._hi;
            if (lo == hi)
                throw new Exception("Deque is empty");
            var oldLo = lo;
            lo++;
            if (lo == this._dataLength) lo = 0;
            this._lo = lo;
            return this._data[oldLo];
        }

        public void DropFirst(int n)
        {
            var hi = this._hi;
            var lo = this._lo;
            if (lo <= hi)
            {
                lo += n;
                if(lo >= hi)
                {
                    // Empty
                    this._lo = this._hi = 0;
                }
                else
                {
                    this._lo = lo;
                }
            }
            else
            {
                lo += n;
                if(lo >= this._dataLength)
                {
                    lo -= this._dataLength;
                    if(lo >= hi)
                    {
                        // Empty
                        this._lo = this._hi = 0;
                    }
                    else
                    {
                        this._lo = lo;
                    }
                }
            }
        }

        public void DropLast(int n)
        {
            var hi = this._hi;
            var lo = this._lo;
            if (lo <= hi)
            {
                hi -= n;
                if(lo >= hi)
                {
                    // Empty
                    this._lo = this._hi = 0;
                }
                else
                {
                    this._hi = hi;
                }
            }
            else
            {
                hi -= n;
                if(hi < 0)
                {
                    hi += this._dataLength;
                    if(lo >= hi)
                    {
                        // Empty
                        this._lo = this._hi = 0;
                    }
                    else
                    {
                        this._hi = hi;
                    }
                }
            }
        }

        public int Count
        {
            get
            {
                var c = this._hi - this._lo;
                if (c < 0)
                    c += this._dataLength;
                return c;
            }
        }

        private void ResizeFromFull()
        {
            var dataLength = this._dataLength;
            var newLength = dataLength * 2;
            var newData = new T[newLength];

            var i = this._lo;
            var j = 0;
            var hi = this._hi;

            do
            {
                newData[j] = this._data[i];

                j++;
                i++;
                if (i == dataLength) i = 0;
            } while (i != hi);

            this._data = newData;
            this._dataLength = newLength;
            this._lo = 0;
            this._hi = j;
        }

        public IEnumerable<T> Slice(int start, int end)
        {
            var lo = this._lo;
            var hi = this._hi;
            var data = this._data;
            var dataLength = this._dataLength;
            var i = lo + start;
            var e = lo + end;

            if (start < 0)
                throw new Exception();

            if (hi >= lo)
            {
                if (e > hi)
                    throw new Exception();
            }
            else
            {
                if (e > hi + dataLength)
                    throw new Exception();
            }

            if (start >= end)
                yield break;

            if (i >= dataLength) i -= dataLength;
            if (e >= dataLength) e -= dataLength;

            do
            {
                yield return data[i];
                i++;
                if (i == dataLength) i = 0;
            } while (i != e);
        }

        public IEnumerable<T> ReverseSlice(int start, int end)
        {
            var lo = this._lo;
            var hi = this._hi;
            var data = this._data;
            var dataLength = this._dataLength;
            var i = lo + start;
            var e = lo + end;

            if (start < 0)
                throw new Exception();

            if (hi >= lo)
            {
                if (e > hi)
                    throw new Exception();
            }
            else
            {
                if (e > hi + dataLength)
                    throw new Exception();
            }

            if (start >= end)
                yield break;

            if (i >= dataLength) i -= dataLength;
            if (e >= dataLength) e -= dataLength;

            do
            {
                e--;
                yield return data[e];
                if (e == 0) e = dataLength - 1;
            } while (i != e);
        }

        public IEnumerator<T> GetEnumerator()
        {

            var lo = this._lo;
            var hi = this._hi;
            var data = this._data;
            var dataLength = this._dataLength;
            var i = lo;
            var e = hi;

            if (hi >= lo)
            {
                if (e > hi)
                    throw new Exception();
            }
            else
            {
                if (e > hi + dataLength)
                    throw new Exception();
            }

            if (lo == hi)
                yield break;

            if (i >= dataLength) i -= dataLength;
            if (e >= dataLength) e -= dataLength;

            do
            {
                yield return data[i];
                i++;
                if (i == dataLength) i = 0;
            } while (i != e);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
