using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Gestures;
using Android.Views.Animations;
using Android.Locations;
using Android.Util;
using Android.Provider;

using Xamarin.ActionbarSherlockBinding.App;
using SherlockActionBar = Xamarin.ActionbarSherlockBinding.App.ActionBar;
using Xamarin.ActionbarSherlockBinding.Views;
using Tab = Xamarin.ActionbarSherlockBinding.App.ActionBar.Tab;
using FragmentTransaction = Android.Support.V4.App.FragmentTransaction;
using ActionProvider = Xamarin.ActionbarSherlockBinding.Views.ActionProvider;
using ActionMode = Xamarin.ActionbarSherlockBinding.Views.ActionMode;
using IMenu = Xamarin.ActionbarSherlockBinding.Views.IMenu;
using IMenuItem = Xamarin.ActionbarSherlockBinding.Views.IMenuItem;

namespace Weather
{
	[Activity (Label = "NZWeather", MainLauncher = true, Icon = "@drawable/icon",
		ScreenOrientation = ScreenOrientation.Portrait)]

	public class MainActivity : SherlockActivity, ILocationListener, GestureDetector.IOnGestureListener
	{
		//MainView
		ImageView ivMainImg;
		TextView txtMainTemp;
		TextView txtMainMaxMin;
		ViewAnimator windowHolder;

		//SingleDayView
		TextView txtFeelsLikeTemp;
		TextView txtWindDisplay;
		TextView txtWeatherStory;
		TextView txtPressureValue;
		TextView txtHumidityValue;
		TextView txtDewPointValue;
		TextView txtPrecipitationValue;

		//FiveDayView
		ListView lvForecast;
		List<Data> rows;

		//CitySelector
		ListView lvCities;
		string [] cityNames;

		//Gestures
		GestureDetector _gestureDetector;
		int swipe_Min_Distance = 0;
		int swipe_Max_Distance = 0;
		int swipe_Min_Velocity = 0;

		//Location
		LocationManager locMgr;
		double latitude;
		double longitude;
		String provider;

		//REST
		RESThandler objRest;
		List<Time> rssData;

		//bool check to avoid double calling
		bool IsRefreshing;

		//bool dont refresh current location if custom local is selected
		bool notCurrentLocation;

#region: Activity Lifecycle
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			SetContentView (Resource.Layout.Main);

			//MainView
			ivMainImg = FindViewById<ImageView> (Resource.Id.ivMainImg);
			txtMainTemp = FindViewById<TextView> (Resource.Id.txtMainTemp);
			txtMainMaxMin = FindViewById<TextView> (Resource.Id.txtMainMaxMin);

			ivMainImg.SetImageResource(Android.Resource.Color.Transparent);

			windowHolder = FindViewById<ViewAnimator> (Resource.Id.vaWindowHold);

			windowHolder.AddView(LayoutInflater.Inflate(Resource.Layout.SingleDayView, null));
			windowHolder.AddView(LayoutInflater.Inflate(Resource.Layout.FiveDayForecast, null));
			windowHolder.AddView(LayoutInflater.Inflate(Resource.Layout.SelectCity, null));

			//SingleDayView
			txtFeelsLikeTemp = FindViewById<TextView> (Resource.Id.txtFeelsLikeTemp);
			txtWindDisplay = FindViewById<TextView> (Resource.Id.txtWindDisplay);
			txtWeatherStory = FindViewById<TextView> (Resource.Id.txtWeatherStory);
			txtPressureValue = FindViewById<TextView> (Resource.Id.txtPressureValue);
			txtHumidityValue = FindViewById<TextView> (Resource.Id.txtHumidityValue);
			txtDewPointValue = FindViewById<TextView> (Resource.Id.txtDewPointValue);
			txtPrecipitationValue = FindViewById<TextView> (Resource.Id.txtPrecipitationValue);

			//FiveDayView
			lvForecast = FindViewById<ListView> (Resource.Id.lvForecast);

			//CitySelector
			lvCities = FindViewById<ListView> (Resource.Id.lvCities);
			cityNames = Resources.GetStringArray (Resource.Array.city_names);
			lvCities.Adapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleListItem1, cityNames);
			lvCities.ItemClick += OnCitySelect;

			//TouchEvents
			_gestureDetector = new GestureDetector(this);

			setSwipeMinDistance (100);
			setSwipeMaxDistance (350);
			setSwipeMinVelocity (100);
		}
			
		protected override void OnResume ()
		{
			base.OnResume (); 

			if (!IsRefreshing & !notCurrentLocation) 
			{
				IsRefreshing;
				// Initialize location manager
				locMgr = GetSystemService (Context.LocationService) as LocationManager;
				// pass in the provider (GPS), 
				// the minimum time between updates (in seconds), 
				// the minimum distance the user needs to move to generate an update (in meters),
				// and an ILocationListener (recall that this class impletents the ILocationListener interface)
				if (locMgr.AllProviders.Contains (LocationManager.NetworkProvider)
					 && locMgr.IsProviderEnabled (LocationManager.NetworkProvider)) 
				{
					!IsRefreshing;
					locMgr.RequestLocationUpdates (LocationManager.NetworkProvider, 10000, 100, this);
				} else {
					Toast.MakeText (this, "The Network Provider does not exist or is not enabled!", ToastLength.Long).Show ();
				}
			}
		}

		protected override void OnPause ()
		{
			base.OnPause ();
			locMgr.RemoveUpdates (this);
		}
#endregion

#region: Location Services
		public void OnLocationChanged (Android.Locations.Location location)
		{
			latitude = location.Latitude;
			longitude = location.Longitude;
			provider = location.Provider;

			LoadRSS ("http://api.openweathermap.org/data/2.5/forecast?lat=" + latitude + 
					 "&lon=" + longitude + "&APPID=c71685dba2279d7031cb97102836ffde&mode=xml");
			
			setCurrentLocationTitle ();
		}
		public void OnProviderDisabled (string provider){}
		public void OnProviderEnabled (string provider){}
		public void OnStatusChanged (string provider, Availability status, Bundle extras){}
#endregion

#region: Touch Events
		public override bool OnTouchEvent(MotionEvent e)
		{
			_gestureDetector.OnTouchEvent(e);
			return false;
		}

		public bool OnFling(MotionEvent e1, MotionEvent e2, float velocityX, float velocityY)
		{
			float xDistance = Math.Abs(e1.GetX() - e2.GetX());
			float yDistance = Math.Abs(e1.GetY() - e2.GetY());

			if (xDistance > this.swipe_Max_Distance || yDistance > this.swipe_Max_Distance) 
			{
				return false;
			}

			velocityX = Math.Abs(velocityX);
			velocityY = Math.Abs(velocityY);

			if (velocityX > this.swipe_Min_Velocity && xDistance > this.swipe_Min_Distance) 
			{
				if (e1.GetX () > e2.GetX ()) 
				{ 
					windowHolder.Animation = AnimationUtils.MakeInAnimation (this, false);
					windowHolder.ShowNext ();
					SetDetails ();
				} 

				else 
				{
					windowHolder.Animation = AnimationUtils.MakeInAnimation (this, true);
					windowHolder.ShowPrevious ();
					SetDetails ();
				}
				return true;
			} 

			else if (velocityY > this.swipe_Min_Velocity && yDistance > this.swipe_Min_Distance) 
			{
				if (e1.GetY () > e2.GetY ()){
				} else { }
				return true;
			}
			return false;
		}

		//Required, but unused
		public bool OnDown(MotionEvent e){return false;}
		public bool OnUp(MotionEvent e){return false;}
		public bool OnScroll(MotionEvent e1, MotionEvent e2, float distanceX, float distanceY){return false;}
		public bool OnSingleTapUp(MotionEvent e){return false;}
		public bool OnDoubleTap(MotionEvent e){return false;}
		public void OnShowPress(MotionEvent e){}
		public void OnLongPress(MotionEvent e){}

		//Set swipe params
		public void setSwipeMaxDistance(int distance){ swipe_Max_Distance = distance; }
		public void setSwipeMinDistance(int distance){ swipe_Min_Distance = distance; }
		public void setSwipeMinVelocity(int distance){ swipe_Min_Velocity = distance; }
#endregion 

#region: ActionBarSherlock
		public override bool OnCreateOptionsMenu (IMenu menu)
		{
			menu.Add ("Refresh")
				.SetIcon (Android.Resource.Drawable.IcMenuMyLocation)
				.SetShowAsAction (MenuItem.ShowAsActionAlways);

			return true;
		}

		public override bool OnOptionsItemSelected (IMenuItem item)
		{
			var itemTitle = item.TitleFormatted.ToString ();

			switch (itemTitle){

			case "Refresh":
				notCurrentLocation = false;

				if (!IsRefreshing) {
					IsRefreshing = true;

					if (locMgr.AllProviders.Contains (LocationManager.NetworkProvider)
					    && locMgr.IsProviderEnabled (LocationManager.NetworkProvider)) 
					{
						locMgr.RequestLocationUpdates (LocationManager.NetworkProvider, 2000, 1, this);
					} else {
						Toast.MakeText (this, "The Network Provider does not exist or is not enabled!", ToastLength.Long).Show ();
					}
					IsRefreshing = false;
				}
				break;
			}
			return base.OnOptionsItemSelected (item);
		}
#endregion

#region: View Interaction

		public async void LoadRSS(string url)
		{
			if (!IsRefreshing) 
			{
				IsRefreshing = true;

				objRest = new RESThandler (@url);
				var response = await objRest.ExecuteRequestAsync ();

				rssData = response.Forecast.Time;

				SetDetails ();
			}
		}

		public void SetDetails()
		{
			if (rssData != null) {

				//Main View

				//Icon
				var mDrawable = "_" + rssData[0].Symbol.Var;

				var resourceId = (int)typeof(Resource.Drawable).GetField(mDrawable).GetValue(null);

				ivMainImg.SetImageResource(resourceId);

				//Temp
				txtMainTemp.Text = Convert.ToString(Math.Round(rssData[0].Temperature.Value, 0) + "º");

				//Min/Max
				txtMainMaxMin.Text = Convert.ToString (
					Math.Round(rssData[0].Temperature.Min, 0) + "º / " + 
					Math.Round(rssData[0].Temperature.Max, 0) + "º"
				);

				//Single Day View

				//Feels Like (WindChill)
				txtFeelsLikeTemp.Text = SetFeelsLike ();

				//Wind e.g "25 KPH NNE"
				txtWindDisplay.Text = Convert.ToString (
					Math.Round (3.6 * rssData[0].WindSpeed.Mps, 0)) + " KPH " + rssData[0].WindDirection.Code;

				//Weather Story
				string storyValue1;
				string storyValue2 = rssData[0].Clouds.Value;

				if (rssData[0].Precipitation.Type != null) {
					storyValue1 = char.ToUpper (rssData[0].Precipitation.Type [0]) + rssData[0].Precipitation.Type.Substring (1);
					txtWeatherStory.Text = storyValue1 + ", " + storyValue2;
				} else {
					txtWeatherStory.Text = char.ToUpper(storyValue2[0]) + storyValue2.Substring(1);
				}

				//Pressure
				txtPressureValue.Text = Convert.ToString(
					Math.Round (rssData[0].Pressure.Value, 1) / 100) + "\"";

				//Humidity as %
				txtHumidityValue.Text = Convert.ToString(
					rssData[0].Humidity.Value) + "%";

				//Dew Point
				txtDewPointValue.Text = SetDewPoint ();

				//Precipitation in mm
				if (rssData[0].Precipitation.Type != null) {
					txtPrecipitationValue.Text = Convert.ToString(rssData[0].Precipitation.Value) + "mm";
				} else {
					txtPrecipitationValue.Text = "0mm";
				}

				//Five Day View

				rows = new List<Data> ();

				lvForecast.Adapter = new DataAdapter (this, rows);

				string dayOfWeek = rssData[0].To.DayOfWeek.ToString ();

				for (int i = 0; i < rssData.Count-1; i++) 
				{

					if (rssData [i].To.DayOfWeek.ToString () != dayOfWeek) 
					{
						var obj = new Data ();

						obj.day = rssData [i].To.DayOfWeek.ToString ();

						obj.temp = Convert.ToString (
							Math.Round (rssData [i].Temperature.Min, 0) + "º / " +
							Math.Round (rssData [i].Temperature.Max, 0) + "º"
						);

						var icon = "_" + rssData [i].Symbol.Var;

						obj.ImageID = (int)typeof(Resource.Drawable).GetField (icon).GetValue (null);

						rows.Add (obj);

						dayOfWeek = rssData [i].To.DayOfWeek.ToString ();
					}
				}

				!IsRefreshing;
			}
		}

		public string SetFeelsLike()
		{
			//T is the air temperature, and V is the wind speed.
			var T = rssData [0].Temperature.Value;
			var V = 3.6 * rssData [0].WindSpeed.Mps;
			//3.6 × Vm/sec = Vkm/h

			var WindChill = 13.12 + (.6215 * T) - (11.37 * Math.Pow (V, 0.16)) + (.3965 * T * Math.Pow (V, 0.16));

			return Convert.ToString(Math.Round(WindChill,0)) + "º";

			//http://www.calcunation.com/calculators/weights%20and%20measures/wind-chill-celsius.php
			//http://www.aqua-calc.com/convert/speed/meter-per-second-to-kilometer-per-hour
		}

		public string SetDewPoint()
		{
			var T = rssData [0].Temperature.Value;
			var H = rssData [0].Humidity.Value;

			var B = (Math.Log(H / 100) + ((17.27 * T) / (237.3 + T))) / 17.27;

			var D = (237.3 * B) / (1 - B);

			return Convert.ToString (Math.Round(D, 0) + "º");

			//Where:
			//T = Air Temperature (Dry Bulb) in Centigrade (C) degrees
			//H = Relative Humidity in percent (%)
			//B = intermediate value (no units) 
			//D = Dewpoint in Centigrade (C) degrees

			//http://ag.arizona.edu/azmet/dewpoint.html
		}

		public async void setCurrentLocationTitle()
		{
			Geocoder geocoder = new Geocoder(this);
			IList<Address> addressList = await geocoder.GetFromLocationAsync(latitude, longitude, 10);

			Address address = addressList.FirstOrDefault();

			Console.WriteLine (address);

			if (address != null)
			{
				Window.SetTitle (address.GetAddressLine(1));
			}
		}
			
		public void OnCitySelect(object sender, AdapterView.ItemClickEventArgs e)
		{
			if (!IsRefreshing) 
			{
				notCurrentLocation = true;
				LoadRSS ("http://api.openweathermap.org/data/2.5/forecast?q=" + cityNames[e.Id] +",new%20zealand&mode=xml");
				Window.SetTitle (cityNames[e.Id]);
			}
		}
#endregion
	}
}


