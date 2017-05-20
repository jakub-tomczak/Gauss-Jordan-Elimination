using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EliminacjaGJ_CSharp.Class
{
    //to remove since MathNet.Numerics.LinearAlgebra is being used
    class MyMatrix<T>
    {
        private T [,] matrix;
        public MyMatrix(int rows, int columns)
        {
            matrix = new T[rows, columns];
        }

        public static MyMatrix<T> GenerateRandomMatrix(int rows, int columns)
        {
            throw new NotImplementedException();
        }
    }
}
