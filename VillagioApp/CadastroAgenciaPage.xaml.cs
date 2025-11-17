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
        tipoAgencia = tipo;

        // Exemplo: usar o tipo para mudar o título
        Title = tipoAgencia == "Agencia" ? "Cadastro Agência" : "Cadastro Família";
    }


    private async void CadastrarAgencia_Clicked(object sender, EventArgs e)
    {
        string nome = NomeEntry.Text?.Trim() ?? "";
        string telefone = TelefoneEntry.Text?.Trim() ?? "";
        string email = EmailEntry.Text?.Trim() ?? "";
        string cnpj = CnpjEntry.Text?.Trim() ?? "";
        string senha = SenhaEntry.Text ?? "";

        // Remove máscara de telefone e CNPJ
        telefone = new string(telefone.Where(char.IsDigit).ToArray());
        cnpj = new string(cnpj.Where(char.IsDigit).ToArray());

        // Validação do Nome
        if (string.IsNullOrWhiteSpace(nome))
        {
            await DisplayAlert("Erro", "O campo Nome não pode estar vazio.", "OK");
            return;
        }

        // Validação do Telefone
        if (telefone.Length < 10 || telefone.Length > 11)
        {
            await DisplayAlert("Erro", "Telefone inválido. Use DDD + número (ex: 11912345678).", "OK");
            return;
        }

        // Validação do Email
        if (string.IsNullOrWhiteSpace(email) || !System.Text.RegularExpressions.Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
        {
            await DisplayAlert("Erro", "Email inválido. Exemplo: contato@empresa.com", "OK");
            return;
        }

        // Validação do CNPJ
        if (string.IsNullOrWhiteSpace(cnpj) || cnpj.Length != 14)
        {
            await DisplayAlert("Erro", "CNPJ inválido. Informe apenas números (14 dígitos).", "OK");
            return;
        }

        // Validação da Senha
        if (senha.Length < 6)
        {
            await DisplayAlert("Erro", "A senha deve ter pelo menos 6 caracteres.", "OK");
            return;
        }

        // Cria objeto para enviar à API
        var agencia = new AgenciaCadastroDto
        {
            Nome = nome,
            Telefone = telefone,
            Email = email,
            CNPJ = cnpj,
            Senha = senha,
            TipoUsuarioId = 2 // Agência
        };

        try
        {
            using var client = new HttpClient();
            client.BaseAddress = new Uri("http://villagioapi.runasp.net/");

            // 1. Verifica se já existe cadastro com Email ou CNPJ
            var usuarios = await client.GetFromJsonAsync<List<UsuarioDto>>("api/Usuario/listar");

            bool existe = usuarios.Any(u =>
                (agencia.Email != null && u.Email == agencia.Email) ||
                (agencia.CNPJ != null && u.CNPJ == agencia.CNPJ)
            );

            if (existe)
            {
                await DisplayAlert("Erro", "Já existe um cadastro com este Email ou CNPJ.", "OK");
                return;
            }

            // 2. Faz o cadastro
            var response = await client.PostAsJsonAsync("api/Usuario/cadastrar", agencia);

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
        await Navigation.PushAsync(new LoginPage("Agencia"));
    }
}