ExpressVoitures - Application de gestion de voitures


## Fonctionnalités principales :
- Création, modification et suppression de voitures  
- Affichage détaillé d’une voiture  
- Gestion des rôles (Administrateur, utilisateur connecté, visiteur)  
- Gestion complète de l’inventaire (voitures, marques, modèles, finitions, réparations, ventes)  
- Interface responsive (Bootstrap + CSS personnalisé)  
- Application conforme aux bonnes pratiques d’accessibilité (WCAG)  
- Base de données Code First avec Entity Framework Core


## Gestion des images :
- L’application permet d’uploader des images pour les annonces de voitures.
- Les images sont stockées dans : "wwwroot/images/voitures/"


## Installation et lancement :
1. Cloner le dépôt : "git clone https://github.com/S-Marnat/OC-Projet5.git" ;
2. Ouvrir la solution "ExpressVoitures.slnx" dans Visual Studio ;
3. Restaurer les dépendances : "dotnet restore" ;
4. Vérifier la chaîne de connexion dans "appsettings.json".
  Par défaut, elle utilise le serveur local (".") et fonctionne sur toute installation Visual Studio.
  Modifier uniquement si nécessaire.

 "ConnectionStrings": {
    "DefaultConnection": "Server=.;Database=ExpressVoitures;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True;"
  }

5. Appliquer les migrations : "dotnet ef database update" ;
6. Lancer l'application : "dotnet run".