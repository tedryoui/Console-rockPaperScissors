using System;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;

namespace ConsoleApplication1
{
    internal class Program
    {
        public static void PrintByteArray(byte[] array)
        {
            for (int i = 0; i < array.Length; i++)
            {
                Console.Write($"{array[i]:X2}");
                if ((i % 4) == 3) Console.Write(" ");
            }
            Console.WriteLine();
        }

        private static void Menu(string[] menuPoints)
        {
            Console.WriteLine("Available moves:");
            for(int i = 1; i <= menuPoints.Length; i++)
                Console.WriteLine(i.ToString() + ". " + menuPoints[i-1]);
            Console.WriteLine("0. Exit");
        }

        private static int GetInt(string[] menuPoints)
        {
            Console.Write("Enter your move: ");
            int turn = InputNum(menuPoints.Length);
            
            return turn - 1;
        }
        
        private static int GenInt(string[] menuPoints, byte[] key)
        {
            Random rand = new Random();
            int turn = rand.Next(menuPoints.Length);

            HMACSHA256 hashObject = new HMACSHA256(key);
            var signature = hashObject.ComputeHash(Encoding.UTF8.GetBytes(menuPoints[turn]));

            Console.Write(menuPoints[turn] + " HMAC: ");
            PrintByteArray(signature);
            
            return turn;
        }

        private static void CheckForGameState(string[] menuPoints, int player, int computer)
        {
            
            int halfLength = menuPoints.Length / 2;
            
            if (player == computer)
                Console.WriteLine("Draw!");
            else if (player <= halfLength)
            {
                if ((computer <= player + halfLength && computer > player))
                    Console.WriteLine("You lose!");
                else
                    Console.WriteLine("You win!");
            }
            else if (player > halfLength)
            {
                if ((computer >= player - halfLength && computer < player))
                    Console.WriteLine("You win!");
                else
                    Console.WriteLine("You lose!");
            }
        }

        private static bool CheckForArguments(string[] menuPoints, out string e)
        {
            e = "";
            if (menuPoints.Length == 0)
            {
                e = "You did not write any param! Try 'test.exe A B C' instead of 'test.exe'";
                return false;
            }
            if (menuPoints.Length % 2 == 0) {
                e = "You`ve wrote even amount of params!";
                return false;
            }
            if (menuPoints.Length == 1) {
                e = "You`ve wrote one param! Write at least two more .";
                return false;
            }
            for (int i = 0; i < menuPoints.Length; i++)
                for (int j = i + 1; j < menuPoints.Length; j++)
                    if (String.CompareOrdinal(menuPoints[i], menuPoints[j]) == 0)
                    {
                        e = "You`ve wrote the same params. Change one of your '" + menuPoints[i] +
                            "' with another letter";
                        return false;
                    }
            return true;
        }

        private static int InputNum(int menuSize)
        {
            int num = menuSize + 1;
            
            while (num > menuSize)
            {
                try
                {
                    num = Convert.ToInt32(Console.ReadLine());
                }
                catch (Exception e)
                {
                    Console.WriteLine("Wrong! Enter integer!");
                    continue;
                }

                if(num > menuSize) Console.WriteLine("Wrong! Out of diapason of accessible nums!");
            }
            
            return num;
        }
        
        public static void Main(string[] args)
        {
            string e = "";
            if (!CheckForArguments(args, out e))
            {   
                Console.WriteLine("Wrong!\n" + e);
                return;
            }
            
            RNGCryptoServiceProvider CprytoRNG = new RNGCryptoServiceProvider();
            
            while(true)
            {
                Console.Write("\n\n");
                
                byte[] key128 = new byte[128];
                CprytoRNG.GetBytes(key128);
                int computerTurn = GenInt(args, key128);
                
                Menu(args);
                
                int playerTurn = GetInt(args);
                if (playerTurn == -1)
                {
                    Console.WriteLine("Exitted!");
                    return;
                } else 
                    Console.Write("Your move: " + args[playerTurn] + "\n");
                
                Console.Write("Computer move: " + args[computerTurn] + "\n");
                
                CheckForGameState(args, playerTurn, computerTurn);
                
                Console.Write("HMAC key: ");
                PrintByteArray(key128);
            }
        }
    }
}