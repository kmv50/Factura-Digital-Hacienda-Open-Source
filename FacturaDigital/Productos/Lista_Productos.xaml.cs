using DataModel;
using DataModel.EF;
using FacturaDigital.Recursos;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace FacturaDigital.Productos
{
    /// <summary>
    /// Interaction logic for Lista_Productos.xaml
    /// </summary>
    public partial class Lista_Productos : Page, ILog
    {
        ObservableCollection<Producto> ProductosCollection;
        public Lista_Productos()
        {
            InitializeComponent();
            GetListaProductos();
        }

        private void GetListaProductos() {
            try
            {
                List<Producto> productos = null;                
                using (db_FacturaDigital db = new db_FacturaDigital())
                {
                    productos = db.Producto.ToList();
                }

                if (productos != null)
                    dgv_Productos.ItemsSource = ProductosCollection = new ObservableCollection<Producto>(productos);


            }
            catch(Exception ex)
            {
                this.LogError(ex);
                MessageBox.Show("Ocurrio un error al obtener la lista de productos","Error", MessageBoxButton.OK,MessageBoxImage.Error);
            }
        }

 

        private void EliminarProducto(object sender, RoutedEventArgs e)
        {
            try
            {
                Button btn = (Button)sender;
                using (db_FacturaDigital db = new db_FacturaDigital())
                {
                    int Id_Producto = (int)btn.CommandParameter;
                    db.Producto.Remove(db.Producto.First(q => q.Id_Producto == Id_Producto));
                    db.SaveChanges();

                    ProductosCollection.Remove(ProductosCollection.First(q => q.Id_Producto == Id_Producto));
                }
            }catch(Exception ex)
            {
                this.LogError(ex);
                MessageBox.Show("Ocurrio un error al eliminar el producto");
            }
        }

        private void AgregarNuevoProducto(object sender, RoutedEventArgs e)
        {
            RecursosSistema.MainConteiner.Content = new Productos();
        }
    }
}
