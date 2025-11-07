using EpicurApp_API.DAO;
using EpicurApp_API.Models;
using EpicurAPP_Partage.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace EpicurApp_API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PlatsController : Controller
    {
        private readonly IPlatDAO _platDAO;
        private readonly ILogger<PlatsController> _logger;

        public PlatsController(IPlatDAO platDAO, ILogger<PlatsController> logger)
        {
            _platDAO = platDAO;
            _logger = logger;
        }

        // GET: plats
        [HttpGet]
        public ActionResult<IEnumerable<Plat>> GetAllPlats()
        {
            try
            {
                _logger.LogInformation("Requête GET /Plats");
                List<Plat> plats = _platDAO.GetAll();


                if (plats == null || !plats.Any())
                {
                    _logger.LogWarning("Aucun plat trouvé en base");
                    return Ok(new List<Plat>()); 
                }

                _logger.LogInformation("{Count} plats retournés", plats.Count);
                return Ok(plats);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Erreur lors de la récupération de la liste des plats");
                return StatusCode(500, $"Erreur interne du serveur : {e.Message}");
            }
        }

        // GET: plats
        [HttpGet("{id}")]
        public ActionResult<Plat> GetPlatById(int id)
        {
            try
            {
                _logger.LogInformation("Requête GET /Plats/{Id}", id);
                Plat plat = _platDAO.GetById(id);
                if (plat == null)
                {
                    _logger.LogWarning("Plat {Id} introuvable", id);
                    return NotFound($"Aucun plat trouvé avec l'ID {id}");
                }
                _logger.LogInformation("Plat {Id} trouvé", id);
                return Ok(plat);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Erreur lors de la récupération du plat {Id}", id);
                return StatusCode(500, $"Erreur interne du serveur : {e.Message}");
            }
        }

        // GET: plats/categorie/{categorie}
        [HttpGet("categorie/{categorie}")]
        public ActionResult<IEnumerable<Plat>> GetPlatsByCategorie(string categorie)
        {
            try
            {
                _logger.LogInformation("Requête GET /Plats/categorie/{Categorie}", categorie);
                List<Plat> plats = _platDAO.GetAll()
                    .Where(p => p.Categorie.Equals(categorie, StringComparison.OrdinalIgnoreCase))
                    .ToList();

                _logger.LogInformation("{Count} plats dans la catégorie {Categorie}", plats.Count, categorie);
                return Ok(plats);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Erreur lors de la récupération des plats de la catégorie {Categorie}", categorie);
                return StatusCode(500, $"Erreur interne du serveur : {e.Message}");
            }
        }

        // POST: plats
        [HttpPost]
        public ActionResult<Plat> CreatePlat([FromBody] Plat plat)
        {
            if (plat == null || !ModelState.IsValid)
            {
                _logger.LogWarning("Requête POST /Plats avec modèle invalide");
                return BadRequest(ModelState);
            }

            try
            {
                _logger.LogInformation("Création d'un plat {Nom}", plat.Nom);
                _platDAO.Add(plat);
                return CreatedAtAction(nameof(GetPlatById), new { id = plat.Id }, plat);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Erreur lors de la création du plat {Nom}", plat.Nom);
                return StatusCode(500, $"Erreur lors de la création : {e.Message}");
            }
        }
    }
}