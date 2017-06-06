﻿using System;
using System.Windows;
using System.Linq;
using MathNet.Numerics.LinearAlgebra;
using EliminacjaGJ_CSharp.Class;
using System.Windows.Controls;
using System.Data;
using System.Collections.Generic;
using System.Windows.Media;
//using FesetroundLibrary;
//using System.Numerics.MPFR;

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
            //Application.Current.MainWindow
            //matrixa = MyMatrix<Interval<double>>.GenerateRandomMatrix(3, 3, 1, 21);
            //matrixa = new MyMatrix<Interval<double>>(n, n);
            //matrixa[0, 0] = new Interval<double>(1f);
            //matrixa[0, 1] = new Interval<double>(1f);
            //matrixa[0, 2] = new Interval<double>(1f);
            //matrixa[1, 0] = new Interval<double>(2f);
            //matrixa[1, 1] = new Interval<double>(3f);
            //matrixa[1, 2] = new Interval<double>(5f);
            //matrixa[2, 0] = new Interval<double>(4f);
            //matrixa[2, 1] = new Interval<double>(0);
            //matrixa[2, 2] = new Interval<double>(5f);


        }

        int n = 3;

        //Vector<Interval<double>> vectorC;
        Interval<float> intervalA;
        Interval<float> intervalB;





        private void CheckInterval(object sender, RoutedEventArgs e)
        {
            Interval<double> a = new Interval<double>(-10, 10);
            Interval<double> b = new Interval<double>(2, 8);
            b = a / b;

            AlertUser(b.ToString());
        }


        private void Gauss(object sender, RoutedEventArgs e)
        {

            if (parsingError)
            {
                AlertUser("Błąd podczas przetwarzania macierzy!");
                return;
            }

            MyMatrix<double> matrixA = new MyMatrix<double>(n);
            MyMatrix<Interval<double>> matrixB = new MyMatrix<Interval<double>>(n);
            List<string> numbersFromUserMatrix;
            try
            {
                numbersFromUserMatrix = TryToLoadFromUserMatrix();
            }
            catch (ArgumentException ex)
            {
                AlertUser(ex.Message);
                return;
            }

            if (intervalMode)
            {
                try
                {
                    matrixA.LoadFromList(numbersFromUserMatrix);
                    matrixB.LoadFromList(numbersFromUserMatrix);

                }catch(ArgumentException ex)
                {
                    AlertUser(ex.Message);
                    return;
                }catch(FormatException ex)
                {
                    AlertUser(ex.Message);
                    return;
                }
                AlertUser(matrixB.ToString());
                AlertUser("interval mode active");
            }
            else
            {
                AlertUser("interval mode unactive");
                return;

            }


            try
            {
                matrixA.GaussElimination();
                AlertUser($"Udało się wygenerować macierz double! {matrixA}");

                matrixB.GaussElimination();
                AlertUser($"Udało się wygenerować macierz z INTERWAŁAMI! {matrixB}");
            }catch(DivideByZeroException ex)
            {
                AlertUser(ex.Message);
            }


            //Console.Write($"{matrixIntervalDouble} {matrixIntervalInt} {matrixDouble} {matrixInt}");            

            //Console.WriteLine("------------matrix interval double ----------");
            //Console.WriteLine(matrixa);
            //try
            //{

            //    matrixa.GaussElimination();
            //}
            //catch (DivideByZeroException ex)
            //{
            //    Console.WriteLine("matrix interval double - Macierz osobliwa!");
            //}
            //Console.WriteLine(matrixa);


            //Console.WriteLine("------------matrix interval int----------");

            //try
            //{

            //    matrixb.GaussElimination();
            //}
            //catch (DivideByZeroException ex)
            //{
            //    Console.WriteLine("matrix interval int - Macierz osobliwa!");
            //}

        }

        private List<string> TryToLoadFromUserMatrix()
        {
            if (matrixCells.Count < 1)
                throw new ArgumentException("Brak elementów do wczytania!");

            var stringElements = from item in matrixCells
                                 select item.Text;


            //foreach (TextBox item in matrixCells)
            //{
            //    var state = Double.TryParse(item.Text, out tempNumber);
            //    if (!state)
            //        throw new FormatException($"Błąd podczas wczytywania elementu - {item.Text}");

            //    elements.Add(tempNumber);
            //}
            return stringElements.ToList();
        }

        private void TryReadUserMatrix()
        {
            if (matrixCells.Count < 1)
            {
                AlertUser("");
            }
        }


        bool intervalMode = false;
        bool parsingError = true;
        List<TextBox> matrixCells = new List<TextBox>();

        private void button_Click(object sender, RoutedEventArgs e)
        {
            if(userMatrixText.Text == string.Empty)
            {
                AlertUser("Brak tekstu!");
                return;
            }
            //userMatrixGrid
            if (!Int32.TryParse(userMatrixText.Text, out n))
            {
                AlertUser("Błąd parsowania");
                return;
            }
            if (n > 10 || n <= 0)
            {
                AlertUser("Podaj liczbę wymiarów z zakresu [1,10]");
                return;
            }

            parsingError = false;
            if (matrixCells.Count > 0)
            {
                AlertUser("Destroying current matrix");
                DestroyActiveTextBoxes();
            }

            for (int row = 0; row < n; row++)
            {
                for (int column = 0; column < n; column++)
                {
                    TextBox newTextBox = new TextBox();

                    //dimensions
                    newTextBox.Margin = new Thickness((column - n / 2) * 150, (row - n / 2) * 100, 0, 0);
                    newTextBox.Height = 35;
                    newTextBox.Width = 40;
                    newTextBox.FontSize = 15;
                    newTextBox.MaxLength = 5;
                    newTextBox.MaxLines = 1;

                    //colors
                    newTextBox.Background = new SolidColorBrush(Color.FromRgb(51, 51, 51));
                    newTextBox.Foreground = Brushes.White;
                    newTextBox.BorderBrush = Brushes.White;
                    newTextBox.BorderThickness = new Thickness(2);

                    //alignment
                    newTextBox.TextAlignment = TextAlignment.Center;
                    newTextBox.HorizontalAlignment = HorizontalAlignment.Center;
                    newTextBox.VerticalAlignment = VerticalAlignment.Stretch;
                    newTextBox.VerticalContentAlignment = VerticalAlignment.Center;
                    newTextBox.TabIndex = row * n + column;
                    newTextBox.Text = "0.0";

                    //clear when clicked fro the first time
                    newTextBox.PreviewMouseLeftButtonDown += MouseClick;
                    matrixCells.Add(newTextBox);

                    userMatrixGrid.Children.Add(newTextBox);
                }
            }

            foreach (TextBox item in matrixCells)
            {
                AlertUser(item.Text);
            }

        }

        private void MouseClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            ((TextBox)sender).PreviewMouseLeftButtonDown -= MouseClick;
            ((TextBox)sender).Text = "";
        }

        private void DestroyActiveTextBoxes()
        {
            if(userMatrixGrid.Children.Count < 1)
            {
                AlertUser("Brak elementów macierzy do usunięcia");
                return;
            }
            userMatrixGrid.Children.Clear();
            matrixCells.Clear();
        }

        private void ClearUserMatrix()
        {
            if (userMatrixGrid.Children.Count < 1)
            {
                AlertUser("Brak macierzy do wyczyszczenia");
                return;

            }
            foreach (TextBox item in matrixCells)
            {
                item.Text = "";
            }
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            DestroyActiveTextBoxes();
        }

        private void clearUserMatrix_Click(object sender, RoutedEventArgs e)
        {
            ClearUserMatrix();
        }

        private void checkBox_Checked(object sender, RoutedEventArgs e)
        {
            intervalMode = true;
        }

        private void checkBox_Unchecked(object sender, RoutedEventArgs e)
        {
            intervalMode = false;
        }

        private void userMatrixCalculateButton_Click(object sender, RoutedEventArgs e)
        {
            AlertUser("mleko");
        }

        public MainWindow GetInstance()
        {
            return this;
        }
        
        bool debug = false;
        private void AlertUser(string message)
        {
            if(debug)
            {
                Console.WriteLine(message);

            }else
            {
                messageLog.Text = message;
            }
        }

        private void generateRandomMatrix_Click(object sender, RoutedEventArgs e)
        {
         //   MyMatrix<Interval<double>> matrixA = new MyMatrix<Interval<double>>();

        }
    }
}
