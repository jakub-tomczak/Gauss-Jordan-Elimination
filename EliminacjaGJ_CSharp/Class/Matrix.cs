using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EliminacjaGJ_CSharp.Class
{
    class Matrix<T>
    {
        private T [,] matrix;
        public Matrix(int rows, int columns)
        {
            matrix = new T[rows, columns];
        }

        public static Matrix<T> GenerateRandomMatrix(int rows, int columns)
        {
            throw new NotImplementedException();
        }
    }
}
