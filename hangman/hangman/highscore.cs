using System;
using SQLite;

namespace hangman
{
	public class highscore
	{
		[PrimaryKey, AutoIncrement]
		public string username{ get; set; }
		public string score{ get; set; }

		public highscore ()
		{
		}
	}
}