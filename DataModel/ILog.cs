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
                    
                }
            }
            catch { }
        }
    }
}
