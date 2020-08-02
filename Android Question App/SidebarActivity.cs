using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Webkit;

namespace Android_Question_App
{
    [Activity(Label = "SidebarActivity")]
    public class SidebarActivity : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            var sidebarHtml = Intent.Extras.GetString("sidebarHtml");
            var webView = new WebView(this);
            var metrics = Resources.DisplayMetrics;

            AddContentView(webView, new ViewGroup.LayoutParams(metrics.WidthPixels, metrics.HeightPixels));
            webView.LoadData(sidebarHtml, "text/html", "utf-8");
        }
    }
}