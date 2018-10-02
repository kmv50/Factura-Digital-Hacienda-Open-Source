using DataModel;
using DataModel.EF;
using System;
using System.Collections.Generic;
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

namespace FacturaDigital.Settings
{
    /// <summary>
    /// Lógica de interacción para Consecutivos.xaml
    /// </summary>
    public partial class Consecutivos : Page, ILog
    {
        public Consecutivos()
        {
            InitializeComponent();
            ObtenerConteoActual();
        }

        private void ObtenerConteoActual()
        {
            try
            {
                if(Recursos.RecursosSistema.Contribuyente == null)
                {
                    MessageBox.Show("Debe completar el perfil de Hacienda antes de continuar","Falta de datos",MessageBoxButton.OK,MessageBoxImage.Stop);
                    Recursos.RecursosSistema.MainConteiner.Content = new Contribuyente.PerfilHacienda();
                    return;
                }
                Contribuyente_Consecutivos consecutivos = null;
                using (db_FacturaDigital db = new db_FacturaDigital())
                {
                    consecutivos = db.Contribuyente_Consecutivos.FirstOrDefault(q => q.Id_Contribuyente == Recursos.RecursosSistema.Contribuyente.Id_Contribuyente);
                    if(consecutivos == null)
                    {
                        consecutivos = new Contribuyente_Consecutivos()
                        {
                            Consecutivo_Facturas = 1,
                            Consecutivo_NotasCredito = 1,
                            Consecutivo_Tiquete_Electrónico = 1,
                            Id_Contribuyente = Recursos.RecursosSistema.Contribuyente.Id_Contribuyente
                        };
                        db.Contribuyente_Consecutivos.Add(consecutivos);
                        db.SaveChanges();
                    }
                }

                txt_TiqueteElectronico.Text = consecutivos.Consecutivo_Tiquete_Electrónico.ToString();
                txt_facturas.Text = consecutivos.Consecutivo_Facturas.ToString();
                txt_NotasCredito.Text = consecutivos.Consecutivo_NotasCredito.ToString();

            }
            catch(Exception ex)
            {
                this.LogError(ex);
            }
        }

        private void Guardar(object sender, RoutedEventArgs e)
        {
            try
            {
                int FacturasConsecutivo ,  NotasCreditoConsecutivo , TiqueteElectronico , Confirmacion;
                if(!int.TryParse(txt_facturas.Text,out FacturasConsecutivo) || !int.TryParse(txt_NotasCredito.Text,out NotasCreditoConsecutivo) || !int.TryParse(txt_TiqueteElectronico.Text, out TiqueteElectronico) || !int.TryParse(txt_Confirmacion.Text , out Confirmacion) || TiqueteElectronico <= 0 || FacturasConsecutivo <= 0|| NotasCreditoConsecutivo <= 0 || Confirmacion <= 0)
                {
                    MessageBox.Show("Error de formato para los consecutivo solo se permiten numeros","Validacion",MessageBoxButton.OK,MessageBoxImage.Stop);
                    return;
                }

                using (db_FacturaDigital db = new db_FacturaDigital())
                {
                    Contribuyente_Consecutivos consecutivos = db.Contribuyente_Consecutivos.First(q => q.Id_Contribuyente == Recursos.RecursosSistema.Contribuyente.Id_Contribuyente);
                    if(consecutivos.Consecutivo_Facturas > FacturasConsecutivo)
                    {
                        if(MessageBox.Show("Esta ingresando un numero menor al que ya existe para los consecutivos de facturas esto puede causar problemas de colisiones desea continuar", "Validacion", MessageBoxButton.YesNo, MessageBoxImage.Information) == MessageBoxResult.No)
                        return;
                    }

                    if (consecutivos.Consecutivo_Tiquete_Electrónico > TiqueteElectronico)
                    {
                        if (MessageBox.Show("Esta ingresando un numero menor al que ya existe para los consecutivos de notas de credito esto puede causar problemas de colisiones desea continuar", "Validacion", MessageBoxButton.YesNo, MessageBoxImage.Information) == MessageBoxResult.No)
                            return;
                    }


                    if (consecutivos.Consecutivo_Confirmacion > Confirmacion)
                    {
                        if (MessageBox.Show("Esta ingresando un numero menor al que ya existe para los consecutivos de confirmacion esto puede causar problemas de colisiones desea continuar", "Validacion", MessageBoxButton.YesNo, MessageBoxImage.Information) == MessageBoxResult.No)
                            return;
                    }

                    if (consecutivos.Consecutivo_NotasCredito > NotasCreditoConsecutivo)
                    {
                        if (MessageBox.Show("Esta ingresando un numero menor al que ya existe para los consecutivos de notas de credito esto puede causar problemas de colisiones desea continuar", "Validacion", MessageBoxButton.YesNo, MessageBoxImage.Information) == MessageBoxResult.No)
                            return;
                    }

                    consecutivos.Consecutivo_Facturas = FacturasConsecutivo;
                    consecutivos.Consecutivo_NotasCredito = NotasCreditoConsecutivo;
                    consecutivos.Consecutivo_Tiquete_Electrónico = TiqueteElectronico;
                    consecutivos.Consecutivo_Confirmacion = FacturasConsecutivo;
                    db.SaveChanges();
                    MessageBox.Show("Cambio realizado correctamente","Estado",MessageBoxButton.OK);
                }

            }catch(Exception ex)
            {
                this.LogError(ex);
            }
        }
    }
}
