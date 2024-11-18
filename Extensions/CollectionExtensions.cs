using System;
using System.Collections.Generic;

namespace Modules.Extensions
{
    public static class CollectionExtensions
    {
        public static T FirstOrDefault<T>(this T[,] array, System.Predicate<T> match)
        {
            var numRows = array.GetLength(0);
            var numCols = array.GetLength(1);

            for (var row = 0; row < numRows; row++)
            {
                for (var col = 0; col < numCols; col++)
                {
                    var item = array[row, col];
                    if (match.Invoke(item))
                    {
                        return item;
                    }
                }
            }
            return default;
        }
        
        public static List<T> Shuffle<T>(this List<T> list)
        {
            for (var index = 0; index < list.Count - 1; index++)
            {
                var r = UnityEngine.Random.Range(index, list.Count);
                (list[index], list[r]) = (list[r], list[index]);
            }

            return list;
        }
        
        public static T[,] RotateArrayClockwise<T>(this T[,] src)
        {
            int width;
            int height;
            T[,] dst;

            width = src.GetUpperBound(0) + 1;
            height = src.GetUpperBound(1) + 1;
            dst = new T[height, width];

            for (int row = 0; row < height; row++)
            {
                for (int col = 0; col < width; col++)
                {
                    int newRow;
                    int newCol;

                    newRow = col;
                    newCol = height - (row + 1);

                    dst[newCol, newRow] = src[col, row];
                }
            }

            return dst;
        }
        
        public static T[,] RotateAntiArrayClockwise<T>(this T[,] src)
        {
            int width;
            int height;
            T[,] dst;

            width = src.GetUpperBound(0) + 1;
            height = src.GetUpperBound(1) + 1;
            dst = new T[height, width];

            for (int row = 0; row < height; row++)
            {
                for (int col = 0; col < width; col++)
                {
                    int newRow;
                    int newCol;

                    newRow = width - (col + 1);
                    newCol = row;

                    dst[newCol, newRow] = src[col, row];
                }
            }

            return dst;
        }
        
        public static T[,] Fill<T>(this T[,] self, T value)
        {
            var length0 = self.GetLength(0);
            var length1 = self.GetLength(1);

            for (var i = 0; i < length0; i++)
            {
                for (var j = 0; j < length1; j++)
                {
                    self[i, j] = value;
                }
            }

            return self;
        }

        public static IEnumerable<T> ToEnumerable<T>(this T[,] self) where T : class
        {
            var dim0 = self.GetLength(0);
            var dim1 = self.GetLength(1);

            for (var i = 0; i < dim0; i++)
            {
                for (var j = 0; j < dim1; j++)
                {
                    yield return self[i, j];
                }
            }
        }

        public static IEnumerable<T> Where<T>(this T[,] self, Func<T, int, int, bool> func) where T : class
        {
            var dim0 = self.GetLength(0);
            var dim1 = self.GetLength(1);

            for (var i = 0; i < dim0; i++)
            {
                for (var j = 0; j < dim1; j++)
                {
                    if (func(self[i, j], i, j))
                    {
                        yield return self[i, j];
                    }
                }
            }
        }
        
        public static IEnumerable<T> Where<T>(this T[,] self, Func<T, bool> func) where T : class
        {
            var dim0 = self.GetLength(0);
            var dim1 = self.GetLength(1);

            for (var i = 0; i < dim0; i++)
            {
                for (var j = 0; j < dim1; j++)
                {
                    if (func(self[i, j]))
                    {
                        yield return self[i, j];
                    }
                }
            }
        }

        public static TResult[,] Select<TSource, TResult>(this TSource[,] self, Func<TSource, TResult> selector)
        {
            var dim0 = self.GetLength(0);
            var dim1 = self.GetLength(1);
            var result = new TResult[dim0, dim1];

            for (var i = 0; i < dim0; i++)
            {
                for (var j = 0; j < dim1; j++)
                {
                    result[i, j] = selector(self[i, j]);
                }
            }

            return result;
        }
        
        public static TResult[,] Select<TSource, TResult>(this TSource[,] self, Func<TSource, int, int, TResult> selector)
        {
            var dim0 = self.GetLength(0);
            var dim1 = self.GetLength(1);
            var result = new TResult[dim0, dim1];

            for (var i = 0; i < dim0; i++)
            {
                for (var j = 0; j < dim1; j++)
                {
                    result[i, j] = selector(self[i, j], i, j);
                }
            }

            return result;
        }
        

        public static void ForEach<T>(this T[,] self, Func<T, T> func)
        {
            var dim0 = self.GetLength(0);
            var dim1 = self.GetLength(1);

            for (var i = 0; i < dim0; i++)
            {
                for (var j = 0; j < dim1; j++)
                {
                    self[i, j] = func(self[i, j]);
                }
            }
        }
        
        public static void ForEach<T>(this T[,] self, Action<T> func)
        {
            var dim0 = self.GetLength(0);
            var dim1 = self.GetLength(1);

            for (var i = 0; i < dim0; i++)
            {
                for (var j = 0; j < dim1; j++)
                {
                    func(self[i, j]);
                }
            }
        }

        public static int Count<T>(this T[,] self, Func<T, bool> func)
        {
            var dim0 = self.GetLength(0);
            var dim1 = self.GetLength(1);
            var result = 0;

            for (var i = 0; i < dim0; i++)
            {
                for (var j = 0; j < dim1; j++)
                {
                    if (func(self[i, j]))
                    {
                        result++;
                    }
                }
            }

            return result;
        }

        public static T[,] Copy<T>(this T[,] self)
        {
            var dim0 = self.GetLength(0);
            var dim1 = self.GetLength(1);
            var result = new T[dim0, dim1];

            for (var i = 0; i < dim0; i++)
            {
                for (var j = 0; j < dim1; j++)
                {
                    result[i, j] = self[i, j];
                }
            }

            return result;
        }

        public static T[,] CopyTo<T>(this T[,] self, T[,] result)
        {
            var dim0 = self.GetLength(0);
            var dim1 = self.GetLength(1);

            for (var i = 0; i < dim0; i++)
            {
                for (var j = 0; j < dim1; j++)
                {
                    result[i, j] = self[i, j];
                }
            }

            return result;
        }
    }
}