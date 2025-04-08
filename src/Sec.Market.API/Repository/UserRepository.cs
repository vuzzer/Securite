using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using Sec.Market.API.Data;
using Sec.Market.API.Entites;
using Sec.Market.API.Interfaces;
using System.Data;
using System.Data.Common;

namespace Sec.Market.API.Repository
{
    public class UserRepository : IUserRepository
    {
        public readonly MarketContext _context;
        private readonly IConfiguration _configuration;
        public UserRepository(MarketContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }
       
        public Task DeleteUser(User user)
        {
            _context.Users.Remove(user);
            return _context.SaveChangesAsync();
        }

        public async Task<User?> GetUserByEmailAndPwd(string email, string pwd)
        {
            string? connectionString = _configuration.GetConnectionString("DefaultConnection");
            using (var connexion = new SqlConnection(connectionString))
            {
                await connexion.OpenAsync();
                string requete = "SELECT * FROM Users WHERE Email = @email AND Password = @pwd";

                SqlDataAdapter myCommand = new SqlDataAdapter(requete, connexion);
                SqlParameter parm1 = myCommand.SelectCommand.Parameters.Add("@email",
                    SqlDbType.VarChar);
                SqlParameter parm2 = myCommand.SelectCommand.Parameters.Add("@pwd",
                    SqlDbType.VarChar);
                parm1.Value = email;
                parm2.Value = pwd;

                var dataTable = new DataTable();

                // Remplir le DataTable avec les données récupérées
                myCommand.Fill(dataTable);

                if (dataTable.Rows.Count > 0)
                {
                    var row = dataTable.Rows[0];

                    // Créer un objet User et y mapper les valeurs récupérées
                    var user = new User
                    {
                        Id = Convert.ToInt32(row["Id"]),
                        Nom = row["Nom"].ToString(),
                        Email = row["Email"].ToString(),
                        Password = row["Password"].ToString(),
                        Role = row["Role"].ToString(),
                        Token = row["Token"].ToString(),
                        DateCreation = (DateTime)row["DateCreation"]
                    };

                    return user;
                }
                else
                {
                    return null;
                }
            }
        }

        public async Task<User?> GetUserById(int userId)
        {
            string? connectionString = _configuration.GetConnectionString("DefaultConnection");
            using (var connexion = new SqlConnection(connectionString))
            {
                await connexion.OpenAsync();
                string requete = "SELECT * FROM Users WHERE Id = @userId";

                SqlDataAdapter myCommand = new SqlDataAdapter(requete, connexion);
                SqlParameter parm = myCommand.SelectCommand.Parameters.Add("@userId",
                    SqlDbType.VarChar);
                parm.Value = userId;

                var dataTable = new DataTable();

                // Remplir le DataTable avec les données récupérées
                myCommand.Fill(dataTable);

                if (dataTable.Rows.Count > 0)
                {
                    var row = dataTable.Rows[0];

                    // Créer un objet User et y mapper les valeurs récupérées
                    var user = new User
                    {
                        Id = Convert.ToInt32(row["Id"]),
                        Nom = row["Nom"].ToString(),
                        Email = row["Email"].ToString(),
                        Password = row["Password"].ToString(),
                        Role = row["Role"].ToString(),
                        Token = row["Token"].ToString(),
                        DateCreation = (DateTime)row["DateCreation"]
                    };

                    return user;
                }
                else
                {
                    return null;
                }
            }
        }

        public Task<List<User>> GetUsers()
        {
            return _context.Users.ToListAsync();
        }

        public Task InsertUser(User user)
        {
            _context.Users.Add(user);
            return _context.SaveChangesAsync();
        }

        public Task UpdateUser(User user)
        {
            _context.Users.Update(user);
            return _context.SaveChangesAsync();
        }
    }
}
