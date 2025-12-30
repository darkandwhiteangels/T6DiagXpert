using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Google.Cloud.Storage.V1;

namespace The6DiagXpert.Infrastructure.Firebase;

/// <summary>
/// Service pour gérer le stockage de fichiers dans Firebase Storage (Google Cloud Storage).
/// Dépend uniquement de Google.Cloud.Storage.V1.
/// </summary>
public class FirebaseStorageService
{
    private readonly StorageClient? _storageClient;
    private readonly string? _bucketName;
    private readonly bool _isEnabled;

    public FirebaseStorageService(string? bucketName = null)
    {
        if (!string.IsNullOrWhiteSpace(bucketName))
        {
            try
            {
                _storageClient = StorageClient.Create();
                _bucketName = bucketName;
                _isEnabled = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur initialisation Firebase Storage: {ex.Message}");
                _isEnabled = false;
            }
        }
        else
        {
            _isEnabled = false;
        }
    }

    /// <summary>
    /// Vérifie si Firebase Storage est activé.
    /// </summary>
    public bool IsEnabled => _isEnabled;

    /// <summary>
    /// Upload un fichier vers Firebase Storage.
    /// </summary>
    public async Task<string?> UploadFileAsync(
        string filePath,
        string destinationPath,
        string contentType = "application/octet-stream",
        CancellationToken cancellationToken = default)
    {
        if (!_isEnabled || _storageClient == null || string.IsNullOrWhiteSpace(_bucketName))
            return null;

        if (!File.Exists(filePath))
        {
            Console.WriteLine($"Fichier introuvable: {filePath}");
            return null;
        }

        try
        {
            await using var fileStream = File.OpenRead(filePath);

            await _storageClient.UploadObjectAsync(
                bucket: _bucketName,
                objectName: destinationPath,
                contentType: contentType,
                source: fileStream,
                cancellationToken: cancellationToken);

            return GetPublicUrl(destinationPath);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erreur upload fichier Firebase Storage: {ex.Message}");
            return null;
        }
    }

    /// <summary>
    /// Upload un fichier depuis un stream.
    /// </summary>
    public async Task<string?> UploadStreamAsync(
        Stream stream,
        string destinationPath,
        string contentType = "application/octet-stream",
        CancellationToken cancellationToken = default)
    {
        if (!_isEnabled || _storageClient == null || string.IsNullOrWhiteSpace(_bucketName))
            return null;

        try
        {
            await _storageClient.UploadObjectAsync(
                bucket: _bucketName,
                objectName: destinationPath,
                contentType: contentType,
                source: stream,
                cancellationToken: cancellationToken);

            return GetPublicUrl(destinationPath);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erreur upload stream Firebase Storage: {ex.Message}");
            return null;
        }
    }

    /// <summary>
    /// Upload un tableau de bytes.
    /// </summary>
    public async Task<string?> UploadBytesAsync(
        byte[] data,
        string destinationPath,
        string contentType = "application/octet-stream",
        CancellationToken cancellationToken = default)
    {
        if (!_isEnabled || _storageClient == null || string.IsNullOrWhiteSpace(_bucketName))
            return null;

        try
        {
            await using var stream = new MemoryStream(data);
            return await UploadStreamAsync(stream, destinationPath, contentType, cancellationToken);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erreur upload bytes Firebase Storage: {ex.Message}");
            return null;
        }
    }

    /// <summary>
    /// Télécharge un fichier depuis Firebase Storage.
    /// </summary>
    public async Task<bool> DownloadFileAsync(
        string sourcePath,
        string destinationPath,
        CancellationToken cancellationToken = default)
    {
        if (!_isEnabled || _storageClient == null || string.IsNullOrWhiteSpace(_bucketName))
            return false;

        try
        {
            var dir = Path.GetDirectoryName(destinationPath);
            if (!string.IsNullOrWhiteSpace(dir))
                Directory.CreateDirectory(dir);

            await using var fileStream = File.Create(destinationPath);
            await _storageClient.DownloadObjectAsync(
                bucket: _bucketName,
                objectName: sourcePath,
                destination: fileStream,
                cancellationToken: cancellationToken);

            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erreur téléchargement fichier Firebase Storage: {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// Télécharge un fichier dans un stream.
    /// </summary>
    public async Task<Stream?> DownloadToStreamAsync(
        string sourcePath,
        CancellationToken cancellationToken = default)
    {
        if (!_isEnabled || _storageClient == null || string.IsNullOrWhiteSpace(_bucketName))
            return null;

        try
        {
            var stream = new MemoryStream();
            await _storageClient.DownloadObjectAsync(
                bucket: _bucketName,
                objectName: sourcePath,
                destination: stream,
                cancellationToken: cancellationToken);

            stream.Position = 0;
            return stream;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erreur téléchargement stream Firebase Storage: {ex.Message}");
            return null;
        }
    }

    /// <summary>
    /// Supprime un fichier de Firebase Storage.
    /// </summary>
    public async Task<bool> DeleteFileAsync(
        string filePath,
        CancellationToken cancellationToken = default)
    {
        if (!_isEnabled || _storageClient == null || string.IsNullOrWhiteSpace(_bucketName))
            return false;

        try
        {
            await _storageClient.DeleteObjectAsync(
                bucket: _bucketName,
                objectName: filePath,
                cancellationToken: cancellationToken);

            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erreur suppression fichier Firebase Storage: {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// Liste tous les fichiers dans un dossier.
    /// </summary>
    public async Task<IReadOnlyList<string>> ListFilesAsync(
        string? prefix = null,
        CancellationToken cancellationToken = default)
    {
        if (!_isEnabled || _storageClient == null || string.IsNullOrWhiteSpace(_bucketName))
            return Array.Empty<string>();

        try
        {
            var objects = _storageClient.ListObjectsAsync(_bucketName, prefix);
            var fileNames = new List<string>();

            await foreach (var obj in objects.WithCancellation(cancellationToken))
            {
                if (!string.IsNullOrWhiteSpace(obj.Name))
                    fileNames.Add(obj.Name);
            }

            return fileNames;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erreur liste fichiers Firebase Storage: {ex.Message}");
            return Array.Empty<string>();
        }
    }

    /// <summary>
    /// Métadonnées simplifiées d'un objet Storage (pour éviter dépendance directe à Google.Apis.*).
    /// </summary>
    public sealed class StorageObjectMetadata
    {
        public string Name { get; init; } = string.Empty;
        public long? Size { get; init; }
        public string? ContentType { get; init; }
        public DateTimeOffset? Updated { get; init; }
        public string? Md5Hash { get; init; }
        public string? ETag { get; init; }
    }

    /// <summary>
    /// Obtient les métadonnées d'un fichier.
    /// </summary>
    public async Task<StorageObjectMetadata?> GetMetadataAsync(
        string filePath,
        CancellationToken cancellationToken = default)
    {
        if (!_isEnabled || _storageClient == null || string.IsNullOrWhiteSpace(_bucketName))
            return null;

        try
        {
            var obj = await _storageClient.GetObjectAsync(
                bucket: _bucketName,
                objectName: filePath,
                cancellationToken: cancellationToken);

            return new StorageObjectMetadata
            {
                Name = obj.Name ?? filePath,

                // ulong? -> long? (conversion explicite sécurisée)
                Size = obj.Size.HasValue
                    ? unchecked((long)obj.Size.Value)
                    : null,

                ContentType = obj.ContentType,

                // Propriété NON obsolète
                Updated = obj.UpdatedDateTimeOffset,

                Md5Hash = obj.Md5Hash,
                ETag = obj.ETag
            };
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erreur récupération métadonnées Firebase Storage: {ex.Message}");
            return null;
        }
    }


    /// <summary>
    /// Obtient l'URL publique d'un fichier.
    /// </summary>
    public string GetPublicUrl(string filePath)
    {
        if (!_isEnabled || string.IsNullOrWhiteSpace(_bucketName))
            return string.Empty;

        return $"https://storage.googleapis.com/{_bucketName}/{filePath}";
    }

    /// <summary>
    /// Copie un fichier vers un nouvel emplacement.
    /// </summary>
    public async Task<bool> CopyFileAsync(
        string sourcePath,
        string destinationPath,
        CancellationToken cancellationToken = default)
    {
        if (!_isEnabled || _storageClient == null || string.IsNullOrWhiteSpace(_bucketName))
            return false;

        try
        {
            await _storageClient.CopyObjectAsync(
                sourceBucket: _bucketName,
                sourceObjectName: sourcePath,
                destinationBucket: _bucketName,
                destinationObjectName: destinationPath,
                cancellationToken: cancellationToken);

            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erreur copie fichier Firebase Storage: {ex.Message}");
            return false;
        }
    }
}
