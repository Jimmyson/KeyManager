using System;
using System.Linq;
using KeyManager.Database;
using KeyManager.Models;

namespace KeyManager
{
    class Program
    {
        private static bool active = true;
        private static DatabaseConfig _databaseConfig;
        private static DatabaseBootstrap _databaseBootstrap;
        private static KeyRecordRepository _keyRecordRepo;

        static void Main(string[] args)
        {
            _databaseConfig = new DatabaseConfig
            {
                Filename = "Keys.sqlite" //@TODO: Prompt user for this filename
            };

            _databaseBootstrap = new DatabaseBootstrap(_databaseConfig);
            _databaseBootstrap.Setup();
            _keyRecordRepo = new KeyRecordRepository(_databaseConfig);

            PrintMenu();
        }

        static void PrintMenu()
        {
            Console.WriteLine("Welcome to the Key Manager");

            while (active) {
                Console.WriteLine("1.) List All Keys");
                Console.WriteLine("2.) Add a Key");
                Console.WriteLine("3.) Delete a Key");
                Console.WriteLine("9.) Exit");
                Console.WriteLine();
                Console.Write("Enter an option: ");
                var input = Console.ReadLine();

                switch (input)
                {
                    case "1":
                        PrintAllKeys();
                        break;
                    case "2":
                        AddAKey();
                        break;
                    case "3":
                        DeleteKey();
                        break;
                    case "9":
                        active = false;
                        break;
                    default:
                        Console.WriteLine("Invalid Input... Try Again...");
                        Console.Write("Enter an option: ");
                        break;
                }
            }

            Console.WriteLine("Goodbye...");
        }

        static void PrintAllKeys()
        {
            var keys = _keyRecordRepo.GetAll();

            if (keys.Any()){
            foreach (var key in keys)
            {
                Console.WriteLine($"ID: {key.Id.ToString().PadRight(keys.Select(x => x.Id.ToString().Length).Max())} | Name: {key.Name.PadRight(keys.Select(x => x.Name.Length).Max())} | Key: {key.Key}");
                if (!string.IsNullOrWhiteSpace(key.Note))
                    Console.WriteLine(key.Note);
            }
            } else
            {
                Console.WriteLine("No records found...");
            }
            Console.WriteLine();
        }

        static void AddAKey()
        {
            var key = new KeyRecord();
            
            Console.Write("Name: ");
            key.Name = ReadNonEmptyLine();
            Console.Write("Key: ");
            key.Key = ReadNonEmptyLine();
            Console.Write("Note: ");
            key.Note = Console.ReadLine();

            _keyRecordRepo.Create(key);
            Console.WriteLine();
        }

        static void DeleteKey()
        {
            Console.Write("Enter the ID of the record to Delete: ");
            var id = Console.ReadLine();
            if (int.TryParse(id, out int value))
                _keyRecordRepo.Delete(value);
            else 
                Console.WriteLine("Invalid Input. Abandoning....");
        }

        static string ReadNonEmptyLine()
        {
            string input;
            do {
                input = Console.ReadLine();
            }
            while (String.IsNullOrWhiteSpace(input));
            
            return input;
        }
    }
}
