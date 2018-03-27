using System;
using System.Security.Cryptography;
using System.Text;

namespace Common
{
    [Serializable]
    public class User
    {
        public string Username { get; set; }
        public string Pass { get; set; }
        public string Name { get; set; }

        public User(string username, string pass, string name)
        {
            Username = username;
            Pass = pass;
            Name = name;
        }

        public static byte[] GetHash(string inputString)
        {
            HashAlgorithm algorithm = MD5.Create();  //or use SHA256.Create();
            return algorithm.ComputeHash(Encoding.UTF8.GetBytes(inputString));
        }

        public static string GetHashString(string inputString)
        {
            StringBuilder sb = new StringBuilder();
            foreach (byte b in GetHash(inputString))
                sb.Append(b.ToString("X2"));

            return sb.ToString();
        }

    }
}
