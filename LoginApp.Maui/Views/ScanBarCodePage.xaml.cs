
using LoginApp.Maui.ViewModels;
using System.Collections.ObjectModel;
using System.Diagnostics;
using Newtonsoft.Json;
using static LoginApp.Maui.Views.ScanBarCodePage;
using static LoginApp.Maui.Views.VentaRapidaPage;

namespace LoginApp.Maui.Views;

public partial class ScanBarCodePage : ContentPage
{
    private ObservableCollection<ProductoViewModel> resultados = new ObservableCollection<ProductoViewModel>();
    //private List<> resultadosLista = new List<>();      
    public ScanBarCodePage()
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
    public class ProductoViewModel2
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
            resultados.Add(new ProductoViewModel { Codigo = result.Value, descripcion = "Producto 3",Cantidad = 0 });
            //ProductoViewModel productoSeleccionado =resultados

            // Convertir la colecci�n a un arreglo
            //ProductoViewModel[] arregloResultados = resultados.ToArray();
            ProductoViewModel productoSeleccionado = resultados[0];

            // Notificar que la propiedad ha cambiado

              
            Dispatcher.Dispatch(async () =>
            {
                var resp = await DisplayAlert("Codigo", result.Value, "Aceptar", "Cancelar");

                if (resp)
                {
                    Debug.WriteLine($"Enviando Producto Seleccionado: {productoSeleccionado.descripcion}");
                    MessagingCenter.Send(this, "scan", productoSeleccionado);
                    // El usuario hizo clic en "Aceptar"
                    Navigation.PopAsync();
                    // Puedes cerrar la p�gina actual utilizando PopAsync
                    //await Navigation.PopAsync();
                    // Si est�s utilizando una p�gina modal, podr�as utilizar PopModalAsync
                    // await Navigation.PopModalAsync();
                }
               
                //App.Current.MainPage = new PrincipalPage();
            });

        }
    }



}