using Parse;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlansPop.Models
{
    public class PlanItemViewModel
    {
        private ObservableCollection<PlanItem> planItems = new ObservableCollection<PlanItem>();

        public ObservableCollection<PlanItem> PlanItems
        {
            get { return this.planItems; }
            set { this.planItems = value; }

        }

        public PlanItemViewModel()
        {
            CargarDatos();
        }

        public async void CargarDatos()
        {
            
            ParseQuery<ParseObject> query = ParseObject.GetQuery("Plan");
            IEnumerable<ParseObject> results = await query.FindAsync();

            ParseObject plan;
            PlanItem item;

            int tamanio = results.Count<ParseObject>();

            for (int i = 0; i < tamanio; i++)
            {
                plan = results.ElementAt<ParseObject>(i);
                ParseRelation<ParseUser> relation = plan.GetRelation<ParseUser>("Asistentes");
                IEnumerable<ParseUser> resul = await relation.Query.FindAsync();

                int numero = resul.Count<ParseUser>();

                Uri imagen = plan.Get<ParseFile>("imagen").Url;
                string imagenURL = imagen.AbsoluteUri;


                item = new PlanItem()
                {
                    obj = plan,
                    Nombre = plan.Get<string>("nombre"),
                    Fecha = plan.Get<string>("fecha"),
                    ImagenUrl = imagenURL,
                    Direccion = plan.Get<string>("direccion"),
                    Descripcion = plan.Get<string>("descripcion"),
                    Asistentes = numero.ToString()
                };

                planItems.Add(item);
            }
        }

    }
}
