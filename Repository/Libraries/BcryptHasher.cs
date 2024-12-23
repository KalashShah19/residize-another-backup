using BCrypt.Net;

namespace Repository.Libraries;

public static class BcryptHasher {
    public static string HashPassword(string password) {
        return BCrypt.Net.BCrypt.HashPassword(password);
    }

    public static bool VerifyPassword(string password, string hash) {
        return BCrypt.Net.BCrypt.Verify(password, hash);
    }
}