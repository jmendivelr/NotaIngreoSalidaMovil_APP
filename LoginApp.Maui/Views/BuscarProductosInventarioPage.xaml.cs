using LoginApp.Maui.ViewModels;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace LoginApp.Maui.Views;

public partial class BuscarProductosInventarioPage : ContentPage
{
	//public BuscarProductosInventarioPage()
	//{
	//	InitializeComponent();
	//}
    private ObservableCollection<ProductoInventarioViewModel> resultados = new ObservableCollection<ProductoInventarioViewModel>();
    //private ObservableCollection<AlmacenViewModel> almacenes;
    private string _codigoAlmacen;
    //private ObservableCollection<ProductoMayViewModel> productos;
    public BuscarProductosInventarioPage(string codigoAlmacen)
    {
        InitializeComponent();
        _codigoAlmacen = codigoAlmacen;
        //CargarAlmacenes(); // Llamada para cargar los almacenes
    }
    //private void Buscar_Clicked(object sender, EventArgs e)
    //{
    //    // Simula una búsqueda de productos (puedes realizar una búsqueda real en tu aplicación)
    //    resultados.Clear();
    //    resultados.Add(new ProductoViewModel { Nombre = "Producto 1", Precio1 = 10, Precio2 = 15 });
    //    resultados.Add(new ProductoViewModel { Nombre = "Producto 2", Precio1 = 20, Precio2 = 25 });
    //    resultados.Add(new ProductoViewModel { Nombre = "Producto 3", Precio1 = 30, Precio2 = 35 });

    //    resultadosLista.ItemsSource = resultados;
    //}
    private async void Buscar_Clicked(object sender, EventArgs e)
    {
        // Obtener la búsqueda ingresada por el usuario
        string busqueda = busquedaEntry.Text;

        // Obtener el almacén seleccionado en el Picker
        //if (almacenPicker.SelectedItem == null)
        //{
        //    await DisplayAlert("Error", "Seleccione un almacén", "OK");
        //    return;
        //}

        //// Obtener el almacén seleccionado del Picker
        //var almacenSeleccionado = (AlmacenViewModel)almacenPicker.SelectedItem;
        //string codigoAlmacen = almacenSeleccionado.Codigo; // Usar el código del almacén seleccionado

        // Construir la URL de la API con la descripción y el código del almacén
        string apiUrl = $"http://192.168.1.3:8022/api/Inventario/listaArticulo?descripcion={busqueda}&alm={_codigoAlmacen}";

        using (HttpClient httpClient = new HttpClient())
        {
            try
            {
                // Hacer la petición GET y obtener el resultado como string
                string jsonResult = await httpClient.GetStringAsync(apiUrl);

                // Deserializar la respuesta completa en ApiResponseProductoMayViewModel
                var apiResponse = JsonConvert.DeserializeObject<ApiResponseProductoInventarioViewModel>(jsonResult);

                // Verificar que el estado interno sea exitoso (InternalStatus = 1)
                if (apiResponse.InternalStatus == 1 && apiResponse.Data != null)
                {
                    foreach (var producto in apiResponse.Data)
                    {
                        producto.Almacen = _codigoAlmacen; // Modificar el valor del almacén
                    }
                    // Deserializar la lista de productos

                    var resultados = new ObservableCollection<ProductoInventarioViewModel>(apiResponse.Data);

                    // Aquí puedes mostrar los resultados en un ListView o cualquier otro control
                    resultadosLista.ItemsSource = resultados;
                }
                else
                {
                    // Manejar el caso donde no hay artículos o InternalStatus no es exitoso
                    await DisplayAlert("Error", "No se pudieron cargar los artículos.", "OK");
                }
            }
            catch (Exception ex)
            {
                // Manejar excepciones de red, deserialización, etc.
                await DisplayAlert("Error", $"No se pudo cargar los artículos: Error al conectar con Servidor, verificar conexión.", "OK");
            }
        }


    }


    private void ResultadoSeleccionado(object sender, SelectedItemChangedEventArgs e)
    {
        // Puedes realizar acciones al seleccionar un resultado si es necesario
    }
    private void SeleccionarProducto_Clicked(object sender, EventArgs e)
    {
        ProductoInventarioViewModel productoSeleccionado = resultadosLista.SelectedItem as ProductoInventarioViewModel;

        if (productoSeleccionado != null)
        {
            Debug.WriteLine($"Enviando Producto Seleccionado: {productoSeleccionado.descripcion}");

            MessagingCenter.Send(this, "ProductoSeleccionadoInventario", productoSeleccionado);

            Navigation.PopAsync();
        }
    }
    public class Producto
    {
        public string Codigo { get; set; }
        public string descripcion { get; set; }
        public string Cantidad { get; set; }
    }


}