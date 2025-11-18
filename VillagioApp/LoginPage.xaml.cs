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

        // Cria o DTO
        var loginDto = new LoginRequest
        {
            TipoUsuarioId = tipoUsuario == "Familia" ? 1 : 2,
            Senha = SenhaEntry.Text?.Trim() ?? ""
        };

        if (tipoUsuario == "Familia")
        {
            string nome = NomeEntry.Text?.Trim() ?? "";
            string telefone = TelefoneEntry.Text?.Trim() ?? "";

            telefone = new string(telefone.Where(char.IsDigit).ToArray());

            // ✅ Validações
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

            if (loginDto.Senha.Length < 6)
            {
                await DisplayAlert("Erro", "A senha deve ter pelo menos 6 caracteres.", "OK");
                return;
            }

            loginDto.Nome = nome;
            loginDto.Telefone = telefone;
        }
        else // Agência
        {
            string email = EmailEntry.Text?.Trim() ?? "";
            string cnpj = CnpjEntry.Text?.Trim() ?? "";

            cnpj = new string(cnpj.Where(char.IsDigit).ToArray());

            // ✅ Validações
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

            if (loginDto.Senha.Length < 6)
            {
                await DisplayAlert("Erro", "A senha deve ter pelo menos 6 caracteres.", "OK");
                return;
            }

            loginDto.Email = email;
            loginDto.CNPJ = cnpj;
        }

        // ✅ Mantém PascalCase no JSON
        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = null
        };

        response = await client.PostAsJsonAsync("api/Usuario/login", loginDto, options);

        // ✅ Verifica resposta
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
