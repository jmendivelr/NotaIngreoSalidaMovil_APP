using LoginApp.Maui.ViewModels;
using System.Collections.ObjectModel;
using System.Diagnostics;
using Newtonsoft.Json;
using static LoginApp.Maui.Views.ScanBarCodeInventarioPage;
using static LoginApp.Maui.Views.RegistroInventarioPage;
namespace LoginApp.Maui.Views;

public partial class ScanBarCodeInventarioPage : ContentPage
{
	//public ScanBarCodeInventarioPage()
	//{
	//	InitializeComponent();
	//}
    private ObservableCollection<ProductoInventarioViewModel> resultados = new ObservableCollection<ProductoInventarioViewModel>();
    //private List<> resultadosLista = new List<>();      
    public ScanBarCodeInventarioPage()
    {
        InitializeComponent();
        detectorImagen.Options = new ZXing.Net.Maui.BarcodeReaderOptions
        {
            Formats = ZXing.Net.Maui.BarcodeFormat.Code128,
            AutoRotate = true,
            Multiple = true
        };
    }
    [Serializable]
    public class ProductoViewMayModel2
    {
        public string Codigo { get; set; }
        //public decimal Precio1 { get; set; }
        //public decimal Precio2 { get; set; }
    }

    private void btnRegresar_Clicked(object sender, EventArgs e)
    {
        //App.Current.MainPage = new PrincipalPage();
        Navigation.PopAsync();
        //Navigation.PushAsync(new VentaRapidaPage());
    }
    private void detectorImagen_BarcodesDetected(object sender, ZXing.Net.Maui.BarcodeDetectionEventArgs e)
    {
        detectorImagen.IsDetecting = false;
        if (e.Results.Any())
        {
            var result = e.Results.FirstOrDefault();
            resultados.Add(new ProductoInventarioViewModel { Codigo = result.Value, descripcion = "Producto 3", Cantidad = 0 });
            //ProductoViewModel productoSeleccionado =resultados

            // Convertir la colección a un arreglo
            //ProductoViewModel[] arregloResultados = resultados.ToArray();
            ProductoInventarioViewModel productoSeleccionado = resultados[0];


            // Notificar que la propiedad ha cambiado


            Dispatcher.Dispatch(async () =>
            {
                var resp = await DisplayAlert("Codigo", result.Value, "Aceptar", "Cancelar");

                if (resp)
                {
                    Debug.WriteLine($"Enviando Producto Seleccionado: {productoSeleccionado.descripcion}");
                    MessagingCenter.Send(this, "scanInventario", productoSeleccionado);
                    // El usuario hizo clic en "Aceptar"
                    Navigation.PopAsync();
                    // Puedes cerrar la página actual utilizando PopAsync
                    //await Navigation.PopAsync();
                    // Si estás utilizando una página modal, podrías utilizar PopModalAsync
                    // await Navigation.PopModalAsync();
                }

                //App.Current.MainPage = new PrincipalPage();
            });

        }
    }



}