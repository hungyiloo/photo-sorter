using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhotoSorter
{
    // This custom enumerator/iterator is necessary to facilitate forward AND 
    // backward iterating as well as more flexible position control.
    public class IterableList<T> : List<T>
    {
        private int _position;
        public int Position
        {
            get
            {
                return _position;
            }
            set
            {
                if (value >= -1 && value < base.Count)
                    _position = value;
                else
                    throw new IndexOutOfRangeException("The specified position is out of bounds");
            }
        }

        public IterableList()
            : base()
        {
            _position = -1;
        }

        public IterableList(IEnumerable<T> collection)
            : base(collection)
        {
            _position = -1;
        }

        public T Current
        {
            get
            {
                if (Position >= 0 && Position < base.Count)
                    return base[Position];
                else
                    return default(T);
            }
        }

        public bool MoveNext()
        {
            if (Position < base.Count - 1)
            {
                Position++;
                return true;
            }
            return false;
        }

        public bool MovePrevious()
        {
            if (Position > 0)
            {
                Position--;
                return true;
            }
            return false;
        }

        public void Reset()
        {
            Position = -1;
        }
    }
}
