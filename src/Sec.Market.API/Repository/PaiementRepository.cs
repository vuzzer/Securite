using Sec.Market.API.Interfaces;
using Sec.Market.API.Entites;
using Sec.Market.API.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using System.Data;

namespace Sec.Market.API.Repository
{
    public class PaiementRepository : IPaiementRepository
    {
        private readonly MarketContext _context;
        private readonly IConfiguration _configuration;
        public PaiementRepository(MarketContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

       

        public async Task InsertCard(Card card)
        {
            _context.Cards.Add(card);
            await _context.SaveChangesAsync();
        }

        public async Task<Card?> GetCardByNumberAndUser(string cardNumber, string userId)
        {
            string? connectionString = _configuration.GetConnectionString("DefaultConnection");
            using (var connexion = new SqlConnection(connectionString))
            {
                await connexion.OpenAsync();
                string requete = "SELECT * FROM Cards WHERE Number = @cardNumber AND UserId = @userId";

                SqlDataAdapter myCommand = new SqlDataAdapter(requete, connexion);
                SqlParameter parm1 = myCommand.SelectCommand.Parameters.Add("@cardNumber",
                    SqlDbType.VarChar);
                SqlParameter parm2 = myCommand.SelectCommand.Parameters.Add("@userId",
                    SqlDbType.VarChar);
                parm1.Value = cardNumber;
                parm2.Value = userId;

                var dataTable = new DataTable();

                // Remplir le DataTable avec les données récupérées
                myCommand.Fill(dataTable);

                if (dataTable.Rows.Count > 0)
                {
                    var row = dataTable.Rows[0];

                    // Créer un objet Card et y mapper les valeurs récupérées
                    var card = new Card
                    {
                        Id = Convert.ToInt32(row["Id"]),
                        Number = row["Number"].ToString(),
                        SecurityCode = row["SecurityCode"].ToString(),
                        Type = row["Type"].ToString(),
                        Owner = row["Owner"].ToString(),
                        ExpirationDate = row["ExpirationDate"].ToString(),
                        UserId = Convert.ToInt32(row["UserId"])
                    };

                    return card;
                }
                else
                {
                    return null;
                }
            }
        }
    }
   
}
