using EpicurApp_API.Models;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System;
using EpicurAPP_Partage.Interfaces;
using Microsoft.Extensions.Configuration;

namespace EpicurApp_API.DAO
{
    public class PlatDAO : IPlatDAO
    {
        private readonly string _connexionString;
        private readonly ILogger<PlatDAO> _logger;

        public PlatDAO(IConfiguration configuration, ILogger<PlatDAO> logger)
        {
            _connexionString = configuration.GetConnectionString("DefaultConnection");
            if (string.IsNullOrWhiteSpace(_connexionString))
            {
                _logger = logger;
                _logger.LogWarning("Chaîne de connexion vide, utilisation du chemin par défaut /app/epicurapp.db");
                _connexionString = "Data Source=/app/epicurapp.db";
            }
            else
            {
                _logger = logger;
            }

            _logger.LogInformation("Chaîne de connexion Plats : {ConnectionString}", _connexionString);
        }

        public List<Plat> GetAll()
        {
            List<Plat> plats = new List<Plat>();
            const string query = "SELECT Id, Nom, Categorie, IngredientsPrincipaux FROM Plats ORDER BY Categorie, Nom;";

            try
            {
                using (SqliteConnection connexion = new SqliteConnection(_connexionString))
                {
                    connexion.Open();
                    _logger.LogInformation("Connexion ouverte vers {DbPath}", connexion.DataSource);

                    using (SqliteCommand cmd = new SqliteCommand(query, connexion))
                    using (SqliteDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            plats.Add(new Plat
                            {
                                Id = reader.GetInt32(0),
                                Nom = reader.GetString(1),
                                Categorie = reader.GetString(2),
                                IngredientsPrincipaux = reader.IsDBNull(3) ? string.Empty : reader.GetString(3)
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la lecture des plats depuis {ConnectionString}", _connexionString);
                throw;
            }

            return plats;
        }

        public Plat? GetById(int id)
        {
            Plat? plat = null;
            const string query = "SELECT Id, Nom, Categorie, IngredientsPrincipaux FROM Plats WHERE Id = @Id;";

            try
            {
                using (SqliteConnection connection = new SqliteConnection(_connexionString))
                {
                    connection.Open();
                    _logger.LogInformation("Recherche du plat {Id} dans {DbPath}", id, connection.DataSource);

                    using (SqliteCommand command = new SqliteCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Id", id);

                        using (SqliteDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                plat = new Plat
                                {
                                    Id = reader.GetInt32(0),
                                    Nom = reader.GetString(1),
                                    Categorie = reader.GetString(2),
                                    IngredientsPrincipaux = reader.IsDBNull(3) ? string.Empty : reader.GetString(3)
                                };
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération du plat {Id} depuis {ConnectionString}", id, _connexionString);
                throw;
            }

            return plat;
        }

        public void Add(Plat plat)
        {
           
            const string query = "INSERT INTO Plats (Nom, Categorie, IngredientsPrincipaux) VALUES (@Nom, @Categorie, @IngredientsPrincipaux);";

            try
            {
                using (SqliteConnection connection = new SqliteConnection(_connexionString))
                {
                    connection.Open();
                    using (SqliteCommand command = new SqliteCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Nom", plat.Nom);
                        command.Parameters.AddWithValue("@Categorie", plat.Categorie);
                        command.Parameters.AddWithValue("@IngredientsPrincipaux", plat.IngredientsPrincipaux ?? string.Empty);

                        command.ExecuteNonQuery();

                        command.CommandText = "SELECT last_insert_rowid();";
                        plat.Id = Convert.ToInt32(command.ExecuteScalar());
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de l'ajout du plat {Nom}", plat.Nom);
                throw;
            }
        }

       
        public void Update(Plat plat)
        {
            const string query = "UPDATE Plats SET Nom = @Nom, Categorie = @Categorie, IngredientsPrincipaux = @IngredientsPrincipaux WHERE Id = @Id;";

            try
            {
                using (SqliteConnection connection = new SqliteConnection(_connexionString))
                {
                    connection.Open();
                    using (SqliteCommand command = new SqliteCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Id", plat.Id);
                        command.Parameters.AddWithValue("@Nom", plat.Nom);
                        command.Parameters.AddWithValue("@Categorie", plat.Categorie);
                        command.Parameters.AddWithValue("@IngredientsPrincipaux", plat.IngredientsPrincipaux ?? string.Empty);

                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la mise à jour du plat {Id}", plat.Id);
                throw;
            }
        }

        public void Delete(int id)
        {
            const string query = "DELETE FROM Plats WHERE Id = @Id;";

            try
            {
                using (SqliteConnection connection = new SqliteConnection(_connexionString))
                {
                    connection.Open();
                    using (SqliteCommand command = new SqliteCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Id", id);
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la suppression du plat {Id}", id);
                throw;
            }
        }
    }
}