using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace DevelopersLife.Resources.Data
{
	class GifPost
	{
		public int Id { get; set; }
		public int Status { get; set; }
		public string Description { get; set; }
		public string GifURL { get; set; }
		public async Task GetData(int category, int pos = -1)
		{
			string uri;
			switch (category)
			{
				case 1: uri = "https://developerslife.ru/latest/" + (pos/5).ToString() + "?json=true";
					break;
				case 2:
					uri = "https://developerslife.ru/hot/" + (pos / 5).ToString() + "?json=true";
					break;
				case 3:
					uri = "https://developerslife.ru/top/" + (pos / 5).ToString() + "?json=true";
					break;
				default: uri = "https://developerslife.ru/random?json=true";
					break;
			}
			try
			{
				Status = 0;
				HttpClient client = new HttpClient();
				client.BaseAddress = new Uri(uri);
				var response = await client.GetAsync(client.BaseAddress);
				response.EnsureSuccessStatusCode();

				var content = await response.Content.ReadAsStringAsync();
				JObject o = JObject.Parse(content);

				GifPost gifPost;
				switch (category)
				{
					case 0: gifPost = JsonConvert.DeserializeObject<GifPost>(o.ToString());
						break;
					default: if (pos >= int.Parse(o.SelectToken("totalCount").ToString()))
						{
							Status = -2;
							GifURL = "";
							Description = "В этой категории больше нет постов";
							return;
						}
						gifPost = JsonConvert.DeserializeObject<GifPost>(o.SelectToken("result[" + (pos%5).ToString() + "]").ToString());
						break;
				}
				

				Id = gifPost.Id;
				GifURL = gifPost.GifURL.Replace("http:", "https:");
				Description = gifPost.Description;
			} catch (Exception ex)
			{
				Status = -1;
				Console.WriteLine("NO INTERNET");
			}
			
		}
	}
}