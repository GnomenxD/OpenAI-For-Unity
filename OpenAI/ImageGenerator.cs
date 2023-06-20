using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Cosmos.AI.Open_AI
{
	/// <summary>
	/// This class represents an image generator that uses the OpenAI API to generate images based on text prompts.
	/// </summary>
	public class ImageGenerator : BaseTool
	{
		public ImageGenerator(OpenAI ai) : base(ai)
		{
		}

		/// <summary>
		/// Sends a request to the OpenAI API for generating an image based on the provided text prompt.
		/// </summary>
		/// <param name="request">The image generation request parameters.</param>
		/// <returns>The generated image response.</returns>
		public async Task<ImageResponse> Request(ImageRequest request)
		{
			Debug.Log($"Image generation request {request}");
			ImageResponseContent resp = await Request(ApiKey, OpenAI.UrlImageGeneration, request.ConstructBody());
			return ImageResponse.Generate(resp);
		}

		/// <summary>
		/// Sends a request to the OpenAI API for generating an image based on optional parameters.
		/// </summary>
		/// <param name="prompt">The text prompt for generating the image.</param>
		/// <param name="amount">The number of images to generate.</param>
		/// <param name="size">The size of the generated image.</param>
		/// <returns>The generated image response.</returns>
		public async Task<ImageResponse> Request(string? prompt = default, short? amount = default, string size = default) => await Request(new ImageRequest(prompt, amount.GetValueOrDefault(), size.Convert()));

		private static async Task<ImageResponseContent> Request(string apiKey, string url, ImageRequestBody body)
		{
			ImageResponseContent resp = new ImageResponseContent();
			using (HttpClient client = new HttpClient())
			{
				client.DefaultRequestHeaders.Clear();

				client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);

				HttpResponseMessage message = await client.PostAsync(
					url,
					new StringContent(JsonConvert.SerializeObject(body),
					Encoding.UTF8, "application/json"));

				// Log the status code and reason phrase
				if (message.IsSuccessStatusCode)
					Debug.Log($"{(int)message.StatusCode} - {message.ReasonPhrase}");
				else
					Debug.LogWarning($"{(int)message.StatusCode} - {message.ReasonPhrase}");

				if (message.IsSuccessStatusCode)
				{
					string content = await message.Content.ReadAsStringAsync();
					resp = JsonConvert.DeserializeObject<ImageResponseContent>(content);
				}
			}
			return resp;
		}
	}
}
