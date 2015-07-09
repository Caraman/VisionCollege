using System;
using SQLite;

namespace Weather
{
	public class city_names
	{
		[PrimaryKey, AutoIncrement]
		public string cities{ get; set; }

		public city_names ()
		{
		}
	}
}

