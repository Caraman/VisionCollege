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
	[Activity (Label = "Hangman", MainLauncher = true, Icon = "@drawable/icon",
		ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
	public class MainActivity : Activity
	{
		EditText txtUserName;
		EditText txtPassword;

		Button btnSignIn;
		Button btnSignUp;

		//TEST
		Button btnTest;

		List<accounts> lstAccounts;

		static string dbName = "hangman.sqlite";
		string dbPath = Path.Combine (Android.OS.Environment.ExternalStorageDirectory.ToString (), dbName);

		databaseManager objDb;

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			SetContentView (Resource.Layout.Main);

			txtUserName = FindViewById<EditText> (Resource.Id.txtUserName);
			txtPassword = FindViewById<EditText> (Resource.Id.txtPassword);

			btnSignIn = FindViewById<Button> (Resource.Id.btnSignIn);
			btnSignUp = FindViewById<Button> (Resource.Id.btnSignUp);

			//TEST
			btnTest = FindViewById<Button> (Resource.Id.btnTest);

			CopyDatabase ();

			objDb = new databaseManager ();

			btnSignIn.Click += OnSignInClick; 
			btnSignUp.Click += OnSignUpClick;

			//TEST
			btnTest.Click += OnTestClick;
		}
			
#region: Click Events

		public void OnSignInClick (object sender, EventArgs e)
		{
			if (txtUserName.Text != "" && txtPassword.Text !="") {

				lstAccounts = objDb.login(txtUserName.Text);

				if (lstAccounts.Count > 0) 
				{
					if (lstAccounts [0].password == txtPassword.Text ) 
					{
						Toast.MakeText (this, "Logging you in", ToastLength.Short).Show ();
						launchMenu ();

					} else {
						Toast.MakeText (this, "Incorrect Password", ToastLength.Long).Show ();
					}

				} else {
					Toast.MakeText (this, "Incorrect Username", ToastLength.Long).Show ();
				}
			}

			else if (txtUserName.Text == "") {
				txtUserName.Hint = "Please enter a Username";
			}

			else if (txtPassword.Text == "") {
				txtPassword.Hint = "Please enter a Password";
			}
		}

		public void OnSignUpClick (object sender, EventArgs e)
		{
			if (txtUserName.Text != "" && txtPassword.Text != "") {

				lstAccounts = objDb.login (txtUserName.Text);

				if (lstAccounts.Count == 0) 
				{
					objDb.AddAccount (txtUserName.Text, txtPassword.Text);
					Toast.MakeText (this, "Creating account", ToastLength.Short).Show ();
					launchMenu ();

				} else {
					txtUserName.Text = "";
					txtUserName.Hint = "Username already exists";
				}	
			}

			else if (txtUserName.Text == "") {
				txtUserName.Hint = "Please enter a Username";
			}

			else if (txtPassword.Text == "") {
				txtPassword.Hint = "Please enter a Password";
			}
		}

#endregion

		/// <summary>
		/// Check if your DB has already been extracted
		/// </summary>

		public void CopyDatabase ()
		{	
			if (!File.Exists (dbPath)) {
				using (BinaryReader br = new BinaryReader (Assets.Open (dbName))) 
				{
					using (BinaryWriter bw = new BinaryWriter (new FileStream (dbPath, FileMode.Create))) 
					{
						byte[] buffer = new byte[2048];
						int len = 0;
						while ((len = br.Read(buffer, 0, buffer.Length)) > 0) 
						{
							bw.Write (buffer, 0, len);
						}
					}
				}
			}
		}

		/// <summary>
		/// Launchs the menu.
		/// </summary>

		public void launchMenu ()
		{
			var menu = new Intent (this, typeof(menu));

			menu.PutExtra ("username", txtUserName.Text);

			StartActivity (menu);
		}

		public void OnTestClick(object sender, EventArgs e)
		{
			var game = new Intent (this, typeof(game));

			game.PutExtra ("username", "test");

			StartActivity (game);
		}
	}
}