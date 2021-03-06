﻿using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Android_Question_App
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", MainLauncher = true)]
    public class LoginActivity : AppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_main);

            Android.Support.V7.Widget.Toolbar toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);

            Button searchButton = FindViewById<Button>(Resource.Id.search_button);
            searchButton.Click += onSearchButtonClicked;
        }

        private void onSearchButtonClicked(object sender, EventArgs e)
        {
            hideKeyboard();
            var query = FindViewById<TextInputEditText>(Resource.Id.textInput1).Text;
            if (query == null || query.Trim().Equals("")) {
                return;
            }
            Button searchButton = FindViewById<Button>(Resource.Id.search_button);
            searchButton.Enabled = false;
            searchButton.Text = GetString(Resource.String.searching);
            ThreadPool.QueueUserWorkItem(o => fetchSubReddits(query));
        }

        private void fetchSubReddits(String query) {
            var json = new WebClient().DownloadString("http://www.reddit.com/subreddits/search.json?q=" + query);
            var subreddits = JsonConvert.DeserializeObject<JObject>(json);

            RunOnUiThread(() => {
                var subredditList = FindViewById<LinearLayout>(Resource.Id.subreddit__list);
                subredditList.RemoveAllViewsInLayout();
                foreach (var subreddit in subreddits["data"]["children"] as JArray) {
                    var name = subreddit["data"]["display_name_prefixed"].ToString();
                    var newListItem = new TextView(this);
                    newListItem.SetPadding(16, 0, 16, 32);
                    newListItem.PaintFlags = Android.Graphics.PaintFlags.UnderlineText;
                    newListItem.SetTextColor(Android.Graphics.Color.Blue);
                    newListItem.Text = name;
                    newListItem.Click += onSubredditClicked;

                    subredditList.AddView(newListItem);
                }
                Button searchButton = FindViewById<Button>(Resource.Id.search_button);
                searchButton.Enabled = true;
                searchButton.Text = GetString(Resource.String.search);
            });
        }

        private void onSubredditClicked(object sender, EventArgs e)
        {
            var listItem = (TextView)sender;
            var subredditName = listItem.Text;
            var sidebarUrl = "http://www.reddit.com/" + subredditName + "/about/sidebar";
            var intent = new Intent(this, typeof(SidebarActivity));
            intent.PutExtra("sidebarUrl", sidebarUrl);
            this.StartActivity(intent);
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.menu_main, menu);
            return true;
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            return base.OnOptionsItemSelected(item);
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        private void hideKeyboard() {
            var textInput = FindViewById<TextInputEditText>(Resource.Id.textInput1);
            textInput.Enabled = false;
            textInput.Enabled = true;
        }
    }
}

