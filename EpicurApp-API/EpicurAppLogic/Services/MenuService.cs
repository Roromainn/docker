using EpicurAPP_Partage.Exceptions;
using EpicurAPP_Partage.Interfaces;
using EpicurApp_API.Models;
using System;
using Microsoft.Extensions.Logging;

namespace EpicurApp.Logic.Services
{
    public class MenuService : IMenuService
    {
        private IMenuDAO _menuRepository;
        private readonly ILogger<MenuService> _logger;

        public MenuService(IMenuDAO menuRepository, ILogger<MenuService> logger)
        {
            _menuRepository = menuRepository;
            _logger = logger;
        }

        public void AjouterMenu(Menu menu)
        {
            if (string.IsNullOrWhiteSpace(menu.Nom))
            {
                throw new InvalidFieldException("Le nom du menu est obligatoire.");
            }

            ValiderStatut(menu.Statut);

            try
            {
                _logger.LogInformation("Ajout d'un menu Statut={Statut} Nom={Nom}", menu.Statut, menu.Nom);
                _menuRepository.AjouterMenu(menu);
                _logger.LogInformation("Menu ajouté avec Id={Id}", menu.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de l'enregistrement du menu");
                throw new ApplicationException("Erreur lors de l'enregistrement du menu.", ex);
            }
        }

        public Menu? GetById(int id)
        {
            try
            {
                _logger.LogInformation("Récupération du menu Id={Id}", id);
                return _menuRepository.GetById(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération du menu {Id}", id);
                throw new ApplicationException("Erreur lors de la récupération du menu.", ex);
            }
        }

        public List<Menu> GetAll()
        {
            try
            {
                _logger.LogInformation("Récupération de tous les menus");
                return _menuRepository.GetAll();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération de tous les menus");
                throw new ApplicationException("Erreur lors de la récupération des menus.", ex);
            }
        }

        public Menu? GetDernierBrouillon()
        {
            try
            {
                _logger.LogInformation("Recherche du dernier menu brouillon");
                return _menuRepository.GetDernierBrouillon();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération du brouillon");
                throw new ApplicationException("Erreur lors de la récupération du brouillon de menu.", ex);
            }
        }

        public void MettreAJourMenu(Menu menu)
        {
            if (menu == null)
            {
                throw new InvalidFieldException("Les informations du menu sont obligatoires.");
            }

            if (menu.Id <= 0)
            {
                throw new InvalidFieldException("L'identifiant du menu est obligatoire pour la mise à jour.");
            }

            if (string.IsNullOrWhiteSpace(menu.Nom))
            {
                throw new InvalidFieldException("Le nom du menu est obligatoire.");
            }

            ValiderStatut(menu.Statut);

            try
            {
                _logger.LogInformation("Mise à jour du menu Id={Id} Statut={Statut}", menu.Id, menu.Statut);
                _menuRepository.MettreAJourMenu(menu);
                _logger.LogInformation("Menu Id={Id} mis à jour", menu.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la mise à jour du menu {Id}", menu.Id);
                throw new ApplicationException("Erreur lors de la mise à jour du menu.", ex);
            }
        }

        public void AjouterPlatsAuMenu(int menuId, List<int> platIds)
        {
            if (platIds == null || platIds.Count == 0)
            {
                throw new InvalidFieldException("Au moins un plat doit être sélectionné pour ajouter au menu.");
            }

            try
            {
                _logger.LogInformation("Ajout de {Count} plats au menu {MenuId}", platIds.Count, menuId);
                _menuRepository.AjouterPlatsAuMenu(menuId, platIds);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de l'ajout des plats au menu {MenuId}", menuId);
                throw new ApplicationException("Erreur lors de l'ajout des plats au menu.", ex);
            }
        }

        private static void ValiderStatut(string statut)
        {
            if (string.IsNullOrWhiteSpace(statut))
            {
                throw new InvalidFieldException("Le statut du menu est obligatoire.");
            }

            if (!string.Equals(statut, "Brouillon", StringComparison.OrdinalIgnoreCase) &&
                !string.Equals(statut, "Validé", StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidFieldException("Le statut du menu doit être 'Brouillon' ou 'Validé'.");
            }
        }
    }
}

