
using System.Net.Http.Json;
using System.Text.Json;

namespace VillagioApp;

public partial class LoginPage : ContentPage
{
    private readonly string tipoUsuario;

    public LoginPage(string tipo)
    {
        InitializeComponent();
        tipoUsuario = tipo;

        // Ajusta visibilidade dos campos
        NomeEntry.IsVisible = true; // Agora sempre visível
        TelefoneEntry.IsVisible = true; // Agora sempre visível
        CnpjEntry.IsVisible = tipoUsuario == "Agencia"; // Apenas para Agência
    }

    private async void Login_Clicked(object sender, EventArgs e)
    {
        using var client = new HttpClient { BaseAddress = new Uri("http://villagioapi.runasp.net/") };

        // Cria DTO com valores padrão
        var loginDto = new LoginRequest
        {
            TipoUsuarioId = tipoUsuario == "Familia" ? 1 : 2,
            Senha = SenhaEntry.Text?.Trim() ?? "",
            Nome = "",
            Telefone = "",
            Email = "", // Mantido no DTO, mas não usado
            CNPJ = ""
        };

        // Validações e preenchimento
        string nome = NomeEntry.Text?.Trim() ?? "";
        string telefone = TelefoneEntry.Text?.Trim() ?? "";
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

        if (string.IsNullOrWhiteSpace(loginDto.Senha) || loginDto.Senha.Length < 6)
        {
            await DisplayAlert("Erro", "A senha deve ter pelo menos 6 caracteres.", "OK");
            return;
        }

        loginDto.Nome = nome;
        loginDto.Telefone = telefone;

        if (tipoUsuario == "Agencia")
        {
            string cnpj = CnpjEntry.Text?.Trim() ?? "";
            cnpj = new string(cnpj.Where(char.IsDigit).ToArray());

            if (string.IsNullOrWhiteSpace(cnpj) || cnpj.Length != 14)
            {
                await DisplayAlert("Erro", "CNPJ inválido. Informe apenas números (14 dígitos).", "OK");
                return;
            }

            loginDto.CNPJ = cnpj;
        }

        // Configura serialização para camelCase (compatível com API)
        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        // Debug: imprime JSON antes de enviar
        string jsonDebug = JsonSerializer.Serialize(loginDto, options);
        Console.WriteLine($"JSON enviado: {jsonDebug}");

        // Envia requisição
        HttpResponseMessage response = await client.PostAsJsonAsync("api/Usuario/login", loginDto, options);

        // Trata resposta
        if (response.IsSuccessStatusCode)
        {
            await DisplayAlert("Sucesso", "Login realizado com sucesso!", "OK");
            await Navigation.PushAsync(new CalendarioPage());
        }
        else
        {
            string errorMsg = await response.Content.ReadAsStringAsync();
            await DisplayAlert("Erro", $"Falha no login: {errorMsg}", "OK");
        }
    }
}
