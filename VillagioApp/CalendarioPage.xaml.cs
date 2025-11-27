using Microsoft.Maui.Controls;
using System.Globalization;
using VillagioApp.Resources.Model;

namespace VillagioApp;

public partial class CalendarioPage : ContentPage
{
    public readonly int _tipoUsuarioId; // 1 = Família, 2 = Agência

    // 🔹 Construtor atualizado para receber o tipo do usuário
    public CalendarioPage(int tipoUsuarioId)
    {
        InitializeComponent();
        _tipoUsuarioId = tipoUsuarioId;
        BindingContext = new CalendarioViewModel(); // mantém seu ViewModel
    }

    private async void OnMesSelecionado(object sender, EventArgs e)
    {
        var botao = sender as Button;
        if (botao?.CommandParameter is string indexStr && int.TryParse(indexStr, out int index))
        {
            var viewModel = BindingContext as CalendarioViewModel;
            if (viewModel != null && index >= 0 && index < viewModel.Meses.Count)
            {
                // Usa a data atual + offset do índice
                var data = DateTime.Now.AddMonths(index);
                string nomeMes = viewModel.Meses[index];
                int mes = data.Month;
                int ano = data.Year;

                // 🔹 Repasse do tipo para MiniCalendarioPage
                await Navigation.PushAsync(new MiniCalendarioPage(nomeMes, mes, ano, _tipoUsuarioId));
            }
        }
    }
}
