using System;

namespace GuessingGame
{
    static class Var
    {
        public static int attempts = 0;
        public static int maxAttempts = 5;
        public static int point; 
        public static int guess;
        public static int RandNum = new Random().Next(1, 11);

        public static User? currentUser;
    }

    class User
    {
        public required string Username { get; set; }
        public required string Password { get; set; }
        public int Score { get; set; }
    }
}
