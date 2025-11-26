
using System.Net.Http.Json;
using System.Text.Json;
using System.Globalization;
using System.Text;

namespace VillagioApp;

public partial class LoginPage : ContentPage
{
    private readonly string tipoUsuario;

    public LoginPage(string tipo)
    {
        InitializeComponent();
        tipoUsuario = tipo;

        NomeEntry.IsVisible = true;
        TelefoneEntry.IsVisible = true;
        CnpjEntry.IsVisible = tipoUsuario == "Agencia";
    }

    // Funções utilitárias para normalização
    private static string NormalizeText(string? input)
    {
        if (string.IsNullOrWhiteSpace(input)) return string.Empty;

        var normalized = input.Trim().Normalize(NormalizationForm.FormD);
        var sb = new StringBuilder(normalized.Length);

        foreach (var ch in normalized)
        {
            var uc = CharUnicodeInfo.GetUnicodeCategory(ch);
            if (uc != UnicodeCategory.NonSpacingMark &&
                uc != UnicodeCategory.SpacingCombiningMark &&
                uc != UnicodeCategory.EnclosingMark)
            {
                sb.Append(char.ToLowerInvariant(ch));
            }
        }

        return sb.ToString().Normalize(NormalizationForm.FormC);
    }

    private static string DigitsOnly(string? s) =>
        new string((s ?? string.Empty).Where(char.IsDigit).ToArray());

    private async void Login_Clicked(object sender, EventArgs e)
    {
        using var client = new HttpClient { BaseAddress = new Uri("http://villagioapi.runasp.net/") };

        var loginDto = new LoginRequest
        {
            TipoUsuarioId = tipoUsuario == "Familia" ? 1 : 2,
            Senha = SenhaEntry.Text?.Trim() ?? "",
            Nome = "",
            Telefone = "",
            Email = "",
            CNPJ = ""
        };

        // Normaliza e valida campos
        string nome = NormalizeText(NomeEntry.Text);
        string telefone = DigitsOnly(TelefoneEntry.Text);

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
            string cnpj = DigitsOnly(CnpjEntry.Text);

            if (string.IsNullOrWhiteSpace(cnpj) || cnpj.Length != 14)
            {
                await DisplayAlert("Erro", "CNPJ inválido. Informe apenas números (14 dígitos).", "OK");
                return;
            }

            loginDto.CNPJ = cnpj;
        }

        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        // Debug: imprime JSON normalizado
        string jsonDebug = JsonSerializer.Serialize(loginDto, options);
        Console.WriteLine($"JSON enviado: {jsonDebug}");

        HttpResponseMessage response = await client.PostAsJsonAsync("api/Usuario/login", loginDto, options);

        if (response.IsSuccessStatusCode)
        {
            await DisplayAlert("Sucesso", "Login realizado com sucesso!", "OK");
            await Navigation.PushAsync(new CalendarioPage());
        }
        else
        {
            string errorMsg = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"Erro: {response.StatusCode} - {errorMsg}");
            await DisplayAlert("Erro", "Falha no login. Verifique seus dados e tente novamente.", "OK");
        }
    }
}
