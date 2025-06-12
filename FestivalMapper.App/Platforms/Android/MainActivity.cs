using Android.App;
using Android.Content.PM;
using Android.OS;
using AndroidX.Activity;
using FestivalMapper.App.Platforms.Android;

namespace FestivalMapper.App
{
    [Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
    public class MainActivity : MauiAppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            OnBackPressedDispatcher.AddCallback(new OnBackPressedCallback(true)
            {
                HandleOnBackPressedImpl = () =>
                {
                    BackHandler.OnBackPressedOverride?.Invoke();
                }
            });



            GlobalLayoutUtil.Initialize();
        }

    }

    public class OnBackPressedCallback : AndroidX.Activity.OnBackPressedCallback
    {
        public Action HandleOnBackPressedImpl { get; set; }

        public OnBackPressedCallback(bool enabled) : base(enabled) { }

        public override void HandleOnBackPressed() => HandleOnBackPressedImpl?.Invoke();
    }

}
