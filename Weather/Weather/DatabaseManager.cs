using System;
using System.IO;
using System.Collections.Generic;

namespace Weather
{
	public class DatabaseManager
	{
		static string dbName = "saved_cities.sqlite";
		string dbPath = Path.Combine (Android.OS.Environment.ExternalStorageDirectory.ToString (), dbName);

		public DatabaseManager ()
		{

		}

		public List<city_names> loadWords ()
		{
			try 
			{
				using (var conn = new SQLite.SQLiteConnection (dbPath)) 
				{
					var cmd = new SQLite.SQLiteCommand (conn);
					cmd.CommandText = "select * from cities"; 

					var tempList = cmd.ExecuteQuery<city_names> ();
					return tempList;
				}

			} catch (Exception ex) {
				Console.WriteLine ("Error:" + ex.Message);
				return null;
			}
		}
	}
}

