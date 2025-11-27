
using System;
using Microsoft.Maui.Controls;

namespace VillagioApp
{
    public partial class VisitantesPage : ContentPage
    {
        private readonly DateTime _diaSelecionado;
        private readonly int _tipoUsuarioId; // 1 = Família, 2 = Agência

        // 🔹 Novo construtor com tipo do usuário
        public VisitantesPage(DateTime diaSelecionado, int tipoUsuarioId)
        {
            InitializeComponent();
            _diaSelecionado = diaSelecionado;
            _tipoUsuarioId = tipoUsuarioId;
        }

        private void OnAumentarClicked(object sender, EventArgs e)
        {
            if (int.TryParse(VisitantesEntry.Text, out int valor))
            {
                VisitantesEntry.Text = (valor + 1).ToString();
            }
            else
            {
                VisitantesEntry.Text = "1";
            }
        }

        private void OnDiminuirClicked(object sender, EventArgs e)
        {
            if (int.TryParse(VisitantesEntry.Text, out int valor) && valor > 1)
            {
                VisitantesEntry.Text = (valor - 1).ToString();
            }
            else
            {
                VisitantesEntry.Text = "1";
            }
        }



        private async void OnContinuarClicked(object sender, EventArgs e)
        {
            if (int.TryParse(VisitantesEntry.Text, out int visitantes) && visitantes > 0)
            {
                // Se for válido, navega para HorariosPage
                await Navigation.PushAsync(new HorariosPage(_diaSelecionado, _tipoUsuarioId, visitantes));
            }
            else
            {
                // Se for inválido, mostra alerta
                await DisplayAlert("Erro", "Por favor, insira um número válido de visitantes (maior que zero).", "OK");
            }
        }


    }
}
