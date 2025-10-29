using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Shapes;
using System.Globalization;

namespace VillagioApp;

public partial class MiniCalendarioPage : ContentPage
{
    private int _mesAtual;
    private int _anoAtual;
    private readonly DateTime _dataInicial;
    private readonly DateTime _dataFinal;
    private DateTime? _diaSelecionado;

    private readonly Dictionary<int, List<string>> frutasPorMes = new()
    {
        { 1, new() { "uva", "goiaba", "morango", "lichia" } },
        { 2, new() { "uva", "goiaba", "morango" } },
        { 3, new() { "uva", "goiaba" } },
        { 4, new() { "goiaba", "morango" } },
        { 5, new() { "uva", "goiaba", "morango" } },
        { 6, new() { "uva", "goiaba", "morango" } },
        { 7, new() { "uva", "goiaba", "morango" } },
        { 8, new() { "morango" } },
        { 9, new() { "morango" } },
        { 10, new() { "pêssego", "morango", "goiaba" } },
        { 11, new() { "pêssego", "morango", "goiaba" } },
        { 12, new() { "uva", "goiaba", "morango", "lichia" } }
    };

    private readonly List<string> todasFrutas = new()
    {
        "uva", "goiaba", "morango", "lichia", "pêssego"
    };

    public MiniCalendarioPage(string nomeMes, int mes, int ano)
    {
        InitializeComponent();

        _mesAtual = mes;
        _anoAtual = ano;

        _dataInicial = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
        _dataFinal = _dataInicial.AddMonths(5);

        BtnAnterior.Clicked += BtnAnterior_Clicked;
        BtnProximo.Clicked += BtnProximo_Clicked;
        BtnProsseguir.Clicked += BtnProsseguir_Clicked;

        AtualizarCalendario();
    }

    private void BtnAnterior_Clicked(object sender, EventArgs e)
    {
        var novaData = new DateTime(_anoAtual, _mesAtual, 1).AddMonths(-1);
        if (novaData >= _dataInicial)
        {
            _mesAtual = novaData.Month;
            _anoAtual = novaData.Year;
            AtualizarCalendario();
        }
    }

    private void BtnProximo_Clicked(object sender, EventArgs e)
    {
        var novaData = new DateTime(_anoAtual, _mesAtual, 1).AddMonths(1);
        if (novaData <= _dataFinal)
        {
            _mesAtual = novaData.Month;
            _anoAtual = novaData.Year;
            AtualizarCalendario();
        }
    }

    private async void BtnProsseguir_Clicked(object sender, EventArgs e)
    {
        if (_diaSelecionado.HasValue)
        {
            await Navigation.PushAsync(new VisitantesPage(_diaSelecionado.Value));
        }
        else
        {
            await DisplayAlert("Selecione um dia", "Por favor, selecione um sábado ou domingo antes de prosseguir.", "OK");
        }
    }

    private void AtualizarCalendario()
    {
        DiasLayout.Children.Clear();
        FrutasLayout.Children.Clear();

        string nomeMes = new DateTime(_anoAtual, _mesAtual, 1).ToString("MMMM", new CultureInfo("pt-BR"));
        TituloMes.Text = $"{CultureInfo.CurrentCulture.TextInfo.ToTitleCase(nomeMes)} de {_anoAtual}";

        int diasNoMes = DateTime.DaysInMonth(_anoAtual, _mesAtual);
        int primeiroDiaSemana = (int)new DateTime(_anoAtual, _mesAtual, 1).DayOfWeek;
        primeiroDiaSemana = primeiroDiaSemana == 0 ? 7 : primeiroDiaSemana;

        string[] diasSemana = { "Seg", "Ter", "Qua", "Qui", "Sex", "Sáb", "Dom" };

        var linhaDiasSemana = new Grid
        {
            ColumnSpacing = 5,
            Padding = new Thickness(0, 10, 0, 10)
        };

        for (int i = 0; i < 7; i++)
        {
            linhaDiasSemana.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

            var border = new Border
            {
                Stroke = Colors.Green,
                StrokeThickness = 1,
                StrokeShape = new RoundRectangle { CornerRadius = 5 },
                WidthRequest = 45,
                HeightRequest = 45,
                Content = new Label
                {
                    Text = diasSemana[i],
                    FontSize = 10,
                    HorizontalTextAlignment = TextAlignment.Center,
                    VerticalTextAlignment = TextAlignment.Center,
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.Center,
                    TextColor = Colors.Black
                }
            };

            linhaDiasSemana.Add(border, i, 0);
        }

        DiasLayout.Children.Add(linhaDiasSemana);

        var gridDias = new Grid
        {
            ColumnSpacing = 5,
            RowSpacing = 5
        };

        for (int j = 0; j < 7; j++)
            gridDias.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

        int linha = 0;
        int coluna = primeiroDiaSemana - 1;

        for (int dia = 1; dia <= diasNoMes; dia++)
        {
            if (coluna == 7)
            {
                coluna = 0;
                linha++;
            }

            if (gridDias.RowDefinitions.Count <= linha)
                gridDias.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

            var dataDia = new DateTime(_anoAtual, _mesAtual, dia);
            var diaSemana = dataDia.DayOfWeek;
            bool ehFinalDeSemana = diaSemana == DayOfWeek.Saturday || diaSemana == DayOfWeek.Sunday;

            var btnDia = new Button
            {
                Text = dia.ToString(),
                WidthRequest = 45,
                HeightRequest = 45,
                BackgroundColor = ehFinalDeSemana ? Colors.White : Colors.LightGray,
                BorderColor = Colors.Green,
                BorderWidth = 1,
                CornerRadius = 5,
                FontSize = 10,
                TextColor = Colors.Black,
                IsEnabled = ehFinalDeSemana
            };

            if (ehFinalDeSemana)
            {
                btnDia.Clicked += (s, e) =>
                {
                    _diaSelecionado = dataDia;

                    foreach (var child in gridDias.Children)
                    {
                        if (child is Button b && b.IsEnabled)
                            b.BackgroundColor = Colors.White;
                    }

                    btnDia.BackgroundColor = Color.FromArgb("#DFFFD6");
                };
            }

            gridDias.Add(btnDia, coluna, linha);
            coluna++;
        }

        DiasLayout.Children.Add(gridDias);
        AtualizarFrutas();
    }

    private void AtualizarFrutas()
    {
        var frutasDisponiveis = frutasPorMes.ContainsKey(_mesAtual)
            ? frutasPorMes[_mesAtual]
            : new List<string>();

        var frutasIndisponiveis = todasFrutas.Except(frutasDisponiveis).ToList();

        FrutasLayout.Children.Add(new Label
        {
            Text = "Frutas disponíveis:",
            FontAttributes = FontAttributes.Bold,
            TextColor = Colors.DarkGreen,
            FontSize = 16
        });

        foreach (var fruta in frutasDisponiveis)
        {
            FrutasLayout.Children.Add(new Label
            {
                Text = fruta,
                TextColor = Colors.Black,
                FontSize = 14
            });
        }

        FrutasLayout.Children.Add(new Label
        {
            Text = "Frutas indisponíveis:",
            FontAttributes = FontAttributes.Bold,
            TextColor = Colors.Gray,
            FontSize = 16,
            Margin = new Thickness(0, 10, 0, 0)
        });

        foreach (var fruta in frutasIndisponiveis)
        {
            FrutasLayout.Children.Add(new Label
            {
                Text = fruta,
                TextColor = Colors.Gray,
                FontSize = 14
            });
        }
    }
}