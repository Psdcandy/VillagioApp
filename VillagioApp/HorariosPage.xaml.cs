
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Shapes;
using System.Globalization;
using System.Net.Http.Json;

namespace VillagioApp
{
    public partial class HorariosPage : ContentPage
    {
        private readonly DateTime _dataSelecionada;
        private readonly int _tipoUsuarioId;
        private readonly int _visitantes;

        public HorariosPage(DateTime dataSelecionada, int tipoUsuarioId, int visitantes)
        {
            InitializeComponent();
            _dataSelecionada = dataSelecionada;
            _tipoUsuarioId = tipoUsuarioId;
            _visitantes = visitantes;

            _ = GerarConteudoDiaAsync();
        }

        private async Task GerarConteudoDiaAsync()
        {
            SemanasLayout.Children.Clear();

            string diaSemana = _dataSelecionada.ToString("dddd", new CultureInfo("pt-BR"));
            string dataFormatada = _dataSelecionada.ToString("dd/MM");

            SemanasLayout.Children.Add(new Border
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
            });

            var horariosDisponiveis = await ObterHorariosDisponiveis();

            var layoutHorarios = new VerticalStackLayout { Spacing = 10 };

            foreach (var h in horariosDisponiveis)
            {
                var btn = new Button
                {
                    Text = $"{h.Horario} ({h.VagasDisponiveis} vagas)",
                    BackgroundColor = Colors.White,
                    TextColor = Colors.Black,
                    CornerRadius = 10,
                    WidthRequest = 150,
                    HeightRequest = 50,
                    BorderColor = Colors.Green,
                    BorderWidth = 2
                };

                btn.Clicked += async (s, e) =>
                {
                    await Navigation.PushAsync(new ReservaPage(_dataSelecionada, h.Horario, _visitantes, _tipoUsuarioId));
                };

                layoutHorarios.Children.Add(btn);
            }

            SemanasLayout.Children.Add(layoutHorarios);
        }

        private async Task<List<HorarioDisponivel>> ObterHorariosDisponiveis()
        {
            using var client = new HttpClient { BaseAddress = new Uri("http://villagioapi.runasp.net/") };
            var response = await client.GetAsync($"api/Reservas/disponibilidade?data={_dataSelecionada:yyyy-MM-dd}&quantidade={_visitantes}&tipoUsuarioId={_tipoUsuarioId}");
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<List<HorarioDisponivel>>();
            }
            return new List<HorarioDisponivel>();
        }
    }

    public class HorarioDisponivel
    {
        public string Horario { get; set; }
        public int VagasDisponiveis { get; set; }
    }
}
