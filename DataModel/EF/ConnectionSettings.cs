using DataModel.Properties;

namespace DataModel.EF
{
    public class ConnectionSettings
    {
        public string Servidor { set; get; }
        public string Usuario { set; get; }
        public string Contrasena { set; get; }
        public string Database { set; get; } //FacturaDigital
        private static string ConnectionString { set; get; }

        public ConnectionSettings LoadSettings()
        {
            try
            {
                Database = DbSettings.Default.Database;
                Usuario = DbSettings.Default.Usuario;
                Contrasena = DbSettings.Default.Contrasena;
                Servidor = DbSettings.Default.Servidor;
                return this;
            }
            catch
            {
                return null;
            }
        }

        public void SaveSettings(ConnectionSettings NewSettings)
        {
            DbSettings.Default.Database = NewSettings.Database;
            DbSettings.Default.Usuario = NewSettings.Usuario;
            DbSettings.Default.Contrasena = NewSettings.Contrasena;
            DbSettings.Default.Servidor = NewSettings.Servidor;
            DbSettings.Default.Save();
            ConnectionString = null;
        }

        public string GetConnectionString()
        {
            try
            {
                if (string.IsNullOrEmpty(ConnectionSettings.ConnectionString))
                {
                    LoadSettings();
                    ConnectionSettings.ConnectionString = "server=" + Servidor + ";user id=" + Usuario + ";password=" + Contrasena + ";persistsecurityinfo=True;database=" + Database;
                }

                return ConnectionSettings.ConnectionString;
            }
            catch
            {
                return null;
            }
        }
    }
}
