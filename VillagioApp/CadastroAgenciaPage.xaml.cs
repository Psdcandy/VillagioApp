using System;
using System.Net.Http.Json;
using Microsoft.Maui.Controls;
using VillagioApp.Resources.Models;

namespace VillagioApp;

public partial class CadastroAgenciaPage : ContentPage
{
    private readonly string tipoAgencia;

    public CadastroAgenciaPage(string tipo)
    {
        InitializeComponent();
        tipoAgencia = tipo; // Recebe o tipo da agência
    }


    private async void CadastrarAgencia_Clicked(object sender, EventArgs e)
    {
        string nome = NomeEntry.Text?.Trim() ?? "";
        string telefone = TelefoneEntry.Text?.Trim() ?? "";
        string email = EmailEntry.Text?.Trim() ?? "";
        string cnpj = CnpjEntry.Text?.Trim() ?? "";
        string senha = SenhaEntry.Text ?? "";

        // Validações (mantém as que você já fez)

        var agencia = new AgenciaCadastroDto
        {
            Nome = nome,
            Telefone = telefone,
            Email = email,
            CNPJ = cnpj,
            Senha = senha,
            TipoUsuarioId = 2 // Agência
        };

        using var client = new HttpClient();
        client.BaseAddress = new Uri("https://localhost:7148/swagger/index.html");
        var response = await client.PostAsJsonAsync("usuarios", agencia);

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
}
