using Microsoft.Maui.Controls;
using Mi_Diario_de_Bienestar.Models;

namespace Mi_Diario_de_Bienestar.Pages
{
    public partial class ListaRegistrosPage : ContentPage
    {
        public ListaRegistrosPage()
        {
            InitializeComponent();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            var registros = await App.Database.GetAllAsync();
            ListaRegistros.ItemsSource = registros;
        }

        private async void Eliminar_Clicked(object sender, EventArgs e)
        {
            var button = sender as Button;
            int id = (int)button.CommandParameter;

            bool confirmar = await DisplayAlert("Eliminar", "¿Deseas eliminar este registro?", "Sí", "No");

            if (!confirmar) return;

            await App.Database.DeleteAsync(id);

            // refrescar lista
            ListaRegistros.ItemsSource = await App.Database.GetAllAsync();
        }
    }
}
