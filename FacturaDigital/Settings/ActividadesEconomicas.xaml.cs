using DataModel;
using DataModel.EF;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FacturaDigital.Settings
{
    public class ResposeActividadesEconomicas {
        public string nombre { set; get; }
        public string tipoIdentificacion { set; get; }
        public List<Contribuyente_ActividadesEconomicas> actividades { set; get; }
    }
    /// <summary>
    /// Lógica de interacción para ActividadesEconomicas.xaml
    /// </summary>
    public partial class ActividadesEconomicas : Page , ILog
    {
        private List<Contribuyente_ActividadesEconomicas> ActividadesEco;
        public ActividadesEconomicas()
        {
            InitializeComponent();
            CargarActividadesEconomicas();
        }

        private void CargarActividadesEconomicas() {
            try
            {
                using (db_FacturaDigital db = new db_FacturaDigital()) {
                    ActividadesEco = db.Contribuyente_ActividadesEconomicas.ToList();
                }
                dgv_actividades.ItemsSource = ActividadesEco;
            }
            catch (Exception ex) {
                this.LogError(ex);
            }
        }

        private void DescargarActividadesEconomicas(object sender, RoutedEventArgs e)
        {
            if (Recursos.RecursosSistema.Contribuyente == null)
            {
                MessageBox.Show("Debe completar el perfil de Hacienda antes de continuar", "Falta de datos", MessageBoxButton.OK, MessageBoxImage.Stop);
                Recursos.RecursosSistema.MainConteiner.Content = new Contribuyente.PerfilHacienda();
                return;
            }

            HttpResponseMessage response = null;
            Task.Run(() =>
            {
                using (HttpClient client = new HttpClient())
                {
                    client.BaseAddress = new Uri("https://api.hacienda.go.cr");
                    response = client.GetAsync($"/fe/ae?identificacion={Recursos.RecursosSistema.Contribuyente.Identificacion_Numero}").Result;
                }

            }).ContinueWith((q) =>
            {
                if (q.IsFaulted) {
                    MessageBox.Show("Error al obtener los datos de actividades economicas", "Falta de datos", MessageBoxButton.OK, MessageBoxImage.Stop);
                    this.LogError(q.Exception);
                    return;
                }

                string json = response.Content.ReadAsStringAsync().Result;
                ResposeActividadesEconomicas actividades = JsonConvert.DeserializeObject<ResposeActividadesEconomicas>(json);
                ActividadesEco = actividades.actividades;
                dgv_actividades.ItemsSource = ActividadesEco;
            }, TaskScheduler.FromCurrentSynchronizationContext());
           
        }

        private void GuardarActividadesEconomicas(object sender, RoutedEventArgs e)
        {
            try
            {
                using (db_FacturaDigital db = new db_FacturaDigital()) {
                    db.Contribuyente_ActividadesEconomicas.RemoveRange(db.Contribuyente_ActividadesEconomicas);
                    db.SaveChanges();
                    db.Contribuyente_ActividadesEconomicas.AddRange(ActividadesEco);
                    db.SaveChanges();
                }
                MessageBox.Show("Actividades economicas guardadas correctamente", "Informacion", MessageBoxButton.OK, MessageBoxImage.Information);

            }
            catch (Exception ex) {
                this.LogError(ex);
                MessageBox.Show("Error al guardar las actividades economicas", "Error de base de datos", MessageBoxButton.OK, MessageBoxImage.Stop);

            }
        }
    }
}
