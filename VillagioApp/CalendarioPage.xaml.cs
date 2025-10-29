using Microsoft.Maui.Controls;
using System.Collections.ObjectModel;
using System.Globalization;
using VillagioApp.Resources.Model;

namespace VillagioApp;

public partial class CalendarioPage : ContentPage
{
    public CalendarioPage()
    {
        InitializeComponent();
        BindingContext = new CalendarioViewModel(); // Certifique-se que o ViewModel está ligado
    }

    private async void OnMesSelecionado(object sender, EventArgs e)
    {
        var botao = sender as Button;
        if (botao?.CommandParameter is string indexStr && int.TryParse(indexStr, out int index))
        {
            var viewModel = BindingContext as CalendarioViewModel;
            if (viewModel != null && index >= 0 && index < viewModel.Meses.Count)
            {
                var data = DateTime.Now.AddMonths(index);
                string nomeMes = viewModel.Meses[index];
                int mes = data.Month;
                int ano = data.Year;

                await Navigation.PushAsync(new MiniCalendarioPage(nomeMes, mes, ano));
            }
        }
    }
}



