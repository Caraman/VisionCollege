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
using Android.Graphics;


namespace hangman
{
	[Activity (Label = "Hangman", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]			
	public class game : Activity
	{
		TextView txtScore;
		ImageView ivHangMan;
		TextView txtWord;

		Button btnQ;
		Button btnW;
		Button btnE;
		Button btnR;
		Button btnT;
		Button btnY;
		Button btnU;
		Button btnI;
		Button btnO;
		Button btnP;
		Button btnA;
		Button btnS;
		Button btnD;
		Button btnF;
		Button btnG;
		Button btnH;
		Button btnJ;
		Button btnK;
		Button btnL;
		Button btnZ;
		Button btnX;
		Button btnC;
		Button btnV;
		Button btnB;
		Button btnN;
		Button btnM;

		Button btnRestart;

		static string dbName = "hangman.sqlite";
		string dbPath = System.IO.Path.Combine (Android.OS.Environment.ExternalStorageDirectory.ToString (), dbName);

		databaseManager objDb;

		//Holds list of words player may get pulled from DB
		List<words> wordsList;

		//List of alphabet buttons
		List<Button> buttonList;

		//Each letter of the chosen word, for checking against
		char[] answerLetters;
		char[] currentDisplay;

		string username = "";

		//Game monitors
		int wrongAnswers = 0;
		int rightAnswers = 0;
		int currentScore = 0;
		int prevHighscore = 0;
		int scoreMultiplier = 0;

		bool restartButtonToggle;

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			SetContentView (Resource.Layout.game);

			txtScore = FindViewById<TextView> (Resource.Id.txtScore);

			ivHangMan = FindViewById<ImageView> (Resource.Id.ivHangMan);

			txtWord = FindViewById<TextView> (Resource.Id.txtWord);

			btnQ = FindViewById<Button> (Resource.Id.btnQ);
			btnW = FindViewById<Button> (Resource.Id.btnW);
			btnE = FindViewById<Button> (Resource.Id.btnE);
			btnR = FindViewById<Button> (Resource.Id.btnR);
			btnT = FindViewById<Button> (Resource.Id.btnT);
			btnY = FindViewById<Button> (Resource.Id.btnY);
			btnU = FindViewById<Button> (Resource.Id.btnU);
			btnI = FindViewById<Button> (Resource.Id.btnI);
			btnO = FindViewById<Button> (Resource.Id.btnO);
			btnP = FindViewById<Button> (Resource.Id.btnP);
			btnA = FindViewById<Button> (Resource.Id.btnA);
			btnS = FindViewById<Button> (Resource.Id.btnS); 
			btnD = FindViewById<Button> (Resource.Id.btnD);
			btnF = FindViewById<Button> (Resource.Id.btnF);
			btnG = FindViewById<Button> (Resource.Id.btnG);
			btnH = FindViewById<Button> (Resource.Id.btnH);
			btnJ = FindViewById<Button> (Resource.Id.btnJ);
			btnK = FindViewById<Button> (Resource.Id.btnK);
			btnL = FindViewById<Button> (Resource.Id.btnL);
			btnZ = FindViewById<Button> (Resource.Id.btnZ);
			btnX = FindViewById<Button> (Resource.Id.btnX);
			btnC = FindViewById<Button> (Resource.Id.btnC);
			btnV = FindViewById<Button> (Resource.Id.btnV);
			btnB = FindViewById<Button> (Resource.Id.btnB);
			btnN = FindViewById<Button> (Resource.Id.btnN);
			btnM = FindViewById<Button> (Resource.Id.btnM);

			btnRestart = FindViewById<Button> (Resource.Id.btnRestart);

			buttonList = new List<Button> {
				btnQ, btnW, btnE, btnR, btnT, btnY, btnU, btnI, btnO, btnP,
				btnA, btnS, btnD, btnF, btnG, btnH, btnJ, btnK, btnL, btnZ,
				btnX, btnC, btnV, btnB, btnN, btnM
			};

			objDb = new databaseManager ();

			wordsList = objDb.loadWords();

			username = Intent.GetStringExtra("username");

			newGame ();

			foreach (Button b in buttonList) 
			{
				b.Click += OnLetterClick;
			}

			btnRestart.Click += OnRestartClick;
		}
#region: resets
		public void newGame()
		{
			if (answerLetters != null) 
			{
				Array.Clear(answerLetters, 0, answerLetters.Length);
			}

			wrongAnswers = 0;
			rightAnswers = 0;
			currentScore = 0;
			scoreMultiplier = 0;

			btnRestart.Text = "Restart";

			restartButtonToggle = false;

			txtWord.Text = "";
			txtWord.SetTextColor(Color.Black);

			prevHighscore = objDb.getPrevHighscore (username);

			setScoreBoard ();

			chooseWord ();

			setImageView (wrongAnswers);

			toggleButtons (true);
		}
#endregion			
		/// <summary>
		/// Chooses the word.
		/// </summary>

		public void chooseWord()
		{
			var ran = new Random ();
			var wordPicker = ran.Next (0, wordsList.Count);

			//Possibility for logic to stop the occurance of the same word, 
			//e.g. add already guessed words to an array to check against

			//Pick one of the answers in the DB to be the question, at 'random'
			string word = wordsList [wordPicker].word;

			word = word.ToUpper ();

			answerLetters = word.ToCharArray();

			//Set the word display to blanks for word.length
			for (int i = 0; i < word.Length; i++) {
				txtWord.Text += "-";
			}				
		}
			
		/// <summary>
		/// Raises the letter click event, which checks if the user's letter is in the word, 
		/// and changes the view accordingly
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>

		public void OnLetterClick(object sender, EventArgs e)
		{
			//The button that was pressed
			var pressedButton = sender as Button;

			//Get the current state of the text display
			currentDisplay = txtWord.Text.ToCharArray(0, txtWord.Text.Length);

			string displayText = "";

			//The letter that the player selected
			char selectedLetter = Convert.ToChar(pressedButton.Text);

			//Holds Whether the player guessed correct T = Yes / F = No
			bool rightOrWrong = false;

			//For comparison 
			char blank = Convert.ToChar("-");

			//Loop through all the letters in the Answer
			for (int i = 0; i < answerLetters.Length; i++) 
			{
				if (selectedLetter == answerLetters[i]) {
					//Match
					displayText += Convert.ToString(selectedLetter);

					rightAnswers++;

					doScoring (true);

					rightOrWrong = true;

				} else {
					//No match
					if (currentDisplay[i] == blank) {
						//If it was an "-", it's an "-" again
						displayText += "-";

					} else {
						//Keep previously revealed letters
						displayText += Convert.ToString(currentDisplay[i]);
					}
				}
			}

			if (!rightOrWrong) {
				//No matches
				//Increment times player has been wrong
				wrongAnswers++;

				doScoring (false);

			} else if (rightAnswers == answerLetters.Length) {
				gameOver (true);
			}

			//Display the updated displayText
			txtWord.Text = displayText;

			setImageView (wrongAnswers);

			pressedButton.Enabled = false;
		}

		/// <summary>
		/// Sets the image view 'hangman' increments, and checks for loss
		/// </summary>
		/// <param name="imageStep">Image step.</param>

		public void setImageView(int imageStep)
		{
			switch (imageStep) {
			case 0:
				ivHangMan.SetImageResource (Resource.Drawable.hm0);
				break;

			case 1:
				ivHangMan.SetImageResource (Resource.Drawable.hm1);
				break;

			case 2:
				ivHangMan.SetImageResource (Resource.Drawable.hm2);
				break;

			case 3:
				ivHangMan.SetImageResource (Resource.Drawable.hm3);
				break;

			case 4:
				ivHangMan.SetImageResource (Resource.Drawable.hm4);
				break;

			case 5:
				ivHangMan.SetImageResource (Resource.Drawable.hm5);
				break;

			case 6:
				ivHangMan.SetImageResource (Resource.Drawable.hm6);
				break;

			case 7:
				ivHangMan.SetImageResource (Resource.Drawable.hm7);
				break;

			case 8:
				ivHangMan.SetImageResource (Resource.Drawable.hm8);
				break;

			case 9:
				ivHangMan.SetImageResource (Resource.Drawable.hm9);
				break;

			case 10:
				ivHangMan.SetImageResource (Resource.Drawable.hm10);
				break;

			case 11:
				ivHangMan.SetImageResource (Resource.Drawable.hm11);
				gameOver(false);
				break;

			default:
				ivHangMan.SetImageResource (Resource.Drawable.hm0);
				break;
			}
		}

		public void doScoring(bool outcome)
		{
			if (outcome) {
				//Score is multiplied by the amount of guesses correct in a row
				scoreMultiplier ++;

				if (rightAnswers == 1 && wrongAnswers == 0) {
					//First guess bonus;
					currentScore += 200;
				} else {
					currentScore += (100 * scoreMultiplier);
				}

			} else {
				//Once an incorrect guess is made, multiplier resets
				scoreMultiplier = 0;
				//Score doesn't go below 0, but there is a -50 penalty if letter is incorrect
				if (currentScore - 50 > 0) {
					currentScore -= 50;
				} else {
					currentScore = 0;
				}
			}

			setScoreBoard ();
		}

		public void setScoreBoard()
		{
			txtScore.Text = "Score: " + currentScore + " (x" + scoreMultiplier + ")" + " Best: " + prevHighscore;
		}
			
		public void gameOver(bool outcome)
		{
			toggleButtons (false);

			if (!outcome) {
				//lose
				//Reveal missed letters
				txtWord.Text = new string(answerLetters);
				//Change Colour
				txtWord.SetTextColor(Color.Red);
			}

			if (currentScore > prevHighscore) {
				objDb.setNewHighscore (username, currentScore);

				var builder = new AlertDialog.Builder(this);
				builder.SetMessage("New High Score!");
				builder.SetPositiveButton("View", (s, ea) => { viewHighScore(); builder.Dispose(); });
				builder.SetNegativeButton("Back", (s, ea) => { builder.Dispose(); });
				builder.Create().Show();
			}

			btnRestart.Text = "Play again?";
			restartButtonToggle = true;
		}

		/// <summary>
		/// enables all 'letter' buttons with OnLetterClick handler
		/// </summary>

		public void toggleButtons(bool e) {
			if (e) {
				foreach (Button b in buttonList) 
				{
					b.Enabled = true;
				}		
			} else {
				foreach (Button b in buttonList) 
				{
					b.Enabled = false;
				}
			}
		}

		/// <summary>
		/// This button changes states when the game ends, or begins/is going
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>

		public void OnRestartClick(object sender, EventArgs e)
		{
			if (!restartButtonToggle) {
				var builder = new AlertDialog.Builder(this);
				builder.SetMessage("Restart game?");
				builder.SetPositiveButton("OK", (s, ea) => { newGame(); builder.Dispose(); });
				builder.SetNegativeButton("Cancel", (s, ea) => { builder.Dispose(); });
				builder.Create().Show();
			} else {
				newGame ();
			}
		}

		public void viewHighScore()
		{
			var highscores = new Intent (this, typeof(highScores));
			StartActivity (highscores);
		}
	}
}

