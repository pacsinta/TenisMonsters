using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UIElements;

namespace Assets.Scripts.Networking
{
    public static class SecureStore
    {
        // This is a conatant password to create a constant salt for the backend encryption
        // The reason for this is to don't send a raw password through the network
        // Please change the password before using it
        // The password has to be a valid base64 string
        const string encryptionPassword = "testpasswordshouldbechanged+";

        public static string CreateHashWithConstSalt(string password)
        {
            byte[] encyptionConsSalt;
            try
            {
                encyptionConsSalt = Convert.FromBase64String(encryptionPassword);
            }
            catch (FormatException)
            {
                Debug.Assert(false, "The encryption password is not a valid base64 string");
                return null;
            }
            
            byte[] hashed = new Rfc2898DeriveBytes(password, encyptionConsSalt, 10000, HashAlgorithmName.SHA256).GetBytes(32);
            string result = Convert.ToBase64String(hashed);
            return result;
        }
        public static void SavePassword(string name, string password)
        {
            string hashed = CreateHashWithConstSalt(password);
            PlayerPrefs.SetString("hash-"+name, hashed);
        }
        public static string GetHashedPassword(string name)
        {
            return PlayerPrefs.GetString("hash-" + name);
        }

        public static bool SecureCheck(string name, string password)
        {
            if (password.Any(c => !char.IsLetterOrDigit(c))) return false;
            if (!PlayerPrefs.HasKey(name)) return true; // If the key is not found, then it is a new user

            string storedHash = GetHashedPassword(name);
            string testHash = CreateHashWithConstSalt(password);
            return CryptographicOperations.FixedTimeEquals(Encoding.UTF8.GetBytes(storedHash), Encoding.UTF8.GetBytes(testHash));
        }
    }
}