using DataModel;
using DataModel.EF;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
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

namespace FacturaDigital.Historial
{
    public class FacturaGrid
    {
        public int Id_Factura { get; set; }
        public int Estado { get; set; }
        public int NumeroConsecutivo { get; set; }
        public string Receptor_Nombre { get; set; }
        public int? Receptor_Telefono_Numero { get; set; }
        public string Receptor_CorreoElectronico { get; set; }
        public int Id_TipoDocumento { get; set; }
        public decimal TotalComprobante { get; set; }
        public DateTime Fecha_Emision_Documento { set; get; }

        public string EstadoHaciendaLabel { get {
                return ((EstadoComprobante)Estado).GetAttribute<DescriptionAttribute>().Description;
            }
        }

        public string GetTipoDocumentoLabel
        {
            get {
                return ((Tipo_documento)Id_TipoDocumento).GetAttribute<DescriptionAttribute>().Description;
            }
        }
    }
    /// <summary>
    /// Interaction logic for Historial.xaml
    /// </summary>
    public partial class Historial : Page , ILog
    {
        int CurrentPage;
        int PaginasTotales;
        int TamanoPagina = 10;
        DateTime FechaInicioActual , FechaFinalActual;
        public Historial()
        {
            CurrentPage = 0;
            InitializeComponent();
            DateTime Now = DateTime.Now;
            dp_FechaFinal.DisplayDateEnd = Now;
            dp_FechaFinal.SelectedDate = Now;
            
            dp_FechaInicio.SelectedDate = Now.AddMonths(-1);
            dp_FechaInicio.DisplayDateEnd = Now.AddDays(-1);
        }

        private void OnLoad(object sender, RoutedEventArgs e)
        {
            Filtrar(sender,e);
        }

        private int GetCantidadPagina() {
            using(db_FacturaDigital db = new db_FacturaDigital())
            {
                int cantidadElementos = db.Factura.Count(q => q.Fecha_Emision_Documento >= FechaInicioActual && q.Fecha_Emision_Documento <= FechaFinalActual);
                if (cantidadElementos == 0)
                    return 0;
                decimal Elementos = (decimal)cantidadElementos / TamanoPagina;
                return (int)Math.Round(Elementos, 0,MidpointRounding.AwayFromZero);
            }
        }

        private void Filtrar(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!dp_FechaInicio.SelectedDate.HasValue || !dp_FechaFinal.SelectedDate.HasValue) {
                    MessageBox.Show("Por favor llene los filtros de fecha antes de continuar","Validacion",MessageBoxButton.OK,MessageBoxImage.Stop);
                    return;
                }
                CurrentPage = 0;
                FechaInicioActual = dp_FechaInicio.SelectedDate.Value.AddDays(-1);
                FechaFinalActual = dp_FechaFinal.SelectedDate.Value.AddDays(1);
                PaginasTotales = GetCantidadPagina();
                if (PaginasTotales == 0)
                    return;

                
                RenderTable();
            }
            catch(Exception ex)
            {
                this.LogError(ex);
                MessageBox.Show("Error al filtrar los datos", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Next(object sender, RoutedEventArgs e)
        {
            try
            {
                if (PaginasTotales == 0)
                    return;

                if (CurrentPage + 1 >= PaginasTotales)
                    return;
                else
                    CurrentPage++;

                RenderTable();
            }catch(Exception ex)
            {
                this.LogError(ex);
                MessageBox.Show("Error al mostrar la siguiente pagina", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Previous(object sender, RoutedEventArgs e)
        {
            try
            {
                if (PaginasTotales == 0)
                    return;

                if (CurrentPage - 1 < 0)
                    return;
                else
                    CurrentPage--;

                RenderTable();
            }catch(Exception ex)
            {
                this.LogError(ex);
                MessageBox.Show("Error al mostrar la pagina anterior","Error",MessageBoxButton.OK,MessageBoxImage.Error);
            }
        }

        private void MostrarFactura(object sender, RoutedEventArgs e)
        {
            try
            {
                Button btn = sender as Button;
                if (btn == null)
                    return;

                int IdFactura = (int)btn.CommandParameter;

                new DetalleFactura()
                {
                    Owner = Window.GetWindow(this)
                }.ShowDialog();
            }
            catch(Exception ex)
            {
                this.LogError(ex);
                MessageBox.Show("Error al Mostrar el detalle de la factura seleccionada","Error",MessageBoxButton.OK,MessageBoxImage.Error);
            }
        }

        private void RenderTable()
        {
            using (db_FacturaDigital db = new db_FacturaDigital())
            {
              List<FacturaGrid> data = db.Factura.Where(q => q.Fecha_Emision_Documento >= FechaInicioActual && q.Fecha_Emision_Documento <= FechaFinalActual)
                    .Select(q => new FacturaGrid()
                    {
                        Id_Factura = q.Id_Factura,
                        Estado = q.Estado,
                        Id_TipoDocumento = q.Id_TipoDocumento,
                        NumeroConsecutivo = q.NumeroConsecutivo,
                        Receptor_CorreoElectronico = q.Receptor_CorreoElectronico,
                        Receptor_Nombre = q.Receptor_Nombre,
                        Receptor_Telefono_Numero = q.Receptor_Telefono_Numero,
                        TotalComprobante = q.TotalComprobante,
                        Fecha_Emision_Documento = q.Fecha_Emision_Documento
                    }).OrderByDescending(q => q.Fecha_Emision_Documento)
                    .Skip(CurrentPage * TamanoPagina)
                    .Take(TamanoPagina)
                    .ToList();

                dgv_Historial.ItemsSource = data;
            }

        }
    }
}
