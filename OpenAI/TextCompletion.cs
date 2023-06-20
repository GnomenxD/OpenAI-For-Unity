using Newtonsoft.Json;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Cosmos.AI.Open_AI
{
	/// <summary>
	/// Provides functionality to perform text completions.
	/// </summary>
	public class TextCompletion : BaseTool
	{
		public TextCompletion(OpenAI ai) : base(ai)
		{
		}

		/// <summary>
		/// Creates a completion for the provided prompt and parameters.
		/// </summary>
		/// <param name="request">The text request containing the prompt and parameters.</param>
		/// <returns>The generated text response.</returns>
		public async Task<TextResponse> Request(TextRequest request)
		{
			Debug.Log($"Text completion request {request}");
			TextResponseContent content = await Request(ApiKey, OpenAI.UrlTextCompletion, request.ConstructBody());
			return TextResponse.Generate(content);
		}

		/// <summary>
		/// Creates a completion for the provided prompts and parameters.
		/// </summary>
		/// <param name="prompts">The prompts to be used in the text completion.</param>
		/// <param name="model">The model to use for the completion.</param>
		/// <param name="suffix">The suffix to be added to each prompt.</param>
		/// <param name="maxTokens">The maximum number of tokens in the completion.</param>
		/// <param name="temperature">The temperature to control the randomness of the completion.</param>
		/// <param name="p">The p-value to control the randomness of the completion.</param>
		/// <param name="amount">The number of completions to generate.</param>
		/// <param name="echo">Whether to include the prompt in the completion response.</param>
		/// <param name="stopSequence">The sequence of tokens at which to stop the completion.</param>
		/// <returns>The generated text response.</returns>
		public async Task<TextResponse> Request(Prompt prompts, Model model = Model.Davinci, string suffix = default, int maxTokens = 10, double temperature = 0.7d, double p = 1.0d, int amount = 1, bool echo = default, string stopSequence = default) => await Request(new TextRequest(prompts, model, suffix, maxTokens, temperature, p, amount, echo, stopSequence));

		private static async Task<TextResponseContent?> Request(string apiKey, string url, TextRequestBody body)
		{
			TextResponseContent resp = new TextResponseContent();
			using (HttpClient client = new HttpClient())
			{
				client.DefaultRequestHeaders.Clear();
				client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);

				System.Diagnostics.Stopwatch sw = System.Diagnostics.Stopwatch.StartNew();
				HttpResponseMessage message = await client.PostAsync(
					url,
					new StringContent(JsonConvert.SerializeObject(body),
					Encoding.UTF8, "application/json"));
				sw.Stop();

				if (message.IsSuccessStatusCode)
					Debug.Log($"{(int)message.StatusCode} - {message.ReasonPhrase} - {sw.Elapsed.TotalSeconds:F2}s");
				else
					Debug.LogError($"{(int)message.StatusCode} - {message.ReasonPhrase} - {sw.Elapsed.TotalSeconds:F2}s");

				if (message.IsSuccessStatusCode)
				{
					string content = await message.Content.ReadAsStringAsync();
					Debug.Log($"{content}");
					resp = JsonConvert.DeserializeObject<TextResponseContent>(content);
				}
			}
			return resp;
		}
	}
}
