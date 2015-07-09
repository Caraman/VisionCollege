using System;
using System.IO;
using System.Collections.Generic;

namespace hangman
{
	public class databaseManager
	{
		static string dbName = "hangman.sqlite";
		string dbPath = Path.Combine (Android.OS.Environment.ExternalStorageDirectory.ToString (), dbName);

		public databaseManager () {}

		/// <summary>
		/// Check if the specified user exists
		/// </summary>
		/// <param name="username">Username.</param>

		public List<accounts> login (string username)
		{
			try 
			{
				using (var conn = new SQLite.SQLiteConnection (dbPath)) 
				{
					var cmd = new SQLite.SQLiteCommand (conn);
					cmd.CommandText = "select * from accounts where username like '%" + username + "%'"; 

					var tempList = cmd.ExecuteQuery<accounts> ();
					return tempList;
				}

			} catch (Exception ex) {
				Console.WriteLine ("Error:" + ex.Message);
				return null;
			}
		}

		/// <summary>
		/// Adds a new user account.
		/// </summary>
		/// <param name="username">Username.</param>
		/// <param name="password">Password.</param>

		public void AddAccount (string username, string password)
		{
			try 
			{
				using (var conn = new SQLite.SQLiteConnection (dbPath)) 
				{
					var cmd = new SQLite.SQLiteCommand (conn);
					cmd.CommandText = "insert into accounts(username,password) values('" + username + "','" + password + "')";
					cmd.ExecuteNonQuery();
				}

			} catch (Exception ex) {
				Console.WriteLine ("Error:" + ex.Message);
			}
		}

		/// <summary>
		/// Loads the words to be used by the game.
		/// </summary>
		/// <returns>The words.</returns>

		public List<words> loadWords ()
		{
			try 
			{
				using (var conn = new SQLite.SQLiteConnection (dbPath)) 
				{
					var cmd = new SQLite.SQLiteCommand (conn);
					cmd.CommandText = "select * from words"; 

					var tempList = cmd.ExecuteQuery<words> ();
					return tempList;
				}

			} catch (Exception ex) {
				Console.WriteLine ("Error:" + ex.Message);
				return null;
			}
		}

		/// <summary>
		/// Loads all the saved High scores
		/// </summary>
		/// <returns>highscores</returns>

		public List<highscore> getScores ()
		{
			try 
			{
				using (var conn = new SQLite.SQLiteConnection (dbPath)) 
				{
					var cmd = new SQLite.SQLiteCommand (conn);
					cmd.CommandText = "select * from highscores order by score desc"; 

					var tempList = cmd.ExecuteQuery<highscore> ();
					return tempList;
				}

			} catch (Exception ex) {
				Console.WriteLine ("Error:" + ex.Message);
				return null;
			}
		}

		/// <summary>
		/// Gets the previous best highscore of the player.
		/// </summary>
		/// <returns>The previous highscore.</returns>
		/// <param name="username">Username.</param>

		public int getPrevHighscore(string username)
		{
			try 
			{
				using (var conn = new SQLite.SQLiteConnection (dbPath)) 
				{
					var cmd = new SQLite.SQLiteCommand (conn);
					cmd.CommandText = "select max(score) from highscores where username like '%" + username + "%'"; 

					int temp= cmd.ExecuteScalar<int> ();
					return temp;
				}

			} catch (Exception ex) {
				Console.WriteLine ("Error:" + ex.Message);
				return 0;
			}
		}

		/// <summary>
		/// Sets the new highscore.
		/// </summary>
		/// <param name="username">Username.</param>
		/// <param name="score">Score.</param>

		public void setNewHighscore(string username, int score)
		{
			try 
			{
				using (var conn = new SQLite.SQLiteConnection (dbPath)) 
				{
					var cmd = new SQLite.SQLiteCommand (conn);
					cmd.CommandText = "insert into highscores(username,score) values('" + username + "','" + score + "')";
					cmd.ExecuteNonQuery();
				}

			} catch (Exception ex) {
				Console.WriteLine ("Error:" + ex.Message);
			}
		}
	}
}