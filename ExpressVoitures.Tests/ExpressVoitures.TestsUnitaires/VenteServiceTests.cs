using ExpressVoitures.Data;
using ExpressVoitures.Models;
using ExpressVoitures.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace ExpressVoitures.Tests.ExpressVoitures.TestsUnitaires
{
    public class VenteServiceTests
    {
        private ApplicationDbContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            return new ApplicationDbContext(options);
        }

        private readonly Vente vente1;
        private readonly Vente vente2;

        private readonly Voiture voiture1;
        private readonly Voiture voiture2;

        public VenteServiceTests()
        {
            vente1 = new Vente
            {
                Id = 1,
                Date = new DateTime(2026, 1, 1),
                IdVoiture = 1
            };

            vente2 = new Vente()
            {
                Id = 2,
                Date = new DateTime(2026, 2, 1),
                IdVoiture = 2
            };

            voiture1 = new Voiture
            {
                Id = 1,
                CodeVin = "00000000000000001",
                Annee = 2021,
                DateAchat = new DateTime(2025, 1, 1),
                PrixAchat = 1000,
                VoitureReparee = true,
                AnnoncePubliee = true,
                VoitureVendue = false
            };

            voiture2 = new Voiture
            {
                Id = 2,
                CodeVin = "00000000000000002",
                Annee = 2022,
                DateAchat = new DateTime(2025, 2, 1),
                PrixAchat = 2000,
                VoitureReparee = true,
                AnnoncePubliee = true,
                VoitureVendue = false
            };
        }

        // Tests de création

        [Fact]
        public async Task CreerAsync_ContextContient1Element()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var service = new VenteService(context);

            // Act
            await service.CreerAsync(vente1);

            // Assert
            Assert.Equal(1, context.Ventes.Count());
        }


        // Tests d'existance

        [Fact]
        public async Task ExisteAsync_RetourneVrai()
        {
            // Arrange
            var context = GetInMemoryDbContext();

            context.Ventes.Add(vente1);
            await context.SaveChangesAsync();

            var service = new VenteService(context);

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

            context.Ventes.Add(vente1);
            await context.SaveChangesAsync();

            var service = new VenteService(context);

            // Act
            var resultat = await service.ExisteAsync(2);

            // Assert
            Assert.False(resultat);
        }

        [Fact]
        public async Task ExistePourVoitureAsync_RetourneVrai()
        {
            // Arrange
            var context = GetInMemoryDbContext();

            context.Ventes.Add(vente1);
            await context.SaveChangesAsync();

            var service = new VenteService(context);

            // Act
            var resultat = await service.ExistePourVoitureAsync(1);

            // Assert
            Assert.True(resultat);
        }


        [Fact]
        public async Task ExistePourVoitureAsync_RetourneFaux()
        {
            // Arrange
            var context = GetInMemoryDbContext();

            context.Ventes.Add(vente1);
            await context.SaveChangesAsync();

            var service = new VenteService(context);

            // Act
            var resultat = await service.ExistePourVoitureAsync(2);

            // Assert
            Assert.False(resultat);
        }

        [Fact]
        public async Task ExistePourVoitureAsync_ExclureIdFinitionActuelle_RetourneFaux()
        {
            // Arrange
            var context = GetInMemoryDbContext();

            context.Ventes.Add(vente1);
            await context.SaveChangesAsync();

            var service = new VenteService(context);

            // Act
            var resultat = await service.ExistePourVoitureAsync(1, 1);

            // Assert
            Assert.False(resultat);
        }


        // Tests de mise à jour

        [Fact]
        public async Task MettreAJourAsync_Vente1SeNommeVente4()
        {
            // Arrange
            var context = GetInMemoryDbContext();

            context.Ventes.Add(vente1);
            await context.SaveChangesAsync();

            var service = new VenteService(context);

            vente1.Date = new DateTime(2026, 3, 1);

            // Act
            await service.MettreAJourAsync(vente1);

            // Assert
            var resultat = await context.Ventes.FindAsync(1);
            Assert.Equal(1, resultat.Id);
            Assert.Equal(new DateTime(2026, 3, 1), resultat.Date);
        }


        // Tests d'obtention

        [Fact]
        public async Task ObtenirParIdAsync_RetourneLaVenteCorrespondantAId()
        {
            // Arrange
            var context = GetInMemoryDbContext();

            context.Ventes.Add(vente1);
            context.Ventes.Add(vente2);
            context.Voitures.Add(voiture1);
            context.Voitures.Add(voiture2);
            await context.SaveChangesAsync();

            var service = new VenteService(context);

            // Act
            var resultat = await service.ObtenirParIdAsync(1);

            // Assert
            Assert.NotNull(resultat);
            Assert.Equal(1, resultat.Id);
            Assert.Equal(new DateTime(2026, 1, 1), resultat.Date);
        }

        [Fact]
        public async Task ObtenirParIdAsync_RetourneAucuneVente()
        {
            // Arrange
            var context = GetInMemoryDbContext();

            context.Ventes.Add(vente1);
            context.Ventes.Add(vente2);
            context.Voitures.Add(voiture1);
            context.Voitures.Add(voiture2);
            await context.SaveChangesAsync();

            var service = new VenteService(context);

            // Act
            var resultat = await service.ObtenirParIdAsync(3);

            // Assert
            Assert.Null(resultat);
        }

        [Fact]
        public async Task ObtenirParVoitureAsync_RetourneLesVentesLieesAVoiture()
        {
            // Arrange
            var context = GetInMemoryDbContext();

            context.Ventes.Add(vente1);
            context.Ventes.Add(vente2);
            await context.SaveChangesAsync();

            var service = new VenteService(context);

            // Act
            var resultat = await service.ObtenirParVoitureAsync(1);

            // Assert
            Assert.NotNull(resultat);
            Assert.Equal(1, resultat.Id);
            Assert.Equal(new DateTime(2026, 1, 1), resultat.Date);
            Assert.Equal(1, resultat.IdVoiture);
        }

        [Fact]
        public async Task ObtenirParVoitureAsync_RetourneAucuneVente()
        {
            // Arrange
            var context = GetInMemoryDbContext();

            context.Ventes.Add(vente1);
            context.Ventes.Add(vente2);
            await context.SaveChangesAsync();

            var service = new VenteService(context);

            // Act
            var resultat = await service.ObtenirParVoitureAsync(3);

            // Assert
            Assert.Null(resultat);
        }

        [Fact]
        public async Task ObtenirToutesAsync_RetourneToutesLesVentes()
        {
            // Arrange
            var context = GetInMemoryDbContext();

            context.Ventes.Add(vente1);
            context.Ventes.Add(vente2);
            context.Voitures.Add(voiture1);
            context.Voitures.Add(voiture2);
            await context.SaveChangesAsync();

            var service = new VenteService(context);

            // Act
            var resultat = await service.ObtenirToutesAsync();

            // Assert
            Assert.Equal(2, resultat.Count);
            Assert.Contains(resultat, f => f.Date == new DateTime(2026, 1, 1));
            Assert.Contains(resultat, f => f.Date == new DateTime(2026, 2, 1));
        }

        [Fact]
        public async Task ObtenirToutesAsync_RetourneAucuneVente()
        {
            // Arrange
            var context = GetInMemoryDbContext();

            var service = new VenteService(context);

            // Act
            var resultat = await service.ObtenirToutesAsync();

            // Assert
            Assert.Empty(resultat);
        }


        // Tests de suppression

        [Fact]
        public async Task SupprimerAsync_Vente2EstSupprimee()
        {
            // Arrange
            var context = GetInMemoryDbContext();

            context.Ventes.Add(vente1);
            context.Ventes.Add(vente2);
            await context.SaveChangesAsync();

            var service = new VenteService(context);

            // Act
            await service.SupprimerAsync(2);

            // Assert
            var resultat = await context.Ventes.FindAsync(2);
            Assert.Null(resultat);
        }
    }
}
