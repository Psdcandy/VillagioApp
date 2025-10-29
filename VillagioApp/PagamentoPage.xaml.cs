using System;
using Microsoft.Maui.Controls;

namespace VillagioApp
{
    public partial class PagamentoPage : ContentPage
    {
        public PagamentoPage(DateTime data, string horario, int total, int adultos, int meia, int naoPagantes, bool cafe, double valorFinal)
        {
            InitializeComponent();

            DataLabel.Text = $"Data da visita: {data:dd/MM/yyyy}";
            HorarioLabel.Text = $"Horário: {horario}";
            TotalLabel.Text = $"Total de visitantes: {total}";
            AdultosLabel.Text = $"Adultos: {adultos}";
            MeiaLabel.Text = $"Meia entrada: {meia}";
            NaoPagantesLabel.Text = $"Não pagantes: {naoPagantes}";
            CafeLabel.Text = $"Café da manhã incluído: {(cafe ? "Sim" : "Não")}";
            ValorFinalLabel.Text = $"Valor total a pagar: R${valorFinal:0.00}";
        }

        private void OnRealizarPagamentoClicked(object sender, EventArgs e)
        {
            PixBorder.IsVisible = true; // Mostra o Border inteiro
            PixLabel.Text = "chavepix@exemplo.com"; // Substitua pela chave real
        }

    }
}