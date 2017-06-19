using System;
using System.Collections.Generic;
using System.Globalization;

namespace EliminacjaGJ_CSharp.Class
{
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
        private bool isIntervalType;

        public MyMatrix(int n)
            : this(n, n)
        {

        }

        public MyMatrix(int rows, int columns)
        {
            this.rows = rows;
            this.columns = columns;
            matrix = new T[rows, columns];


            random = new Random(DateTime.Now.Millisecond);

            bool intervalType = false;
            var genericType = GetGenericType(out intervalType);
            if (genericType != typeof(System.Int32)
                && genericType != typeof(System.Double))
            {
                throw new FormatException($"Matrix class doesn't accept other data types than int, double or Interval<int> Interval<double>. Current type {genericType}");
            }
            isIntervalType = intervalType;
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
                        //if (row == 0)
                        //{
                        matrix[row, column] = (dynamic)new Interval<double>(randFloor + (randCelling - randFloor) * random.NextDouble()); //, randFloor + (randCelling - randFloor) * random.NextDouble()
                        //}
                        //else
                        //{
                        //    matrix[row, column] = (dynamic)matrix[row - 1, column] * temp;
                        //}
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

        public void CalculateError(ref double meanError, ref double minError, ref double maxError)
        {
            

            double error = 0f;
            if (isIntervalType)
            {
                for (int row = 0; row < rows; row++)
                {
                    for (int column = 0; column < columns; column++)
                    {
                        Interval<double> temp = (dynamic)matrix[row, column];
                        double tempWidth = temp.GetWidth();
                        if (row==0 && column == 0)
                        {
                            minError = tempWidth;
                            maxError = tempWidth;
                        }
                        else
                        {
                            if (minError > tempWidth)
                            {
                                minError = tempWidth;
                            }
                            if (maxError < tempWidth)
                            {
                                maxError = tempWidth;
                            }
                        }
                        error += tempWidth;
                    }
                }
            }
            else
            {
                for (int row = 0; row < rows; row++)
                {
                    for (int column = 0; column < columns; column++)
                    {
                        double temp = (dynamic)matrix[row, column];

                        if (row == 0 && column ==0)
                        {
                            minError = Math.Abs(1f - temp);
                            maxError = minError;
                        }
                        else
                        {
                            if(minError > temp)
                            {
                                minError = temp;
                            }
                            if(maxError < temp)
                            {
                                maxError = temp;
                            }
                        }
                        if (row == column)
                        {
                            error += Math.Abs(1f - temp);
                        }
                        else
                        {
                            error += Math.Abs(0f - temp);
                        }
                    }
                }
            }

            error /= (rows * columns);
            meanError = error;
        }

        private void TryToPerformPartialPivoting(int startingRow)
        {
            if (startingRow + 1 == rows)
                return;

            int maxIndex = -1;

            if (isIntervalType)
            {
                Interval<double> maxValue = new Interval<double>(0f);
                for (int row = startingRow + 1; row < rows; row++)
                {
                    Interval<double> temp = ((dynamic) matrix[row, startingRow]);
                    if(maxValue < temp.Abs())
                    {
                        maxValue = temp;
                        maxIndex = row;
                    }
                }
            }
            else
            {
                double maxValue = 0f;
                for(int row = startingRow + 1; row < rows; row++)
                {
                    double temp = (dynamic)matrix[row, startingRow];
                    if (maxValue < Math.Abs(temp))
                    {
                        maxValue = temp;
                        maxIndex = row;
                    }
                }
            }


            if (maxIndex == -1)
                return;
            PartialPivoting(startingRow, maxIndex);
        }

        private void PartialPivoting(int rowA, int rowB)
        {
            for (int column = 0; column < columns; column++)
            {
                Swap(ref matrix[rowA, column],ref matrix[rowB, column]);
            }
        }
        private void Swap(ref T a,ref T b)
        {
            T temp = a;
            a = b;
            b = temp;
        }

        public void GaussElimination()
        {
            //eliminacja Gaussa
            for (int gw = 0; gw < rows; gw++)
            {
                //wybranie wiersza do ewentualnej zamiany
                //poprzez znalezienie pivota o najwiekszej wartosci bezwzglednej
                TryToPerformPartialPivoting(gw);
                for (int w = gw + 1; w < columns; w++)
                {
                    T m = (dynamic)matrix[w, gw] / matrix[gw, gw];
                    for (int k = gw; k < columns; k++)
                    {
                        matrix[w, k] = matrix[w, k] - (dynamic)m * matrix[gw, k];
                    }
                }
            }


            //uzyskaj 1 na przekątnej poprzez podzielenie każdego elementu w danym wierszu przez wartość
            //elementu na przekątnej tego wiersza
            // i-ty wiersz / matrix[i,i]
            for (int row = 0; row < rows; row++)
            {
                for (int column = 0; column < columns; column++)
                {
                    matrix[row, column] = (dynamic)matrix[row, column] / matrix[row, row];
                }
            }

            //zerowanie gornej macierzy
            for (int gw = rows - 1; gw >= 0; gw--)
            {
                for (int w = gw - 1; w >= 0; w--)
                {
                    T m = (dynamic)matrix[w, gw] / matrix[gw, gw];
                    for (int k = gw; k >= 0; k--)
                    {
                        matrix[w, k] = matrix[w, k] - (dynamic)m * matrix[gw, k];
                    }
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
                    matrix[row, column] = GetGenericRandom(floor, celling);

                }

            }

        }
        Random random;

        private T GetGenericRandom(double floor, double celling)
        {

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
            returnValue += $"Macierz {matrixType}, {this.rows}x{this.columns}\n";
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

        public string PrintMatrix()
        {
            string returnValue = string.Empty;
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

        public string ShortDescription()
        {
            String returnValue = string.Empty;
            returnValue += $"Macierz {matrixType}, {this.rows}x{this.columns}\n";


            return returnValue;
        }
     

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
