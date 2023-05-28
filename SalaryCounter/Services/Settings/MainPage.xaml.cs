namespace SalaryCounter;

public partial class MainPage : ContentPage
{
    int count = 0;
    public static string mailHolder = "alexm.17391@gmail.com";
    public MainPage()
    {
        InitializeComponent();
    }
    void switcher_Toggled(object sender, ToggledEventArgs e)
    {
        if (e.Value)
        {
            Application.Current.UserAppTheme = AppTheme.Dark;
        }
        else Application.Current.UserAppTheme = AppTheme.Light;
    }
    private void OnCounterClicked(object sender, EventArgs e)
    {
        mailHolder = mail.Text;
        AppShell.Current.DisplayAlert("Настрока", "Почта сохранена на время сессии", "OK");
    }
    public async void TakePhoto(object sender, EventArgs e)
    {
        if (MediaPicker.Default.IsCaptureSupported)
        {
            FileResult photo = await MediaPicker.Default.CapturePhotoAsync();

            if (photo != null)
            {
                // save the file into local storage
                string localFilePath = Path.Combine(FileSystem.CacheDirectory, photo.FileName);
                using Stream sourceStream = await photo.OpenReadAsync();
                using FileStream localFileStream = File.OpenWrite(localFilePath);
                await sourceStream.CopyToAsync(localFileStream);

                Image.Source = localFilePath;
            }
        }
    }
}

