using Microsoft.Maui.Controls;
using Mi_Diario_de_Bienestar.Data;

namespace Mi_Diario_de_Bienestar
{
    public partial class App : Application
    {
        public static DatabaseService Database { get; private set; }

        public App()
        {
            InitializeComponent();

            // Crear instancia del servicio de base de datos
            Database = new DatabaseService();

            // Iniciar BD de forma asíncrona
            _ = InicializarBaseDatos();

            // Definir página principal (Flyout)
            MainPage = new AppShell();
        }

        private async Task InicializarBaseDatos()
        {
            await Database.InitializeAsync();
        }
    }
}
