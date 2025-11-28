
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Shapes;
using System.Globalization;
using System.Net.Http.Json;

namespace VillagioApp
{
    public partial class HorariosPage : ContentPage
    {
        private static readonly HttpClient Http = new HttpClient
        {
            BaseAddress = new Uri("http://villagioapi.runasp.net/") // ajuste se necessário
        };

        private readonly DateTime _dataSelecionada;
        private readonly int _tipoUsuarioId; // 1 = Família, 2 = Agência
        private readonly int _visitantes;

        public HorariosPage(DateTime dataSelecionada, int tipoUsuarioId, int visitantes)
        {
            InitializeComponent();
            _dataSelecionada = dataSelecionada;
            _tipoUsuarioId = tipoUsuarioId;
            _visitantes = visitantes;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            _ = GerarConteudoDiaAsync();
        }

        private async void BtnRecarregar_Clicked(object sender, EventArgs e)
        {
            await GerarConteudoDiaAsync();
        }

        private async Task GerarConteudoDiaAsync()
        {
            try
            {
                LoadingIndicator.IsVisible = true;
                LoadingIndicator.IsRunning = true;

                SemanasLayout.Children.Clear();

                // Cabeçalho com dia
                string diaSemana = _dataSelecionada.ToString("dddd", new CultureInfo("pt-BR"));
                string dataFormatada = _dataSelecionada.ToString("dd/MM");
                TituloDiaHeader.Text = $"{CultureInfo.CurrentCulture.TextInfo.ToTitleCase(diaSemana)} - {dataFormatada}";

                // Borda título
                var tituloBorder = new Border
                {
                    BackgroundColor = Color.FromArgb("#F0F0F0"),
                    Stroke = Colors.Purple,
                    StrokeThickness = 1,
                    StrokeShape = new RoundRectangle { CornerRadius = 10 },
                    Padding = 16,
                    Content = new Label
                    {
                        Text = _tipoUsuarioId == 2
                            ? "Agência: selecione um horário totalmente livre (50 vagas)."
                            : $"Família: selecione um horário com vagas suficientes ({_visitantes}).",
                        FontSize = 14,
                        HorizontalOptions = LayoutOptions.Center,
                        TextColor = Colors.Black
                    }
                };
                SemanasLayout.Children.Add(tituloBorder);

                // Busca horários disponíveis na API
                var horariosDisponiveis = await ObterHorariosDisponiveis();

                // Mensagem se não houver horários
                if (horariosDisponiveis.Count == 0)
                {
                    SemanasLayout.Children.Add(new Border
                    {
                        BackgroundColor = Color.FromArgb("#FFF3CD"),
                        Stroke = Colors.Orange,
                        StrokeThickness = 1,
                        StrokeShape = new RoundRectangle { CornerRadius = 10 },
                        Padding = 16,
                        Content = new Label
                        {
                            Text = "Nenhum horário disponível para esta data.",
                            TextColor = Colors.Black,
                            HorizontalOptions = LayoutOptions.Center
                        }
                    });
                    return;
                }

                // Lista de horários (botões)
                var layoutHorarios = new VerticalStackLayout { Spacing = 10 };

                foreach (var h in horariosDisponiveis.OrderBy(x => x.Horario))
                {
                    var (bg, border, textColor) = CoresPorVagas(h.VagasDisponiveis);

                    var btn = new Button
                    {
                        Text = $"{h.Horario} ({h.VagasDisponiveis} vagas)",
                        BackgroundColor = bg,
                        TextColor = textColor,
                        CornerRadius = 10,
                        HeightRequest = 48,
                        BorderColor = border,
                        BorderWidth = 2,
                        FontAttributes = FontAttributes.Bold
                    };

                    btn.Clicked += async (s, e) =>
                    {
                        // Navega para ReservaPage levando os parâmetros
                        await Navigation.PushAsync(new ReservaPage(_dataSelecionada, h.Horario, _visitantes, _tipoUsuarioId));
                    };

                    layoutHorarios.Children.Add(btn);
                }

                SemanasLayout.Children.Add(layoutHorarios);
            }
            catch (Exception ex)
            {
                await DisplayAlert("Erro", $"Falha ao carregar horários: {ex.Message}", "OK");
            }
            finally
            {
                LoadingIndicator.IsRunning = false;
                LoadingIndicator.IsVisible = false;
            }
        }

        private async Task<List<HorarioDisponivel>> ObterHorariosDisponiveis()
        {
            var url = $"api/Reservas/disponibilidade?data={_dataSelecionada:yyyy-MM-dd}&quantidade={_visitantes}&tipoUsuarioId={_tipoUsuarioId}";
            var response = await Http.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                var erro = await response.Content.ReadAsStringAsync();
                throw new InvalidOperationException($"API retornou {response.StatusCode}: {erro}");
            }

            var lista = await response.Content.ReadFromJsonAsync<List<HorarioDisponivel>>();
            return lista ?? new List<HorarioDisponivel>();
        }

        /// <summary>
        /// Retorna paleta de cores para o botão conforme vagas restantes.
        /// </summary>
        private static (Color bg, Color border, Color text) CoresPorVagas(int vagas)
        {
            if (vagas >= 30)
                return (Color.FromArgb("#DFFFD6"), Colors.Green, Colors.Black); // bastante vaga
            if (vagas >= 10)
                return (Color.FromArgb("#FFFBE6"), Colors.Goldenrod, Colors.Black); // médio
            return (Color.FromArgb("#FFE8E6"), Colors.DarkRed, Colors.Black); // poucas vagas
        }
    }

    public class HorarioDisponivel
    {
        public string Horario { get; set; } = string.Empty;
        public int VagasDisponiveis { get; set; }
    }
}
