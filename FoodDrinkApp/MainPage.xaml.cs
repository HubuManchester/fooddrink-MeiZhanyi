using FoodDrinkApp.Models;
using FoodDrinkApp.Services;

namespace FoodDrinkApp;

public partial class MainPage : ContentPage
{
    private List<FoodItem> _allItems = new();

    public MainPage()
    {
        InitializeComponent();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await LoadFoodItemsAsync();
    }

    private async Task LoadFoodItemsAsync()
    {
        var items = await FoodCatalogService.SearchAsync(null);
        _allItems = items.ToList();
        FoodCollection.ItemsSource = _allItems;
    }

    private async void OnSearchTextChanged(object? sender, TextChangedEventArgs e)
    {
        await FilterFoodItemsAsync(e.NewTextValue);
    }

    private async void OnSearchButtonPressed(object? sender, EventArgs e)
    {
        await FilterFoodItemsAsync(SearchFoodBar.Text);
    }

    private async Task FilterFoodItemsAsync(string? searchText)
    {
        var items = await FoodCatalogService.SearchAsync(searchText);
        FoodCollection.ItemsSource = items.ToList();
    }

    private async void OnRefreshing(object? sender, EventArgs e)
    {
        await LoadFoodItemsAsync();
        FoodRefreshView.IsRefreshing = false;
        SemanticScreenReader.Announce("Food list refreshed.");
    }

    private async void OnDetailsClicked(object? sender, EventArgs e)
    {
        if (sender is Button button && button.CommandParameter is string itemId)
        {
            await Shell.Current.GoToAsync($"FoodDetailPage?id={itemId}");
        }
    }

    // Swipe to delete
    private async void OnDeleteSwipeInvoked(object? sender, EventArgs e)
    {
        if (sender is SwipeItem swipeItem && swipeItem.CommandParameter is string itemId)
        {
            // Show confirmation dialog
            bool confirm = await DisplayAlert(
                "Delete Item",
                "Are you sure you want to delete this food item?",
                "Delete",
                "Cancel");

            if (confirm)
            {
                // Delete from service (async)
                var deleted = await FoodCatalogService.DeleteAsync(itemId);

                if (deleted)
                {
                    // Refresh the list
                    await LoadFoodItemsAsync();

                    // Accessibility announcement
                    SemanticScreenReader.Announce("Food item deleted successfully.");
                }
                else
                {
                    await DisplayAlert("Error", "Failed to delete the item.", "OK");
                }
            }
        }
    }

    // Swipe to edit
    private async void OnEditSwipeInvoked(object? sender, EventArgs e)
    {
        if (sender is SwipeItem swipeItem && swipeItem.CommandParameter is string itemId)
        {
            await DisplayAlert("Edit", $"Edit feature coming soon for item {itemId}!", "OK");
        }
    }
}
