using Microsoft.Maui.Controls;

namespace VillagioApp;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();
    }

    // Botão "Agência"
    private async void OnCounterClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new CadastroAgenciaPage("Agencia"));
    }

    // Botão "Família"
    private async void Aula1_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new CadastroPage());
    }
}