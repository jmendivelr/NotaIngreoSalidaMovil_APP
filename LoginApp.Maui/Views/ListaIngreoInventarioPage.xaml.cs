using LoginApp.Maui.ViewModels;
using Newtonsoft.Json;
using System.Collections.ObjectModel;

namespace LoginApp.Maui.Views;

public partial class ListaIngreoInventarioPage : ContentPage
{
    private ObservableCollection<IngresoItem> _ingresoItems;

    public ObservableCollection<AlmacenViewModel> almacenes;
    public ListaIngreoInventarioPage()
	{
		InitializeComponent();
        CargarAlmacenes(); // Llamada para cargar los almacenes
    }
    private void EliminarMenuItem_Clicked(object sender, EventArgs e)
    {
        //if (sender is MenuItem menuItem && menuItem.CommandParameter is ProductoViewModel producto)
        //{
        //    // Aqu� debes eliminar el producto de tu listaProductos
        //    listaProductos.Remove(producto);
        //    ActualizarTotalCantidades(); // Actualiza el total despu�s de eliminar un producto
        //}
    }
    private async void CargarAlmacenes()
    {
        string apiUrl = "http://192.168.1.3:8022/api/Inventario/listaalmacenes";

        using (HttpClient httpClient = new HttpClient())
        {
            try
            {
                // Hacer la petici�n GET y obtener el resultado como string
                string jsonResult = await httpClient.GetStringAsync(apiUrl);

                // Deserializar la respuesta completa en ApiResponse
                var apiResponse = JsonConvert.DeserializeObject<ApiResponseViewModel>(jsonResult);

                // Verificar que el estado interno sea exitoso (InternalStatus = 1)
                if (apiResponse.InternalStatus == 1 && apiResponse.Data != null)
                {
                    // Deserializar la lista de almacenes
                    almacenes = new ObservableCollection<AlmacenViewModel>(apiResponse.Data);

                    // Asignar la lista al Picker
                    almacenPicker.ItemsSource = almacenes;
                }
                else
                {
                    // Manejar el caso donde no hay almacenes o InternalStatus no es exitoso
                    await DisplayAlert("Error", "No se pudieron cargar los almacenes.", "OK");
                }
            }
            catch (Exception ex)
            {
                // Manejar excepciones de red, deserializaci�n, etc.
                await DisplayAlert("Error", $"No se pudo cargar los almacenes: Error al conectar con Servidor, verificar conexi�n.", "OK");
            }
        }
    }
    private void BuscarOperacion_Clicked(object sender, EventArgs e)
    {
        CargarNotasIngreso();
    }
    private void AgregarOperacion_Clicked(object sender, EventArgs e)

    {
        // Obtener el almac�n seleccionado
        var almacenSeleccionado = almacenPicker.SelectedItem as AlmacenViewModel;

        if (almacenSeleccionado != null)
        {
            // Pasar el `Codigo` del almac�n a la nueva p�gina
            Navigation.PushAsync(new RegistroInventarioPage(almacenSeleccionado.Codigo));
        }
        else
        {
            DisplayAlert("Error", "Debe seleccionar un almac�n antes de agregar una nota.", "OK");
        }

    }

    private async void CargarNotasIngreso()
    {
        //try
        //{
        //    var almacenSeleccionado = almacenPicker.SelectedItem as AlmacenViewModel;
        //    var txtAlmacen_ = "";
        //    if (almacenSeleccionado != null)
        //    {
        //        // Pasar el `Codigo` del almac�n a la nueva p�gina
        //        txtAlmacen_ = almacenSeleccionado.Codigo;
        //    }
        //    else
        //    {
        //        DisplayAlert("Error", "Debe seleccionar un almac�n antes de agregar una nota.", "OK");
        //        return;
        //    }


        //    // URL de la API
        //    string url = $"http://192.168.1.3:8022/api/Inventario/ListadoNotas?CAALMA={txtAlmacen_}&CATD=SA";

        //    // Crear cliente HTTP
        //    HttpClient client = new HttpClient();
        //    var response = await client.GetStringAsync(url);

        //    // Parsear la respuesta a un objeto
        //    var result = JsonConvert.DeserializeObject<RootObject>(response);

        //    if (result.InternalStatus == 1)
        //    {
        //        _ingresoItems = new ObservableCollection<IngresoItem>(result.Data);
        //        listaNotasListView.ItemsSource = _ingresoItems; // Asignar la lista de datos al ListView
        //    }
        //    else
        //    {
        //        await DisplayAlert("Error", "No se visualiza Notas ingresadas el dia de hoy.", "OK");
        //    }
        //}
        //catch (Exception ex)
        //{
        //    await DisplayAlert("Error", ex.Message, "OK");
        //}
    }

    public class RootObject
    {
        public int InternalStatus { get; set; }
        public List<IngresoItem> Data { get; set; }
    }
    public class IngresoItem
    {
        public string CAALMA { get; set; }

        // Tipo de Documento
        public string CATD { get; set; }

        // N�mero de Documento
        public string CANUMDOC { get; set; }

        // C�digo del Proveedor
        public string CACODPRO { get; set; }

        // Glosa (Descripci�n)
        public string CAGLOSA { get; set; }

        // Nombre del Proveedor
        public string PRVCNOMBRE { get; set; }

        // Cantidad de Items
        public int Cantidad { get; set; }

        // Fecha del Documento
        public DateTime CAFECDOC { get; set; }
    }

}