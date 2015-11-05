using Parse;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlansPop.Models
{
    public class MiPlanItemViewModel
    {

        private ObservableCollection<PlanItem> miPlanItems = new ObservableCollection<PlanItem>();

        public ObservableCollection<PlanItem> MiPlanItems
        {
            get { return this.miPlanItems; }
            set { this.miPlanItems = value; }

        }

        public MiPlanItemViewModel()
        {
            CargarMisPlanes();
        }

        public async void CargarMisPlanes()
        {

            ParseUser user = ParseUser.CurrentUser;

            var query = ParseObject.GetQuery("Plan").WhereEqualTo("id_user", user);
            IEnumerable<ParseObject> results = await query.FindAsync();

            ParseObject plan;
            PlanItem item;

            int tamanio = results.Count<ParseObject>();

            for (int i = 0; i < tamanio; i++)
            {
                plan = results.ElementAt<ParseObject>(i);

                Uri imagen = plan.Get<ParseFile>("imagen").Url;
                string imagenURL = imagen.AbsoluteUri;


                item = new PlanItem()
                {
                    obj = plan,
                    Nombre = plan.Get<string>("nombre"),
                    Fecha = plan.Get<string>("fecha"),
                    ImagenUrl = imagenURL,
                    Direccion = plan.Get<string>("direccion")
                };

                miPlanItems.Add(item);
            }
        }
    }
}
