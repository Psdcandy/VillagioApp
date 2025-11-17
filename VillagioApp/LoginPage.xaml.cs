using Microsoft.Maui.ApplicationModel.Communication;
using Microsoft.Maui.Controls;
using System.Net.Http.Json;
using VillagioApp.Resources.Models;

namespace VillagioApp;

public partial class LoginPage : ContentPage
{
    private readonly string tipoUsuario;

    public LoginPage(string tipo)
    {
        InitializeComponent();
        tipoUsuario = tipo;

        if (tipoUsuario == "Familia")
        {
            NomeEntry.IsVisible = true;
            TelefoneEntry.IsVisible = true;
            EmailEntry.IsVisible = false;
            CnpjEntry.IsVisible = false;
        }
        else if (tipoUsuario == "Agencia")
        {
            NomeEntry.IsVisible = false;
            TelefoneEntry.IsVisible = false;
            EmailEntry.IsVisible = true;
            CnpjEntry.IsVisible = true;
        }
    }

    private async void Login_Clicked(object sender, EventArgs e)
    {
        using var client = new HttpClient { BaseAddress = new Uri("http://villagioapi.runasp.net/") };

        HttpResponseMessage response;

        if (tipoUsuario == "Familia")
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

            var loginDto = new
            {
                TipoUsuarioId = 1,
                Nome = nome,
                Telefone = telefone,
                Senha = senha
            };

            response = await client.PostAsJsonAsync("api/Usuario/login", loginDto);
        }
        else // Agencia
        {
            string email = EmailEntry.Text?.Trim() ?? "";
            string cnpj = CnpjEntry.Text?.Trim() ?? "";
            string senha = SenhaEntry.Text ?? "";

            cnpj = new string(cnpj.Where(char.IsDigit).ToArray());

            if (string.IsNullOrWhiteSpace(email) || !System.Text.RegularExpressions.Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            {
                await DisplayAlert("Erro", "Email inválido. Exemplo: contato@empresa.com", "OK");
                return;
            }

            if (string.IsNullOrWhiteSpace(cnpj) || cnpj.Length != 14)
            {
                await DisplayAlert("Erro", "CNPJ inválido. Informe apenas números (14 dígitos).", "OK");
                return;
            }

            if (senha.Length < 6)
            {
                await DisplayAlert("Erro", "A senha deve ter pelo menos 6 caracteres.", "OK");
                return;
            }

            var loginDto = new
            {
                TipoUsuarioId = 2,
                Email = email,
                CNPJ = cnpj,
                Senha = senha
            };

            response = await client.PostAsJsonAsync("api/Usuario/login", loginDto);
        }

        // ✅ Verifica se o login foi bem-sucedido
        if (response.IsSuccessStatusCode)
        {
            await DisplayAlert("Sucesso", "Login realizado com sucesso!", "OK");

            // ✅ Navega para CalendarioPage
            await Navigation.PushAsync(new CalendarioPage());
        }
        else
        {
            string errorMsg = await response.Content.ReadAsStringAsync();
            await DisplayAlert("Erro", $"Falha no login: {errorMsg}", "OK");
        }
    }
}
