using System;
using System.Collections.Generic;

namespace GuessingGame
{
    class Program
    {
        public static List<User> users = new List<User>();

        static void Main()
        {
            Console.CancelKeyPress += new ConsoleCancelEventHandler(Utils.OnExit);

            users = Utils.LoadUsers();

            Console.Write("Enter username: ");
            string username = Console.ReadLine()!;
            string password = Utils.ReadPassword("Enter password: ");

            Var.currentUser = users.Find(u => u.Username == username);

            if (Var.currentUser == null)
            {
                // Register account
                Var.currentUser = new User { Username = username, Password = password, Score = 100 };
                users.Add(Var.currentUser);
                Console.WriteLine("✅ Account created! Starting with 100 points.");
            }
            else
            {
                // sign in
                if (Var.currentUser.Password != password)
                {
                    Console.WriteLine("❌ Wrong password!");
                    return;
                }
                Console.WriteLine($"👋 Welcome back, {username}! Current score: {Var.currentUser.Score}");
            }

            Var.point = Var.currentUser.Score;

            // Asking user to play the game or recharge
            while (true)
            {
                Console.WriteLine("\nWhat would you like to do?");
                Console.WriteLine("1. Start guessing game");
                Console.WriteLine("2. Recharge (10,000 VND = 1 point)");
                Console.WriteLine("3. Exit");
                string? option = Console.ReadLine();
                if (option == "1"){break;}
                else if (option == "2")
                {
                    int money = Utils.GetValidInput("Enter amount to recharge (10000 VNĐ): ", 10000, int.MaxValue);
                    if (money % 10000 != 0) // money must be multiple of 10k vnd
                    {
                        Console.WriteLine("❌ Amount must be a multiple of 10,000 VND.");
                        continue;
                    }

                    int addedPoints = money / 10000;

                    // Apply recharge without verification
                    Var.point += addedPoints;
                    Console.WriteLine($"💰 Successfully recharged {money} VND → {addedPoints} points.");
                    Console.WriteLine($"📌 Your current points: {Var.point}");
                    Var.currentUser.Score = Var.point;
                    Utils.SaveUsers(users);
                    Console.WriteLine("Goodbye!");
                    return;
                }
                else{Console.WriteLine("Invalid option, please try again.");}
            }


            // Guessing Game
            while (Var.attempts < Var.maxAttempts && Var.point > 0)
            {
                Console.WriteLine($"\nAttempts left: {Var.maxAttempts - Var.attempts}");
                Console.WriteLine($"Your points: {Var.point}");

                int bet = Utils.GetValidInput("Input points to bet: ", 1, Var.point);
                Var.guess = Utils.GetValidInput("Guess a number from 1-10: ", 1, 10);
                Var.attempts++;

                if (Var.guess == Var.RandNum) // If correct number guess
                {
                    Var.point += bet; // Update point bet
                    Console.WriteLine($"\nCongratulations! You guessed correctly and won {bet} points!");

                    // Asking user to continue
                    Console.WriteLine("Do you want to continue? (Y/n): ");
                    string? contInput = Console.ReadLine();
                    string cont = contInput != null ? contInput.Trim().ToLower() : string.Empty;
                    if (cont != "n" && cont != "no"){Var.attempts = 0;continue;} // reset user attempts and continue
                    else{break;}
                }
                else // if incorrect number guess
                {
                    Var.point -= bet; // Update point bet
                    Console.WriteLine($"\nIncorrect! You lost {bet} points.");
                    Console.WriteLine($"Hint: The number is {(Var.guess < Var.RandNum ? "bigger" : "smaller")} than {Var.guess}.");
                }
            }

            // Point updating
            Var.currentUser.Score = Var.point;
            Utils.SaveUsers(users);

            Console.WriteLine($"\nGame over! Your final points: {Var.point}");
        }
    }
}
