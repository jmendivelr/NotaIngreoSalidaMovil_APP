using Newtonsoft.Json;
using static LoginApp.Maui.Views.ListadoNotaIngresoPage;
using System.Collections.ObjectModel;
using LoginApp.Maui.ViewModels;

namespace LoginApp.Maui.Views;

public partial class ListadoNotaIngresoPage : ContentPage
{
    private ObservableCollection<IngresoItem> _ingresoItems;
    public ObservableCollection<AlmacenViewModel> almacenes;
    public ListadoNotaIngresoPage()
	{
		InitializeComponent();
        CargarAlmacenes(); // Llamada para cargar los almacenes
        //CargarNotasIngreso(); // Llamada al método para cargar los datos al inicializar la vista.

    }
    private async void CargarAlmacenes()
    {
        string apiUrl = "http://192.168.1.152:8022/api/Inventario/listaalmacenes";

        using (HttpClient httpClient = new HttpClient())
        {
            try
            {
                // Hacer la petición GET y obtener el resultado como string
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
                // Manejar excepciones de red, deserialización, etc.
                await DisplayAlert("Error", $"No se pudo cargar los almacenes: {ex.Message}", "OK");
            }
        }
    }
    private void AgregarOperacion_Clicked(object sender, EventArgs e)

    {
        // Obtener el almacén seleccionado
        var almacenSeleccionado = almacenPicker.SelectedItem as AlmacenViewModel;

        if (almacenSeleccionado != null)
        {
            // Pasar el `Codigo` del almacén a la nueva página
            Navigation.PushAsync(new VentaRapidaMayPage(almacenSeleccionado.Codigo));
        }
        else
        {
            DisplayAlert("Error", "Debe seleccionar un almacén antes de agregar una nota.", "OK");
        }

    }
    public class IngresoItem
    {
        public string CAALMA { get; set; }

        // Tipo de Documento
        public string CATD { get; set; }

        // Número de Documento
        public string CANUMDOC { get; set; }

        // Código del Proveedor
        public string CACODPRO { get; set; }

        // Glosa (Descripción)
        public string CAGLOSA { get; set; }

        // Nombre del Proveedor
        public string PRVCNOMBRE { get; set; }

        // Cantidad de Items
        public int Cantidad { get; set; }

        // Fecha del Documento
        public DateTime CAFECDOC { get; set; }
    }

    private async void CargarNotasIngreso()
    {
        try
        {
            var almacenSeleccionado = almacenPicker.SelectedItem as AlmacenViewModel;
            var txtAlmacen_ = "";
            if (almacenSeleccionado != null)
            {
                // Pasar el `Codigo` del almacén a la nueva página
                txtAlmacen_ = almacenSeleccionado.Codigo;
            }
            else
            {
                DisplayAlert("Error", "Debe seleccionar un almacén antes de agregar una nota.", "OK");
                return;
            }


            // URL de la API
            string url = $"http://192.168.1.152:8022/api/Inventario/ListadoNotas?CAALMA={txtAlmacen_}&CATD=NI";

            // Crear cliente HTTP
            HttpClient client = new HttpClient();
            var response = await client.GetStringAsync(url);

            // Parsear la respuesta a un objeto
            var result = JsonConvert.DeserializeObject<RootObject>(response);

            if (result.InternalStatus == 1)
            {
                _ingresoItems = new ObservableCollection<IngresoItem>(result.Data);
                listaNotasListView.ItemsSource = _ingresoItems; // Asignar la lista de datos al ListView
            }
            else
            {
                await DisplayAlert("Error", "No se visualiza Notas ingresadas el dia de hoy.", "OK");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", ex.Message, "OK");
        }
    }
    // Evento Clicked para el Botón "Buscar"
    private void BuscarOperacion_Clicked(object sender, EventArgs e)
    {
        CargarNotasIngreso();
    }

    private void EliminarMenuItem_Clicked(object sender, EventArgs e)
    {
        //if (sender is MenuItem menuItem && menuItem.CommandParameter is ProductoViewModel producto)
        //{
        //    // Aquí debes eliminar el producto de tu listaProductos
        //    listaProductos.Remove(producto);
        //    ActualizarTotalCantidades(); // Actualiza el total después de eliminar un producto
        //}
    }
    public class RootObject
    {
        public int InternalStatus { get; set; }
        public List<IngresoItem> Data { get; set; }
    }


}

