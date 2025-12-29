namespace The6DiagXpert.Core.Enums;

/// <summary>
/// Énumération des types de diagnostics immobiliers réglementaires
/// Conforme à la législation française en vigueur
/// </summary>
public enum DiagnosticType
{
    /// <summary>
    /// Diagnostic de Performance Énergétique (obligatoire)
    /// Validité : 10 ans
    /// Certification requise
    /// </summary>
    DPE = 1,

    /// <summary>
    /// Constat de Risque d'Exposition au Plomb (CREP)
    /// Biens construits avant 1949
    /// Validité : 1 an (vente) / 6 ans (location)
    /// </summary>
    Plomb = 2,

    /// <summary>
    /// Diagnostic Amiante
    /// Biens dont permis de construire avant 01/07/1997
    /// Validité : Illimitée si négatif / 3 ans si positif
    /// </summary>
    Amiante = 3,

    /// <summary>
    /// Diagnostic Électricité
    /// Installation de plus de 15 ans
    /// Validité : 3 ans (vente) / 6 ans (location)
    /// </summary>
    Electricite = 4,

    /// <summary>
    /// Diagnostic Gaz
    /// Installation de plus de 15 ans
    /// Validité : 3 ans (vente) / 6 ans (location)
    /// </summary>
    Gaz = 5,

    /// <summary>
    /// État des Risques et Pollutions (ERP)
    /// Anciennement ERNMT
    /// Validité : 6 mois
    /// </summary>
    ERP = 6,

    /// <summary>
    /// Diagnostic Termites
    /// Zones délimitées par arrêté préfectoral
    /// Validité : 6 mois
    /// </summary>
    Termites = 7,

    /// <summary>
    /// Mesurage Loi Carrez
    /// Copropriété uniquement
    /// Validité : Illimitée (sauf travaux modifiant la surface)
    /// </summary>
    LoiCarrez = 8,

    /// <summary>
    /// Mesurage Loi Boutin
    /// Location vide uniquement
    /// Validité : Illimitée (sauf travaux)
    /// </summary>
    LoiBoutin = 9,

    /// <summary>
    /// Diagnostic Assainissement Non Collectif
    /// Biens non raccordés au tout-à-l'égout
    /// Validité : 3 ans
    /// </summary>
    AssainissementNC = 10,

    /// <summary>
    /// État de l'Installation Intérieure d'Électricité (ERP/ERT)
    /// Établissements Recevant du Public
    /// Validité : Variable selon type d'établissement
    /// </summary>
    ElectriciteERP = 11,

    /// <summary>
    /// Diagnostic Technique Amiante (DTA)
    /// Parties communes des immeubles collectifs
    /// Validité : 3 ans
    /// </summary>
    AmianteDTA = 12,

    /// <summary>
    /// Audit Énergétique
    /// Obligatoire pour passoires thermiques (F-G) en vente
    /// Validité : 5 ans
    /// </summary>
    AuditEnergetique = 13,

    /// <summary>
    /// Diagnostic Bruit (Nuisances Aéroportuaires)
    /// Zones exposées au bruit des aéroports
    /// Validité : Illimitée
    /// </summary>
    Bruit = 14,

    /// <summary>
    /// Diagnostic Mérules
    /// Zones à risque définies par arrêté préfectoral
    /// Validité : 6 mois
    /// </summary>
    Merules = 15,

    /// <summary>
    /// État des Servitudes, Risques et Informations sur les Sols (ESRIS)
    /// Remplace ERP depuis 2018
    /// Validité : 6 mois
    /// </summary>
    ESRIS = 16,

    /// <summary>
    /// Contrôle Technique Construction
    /// Solidité, sécurité des ouvrages
    /// Validité : Variable
    /// </summary>
    ControleTechnique = 17,

    /// <summary>
    /// Diagnostic Radon
    /// Zones à potentiel radon niveau 3
    /// Validité : 10 ans
    /// </summary>
    Radon = 18,

    /// <summary>
    /// État Parasitaire (hors termites)
    /// Capricornes, vrillettes, etc.
    /// Validité : 6 mois
    /// </summary>
    EtatParasitaire = 19,

    /// <summary>
    /// Diagnostic Piscine
    /// Dispositifs de sécurité obligatoires
    /// Validité : Variable
    /// </summary>
    Piscine = 20
}
