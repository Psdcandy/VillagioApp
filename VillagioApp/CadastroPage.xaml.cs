using Microsoft.Maui.Controls;

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

        // Validações
        if (string.IsNullOrWhiteSpace(nome))
        {
            await DisplayAlert("Erro", "Por favor, preencha o nome.", "OK");
            return;
        }

        if (string.IsNullOrWhiteSpace(telefone) || telefone.Length < 10)
        {
            await DisplayAlert("Erro", "Telefone inválido. Use DDD + número.", "OK");
            return;
        }

        if (string.IsNullOrWhiteSpace(senha) || senha.Length < 6)
        {
            await DisplayAlert("Erro", "A senha deve ter pelo menos 6 caracteres.", "OK");
            return;
        }

        // Aqui você pode enviar os dados para a API
        // Exemplo: await client.PostAsJsonAsync("usuarios", usuario);

        await DisplayAlert("Sucesso", "Cadastro realizado com sucesso!", "OK");

        // Redirecionar para a página de calendário
        await Navigation.PushAsync(new CalendarioPage());
    }
}