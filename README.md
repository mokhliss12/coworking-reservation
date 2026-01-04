# 📋 Plateforme de Réservation de Salles (Coworking) - Guide Complet des Workflows

## 🎯 Vue d'ensemble

Cette application permet la gestion complète d'un espace de coworking avec deux types d'utilisateurs :
- **👤 Client** : Peut consulter les salles, réserver, voir ses factures et notifications
- **👨‍💼 Admin** : Gestion complète (salles, réservations, factures, notifications)

---

## 🚀 Démarrage Rapide

### Prérequis
- .NET 10.0 SDK
- SQL Server (LocalDB ou SQL Server Express)
- Visual Studio ou VS Code

### Installation

1. **Restaurer les packages NuGet**
```bash
dotnet restore
```

2. **Appliquer les migrations**
```bash
cd Coworking.Infrastructure
dotnet ef database update --startup-project ../Coworking.Web
```

3. **Lancer l'application**
```bash
cd Coworking.Web
dotnet run
```

4. **Accéder à l'application**
- URL : `https://localhost:5001` ou `http://localhost:5000`

### Comptes par défaut

**Administrateur :**
- Email : `admin@coworking.com`
- Mot de passe : `Admin123!`

**Note** : Les nouveaux utilisateurs sont automatiquement assignés au rôle "Client".

---

## 📖 Workflows Complets

## 1️⃣ WORKFLOW D'AUTHENTIFICATION

### 1.1 Inscription d'un nouveau client

**Objectif** : Créer un compte utilisateur avec le rôle Client

**Étapes** :
1. Accéder à la page d'accueil
2. Cliquer sur "Inscription" dans le menu (ou "✨ Inscription")
3. Remplir le formulaire :
   - **Nom** : Ex. "Dupont"
   - **Prénom** : Ex. "Jean"
   - **Email** : Ex. "jean.dupont@email.com"
   - **Type d'utilisateur** : Sélectionner (Étudiant, Freelance, Startup, Entreprise, Formateur)
   - **Entreprise** (optionnel) : Ex. "Ma Startup"
   - **Mot de passe** : Minimum 6 caractères (avec majuscule, minuscule, chiffre)
   - **Confirmer le mot de passe** : Répéter le mot de passe
4. Cliquer sur "✨ Créer mon compte"
5. **Résultat attendu** : Redirection automatique vers la page d'accueil, utilisateur connecté

**Vérifications** :
- ✅ L'utilisateur est connecté
- ✅ Le menu affiche le nom de l'utilisateur
- ✅ Le rôle "Client" est assigné automatiquement

---

### 1.2 Connexion

**Objectif** : Se connecter avec un compte existant

**Étapes** :
1. Cliquer sur "🔐 Connexion" dans le menu
2. Entrer les identifiants :
   - **Email** : `admin@coworking.com` (admin) ou email du compte créé
   - **Mot de passe** : `Admin123!` (admin) ou mot de passe du compte
   - **Se souvenir de moi** (optionnel) : Cocher pour rester connecté
3. Cliquer sur "🚀 Se connecter"
4. **Résultat attendu** : Redirection vers la page d'accueil, utilisateur connecté

**Vérifications** :
- ✅ Menu affiche le nom de l'utilisateur
- ✅ Options de navigation selon le rôle

---

### 1.3 Déconnexion

**Étapes** :
1. Cliquer sur le nom d'utilisateur dans le menu (dropdown)
2. Sélectionner "Déconnexion"
3. **Résultat attendu** : Redirection vers la page d'accueil, utilisateur déconnecté

---

## 2️⃣ WORKFLOW CLIENT - CONSULTATION DES SALLES

### 2.1 Consulter la liste des salles (publique)

**Objectif** : Voir toutes les salles disponibles sans être connecté

**Étapes** :
1. Accéder à la page d'accueil
2. Cliquer sur "🚪 Salles" dans le menu
3. **Résultat attendu** : Affichage de toutes les salles avec :
   - Nom de la salle
   - Capacité
   - Prix horaire
   - Nombre de réservations actives
   - Bouton "Voir les détails"
   - Bouton "Réserver (connexion requise)" si non connecté

**Vérifications** :
- ✅ Toutes les salles sont visibles
- ✅ Les informations sont correctes
- ✅ Les boutons fonctionnent

---

### 2.2 Voir les détails d'une salle

**Étapes** :
1. Depuis la liste des salles, cliquer sur "Voir les détails" d'une salle
2. **Résultat attendu** : Page de détails affichant :
   - Informations complètes (capacité, prix)
   - Prochaines réservations (5 maximum)
   - Bouton "Réserver cette salle" (si connecté) ou "Connectez-vous pour réserver"

**Vérifications** :
- ✅ Informations correctes
- ✅ Réservations futures affichées
- ✅ Bouton de réservation fonctionnel

---

## 3️⃣ WORKFLOW CLIENT - RÉSERVATION

### 3.1 Créer une réservation (Client connecté)

**Objectif** : Réserver une salle pour une période donnée

**Étapes** :
1. Se connecter en tant que Client
2. Option A - Depuis la liste des salles :
   - Aller dans "🚪 Salles"
   - Cliquer sur "Réserver" sur une salle
3. Option B - Depuis le menu :
   - Cliquer sur "📅 Mes Réservations"
   - Cliquer sur "➕ Nouvelle réservation"
4. Remplir le formulaire :
   - **Salle** : Sélectionner une salle (pré-sélectionnée si venant de la liste)
   - **Date début** : Ex. "2026-01-15 09:00"
   - **Date fin** : Ex. "2026-01-15 12:00"
   - **Note** : Les informations client sont pré-remplies automatiquement
5. Cliquer sur "💾 Enregistrer"
6. **Résultat attendu** :
   - Redirection vers la liste des réservations
   - Nouvelle réservation avec statut "En attente"
   - Notification créée automatiquement
   - Montant calculé automatiquement (durée × prix horaire)

**Vérifications** :
- ✅ Réservation créée avec statut "EnAttente"
- ✅ Montant calculé correctement
- ✅ Notification générée
- ✅ Pas de conflit avec d'autres réservations

---

### 3.2 Tester la validation des conflits

**Objectif** : Vérifier que le système empêche les réservations en conflit

**Étapes** :
1. Créer une première réservation pour une salle (ex: Salle A, 10h-12h)
2. Essayer de créer une deuxième réservation pour la même salle avec un créneau qui chevauche :
   - Cas 1 : 11h-13h (chevauchement partiel)
   - Cas 2 : 09h-11h (chevauchement partiel)
   - Cas 3 : 09h-13h (chevauchement complet)
3. **Résultat attendu** : Message d'erreur "Cette salle est déjà réservée pour cette période"

**Vérifications** :
- ✅ Les conflits sont détectés
- ✅ Message d'erreur affiché
- ✅ La réservation n'est pas créée

---

### 3.3 Consulter ses réservations

**Étapes** :
1. Se connecter en tant que Client
2. Cliquer sur "📅 Mes Réservations"
3. **Résultat attendu** : Liste de toutes les réservations du client avec :
   - Informations client
   - Salle réservée
   - Dates (début et fin)
   - Durée
   - Montant
   - Statut (badge coloré)
   - Actions (Détails, Modifier)

**Vérifications** :
- ✅ Seules les réservations du client sont visibles
- ✅ Informations correctes
- ✅ Statuts affichés correctement

---

### 3.4 Utiliser les filtres de recherche (Réservations)

**Étapes** :
1. Aller dans "📅 Mes Réservations"
2. Utiliser les filtres :
   - **Recherche** : Taper un nom de salle ou client
   - **Statut** : Sélectionner "Confirmée", "En attente", ou "Annulée"
   - **Date début** : Sélectionner une date minimale
   - **Date fin** : Sélectionner une date maximale
3. Cliquer sur "Filtrer"
4. **Résultat attendu** : Liste filtrée selon les critères

**Vérifications** :
- ✅ Les filtres fonctionnent correctement
- ✅ La recherche texte fonctionne
- ✅ Les filtres de date fonctionnent

---

### 3.5 Voir les détails d'une réservation

**Étapes** :
1. Dans la liste des réservations, cliquer sur "Détails"
2. **Résultat attendu** : Page de détails affichant :
   - Toutes les informations de la réservation
   - Informations client
   - Informations de la salle
   - Dates et durée
   - Montant total
   - Statut
   - Liste des factures associées (si existantes)
   - Boutons d'action

**Vérifications** :
- ✅ Toutes les informations sont affichées
- ✅ Les factures associées sont visibles
- ✅ Les boutons fonctionnent

---

### 3.6 Modifier une réservation

**Étapes** :
1. Dans la liste des réservations, cliquer sur "Modifier"
2. Modifier les champs souhaités (dates, salle, etc.)
3. Cliquer sur "💾 Enregistrer"
4. **Résultat attendu** :
   - Réservation mise à jour
   - Montant recalculé automatiquement
   - Validation des conflits effectuée

**Vérifications** :
- ✅ Les modifications sont sauvegardées
- ✅ Le montant est recalculé
- ✅ Les conflits sont vérifiés

---

### 3.7 Annuler une réservation

**Étapes** :
1. Dans la liste des réservations, cliquer sur "Modifier"
2. Changer le statut à "Annulée" OU cliquer sur "Supprimer"
3. Confirmer l'annulation
4. **Résultat attendu** :
   - Statut changé à "Annulee"
   - Réservation toujours visible mais marquée comme annulée

**Vérifications** :
- ✅ Le statut est mis à jour
- ✅ La réservation n'apparaît plus dans les disponibilités

---

## 4️⃣ WORKFLOW CLIENT - PLANNING

### 4.1 Consulter le planning (sans connexion)

**Objectif** : Voir les disponibilités sans être connecté

**Étapes** :
1. Sans être connecté, cliquer sur "📊 Planning" dans le menu
2. **Résultat attendu** : Calendrier mensuel affichant :
   - Tous les jours du mois
   - Les réservations pour chaque jour
   - Statut des réservations (badges colorés)
   - Indication "Disponible" pour les jours sans réservation

**Vérifications** :
- ✅ Le planning est accessible sans connexion
- ✅ Les réservations sont visibles
- ✅ Les informations sont correctes

---

### 4.2 Filtrer le planning

**Étapes** :
1. Aller dans "📊 Planning"
2. Utiliser les filtres :
   - **Salle** : Sélectionner une salle spécifique
   - **Mois** : Choisir un mois
   - **Année** : Choisir une année
3. Cliquer sur "Filtrer"
4. **Résultat attendu** : Planning filtré selon les critères

**Vérifications** :
- ✅ Les filtres fonctionnent
- ✅ Seules les réservations de la salle sélectionnée sont affichées
- ✅ Le mois et l'année changent correctement

---

## 5️⃣ WORKFLOW CLIENT - FACTURES

### 5.1 Consulter ses factures

**Étapes** :
1. Se connecter en tant que Client
2. Cliquer sur "💰 Factures"
3. **Résultat attendu** : Liste de toutes les factures du client avec :
   - Numéro de facture
   - Date d'émission
   - Réservation associée
   - Salle
   - Montant
   - Statut (Payée/En attente)
   - Actions (Détails, PDF)

**Vérifications** :
- ✅ Seules les factures du client sont visibles
- ✅ Les informations sont correctes

---

### 5.2 Utiliser les filtres de recherche (Factures)

**Étapes** :
1. Aller dans "💰 Factures"
2. Utiliser les filtres :
   - **Recherche** : Numéro, client, salle
   - **Statut** : Payées ou En attente
   - **Date début** : Date minimale d'émission
   - **Date fin** : Date maximale d'émission
3. Cliquer sur "Filtrer"
4. **Résultat attendu** : Liste filtrée

**Vérifications** :
- ✅ Les filtres fonctionnent
- ✅ La recherche texte fonctionne

---

### 5.3 Voir les détails d'une facture

**Étapes** :
1. Dans la liste des factures, cliquer sur "Détails"
2. **Résultat attendu** : Page de détails avec :
   - Numéro de facture
   - Date d'émission
   - Informations client
   - Informations de réservation
   - Montant total
   - Statut
   - Bouton "📄 Télécharger PDF"

**Vérifications** :
- ✅ Toutes les informations sont affichées
- ✅ Le bouton PDF est visible

---

### 5.4 Télécharger une facture en PDF

**Étapes** :
1. Dans les détails d'une facture, cliquer sur "📄 Télécharger PDF"
   OU depuis la liste, cliquer sur "📄 PDF"
2. **Résultat attendu** :
   - Téléchargement d'un fichier PDF
   - Nom du fichier : `Facture_FACT-YYYY-NNNN.pdf`
   - Contenu du PDF :
     - En-tête avec numéro de facture
     - Date d'émission
     - Informations client
     - Détails de la réservation
     - Montant total en évidence
     - Pied de page avec nom de l'application

**Vérifications** :
- ✅ Le PDF se télécharge correctement
- ✅ Le contenu est complet et bien formaté
- ✅ Le nom du fichier est correct

---

## 6️⃣ WORKFLOW CLIENT - NOTIFICATIONS

### 6.1 Consulter ses notifications

**Étapes** :
1. Se connecter en tant que Client
2. Cliquer sur "🔔 Notifications"
3. **Résultat attendu** : Liste de toutes les notifications du client avec :
   - Statut (Lue/Non lue) - badges colorés
   - Titre
   - Message
   - Type (Reservation, Facture, Rappel)
   - Date de création
   - Actions (Détails, Marquer lue, Supprimer)

**Vérifications** :
- ✅ Seules les notifications du client sont visibles
- ✅ Les notifications non lues sont mises en évidence
- ✅ Les types sont correctement identifiés

---

### 6.2 Utiliser les filtres de recherche (Notifications)

**Étapes** :
1. Aller dans "🔔 Notifications"
2. Utiliser les filtres :
   - **Recherche** : Titre ou message
   - **Type** : Reservation, Facture, ou Rappel
   - **Statut** : Lues ou Non lues
3. Cliquer sur "Filtrer"
4. **Résultat attendu** : Liste filtrée

**Vérifications** :
- ✅ Les filtres fonctionnent
- ✅ La recherche texte fonctionne

---

### 6.3 Marquer une notification comme lue

**Étapes** :
1. Dans la liste des notifications, trouver une notification non lue
2. Cliquer sur "Marquer lue"
3. **Résultat attendu** :
   - Badge change de "Non lue" à "Lue"
   - Notification reste dans la liste

**Vérifications** :
- ✅ Le statut est mis à jour
- ✅ L'apparence change (couleur du badge)

---

### 6.4 Marquer toutes les notifications comme lues

**Étapes** :
1. Aller dans "🔔 Notifications"
2. Si des notifications non lues existent, cliquer sur "Marquer toutes comme lues"
3. **Résultat attendu** : Toutes les notifications sont marquées comme lues

**Vérifications** :
- ✅ Toutes les notifications sont mises à jour
- ✅ Le bouton disparaît s'il n'y a plus de notifications non lues

---

### 6.5 Voir les détails d'une notification

**Étapes** :
1. Cliquer sur "Détails" d'une notification
2. **Résultat attendu** :
   - Page de détails avec toutes les informations
   - Lien vers la réservation associée (si applicable)
   - Notification automatiquement marquée comme lue

**Vérifications** :
- ✅ Les détails sont complets
- ✅ Le lien vers la réservation fonctionne
- ✅ La notification est marquée comme lue automatiquement

---

## 7️⃣ WORKFLOW ADMIN - GESTION DES SALLES

### 7.1 Se connecter en tant qu'Admin

**Étapes** :
1. Cliquer sur "🔐 Connexion"
2. Entrer :
   - Email : `admin@coworking.com`
   - Mot de passe : `Admin123!`
3. Cliquer sur "Se connecter"
4. **Résultat attendu** :
   - Connexion réussie
   - Menu affiche "⚙️ Gestion Salles" (en plus des autres options)

**Vérifications** :
- ✅ Le menu admin est visible
- ✅ Les options admin sont accessibles

---

### 7.2 Consulter la liste des salles (Admin)

**Étapes** :
1. Se connecter en tant qu'Admin
2. Cliquer sur "⚙️ Gestion Salles"
3. **Résultat attendu** : Liste de toutes les salles avec :
   - Nom
   - Capacité
   - Prix horaire
   - Actions (Détails, Modifier, Supprimer)

**Vérifications** :
- ✅ Toutes les salles sont visibles
- ✅ Les actions admin sont disponibles

---

### 7.3 Créer une nouvelle salle

**Étapes** :
1. Dans "⚙️ Gestion Salles", cliquer sur "➕ Ajouter une salle"
2. Remplir le formulaire :
   - **Nom** : Ex. "Salle de réunion A"
   - **Capacité** : Ex. "10"
   - **Prix horaire** : Ex. "150.00"
3. Cliquer sur "💾 Enregistrer"
4. **Résultat attendu** :
   - Redirection vers la liste
   - Nouvelle salle visible dans la liste
   - Salle disponible pour les réservations

**Vérifications** :
- ✅ La salle est créée
- ✅ Elle apparaît dans la liste publique
- ✅ Elle peut être sélectionnée lors d'une réservation

---

### 7.4 Modifier une salle

**Étapes** :
1. Dans la liste des salles, cliquer sur "Modifier"
2. Modifier les informations (nom, capacité, prix)
3. Cliquer sur "💾 Enregistrer"
4. **Résultat attendu** : Salle mise à jour

**Vérifications** :
- ✅ Les modifications sont sauvegardées
- ✅ Les nouvelles informations apparaissent partout

---

### 7.5 Voir les détails d'une salle (Admin)

**Étapes** :
1. Cliquer sur "Détails" d'une salle
2. **Résultat attendu** : Page de détails avec :
   - Informations de la salle
   - Nombre de réservations
   - Liste de toutes les réservations (avec statuts)

**Vérifications** :
- ✅ Toutes les informations sont affichées
- ✅ La liste des réservations est complète

---

### 7.6 Supprimer une salle

**Étapes** :
1. Cliquer sur "Supprimer" d'une salle
2. Confirmer la suppression
3. **Résultat attendu** :
   - Salle supprimée
   - Redirection vers la liste
   - Salle n'apparaît plus

**Vérifications** :
- ✅ La salle est supprimée
- ✅ Elle n'apparaît plus dans les listes

---

## 8️⃣ WORKFLOW ADMIN - GESTION DES RÉSERVATIONS

### 8.1 Voir toutes les réservations (Admin)

**Étapes** :
1. Se connecter en tant qu'Admin
2. Cliquer sur "📅 Mes Réservations" (affiche toutes les réservations)
3. **Résultat attendu** : Liste de TOUTES les réservations avec :
   - Informations client
   - Salle
   - Dates
   - Montant
   - Statut
   - Actions (Détails, Modifier, Confirmer)

**Vérifications** :
- ✅ Toutes les réservations sont visibles (pas seulement celles de l'admin)
- ✅ Le bouton "Confirmer" est visible pour les réservations en attente

---

### 8.2 Confirmer une réservation

**Objectif** : Valider une réservation en attente

**Étapes** :
1. Dans la liste des réservations, trouver une réservation avec statut "En attente"
2. Cliquer sur "Confirmer"
3. **Résultat attendu** :
   - Statut changé à "Confirmee"
   - Notification créée automatiquement pour le client
   - Badge mis à jour (vert)

**Vérifications** :
- ✅ Le statut est mis à jour
- ✅ Une notification est créée pour le client
- ✅ Le badge change de couleur

---

### 8.3 Créer une réservation pour un client (Admin)

**Étapes** :
1. Cliquer sur "➕ Nouvelle réservation"
2. Remplir le formulaire :
   - **Salle** : Sélectionner
   - **Client Nom** : Ex. "Martin"
   - **Client Email** : Ex. "martin@email.com"
   - **Client Téléphone** : Ex. "0612345678"
   - **Date début** : Ex. "2026-01-20 14:00"
   - **Date fin** : Ex. "2026-01-20 17:00"
3. Cliquer sur "💾 Enregistrer"
4. **Résultat attendu** :
   - Réservation créée
   - Statut "EnAttente"
   - Notification créée pour le client

**Vérifications** :
- ✅ La réservation est créée avec les informations du client
- ✅ Le client peut la voir dans sa liste

---

### 8.4 Utiliser les filtres avancés (Admin)

**Étapes** :
1. Dans la liste des réservations, utiliser tous les filtres :
   - **Recherche** : Nom de client ou salle
   - **Statut** : EnAttente, Confirmee, Annulee
   - **Salle** : Sélectionner une salle spécifique
   - **Date début** : Date minimale
   - **Date fin** : Date maximale
2. Cliquer sur "Filtrer"
3. **Résultat attendu** : Liste filtrée selon tous les critères

**Vérifications** :
- ✅ Tous les filtres fonctionnent ensemble
- ✅ Les résultats sont corrects

---

## 9️⃣ WORKFLOW ADMIN - GESTION DES FACTURES

### 9.1 Voir toutes les factures (Admin)

**Étapes** :
1. Se connecter en tant qu'Admin
2. Cliquer sur "💰 Factures"
3. **Résultat attendu** : Liste de TOUTES les factures avec :
   - Numéro
   - Date d'émission
   - Réservation
   - Client
   - Montant
   - Statut
   - Actions (Détails, PDF, Marquer payée)

**Vérifications** :
- ✅ Toutes les factures sont visibles
- ✅ Les actions admin sont disponibles

---

### 9.2 Créer une facture pour une réservation confirmée

**Étapes** :
1. Aller dans les détails d'une réservation avec statut "Confirmee"
2. Vérifier qu'aucune facture n'existe déjà
3. Cliquer sur "Créer facture"
4. Vérifier les informations pré-remplies :
   - Numéro généré automatiquement (FACT-YYYY-NNNN)
   - Date d'émission (aujourd'hui)
   - Montant (calculé automatiquement)
5. Cliquer sur "💾 Enregistrer"
6. **Résultat attendu** :
   - Facture créée
   - Numéro unique généré
   - Notification créée pour le client
   - Redirection vers les détails de la facture

**Vérifications** :
- ✅ La facture est créée avec un numéro unique
- ✅ Le montant est correct
- ✅ Une notification est créée
- ✅ Le client peut voir sa facture

---

### 9.3 Marquer une facture comme payée

**Étapes** :
1. Dans la liste des factures, trouver une facture "En attente"
2. Cliquer sur "Marquer payée"
3. **Résultat attendu** :
   - Statut changé à "Payée"
   - Badge mis à jour (vert)
   - Notification créée pour le client

**Vérifications** :
- ✅ Le statut est mis à jour
- ✅ Une notification est créée
- ✅ Le badge change de couleur

---

### 9.4 Télécharger une facture en PDF (Admin)

**Étapes** :
1. Dans les détails d'une facture, cliquer sur "📄 Télécharger PDF"
2. **Résultat attendu** : PDF téléchargé avec toutes les informations

**Vérifications** :
- ✅ Le PDF se télécharge
- ✅ Le contenu est complet

---

## 🔟 WORKFLOW ADMIN - NOTIFICATIONS

### 10.1 Voir toutes les notifications (Admin)

**Étapes** :
1. Se connecter en tant qu'Admin
2. Cliquer sur "🔔 Notifications"
3. **Résultat attendu** : Liste de TOUTES les notifications (tous les clients)

**Vérifications** :
- ✅ Toutes les notifications sont visibles
- ✅ Les notifications de tous les clients sont affichées

---

## 1️⃣1️⃣ WORKFLOW - RAPPELS AUTOMATIQUES

### 11.1 Tester les rappels automatiques

**Objectif** : Vérifier que les rappels sont envoyés 24h avant une réservation

**Étapes** :
1. Créer une réservation confirmée avec une date début dans moins de 24h
   - Ex: Date actuelle : 2026-01-14 10:00
   - Créer réservation : Date début 2026-01-15 09:00 (dans 23h)
2. Attendre que le service de rappels s'exécute (vérifie toutes les heures)
3. **Résultat attendu** :
   - Notification de type "Rappel" créée automatiquement
   - Message : "Rappel : Votre réservation pour la salle [Nom] commence le [Date]"
   - Notification visible dans la liste des notifications du client

**Vérifications** :
- ✅ La notification de rappel est créée
- ✅ Elle apparaît dans les notifications du client
- ✅ Un seul rappel par jour (pas de doublons)

**Note** : Le service s'exécute automatiquement toutes les heures. Pour tester rapidement, vous pouvez créer une réservation avec une date très proche.

---

## 1️⃣2️⃣ WORKFLOW - TEST DE SÉCURITÉ

### 12.1 Tester l'accès non autorisé

**Objectif** : Vérifier que les clients ne peuvent pas accéder aux fonctionnalités admin

**Étapes** :
1. Se connecter en tant que Client
2. Essayer d'accéder directement à l'URL : `/Salles` (gestion admin)
3. **Résultat attendu** : Redirection vers "/Account/AccessDenied" ou erreur 403

**Vérifications** :
- ✅ L'accès est refusé
- ✅ Message d'erreur approprié affiché

---

### 12.2 Tester l'accès aux données d'autres clients

**Objectif** : Vérifier qu'un client ne peut pas voir les réservations d'un autre client

**Étapes** :
1. Se connecter en tant que Client A
2. Créer une réservation
3. Se déconnecter
4. Se connecter en tant que Client B
5. Essayer d'accéder à l'URL de la réservation du Client A : `/Reservations/Details/[Id]`
6. **Résultat attendu** : Accès refusé (Forbid)

**Vérifications** :
- ✅ L'accès est refusé
- ✅ Le client B ne voit que ses propres réservations

---

### 12.3 Tester l'accès aux factures d'autres clients

**Étapes** :
1. Se connecter en tant que Client A
2. Noter l'ID d'une facture
3. Se déconnecter
4. Se connecter en tant que Client B
5. Essayer d'accéder à l'URL : `/Factures/Details/[IdFactureClientA]`
6. **Résultat attendu** : Accès refusé

**Vérifications** :
- ✅ L'accès est refusé
- ✅ Le client B ne voit que ses propres factures

---

## 1️⃣3️⃣ WORKFLOW - TEST DES VALIDATIONS

### 13.1 Tester la validation des dates

**Étapes** :
1. Créer une réservation avec :
   - Date début : 2026-01-20 10:00
   - Date fin : 2026-01-20 09:00 (avant la date début)
2. Cliquer sur "Enregistrer"
3. **Résultat attendu** : Message d'erreur "La date de fin doit être postérieure à la date de début"

**Vérifications** :
- ✅ La validation fonctionne
- ✅ Le message d'erreur est clair

---

### 13.2 Tester la validation des champs requis

**Étapes** :
1. Essayer de créer une réservation sans remplir tous les champs obligatoires
2. Cliquer sur "Enregistrer"
3. **Résultat attendu** : Messages d'erreur pour chaque champ manquant

**Vérifications** :
- ✅ Les validations fonctionnent
- ✅ Les messages sont clairs

---

## 📊 CHECKLIST DE TEST COMPLÈTE

### ✅ Authentification
- [ ] Inscription d'un nouveau client
- [ ] Connexion avec compte existant
- [ ] Connexion admin
- [ ] Déconnexion
- [ ] "Se souvenir de moi" fonctionne

### ✅ Consultation publique
- [ ] Liste des salles (sans connexion)
- [ ] Détails d'une salle (sans connexion)
- [ ] Planning (sans connexion)
- [ ] Filtres du planning

### ✅ Réservations (Client)
- [ ] Créer une réservation
- [ ] Voir ses réservations
- [ ] Modifier une réservation
- [ ] Annuler une réservation
- [ ] Voir les détails
- [ ] Validation des conflits
- [ ] Calcul automatique du prix
- [ ] Filtres de recherche

### ✅ Factures (Client)
- [ ] Voir ses factures
- [ ] Voir les détails d'une facture
- [ ] Télécharger PDF
- [ ] Filtres de recherche

### ✅ Notifications (Client)
- [ ] Voir ses notifications
- [ ] Marquer comme lue
- [ ] Marquer toutes comme lues
- [ ] Voir les détails
- [ ] Filtres de recherche
- [ ] Types de notifications (Reservation, Facture, Rappel)

### ✅ Gestion Admin - Salles
- [ ] Voir toutes les salles
- [ ] Créer une salle
- [ ] Modifier une salle
- [ ] Supprimer une salle
- [ ] Voir les détails avec réservations

### ✅ Gestion Admin - Réservations
- [ ] Voir toutes les réservations
- [ ] Confirmer une réservation
- [ ] Créer une réservation pour un client
- [ ] Modifier n'importe quelle réservation
- [ ] Filtres avancés

### ✅ Gestion Admin - Factures
- [ ] Voir toutes les factures
- [ ] Créer une facture pour une réservation confirmée
- [ ] Marquer comme payée
- [ ] Télécharger PDF
- [ ] Filtres de recherche

### ✅ Notifications Admin
- [ ] Voir toutes les notifications
- [ ] Filtres de recherche

### ✅ Rappels automatiques
- [ ] Notification créée 24h avant une réservation
- [ ] Pas de doublons

### ✅ Sécurité
- [ ] Client ne peut pas accéder à la gestion des salles
- [ ] Client ne peut pas voir les réservations d'autres clients
- [ ] Client ne peut pas voir les factures d'autres clients
- [ ] Client ne peut pas modifier les réservations d'autres clients

### ✅ Validations
- [ ] Validation des dates (fin > début)
- [ ] Validation des champs requis
- [ ] Validation des conflits de réservation
- [ ] Validation des emails

---

## 🐛 Dépannage

### Problème : Migration échoue
**Solution** :
```bash
dotnet ef database update --project Coworking.Infrastructure --startup-project Coworking.Web
```

### Problème : Admin par défaut non créé
**Solution** : Vérifier que `RoleInitializer` s'exécute au démarrage dans `Program.cs`

### Problème : Rappels ne fonctionnent pas
**Solution** : Vérifier que le `ReminderBackgroundService` est enregistré dans `Program.cs`

### Problème : PDF ne se génère pas
**Solution** : Vérifier que le package `QuestPDF` est installé et que la licence est configurée

---

## 📝 Notes importantes

1. **Rappels automatiques** : Le service s'exécute toutes les heures. Pour tester rapidement, créez une réservation avec une date très proche.

2. **Calcul du prix** : Le montant est calculé automatiquement : `(DateFin - DateDebut) × PrixHoraire`

3. **Numérotation des factures** : Format `FACT-YYYY-NNNN` (ex: FACT-2026-0001)

4. **Notifications automatiques** : Créées lors de :
   - Création de réservation
   - Confirmation de réservation
   - Génération de facture
   - Paiement de facture
   - Rappel 24h avant réservation

5. **Statuts de réservation** :
   - `EnAttente` : En attente de confirmation
   - `Confirmee` : Confirmée par l'admin
   - `Annulee` : Annulée

---

## 🎓 Pour votre rapport

Ce guide couvre tous les workflows nécessaires pour tester l'application complète. Vous pouvez suivre chaque workflow étape par étape et documenter les résultats dans votre rapport.

**Points clés à documenter** :
- ✅ Tous les workflows fonctionnent correctement
- ✅ La séparation des rôles est respectée
- ✅ Les validations fonctionnent
- ✅ Les notifications automatiques sont générées
- ✅ Les PDFs sont générés correctement
- ✅ Les filtres et recherches fonctionnent
- ✅ La sécurité est assurée

---

**Bon test ! 🚀**

