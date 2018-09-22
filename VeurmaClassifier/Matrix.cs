using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VeurmaClassifier
{
    public static class Matrix
    {

        public static void Range<T>(this T[] values, out T min, out T max)
     where T : IComparable<T>
        {
            if (values.Length == 0)
            {
                min = max = default(T);
                return;
            }

            min = max = values[0];
            for (int i = 1; i < values.Length; i++)
            {
                if (values[i].CompareTo(min) < 0)
                    min = values[i];
                if (values[i].CompareTo(max) > 0)
                    max = values[i];
            }
        }

        public static T[] GetColumn<T>(this T[][] m, int index) where T : IComparable
        {
            T[] result = new T[m.Length];

            index = Matrix.index(index, m.Columns());
            for (int i = 0; i < result.Length; i++)
                result[i] = m[i][index];

            return result;
        }

        private static int index(int end, int length)
        {
            if (end < 0)
                end = length + end;
            return end;
        }

        public static int Columns<T>(this T[][] matrix)
        {
            if (matrix.Length == 0)
                return 0;
            return matrix[0].Length;
        }

        public static int[] Find<T>(this T[] data, Func<T, bool> func)
        {
            List<int> idx = new List<int>();

            for (int i = 0; i < data.Length; i++)
                if (func(data[i]))
                    idx.Add(i);

            return idx.ToArray();
        }

        public static T[] Sub<T>(this T[] source, int[] indexes)
        {
            var destination = new T[indexes.Length];
            for (int i = 0; i < indexes.Length; i++)
            {
                int j = indexes[i];
                if (j >= 0)
                    destination[i] = source[j];
                else
                    destination[i] = source[source.Length + j];
            }
            return destination;
        }

        public static T MaxElement<T>(this T[] values, out int imax)
            where T : IComparable<T>
        {
            imax = 0;
            T max = values[0];
            for (int i = 1; i < values.Length; i++)
            {
                if (values[i].CompareTo(max) > 0)
                {
                    max = values[i];
                    imax = i;
                }
            }
            return max;
        }

        public static bool AreEqual<T>(this IEnumerable<T> list1, IEnumerable<T> list2)
        {
            var cnt = new Dictionary<T, int>();

            foreach (var s in list1)
            {
                if (cnt.ContainsKey(s))
                    cnt[s]++;
                else cnt.Add(s, 1);
            }

            foreach (var s in list2)
            {
                if (cnt.ContainsKey(s))
                    cnt[s]--;
                else return false;
            }

            foreach (var s in cnt)
            {
                if (s.Value != 0)
                    return false;
            }

            return true;
        }

        #region Conversions
        public static double[] ConvertToDouble(this int[] value)
        {
            return ConvertToDouble(value, new double[value.Length]);
        }
        public static double[] ConvertToDouble(this int[] value, double[] result)
        {
            for (int i = 0; i < value.Length; i++)
                result[i] = (Double)value[i];
            return result;
        }
        public static double[] ConvertToDouble(this float[] value)
        {
            return ConvertToDouble(value, new double[value.Length]);
        }
        public static double[] ConvertToDouble(this float[] value, double[] result)
        {
            for (int i = 0; i < value.Length; i++)
                result[i] = (Double)value[i];
            return result;
        }
        #endregion
    }
}
