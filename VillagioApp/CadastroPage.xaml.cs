using Microsoft.Maui.Controls;
using System.Net.Http.Json;
using VillagioApp.Resources.Models;

namespace VillagioApp;

public partial class CadastroPage : ContentPage
{
    public CadastroPage()
    {
        InitializeComponent();
    }

    private async void Cadastrar_Clicked(object sender, EventArgs e)
    {
        string nome = NomeEntry.Text?.Trim() ?? "";
        string telefone = TelefoneEntry.Text?.Trim() ?? "";
        string senha = SenhaEntry.Text ?? "";

        telefone = new string(telefone.Where(char.IsDigit).ToArray());

        if (string.IsNullOrWhiteSpace(nome))
        {
            await DisplayAlert("Erro", "Por favor, preencha o nome.", "OK");
            return;
        }

        if (telefone.Length < 10 || telefone.Length > 11)
        {
            await DisplayAlert("Erro", "Telefone inválido. Use DDD + número.", "OK");
            return;
        }

        if (senha.Length < 6)
        {
            await DisplayAlert("Erro", "A senha deve ter pelo menos 6 caracteres.", "OK");
            return;
        }

        var familia = new FamiliaCadastroDto
        {
            Nome = nome,
            Telefone = telefone,
            Senha = senha,
            TipoUsuarioId = 1
        };

        try
        {
            using var client = new HttpClient { BaseAddress = new Uri("http://villagioapi.runasp.net/") };

            var response = await client.PostAsJsonAsync("api/Usuario/cadastrar", familia);

            if (response.IsSuccessStatusCode)
            {
                await DisplayAlert("Sucesso", "Cadastro realizado com sucesso!", "OK");
                await Navigation.PushAsync(new CalendarioPage());
            }
            else
            {
                var erro = await response.Content.ReadAsStringAsync();
                await DisplayAlert("Erro", $"Falha no cadastro: {erro}", "OK");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erro", $"Erro inesperado: {ex.Message}", "OK");
        }
    }
    private async void IrParaLogin_Tapped(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new LoginPage("Familia"));
    }
}