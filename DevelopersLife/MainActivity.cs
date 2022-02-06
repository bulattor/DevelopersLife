using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.App;
using AndroidX.SwipeRefreshLayout.Widget;
using Com.Bumptech.Glide;
using Com.Bumptech.Glide.Request;
using DevelopersLife.Resources.Data;
using Google.Android.Material.BottomNavigation;
using System.Collections.Generic;

namespace DevelopersLife
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true, ScreenOrientation = ScreenOrientation.Portrait)]
    public class MainActivity : AppCompatActivity, BottomNavigationView.IOnNavigationItemSelectedListener
    {
        TextView description;
        ImageView gifview;
        Button next, back, update;
        List<GifPost>[] posts;
        RelativeLayout errorLayout;
        LinearLayout contentLayout;
        int category = 0;
        int[] pos;
        RequestOptions requestOptions;
        CircularProgressDrawable circularProgress;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_main);

            BottomNavigationView navigation = FindViewById<BottomNavigationView>(Resource.Id.navigation);
            navigation.SetOnNavigationItemSelectedListener(this);

            pos = new int[4] { 0, 0, 0, 0 };

            errorLayout = FindViewById<RelativeLayout>(Resource.Id.error);
            contentLayout = FindViewById<LinearLayout>(Resource.Id.content);
            gifview = FindViewById<ImageView>(Resource.Id.gifview);
            description = FindViewById<TextView>(Resource.Id.description);
            next = FindViewById<Button>(Resource.Id.next);
            next.Click += (s, e) => { Show(category, ++pos[category]); };

            back = FindViewById<Button>(Resource.Id.back);
            back.Click += (s, e) => { Show(category, --pos[category]); };

            update = FindViewById<Button>(Resource.Id.update);
            update.Click += (s, e) =>
            {
                contentLayout.Visibility = ViewStates.Visible;
                errorLayout.Visibility = ViewStates.Invisible;
                Show(category, pos[category]);
            };

			circularProgress = new CircularProgressDrawable(this);
			circularProgress.StrokeWidth = 10f;
			circularProgress.CenterRadius = 50f;
            circularProgress.SetColorSchemeColors(Resource.Color.colorAccent);
            circularProgress.SetAlpha(255);
			circularProgress.Start();

			requestOptions = new RequestOptions();
            requestOptions.Placeholder(circularProgress);
            requestOptions.CenterCrop();
            requestOptions.Error(Resource.Drawable.baseline_broken_image_24);

            posts = new List<GifPost>[4] { new List<GifPost>(), new List<GifPost>(), new List<GifPost>(), new List<GifPost>() };

            StrictMode.ThreadPolicy policy = new StrictMode.ThreadPolicy.Builder().PermitAll().Build();
            StrictMode.SetThreadPolicy(policy);

            Show(category, pos[category]);
        }
        public async void Show(int cat, int pos)
		{
            next.Enabled = true;
            if (pos == 0) back.Enabled = false; else back.Enabled = true;


            if (pos+1 > posts[cat].Count)
			{
                GifPost gifPost = new GifPost();
                await gifPost.GetData(cat, pos);

                if (gifPost.Status == -2)
				{
                    next.Enabled = false;
                    Glide.With(this)
                            .Load(gifPost.GifURL)
                            .Apply(requestOptions)
                            .Into(gifview);
                    description.Text = gifPost.Description;
                    return;
                }
                if (gifPost.Status == -1)
				{
                    contentLayout.Visibility = ViewStates.Invisible;
                    errorLayout.Visibility = ViewStates.Visible;
				}
                else
				{
                    Glide.With(this)
                            .Load(gifPost.GifURL)
                            .Apply(requestOptions)
                            .Into(gifview);
                    description.Text = gifPost.Description;

                    posts[cat].Add(gifPost);
                }
            } else
			{
                Glide.With(this)
                            .Load(posts[cat][pos].GifURL)
                            .Apply(requestOptions)
                            .Into(gifview);
                description.Text = posts[cat][pos].Description;
			}            
        }
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
        public bool OnNavigationItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Resource.Id.navigation_random:
                    category = 0;
                    Show(category, pos[category]);
                    return true;

                case Resource.Id.navigation_latest:
                    category = 1;
                    Show(category, pos[category]);
                    return true;
                case Resource.Id.navigation_hot:
                    category = 2;
                    Show(category, pos[category]);
                    return true;
                case Resource.Id.navigation_top:
                    category = 3;
                    Show(category, pos[category]);
                    return true;
            }
            
            return false;
        }
    }
}

