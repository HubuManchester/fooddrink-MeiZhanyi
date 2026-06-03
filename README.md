# NutriBite - Food & Drink Tracker

A mobile application built with .NET MAUI for tracking food and drink intake. This app demonstrates mobile hardware capabilities, modern UI design, and cross-platform development.

## Features

### Core Functionality
- **Food Catalog**: Browse, search, and manage food items
- **Add Food Items**: Record new food and drinks with nutritional information
- **Food Details**: View detailed information including calories, protein, carbs, and fat
- **Search**: Filter food items by name or category
- **Swipe Gestures**: Swipe left to edit/delete, swipe right to quick delete

### Mobile Hardware Integration
The app demonstrates 7 mobile hardware features:

| Feature | Description |
|---------|-------------|
| **Camera** | Capture food photos using device camera |
| **Geolocation** | Record meal location with GPS coordinates |
| **Text-to-Speech** | Read help text aloud for accessibility |
| **Haptic Feedback** | Vibration feedback for user interactions |
| **Accelerometer** | Shake detection with motion sensing |
| **Flashlight** | Toggle camera flash for food photography lighting |
| **Compass** | Display device orientation and direction |

### Accessibility Features
- Semantic properties for screen readers
- SemanticScreenReader announcements
- Adjustable font scaling
- High contrast color support
- WCAG compliant design

## Technology Stack

- **.NET MAUI** - Cross-platform framework
- **C# 12** - Programming language
- **XAML** - UI markup language
- **Target Platforms**: Android, iOS, Windows, macOS

## Project Structure
FoodDrinkApp/
├── Models/
│   └── FoodItem.cs          # Food item data model
├── Services/
│   ├── FoodCatalogService.cs    # Food data management
│   ├── SpeechService.cs         # Text-to-speech service
│   ├── AccessibilityService.cs  # Accessibility helpers
│   └── MockApiConfig.cs         # API configuration
├── Pages/
│   ├── MainPage.xaml(.cs)       # Home page with food list
│   ├── AddItemPage.xaml(.cs)    # Add new food item
│   ├── FoodDetailPage.xaml(.cs) # Food details view
│   ├── HardwarePage.xaml(.cs)   # Hardware demo page
│   └── SettingsPage.xaml(.cs)   # App settings
├── AppShell.xaml                # Navigation shell
└── App.xaml                     # Application entry


## How to Run

### Prerequisites
- Visual Studio 2022 (v17.8 or later)
- .NET 9 SDK
- .NET MAUI workload installed

### Windows
1. Open `FoodDrinkApp.sln` in Visual Studio
2. Select **Windows Machine** as the target
3. Press **F5** to run

### Android Emulator
1. Open `FoodDrinkApp.sln` in Visual Studio
2. Go to **Tools** > **Android** > **Android Device Manager**
3. Create a new emulator (recommended: Pixel 5, API 34)
4. Start the emulator
5. Select the emulator in the device dropdown
6. Press **F5** to run

### Android Device
1. Enable **Developer Options** and **USB Debugging** on your Android device
2. Connect via USB cable
3. Select your device in Visual Studio
4. Press **F5** to run

## Design Features

- Modern Teal/Cyan color scheme
- Light and Dark theme support
- Card-based UI with shadows
- Gradient backgrounds
- Rounded corners (16px)
- Consistent spacing and typography

## Hardware Features Testing

Note: Some hardware features require a physical device:

| Feature | Windows | Android Emulator | Android Device |
|---------|---------|------------------|----------------|
| Camera | Limited | Yes | Yes |
| Location | Yes | Yes | Yes |
| Text-to-Speech | Yes | Yes | Yes |
| Haptic Feedback | No | Limited | Yes |
| Accelerometer | No | Yes (with controls) | Yes |
| Flashlight | No | No | Yes |
| Compass | No | Yes (with controls) | Yes |


## Author

**[Mei Zhanyi]**  
Student ID: [21906378] 
