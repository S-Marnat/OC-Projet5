using ExpressVoitures.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ExpressVoitures.Data
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext(options)
    {
        public DbSet<Voiture> Voitures { get; set; }
        public DbSet<Marque> Marques { get; set; }
        public DbSet<Modele> Modeles { get; set; }
        public DbSet<Finition> Finitions { get; set; }
        public DbSet<Reparation> Reparations { get; set; }
        public DbSet<Vente> Ventes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Relation Marque-Modele - Supprimer une marque supprime ses modèles
            modelBuilder.Entity<Marque>()
                .HasMany(ma => ma.Modeles)
                .WithOne(mo => mo.Marque)
                .HasForeignKey(mo => mo.IdMarque)
                .OnDelete(DeleteBehavior.Cascade);

            // Relation Modele-Finition - Supprimer un modèle supprime ses finitions
            modelBuilder.Entity<Modele>()
                .HasMany(mo => mo.Finitions)
                .WithOne(ma => ma.Modele)
                .HasForeignKey(ma => ma.IdModele)
                .OnDelete(DeleteBehavior.Cascade);

            // Relation Marque-Voiture - On ne peut pas supprimer une marque si une voiture lui est associée
            modelBuilder.Entity<Marque>()
                .HasMany(ma => ma.Voitures)
                .WithOne(vo => vo.Marque)
                .HasForeignKey(vo => vo.IdMarque)
                .OnDelete(DeleteBehavior.Restrict);

            // Relation Modele-Voiture - On ne peut pas supprimer un modèle si une voiture lui est associée
            modelBuilder.Entity<Modele>()
                .HasMany(mo => mo.Voitures)
                .WithOne(vo => vo.Modele)
                .HasForeignKey(vo => vo.IdModele)
                .OnDelete(DeleteBehavior.Restrict);

            // Relation Finition-Voiture - On ne peut pas supprimer une finition si une voiture lui est associée
            modelBuilder.Entity<Finition>()
                .HasMany(f => f.Voitures)
                .WithOne(vo => vo.Finition)
                .HasForeignKey(vo => vo.IdFinition)
                .OnDelete(DeleteBehavior.Restrict);

            // Relation Voiture-Réparation - Supprimer une voiture supprime ses réparations éventuelles
            modelBuilder.Entity<Voiture>()
                .HasMany(vo => vo.Reparations)
                .WithOne(r => r.Voiture)
                .HasForeignKey(r => r.IdVoiture)
                .OnDelete(DeleteBehavior.Cascade);

            // Relation Voiture-Vente - Supprimer une voiture supprime sa vente éventuelle
            modelBuilder.Entity<Voiture>()
                .HasOne(vo => vo.Vente)
                .WithOne(ve => ve.Voiture)
                .HasForeignKey<Vente>(ve => ve.IdVoiture)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
