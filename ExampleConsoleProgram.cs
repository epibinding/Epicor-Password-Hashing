using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace ConsoleApplication2
{
    class Program
    {
        static void Main(string[] args)
        {
            // Create a New Hash with password manager
            Console.WriteLine(ComputeHash("manager"));

            // Lets Validate all these possible hashes against Password manager (one-way hash)
            Console.WriteLine(VerifyHash("manager", "46uIY6/nQjHL5mX1KeE/7NtEXD3MIOblGxpVRH5ZWXNORGsNwT3WHg=="));
            Console.WriteLine(VerifyHash("manager", "JIzKviG6ZG5rE52KOeKEg4AvPnU72FOUQ0y7y/R8h8GZNQaV2G+GHw=="));
            Console.WriteLine(VerifyHash("manager", "tjk/TfHesmjnhq7NcUbCkfQKJnqfV7Q2mJRG6hwYRD8xGIofA6tGIQ=="));
            Console.WriteLine(VerifyHash("WRONG_PASSWORD", "tjk/TfHesmjnhq7NcUbCkfQKJnqfV7Q2mJRG6hwYRD8xGIofA6tGIQ=="));
            Console.ReadLine();
        }

        public static string ComputeHash(string password, byte[] saltBytes = null)
        {
            HashAlgorithm algorithm = new SHA256Managed();

            if (saltBytes == null)
            {
                saltBytes = new byte[8];
                new RNGCryptoServiceProvider().GetNonZeroBytes(saltBytes);
            }

            byte[] bytes = Encoding.UTF8.GetBytes(password);
            byte[] buffer = new byte[bytes.Length + saltBytes.Length];
            for (int i = 0; i < bytes.Length; i++)
                buffer[i] = bytes[i];

            for (int j = 0; j < saltBytes.Length; j++)
                buffer[bytes.Length + j] = saltBytes[j];

            byte[] buffer2 = algorithm.ComputeHash(buffer);
            byte[] combinedArr = new byte[buffer2.Length + saltBytes.Length];
            for (int k = 0; k < buffer2.Length; k++)
                combinedArr[k] = buffer2[k];

            for (int m = 0; m < saltBytes.Length; m++)
                combinedArr[buffer2.Length + m] = saltBytes[m];

            return Convert.ToBase64String(combinedArr);
        }

        public static bool VerifyHash(string password, string hash)
        {
            byte[] saltBytes = new byte[8];
            byte[] buffer = Convert.FromBase64String(hash);
            int x = 0x100 / 8; // SHA256

            if (buffer.Length < x)
                return false;

            for (int i = 0; i < saltBytes.Length; i++)
                saltBytes[i] = buffer[x + i];

            return (hash == ComputeHash(password, saltBytes));
        }
    }
}
