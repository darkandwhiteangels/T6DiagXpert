using System.Text.Json;
using Google.Cloud.Firestore;

namespace The6DiagXpert.Infrastructure.Firebase;

/// <summary>
/// Service pour gérer les opérations Firestore (synchronisation cloud).
/// </summary>
public class FirestoreService
{
    private readonly FirestoreDb? _firestoreDb;
    private readonly bool _isEnabled;

    public FirestoreService(string? projectId = null)
    {
        if (!string.IsNullOrEmpty(projectId))
        {
            try
            {
                _firestoreDb = FirestoreDb.Create(projectId);
                _isEnabled = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur initialisation Firestore: {ex.Message}");
                _isEnabled = false;
            }
        }
        else
        {
            _isEnabled = false;
        }
    }

    /// <summary>
    /// Vérifie si Firestore est activé.
    /// </summary>
    public bool IsEnabled => _isEnabled;

    /// <summary>
    /// Ajoute un document dans une collection.
    /// </summary>
    public async Task<string?> AddDocumentAsync<T>(string collectionPath, T data, CancellationToken cancellationToken = default) where T : class
    {
        if (!_isEnabled || _firestoreDb == null)
            return null;

        try
        {
            var collection = _firestoreDb.Collection(collectionPath);
            var docRef = await collection.AddAsync(data, cancellationToken);
            return docRef.Id;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erreur ajout document Firestore: {ex.Message}");
            return null;
        }
    }

    /// <summary>
    /// Met à jour un document existant.
    /// </summary>
    public async Task<bool> UpdateDocumentAsync<T>(string collectionPath, string documentId, T data, CancellationToken cancellationToken = default) where T : class
    {
        if (!_isEnabled || _firestoreDb == null)
            return false;

        try
        {
            var docRef = _firestoreDb.Collection(collectionPath).Document(documentId);
            await docRef.SetAsync(data, SetOptions.MergeAll, cancellationToken);
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erreur mise à jour document Firestore: {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// Supprime un document.
    /// </summary>
    public async Task<bool> DeleteDocumentAsync(string collectionPath, string documentId, CancellationToken cancellationToken = default)
    {
        if (!_isEnabled || _firestoreDb == null)
            return false;

        try
        {
            var docRef = _firestoreDb.Collection(collectionPath).Document(documentId);
            await docRef.DeleteAsync(cancellationToken: cancellationToken);
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erreur suppression document Firestore: {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// Récupère un document par son ID.
    /// </summary>
    public async Task<T?> GetDocumentAsync<T>(string collectionPath, string documentId, CancellationToken cancellationToken = default) where T : class
    {
        if (!_isEnabled || _firestoreDb == null)
            return null;

        try
        {
            var docRef = _firestoreDb.Collection(collectionPath).Document(documentId);
            var snapshot = await docRef.GetSnapshotAsync(cancellationToken);

            if (!snapshot.Exists)
                return null;

            return snapshot.ConvertTo<T>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erreur récupération document Firestore: {ex.Message}");
            return null;
        }
    }

    /// <summary>
    /// Récupère tous les documents d'une collection.
    /// </summary>
    public async Task<IEnumerable<T>> GetCollectionAsync<T>(string collectionPath, CancellationToken cancellationToken = default) where T : class
    {
        if (!_isEnabled || _firestoreDb == null)
            return Enumerable.Empty<T>();

        try
        {
            var collection = _firestoreDb.Collection(collectionPath);
            var snapshot = await collection.GetSnapshotAsync(cancellationToken);

            return snapshot.Documents
                .Where(d => d.Exists)
                .Select(d => d.ConvertTo<T>())
                .ToList();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erreur récupération collection Firestore: {ex.Message}");
            return Enumerable.Empty<T>();
        }
    }

    /// <summary>
    /// Récupère les documents filtrés par une condition.
    /// </summary>
    public async Task<IEnumerable<T>> QueryDocumentsAsync<T>(
        string collectionPath,
        string fieldPath,
        object value,
        CancellationToken cancellationToken = default) where T : class
    {
        if (!_isEnabled || _firestoreDb == null)
            return Enumerable.Empty<T>();

        try
        {
            var collection = _firestoreDb.Collection(collectionPath);
            var query = collection.WhereEqualTo(fieldPath, value);
            var snapshot = await query.GetSnapshotAsync(cancellationToken);

            return snapshot.Documents
                .Where(d => d.Exists)
                .Select(d => d.ConvertTo<T>())
                .ToList();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erreur requête Firestore: {ex.Message}");
            return Enumerable.Empty<T>();
        }
    }

    /// <summary>
    /// Synchronise un document avec Firestore (upsert).
    /// </summary>
    public async Task<bool> SyncDocumentAsync<T>(string collectionPath, string documentId, T data, CancellationToken cancellationToken = default) where T : class
    {
        if (!_isEnabled || _firestoreDb == null)
            return false;

        try
        {
            var docRef = _firestoreDb.Collection(collectionPath).Document(documentId);
            await docRef.SetAsync(data, SetOptions.MergeAll, cancellationToken);
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erreur synchronisation document Firestore: {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// Écoute les changements en temps réel sur une collection.
    /// </summary>
    public void ListenToCollection<T>(string collectionPath, Action<IEnumerable<T>> onDataChanged) where T : class
    {
        if (!_isEnabled || _firestoreDb == null)
            return;

        try
        {
            var collection = _firestoreDb.Collection(collectionPath);
            
            collection.Listen(snapshot =>
            {
                var documents = snapshot.Documents
                    .Where(d => d.Exists)
                    .Select(d => d.ConvertTo<T>())
                    .ToList();

                onDataChanged(documents);
            });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erreur écoute collection Firestore: {ex.Message}");
        }
    }

    /// <summary>
    /// Effectue un batch write (plusieurs opérations en une transaction).
    /// </summary>
    public async Task<bool> BatchWriteAsync(
        IEnumerable<(string collectionPath, string documentId, object data)> operations,
        CancellationToken cancellationToken = default)
    {
        if (!_isEnabled || _firestoreDb == null)
            return false;

        try
        {
            var batch = _firestoreDb.StartBatch();

            foreach (var (collectionPath, documentId, data) in operations)
            {
                var docRef = _firestoreDb.Collection(collectionPath).Document(documentId);
                batch.Set(docRef, data, SetOptions.MergeAll);
            }

            await batch.CommitAsync(cancellationToken);
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erreur batch write Firestore: {ex.Message}");
            return false;
        }
    }
}
