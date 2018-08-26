using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace FacturaDigital
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void OnApplicationExit(object sender, ExitEventArgs e)
        {
            Recursos.RecursosSistema.DetenerServicioConsulta();
        }
    }
}
