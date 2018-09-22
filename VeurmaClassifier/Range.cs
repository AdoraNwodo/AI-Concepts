using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace VeurmaClassifier
{
    //Generic Continous range class
    public class Range<T> where T : IComparable, IComparable<T>, IConvertible, IEquatable<T>, IFormattable
    {
        private T min, max;

        public T Min
        {
            get { return min; }
            set { min = value; }
        }

        public T Length
        {
            get { return (dynamic)max - (dynamic)min; }
        }


        public T Max
        {
            get { return max; }
            set { max = value; }
        }


        public Range(T min, T max)
        {
            this.min = min;
            this.max = max;
        }

        public Range<int> ToIntRange()
        {
            int iMin, iMax;

            iMin = (int)System.Math.Floor((dynamic)min);
            iMax = (int)System.Math.Ceiling((dynamic)max);

            return new Range<int>(iMin, iMax);
        }
    }
}
