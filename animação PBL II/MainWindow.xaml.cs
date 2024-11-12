using OxyPlot;
using OxyPlot.Series;
using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Threading;

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

            // Calcular as coordenadas da trajetória do barco
            var pontosTrajeto = CalcularCoordenadas(vetorVelocidadeRes, tempoTravessia);

            // Iniciar a animação do barco
            IniciarAnimacaoBarco(pontosTrajeto, tempoTravessia);
        }

        private void IniciarAnimacaoBarco(System.Collections.Generic.List<System.Windows.Point> pontosTrajeto, double tempoTravessia)
        {
            // Limpar o gráfico
            var points = new LineSeries { Title = "Trajetória" };

            foreach (var ponto in pontosTrajeto)
            {
                points.Points.Add(new DataPoint(ponto.X, ponto.Y));
            }

            var plotModel = new PlotModel { Title = "Trajetória do Barco" };
            plotModel.Series.Add(points);
            plotView.Model = plotModel;

            // Animação do Barco
            double totalTempo = tempoTravessia;
            double intervaloTempo = totalTempo / pontosTrajeto.Count;
            int i = 0;

            // Definir o início da animação
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(intervaloTempo);
            timer.Tick += (s, e) =>
            {
                if (i < pontosTrajeto.Count)
                {
                    // Obter as coordenadas do ponto a ser movido
                    var ponto = pontosTrajeto[i];
                    // Animar a posição do barco
                    AnimarBarco(ponto.X, ponto.Y);
                    i++;
                }
                else
                {
                    timer.Stop(); // Parar a animação quando atingir o último ponto
                }
            };

            // Iniciar o timer
            timer.Start();
        }

        private void AnimarBarco(double posX, double posY)
        {
            // Acessar o TranslateTransform diretamente
            var translateTransform = ((TranslateTransform)((TransformGroup)Barco.RenderTransform).Children[3]);

            // Animação para o movimento no eixo X (para atravessar o rio)
            DoubleAnimation animX = new DoubleAnimation
            {
                To = posX,
                Duration = TimeSpan.FromMilliseconds(500), // Velocidade da animação
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseInOut }
            };

            // Animação para o movimento no eixo Y (com a correnteza)
            DoubleAnimation animY = new DoubleAnimation
            {
                To = posY,
                Duration = TimeSpan.FromMilliseconds(500),
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseInOut }
            };

            // Aplicar as animações
            translateTransform.BeginAnimation(TranslateTransform.XProperty, animX);
            translateTransform.BeginAnimation(TranslateTransform.YProperty, animY);
        }

        private System.Collections.Generic.List<System.Windows.Point> CalcularCoordenadas(double[] vetorVelocidadeRes, double tempoTravessia)
        {
            int numIntervalos = 10;
            double deltaT = tempoTravessia / numIntervalos;
            var pontos = new System.Collections.Generic.List<System.Windows.Point>();

            for (int i = 0; i <= numIntervalos; i++)
            {
                double t = i * deltaT;

                double coordX = vetorVelocidadeRes[0] * t;
                double coordY = vetorVelocidadeRes[1] * t;

                pontos.Add(new System.Windows.Point(coordX, coordY));
            }

            return pontos;
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
