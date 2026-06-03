using FoodDrinkApp.Services;


namespace FoodDrinkApp;

public partial class HardwarePage : ContentPage
{
    private int feedbackTestCount;

    
    private bool isAccelerometerActive;
    private int shakeCount;
    private DateTime lastShakeTime = DateTime.MinValue;

    
    private bool isFlashlightOn;

    
    private bool isCompassActive;

    public HardwarePage()
    {
        InitializeComponent();
    }

    protected override void OnDisappearing()
    {
        
        SpeechService.Stop();

        
        if (isAccelerometerActive)
        {
            Accelerometer.Default.ReadingChanged -= OnAccelerometerReadingChanged;
            Accelerometer.Default.Stop();
            isAccelerometerActive = false;
        }

        
        if (isFlashlightOn)
        {
            Flashlight.Default.TurnOffAsync();
            isFlashlightOn = false;
        }

        
        if (isCompassActive)
        {
            Compass.Default.ReadingChanged -= OnCompassReadingChanged;
            Compass.Default.Stop();
            isCompassActive = false;
        }

        base.OnDisappearing();
    }

   
    private async void OnTakePhotoClicked(object? sender, EventArgs e)
    {
        try
        {
            SetStatus("Opening camera...");

            if (!MediaPicker.Default.IsCaptureSupported)
            {
                SetStatus("Camera not supported on this device.");
                return;
            }

            var photo = await MediaPicker.Default.CapturePhotoAsync();
            if (photo is null)
            {
                SetStatus("Photo capture was cancelled.");
                return;
            }

            var stream = await photo.OpenReadAsync();
            FoodPhoto.Source = ImageSource.FromStream(() => stream);

            SetStatus($"Photo captured: {photo.FileName}");
            SemanticScreenReader.Announce("Photo captured successfully.");
        }
        catch (PermissionException)
        {
            SetStatus("Camera permission was denied.");
        }
        catch (Exception ex)
        {
            SetStatus($"Camera error: {ex.Message}");
        }
    }

    
    private async void OnGetLocationClicked(object? sender, EventArgs e)
    {
        try
        {
            SetStatus("Fetching location...");

            var request = new GeolocationRequest(GeolocationAccuracy.Best, TimeSpan.FromSeconds(10));
            var location = await Geolocation.Default.GetLocationAsync(request);

            if (location is null)
            {
                SetStatus("Unable to determine location.");
                return;
            }

            CoordinateLabel.Text = $"Lat: {location.Latitude:F5}, Lon: {location.Longitude:F5}";

            var placemarks = await Geocoding.Default.GetPlacemarksAsync(location.Latitude, location.Longitude);
            var place = placemarks?.FirstOrDefault();

            if (place is not null)
            {
                LocationLabel.Text = $"{place.Locality}, {place.AdminArea}, {place.CountryName}";
                SetStatus("Location captured.");
                SemanticScreenReader.Announce($"Location is {place.Locality}, {place.CountryName}");
            }
            else
            {
                LocationLabel.Text = "Address not found.";
                SetStatus("Coordinates captured, but address lookup failed.");
            }
        }
        catch (FeatureNotSupportedException)
        {
            SetStatus("Location is not supported on this device.");
        }
        catch (PermissionException)
        {
            SetStatus("Location permission was denied.");
        }
        catch (Exception ex)
        {
            SetStatus($"Location error: {ex.Message}");
        }
    }

    
    private void OnToggleAccelerometerClicked(object? sender, EventArgs e)
    {
        try
        {
            if (!isAccelerometerActive)
            {
                
                Accelerometer.Default.ReadingChanged += OnAccelerometerReadingChanged;
                Accelerometer.Default.Start(SensorSpeed.UI);
                isAccelerometerActive = true;
                AccelerometerButton.Text = "Stop";
                AccelerometerLabel.Text = "Accelerometer is running. Shake your phone!";
                SetStatus("Accelerometer started.");
            }
            else
            {
                
                Accelerometer.Default.ReadingChanged -= OnAccelerometerReadingChanged;
                Accelerometer.Default.Stop();
                isAccelerometerActive = false;
                AccelerometerButton.Text = "Start";
                AccelerometerLabel.Text = "Accelerometer is off. Tap Start to begin.";
                SetStatus("Accelerometer stopped.");
            }
        }
        catch (FeatureNotSupportedException)
        {
            SetStatus("Accelerometer is not supported on this device.");
        }
        catch (Exception ex)
        {
            SetStatus($"Accelerometer error: {ex.Message}");
        }
    }

    private void OnAccelerometerReadingChanged(object? sender, AccelerometerChangedEventArgs e)
    {
        
        var data = e.Reading;
        double x = data.Acceleration.X;
        double y = data.Acceleration.Y;
        double z = data.Acceleration.Z;

        
        double acceleration = Math.Sqrt(x * x + y * y + z * z);

        
        if (acceleration > 2.5 && DateTime.Now - lastShakeTime > TimeSpan.FromMilliseconds(500))
        {
            lastShakeTime = DateTime.Now;
            shakeCount++;

            
            MainThread.BeginInvokeOnMainThread(() =>
            {
                ShakeCountLabel.Text = $"Shakes detected: {shakeCount}";
                AccelerometerLabel.Text = $"Shake detected! X:{x:F2} Y:{y:F2} Z:{z:F2}";
                HapticFeedback.Default.Perform(HapticFeedbackType.Click);
                SetStatus($"Shake #{shakeCount} detected!");
            });
        }
    }

    
    private async void OnToggleFlashlightClicked(object? sender, EventArgs e)
    {
        try
        {
            if (!isFlashlightOn)
            {
                
                await Flashlight.Default.TurnOnAsync();
                isFlashlightOn = true;
                FlashlightButton.Text = "Turn off";
                SetStatus("Flashlight is now on.");
            }
            else
            {
                
                await Flashlight.Default.TurnOffAsync();
                isFlashlightOn = false;
                FlashlightButton.Text = "Turn on";
                SetStatus("Flashlight is now off.");
            }
        }
        catch (FeatureNotSupportedException)
        {
            SetStatus("Flashlight is not supported on this device.");
        }
        catch (PermissionException)
        {
            SetStatus("Flashlight permission was denied.");
        }
        catch (Exception ex)
        {
            SetStatus($"Flashlight error: {ex.Message}");
        }
    }

    
    private void OnToggleCompassClicked(object? sender, EventArgs e)
    {
        try
        {
            if (!isCompassActive)
            {
                
                Compass.Default.ReadingChanged += OnCompassReadingChanged;
                Compass.Default.Start(SensorSpeed.UI);
                isCompassActive = true;
                CompassButton.Text = "Stop";
                CompassLabel.Text = "Compass is running...";
                SetStatus("Compass started.");
            }
            else
            {
                
                Compass.Default.ReadingChanged -= OnCompassReadingChanged;
                Compass.Default.Stop();
                isCompassActive = false;
                CompassButton.Text = "Start";
                CompassLabel.Text = "Compass is off. Tap Start to begin.";
                CompassDirectionLabel.Text = "Direction: --";
                SetStatus("Compass stopped.");
            }
        }
        catch (FeatureNotSupportedException)
        {
            SetStatus("Compass is not supported on this device.");
        }
        catch (Exception ex)
        {
            SetStatus($"Compass error: {ex.Message}");
        }
    }

    private void OnCompassReadingChanged(object? sender, CompassChangedEventArgs e)
    {
      
        double heading = e.Reading.HeadingMagneticNorth;

        
        string direction = heading switch
        {
            >= 337.5 or < 22.5 => "North (N)",
            >= 22.5 and < 67.5 => "Northeast (NE)",
            >= 67.5 and < 112.5 => "East (E)",
            >= 112.5 and < 157.5 => "Southeast (SE)",
            >= 157.5 and < 202.5 => "South (S)",
            >= 202.5 and < 247.5 => "Southwest (SW)",
            >= 247.5 and < 292.5 => "West (W)",
            >= 292.5 and < 337.5 => "Northwest (NW)",
            _ => "Unknown"
        };

        
        MainThread.BeginInvokeOnMainThread(() =>
        {
            CompassLabel.Text = $"Heading: {heading:F1}бу";
            CompassDirectionLabel.Text = $"Direction: {direction}";
        });
    }

    
    private async void OnReadHelpClicked(object? sender, EventArgs e)
    {
        SetStatus("Reading help text...");

        string helpText = "This page demonstrates mobile hardware capabilities. " +
                         "You can capture a food photo using the camera, " +
                         "record the meal location with GPS, " +
                         "detect phone shakes with the accelerometer, " +
                         "use the flashlight for better lighting, " +
                         "check your direction with the compass, " +
                         "and test haptic feedback.";

        await SpeechService.SpeakAsync(helpText);
        SetStatus("Speech finished.");
    }

    private void OnStopSpeechClicked(object? sender, EventArgs e)
    {
        SpeechService.Stop();
        SetStatus("Speech stopped.");
    }

    
    private void OnFeedbackClicked(object? sender, EventArgs e)
    {
        try
        {
            HapticFeedback.Default.Perform(HapticFeedbackType.LongPress);
            Vibration.Default.Vibrate(TimeSpan.FromMilliseconds(200));

            feedbackTestCount++;
            FeedbackCountLabel.Text = $"Haptic feedback tests: {feedbackTestCount}";
            SetStatus("Haptic feedback triggered.");
        }
        catch (FeatureNotSupportedException)
        {
            SetStatus("Haptic feedback is not supported.");
        }
        catch (Exception ex)
        {
            SetStatus($"Feedback error: {ex.Message}");
        }
    }

    
    private void SetStatus(string message)
    {
        HardwareStatusLabel.Text = message;
        SemanticScreenReader.Announce(message);
    }
}