using System;
using SQLite;

namespace hangman
{
	public class accounts
	{
		[PrimaryKey, AutoIncrement]
		public string username{ get; set; }
		public string password{ get; set; }

		public accounts ()
		{
		}
	}
}

