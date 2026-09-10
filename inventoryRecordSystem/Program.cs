using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace inventoryRecordSystem
{
    internal class Program
    {
        static void Main(string[] args)
        {
            InventoryApp appInstance = new();

            // 1. Seed initial data
            Console.WriteLine("- Seeding sample inventory data...");
            appInstance.SeedSampleData();

            // 2. Persist to disk
            Console.WriteLine("\n- Saving inventory data to disk...");
            appInstance.SaveData();

            // 3. Clear memory and simulate a fresh session
            Console.WriteLine("\n\n\n- Clearing memory to simulate a fresh session...\n\n");
            appInstance.ResetMemory();
            // Displaying empty inventory to confirm reset
            appInstance.PrintAllItems();

            // 4. Reload from disk
            Console.WriteLine("\n\n\n- Loading data back from file...");
            appInstance.LoadData();

            // 5. Print loaded records
            Console.WriteLine("\nRECOVERED INVENTORY DIRECTORY\n-------------------------------------------------------------");
            appInstance.PrintAllItems();
        }
    }

   

    // immutable inventory record 
    public record InventoryItem(int Id, string Name, int Quantity, DateTime DateAdded) : IInventoryEntity
    {
        // formating records for better display
        public override string ToString() =>
            $"[Item ID: {Id}] {Name,-22} | Qty: {Quantity,-5} | Added: {DateAdded:yyyy-MM-dd}";
    }

    // marker interface for entity logging
    public interface IInventoryEntity
    {
        public int Id { get; }
    }

    // generic inventory logger
    public class InventoryLogger<T> where T : IInventoryEntity
    {
        // fields
        private List<T> _log = new();
        private readonly string _filePath;

        public InventoryLogger(string filePath)
        {
            _filePath = filePath;
        }

        // methods
        public void Add(T item)
        {
            _log.Add(item);
        }

        public List<T> GetAll()
        {
            return new List<T>(_log);
        }


        public void SaveToFile()
        {
            try
            {
                var options = new JsonSerializerOptions { WriteIndented = true };
                string json = JsonSerializer.Serialize(_log, options);

                using StreamWriter writer = new(_filePath);
                writer.Write(json);
            }
            catch (IOException ex)
            {
                Console.WriteLine($"[IO ERROR] Failed saving log to file: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Serialization failure: {ex.Message}");
            }
        }

        public void LoadFromFile()
        {
            try
            {
                if (!File.Exists(_filePath))
                {
                    Console.WriteLine($"[WARNING] Log file not found at '{_filePath}'. Initializing empty log.");
                    _log = new List<T>();
                    return;
                }

                using StreamReader reader = new(_filePath);
                string json = reader.ReadToEnd();

                var items = JsonSerializer.Deserialize<List<T>>(json);
                _log = items ?? new List<T>();
            }
            catch (IOException ex)
            {
                Console.WriteLine($"[IO ERROR] Failed reading file: {ex.Message}");
            }
            catch (JsonException ex)
            {
                Console.WriteLine($"[JSON ERROR] Failed deserializing file content: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[UNEXPECTED ERROR] {ex.Message}");
            }
        }
    }

    // integrated application class
    public class InventoryApp
    {
        // fields
        private InventoryLogger<InventoryItem> _logger = new("inventory_data.json");

        // methods
        public void SeedSampleData()
        {
            _logger.Add(new InventoryItem(5001, "Cannula", 150, DateTime.Today.AddDays(-14)));
            _logger.Add(new InventoryItem(5002, "Crepe Bandage", 80, DateTime.Today.AddDays(-1)));
            _logger.Add(new InventoryItem(5003, "Povidone Iodine", 35, DateTime.Today.AddDays(-5)));
            _logger.Add(new InventoryItem(5004, "Gauze", 50, DateTime.Today.AddDays(-2)));
        }

        public void SaveData()
        {
            _logger.SaveToFile();
        }

        public void ResetMemory()
        {
            // Reset reference to mimic fresh application instance
            _logger = new InventoryLogger<InventoryItem>("inventory_data.json");
        }

        public void LoadData()
        {
            _logger.LoadFromFile();
        }

        public void PrintAllItems()
        {
            var items = _logger.GetAll();

            if (items.Count == 0)
            {
                Console.WriteLine("  No inventory items currently in log.");
                return;
            }

            foreach (var item in items)
            {
                Console.WriteLine($"\n  - {item}");
            }
        }
    }
}