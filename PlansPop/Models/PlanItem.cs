using Parse;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlansPop.Models
{
    public class PlanItem
    {
        public ParseObject obj { get; set; }
        public string Nombre { get; set; }
        public string Fecha { get; set; }
        public string ImagenUrl { get; set; }
        public string Direccion { get; set; }
        public string Descripcion { get; set; }

    }
}