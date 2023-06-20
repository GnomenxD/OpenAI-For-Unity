using Cosmos.AI.Open_AI;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Cosmos.AI
{
	/// <summary>
	/// Represents the OpenAI client for accessing various OpenAI API services.
	/// </summary>
	public class OpenAI
	{
		internal const string UrlModels = "https://api.openai.com/v1/models";
		internal const string UrlTextCompletion = "https://api.openai.com/v1/completions";
		internal const string UrlImageGeneration = "https://api.openai.com/v1/images/generations";
		internal const string UrlChatCompletion = "https://api.openai.com/v1/chat/completions";

		private readonly string apiKey;
		private readonly ImageGenerator imageGeneration;
		private readonly TextCompletion textCompletion;
		private readonly ChatCompletion chatCompletion;

		internal string ApiKey => apiKey;
		/// <summary>
		/// Access to Image Generation requests.
		/// </summary>
		public ImageGenerator ImageGeneration => imageGeneration;
		/// <summary>
		/// Access to Text Completetion requests.
		/// </summary>
		public TextCompletion TextCompletion => textCompletion;
		/// <summary>
		/// [Obsolete] Access to Chat Completetion requests.
		/// </summary>
		[Obsolete("This is incomplete and does not work.")]
		public ChatCompletion ChatCompletion => chatCompletion;

		/// <summary>
		/// Initializes a new instance of the <see cref="OpenAI"/> class with the specified API key.
		/// <para>Make sure not to include the API key in uploads to github.</para>
		/// </summary>
		/// <param name="apiKey">The API key to access OpenAI services.</param>
		public OpenAI(string apiKey)
		{
			this.apiKey = apiKey;
			this.imageGeneration = new ImageGenerator(this);
			this.textCompletion = new TextCompletion(this);
			this.chatCompletion = new ChatCompletion(this);
		}

		/// <summary>
		/// Retrieves a list of models available from the OpenAI API.
		/// </summary>
		/// <returns>A string representing the list of models.</returns>
		public async Task<string> ListModels()
		{
			var resp = string.Empty;
			using (HttpClient client = new HttpClient())
			{
				client.DefaultRequestHeaders.Clear();
				client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", apiKey);

				var Message = await client.GetAsync(UrlModels);

				resp = await Message.Content.ReadAsStringAsync();
			}
			return resp;
		}
	}
}
