using ExpressVoitures.Data;
using ExpressVoitures.Models;
using ExpressVoitures.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace ExpressVoitures.Tests.ExpressVoitures.TestsUnitaires
{
    public class VoitureServiceTests
    {
        private ApplicationDbContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            return new ApplicationDbContext(options);
        }

        private readonly Voiture voiture1;
        private readonly Voiture voiture2;
        private readonly Voiture voiture3;

        private readonly Marque marque1;
        private readonly Marque marque2;

        private readonly Modele modele1;
        private readonly Modele modele2;

        private readonly Finition finition1;
        private readonly Finition finition2;

        private readonly Vente vente3;

        private readonly Reparation reparation1;
        private readonly Reparation reparation3;

        public VoitureServiceTests()
        {
            voiture1 = new Voiture
            {
                Id = 1,
                CodeVin = "00000000000000001",
                Annee = 2021,
                DateAchat = new DateTime(2025, 1, 1),
                PrixAchat = 1000,
                VoitureReparee = true,
                AnnoncePubliee = true,
                VoitureVendue = false,
                IdMarque = 1,
                IdModele = 1,
                IdFinition = 1
            };

            voiture2 = new Voiture
            {
                Id = 2,
                CodeVin = "00000000000000002",
                Annee = 2022,
                DateAchat = new DateTime(2025, 2, 1),
                PrixAchat = 2000,
                VoitureReparee = false,
                AnnoncePubliee = true,
                VoitureVendue = false,
                IdMarque = 2,
                IdModele = 2,
                IdFinition = 2
            };

            voiture3 = new Voiture
            {
                Id = 3,
                CodeVin = "00000000000000003",
                Annee = 2023,
                DateAchat = new DateTime(2025, 3, 1),
                PrixAchat = 3000,
                VoitureReparee = true,
                AnnoncePubliee = false,
                VoitureVendue = true,
                IdMarque = 1,
                IdModele = 1,
                IdFinition = 1
            };

            marque1 = new Marque
            {
                Id = 1,
                Nom = "Marque1"
            };

            marque2 = new Marque
            {
                Id = 2,
                Nom = "Marque2"
            };

            modele1 = new Modele
            {
                Id = 1,
                Nom = "Modele1",
                IdMarque = 1
            };

            modele2 = new Modele
            {
                Id = 2,
                Nom = "Modele2",
                IdMarque = 2
            };

            finition1 = new Finition
            {
                Id = 1,
                Nom = "Finition1",
                IdModele = 1
            };

            finition2 = new Finition
            {
                Id = 2,
                Nom = "Finition2",
                IdModele = 2
            };

            vente3 = new Vente
            {
                Id = 3,
                Date = new DateTime(2025, 3, 10),
                IdVoiture = 3
            };

            reparation1 = new Reparation
            {
                Id = 1,
                ReparationEffectuee = "Reparation1",
                Cout = 1000,
                IdVoiture = 1
            };

            reparation3 = new Reparation
            {
                Id = 3,
                ReparationEffectuee = "Reparation3",
                Cout = 3000,
                IdVoiture = 3
            };
        }

        // Tests de création

        [Fact]
        public async Task CreerAsync_ContextContient1Element()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var service = new VoitureService(context);

            // Act
            await service.CreerAsync(voiture1);

            // Assert
            Assert.Equal(1, context.Voitures.Count());
        }


        // Tests d'existance

        [Fact]
        public async Task ExisteAsync_RetourneVrai()
        {
            // Arrange
            var context = GetInMemoryDbContext();

            context.Voitures.Add(voiture1);
            await context.SaveChangesAsync();

            var service = new VoitureService(context);

            // Act
            var resultat = await service.ExisteAsync(1);

            // Assert
            Assert.True(resultat);
        }

        [Fact]
        public async Task ExisteAsync_RetourneFaux()
        {
            // Arrange
            var context = GetInMemoryDbContext();

            context.Voitures.Add(voiture1);
            await context.SaveChangesAsync();

            var service = new VoitureService(context);

            // Act
            var resultat = await service.ExisteAsync(2);

            // Assert
            Assert.False(resultat);
        }

        [Fact]
        public async Task VinExisteAsync_RetourneVrai()
        {
            // Arrange
            var context = GetInMemoryDbContext();

            context.Voitures.Add(voiture1);
            await context.SaveChangesAsync();

            var service = new VoitureService(context);

            // Act
            var resultat = await service.VinExisteAsync("00000000000000001");

            // Assert
            Assert.True(resultat);
        }

        [Fact]
        public async Task VinExisteAsync_RetourneFaux()
        {
            // Arrange
            var context = GetInMemoryDbContext();

            context.Voitures.Add(voiture1);
            await context.SaveChangesAsync();

            var service = new VoitureService(context);

            // Act
            var resultat = await service.VinExisteAsync("00000000000000004");

            // Assert
            Assert.False(resultat);
        }

        [Fact]
        public async Task VinExisteAsync_ExclureVoitureActuelle_RetourneFaux()
        {
            // Arrange
            var context = GetInMemoryDbContext();

            context.Voitures.Add(voiture1);
            await context.SaveChangesAsync();

            var service = new VoitureService(context);

            // Act
            var resultat = await service.VinExisteAsync("00000000000000001", 1);

            // Assert
            Assert.False(resultat);
        }


        // Tests des éléments utilisés

        [Fact]
        public async Task MarqueUtiliseeAsync_RetourneVrai()
        {
            // Arrange
            var context = GetInMemoryDbContext();

            context.Voitures.Add(voiture1);
            await context.SaveChangesAsync();

            var service = new VoitureService(context);

            // Act
            var resultat = await service.MarqueUtiliseeAsync(1);

            // Assert
            Assert.True(resultat);
        }

        [Fact]
        public async Task MarqueUtiliseeAsync_RetourneFaux()
        {
            // Arrange
            var context = GetInMemoryDbContext();

            context.Voitures.Add(voiture1);
            await context.SaveChangesAsync();

            var service = new VoitureService(context);

            // Act
            var resultat = await service.MarqueUtiliseeAsync(4);

            // Assert
            Assert.False(resultat);
        }

        [Fact]
        public async Task ModeleUtiliseAsync_RetourneVrai()
        {
            // Arrange
            var context = GetInMemoryDbContext();

            context.Voitures.Add(voiture1);
            await context.SaveChangesAsync();

            var service = new VoitureService(context);

            // Act
            var resultat = await service.ModeleUtiliseAsync(1);

            // Assert
            Assert.True(resultat);
        }

        [Fact]
        public async Task ModeleUtiliseAsync_RetourneFaux()
        {
            // Arrange
            var context = GetInMemoryDbContext();

            context.Voitures.Add(voiture1);
            await context.SaveChangesAsync();

            var service = new VoitureService(context);

            // Act
            var resultat = await service.ModeleUtiliseAsync(4);

            // Assert
            Assert.False(resultat);
        }

        [Fact]
        public async Task FinitionUtiliseeAsync_RetourneVrai()
        {
            // Arrange
            var context = GetInMemoryDbContext();

            context.Voitures.Add(voiture1);
            await context.SaveChangesAsync();

            var service = new VoitureService(context);

            // Act
            var resultat = await service.FinitionUtiliseeAsync(1);

            // Assert
            Assert.True(resultat);
        }

        [Fact]
        public async Task FinitionUtiliseeAsync_RetourneFaux()
        {
            // Arrange
            var context = GetInMemoryDbContext();

            context.Voitures.Add(voiture1);
            await context.SaveChangesAsync();

            var service = new VoitureService(context);

            // Act
            var resultat = await service.FinitionUtiliseeAsync(4);

            // Assert
            Assert.False(resultat);
        }


        // Tests de mise à jour

        [Fact]
        public async Task MettreAJourAsync_Voiture1SeNommeVoiture4()
        {
            // Arrange
            var context = GetInMemoryDbContext();

            context.Voitures.Add(voiture1);
            context.Marques.Add(marque1);
            context.Modeles.Add(modele1);
            context.Finitions.Add(finition1);
            context.Reparations.Add(reparation1);
            await context.SaveChangesAsync();

            var service = new VoitureService(context);

            voiture1.CodeVin = "00000000000000004";

            // Act
            await service.MettreAJourAsync(voiture1);

            // Assert
            var resultat = await context.Voitures.FindAsync(1);
            Assert.Equal(1, resultat.Id);
            Assert.Equal("00000000000000004", resultat.CodeVin);
        }


        // Tests d'obtention

        [Fact]
        public async Task ObtenirParIdAsync_RetourneLaVoitureCorrespondantAId()
        {
            // Arrange
            var context = GetInMemoryDbContext();

            context.Voitures.Add(voiture1);
            context.Voitures.Add(voiture2);
            AjouterRelations2Voitures(context);
            await context.SaveChangesAsync();

            var service = new VoitureService(context);

            // Act
            var resultat = await service.ObtenirParIdAsync(1);

            // Assert
            Assert.NotNull(resultat);
            Assert.Equal(1, resultat.Id);
            Assert.Equal("00000000000000001", resultat.CodeVin);
        }

        [Fact]
        public async Task ObtenirParIdAsync_RetourneAucuneVoiture()
        {
            // Arrange
            var context = GetInMemoryDbContext();

            context.Voitures.Add(voiture1);
            context.Voitures.Add(voiture2);
            AjouterRelations2Voitures(context);
            await context.SaveChangesAsync();

            var service = new VoitureService(context);

            // Act
            var resultat = await service.ObtenirParIdAsync(4);

            // Assert
            Assert.Null(resultat);
        }

        [Fact]
        public async Task ObtenirParMarqueAsync_RetourneLesVoituresLieesALaMarque()
        {
            // Arrange
            var context = GetInMemoryDbContext();

            context.Voitures.Add(voiture1);
            context.Voitures.Add(voiture2);
            context.Voitures.Add(voiture3);
            AjouterRelations3Voitures(context);
            await context.SaveChangesAsync();

            var service = new VoitureService(context);

            // Act
            var resultat = await service.ObtenirParMarqueAsync(1);

            // Assert
            Assert.Equal(2, resultat.Count);
            Assert.Contains(resultat, f => f.CodeVin == "00000000000000001");
            Assert.Contains(resultat, f => f.CodeVin == "00000000000000003");
        }

        [Fact]
        public async Task ObtenirParMarqueAsync_RetourneAucuneVoiture()
        {
            // Arrange
            var context = GetInMemoryDbContext();

            context.Voitures.Add(voiture1);
            context.Voitures.Add(voiture2);
            AjouterRelations2Voitures(context);
            await context.SaveChangesAsync();

            var service = new VoitureService(context);

            // Act
            var resultat = await service.ObtenirParMarqueAsync(4);

            // Assert
            Assert.Empty(resultat);
        }

        [Fact]
        public async Task ObtenirParModeleAsync_RetourneLesVoituresLieesAuModele()
        {
            // Arrange
            var context = GetInMemoryDbContext();

            context.Voitures.Add(voiture1);
            context.Voitures.Add(voiture2);
            context.Voitures.Add(voiture3);
            AjouterRelations3Voitures(context);
            await context.SaveChangesAsync();

            var service = new VoitureService(context);

            // Act
            var resultat = await service.ObtenirParModeleAsync(1);

            // Assert
            Assert.Equal(2, resultat.Count);
            Assert.Contains(resultat, f => f.CodeVin == "00000000000000001");
            Assert.Contains(resultat, f => f.CodeVin == "00000000000000003");
        }

        [Fact]
        public async Task ObtenirParModeleAsync_RetourneAucuneVoiture()
        {
            // Arrange
            var context = GetInMemoryDbContext();

            context.Voitures.Add(voiture1);
            context.Voitures.Add(voiture2);
            AjouterRelations2Voitures(context);
            await context.SaveChangesAsync();

            var service = new VoitureService(context);

            // Act
            var resultat = await service.ObtenirParModeleAsync(4);

            // Assert
            Assert.Empty(resultat);
        }

        [Fact]
        public async Task ObtenirParFinitionAsync__RetourneLesVoituresLieesALaFinition()
        {
            // Arrange
            var context = GetInMemoryDbContext();

            context.Voitures.Add(voiture1);
            context.Voitures.Add(voiture2);
            context.Voitures.Add(voiture3);
            AjouterRelations3Voitures(context);
            await context.SaveChangesAsync();

            var service = new VoitureService(context);

            // Act
            var resultat = await service.ObtenirParFinitionAsync(1);

            // Assert
            Assert.Equal(2, resultat.Count);
            Assert.Contains(resultat, f => f.CodeVin == "00000000000000001");
            Assert.Contains(resultat, f => f.CodeVin == "00000000000000003");
        }

        [Fact]
        public async Task ObtenirParFinitionAsync_RetourneAucuneVoiture()
        {
            // Arrange
            var context = GetInMemoryDbContext();

            context.Voitures.Add(voiture1);
            context.Voitures.Add(voiture2);
            AjouterRelations2Voitures(context);
            await context.SaveChangesAsync();

            var service = new VoitureService(context);

            // Act
            var resultat = await service.ObtenirParFinitionAsync(4);

            // Assert
            Assert.Empty(resultat);
        }

        [Fact]
        public async Task ObtenirToutesAsync_RetourneToutesLesVoitures()
        {
            // Arrange
            var context = GetInMemoryDbContext();

            context.Voitures.Add(voiture1);
            context.Voitures.Add(voiture2);
            AjouterRelations2Voitures(context);
            await context.SaveChangesAsync();

            var service = new VoitureService(context);

            // Act
            var resultat = await service.ObtenirToutesAsync();

            // Assert
            Assert.Equal(2, resultat.Count);
            Assert.Contains(resultat, f => f.CodeVin == "00000000000000001");
            Assert.Contains(resultat, f => f.CodeVin == "00000000000000002");
        }

        [Fact]
        public async Task ObtenirToutesAsync_RetourneAucuneVoiture()
        {
            // Arrange
            var context = GetInMemoryDbContext();

            var service = new VoitureService(context);

            // Act
            var resultat = await service.ObtenirToutesAsync();

            // Assert
            Assert.Empty(resultat);
        }


        // Tests de suppression

        [Fact]
        public async Task SupprimerAsync_Voiture2EstSupprimee()
        {
            // Arrange
            var context = GetInMemoryDbContext();

            context.Voitures.Add(voiture1);
            context.Voitures.Add(voiture2);
            await context.SaveChangesAsync();

            var service = new VoitureService(context);

            // Act
            await service.SupprimerAsync(2);

            // Assert
            var resultat = await context.Voitures.FindAsync(2);
            Assert.Null(resultat);
        }


        // Méthodes utilitaires

        private void AjouterRelations2Voitures(ApplicationDbContext context)
        {
            context.Marques.Add(marque1);
            context.Marques.Add(marque2);
            context.Modeles.Add(modele1);
            context.Modeles.Add(modele2);
            context.Finitions.Add(finition1);
            context.Finitions.Add(finition2);
            context.Reparations.Add(reparation1);
        }

        private void AjouterRelations3Voitures(ApplicationDbContext context)
        {
            context.Marques.Add(marque1);
            context.Marques.Add(marque2);
            context.Modeles.Add(modele1);
            context.Modeles.Add(modele2);
            context.Finitions.Add(finition1);
            context.Finitions.Add(finition2);
            context.Ventes.Add(vente3);
            context.Reparations.Add(reparation1);
            context.Reparations.Add(reparation3);
        }
    }
}
