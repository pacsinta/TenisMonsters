using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;
using UnityEngine.UIElements;

static class SecureStore
{
    // This is a conatant password to create a constant salt for the backend encryption
    // The reason for this is to don't send a raw password through the network
    // Please change the password before using it
    // The password has to be a valid base64 string
    const string encryptionPassword = "testpasswordshouldbechanged+";

    public static string CreateHashWithConstSalt(string password)
    {
        byte[] encyptionConsSalt = Convert.FromBase64String(encryptionPassword);
        byte[] hashed = new Rfc2898DeriveBytes(password, encyptionConsSalt, 10000, HashAlgorithmName.SHA256).GetBytes(32);
        string result = Convert.ToBase64String(hashed);
        return result;
    }
    public static void SavePassword(string key, string password)
    {
        string hashed = CreateHashWithConstSalt(password);
        PlayerPrefs.SetString(key, hashed);
    }
    public static string GetHashedPassword(string key)
    {
        return PlayerPrefs.GetString(key);
    }

    public static string GetHashPassword(string key)
    {
        return PlayerPrefs.GetString(key);
    }

    public static bool SecureCheck(string key, string password)
    {
        if (password.Any(c => !char.IsLetterOrDigit(c))) return false;
        if (!PlayerPrefs.HasKey(key)) return true; // If the key is not found, then it is a new user

        string storedHash = GetHashedPassword(key);
        string testHash = CreateHashWithConstSalt(password);
        return CryptographicOperations.FixedTimeEquals(Encoding.UTF8.GetBytes(storedHash), Encoding.UTF8.GetBytes(testHash));
    }
}
