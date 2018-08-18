using DataModel.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModel.UbicacionesData
{
    public class UbicacionesType
    {
        public int Id { set; get; }
        public string Nombre { set; get; }
    }

    public class UbicacionesData
    {
        private db_FacturaDigital db;

        public List<UbicacionesType> GetProvincias()
        {
            try
            {
                db = new db_FacturaDigital();
                List<UbicacionesType> v = (from q in db.Ubicaciones
                                           select new UbicacionesType()
                                           {
                                               Id = q.Id_Provincia,
                                               Nombre = q.Provincia
                                           }).Distinct().ToList();
                return v;
            }
            catch (Exception e)
            {
                return null;
            }
            finally
            {
                db.Dispose();
            }
        }


        public List<UbicacionesType> GetCantones(int Id_Provincia)
        {
            try
            {
                db = new db_FacturaDigital();
                return (from q in db.Ubicaciones
                        where q.Id_Provincia == Id_Provincia
                        select new UbicacionesType()
                        {
                            Id = q.Id_Canton,
                            Nombre = q.Canton
                        }).Distinct().ToList();
            }
            catch (Exception e)
            {
                return null;
            }
            finally
            {
                db.Dispose();
            }
        }


        public List<UbicacionesType> GetDistritos(int Id_Provincia, int Id_Canton)
        {
            try
            {
                db = new db_FacturaDigital();
                return (from q in db.Ubicaciones
                        where q.Id_Provincia == Id_Provincia && q.Id_Canton == Id_Canton
                        select new UbicacionesType()
                        {
                            Id = q.Id_Distrito,
                            Nombre = q.Distrito
                        }).Distinct().ToList();
            }
            catch (Exception e)
            {
                return null;
            }
            finally
            {
                db.Dispose();
            }
        }

        public List<UbicacionesType> GetBarrios(int Id_Provincia, int Id_Canton, int Id_Distrito)
        {
            try
            {
                db = new db_FacturaDigital();
                return (from q in db.Ubicaciones
                        where q.Id_Provincia == Id_Provincia && q.Id_Canton == Id_Canton && q.Id_Distrito == Id_Distrito
                        select new UbicacionesType()
                        {
                            Id = q.Id_Barrio,
                            Nombre = q.Barrio
                        }).Distinct().ToList();
            }
            catch (Exception e)
            {
                return null;
            }
            finally
            {
                db.Dispose();
            }
        }


    }
}
