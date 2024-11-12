using OxyPlot;
using OxyPlot.Series;
using System;
using System.Windows;

namespace PBL_II_WPF
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Calcular_Click(object sender, RoutedEventArgs e)
        {
            // Ler valores das entradas
            double largura = Convert.ToDouble(txtLargura.Text);
            double velocidadeMotor = Convert.ToDouble(txtVelocidadeMotor.Text);
            double velocidadeCorrenteza = Convert.ToDouble(txtVelocidadeCorrenteza.Text);
            double angulo = Convert.ToDouble(txtAngulo.Text);

            if (!ValidarEntradas(largura, velocidadeMotor, velocidadeCorrenteza, angulo))
                return;

            // Calcular o vetor de velocidade resultante
            double[] vetorVelocidadeRes = CalcularVetorVelocidadeRes(velocidadeMotor, velocidadeCorrenteza, angulo);
            double tempoTravessia = largura / vetorVelocidadeRes[1];

            // Calcular as coordenadas e plotar no gráfico
            PlotarGrafico(vetorVelocidadeRes, tempoTravessia);
        }

        private void PlotarGrafico(double[] vetorVelocidadeRes, double tempoTravessia)
        {
            // Calcular as coordenadas ao longo do tempo
            int numIntervalos = 10;
            double deltaT = tempoTravessia / numIntervalos;
            var points = new LineSeries { Title = "Trajetória" };

            for (int i = 0; i <= numIntervalos; i++)
            {
                double t = i * deltaT;
                double coordX = vetorVelocidadeRes[0] * t;
                double coordY = vetorVelocidadeRes[1] * t;
                points.Points.Add(new DataPoint(coordX, coordY));
            }

            // Configurar o gráfico e exibir
            var plotModel = new PlotModel { Title = "Trajetória do Barco" };
            plotModel.Series.Add(points);
            plotView.Model = plotModel;
        }

        private double[] CalcularVetorVelocidadeRes(double velocidadeMotor, double velocidadeCorrenteza, double angulo)
        {
            double anguloRad = angulo * Math.PI / 180;
            double[] vetorVelocidadeRel = new double[]
            {
                velocidadeMotor * Math.Cos(anguloRad),
                velocidadeMotor * Math.Sin(anguloRad)
            };

            return new double[]
            {
                vetorVelocidadeRel[0] + velocidadeCorrenteza,
                vetorVelocidadeRel[1]
            };
        }

        private bool ValidarIntervalo(double valor, double min, double max, string campo)
        {
            if (valor < min || valor > max)
            {
                MessageBox.Show($"Por favor, insira um valor entre {min} e {max} para {campo}.");
                return false;
            }
            return true;
        }

        private bool ValidarEntradas(double largura, double velocidadeMotor, double velocidadeCorrenteza, double angulo)
        {
            if (!ValidarIntervalo(largura, 20, 100, "largura")) return false;
            if (!ValidarIntervalo(velocidadeMotor, 2, 10, "velocidade do motor")) return false;
            if (!ValidarIntervalo(velocidadeCorrenteza, 1, 4, "velocidade da correnteza")) return false;
            if (!ValidarIntervalo(angulo, 20, 160, "ângulo")) return false;

            return true;
        }
    }
}
