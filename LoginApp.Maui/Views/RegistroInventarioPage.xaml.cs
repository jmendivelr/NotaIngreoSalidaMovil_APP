using LoginApp.Maui.ViewModels;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;

namespace LoginApp.Maui.Views;

public partial class RegistroInventarioPage : ContentPage
{
	

    public ObservableCollection<ProductoInventarioViewModel> listaProductos = new ObservableCollection<ProductoInventarioViewModel>();

    public class Producto
    {
        public string Codigo { get; set; }
        public string Nombre { get; set; }
        public string Cantidad { get; set; }
    }
    // Lista completa de documentos
    public List<Documento_Salida> _documentos_Salida;
    // Lista filtrada
    public ObservableCollection<Documento_Salida> DocumentosFiltrados_Salida { get; set; }
    // Lista completa de proveedores
    //public List<Proveedor_Salida> _proveedores;

    // Lista filtrada para mostrar en la interfaz
    //public ObservableCollection<Proveedor_Salida> ProveedoresFiltrados_Salida { get; set; }
    //public ObservableCollection<Transaccion_Salida> _transacciones_Salida;
    //public ObservableCollection<Transaccion_Salida> TransaccionesFiltradas_Salida { get; set; } = new ObservableCollection<Transaccion_Salida>();

    public string _codigoAlmacen;

    private string CodTransacciones;
    private string CodProveedor;
    private string CodTipoDocumento;

    public class Totalgen
    {
        public string total { get; set; }
    }
    public RegistroInventarioPage(string codigoAlmacen)
    {
        InitializeComponent();
        _codigoAlmacen = codigoAlmacen;
        //ProveedoresFiltrados_Salida = new ObservableCollection<Proveedor_Salida>();
        //DocumentosFiltrados_Salida = new ObservableCollection<Documento_Salida>();
        //TransaccionesFiltradas_Salida = new ObservableCollection<Transaccion_Salida>();
        BindingContext = this;
        // Llamar al método para cargar documentos desde la API
        //CargarDocumentosDesdeAPI();
        //CargarTransaccionesDesdeAPI();
        MessagingCenter.Subscribe<ScanBarCodeInventarioPage, ProductoInventarioViewModel>(this, "scanInventario", (sender, producto) =>
        {
            Debug.WriteLine($"Recibido Producto Seleccionado en VentaRapidaPage: {producto.Codigo}");

            CargarPreciosDesdeAPIScan(producto.Codigo);
        });
        MessagingCenter.Subscribe<BuscarProductosInventarioPage, ProductoInventarioViewModel>(this, "ProductoSeleccionadoInventario", (sender, producto) =>
        {
            Debug.WriteLine($"Recibido Producto Seleccionado en VentaRapidaPage: {producto.Codigo}");

            CargarPreciosDesdeAPI(producto.Codigo, producto.descripcion, producto.Almacen, producto.Cantidad);
        });
        listaProductosListView.ItemsSource = listaProductos;

        //listaProductos.CollectionChanged += (sender, e) =>
        //{
        //    if (e.NewItems != null)
        //    {
        //        foreach (ProductoMayViewModel nuevoProducto in e.NewItems)
        //        {
        //            nuevoProducto.PropertyChanged += ProductoViewModel_PropertyChanged;
        //        }
        //    }

        //    if (e.OldItems != null)
        //    {
        //        foreach (ProductoMayViewModel viejoProducto in e.OldItems)
        //        {
        //            viejoProducto.PropertyChanged -= ProductoViewModel_PropertyChanged;
        //        }
        //    }

        //    //ActualizarTotalPreciosGenerales();
        //};
    }


    private async void CargarPreciosDesdeAPIScan(string codigoProducto)
    {
        string apiUrl = $"http://192.168.1.3:8022/api/Inventario/buscarArticulo?codbarras={codigoProducto}&alm={_codigoAlmacen}";

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
                    //resultadosLista.ItemsSource = resultados;
                    listaProductos.Add(new ProductoInventarioViewModel
                    {
                        Codigo = resultados[0].Codigo,
                        descripcion = resultados[0].descripcion,
                        Almacen = _codigoAlmacen,

                        Cantidad_Salida = resultados[0].Cantidad,
                        Cantidad_input = 1

                    });

                    // Alternar colores de las filas
                    bool isOddRow = true;
                    foreach (var producto in listaProductos)
                    {
                        producto.RowColor = isOddRow ? Colors.Silver : Colors.White;
                        isOddRow = !isOddRow;
                    }


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
        //ActualizarTotalCantidades();


    }


    private void ProductoViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        //if (e.PropertyName == nameof(ProductoViewModel.TotalPrecios))
        //{
        //    ActualizarTotalPreciosGenerales();
        //}
        //else if (e.PropertyName == nameof(ProductoViewModel.Precio1Seleccionado))
        //{
        //    ActualizarTotalPreciosGenerales();
        //}
        // if (e.PropertyName == nameof(ProductoViewModel.Precio2Seleccionado))
        //{
        //    ActualizarTotalPreciosGenerales();
        //}
        if (e.PropertyName == nameof(ProductoInventarioViewModel.Cantidad))
        {
            string validaCantidad = "";
        }
        //else if (e.PropertyName == nameof(ProductoViewModel.PrecioTotal))
        //{
        //    ActualizarTotalPreciosGenerales();
        //}
    }

    //private void ActualizarTotalPreciosGenerales()
    //{
    //    OnPropertyChanged(nameof(TotalPreciosSeleccionadosGeneral));
    //}

    private void AgregarProducto_Clicked(object sender, EventArgs e)

    {
        Navigation.PushAsync(new BuscarProductosInventarioPage(_codigoAlmacen));
    }
    // Propiedad para la suma de cantidades
    private decimal totalCantidades;

    public decimal TotalCantidades
    {
        get { return totalCantidades; }
        set
        {
            if (totalCantidades != value)
            {
                totalCantidades = value;
                OnPropertyChanged(nameof(TotalCantidades));
            }
        }
    }

    //public decimal TotalPreciosSeleccionadosGeneral
    //{
    //    get
    //    {
    //        return listaProductos.Sum(producto => producto.PrecioTotal);
    //    }
    //}

    //public void ActualizarTotalPrecios()
    //{
    //    OnPropertyChanged(nameof(TotalPreciosSeleccionadosGeneral));
    //}
    private void EscanearCodigoBarras_Clicked(object sender, EventArgs e)
    {
        Navigation.PushAsync(new ScanBarCodeInventarioPage());
    }

    //public void ActualizarTotalCantidades()
    //{
    //    Device.BeginInvokeOnMainThread(() =>
    //    {
    //        //TotalCantidades = listaProductos.Sum(producto => producto.TotalPrecios);
    //        OnPropertyChanged(nameof(TotalCantidades));
    //    });
    //}
    private void CancelarPopupFinalizaVenta(object sender, EventArgs e)
    {
        CancelarPoputFinaliza();
    }
    private void CancelarPoputFinaliza()
    {
        popupViewFinalizaVenta.IsVisible = false;
        finalizarVentaButton.IsVisible = true;
        btnAgregar.IsVisible = true;
        btnScan.IsVisible = true;
    }

    private void FinalizarVenta_Clicked(object sender, EventArgs e)
    {
        string exiteproducto = "0";
        foreach (var detalle in listaProductos)
        {
            exiteproducto = "1";
        }
        if (exiteproducto == "0")
        {
            string mensaje2 = "Por favor ingresar productos antes de continuar.";
            MostrarMensaje(mensaje2);
            return;
        }
        //if (CodTransacciones == "")
        //{
        //    string mensaje2 = "Por favor ingresar Tipo de transaccion antes de continuar.";
        //    MostrarMensaje(mensaje2);
        //    return;
        //}
        //if (CodProveedor == "")
        //{
        //    string mensaje2 = "Por favor ingresar Proveedor antes de continuar.";
        //    MostrarMensaje(mensaje2);
        //    return;
        //}
        //if (CodTipoDocumento == "")
        //{
        //    string mensaje2 = "Por favor ingresar Tipo de Documento antes de continuar.";
        //    MostrarMensaje(mensaje2);
        //    return;
        //}

        // Mostrar la "ventana emergente"
        popupViewFinalizaVenta.IsVisible = true;
        finalizarVentaButton.IsVisible = false;
        btnAgregar.IsVisible = false;
        btnScan.IsVisible = false;
    }

    private void EliminarCommand(object sender, EventArgs e)
    {
        if (sender is Button button)
        {
            if (button.BindingContext is ProductoInventarioViewModel producto)
            {
                listaProductos.Remove(producto);
                //ActualizarTotalCantidades(); // Actualizar el total después de agregar un producto

            }
        }
    }
    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
    protected override void OnDisappearing()
    {
        base.OnDisappearing();
    }

    private void EliminarMenuItem_Clicked(object sender, EventArgs e)
    {
        if (sender is MenuItem menuItem && menuItem.CommandParameter is ProductoInventarioViewModel producto)
        {
            // Aquí debes eliminar el producto de tu listaProductos
            listaProductos.Remove(producto);
            //ActualizarTotalCantidades(); // Actualiza el total después de eliminar un producto
        }
    }

    private void LimpiarVenta()
    {
        //nombreEntry.Text="";
        //direccionEntry.Text = "";
        //dniEntry.Text = "";
        //RucEntry.Text = "";
        //vueltoLabel.Text = "";

        //montoPagadoEntry.Text = "";
        // Limpiar la lista después de guardar
        listaProductos.Clear();
    }
    private async void CerrarPopupFinalizaVenta(object sender, EventArgs e)
    {
        // Ocultar la "ventana emergente"
        popupViewFinalizaVenta.IsVisible = false;
        btnScan.IsVisible = true;
        btnAgregar.IsVisible = true;
        finalizarVentaButton.IsVisible = true;
        //var CodTransacciones_txt = CodTransacciones ?? "";

        //var ordenCompra = OrdenCompraEntry.Text ?? "";
        //var CodProveedor_txt = CodProveedor ?? "";

        //var CodTipoDocumento_txt = CodTipoDocumento ?? "";
        //var correlativo = CorrelativoEntry.Text ?? "";
        var detalles = detallesEditor.Text ?? "";

        //if (ordenCompra == "")
        //{
        //    string mensaje2 = "Por favor ingresar orden de compra, antes de continuar.";
        //    MostrarMensaje(mensaje2);
        //    return;
        //}
        //if (correlativo == "")
        //{
        //    string mensaje2 = "por favor ingresar correlativo de documento selecionado antes de continuar.";
        //    MostrarMensaje(mensaje2);
        //    return;
        //}
        if (detalles == "")
        {
            string mensaje2 = "Por favor ingresar detalle antes de continuar.";
            MostrarMensaje(mensaje2);
            return;
        }

        VentaData ventaData = new VentaData
        {
            Faccab = new FACCAB
            {
                CodTransacciones = "",
                ordenCompra = "",
                CodProveedor = "",
                CodTipoDocumento = "",
                correlativo = "",
                detalles = detalles,
                CFUSER = App.user.Id.ToString(),     // Código de usuario
                TipoNota = "1",
                Almacen = _codigoAlmacen
            },
            FacdetList = new List<FACDET>()
        };

        int Secuencia = 0;
        foreach (var detalle in listaProductos)
        {
            decimal PrecioUnida = 0m;
            decimal PrecioReal = 0m;

            Secuencia++;
            // Aquí debes llenar los datos de detalle según tu lógica
            FACDET facdet = new FACDET
            {
                DFSECUEN = Secuencia, // Secuencia del detalle (debe obtenerse de algún lugar)
                DFCODIGO = detalle.Codigo, // Código del producto
                DFCANTID = detalle.Cantidad_input, // Cantidad
                DFPREC = 0, // Precio de venta por unidad
                Almacen = detalle.Almacen
            };

            ventaData.FacdetList.Add(facdet);
        }

        // Convertir el objeto a JSON
        string json = JsonConvert.SerializeObject(ventaData);

        // Enviar los datos a través de la API
        //await EnviarDatosAPI(json);
        // Enviar los datos a través de la API
        string respuestaJson = await EnviarDatosAPI(json);

        if (respuestaJson == "Transacción procesada exitosamente.")
        {
            string mensaje2 = "Se registro nota de ingreso.";
            MostrarMensaje(mensaje2);

            LimpiarVenta();

            CancelarPoputFinaliza();

            Navigation.PopToRootAsync();
            //var pageToRemove = Navigation.NavigationStack.FirstOrDefault(p => p.GetType() == typeof(VentaRapidaPage));
            //if (pageToRemove != null)
            //{
            //    Navigation.RemovePage(pageToRemove);
            //}
        }
        else
        {
            string mensaje2 = "Error al registrar, intentar nuevamente.";
            MostrarMensaje(mensaje2);
        }

    }



    private void MostrarMensaje(string mensaje)
    {
        // Utilizar DisplayAlert para mostrar el mensaje en el móvil
        App.Current.MainPage.DisplayAlert("Mensaje", mensaje, "Aceptar");
    }
    private async Task<string> EnviarDatosAPI(string jsonData)
    {
        string respuesta = null;

        try
        {
            using (HttpClient httpClient = new HttpClient())
            {
                // URL de tu API
                string apiUrl = "http://192.168.1.3:8022/api/Inventario/procesarInventario";

                // Configurar la solicitud HTTP
                var content = new StringContent(jsonData, Encoding.UTF8, "application/json");

                // Realizar la solicitud POST
                HttpResponseMessage response = await httpClient.PostAsync(apiUrl, content);

                // Manejar la respuesta (puedes verificar el código de estado, etc.)
                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine("Datos enviados correctamente");
                    respuesta = await response.Content.ReadAsStringAsync();
                }
                else
                {
                    Console.WriteLine($"Error en la solicitud: {response.StatusCode}");
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error en la solicitud: {ex.Message}");
        }

        return respuesta;
    }

    private async void CargarPreciosDesdeAPI(string codigoProducto, string nombreProducto, string codigoAlmacen, int cantidad)
    {
        try
        {
            //// URL de la API con el código del precio
            //string apiUrl = $"http://192.168.1.3:8022/api/Inventario/buscarArticulo?codbarras={codigoProducto}&alm={codigoAlmacen}";

            // Usar HttpClient dentro de un bloque using
            //using HttpClient httpClient = new HttpClient();

            // Realizar la solicitud GET
            //HttpResponseMessage response = await httpClient.GetAsync(apiUrl);

            // Verificar si la solicitud fue exitosa
            //if (response.IsSuccessStatusCode)
            {
                // Obtener el contenido de la respuesta como cadena JSON
                //string jsonContent = await response.Content.ReadAsStringAsync();

                // Deserializar la cadena JSON a una lista de precios
                //var apiResponse = JsonConvert.DeserializeObject<PrecioViewModel>(jsonContent);

                //var precios = new ObservableCollection<Precio>(apiResponse.Data); 

                decimal PrecioMin = 0;
                decimal PrecioMay = 0;
                string cod_listPr1 = "";
                string cod_listPr2 = "";

                // Agregar los precios a la lista de productos
                //foreach (var precio in precios)
                //{
                //    if (precio.DESC_CORTO == "P.MIN")
                //    {
                //        PrecioMin = Math.Round(decimal.Parse(precio.PRE_ACT), 2);
                //        cod_listPr1 = precio.COD_LISPRE;
                //    }
                //    else if (precio.DESC_CORTO == "P.MAY")
                //    {
                //        PrecioMay = Math.Round(decimal.Parse(precio.PRE_ACT), 2);
                //        cod_listPr2 = precio.COD_LISPRE;
                //    }
                //}

                listaProductos.Add(new ProductoInventarioViewModel
                {
                    Codigo = codigoProducto,
                    descripcion = nombreProducto,
                    Almacen = codigoAlmacen,
                    Cantidad_Salida = cantidad,
                    Cantidad_input = 1
                });

                // Alternar colores de las filas
                bool isOddRow = true;
                foreach (var producto in listaProductos)
                {
                    producto.RowColor = isOddRow ? Colors.Silver : Colors.White;
                    isOddRow = !isOddRow;
                }

                // Actualizar el total después de agregar un producto
                //ActualizarTotalCantidades();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error en la solicitud: {ex.Message}");
        }
    }
    // Declarar un evento que se disparará cuando haya cambios relevantes
    public event EventHandler CambiosRealizados;

    // Método para notificar que se han realizado cambios
    protected virtual void OnCambiosRealizados()
    {
        CambiosRealizados?.Invoke(this, EventArgs.Empty);
    }
    public class VentaData
    {
        public FACCAB Faccab { get; set; }
        public List<FACDET> FacdetList { get; set; }
    }
    public class FACCAB
    {
        public string CodTransacciones { get; set; }
        public string ordenCompra { get; set; }
        public string CodProveedor { get; set; }
        public string CodTipoDocumento { get; set; }
        public string correlativo { get; set; }
        public string detalles { get; set; }
        public string CFUSER { get; set; }
        public string TipoNota { get; set; }

        public string Almacen { get; set; }
    }
    public class FACDET
    {
        public int DFSECUEN { get; set; }
        public string DFCODIGO { get; set; }
        public decimal DFCANTID { get; set; }
        public decimal DFPREC { get; set; }
        public string Almacen { get; set; }
    }
    public class RespuestaVenta
    {
        public string status { get; set; }
        public ResultVenta result { get; set; }
    }
    public class ResultVenta
    {
        public string mensaje { get; set; }
        public string tpd { get; set; }
        public string serie_doc { get; set; }
        public string numero_doc { get; set; }
    }
    public class Precio
    {
        public string codigo { get; set; }
        public string cantidad { get; set; }
        public string DESC_CORTO { get; set; }
        public string descripcion { get; set; }
    }
    public class PrecioViewModel
    {
        public int InternalStatus { get; set; }
        public List<Precio> Data { get; set; }
        public string Mensaje { get; set; }
    }



    //private async void CargarDocumentosDesdeAPI()
    //{
    //    try
    //    {
    //        using (HttpClient httpClient = new HttpClient())
    //        {
    //            // URL de tu API
    //            string apiUrl = "http://192.168.1.3:8022/api/Inventario/listatipodoc";

    //            // Realizar la solicitud GET
    //            HttpResponseMessage response = await httpClient.GetAsync(apiUrl);

    //            if (response.IsSuccessStatusCode)
    //            {
    //                // Obtener el contenido de la respuesta como cadena JSON
    //                string jsonContent = await response.Content.ReadAsStringAsync();

    //                // Deserializar la cadena JSON a una lista de documentos
    //                var apiResponse = JsonConvert.DeserializeObject<DocumentoResponse_Salida>(jsonContent);

    //                if (apiResponse.InternalStatus == 1 && apiResponse.Data != null)
    //                {
    //                    // Asignar la lista completa de documentos
    //                    _documentos_Salida = apiResponse.Data;

    //                    // Inicialmente mostrar todos los documentos
    //                    foreach (var doc in _documentos_Salida)
    //                    {
    //                        DocumentosFiltrados_Salida.Add(doc);
    //                    }

    //                    documentosListView.IsVisible = true;
    //                }
    //                else
    //                {
    //                    await DisplayAlert("Error", "No se pudieron cargar los documentos.", "OK");
    //                }
    //            }
    //            else
    //            {
    //                await DisplayAlert("Error", $"Error en la solicitud: {response.StatusCode}", "OK");
    //            }
    //        }
    //    }
    //    catch (Exception ex)
    //    {
    //        await DisplayAlert("Error", $"No se pudo cargar los documentos: {ex.Message}", "OK");
    //    }
    //}
    // Evento que se ejecuta cuando cambia el texto de búsqueda
    //private void OnSearchTextChanged(object sender, TextChangedEventArgs e)
    //{
    //    var textoBusqueda = e.NewTextValue;
    //    documentosListView.IsVisible = true;
    //    // Filtrar la lista de documentos en base al texto de búsqueda
    //    var documentosFiltrados = _documentos_Salida
    //        .Where(d => d.tipodoc.ToLower().Contains(textoBusqueda.ToLower()))
    //        .ToList();

    //    // Actualizar la lista filtrada
    //    DocumentosFiltrados_Salida.Clear();
    //    foreach (var documento in documentosFiltrados)
    //    {
    //        DocumentosFiltrados_Salida.Add(documento);
    //    }
    //}

    //// Evento que se ejecuta cuando se selecciona un documento de la lista
    //private void OnDocumentoSelected(object sender, ItemTappedEventArgs e)
    //{
    //    if (e.Item is Documento_Salida documentoSeleccionado)
    //    {
    //        documentosListView.IsVisible = false;
    //        codigoDocumentoLabel.Text = $"COD: {documentoSeleccionado.codigo}";
    //        CodTipoDocumento = documentoSeleccionado.codigo;
    //        // Limpiar el campo de búsqueda o mantener el valor seleccionado
    //        searchEntry.Text = documentoSeleccionado.tipodoc;
    //    }

    //    // Deseleccionar el elemento
    //    ((ListView)sender).SelectedItem = null;
    //}

    //// Evento que se ejecuta cuando cambia el texto en el campo de búsqueda
    //private async void OnSearchProveedorTextChanged(object sender, TextChangedEventArgs e)
    //{
    //    var textoBusqueda = e.NewTextValue;

    //    if (!string.IsNullOrEmpty(textoBusqueda))
    //    {
    //        // Realizar la búsqueda en la API cada vez que cambia el texto
    //        await BuscarProveedoresDesdeAPI(textoBusqueda);
    //    }
    //    else
    //    {
    //        // Si el texto está vacío, mostrar todos los proveedores
    //        await BuscarProveedoresDesdeAPI("%25"); // Enviar petición para obtener todos los proveedores
    //    }
    //}

    // Método para realizar la búsqueda en la API
    //private async Task BuscarProveedoresDesdeAPI(string busqueda)
    //{
    //    try
    //    {
    //        using (HttpClient httpClient = new HttpClient())
    //        {
    //            // URL de la API con el parámetro de búsqueda
    //            string apiUrl = $"http://192.168.1.3:8022/api/Inventario/listaProveedores?descripcion={busqueda}%25";

    //            // Realizar la solicitud GET
    //            HttpResponseMessage response = await httpClient.GetAsync(apiUrl);

    //            if (response.IsSuccessStatusCode)
    //            {
    //                // Obtener el contenido de la respuesta como cadena JSON
    //                string jsonContent = await response.Content.ReadAsStringAsync();

    //                // Deserializar la cadena JSON a ProveedorResponse
    //                var proveedorResponse = JsonConvert.DeserializeObject<ProveedorResponse_Salida>(jsonContent);

    //                // Verificar si el InternalStatus es exitoso
    //                if (proveedorResponse.InternalStatus == 1 && proveedorResponse.Data != null)
    //                {
    //                    // Limpiar la lista filtrada
    //                    ProveedoresFiltrados_Salida.Clear();

    //                    // Agregar los proveedores obtenidos a la lista filtrada
    //                    foreach (var proveedor in proveedorResponse.Data)
    //                    {
    //                        ProveedoresFiltrados_Salida.Add(proveedor);
    //                    }

    //                    // Mostrar el ListView si hay resultados
    //                    proveedoresListView.IsVisible = ProveedoresFiltrados_Salida.Count > 0;
    //                }
    //                else
    //                {
    //                    // Si no hay datos o InternalStatus no es exitoso
    //                    await DisplayAlert("Error", proveedorResponse.Mensaje ?? "No se encontraron proveedores.", "OK");
    //                    proveedoresListView.IsVisible = false;
    //                }
    //            }
    //            else
    //            {
    //                Console.WriteLine($"Error en la solicitud: {response.StatusCode}");
    //            }
    //        }
    //    }
    //    catch (Exception ex)
    //    {
    //        Console.WriteLine($"Error en la solicitud: {ex.Message}");
    //    }
    //}

    //// Evento que se ejecuta cuando se selecciona un proveedor de la lista
    //private void OnProveedorSelected(object sender, ItemTappedEventArgs e)
    //{
    //    if (e.Item is Proveedor_Salida proveedorSeleccionado)
    //    {
    //        // Mostrar alerta con el proveedor seleccionado
    //        codigoProveedorLabel.Text = $"COD: {proveedorSeleccionado.codigo}";
    //        CodProveedor = proveedorSeleccionado.codigo;
    //        // Ocultar el ListView después de la selección
    //        proveedoresListView.IsVisible = false;

    //        // Colocar el nombre del proveedor seleccionado en el campo de búsqueda
    //        searchProveedorEntry.Text = proveedorSeleccionado.descripcion;
    //    }

    //    // Deseleccionar el elemento
    //    ((ListView)sender).SelectedItem = null;
    //}

    //private async void CargarTransaccionesDesdeAPI()
    //{
    //    try
    //    {
    //        using (HttpClient httpClient = new HttpClient())
    //        {
    //            string apiUrl = "http://192.168.1.3:8022/api/Inventario/listatransacsalida";
    //            HttpResponseMessage response = await httpClient.GetAsync(apiUrl);

    //            if (response.IsSuccessStatusCode)
    //            {
    //                string jsonContent = await response.Content.ReadAsStringAsync();
    //                var apiResponse = JsonConvert.DeserializeObject<TransaccionResponse_Salida>(jsonContent);

    //                if (apiResponse.InternalStatus == 1 && apiResponse.Data != null)
    //                {
    //                    _transacciones_Salida = new ObservableCollection<Transaccion_Salida>(apiResponse.Data);

    //                    // Inicialmente mostrar todas las transacciones
    //                    foreach (var transaccion in _transacciones_Salida)
    //                    {
    //                        TransaccionesFiltradas_Salida.Add(transaccion);
    //                    }

    //                    transaccionesListView.IsVisible = true;
    //                }
    //                else
    //                {
    //                    await DisplayAlert("Error", "No se pudieron cargar las transacciones.", "OK");
    //                }
    //            }
    //            else
    //            {
    //                await DisplayAlert("Error", $"Error en la solicitud: {response.StatusCode}", "OK");
    //            }
    //        }
    //    }
    //    catch (Exception ex)
    //    {
    //        await DisplayAlert("Error", $"No se pudo cargar las transacciones: {ex.Message}", "OK");
    //    }
    //}
    //private void OnTransaccionSearchBarTextChanged(object sender, TextChangedEventArgs e)
    //{
    //    var textoBusqueda = e.NewTextValue?.ToLower() ?? string.Empty;

    //    // Filtrar la lista de transacciones en base al texto de búsqueda
    //    var transaccionesFiltradas = _transacciones_Salida
    //        .Where(t => t.descripcion.ToLower().Contains(textoBusqueda) || t.codigo.ToLower().Contains(textoBusqueda))
    //        .ToList();

    //    // Limpiar la lista filtrada
    //    TransaccionesFiltradas_Salida.Clear();
    //    foreach (var transaccion in transaccionesFiltradas)
    //    {
    //        TransaccionesFiltradas_Salida.Add(transaccion);
    //    }

    //    transaccionesListView.IsVisible = TransaccionesFiltradas_Salida.Count > 0; // Mostrar el Picker solo si hay resultados
    //}

    //private void OnTransaccionSelected(object sender, ItemTappedEventArgs e)
    //{
    //    //var selectedTransaccion = e.Item as Transaccion;

    //    if (e.Item is Transaccion_Salida transaccionSeleccionada)
    //    {
    //        codigoLabel.Text = $"COD: {transaccionSeleccionada.codigo}";
    //        // Ocultar el ListView después de la selección
    //        transaccionesListView.IsVisible = false;
    //        CodTransacciones = transaccionSeleccionada.codigo;
    //        // Colocar la descripción de la transacción seleccionada en el campo de búsqueda
    //        transaccionSearchBar.Text = transaccionSeleccionada.descripcion;
    //    }

    // // Deseleccionar el elemento
    // ((ListView)sender).SelectedItem = null;
    //}


}
//public class Documento_Salida
//{
//    public string codigo { get; set; }
//    public string tipodoc { get; set; }
//}

//// Estructura de la respuesta de la API
//public class DocumentoResponse_Salida
//{
//    public int InternalStatus { get; set; }
//    public List<Documento_Salida> Data { get; set; }
//    public string Mensaje { get; set; }
//}

//public class Proveedor_Salida
//{
//    public string codigo { get; set; }
//    public string descripcion { get; set; }
//}
//public class ProveedorResponse_Salida
//{
//    public int InternalStatus { get; set; }
//    public List<Proveedor_Salida> Data { get; set; }
//    public string Mensaje { get; set; }
//}
//public class Transaccion_Salida
//{
//    public string codigo { get; set; }
//    public string descripcion { get; set; }
//}

//public class TransaccionResponse_Salida
//{
//    public int InternalStatus { get; set; }
//    public List<Transaccion_Salida> Data { get; set; }
//    public string Mensaje { get; set; }
//}