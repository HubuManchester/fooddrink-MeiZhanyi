using System.Net.Http.Json;
using System.Text.Json;
using FoodDrinkApp.Models;

namespace FoodDrinkApp.Services;

public static class FoodCatalogService
{
    private static readonly HttpClient HttpClient = new();

    private static readonly List<FoodItem> cachedItems = new()
    {
        new FoodItem
        {
            Id = "1",
            Name = "Grilled Chicken Salad",
            Category = "Lunch",
            Description = "Fresh garden salad with grilled chicken breast, cherry tomatoes, and balsamic dressing.",
            Calories = 350,
            Protein = 35,
            Carbs = 12,
            Fat = 18,
            AllergyNote = "None"
        },
        new FoodItem
        {
            Id = "2",
            Name = "Avocado Toast",
            Category = "Breakfast",
            Description = "Whole grain toast topped with smashed avocado, poached egg, and chili flakes.",
            Calories = 280,
            Protein = 12,
            Carbs = 24,
            Fat = 16,
            AllergyNote = "Gluten, Eggs"
        },
        new FoodItem
        {
            Id = "3",
            Name = "Green Smoothie",
            Category = "Beverage",
            Description = "Blend of spinach, banana, almond milk, and chia seeds.",
            Calories = 180,
            Protein = 6,
            Carbs = 32,
            Fat = 4,
            AllergyNote = "Tree nuts"
        },
        new FoodItem
        {
            Id = "4",
            Name = "Salmon Bowl",
            Category = "Dinner",
            Description = "Grilled salmon fillet with quinoa, steamed broccoli, and lemon herb sauce.",
            Calories = 520,
            Protein = 42,
            Carbs = 38,
            Fat = 22,
            AllergyNote = "Fish"
        },
        new FoodItem
        {
            Id = "5",
            Name = "Greek Yogurt Parfait",
            Category = "Snack",
            Description = "Creamy Greek yogurt layered with granola and fresh berries.",
            Calories = 220,
            Protein = 14,
            Carbs = 28,
            Fat = 6,
            AllergyNote = "Dairy, Gluten"
        }
    };

    public static async Task<FoodItem?> AddAsync(FoodItem item)
    {
        item.Id = Guid.NewGuid().ToString("N");

        if (MockApiConfig.IsConfigured)
        {
            try
            {
                var response = await HttpClient.PostAsJsonAsync(MockApiConfig.EndpointUrl, item);
                if (response.IsSuccessStatusCode)
                {
                    var created = await response.Content.ReadFromJsonAsync<FoodItem>();
                    if (created != null)
                    {
                        cachedItems.Add(created);
                        return created;
                    }
                }
            }
            catch
            {
            }
        }

        cachedItems.Add(item);
        return item;
    }

    public static async Task<IEnumerable<FoodItem>> SearchAsync(string? query)
    {
        if (MockApiConfig.IsConfigured)
        {
            try
            {
                var url = string.IsNullOrWhiteSpace(query)
                    ? MockApiConfig.EndpointUrl
                    : $"{MockApiConfig.EndpointUrl}?search={Uri.EscapeDataString(query)}";

                var items = await HttpClient.GetFromJsonAsync<List<FoodItem>>(url);
                if (items != null)
                {
                    return items;
                }
            }
            catch
            {
            }
        }

        if (string.IsNullOrWhiteSpace(query))
        {
            return cachedItems;
        }

        return cachedItems.Where(i =>
            i.Name.Contains(query, StringComparison.OrdinalIgnoreCase) ||
            i.Category.Contains(query, StringComparison.OrdinalIgnoreCase));
    }

    public static async Task<FoodItem?> GetByIdAsync(string id)
    {
        if (MockApiConfig.IsConfigured)
        {
            try
            {
                var item = await HttpClient.GetFromJsonAsync<FoodItem>(
                    $"{MockApiConfig.EndpointUrl.TrimEnd('/')}/{Uri.EscapeDataString(id)}");
                return item;
            }
            catch
            {
            }
        }

        return cachedItems.FirstOrDefault(i => i.Id == id);
    }

    public static async Task<IEnumerable<FoodItem>> GetAllAsync()
    {
        return await SearchAsync(null);
    }

    public static async Task<bool> DeleteAsync(string id)
    {
        if (MockApiConfig.IsConfigured)
        {
            try
            {
                var response = await HttpClient.DeleteAsync(
                    $"{MockApiConfig.EndpointUrl.TrimEnd('/')}/{Uri.EscapeDataString(id)}");

                if (response.IsSuccessStatusCode)
                {
                    var item = cachedItems.FirstOrDefault(x => x.Id == id);
                    if (item != null)
                    {
                        cachedItems.Remove(item);
                    }
                    return true;
                }
            }
            catch
            {
            }
        }

        var localItem = cachedItems.FirstOrDefault(x => x.Id == id);
        if (localItem != null)
        {
            cachedItems.Remove(localItem);
            return true;
        }
        return false;
    }
}
