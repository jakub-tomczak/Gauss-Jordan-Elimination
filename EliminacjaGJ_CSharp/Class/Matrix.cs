using System;
using System.Collections.Generic;
using System.Globalization;

namespace EliminacjaGJ_CSharp.Class
{
    //to remove since MathNet.Numerics.LinearAlgebra is being used
    /// <summary>
    /// 
    /// </summary>
    /// <exception cref="FormatException">Excpetion thrown when using diffrent type than int or double as generic</exception>
    /// <typeparam name="T"></typeparam>
    class MyMatrix<T>
    {
        private T[,] matrix;
        private int rows;
        private int columns;
        private string matrixType = string.Empty;

        public MyMatrix(int n)
            : this(n, n)
        {

        }

        public MyMatrix(int rows, int columns)
        {
            this.rows = rows;
            this.columns = columns;
            matrix = new T[rows, columns];

            bool intervalType = false;
            var genericType = GetGenericType(out intervalType);
            if (genericType != typeof(System.Int32)
                && genericType != typeof(System.Double))
            {
                throw new FormatException($"Matrix class doesn't accept other data types than int, double or Interval<int> Interval<double>. Current type {genericType}");
            }

            if (intervalType)
            {
                matrixType = $"Interval<{genericType}>";
            }
            else
            {
                matrixType = genericType.ToString();
            }

        }

        private Type GetGenericType(out bool intervalType)
        {
            intervalType = false;
            if (matrix.GetType().GetGenericArguments().Length > 0)
            {
                intervalType = true;
                return matrix.GetType().GetGenericArguments()[0];
            }

            return matrix.GetType().GetElementType();
        }

        public T[,] GetMatrix()
        {
            return matrix;
        }

        public static MyMatrix<T> GenerateRandomMatrix(int rows, int columns, double randFloor, double randCelling)
        {
            if (randFloor < randCelling)
            {
                double temp = randCelling;
                randCelling = randFloor;
                randFloor = temp;
            }


            Random random = new Random();
            MyMatrix<T> matrix = new MyMatrix<T>(rows, columns);
            var matrixType = matrix.GetMatrix().GetType().GetGenericArguments()[0];


            for (int row = 0; row < rows; row++)
            {
                Interval<double> temp = new Interval<double>((double)row + 1);

                for (int column = 0; column < columns; column++)
                {
                    matrix.GetMatrix()[row, column] = (dynamic)new Interval<double>(0, 0);
                    if (matrixType == typeof(System.Int32))
                    {
                        matrix[row, column] = (dynamic)random.Next((int)randFloor, (int)randCelling);
                    }
                    else if (matrixType == typeof(System.Double))
                    {
                        if (row == 0)
                        {
                            matrix[row, column] = (dynamic)new Interval<double>(randFloor + (randCelling - randFloor) * random.NextDouble()); //, randFloor + (randCelling - randFloor) * random.NextDouble()
                        }
                        else
                        {
                            matrix[row, column] = (dynamic)matrix[row - 1, column] * temp;
                        }
                        //matrix.GetMatrix()[row, column] = (dynamic)0;// (double)randFloor + (randCelling - randFloor) * (dynamic)random.NextDouble();
                    }
                    else
                    {
                        throw new FormatException("Wrong data type");
                    }
                }


            }

            return matrix;
        }

        public void GaussElimination()
        {
            for (int gw = 0; gw < rows; gw++)
            {
                for (int w = gw + 1; w < columns; w++)
                {

                    //Interval<double> m = matrixa[w, gw] / matrixa[gw, gw];
                    T m = (dynamic)matrix[w, gw] / matrix[gw, gw];
                    for (int k = gw; k < columns; k++)
                    {
                        matrix[w, k] = matrix[w, k] - (dynamic)m * matrix[gw, k];
                    }
                    //do the same operation on vector
                    //vectorB[w] = vectorB[w] - m * vectorB[gw];
                }
            }
        }

        public void GenerateRandomMatrix(double floor, double celling)
        {


            for (int row = 0; row < rows; row++)
            {
                T coefficient = GetGenericRandom(floor, celling);
                for (int column = 0; column < columns; column++)
                {
                    //random first row, the rest is the multiplication of the first one
                    if (row == 0)
                    {
                        matrix[row, column] = GetGenericRandom(floor, celling);
                    }
                    else
                    {
                        matrix[row, column] = (dynamic)matrix[row - 1, column] * coefficient;
                    }

                }

            }

        }

        private T GetGenericRandom(double floor, double celling)
        {
            Random random = new Random();

            //recognizing type to apply random
            var matrixType = matrix.GetType();
            if (matrixType.GetGenericArguments().Length > 0)
            {
                if (matrixType.GetGenericArguments()[0] == typeof(System.Double))
                {
                    //interval double
                    return (dynamic)(new Interval<double>(floor + random.NextDouble() * (celling - floor)));
                }
                else
                {
                    //interval int
                    return (dynamic)(new Interval<int>(random.Next((int)floor, (int)celling)));
                }

            }
            else
            {
                if (matrixType.GetElementType() == typeof(System.Double))
                {
                    //double
                    return (dynamic)(floor + random.NextDouble() * (celling - floor));
                }
                else
                {
                    //int
                    return (dynamic)(random.Next((int)floor, (int)celling));
                }
            }

        }

        public void LoadFromList(List<string> elements)
        {
            var matrixType = matrix.GetType();
            int iterator = 0;

            bool intervalType = false;
            if (matrixType.GetGenericArguments().Length > 0)
            {
                if (matrix.GetType().GetGenericArguments()[0] == typeof(double))
                {
                    //interval double
                    intervalType = true;
                }

            }



            if (elements.Count != rows * columns)
                throw new ArgumentException($"Liczba wczytywanych elementów niezgodna z rozmiarem macierzy (lista:{elements.Count}, macierz:{rows * columns}");

            foreach (string item in elements)
            {
                T number;
                if (intervalType)
                {
                    number = (dynamic)Interval<double>.NumberRead(item);

                }
                else
                {
                    double temp = 0f;
                    number = (dynamic)0f;
                    if (double.TryParse(
                        item,
                        NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingSign | NumberStyles.Float,
                        new CultureInfo("en-US"),
                        out temp))
                    {
                        number = (dynamic)temp;
                    }

                    else
                        throw new FormatException($"Konwersja ciągu {item} do typu double się nie powiodła!");
                }

                matrix[iterator / rows, iterator % columns] = (dynamic)number;
                iterator++;
            }
        }

        public override string ToString()
        {
            String returnValue = string.Empty;
            returnValue += $"Matrix {matrixType}, {this.rows}x{this.columns}\n";
            for (int row = 0; row < this.rows; row++)
            {
                for (int column = 0; column < this.columns; column++)
                {
                    returnValue += $"{matrix[row, column].ToString()}\t";
                }
                returnValue += "\n";
            }
            return returnValue;
        }

        //public void GaussJordan()
        //{
        //    for (int row = 0; row < n; row++)
        //    {
        //        for (int column = 0; column < n; column++)
        //        {
        //            //1.check if pivot cell has a value diffrent from 0
        //            if (matrix[row, column] == 0)
        //            {
        //                if (!TryToSwapRows(row))
        //                {
        //                    continue;   //increase column by 1 if all entries in the column are 0
        //                }
        //            }

        //            //2divide row-th row by a[row column]
        //            for (int j = 0; j < n; j++)
        //            {
        //                matrix[row, j] /= matrix[row, 0];
        //            }



        //        }
        //    }
        //}

        //private bool TryToSwapRows(int row)
        //{
        //    for (int i = 0; i < n; i++)
        //    {
        //        if (matrix[i, i] != 0)
        //        {
        //            for (int j = 0; j < n; j++)
        //            {
        //                double temp = matrix[row, j];
        //                matrix[row, j] = matrix[i, j];
        //                matrix[i, j] = temp;
        //            }
        //            return true;
        //        }
        //    }
        //    return false;
        //}

        public T this[int row, int column]
        {
            get
            {
                return this.matrix[row, column];
            }
            set
            {
                this.matrix[row, column] = value;
            }
        }
    }
}
