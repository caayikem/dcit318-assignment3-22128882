using System;
using System.Collections.Generic;

namespace inventoryManagementSystem
{
    internal class Program
    {
        static void Main(string[] args)
        {
            WareHouseManager myManager = new();

            myManager.SeedData();

            Console.WriteLine("GROCERY INVENTORY\n-------------------------");
            myManager.PrintAllItems(myManager.Groceries);

            Console.WriteLine("\n\n");

            Console.WriteLine("\nELECTRONIC INVENTORY\n-------------------------");
            myManager.PrintAllItems(myManager.Electronics);

            Console.WriteLine("\n\n--------------------------------------------------------------------------------------------\n\n");

            // 1. Try adding a duplicate  item
            Console.WriteLine("\n\nAttempting to add a duplicate grocery item...");
            myManager.AddItem(myManager.Groceries, new GroceryItem(201, "Whole Milk", 40, DateTime.Today.AddDays(12)));

            // 2. Try removing a non-existent item
            Console.WriteLine("\n\nAttempting to remove a non-existent grocery item...");
            myManager.RemoveItemById(myManager.Groceries, 99999);

            // 3. Try updating with an invalid quantity
            Console.WriteLine("\n\nAttempting to update stock with a negative quantity...");
            myManager.IncreaseStock(myManager.Electronics, 101, -20);
        }
    }

    // marker interface for inventory items
    public interface IInventoryItem
    {
        int Id { get; }
        string Name { get; }
        int Quantity { get; set; }
    }

    // electronic item class
    public class ElectronicItem : IInventoryItem
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Quantity { get; set; }
        public string Brand { get; set; }
        public int WarrantyMonths { get; set; }

        public ElectronicItem(int id, string name, int quantity, string brand, int warrantyMonths)
        {
            Id = id;
            Name = name;
            Quantity = quantity;
            Brand = brand;
            WarrantyMonths = warrantyMonths;
        }

        public override string ToString() =>
            $"[ID: {Id}] {Name,-20} | Qty: {Quantity,-4} | Brand: {Brand,-12} | Warranty: {WarrantyMonths} months";
    }

    // grocery item class
    public class GroceryItem : IInventoryItem
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Quantity { get; set; }
        public DateTime ExpiryDate { get; set; }

        public GroceryItem(int id, string name, int quantity, DateTime expiryDate)
        {
            Id = id;
            Name = name;
            Quantity = quantity;
            ExpiryDate = expiryDate;
        }

        public override string ToString() =>
            $"[ID: {Id}] {Name,-20} | Qty: {Quantity,-4} | Expires: {ExpiryDate:yyyy-MM-dd}";
    }

   

    // generic inventory repository
    public class InventoryRepository<T> where T : IInventoryItem
    {
        // fields
        private readonly Dictionary<int, T> _items = new();

        // methods
        public void AddItem(T item)
        {
            if (_items.ContainsKey(item.Id))
            {
                throw new DuplicateItemException($"Item with ID '{item.Id}' already exists in this inventory.");
            }
            _items.Add(item.Id, item);
        }

        public T GetItemById(int id)
        {
            if (!_items.TryGetValue(id, out var item))
            {
                throw new ItemNotFoundException($"Item with ID '{id}' was not found.");
            }
            return item;
        }

        public void RemoveItem(int id)
        {
            if (!_items.Remove(id))
            {
                throw new ItemNotFoundException($"Cannot remove. ---Item with ID '{id}' was not found.");
            }
        }

        public List<T> GetAllItems()
        {
            return new List<T>(_items.Values);
        }

        public void UpdateQuantity(int id, int newQuantity)
        {
            if (newQuantity < 0)
            {
                throw new InvalidQuantityException($"Quantity cannot be negative: {newQuantity}");
            }

            var item = GetItemById(id);
            item.Quantity = newQuantity;
        }
    }

    // custom exceptions
    public class DuplicateItemException : Exception
    {
        public DuplicateItemException(string message) : base(message) { }
    }

    public class ItemNotFoundException : Exception
    {
        public ItemNotFoundException(string message) : base(message) { }
    }

    public class InvalidQuantityException : Exception
    {
        public InvalidQuantityException(string message) : base(message) { }
    }

    // warehouse manager class
    public class WareHouseManager
    {
        // fields
        private readonly InventoryRepository<ElectronicItem> _electronics = new();
        private readonly InventoryRepository<GroceryItem> _groceries = new();

        public InventoryRepository<ElectronicItem> Electronics => _electronics;
        public InventoryRepository<GroceryItem> Groceries => _groceries;

        // methods
        public void SeedData()
        {
            // Seed electronic items
            _electronics.AddItem(new ElectronicItem(101, "Latitude 5490", 12, "Dell", 24));
            _electronics.AddItem(new ElectronicItem(102, "4K TV", 8, "Samsung", 36));
            _electronics.AddItem(new ElectronicItem(103, "Camera", 25, "Sony", 12));

            // Seed grocery items
            _groceries.AddItem(new GroceryItem(201, "Whole Milk", 40, DateTime.Today.AddDays(10)));
            _groceries.AddItem(new GroceryItem(202, "Brown Rice 5kg", 60, DateTime.Today.AddYears(1)));
            _groceries.AddItem(new GroceryItem(203, "Greek Yogurt", 18, DateTime.Today.AddDays(14)));
        }

        public void PrintAllItems<T>(InventoryRepository<T> repo) where T : IInventoryItem
        {
            foreach (var item in repo.GetAllItems())
            {
                Console.WriteLine($"  - {item}");
            }
        }

        public void IncreaseStock<T>(InventoryRepository<T> repo, int id, int quantity) where T : IInventoryItem
        {
            try
            {
                var item = repo.GetItemById(id);
                int updatedQuantity = item.Quantity + quantity;
                repo.UpdateQuantity(id, updatedQuantity);
                Console.WriteLine($"\n [SUCCESS] Stock updated for ID {id}. New Quantity: {item.Quantity}");
            }
            catch (ItemNotFoundException ex)
            {
                Console.WriteLine($"\n [ERROR] {ex.Message}");
            }
            catch (InvalidQuantityException ex)
            {
                Console.WriteLine($"\n [ERROR] {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n [UNEXPECTED ERROR] {ex.Message}");
            }
        }

        public void RemoveItemById<T>(InventoryRepository<T> repo, int id) where T : IInventoryItem
        {
            try
            {
                repo.RemoveItem(id);
                Console.WriteLine($"\n [SUCCESS] Removed item with ID: {id} from {repo.GetType().Name} inventory.");
            }
            catch (ItemNotFoundException ex)
            {
                Console.WriteLine($"\n [ERROR] {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n [UNEXPECTED ERROR] {ex.Message}");
            }
        }

        // A method to add an item to the inventory repository with exception handling for duplicate items.
        public void AddItem<T>(InventoryRepository<T> repo, T item) where T : IInventoryItem
        {
            try
            {
                repo.AddItem(item);
                Console.WriteLine($"\n [SUCCESS] Added '{item.Name}' (ID: {item.Id}) to this inventory.");
            }
            catch (DuplicateItemException ex)
            {
                Console.WriteLine($"\n [ERROR] {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n [UNEXPECTED ERROR] {ex.Message}");
            }
        }
    }
}