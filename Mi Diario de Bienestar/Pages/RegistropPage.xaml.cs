using Microsoft.Maui.Controls;
using Mi_Diario_de_Bienestar.Models;

namespace Mi_Diario_de_Bienestar.Pages
{
    public partial class RegistroPage : ContentPage
    {
        // Valor de energía inicial
        private int energia = 1;

        public RegistroPage()
        {
            InitializeComponent();

            FechaPicker.Date = DateTime.Today;
            BarraActividad.Progress = 0;

            // Inicializar labels
            LabelSliderValor.Text = ((int)SliderActividad.Value).ToString();
            LabelStepperValor.Text = energia.ToString();
        }

        // Evento del Slider
        private void SliderActividad_ValueChanged(object sender, ValueChangedEventArgs e)
        {
            int valorEntero = (int)e.NewValue;
            BarraActividad.Progress = valorEntero / 10.0;
            LabelSliderValor.Text = valorEntero.ToString();
        }

        // Botón + personalizado
        private void BotonMas_Clicked(object sender, EventArgs e)
        {
            if (energia < 5)
                energia++;

            LabelStepperValor.Text = energia.ToString();
        }

        // Botón - personalizado
        private void BotonMenos_Clicked(object sender, EventArgs e)
        {
            if (energia > 1)
                energia--;

            LabelStepperValor.Text = energia.ToString();
        }

        // Guardar registro
        private async void ButtonGuardar_Clicked(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(EditorDescripcion.Text))
            {
                await DisplayAlert("Error", "Debes escribir una descripción.", "OK");
                return;
            }

            var registro = new RegistroDiario
            {
                Fecha = FechaPicker.Date,
                Descripcion = EditorDescripcion.Text,
                ActividadFisica = (int)SliderActividad.Value,
                Energia = energia
            };

            await App.Database.InsertAsync(registro);

            await DisplayAlert("Guardado", "El registro ha sido guardado con éxito.", "OK");

            // Limpiar formulario
            EditorDescripcion.Text = "";
            SliderActividad.Value = 0;
            BarraActividad.Progress = 0;
            energia = 1;
            LabelStepperValor.Text = energia.ToString();
            FechaPicker.Date = DateTime.Today;
        }
    }
}
