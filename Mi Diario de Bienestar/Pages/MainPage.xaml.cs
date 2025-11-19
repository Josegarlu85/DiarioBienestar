using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;

namespace Mi_Diario_de_Bienestar.Pages
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
            CargarNombre();
        }

        private void CargarNombre()
        {
            string nombre = Preferences.Get("NombreUsuario", string.Empty);

            if (string.IsNullOrWhiteSpace(nombre))
                LblSaludo.Text = "¡Hola! 😊";
            else
                LblSaludo.Text = $"¡Hola, {nombre}! 🌿";
        }

        private async void Button_Clicked(object sender, EventArgs e)
        {
            string result = await DisplayPromptAsync(
                "Nombre",
                "Introduce tu nombre:");

            if (!string.IsNullOrWhiteSpace(result))
            {
                Preferences.Set("NombreUsuario", result);
                CargarNombre();
            }
        }
    }
}
