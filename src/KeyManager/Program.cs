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
            PrepareApplication();
            PrintMenu();
        }

        static void PrepareApplication()
        {
            _databaseConfig = new DatabaseConfig()
            {
                Filename = "Keys.sqlite" //@TODO: Prompt user for this filename
            };

            _databaseBootstrap = new DatabaseBootstrap(_databaseConfig);
            _keyRecordRepo = new KeyRecordRepository(_databaseConfig);

            _databaseBootstrap.Setup();
        }

        static void PrintMenu()
        {
            Console.WriteLine("Welcome to the Key Manager");

            while (active) {
                Console.WriteLine("1.) List All Keys");
                Console.WriteLine("2.) Add a Key");
                Console.WriteLine("3.) Edit a Key");
                Console.WriteLine("4.) Delete a Key");
                Console.WriteLine("7.) List Duplicate Keys");
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
                        EditKey();
                        break;
                    case "4":
                        DeleteKey();
                        break;
                    case "7":
                        PrintDuplicatedKeys();
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

            if (keys.Any())
            {
                foreach (var key in keys)
                {
                    Console.WriteLine($"ID: {key.Id.ToString().PadRight(keys.Select(x => x.Id.ToString().Length).Max())} | Name: {key.Name.PadRight(keys.Select(x => x.Name.Length).Max())} | Key: {key.Key}");
                    if (!string.IsNullOrWhiteSpace(key.Note))
                        Console.WriteLine("- Note: " + key.Note);
                }
            }
            else
            {
                Console.WriteLine("No records found...");
            }
            Console.WriteLine();
        }

        static void PrintDuplicatedKeys()
        {
            var keys = _keyRecordRepo.FetchDuplicatedKey();

            if (keys.Any())
            {
                foreach (var key in keys)
                {
                    Console.WriteLine($"ID: {key.Id.ToString().PadRight(keys.Select(x => x.Id.ToString().Length).Max())} | Name: {key.Name.PadRight(keys.Select(x => x.Name.Length).Max())} | Key: {key.Key}");
                    if (!string.IsNullOrWhiteSpace(key.Note))
                        Console.WriteLine("- Note: " + key.Note);
                }
            }
            else
            {
                Console.WriteLine("No records found...");
            }
            Console.WriteLine();
        }

        static void EditKey()
        {
            Console.Write("Enter ID of Key Record: ");
            if (!int.TryParse(Console.ReadLine(), out int id))
            {
                Console.WriteLine("Input Invalid.");
            }
            else
            {
                KeyRecord record = _keyRecordRepo.GetSingle(id);

                if (record == null)
                {
                    Console.WriteLine("Record not found");
                }
                else 
                {
                    string input;
                    Console.Write($"Name ({record.Name}): ");
                    input = Console.ReadLine();
                    if (!string.IsNullOrWhiteSpace(input))
                        record.Name = input;

                    Console.Write($"Key ({record.Key}): ");
                    input = Console.ReadLine();
                    if (!string.IsNullOrWhiteSpace(input))
                        record.Key = input;

                    Console.Write($"Note: ");
                    input = Console.ReadLine();
                    if (!string.IsNullOrWhiteSpace(input))
                        record.Note = input;

                    _keyRecordRepo.Update(record);
                    Console.WriteLine("Record Updated.");
                }
            }
            Console.WriteLine();
        }

        static void AddAKey()
        {
            bool loop = true;
            while (loop)
            {
                var key = new KeyRecord();
                
                Console.Write("Name: ");
                key.Name = ReadNonEmptyLine();
                Console.Write("Key: ");
                key.Key = ReadNonEmptyLine();
                Console.Write("Note: ");
                key.Note = Console.ReadLine();

                _keyRecordRepo.Create(key);

                Console.Write("Record Created. Add another? (Y) ");
                if (Console.ReadLine().ToUpper() != "Y")
                    loop = false;

                Console.WriteLine();
            }
        }

        static void DeleteKey()
        {
            Console.Write("Enter the ID of the record to Delete: ");
            var id = Console.ReadLine();
            if (int.TryParse(id, out int value))
            {
                _keyRecordRepo.Delete(value);
                Console.WriteLine("Record Deleted.");
            }
            else 
                Console.WriteLine("Invalid Input. Abandoning....");

            Console.WriteLine();
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
