namespace ApiModels.Migrations
{
    using MySql.Data.Entity;
    using System;
    using System.Data.Common;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Data.Entity.Migrations.History;
    using System.Data.Entity.Migrations.Model;
    using System.Data.Entity.Migrations.Sql;
    using System.Linq;
    public sealed class Configuration : DbMigrationsConfiguration<DataModel.EF.db_FacturaDigital>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
            AutomaticMigrationDataLossAllowed = true;
            SetSqlGenerator("MySql.Data.MySqlClient", new MySqlMigrationSqlGeneratorNS());

            SetHistoryContextFactory("MySql.Data.MySqlClient", (conn, schema) => new MySqlHistoryContext(conn, schema));
        }
        protected override void Seed(DataModel.EF.db_FacturaDigital context)
        {

        }
    }
    public class MySqlHistoryContext : HistoryContext
    {
        public MySqlHistoryContext(DbConnection connection, string defaultSchema) : base(connection, defaultSchema)
        {
        }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<HistoryRow>().Property(h => h.MigrationId)
                .HasMaxLength(100)
                .IsRequired();
            modelBuilder.Entity<HistoryRow>().Property(h => h.ContextKey)
                .HasMaxLength(200)
                .IsRequired();
        }
    }

    public class MySqlMigrationSqlGeneratorNS : MySqlMigrationSqlGenerator
    {
        protected override MigrationStatement Generate(RenameTableOperation op)
        {
            RenameTableOperation o = new RenameTableOperation(op.Name.Replace("dbo.", ""), op.NewName.Replace("dbo.", ""), op.AnonymousArguments);
            return base.Generate(op);
        }
        protected override MigrationStatement Generate(DropPrimaryKeyOperation op)
        {
            op.Name = op.Name.Replace("dbo.", "");
            op.Table = op.Table.Replace("dbo.", "");
            return base.Generate(op);
        }
        protected override MigrationStatement Generate(AddPrimaryKeyOperation op)
        {
            op.Name = op.Name.Replace("dbo.", "");
            op.Table = op.Table.Replace("dbo.", "");
            return base.Generate(op);
        }

        protected override MigrationStatement Generate(AddForeignKeyOperation op)
        {
            op.Name = op.Name.Replace("dbo.", "");
            op.PrincipalTable = op.PrincipalTable.Replace("dbo.", "");
            op.DependentTable = op.DependentTable.Replace("dbo.", "");
            return base.Generate(op);
        }


        /*protected override MigrationStatement Generate(DropTableOperation op)
        {

            return base.Generate(op);
        }

        protected override MigrationStatement Generate(CreateTableOperation op)
        {
            return base.Generate(op);
        }
        */

        protected override MigrationStatement Generate(DropIndexOperation op)
        {
            op.Name = op.Name.Replace("dbo.", "");
            op.Table = op.Table.Replace("dbo.", "");
            return base.Generate(op);
        }
        protected override MigrationStatement Generate(CreateIndexOperation op)
        {
            op.Name = op.Name.Replace("dbo.", "");
            op.Table = op.Table.Replace("dbo.", "");
            return base.Generate(op);
        }
        protected override MigrationStatement Generate(DropForeignKeyOperation op)
        {
            op.Name = op.Name.Replace("dbo.", "");
            op.PrincipalTable = op.PrincipalTable.Replace("dbo.", "");
            op.DependentTable = op.DependentTable.Replace("dbo.", "");
            return base.Generate(op);
        }
    }
}
