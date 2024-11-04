using LoginApp.Maui.ViewModels;
using LoginApp.Maui.Views;

namespace LoginApp.Maui
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            this.BindingContext = new AppShellViewModel();
            Routing.RegisterRoute(nameof(HomePage), typeof(HomePage));
            Routing.RegisterRoute(nameof(AboutPage), typeof(AboutPage));
            Routing.RegisterRoute(nameof(ListadoNotaIngresoPage), typeof(ListadoNotaIngresoPage));
            Routing.RegisterRoute(nameof(ListadoNotaSalidaPage), typeof(ListadoNotaSalidaPage));
            Routing.RegisterRoute(nameof(ListaIngreoInventarioPage), typeof(ListaIngreoInventarioPage));
        }
    }
}