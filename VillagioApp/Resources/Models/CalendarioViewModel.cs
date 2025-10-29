using System.Globalization;
using System.Collections.ObjectModel;

namespace VillagioApp.Resources.Model;

public class CalendarioViewModel
{
    public ObservableCollection<string> Meses { get; set; }

    public CalendarioViewModel()
    {
        Meses = new ObservableCollection<string>();
        var dataAtual = DateTime.Now;

        for (int i = 0; i < 6; i++)
        {
            var mes = dataAtual.AddMonths(i);
            string nomeMes = mes.ToString("MMMM", new CultureInfo("pt-BR"));
            Meses.Add(CultureInfo.CurrentCulture.TextInfo.ToTitleCase(nomeMes));
        }
    }
}