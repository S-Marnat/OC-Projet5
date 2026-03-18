using System;
using System.Collections.Generic;
using System.Text;
using ExpressVoitures.Data;
using ExpressVoitures.Models;
using ExpressVoitures.Services;
using Microsoft.EntityFrameworkCore;


namespace ExpressVoitures.Tests.ExpressVoitures.TestsUnitaires
{
    public class FinitionServiceTests
    {
        private ApplicationDbContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            return new ApplicationDbContext(options);
        }

        private readonly Finition finition1;
        private readonly Finition finition2;
        private readonly Finition finition3;

        private readonly Modele modele1;
        private readonly Modele modele2;

        public FinitionServiceTests()
        {
            finition1 = new Finition
            {
                Id = 1,
                Nom = "Finition1",
                IdModele = 1
            };

            finition2 = new Finition()
            {
                Id = 2,
                Nom = "Finition2",
                IdModele = 2
            };

            finition3 = new Finition
            {
                Id = 3,
                Nom = "Finition3",
                IdModele = 1
            };

            modele1 = new Modele
            {
                Id = 1,
                Nom = "Modèle1",
                IdMarque = 1
            };

            modele2 = new Modele
            {
                Id = 2,
                Nom = "Modèle2",
                IdMarque = 2
            };
        }

        // Tests de création

        [Fact]
        public async Task CreerAsync_ContextContient1Element()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var service = new FinitionService(context);

            // Act
            await service.CreerAsync(finition1);

            // Assert
            Assert.Equal(1, context.Finitions.Count());
        }


        // Tests d'existance

        [Fact]
        public async Task ExisteAsync_RetourneVrai()
        {
            // Arrange
            var context = GetInMemoryDbContext();

            context.Finitions.Add(finition1);
            await context.SaveChangesAsync();

            var service = new FinitionService(context);

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

            context.Finitions.Add(finition1);
            await context.SaveChangesAsync();

            var service = new FinitionService(context);

            // Act
            var resultat = await service.ExisteAsync(2);

            // Assert
            Assert.False(resultat);
        }

        [Fact]
        public async Task ExistePourModeleAsync_RetourneVrai()
        {
            // Arrange
            var context = GetInMemoryDbContext();

            context.Finitions.Add(finition1);
            await context.SaveChangesAsync();

            var service = new FinitionService(context);

            // Act
            var resultat = await service.ExistePourModeleAsync("Finition1", 1);

            // Assert
            Assert.True(resultat);
        }


        [Fact]
        public async Task ExistePourModeleAsync_RetourneFaux()
        {
            // Arrange
            var context = GetInMemoryDbContext();

            context.Finitions.Add(finition1);
            await context.SaveChangesAsync();

            var service = new FinitionService(context);

            // Act
            var resultat = await service.ExistePourModeleAsync("Finition2", 1);

            // Assert
            Assert.False(resultat);
        }

        [Fact]
        public async Task ExistePourModeleAsync_ExclureIdFinitionActuelle_RetourneFaux()
        {
            // Arrange
            var context = GetInMemoryDbContext();

            context.Finitions.Add(finition1);
            await context.SaveChangesAsync();

            var service = new FinitionService(context);

            // Act
            var resultat = await service.ExistePourModeleAsync("Finition1", 1, 1);

            // Assert
            Assert.False(resultat);
        }


        // Tests de mise à jour

        [Fact]
        public async Task MettreAJourAsync_Finition1SeNommeFinition4()
        {
            // Arrange
            var context = GetInMemoryDbContext();

            context.Finitions.Add(finition1);
            await context.SaveChangesAsync();

            var service = new FinitionService(context);

            finition1.Nom = "Finition4";

            // Act
            await service.MettreAJourAsync(finition1);

            // Assert
            var resultat = await context.Finitions.FindAsync(1);
            Assert.Equal(1, resultat.Id);
            Assert.Equal("Finition4", resultat.Nom);
        }


        // Tests d'obtention

        [Fact]
        public async Task ObtenirParIdAsync_RetourneLaFinitionCorrespondantAId()
        {
            // Arrange
            var context = GetInMemoryDbContext();

            context.Finitions.Add(finition1);
            context.Finitions.Add(finition2);
            context.Modeles.Add(modele1);
            context.Modeles.Add(modele2);
            await context.SaveChangesAsync();

            var service = new FinitionService(context);

            // Act
            var resultat = await service.ObtenirParIdAsync(1);

            // Assert
            Assert.NotNull(resultat);
            Assert.Equal(1, resultat.Id);
            Assert.Equal("Finition1", resultat.Nom);
        }

        [Fact]
        public async Task ObtenirParIdAsync_RetourneAucuneFinition()
        {
            // Arrange
            var context = GetInMemoryDbContext();

            context.Finitions.Add(finition1);
            context.Finitions.Add(finition2);
            context.Modeles.Add(modele1);
            context.Modeles.Add(modele2);
            await context.SaveChangesAsync();

            var service = new FinitionService(context);

            // Act
            var resultat = await service.ObtenirParIdAsync(3);

            // Assert
            Assert.Null(resultat);
        }

        [Fact]
        public async Task ObtenirParModeleAsync_RetourneLesFinitionsLieesAuModele()
        {
            // Arrange
            var context = GetInMemoryDbContext();

            context.Finitions.Add(finition1);
            context.Finitions.Add(finition2);
            context.Finitions.Add(finition3);
            await context.SaveChangesAsync();

            var service = new FinitionService(context);

            // Act
            var resultat = await service.ObtenirParModeleAsync(1);

            // Assert
            Assert.Equal(2, resultat.Count);
            Assert.Contains(resultat, f => f.Nom == "Finition1");
            Assert.Contains(resultat, f => f.Nom == "Finition3");
        }

        [Fact]
        public async Task ObtenirParModeleAsync_RetourneAucuneFinition()
        {
            // Arrange
            var context = GetInMemoryDbContext();

            context.Finitions.Add(finition1);
            context.Finitions.Add(finition2);
            await context.SaveChangesAsync();

            var service = new FinitionService(context);

            // Act
            var resultat = await service.ObtenirParModeleAsync(3);

            // Assert
            Assert.Empty(resultat);
        }

        [Fact]
        public async Task ObtenirToutesAsync_RetourneToutesLesFinitions()
        {
            // Arrange
            var context = GetInMemoryDbContext();

            context.Finitions.Add(finition1);
            context.Finitions.Add(finition2);
            context.Modeles.Add(modele1);
            context.Modeles.Add(modele2);
            await context.SaveChangesAsync();

            var service = new FinitionService(context);

            // Act
            var resultat = await service.ObtenirToutesAsync();

            // Assert
            Assert.Equal(2, resultat.Count);
            Assert.Contains(resultat, f => f.Nom == "Finition1");
            Assert.Contains(resultat, f => f.Nom == "Finition2");
        }

        [Fact]
        public async Task ObtenirToutesAsync_RetourneAucuneFinition()
        {
            // Arrange
            var context = GetInMemoryDbContext();

            var service = new FinitionService(context);

            // Act
            var resultat = await service.ObtenirToutesAsync();

            // Assert
            Assert.Empty(resultat);
        }


        // Tests de suppression

        [Fact]
        public async Task SupprimerAsync_Finition2EstSupprimee()
        {
            // Arrange
            var context = GetInMemoryDbContext();

            context.Finitions.Add(finition1);
            context.Finitions.Add(finition2);
            await context.SaveChangesAsync();

            var service = new FinitionService(context);

            // Act
            await service.SupprimerAsync(2);

            // Assert
            var resultat = await context.Finitions.FindAsync(2);
            Assert.Null(resultat);
        }
    }
}

