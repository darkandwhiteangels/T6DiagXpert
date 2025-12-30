using System.Net.Http.Json;
using System.Text.Json;
using The6DiagXpert.Shared.Contracts.Requests.Authentication;
using The6DiagXpert.Shared.Contracts.Responses.Authentication;
using The6DiagXpert.Shared.DTOs.Identity;

namespace The6DiagXpert.Infrastructure.Firebase;

/// <summary>
/// Service d'authentification Firebase
/// Gère l'authentification des utilisateurs via Firebase Auth
/// </summary>
public class FirebaseAuthService
{
    private readonly HttpClient _httpClient;
    private readonly FirebaseConfig _config;
    private readonly JsonSerializerOptions _jsonOptions;

    /// <summary>
    /// Constructeur
    /// </summary>
    public FirebaseAuthService(FirebaseConfig config)
    {
        _config = config ?? throw new ArgumentNullException(nameof(config));
        
        if (!_config.IsValid())
            throw new InvalidOperationException("Configuration Firebase invalide");

        _httpClient = new HttpClient
        {
            Timeout = TimeSpan.FromSeconds(_config.RequestTimeout)
        };

        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
    }

    /// <summary>
    /// Constructeur avec HttpClient injecté
    /// </summary>
    public FirebaseAuthService(HttpClient httpClient, FirebaseConfig config)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _config = config ?? throw new ArgumentNullException(nameof(config));

        if (!_config.IsValid())
            throw new InvalidOperationException("Configuration Firebase invalide");

        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
    }

    #region Authentification

    /// <summary>
    /// Connexion avec email et mot de passe
    /// </summary>
    public async Task<TokenResponse> SignInWithEmailPasswordAsync(LoginRequest request)
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        var url = $"{_config.GetAuthApiUrl()}:signInWithPassword?key={_config.ApiKey}";

        var payload = new
        {
            email = request.Email,
            password = request.Password,
            returnSecureToken = true
        };

        var response = await _httpClient.PostAsJsonAsync(url, payload);
        var content = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            var error = ParseFirebaseError(content);
            throw new Exception($"Échec de connexion Firebase : {error}");
        }

        var firebaseResponse = JsonSerializer.Deserialize<FirebaseAuthResponse>(content, _jsonOptions);
        
        if (firebaseResponse == null)
            throw new Exception("Réponse Firebase invalide");

        return new TokenResponse
        {
            AccessToken = firebaseResponse.IdToken,
            RefreshToken = firebaseResponse.RefreshToken,
            ExpiresIn = int.Parse(firebaseResponse.ExpiresIn),
            ExpiresAt = DateTime.UtcNow.AddSeconds(int.Parse(firebaseResponse.ExpiresIn)),
            UserId = firebaseResponse.LocalId,
            TokenId = Guid.NewGuid().ToString()
        };
    }

    /// <summary>
    /// Inscription avec email et mot de passe
    /// </summary>
    public async Task<TokenResponse> SignUpWithEmailPasswordAsync(RegisterRequest request)
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        var url = $"{_config.GetAuthApiUrl()}:signUp?key={_config.ApiKey}";

        var payload = new
        {
            email = request.Email,
            password = request.Password,
            returnSecureToken = true
        };

        var response = await _httpClient.PostAsJsonAsync(url, payload);
        var content = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            var error = ParseFirebaseError(content);
            throw new Exception($"Échec d'inscription Firebase : {error}");
        }

        var firebaseResponse = JsonSerializer.Deserialize<FirebaseAuthResponse>(content, _jsonOptions);
        
        if (firebaseResponse == null)
            throw new Exception("Réponse Firebase invalide");

        return new TokenResponse
        {
            AccessToken = firebaseResponse.IdToken,
            RefreshToken = firebaseResponse.RefreshToken,
            ExpiresIn = int.Parse(firebaseResponse.ExpiresIn),
            ExpiresAt = DateTime.UtcNow.AddSeconds(int.Parse(firebaseResponse.ExpiresIn)),
            UserId = firebaseResponse.LocalId,
            TokenId = Guid.NewGuid().ToString()
        };
    }

    /// <summary>
    /// Rafraîchir le token d'authentification
    /// </summary>
    public async Task<TokenResponse> RefreshTokenAsync(string refreshToken)
    {
        if (string.IsNullOrWhiteSpace(refreshToken))
            throw new ArgumentException("Le refresh token est requis", nameof(refreshToken));

        var url = $"https://securetoken.googleapis.com/v1/token?key={_config.ApiKey}";

        var payload = new
        {
            grant_type = "refresh_token",
            refresh_token = refreshToken
        };

        var response = await _httpClient.PostAsJsonAsync(url, payload);
        var content = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            var error = ParseFirebaseError(content);
            throw new Exception($"Échec de rafraîchissement du token : {error}");
        }

        var firebaseResponse = JsonSerializer.Deserialize<FirebaseRefreshResponse>(content, _jsonOptions);
        
        if (firebaseResponse == null)
            throw new Exception("Réponse Firebase invalide");

        return new TokenResponse
        {
            AccessToken = firebaseResponse.IdToken,
            RefreshToken = firebaseResponse.RefreshToken,
            ExpiresIn = int.Parse(firebaseResponse.ExpiresIn),
            ExpiresAt = DateTime.UtcNow.AddSeconds(int.Parse(firebaseResponse.ExpiresIn)),
            UserId = firebaseResponse.UserId,
            TokenId = Guid.NewGuid().ToString()
        };
    }

    /// <summary>
    /// Déconnexion (côté client uniquement)
    /// Firebase ne nécessite pas d'appel API pour la déconnexion
    /// </summary>
    public Task SignOutAsync()
    {
        // La déconnexion Firebase se fait côté client en supprimant les tokens
        // Pas d'appel API nécessaire
        return Task.CompletedTask;
    }

    #endregion

    #region Gestion du compte

    /// <summary>
    /// Obtenir les informations de l'utilisateur connecté
    /// </summary>
    public async Task<UserDto> GetCurrentUserAsync(string idToken)
    {
        if (string.IsNullOrWhiteSpace(idToken))
            throw new ArgumentException("Le token d'ID est requis", nameof(idToken));

        var url = $"{_config.GetAuthApiUrl()}:lookup?key={_config.ApiKey}";

        var payload = new
        {
            idToken = idToken
        };

        var response = await _httpClient.PostAsJsonAsync(url, payload);
        var content = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            var error = ParseFirebaseError(content);
            throw new Exception($"Échec de récupération de l'utilisateur : {error}");
        }

        var firebaseResponse = JsonSerializer.Deserialize<FirebaseUserResponse>(content, _jsonOptions);
        
        if (firebaseResponse?.Users == null || !firebaseResponse.Users.Any())
            throw new Exception("Utilisateur non trouvé");

        var firebaseUser = firebaseResponse.Users.First();

        return new UserDto
        {
            FirebaseUid = firebaseUser.LocalId,
            Email = firebaseUser.Email,
            EmailVerified = firebaseUser.EmailVerified,
            DisplayName = firebaseUser.DisplayName,
            PhotoUrl = firebaseUser.PhotoUrl,
            LastLoginAt = DateTime.UtcNow
        };
    }

    /// <summary>
    /// Envoyer un email de vérification
    /// </summary>
    public async Task SendEmailVerificationAsync(string idToken)
    {
        if (string.IsNullOrWhiteSpace(idToken))
            throw new ArgumentException("Le token d'ID est requis", nameof(idToken));

        var url = $"{_config.GetAuthApiUrl()}:sendOobCode?key={_config.ApiKey}";

        var payload = new
        {
            requestType = "VERIFY_EMAIL",
            idToken = idToken
        };

        var response = await _httpClient.PostAsJsonAsync(url, payload);

        if (!response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            var error = ParseFirebaseError(content);
            throw new Exception($"Échec d'envoi de l'email de vérification : {error}");
        }
    }

    /// <summary>
    /// Envoyer un email de réinitialisation de mot de passe
    /// </summary>
    public async Task SendPasswordResetEmailAsync(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("L'email est requis", nameof(email));

        var url = $"{_config.GetAuthApiUrl()}:sendOobCode?key={_config.ApiKey}";

        var payload = new
        {
            requestType = "PASSWORD_RESET",
            email = email
        };

        var response = await _httpClient.PostAsJsonAsync(url, payload);

        if (!response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            var error = ParseFirebaseError(content);
            throw new Exception($"Échec d'envoi de l'email de réinitialisation : {error}");
        }
    }

    /// <summary>
    /// Changer le mot de passe
    /// </summary>
    public async Task ChangePasswordAsync(string idToken, string newPassword)
    {
        if (string.IsNullOrWhiteSpace(idToken))
            throw new ArgumentException("Le token d'ID est requis", nameof(idToken));
        
        if (string.IsNullOrWhiteSpace(newPassword))
            throw new ArgumentException("Le nouveau mot de passe est requis", nameof(newPassword));

        var url = $"{_config.GetAuthApiUrl()}:update?key={_config.ApiKey}";

        var payload = new
        {
            idToken = idToken,
            password = newPassword,
            returnSecureToken = true
        };

        var response = await _httpClient.PostAsJsonAsync(url, payload);

        if (!response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            var error = ParseFirebaseError(content);
            throw new Exception($"Échec de changement de mot de passe : {error}");
        }
    }

    /// <summary>
    /// Supprimer le compte utilisateur
    /// </summary>
    public async Task DeleteAccountAsync(string idToken)
    {
        if (string.IsNullOrWhiteSpace(idToken))
            throw new ArgumentException("Le token d'ID est requis", nameof(idToken));

        var url = $"{_config.GetAuthApiUrl()}:delete?key={_config.ApiKey}";

        var payload = new
        {
            idToken = idToken
        };

        var response = await _httpClient.PostAsJsonAsync(url, payload);

        if (!response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            var error = ParseFirebaseError(content);
            throw new Exception($"Échec de suppression du compte : {error}");
        }
    }

    #endregion

    #region Helper Methods

    /// <summary>
    /// Parse une erreur Firebase
    /// </summary>
    private string ParseFirebaseError(string content)
    {
        try
        {
            var errorResponse = JsonSerializer.Deserialize<FirebaseErrorResponse>(content, _jsonOptions);
            return errorResponse?.Error?.Message ?? "Erreur inconnue";
        }
        catch
        {
            return "Erreur inconnue";
        }
    }

    #endregion

    #region Response Models

    private class FirebaseAuthResponse
    {
        public string IdToken { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
        public string ExpiresIn { get; set; } = string.Empty;
        public string LocalId { get; set; } = string.Empty;
    }

    private class FirebaseRefreshResponse
    {
        public string IdToken { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
        public string ExpiresIn { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
    }

    private class FirebaseUserResponse
    {
        public List<FirebaseUser> Users { get; set; } = new();
    }

    private class FirebaseUser
    {
        public string LocalId { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public bool EmailVerified { get; set; }
        public string DisplayName { get; set; } = string.Empty;
        public string PhotoUrl { get; set; } = string.Empty;
    }

    private class FirebaseErrorResponse
    {
        public FirebaseError? Error { get; set; }
    }

    private class FirebaseError
    {
        public string Message { get; set; } = string.Empty;
    }

    #endregion
}
