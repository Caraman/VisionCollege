using System;
using SQLite;

namespace hangman
{
	public class words
	{
		[PrimaryKey, AutoIncrement]
		public string word{ get; set; }

		public words ()
		{
		}
	}
}

