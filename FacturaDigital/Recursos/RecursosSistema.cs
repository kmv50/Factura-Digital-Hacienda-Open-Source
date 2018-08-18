using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using DataModel.EF;

namespace FacturaDigital.Recursos
{
    public static class  RecursosSistema
    {
        public delegate void Contribuyente_Load();
        public static Contribuyente_Load OnContribuyente_Load;
        public static Frame MainConteiner { set; get; }
        public static DataModel.EF.Contribuyente Contribuyente { get; internal set; }        
        public static void LogError(Exception ex)
        {

        }
    }
}
