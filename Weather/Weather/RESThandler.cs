using System;
using RestSharp;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.Text;
using System.IO;

namespace Weather
{
	public class RESThandler
	{
		private string url;
		private IRestResponse response;

		public RESThandler ()
		{
			url = "";
		}

		public RESThandler(string lurl)
		{
			url = lurl;
		}

		public async Task<Weatherdata> ExecuteRequestAsync()
		{
			var client = new RestClient (url);
			var request = new RestRequest ();

			response = await client.ExecuteTaskAsync (request);

			XmlSerializer serializer = new XmlSerializer (typeof(Weatherdata));
			Weatherdata objRss;

			TextReader sr = new StringReader (response.Content);
			objRss = (Weatherdata)serializer.Deserialize (sr);
			return objRss;
		}
	}
}
