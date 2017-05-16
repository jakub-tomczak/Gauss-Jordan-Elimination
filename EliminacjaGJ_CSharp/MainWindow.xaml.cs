using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MathNet.Numerics.LinearAlgebra;

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

        private void DisplayMatrix(int dim)
        {
            //for(int i=1;i<dim;i++)
            //{
            //    for(int j=1;j<dim;j++)
            //    {
            //        Console.Write(string.Format("{1}\t", matrix[i, j]));
            //    }
            //    Console.Write("\n");
            //}

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

        private void Gauss(object sender, RoutedEventArgs e)
        {
            Gauss();
            DisplayMatrix(n);
        }
    }
}
