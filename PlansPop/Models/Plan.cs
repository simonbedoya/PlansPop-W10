using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace PlansPop.Models
{
    public class Plan
    {

        public string NombrePlan { get; set; }
        public string DescripcionPlan { get; set; }
        public string FechaPlan { get; set; }
        public string HoraPlan { get; set; }
        public StorageFile ImagenPlan { get; set; }


    }
}
