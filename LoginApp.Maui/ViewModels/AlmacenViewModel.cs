using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoginApp.Maui.ViewModels
{
    public class AlmacenViewModel
    {
        public string Codigo { get; set; }
        public string Descripcion { get; set; }
    }

    public class ApiResponseViewModel
    {
        public int InternalStatus { get; set; }
        public List<AlmacenViewModel> Data { get; set; }
        public string Mensaje { get; set; }
    }
}
