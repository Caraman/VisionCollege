using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace hangman
{
	[Activity (Label = "Hangman | Menu", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]			
	public class menu : Activity
	{
		TextView txtWelcome;

		Button btnPlay;
		Button btnHighScores;

		string username = "";

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			SetContentView (Resource.Layout.menu);

			username = Intent.GetStringExtra("username");

			txtWelcome = FindViewById<TextView> (Resource.Id.txtWelcome);

			btnPlay = FindViewById<Button> (Resource.Id.btnPlay);
			btnHighScores = FindViewById<Button> (Resource.Id.btnHighScores);

			btnPlay.Click += OnPlayClick;
			btnHighScores.Click += OnHighScoresClick;

			txtWelcome.Text = "Welcome " + username + "!";
		}

		public void OnPlayClick(object sender, EventArgs e)
		{
			var game = new Intent (this, typeof(game));

			game.PutExtra ("username", username);

			StartActivity (game);
		}

		public void OnHighScoresClick(object sender, EventArgs e)
		{
			var highscores = new Intent (this, typeof(highScores));
			StartActivity (highscores);
		}
	}
}