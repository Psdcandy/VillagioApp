using System;
using Microsoft.Maui.Controls;

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

        if (string.IsNullOrWhiteSpace(nome))
        {
            await DisplayAlert("Erro", "Por favor, preencha o nome.", "OK");
            return;
        }

        if (string.IsNullOrWhiteSpace(telefone) || telefone.Length < 10)
        {
            await DisplayAlert("Erro", "Telefone inválido.", "OK");
            return;
        }

        if (string.IsNullOrWhiteSpace(email) || !email.Contains("@"))
        {
            await DisplayAlert("Erro", "Email inválido.", "OK");
            return;
        }

        if (string.IsNullOrWhiteSpace(cnpj) || cnpj.Length != 14)
        {
            await DisplayAlert("Erro", "CNPJ inválido. Deve conter 14 dígitos.", "OK");
            return;
        }

        if (string.IsNullOrWhiteSpace(senha) || senha.Length < 6)
        {
            await DisplayAlert("Erro", "A senha deve ter pelo menos 6 caracteres.", "OK");
            return;
        }

        // Aqui você pode usar tipoAgencia para salvar no banco
        // Exemplo: await ApiService.CadastrarAgencia(nome, telefone, email, cnpj, senha, tipoAgencia);

        await DisplayAlert("Sucesso", "Cadastro da agência realizado com sucesso!", "OK");

        await Navigation.PushAsync(new CalendarioPage());
    }
}
