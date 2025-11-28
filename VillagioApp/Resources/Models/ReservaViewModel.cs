
using System.ComponentModel;
using System.Net.Http.Json;
using System.Windows.Input;

public class ReservaViewModel : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler PropertyChanged;

    private DateTime _data;
    private string _horario;
    private int _visitantes;
    private int _tipoUsuarioId;

    public DateTime Data
    {
        get => _data;
        set { _data = value; OnPropertyChanged(nameof(Data)); }
    }

    public string Horario
    {
        get => _horario;
        set { _horario = value; OnPropertyChanged(nameof(Horario)); }
    }

    public int Visitantes
    {
        get => _visitantes;
        set { _visitantes = value; OnPropertyChanged(nameof(Visitantes)); }
    }

    public ICommand ConfirmarReservaCommand { get; }

    public ReservaViewModel(DateTime data, string horario, int visitantes, int tipoUsuarioId)
    {
        Data = data;
        Horario = horario;
        Visitantes = visitantes;
        _tipoUsuarioId = tipoUsuarioId;

        ConfirmarReservaCommand = new Command(async () => await ConfirmarReserva());
    }

    private async Task ConfirmarReserva()
    {
        var reserva = new
        {
            UsuarioId = _tipoUsuarioId,
            DataVisita = Data,
            HorarioVisita = TimeSpan.Parse(Horario),
            QuantidadeVisitantes = Visitantes
        };

        using var http = new HttpClient { BaseAddress = new Uri("http://villagioapi.runasp.net/") };
        var response = await http.PostAsJsonAsync("api/Reservas/reservar", reserva);

        if (response.IsSuccessStatusCode)
            await Application.Current.MainPage.DisplayAlert("Sucesso", "Reserva confirmada!", "OK");
        else
            await Application.Current.MainPage.DisplayAlert("Erro", "Falha ao confirmar reserva.", "OK");
    }

    private void OnPropertyChanged(string propertyName) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}
