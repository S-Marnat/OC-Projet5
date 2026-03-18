using System;
using System.Collections.Generic;
using System.Text;
using ExpressVoitures.Data;
using ExpressVoitures.Models;
using ExpressVoitures.Services;
using Microsoft.EntityFrameworkCore;

namespace ExpressVoitures.Tests.ExpressVoitures.TestsUnitaires
{
    public class ModeleServiceTests
    {
        private ApplicationDbContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            return new ApplicationDbContext(options);
        }

        private readonly Modele modele1;
        private readonly Modele modele2;
        private readonly Modele modele3;

        private readonly Marque marque1;
        private readonly Marque marque2;

        public ModeleServiceTests()
        {
            modele1 = new Modele
            {
                Id = 1,
                Nom = "Modele1",
                IdMarque = 1
            };

            modele2 = new Modele()
            {
                Id = 2,
                Nom = "Modele2",
                IdMarque = 2
            };

            modele3 = new Modele
            {
                Id = 3,
                Nom = "Modele3",
                IdMarque = 1
            };

            marque1 = new Marque
            {
                Id = 1,
                Nom = "Modèle1"
            };

            marque2 = new Marque
            {
                Id = 2,
                Nom = "Modèle2"
            };
        }

        // Tests de création

        [Fact]
        public async Task CreerAsync_ContextContient1Element()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var service = new ModeleService(context);

            // Act
            await service.CreerAsync(modele1);

            // Assert
            Assert.Equal(1, context.Modeles.Count());
        }


        // Tests d'existance

        [Fact]
        public async Task ExisteAsync_RetourneVrai()
        {
            // Arrange
            var context = GetInMemoryDbContext();

            context.Modeles.Add(modele1);
            await context.SaveChangesAsync();

            var service = new ModeleService(context);

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

            context.Modeles.Add(modele1);
            await context.SaveChangesAsync();

            var service = new ModeleService(context);

            // Act
            var resultat = await service.ExisteAsync(2);

            // Assert
            Assert.False(resultat);
        }

        [Fact]
        public async Task ExistePourMarqueAsync_RetourneVrai()
        {
            // Arrange
            var context = GetInMemoryDbContext();

            context.Modeles.Add(modele1);
            await context.SaveChangesAsync();

            var service = new ModeleService(context);

            // Act
            var resultat = await service.ExistePourMarqueAsync("Modele1", 1);

            // Assert
            Assert.True(resultat);
        }


        [Fact]
        public async Task ExistePourMarqueAsync_RetourneFaux()
        {
            // Arrange
            var context = GetInMemoryDbContext();

            context.Modeles.Add(modele1);
            await context.SaveChangesAsync();

            var service = new ModeleService(context);

            // Act
            var resultat = await service.ExistePourMarqueAsync("Modele2", 1);

            // Assert
            Assert.False(resultat);
        }

        [Fact]
        public async Task ExistePourMarqueAsync_ExclureIdModeleActuelle_RetourneFaux()
        {
            // Arrange
            var context = GetInMemoryDbContext();

            context.Modeles.Add(modele1);
            await context.SaveChangesAsync();

            var service = new ModeleService(context);

            // Act
            var resultat = await service.ExistePourMarqueAsync("Modele1", 1, 1);

            // Assert
            Assert.False(resultat);
        }


        // Tests de mise à jour

        [Fact]
        public async Task MettreAJourAsync_Modele1SeNommeModele4()
        {
            // Arrange
            var context = GetInMemoryDbContext();

            context.Modeles.Add(modele1);
            await context.SaveChangesAsync();

            var service = new ModeleService(context);

            modele1.Nom = "Modele4";

            // Act
            await service.MettreAJourAsync(modele1);

            // Assert
            var resultat = await context.Modeles.FindAsync(1);
            Assert.Equal(1, resultat.Id);
            Assert.Equal("Modele4", resultat.Nom);
        }


        // Tests d'obtention

        [Fact]
        public async Task ObtenirParIdAsync_RetourneLeModeleCorrespondantAId()
        {
            // Arrange
            var context = GetInMemoryDbContext();

            context.Modeles.Add(modele1);
            context.Modeles.Add(modele2);
            context.Marques.Add(marque1);
            context.Marques.Add(marque2);
            await context.SaveChangesAsync();

            var service = new ModeleService(context);

            // Act
            var resultat = await service.ObtenirParIdAsync(1);

            // Assert
            Assert.NotNull(resultat);
            Assert.Equal(1, resultat.Id);
            Assert.Equal("Modele1", resultat.Nom);
        }

        [Fact]
        public async Task ObtenirParIdAsync_RetourneAucuneModele()
        {
            // Arrange
            var context = GetInMemoryDbContext();

            context.Modeles.Add(modele1);
            context.Modeles.Add(modele2);
            context.Marques.Add(marque1);
            context.Marques.Add(marque2);
            await context.SaveChangesAsync();

            var service = new ModeleService(context);

            // Act
            var resultat = await service.ObtenirParIdAsync(3);

            // Assert
            Assert.Null(resultat);
        }

        [Fact]
        public async Task ObtenirParMarqueAsync_RetourneLesModelesLiesAMarque()
        {
            // Arrange
            var context = GetInMemoryDbContext();

            context.Modeles.Add(modele1);
            context.Modeles.Add(modele2);
            context.Modeles.Add(modele3);
            await context.SaveChangesAsync();

            var service = new ModeleService(context);

            // Act
            var resultat = await service.ObtenirParMarqueAsync(1);

            // Assert
            Assert.Equal(2, resultat.Count);
            Assert.Contains(resultat, f => f.Nom == "Modele1");
            Assert.Contains(resultat, f => f.Nom == "Modele3");
        }

        [Fact]
        public async Task ObtenirParMarqueAsync_RetourneAucuneModele()
        {
            // Arrange
            var context = GetInMemoryDbContext();

            context.Modeles.Add(modele1);
            context.Modeles.Add(modele2);
            await context.SaveChangesAsync();

            var service = new ModeleService(context);

            // Act
            var resultat = await service.ObtenirParMarqueAsync(3);

            // Assert
            Assert.Empty(resultat);
        }

        [Fact]
        public async Task ObtenirTousAsync_RetourneTousLesModeles()
        {
            // Arrange
            var context = GetInMemoryDbContext();

            context.Modeles.Add(modele1);
            context.Modeles.Add(modele2);
            context.Marques.Add(marque1);
            context.Marques.Add(marque2);
            await context.SaveChangesAsync();

            var service = new ModeleService(context);

            // Act
            var resultat = await service.ObtenirTousAsync();

            // Assert
            Assert.Equal(2, resultat.Count);
            Assert.Contains(resultat, f => f.Nom == "Modele1");
            Assert.Contains(resultat, f => f.Nom == "Modele2");
        }

        [Fact]
        public async Task ObtenirTousAsync_RetourneAucuneModele()
        {
            // Arrange
            var context = GetInMemoryDbContext();

            var service = new ModeleService(context);

            // Act
            var resultat = await service.ObtenirTousAsync();

            // Assert
            Assert.Empty(resultat);
        }


        // Tests de suppression

        [Fact]
        public async Task SupprimerAsync_Modele2EstSupprime()
        {
            // Arrange
            var context = GetInMemoryDbContext();

            context.Modeles.Add(modele1);
            context.Modeles.Add(modele2);
            await context.SaveChangesAsync();

            var service = new ModeleService(context);

            // Act
            await service.SupprimerAsync(2);

            // Assert
            var resultat = await context.Modeles.FindAsync(2);
            Assert.Null(resultat);
        }
    }
}
