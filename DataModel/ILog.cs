using DataModel.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModel
{
    public interface ILog
    {
    }

    public static class IlogExtencion
    {
        public static void LogError(this ILog p, Exception ex)
        {
            try
            {
                using (db_FacturaDigital db = new db_FacturaDigital())
                {
                    string InnerMsn = null , InnerStackTrace = null;

                    if(ex.InnerException != null)
                    {
                        InnerMsn = ex.InnerException.Message;
                        InnerStackTrace = ex.StackTrace;
                    }


                    db.Errores_Sistema.Add(new Errores_Sistema() {
                        Fecha = DateTime.Now,
                        Mensaje = ex.Message,
                        Stacktrace = ex.StackTrace,
                        Inner_Mensaje = InnerMsn,
                        Inner_Stacktrace = InnerStackTrace
                    });
                    db.SaveChanges();
                }
            }
            catch { }
        }
    }
}
