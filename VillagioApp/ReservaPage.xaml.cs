using System;
using Microsoft.Maui.Controls;

namespace VillagioApp
{
    public partial class ReservaPage : ContentPage
    {
        private readonly DateTime _dataSelecionada;
        private readonly string _horarioSelecionado;

        private int totalVisitantes = 0;
        private int adultos = 0;
        private int meiaEntrada = 0;
        private int naoPagantes = 0;
        private bool incluirCafe = false;

        // Flags para exibir alertas apenas uma vez
        private bool meiaEntradaAvisado = false;
        private bool naoPaganteAvisado = false;

        private bool cafeAvisado = false;

        public ReservaPage(DateTime dataSelecionada, string horarioSelecionado)
        {
            InitializeComponent();
            _dataSelecionada = dataSelecionada;
            _horarioSelecionado = horarioSelecionado;

            Title = $"Reserva para {dataSelecionada:dd/MM} às {horarioSelecionado}";
            AtualizarValorFinal();
        }

        // Total de visitantes
        private void OnAumentarTotalClicked(object sender, EventArgs e)
        {
            totalVisitantes++;
            TotalEntry.Text = totalVisitantes.ToString();
            ValidarDistribuicao();
            AtualizarValorFinal();
        }

        private void OnDiminuirTotalClicked(object sender, EventArgs e)
        {
            if (totalVisitantes > 0)
            {
                totalVisitantes--;
                TotalEntry.Text = totalVisitantes.ToString();
                ValidarDistribuicao();
                AtualizarValorFinal();
            }
        }

        // Adultos
        private void OnAumentarAdultoClicked(object sender, EventArgs e)
        {
            if (adultos + meiaEntrada + naoPagantes < totalVisitantes)
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

            if (adultos + meiaEntrada + naoPagantes < totalVisitantes)
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

            if (adultos + meiaEntrada + naoPagantes < totalVisitantes)
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

        // Garante que a soma dos tipos não ultrapasse o total
        private void ValidarDistribuicao()
        {
            int soma = adultos + meiaEntrada + naoPagantes;
            if (soma > totalVisitantes)
            {
                int excesso = soma - totalVisitantes;

                if (naoPagantes >= excesso)
                    naoPagantes -= excesso;
                else if (meiaEntrada >= excesso - naoPagantes)
                    meiaEntrada -= (excesso - naoPagantes);
                else
                    adultos -= (excesso - naoPagantes - meiaEntrada);

                AdultoEntry.Text = adultos.ToString();
                MeiaEntry.Text = meiaEntrada.ToString();
                NaoPaganteEntry.Text = naoPagantes.ToString();
            }
        }

        // Confirmação da reserva
        private async void OnConfirmarReservaClicked(object sender, EventArgs e)
        {
            if (totalVisitantes == 0)
            {
                await DisplayAlert("Erro", "Informe o número de visitantes.", "OK");
                return;
            }

            int valorEntrada = (adultos * 12) + (meiaEntrada * 6);
            double valorCafe = incluirCafe ? (adultos * 65) + (meiaEntrada * 32.50) : 0;
            double valorFinal = valorEntrada + valorCafe;

            // Navega para a PagamentoPage com os dados
            await Navigation.PushAsync(new PagamentoPage(
                _dataSelecionada,
                _horarioSelecionado,
                totalVisitantes,
                adultos,
                meiaEntrada,
                naoPagantes,
                incluirCafe,
                valorFinal
            ));
        }
    }
}