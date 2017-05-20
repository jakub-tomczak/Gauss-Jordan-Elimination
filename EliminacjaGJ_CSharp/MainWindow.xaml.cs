using System;
using System.Windows;
using MathNet.Numerics.LinearAlgebra;
using EliminacjaGJ_CSharp.Class;
using FesetroundLibrary;
using System.Numerics.MPFR;

namespace EliminacjaGJ_CSharp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        const int n = 100;
        Matrix<double> matrix = Matrix<double>.Build.Random(n, n);
        Vector<double> vectorB = Vector<double>.Build.Random(n);
        
        Interval<float> intervalA;
        Interval<float> intervalB;


        private void DisplayMatrix(int dim)
        {
            Console.WriteLine(matrix.ToString(n, n));
        }




        public void Gauss()
        {
            
            for (int gw = 0; gw < n - 1; gw++)
            {
                for (int w = gw + 1; w < n; w++)
                {
                    double m = matrix[w, gw] / matrix[gw,gw];
                    for(int k = gw; k<n;k++)
                    {
                        matrix[w, k] = matrix[w, k] - m * matrix[gw, k];
                    }
                    vectorB[w] = vectorB[w] - m * vectorB[gw];
                }
            }


        }

        private void CheckInterval(object sender, RoutedEventArgs e)
        {
            Interval<double> a = new Interval<double>(-10,10);
            Interval<double> b = new Interval<double>(2,8);
            b = a * b;

            Console.WriteLine(b);
        }


        private void Gauss(object sender, RoutedEventArgs e)
        {
            Gauss();
            DisplayMatrix(n);
        }
    }
}
