using System;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;
using UnityEngine.UIElements;

static class SecureStore
{
    private static byte[] CreateSalt(int length)
    {
        byte[] salt = new byte[length];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(salt);
        }
        return salt;
    }

    private static string HashPassword(string password, byte[] salt)
    {
        byte[] hashed = new Rfc2898DeriveBytes(password, salt, 10000, HashAlgorithmName.SHA256).GetBytes(32);
        return Convert.ToBase64String(hashed);
    }

    public static void SecureSave(string key, string value)
    {
        if(!PlayerPrefs.HasKey(key))
        {
            byte[] salt = CreateSalt(16);
            PlayerPrefs.SetString(key, HashPassword(value, salt));
            PlayerPrefs.SetString(key + "_salt", Convert.ToBase64String(salt));
        }
    }

    public static string GetHashPassword(string value)
    {
        return PlayerPrefs.GetString(value);
    }


    public static bool SecureCheck(string key, string value)
    {
        if (!PlayerPrefs.HasKey(key)) return true; // If the key is not found, then it is a new user
        byte[] salt = Convert.FromBase64String(PlayerPrefs.GetString(key + "_salt"));
        string storedHash = PlayerPrefs.GetString(key);
        string testHash = HashPassword(value, salt);
        return CryptographicOperations.FixedTimeEquals(Encoding.UTF8.GetBytes(storedHash), Encoding.UTF8.GetBytes(testHash));
    }
}
