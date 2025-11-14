using System.Net.Http.Json;
using Microsoft.Maui.Controls;

namespace VillagioApp;

public partial class LoginPage : ContentPage
{
    public LoginPage()
    {
        InitializeComponent();
    }

    private async void Login_Clicked(object sender, EventArgs e)
    {
        string email = EmailEntry.Text?.Trim() ?? "";
        string senha = SenhaEntry.Text ?? "";

        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(senha))
        {
            await DisplayAlert("Erro", "Preencha email e senha.", "OK");
            return;
        }

        var loginDto = new { Email = email, Senha = senha };

        using var client = new HttpClient();
        client.BaseAddress = new Uri("http://villagioapi.runasp.net/");
        var response = await client.PostAsJsonAsync("login", loginDto);

        if (response.IsSuccessStatusCode)
        {
            var token = await response.Content.ReadAsStringAsync();
            await SecureStorage.SetAsync("auth_token", token);
            await DisplayAlert("Sucesso", "Login realizado!", "OK");
            await Navigation.PushAsync(new CalendarioPage());
        }
        else
        {
            await DisplayAlert("Erro", "Email ou senha inválidos.", "OK");
        }
    }
}
