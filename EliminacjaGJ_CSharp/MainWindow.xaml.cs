using System;
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

        }

        int n = 3;
        bool debug = false;


        private void Gauss(object sender, RoutedEventArgs e)
        {

            if (matrixCells.Count < 0)
            {
                AlertUser("Brak macierzy użytkownika do przetworzenia!");
            }
            if (parsingError)
            {
                AlertUser("Błąd podczas przetwarzania macierzy!");
                return;
            }

            if (n < 0)
            {
                n = (int)Math.Sqrt(matrixCells.Count);
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

                }
                catch (ArgumentException ex)
                {
                    AlertUser(ex.Message);
                    return;
                }
                catch (FormatException ex)
                {
                    AlertUser(ex.Message);
                    return;
                }
                AlertUser(matrixB.ToString());
                AlertUser("interval mode active");
            }
            else
            {
                matrixA.LoadFromList(numbersFromUserMatrix);
                double meanError = 0f;
                double minError = 0f;
                double maxError = 0f;

                ClearErrorBox();
                matrixA.GaussElimination();
                matrixA.CalculateError(ref meanError, ref minError, ref maxError);
                double mean = meanError;
                AddToErrorBoxBuffer($"Udało się wygenerować macierz \n {matrixA: e},\n średni błąd = {mean: e}");
                AddToErrorBoxBuffer($"Błąd minimalny = {minError: e}");
                AddToErrorBoxBuffer($"Błąd maksymalny = {maxError: e}");
                WriteToErrorBox();
                return;

            }


            try
            {
                double meanError = 0f;
                double minError = 0f;
                double maxError = 0f;

                ClearErrorBox();
                matrixA.GaussElimination();
                matrixA.CalculateError(ref meanError, ref minError, ref maxError);
                AddToErrorBoxBuffer($"Udało się wygenerować macierz \n {matrixA},\n średni błąd = {meanError:e}");
                AlertUser($"błąd minimalny = { minError:e}");
                AlertUser($"błąd maksymalny = { maxError:e}");
                matrixB.GaussElimination();

                matrixA.CalculateError(ref meanError, ref minError, ref maxError);
                AddToErrorBoxBuffer($"Udało się wygenerować macierz z\n {matrixB},\n średni błąd = {meanError:e}");
                AddToErrorBoxBuffer($"błąd minimalny = { minError:e}");
                AddToErrorBoxBuffer($"błąd maksymalny = { maxError:e}");
                WriteToErrorBox();


                ClearUserMatrixOutput();
                if (intervalMode)
                {
                    PrintUserMatrix(matrixA.PrintMatrix());

                }

                else
                {
                    PrintUserMatrix(matrixB.PrintMatrix());

                }

            }
            catch (DivideByZeroException ex)
            {
                AlertUser(ex.Message);
            }
        }

        private List<string> TryToLoadFromUserMatrix()
        {
            if (matrixCells.Count < 1)
                throw new ArgumentException("Brak elementów do wczytania!");

            var stringElements = from item in matrixCells
                                 select item.Text;


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
            if (!ReadNumberFromField(userMatrixText, 1, 10))
                return;

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
                    newTextBox.PreviewGotKeyboardFocus += NewTextBox_PreviewGotKeyboardFocus;

                    matrixCells.Add(newTextBox);

                    userMatrixGrid.Children.Add(newTextBox);
                }
            }

            //foreach (TextBox item in matrixCells)
            //{
            //    AlertUser(item.Text);
            //}

        }

        private bool ReadNumberFromField(TextBox fieldWithNumberToParse, int bottomBorder, int upperBorder)
        {
            if (fieldWithNumberToParse.Text == string.Empty)
            {
                AlertUser("Brak tekstu!");
                return false;
            }
            //userMatrixGrid
            if (!Int32.TryParse(fieldWithNumberToParse.Text, out n))
            {
                AlertUser("Błąd parsowania");
                return false;
            }
            if (n < bottomBorder || n > upperBorder)
            {
                AlertUser("Podaj liczbę wymiarów z zakresu [1,10] dla macierzy wpisywanej przez użytkownika, lub liczbę z przedziału [1,2000] dla macierzy generowanej losowo");
                return false;
            }
            return true;
        }

        //tab clearing
        private void NewTextBox_PreviewGotKeyboardFocus(object sender, System.Windows.Input.KeyboardFocusChangedEventArgs e)
        {
            ((TextBox)sender).PreviewGotKeyboardFocus -= NewTextBox_PreviewGotKeyboardFocus;
            ((TextBox)sender).Text = "";
        }

        //left mouse click clearing
        private void MouseClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            ((TextBox)sender).PreviewMouseLeftButtonDown -= MouseClick;
            ((TextBox)sender).Text = "";
        }

        private void DestroyActiveTextBoxes()
        {
            if (userMatrixGrid.Children.Count < 1)
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

        
        private void generateRandomMatrix_Click(object sender, RoutedEventArgs e)
        {
            DestroyActiveTextBoxes();

            bool success = false;
            int iterations = 0;
            do
            {

                ClearErrorBox();
                //generator liczb, używany przy nierozpoznaniu wielkości macierzy podanej przez użytkownika
                Random randomMatrixRank = new Random(DateTime.Now.Millisecond);

                if (!ReadNumberFromField(randomMatrixText, 1, 2000))
                {
                    n = randomMatrixRank.Next(10, 25);
                    AlertUser($"Błąd przy odczytywaniu rzędu macierzy, używam losowej liczby n = {n}");
                }
                try
                {
                    double meanError = 0f;
                    double minError = 0f;
                    double maxError = 0f;


                    if (intervalMode)   //posługujemy się arytmetyki przedziałową
                    {
                        MyMatrix<Interval<double>> matrixA = new MyMatrix<Interval<double>>(n);
                        matrixA.GenerateRandomMatrix(1, 1000);   //wygeneruj macierz o przedziale z zakresu od 1 do 100
                        matrixA.GaussElimination();
                        if (n < 10)
                            AddToErrorBoxBuffer($"Udało się wykonać eliminację dla interwału! {matrixA}");
                        else
                            AddToErrorBoxBuffer("Udało się wykonać eliminację");

                        matrixA.CalculateError(ref meanError, ref minError, ref maxError);
                        string temp = $"{meanError:e}";

                        AddToErrorBoxBuffer($"średni błąd - { temp}");
                        AddToErrorBoxBuffer($"błąd minimalny - { minError:e}");
                        AddToErrorBoxBuffer($"błąd maksymalny - { maxError:e}");
                        WriteToErrorBox();


                        MyMatrix<double> matrixB = new MyMatrix<double>(n);
                        //wygeneruj macierz o liczbach z zakresu od 1-100, dla typu generycznego double są losowane elementy macierzy typu double
                        matrixA.GenerateRandomMatrix(1, 1000);
                        matrixA.GaussElimination();
                        if (n < 10)
                            AddToErrorBoxBuffer($"Udało się wykonać eliinację dla typu double! {matrixA}");
                        else
                            AddToErrorBoxBuffer("Udało się wykonać eliminację");

                        matrixA.CalculateError(ref meanError, ref minError, ref maxError);

                        string temp2 = $"{meanError:e}";
                        AddToErrorBoxBuffer($"średni błąd = { temp2}");
                        AddToErrorBoxBuffer($"błąd minimalny = { minError:e}");
                        AddToErrorBoxBuffer($"błąd maksymalny = { maxError:e}");
                        WriteToErrorBox();
                    }
                    else //nie posługujemy się arytmetyki przedziałową
                    {
                        MyMatrix<double> matrixA = new MyMatrix<double>(n);
                        //wygeneruj macierz o liczbach z zakresu od 1-100, dla typu generycznego double są losowane elementy macierzy typu double
                        matrixA.GenerateRandomMatrix(1, 1000);
                        matrixA.GaussElimination();
                        if (n < 10)
                            AddToErrorBoxBuffer($"Udało się wykonać eliinację dla typu double! {matrixA}");
                        else
                            AddToErrorBoxBuffer("Udało się wykonać eliminację");

                        matrixA.CalculateError(ref meanError, ref minError, ref maxError);

                        string temp = $"{meanError:e}";
                        AddToErrorBoxBuffer($"średni błąd = { temp}");
                        AddToErrorBoxBuffer($"błąd minimalny = { minError:e}");
                        AddToErrorBoxBuffer($"błąd maksymalny = { maxError:e}");
                        WriteToErrorBox();
                    }
                    success = true;

                }
                catch (FormatException ex)
                {
                    AlertUser(ex.Message);  //zły typ danych
                }
                catch (ArgumentException ex)
                {
                    AlertUser(ex.Message); //zły argument  - np błąd parsowania liczby
                }
                catch (DivideByZeroException ex)
                {
                    AlertUser(ex.Message);  //błąd arytmetyki przedziałowej
                }
                finally
                {
                    iterations++;
                    Console.WriteLine($"Iteration: {iterations}");
                    n = -1;
                }


            } while (!success && iterations < 10);
            if (success)
                MessageBox.Show($"Dokonano eliminacji zupełnej w {iterations} iteracji.");
            else
                MessageBox.Show("Nie udało się wykonać eliminacji");
        }

        private void clearMessageLog_Click(object sender, RoutedEventArgs e)
        {
            messageLog.Text = "";
            ClearErrorBox();
        }

        void ClearErrorBox()
        {
            errorTextBox.Text = string.Empty;
            buffer = string.Empty;
        }

        void WriteToErrorBox()
        { errorTextBox.Text = buffer; }

        string buffer = string.Empty;
        void AddToErrorBoxBuffer(string text)
        {
            buffer += Environment.NewLine;
            buffer += text;
        }

        void PrintUserMatrix(string text)
        {
            userMatrixOutputTextBox.Text = text;
        }
        void ClearUserMatrixOutput()
        {
            userMatrixOutputTextBox.Text = string.Empty;
        }


        private void AlertUser(string message)
        {
            if (debug)
            {
                Console.WriteLine(message);

            }
            else
            {
                if (messageLog.Text.Count() > 500)
                    clearMessageLog_Click(null, null);
                messageLog.Text += string.Format("{0}{1}", message, Environment.NewLine);
            }
        }
    }
}
