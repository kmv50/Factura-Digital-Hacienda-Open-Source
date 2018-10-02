using DataModel;
using DataModel.EF;
using DataModel.UbicacionesData;
using FacturaDigital.Recursos;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
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

namespace FacturaDigital.Contribuyente
{   
    /// <summary>
    /// Interaction logic for PerfilHacienda.xaml
    /// </summary>
    public partial class PerfilHacienda : Page , ILog
    {
        string CertificadoUrl;
        public PerfilHacienda()
        {
            CertificadoUrl = null;
            InitializeComponent();
            CargarContribuyente();
            LoadEvents();

        }

        private void LoadEvents()
        {
            cb_Provincia.SelectionChanged += SeleccionProvincia;
            cb_canton.SelectionChanged += SeleccionarCanton;
            cb_distrito.SelectionChanged += SeleccionarDistrito;
            cb_Provincia.SelectionChanged += SeleccionProvincia;
        }

        private void CargarContribuyente() {
            try
            {
                DataModel.EF.Contribuyente Contribuyente = null;
                using (db_FacturaDigital db = new db_FacturaDigital())
                {
                    Contribuyente = db.Contribuyente.FirstOrDefault();
                }

                UbicacionesData ubi = new UbicacionesData();
                List<UbicacionesType> Provincias = ubi.GetProvincias();
                cb_Provincia.ItemsSource = Provincias;
                if (Contribuyente == null)
                {
                    return;
                }

                #region Load Provincias
                cb_Provincia.SelectedItem = Provincias.First(q => q.Id == Contribuyente.Provincia);

                List<UbicacionesType> Cantones = ubi.GetCantones(Contribuyente.Provincia);
                cb_canton.ItemsSource = Cantones;
                cb_canton.SelectedItem = Cantones.First(q => q.Id == Contribuyente.Canton);

                List<UbicacionesType> Distritos = ubi.GetDistritos(Contribuyente.Provincia,Contribuyente.Canton);
                cb_distrito.ItemsSource = Distritos;
                cb_distrito.SelectedItem = Distritos.First(q => q.Id == Contribuyente.Distrito);


                List<UbicacionesType> Barrio = ubi.GetBarrios(Contribuyente.Provincia, Contribuyente.Canton , Contribuyente.Distrito);
                cb_barrio.ItemsSource = Barrio;
                cb_barrio.SelectedItem = Barrio.First(q => q.Id == Contribuyente.Barrio);

                #endregion
                txt_Nombre.Text = Contribuyente.Nombre;
                txt_comercial.Text = Contribuyente.NombreComercial;
                txt_Correo.Text = Contribuyente.CorreoElectronico;
                txt_TelefonoNumero.Text = Contribuyente.Telefono_Numero.ToString();
                txt_TelefonoRegion.Text = Contribuyente.Telefono_Codigo.ToString();
                txt_UsuarioHacienda.Text = Contribuyente.UsuarioHacienda;
                txt_otrasSenas.Text = Contribuyente.OtrasSenas;
                txt_Identificacion.Text = Contribuyente.Identificacion_Numero;
                txt_contrasenaCertificado.Text = Contribuyente.Contrasena_Certificado;
                txt_contrasena.Text = Contribuyente.ContrasenaHacienda;
                if (Contribuyente.Identificacion_Tipo == "01")
                    cb_Cedula.SelectedIndex = 0;
                else if (Contribuyente.Identificacion_Tipo == "02")
                    cb_Cedula.SelectedIndex = 1;
                else if (Contribuyente.Identificacion_Tipo == "03")
                    cb_Cedula.SelectedIndex = 2;
                else if (Contribuyente.Identificacion_Tipo == "04")
                    cb_Cedula.SelectedIndex = 3;
                
            }
            catch (Exception ex)
            {
                this.LogError(ex);
                MessageBox.Show("Ocurrio un error al obtener los datos del contribuyente", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SeleccionarCertificado(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Multiselect = false;
            openFileDialog.DefaultExt = "*.p12";
            openFileDialog.Filter = "Certificado (*.p12) | *.p12;";
            if (openFileDialog.ShowDialog() == true)
            {
                CertificadoUrl = openFileDialog.FileName;
            }
        }

        private void SeleccionProvincia(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                UbicacionesType provincia = (UbicacionesType)cb_Provincia.SelectedItem;
                cb_canton.ItemsSource = new UbicacionesData().GetCantones(provincia.Id);
            }
            catch(Exception ex)
            {
                this.LogError(ex);
                MessageBox.Show("Ocurrio al seleccionar la provincia", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SeleccionarCanton(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                UbicacionesType provincia = (UbicacionesType)cb_Provincia.SelectedItem;
                UbicacionesType canton = (UbicacionesType)cb_canton.SelectedItem;
                cb_distrito.ItemsSource = new UbicacionesData().GetDistritos(provincia.Id, canton.Id);
            }
            catch (Exception ex)
            {
                this.LogError(ex);
                MessageBox.Show("Ocurrio al seleccionar la provincia", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SeleccionarDistrito(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                UbicacionesType provincia = (UbicacionesType)cb_Provincia.SelectedItem;
                UbicacionesType canton = (UbicacionesType)cb_canton.SelectedItem;
                UbicacionesType distrito = (UbicacionesType)cb_distrito.SelectedItem;
                cb_barrio.ItemsSource = new UbicacionesData().GetBarrios(provincia.Id, canton.Id, distrito.Id);
            }
            catch (Exception ex)
            {
                this.LogError(ex);
                MessageBox.Show("Ocurrio al seleccionar la provincia", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Guardar(object sender, RoutedEventArgs e)
        {
            try
            {
                bool ExisteContribuyente = true;
                using (db_FacturaDigital db = new db_FacturaDigital())
                {
                    DataModel.EF.Contribuyente Contribuyente = db.Contribuyente.FirstOrDefault();
                    if (Contribuyente == null)
                    {
                        ExisteContribuyente = false;
                        Contribuyente = new DataModel.EF.Contribuyente();
                    }

                    #region DatosGenerales
                    if (string.IsNullOrEmpty(txt_Identificacion.Text))
                    {
                        MessageBox.Show("Favor llenar el campo de identificacion", "Validacion", MessageBoxButton.OK, MessageBoxImage.Stop);
                        return;
                    }
                    Contribuyente.Identificacion_Numero = txt_Identificacion.Text;
                    Contribuyente.Identificacion_Tipo = (cb_Cedula.SelectedItem as ComboBoxItem).Tag.ToString();


                    if (string.IsNullOrEmpty(txt_Nombre.Text))
                    {
                        MessageBox.Show("Favor llenar el campo de nombre", "Validacion", MessageBoxButton.OK, MessageBoxImage.Stop);
                        return;
                    }
                    Contribuyente.Nombre = txt_Nombre.Text;



                    if (string.IsNullOrEmpty(txt_comercial.Text))
                    {
                        MessageBox.Show("Favor llenar el campo de nombre comercial", "Validacion", MessageBoxButton.OK, MessageBoxImage.Stop);
                        return;
                    }
                    Contribuyente.NombreComercial = txt_comercial.Text;

                    if (string.IsNullOrEmpty(txt_Correo.Text))
                    {
                        MessageBox.Show("Favor llenar el campo de correo", "Validacion", MessageBoxButton.OK, MessageBoxImage.Stop);
                        return;
                    }
                    Contribuyente.CorreoElectronico = txt_Correo.Text;
                    int Region;
                    if (!int.TryParse(txt_TelefonoRegion.Text, out Region))
                    {
                        MessageBox.Show("Favor llenar el campo de region (solamente numeros) ", "Validacion", MessageBoxButton.OK, MessageBoxImage.Stop);
                        return;
                    }
                    Contribuyente.Telefono_Codigo = Region;

                    int telefono;
                    if (!int.TryParse(txt_TelefonoNumero.Text, out telefono))
                    {
                        MessageBox.Show("Favor llenar el campo de telefono (solamente numeros)", "Validacion", MessageBoxButton.OK, MessageBoxImage.Stop);
                        return;
                    }
                    Contribuyente.Telefono_Numero = telefono;                    
                    #endregion

                    #region Ubicacion
                    UbicacionesType provincia = cb_Provincia.SelectedItem as UbicacionesType;
                    if (provincia == null)
                    {
                        MessageBox.Show("Favor seleccionar una provincia", "Validacion", MessageBoxButton.OK, MessageBoxImage.Stop);
                        return;
                    }
                    Contribuyente.Provincia = provincia.Id;


                    UbicacionesType canton = cb_canton.SelectedItem as UbicacionesType;
                    if (canton == null)
                    {
                        MessageBox.Show("Favor seleccionar una Canton", "Validacion", MessageBoxButton.OK, MessageBoxImage.Stop);
                        return;
                    }
                    Contribuyente.Canton = canton.Id;


                    UbicacionesType Distrito = cb_distrito.SelectedItem as UbicacionesType;
                    if (Distrito == null)
                    {
                        MessageBox.Show("Favor seleccionar una Distrito", "Validacion", MessageBoxButton.OK, MessageBoxImage.Stop);
                        return;
                    }
                    Contribuyente.Distrito = Distrito.Id;


                    UbicacionesType Barrio = cb_barrio.SelectedItem as UbicacionesType;
                    if (Distrito == null)
                    {
                        MessageBox.Show("Favor seleccionar una Barrio", "Validacion", MessageBoxButton.OK, MessageBoxImage.Stop);
                        return;
                    }
                    Contribuyente.Barrio = Barrio.Id;


                    if(string.IsNullOrEmpty(txt_otrasSenas.Text))
                    {
                        MessageBox.Show("Favor llenar el campo de otras senas", "Validacion", MessageBoxButton.OK, MessageBoxImage.Stop);
                        return;
                    }
                    Contribuyente.OtrasSenas = txt_otrasSenas.Text;
                    #endregion

                    #region Datos Hacienda
                    if (string.IsNullOrEmpty(txt_UsuarioHacienda.Text))
                    {
                        MessageBox.Show("Favor llenar el campo de usuario de hacienda ", "Validacion", MessageBoxButton.OK, MessageBoxImage.Stop);
                        return;
                    }
                    Contribuyente.UsuarioHacienda = txt_UsuarioHacienda.Text;


                    if (string.IsNullOrEmpty(txt_contrasena.Text))
                    {
                        MessageBox.Show("Favor llenar el campo de contrasena de hacienda", "Validacion", MessageBoxButton.OK, MessageBoxImage.Stop);
                        return;
                    }
                    Contribuyente.ContrasenaHacienda = txt_contrasena.Text;
                    #endregion

                    #region Certificado
                    if(string.IsNullOrEmpty(CertificadoUrl) && (Contribuyente.Certificado == null || Contribuyente.Certificado.Length == 0))
                    {
                        MessageBox.Show("Favor seleccionar un certificado", "Validacion", MessageBoxButton.OK, MessageBoxImage.Stop);
                        return;
                    }

                    if (!string.IsNullOrEmpty(CertificadoUrl))
                    {
                        Contribuyente.Certificado = File.ReadAllBytes(CertificadoUrl);
                    }

                    if (string.IsNullOrEmpty(txt_contrasenaCertificado.Text))
                    {
                        MessageBox.Show("Favor llenar el campo de contrasena de certificado", "Validacion", MessageBoxButton.OK, MessageBoxImage.Stop);
                        return;
                    }
                    Contribuyente.Contrasena_Certificado = txt_contrasenaCertificado.Text;


                    try
                    {
                        X509Certificate2 x509 = new X509Certificate2(Contribuyente.Certificado, Contribuyente.Contrasena_Certificado, X509KeyStorageFlags.Exportable);
                        x509.Verify();
                        string Values = x509.Subject.Split(',').First(q => q.ToUpper().Contains("SERIALNUMBER"));
                        string SerialNumber = Values.Split('=')[1];
                        string OnlyNumber = new string(SerialNumber.Where(c => char.IsDigit(c)).ToArray());
                        if (!OnlyNumber.Contains(Contribuyente.Identificacion_Numero) && !Contribuyente.Identificacion_Numero.Contains(OnlyNumber))
                        {
                            if(MessageBox.Show("El numero de identificacion registrado en el certificado no concuerda con el ingresado por el usuario en el campo [Identificacion]. Aun asi desea continuar?","Validacion",MessageBoxButton.YesNo,MessageBoxImage.Question) == MessageBoxResult.No)
                            {
                                return;
                            }
                        }

                    }
                    catch (Exception ex)
                    {
                        this.LogError(ex);
                        MessageBox.Show("No se pudo abrir el ceritificado. Esto se puede deber a que la contrasena indicada no es valida o que el contenido del certificado es invalido","Error",MessageBoxButton.OK,MessageBoxImage.Stop);
                        return;
                    }


                    #endregion                 

                    if (!ExisteContribuyente) {
                        db.Contribuyente.Add(Contribuyente);
                    }

                    db.SaveChanges();

                    Contribuyente_Consecutivos conse = db.Contribuyente_Consecutivos.FirstOrDefault(q => q.Id_Contribuyente == Contribuyente.Id_Contribuyente);

                    if(conse == null)
                    {
                        conse = new Contribuyente_Consecutivos() {
                            Consecutivo_Facturas = 1,
                            Consecutivo_NotasCredito = 1,
                            Consecutivo_Tiquete_Electrónico = 1,
                            Consecutivo_Confirmacion = 1,
                            Id_Contribuyente = Contribuyente.Id_Contribuyente
                        };
                        db.Contribuyente_Consecutivos.Add(conse);
                        db.SaveChanges();
                    }

                    MessageBox.Show("Perfil actualizado correctamente","Informacion",MessageBoxButton.OK,MessageBoxImage.Information);
                    try
                    {
                        RecursosSistema.Contribuyente = Contribuyente;
                        RecursosSistema.OnStartMain_Load();
                    }catch(Exception ex)
                    {
                        this.LogError(ex);
                        MessageBox.Show("Reinicie el sistema para continuar");
                    }
                }
            }
            catch (Exception ex)
            {
                this.LogError(ex);
                MessageBox.Show("Ocurrio al guardar el perfil del contribuyente", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
