using System;
using System.IO;
using System.Collections.Generic;

using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;

namespace hangman
{
	[Activity (Label = "Hangman | High Scores", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]			
	public class highScores : Activity
	{
		ListView lvHighscores;

		static string dbName = "hangman.sqlite";
		string dbPath = Path.Combine (Android.OS.Environment.ExternalStorageDirectory.ToString (), dbName);

		databaseManager objDb;

		List<highscore> lstHighscore;

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			SetContentView (Resource.Layout.highScores);

			lvHighscores = FindViewById<ListView> (Resource.Id.listView1);

			objDb = new databaseManager ();

			lstHighscore = objDb.getScores ();
	
			lvHighscores.Adapter = new SimpleListItem2Adapter (this, lstHighscore);
		}
	}
}

