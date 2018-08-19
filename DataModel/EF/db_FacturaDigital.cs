namespace DataModel.EF
{
    using MySql.Data.Entity;
    using System;
    using System.Data.Entity;
    using System.Linq;

    [DbConfigurationType(typeof(MySqlEFConfiguration))]
    public class db_FacturaDigital : DbContext
    {
        // Your context has been configured to use a 'db_FacturaDigital' connection string from your application's 
        // configuration file (App.config or Web.config). By default, this connection string targets the 
        // 'DataModel.EF.db_FacturaDigital' database on your LocalDb instance. 
        // 
        // If you wish to target a different database and/or database provider, modify the 'db_FacturaDigital' 
        // connection string in the application configuration file.
        public db_FacturaDigital()
            : base("name=db_FacturaDigital")
            //:base(new ConnectionSettings().GetConnectionString())
        {
            this.Configuration.ProxyCreationEnabled = false;
            Database.SetInitializer<db_FacturaDigital>(null);
        }

        // Add a DbSet for each entity type that you want to include in your model. For more information 
        // on configuring and using a Code First model, see http://go.microsoft.com/fwlink/?LinkId=390109.

        public virtual DbSet<Contribuyente> Contribuyente { get; set; }
        public virtual DbSet<Factura> Factura { get; set; }
        public virtual DbSet<Factura_Detalle> Factura_Detalle { get; set; }
        public virtual DbSet<Factura_Detalle_Impuesto> Factura_Detalle_Impuesto { get; set; }
        public virtual DbSet<Producto> Producto { get; set; }
        public virtual DbSet<Producto_Impuesto> Producto_Impuesto { get; set; }
        public virtual DbSet<Cliente> Cliente { get; set; }
        public virtual DbSet<SMTP> SMTP { get; set; }
        public virtual DbSet<Ubicacion> Ubicaciones { get; set; }
        public virtual DbSet<Contribuyente_Consecutivos> Contribuyente_Consecutivos { set; get; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Producto>()
             .HasMany(e => e.Producto_Impuesto)
             .WithRequired(e => e.Producto)
             .WillCascadeOnDelete(true);


            modelBuilder.Entity<Factura>()
             .HasMany(e => e.Factura_Detalle)
             .WithRequired(e => e.Factura)
             .WillCascadeOnDelete(true);

            modelBuilder.Entity<Factura_Detalle>()
             .HasMany(e => e.Factura_Detalle_Impuesto)
             .WithRequired(e => e.Factura_Detalle)
             .WillCascadeOnDelete(true);
        }
    }

   
       

    //public class MyEntity
    //{
    //    public int Id { get; set; }
    //    public string Name { get; set; }
    //}
}