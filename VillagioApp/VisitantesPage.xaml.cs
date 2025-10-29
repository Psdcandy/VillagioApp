using System;
using Microsoft.Maui.Controls;

namespace VillagioApp
{
    public partial class VisitantesPage : ContentPage
    {
        private readonly DateTime _diaSelecionado;

        public VisitantesPage(DateTime diaSelecionado)
        {
            InitializeComponent();
            _diaSelecionado = diaSelecionado;
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
                await Navigation.PushAsync(new HorariosPage(_diaSelecionado));
            }
            else
            {
                await DisplayAlert("Erro", "Por favor, insira um número válido de visitantes (maior que zero).", "OK");
            }
        }
    }
}