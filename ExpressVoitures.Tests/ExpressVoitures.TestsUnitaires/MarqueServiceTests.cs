using System;
using System.Collections.Generic;
using System.Text;
using ExpressVoitures.Data;
using ExpressVoitures.Models;
using ExpressVoitures.Services;
using Microsoft.EntityFrameworkCore;

namespace ExpressVoitures.Tests.ExpressVoitures.TestsUnitaires
{
    public class MarqueServiceTests
    {
        private ApplicationDbContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            return new ApplicationDbContext(options);
        }

        private readonly Marque marque1;
        private readonly Marque marque2;

        public MarqueServiceTests()
        {
            marque1 = new Marque
            {
                Id = 1,
                Nom = "Marque1"
            };

            marque2 = new Marque()
            {
                Id = 2,
                Nom = "Marque2"
            };
        }

        // Tests de création

        [Fact]
        public async Task CreerAsync_ContextContient1Element()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var service = new MarqueService(context);

            // Act
            await service.CreerAsync(marque1);

            // Assert
            Assert.Equal(1, context.Marques.Count());
        }


        // Tests d'existance

        [Fact]
        public async Task ExisteAsync_RetourneVrai()
        {
            // Arrange
            var context = GetInMemoryDbContext();

            context.Marques.Add(marque1);
            await context.SaveChangesAsync();

            var service = new MarqueService(context);

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

            context.Marques.Add(marque1);
            await context.SaveChangesAsync();

            var service = new MarqueService(context);

            // Act
            var resultat = await service.ExisteAsync(2);

            // Assert
            Assert.False(resultat);
        }


        // Tests de mise à jour

        [Fact]
        public async Task MettreAJourAsync_Marque1SeNommeMarque4()
        {
            // Arrange
            var context = GetInMemoryDbContext();

            context.Marques.Add(marque1);
            await context.SaveChangesAsync();

            var service = new MarqueService(context);

            marque1.Nom = "Marque4";

            // Act
            await service.MettreAJourAsync(marque1);

            // Assert
            var resultat = await context.Marques.FindAsync(1);
            Assert.Equal(1, resultat.Id);
            Assert.Equal("Marque4", resultat.Nom);
        }


        // Tests d'obtention

        [Fact]
        public async Task ObtenirParIdAsync_RetourneLaMarqueCorrespondantAId()
        {
            // Arrange
            var context = GetInMemoryDbContext();

            context.Marques.Add(marque1);
            context.Marques.Add(marque2);
            await context.SaveChangesAsync();

            var service = new MarqueService(context);

            // Act
            var resultat = await service.ObtenirParIdAsync(1);

            // Assert
            Assert.NotNull(resultat);
            Assert.Equal(1, resultat.Id);
            Assert.Equal("Marque1", resultat.Nom);
        }

        [Fact]
        public async Task ObtenirParIdAsync_RetourneAucuneMarque()
        {
            // Arrange
            var context = GetInMemoryDbContext();

            context.Marques.Add(marque1);
            context.Marques.Add(marque2);
            await context.SaveChangesAsync();

            var service = new MarqueService(context);

            // Act
            var resultat = await service.ObtenirParIdAsync(3);

            // Assert
            Assert.Null(resultat);
        }

        [Fact]
        public async Task ObtenirToutesAsync_RetourneToutesLesMarques()
        {
            // Arrange
            var context = GetInMemoryDbContext();

            context.Marques.Add(marque1);
            context.Marques.Add(marque2);
            await context.SaveChangesAsync();

            var service = new MarqueService(context);

            // Act
            var resultat = await service.ObtenirToutesAsync();

            // Assert
            Assert.Equal(2, resultat.Count);
            Assert.Contains(resultat, f => f.Nom == "Marque1");
            Assert.Contains(resultat, f => f.Nom == "Marque2");
        }

        [Fact]
        public async Task ObtenirToutesAsync_RetourneAucuneMarque()
        {
            // Arrange
            var context = GetInMemoryDbContext();

            var service = new MarqueService(context);

            // Act
            var resultat = await service.ObtenirToutesAsync();

            // Assert
            Assert.Empty(resultat);
        }


        // Tests de suppression

        [Fact]
        public async Task SupprimerAsync_Marque2EstSupprimee()
        {
            // Arrange
            var context = GetInMemoryDbContext();

            context.Marques.Add(marque1);
            context.Marques.Add(marque2);
            await context.SaveChangesAsync();

            var service = new MarqueService(context);

            // Act
            await service.SupprimerAsync(2);

            // Assert
            var resultat = await context.Marques.FindAsync(2);
            Assert.Null(resultat);
        }
    }
}
