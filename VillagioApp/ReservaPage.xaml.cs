
using System;
using Microsoft.Maui.Controls;
using System.Net.Http.Json;

namespace VillagioApp
{
    public partial class ReservaPage : ContentPage
    {
        private readonly DateTime _dataSelecionada;
        private readonly string _horarioSelecionado;
        private readonly int _tipoUsuarioId; // 1 = Família, 2 = Agência
        private readonly int _visitantes;    // Total de visitantes

        private int adultos = 0;
        private int meiaEntrada = 0;
        private int naoPagantes = 0;
        private bool incluirCafe = false;

        private bool meiaEntradaAvisado = false;
        private bool naoPaganteAvisado = false;
        private bool cafeAvisado = false;

        public ReservaPage(DateTime dataSelecionada, string horarioSelecionado, int visitantes, int tipoUsuarioId)
        {
            InitializeComponent();
            _dataSelecionada = dataSelecionada;
            _horarioSelecionado = horarioSelecionado;
            _visitantes = visitantes;
            _tipoUsuarioId = tipoUsuarioId;

            Title = $"Reserva para {dataSelecionada:dd/MM} às {horarioSelecionado}";
            TotalEntry.Text = _visitantes.ToString();
            AtualizarValorFinal();
        }

        // Adultos
        private void OnAumentarAdultoClicked(object sender, EventArgs e)
        {
            if (adultos + meiaEntrada + naoPagantes < _visitantes)
            {
                adultos++;
                AdultoEntry.Text = adultos.ToString();
                AtualizarValorFinal();
            }
        }

        private void OnDiminuirAdultoClicked(object sender, EventArgs e)
        {
            if (adultos > 0)
            {
                adultos--;
                AdultoEntry.Text = adultos.ToString();
                AtualizarValorFinal();
            }
        }

        // Meia entrada
        private async void OnAumentarMeiaClicked(object sender, EventArgs e)
        {
            if (!meiaEntradaAvisado)
            {
                meiaEntradaAvisado = true;
                await DisplayAlert("Informação", "Meia entrada é válida para crianças de 6 a 12 anos.", "Entendi");
            }

            if (adultos + meiaEntrada + naoPagantes < _visitantes)
            {
                meiaEntrada++;
                MeiaEntry.Text = meiaEntrada.ToString();
                AtualizarValorFinal();
            }
        }

        private void OnDiminuirMeiaClicked(object sender, EventArgs e)
        {
            if (meiaEntrada > 0)
            {
                meiaEntrada--;
                MeiaEntry.Text = meiaEntrada.ToString();
                AtualizarValorFinal();
            }
        }

        // Não pagantes
        private async void OnAumentarNaoPaganteClicked(object sender, EventArgs e)
        {
            if (!naoPaganteAvisado)
            {
                naoPaganteAvisado = true;
                await DisplayAlert("Informação", "Não pagantes são crianças de 0 a 5 anos.", "Entendi");
            }

            if (adultos + meiaEntrada + naoPagantes < _visitantes)
            {
                naoPagantes++;
                NaoPaganteEntry.Text = naoPagantes.ToString();
                AtualizarValorFinal();
            }
        }

        private void OnDiminuirNaoPaganteClicked(object sender, EventArgs e)
        {
            if (naoPagantes > 0)
            {
                naoPagantes--;
                NaoPaganteEntry.Text = naoPagantes.ToString();
                AtualizarValorFinal();
            }
        }

        // Café da manhã
        private async void OnCafeCheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            incluirCafe = e.Value;

            if (incluirCafe && !cafeAvisado)
            {
                cafeAvisado = true;
                await DisplayAlert("Informação sobre o café da manhã",
                    "O café da manhã custa R$65,00 por adulto e R$32,50 por meia entrada. Não pagantes não pagam café.",
                    "Entendi");
            }

            AtualizarValorFinal();
        }

        // Atualiza o valor total
        private void AtualizarValorFinal()
        {
            int valorEntrada = (adultos * 12) + (meiaEntrada * 6);
            double valorCafe = incluirCafe ? (adultos * 65) + (meiaEntrada * 32.50) : 0;
            double valorFinal = valorEntrada + valorCafe;

            ValorFinalLabel.Text = $"Valor Final: R${valorFinal:0.00}";
        }

        // Confirmação da reserva
        private async void OnConfirmarReservaClicked(object sender, EventArgs e)
        {
            if (_visitantes == 0)
            {
                await DisplayAlert("Erro", "Informe o número de visitantes.", "OK");
                return;
            }

            var reserva = new
            {
                Data = _dataSelecionada,
                Horario = _horarioSelecionado,
                VagasReservadas = _tipoUsuarioId == 2 ? 50 : _visitantes,
                TipoUsuarioId = _tipoUsuarioId
            };

            try
            {
                using var client = new HttpClient { BaseAddress = new Uri("http://villagioapi.runasp.net/") };
                var response = await client.PostAsJsonAsync("api/Reservas/reservar", reserva);

                if (response.IsSuccessStatusCode)
                {
                    await DisplayAlert("Sucesso", "Reserva confirmada!", "OK");
                    await Navigation.PopToRootAsync();
                }
                else
                {
                    var erro = await response.Content.ReadAsStringAsync();
                    await DisplayAlert("Erro", $"Falha na reserva: {erro}", "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Erro", $"Erro inesperado: {ex.Message}", "OK");
            }
        }
    }
}
