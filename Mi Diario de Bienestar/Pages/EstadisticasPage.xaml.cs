using Microsoft.Maui.Controls;

namespace Mi_Diario_de_Bienestar.Pages
{
    public partial class EstadisticasPage : ContentPage
    {
        public EstadisticasPage()
        {
            InitializeComponent();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            var stats = await App.Database.GetStatisticsLast7DaysAsync();

            if (stats == null)
            {
                LblActividad.Text = "Sin datos";
                LblEnergia.Text = "Sin datos";
                BarraProgreso.Progress = 0;
                return;
            }

            LblActividad.Text = stats.Value.avgActividad.ToString("0.0");
            LblEnergia.Text = stats.Value.avgEnergia.ToString("0.0");

            // progreso general simple
            double progreso = ((stats.Value.avgActividad / 10) + (stats.Value.avgEnergia / 5)) / 2;

            BarraProgreso.Progress = progreso;
        }
    }
}
