using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;
using Newtonsoft.Json;
using System.Security.Cryptography;

namespace GuessingGame
{
    static class Utils
    {
        private static string dataDir = Path.Combine(Directory.GetCurrentDirectory(), ".UserData");
        private static string filePath = Path.Combine(dataDir, "users.json");

        // AES Key
        private static readonly byte[] Key = Encoding.UTF8.GetBytes("1234567890123456");
        private static readonly byte[] IV = Encoding.UTF8.GetBytes("6543210987654321");

        // valid input between [min, max]
        public static int GetValidInput(string prompt, int min, int max)
        {
            int value;
            while (true)
            {
                Console.Write(prompt);
                string? input = Console.ReadLine();
                // checking input if null
                if (input == null)
                {
                    continue;
                }

                // checking the valid input
                if (int.TryParse(input, out value) && value >= min && value <= max)
                    return value;

                Console.WriteLine($"❌ Invalid input! Enter a number between {min} and {max}.");
            }
        }

        // CTRL+C Exiting program
        public static void OnExit(object? sender, ConsoleCancelEventArgs e)
        {
            SaveUsers(Program.users); // Save user data before quit1
            e.Cancel = true;
            Environment.Exit(0); // exit enviroment
        }

        // load User DATA
        public static List<User> LoadUsers()
        {
            if (!Directory.Exists(dataDir))
                Directory.CreateDirectory(dataDir);

            if (!File.Exists(filePath))
                return new List<User>();

            try
            {
                using Aes aes = Aes.Create();
                aes.Key = Key;
                aes.IV = IV;

                using var fs = new FileStream(filePath, FileMode.Open);
                using var cs = new CryptoStream(fs, aes.CreateDecryptor(), CryptoStreamMode.Read);
                using var sr = new StreamReader(cs);
                string json = sr.ReadToEnd();

                return JsonConvert.DeserializeObject<List<User>>(json) ?? new List<User>();
            }
            catch
            {
                Console.WriteLine("⚠️ Failed to decrypt user data. File may be corrupted.");
                return new List<User>();
            }
        }

        // Save user DATA
        public static void SaveUsers(List<User> users)
        {
            if (!Directory.Exists(dataDir)) // checking if User Data exists
                Directory.CreateDirectory(dataDir);

            string json = JsonConvert.SerializeObject(users, Formatting.Indented); // create user data json

            using Aes aes = Aes.Create(); // create AES Key
            aes.Key = Key;
            aes.IV = IV;


            using var fs = new FileStream(filePath, FileMode.Create);
            using var cs = new CryptoStream(fs, aes.CreateEncryptor(), CryptoStreamMode.Write);
            using var sw = new StreamWriter(cs);
            sw.Write(json);
        }

        public static string ReadPassword(string prompt)
        {
            Console.WriteLine(prompt);
            string password = "";
            ConsoleKeyInfo key;

            do
            {
                key = Console.ReadKey(true); // true = not print

                if (key.Key == ConsoleKey.Backspace && password.Length > 0)
                {
                    password = password.Substring(0, password.Length - 1);
                    Console.Write("\b \b");
                }
                else if (!char.IsControl(key.KeyChar))
                {
                    password += key.KeyChar;
                    Console.Write("");
                }
            }
            while (key.Key != ConsoleKey.Enter);
            Console.WriteLine("");
            return password.Trim();
        }
    }
}
