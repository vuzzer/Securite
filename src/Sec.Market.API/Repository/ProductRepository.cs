
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Sec.Market.API.Data;
using Sec.Market.API.Entites;
using Sec.Market.API.Interfaces;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Sec.Market.API.Repository
{
    public class ProductRepository: IProductRepository
    {
        public readonly MarketContext _context;
        private readonly IConfiguration _configuration;

        public ProductRepository(MarketContext context , IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }


        public ValueTask<Product?> GetProductById(int productId)
        {
            return _context.Products.FindAsync(productId);
        }
        
        public Task<List<Product>> GetProducts()
        {
            return _context.Products.ToListAsync();
        }

        public Task InsertProduct(Product product)
        {
            _context.Products.Add(product);
            return _context.SaveChangesAsync();
        }

        public async Task<List<Product>> SearchProducts(string filter)
        {
            string? connectionString = _configuration.GetConnectionString("DefaultConnection");
            using (var connexion = new SqlConnection(connectionString))
            {
                await connexion.OpenAsync();
                string requete = "SELECT * FROM Products WHERE Name LIKE @filter OR Description LIKE @filter";

                SqlDataAdapter myCommand = new SqlDataAdapter(requete, connexion);
                SqlParameter parm = myCommand.SelectCommand.Parameters.Add("@filter",
                    SqlDbType.VarChar);
                parm.Value = "%" + filter + "%";

                var dataTable = new DataTable();

                // Remplir le DataTable avec les données récupérées
                myCommand.Fill(dataTable);

                if (dataTable.Rows.Count > 0)
                {

                    // Créer une liste Product qui contiendra les produits récupérer
                    List<Product> products = new List<Product>();
                    foreach(DataRow row in dataTable.Rows)
                    {
                        Product product = new Product
                        {
                            Id = Convert.ToInt32(row["Id"]),
                            Name = row["Name"].ToString(),
                            Description = row["Description"].ToString(),
                            Image = row["Image"].ToString(),
                            Price = Convert.ToInt32(row["Price"]),
                            DateCreation = (DateTime)row["DateCreation"]
                        };

                        products.Add(product);
                    }
                    

                    return products;
                }
                else
                {
                    return null;
                }
            }
        }

        public Task UpdateProduct(Product product)
        {
            _context.Entry(product).State = EntityState.Modified;
            return _context.SaveChangesAsync();
        }

        public Task DeleteProduct(Product product)
        {
            _context.Products.Remove(product);
            return _context.SaveChangesAsync();
        }
    }
}
