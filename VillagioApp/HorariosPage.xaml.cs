using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Shapes;
using System.Globalization;

namespace VillagioApp
{
    public partial class HorariosPage : ContentPage
    {
        private readonly DateTime _dataSelecionada;

        public HorariosPage(DateTime dataSelecionada)
        {
            InitializeComponent();
            _dataSelecionada = dataSelecionada;
            GerarConteudoDia();
        }

        private void GerarConteudoDia()
        {
            SemanasLayout.Children.Clear();

            string diaSemana = _dataSelecionada.ToString("dddd", new CultureInfo("pt-BR"));
            string dataFormatada = _dataSelecionada.ToString("dd/MM");

            var borderTitulo = new Border
            {
                BackgroundColor = Color.FromArgb("#F0F0F0"),
                Stroke = Colors.Purple,
                StrokeThickness = 1,
                StrokeShape = new RoundRectangle { CornerRadius = 10 },
                WidthRequest = 300,
                Padding = 20,
                Margin = new Thickness(20, 10, 20, 0),
                Content = new Label
                {
                    Text = $"{CultureInfo.CurrentCulture.TextInfo.ToTitleCase(diaSemana)} - {dataFormatada}",
                    FontSize = 18,
                    FontAttributes = FontAttributes.Bold,
                    HorizontalOptions = LayoutOptions.Center,
                    
                    TextColor = Colors.Black
                }
            };

            SemanasLayout.Children.Add(borderTitulo);

            bool ehFinalDeSemana = diaSemana == "sábado" || diaSemana == "domingo";

            var borderConteudo = new Border
            {
                BackgroundColor = Color.FromArgb("#F0F0F0"),
                Stroke = Colors.Purple,
                StrokeThickness = 1,
                StrokeShape = new RoundRectangle { CornerRadius = 15 },
                Padding = 10,
                Margin = new Thickness(20, 10, 20, 20),
                WidthRequest = 400,
                Content = CriarHorariosDia(ehFinalDeSemana)
            };

            SemanasLayout.Children.Add(borderConteudo);

            var navegacaoLayout = new HorizontalStackLayout
            {
                HorizontalOptions = LayoutOptions.Center,
                Spacing = 10,
                Margin = new Thickness(0, 10, 0, 10)
            };

            var btnAnterior = new Button
            {
                Text = "◀ Dia anterior",
                FontSize = 10,
                WidthRequest = 120
            };
            btnAnterior.Clicked += (s, e) =>
            {
                DateTime anterior = _dataSelecionada.AddDays(-1);
                while (anterior.DayOfWeek != DayOfWeek.Saturday && anterior.DayOfWeek != DayOfWeek.Sunday)
                {
                    anterior = anterior.AddDays(-1);
                }
                Navigation.PushAsync(new HorariosPage(anterior));
            };

            var btnProximo = new Button
            {
                Text = "Dia seguinte ▶",
                FontSize = 10,
                WidthRequest = 120
            };
            btnProximo.Clicked += (s, e) =>
            {
                DateTime proximo = _dataSelecionada.AddDays(1);
                while (proximo.DayOfWeek != DayOfWeek.Saturday && proximo.DayOfWeek != DayOfWeek.Sunday)
                {
                    proximo = proximo.AddDays(1);
                }
                Navigation.PushAsync(new HorariosPage(proximo));
            };

            navegacaoLayout.Children.Add(btnAnterior);
            navegacaoLayout.Children.Add(btnProximo);

            SemanasLayout.Children.Add(navegacaoLayout);
        }

        private VerticalStackLayout CriarHorariosDia(bool habilitado)
        {
            var horarios = new[]
            {
                "08:00", "09:00", "10:00", "11:00",
                "12:00", "13:00", "14:00", "15:00",
                "16:00", "17:00"
            };

            var layout = new VerticalStackLayout { Spacing = 5 };
            int porLinha = 4;

            for (int i = 0; i < horarios.Length; i += porLinha)
            {
                var linha = new HorizontalStackLayout
                {
                    Spacing = 5,
                    HorizontalOptions = LayoutOptions.Center
                };

                for (int j = i; j < i + porLinha && j < horarios.Length; j++)
                {
                    var horario = horarios[j];
                    var btn = new Button
                    {
                        Text = horario,
                        BackgroundColor = habilitado ? Colors.White : Colors.Gray,
                        TextColor = Colors.Black,
                        CornerRadius = 10,
                        WidthRequest = 70,
                        HeightRequest = 40,
                        BorderColor = Colors.Green,
                        BorderWidth = 2,
                        IsEnabled = habilitado
                    };

                    if (habilitado)
                    {
                        btn.Clicked += async (s, e) =>
                        {
                            await Navigation.PushAsync(new ReservaPage(_dataSelecionada, horario));
                        };
                    }

                    linha.Children.Add(btn);
                }

                layout.Children.Add(linha);
            }

            return layout;
        }
    }
}