using Cosmos.AI.Open_AI;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine.Networking;
using UnityEngine;

namespace Cosmos.AI
{
	/// <summary>
	/// Represents a response containing images.
	/// </summary>
	public class ImageResponse : IEnumerable<Texture2D>
	{
		private bool fetched;
		private readonly long created;
		private readonly string[] urls;
		private List<Texture2D> textures;

		/// <summary>
		/// Gets the timestamp when the response was created.
		/// </summary>
		public long Created => created;

		/// <summary>
		/// Gets the URL of the first image.
		/// </summary>
		public string? Url => urls.Length > 0 ? Urls[0] : null;

		/// <summary>
		/// Gets an array of URLs for all the images.
		/// </summary>
		public string[] Urls => urls;

		/// <summary>
		/// Gets the first image in the response.
		/// </summary>
		public Texture2D? Texture => Textures.Count > 0 ? Textures[0] : null;


		/// <summary>
		/// Gets a list of all the images in the response.
		/// </summary>
		public List<Texture2D> Textures
		{
			get
			{
				if (!fetched || textures == null)
				{
					Debug.LogWarning("Textures have not been fetched. Remember to invoke FetchTextures() before requesting images.");
					return new List<Texture2D>();
				}
				return textures;
			}
		}

		/// <summary>
		/// Gets the number of images in the response.
		/// </summary>
		public int Count => Textures.Count;

		/// <summary>
		/// Constructs a new ImageResponse instance.
		/// </summary>
		public ImageResponse(long created, string[] urls)
		{
			this.created = created;
			this.urls = urls;
		}

		/// <summary>
		/// Returns an enumerator that iterates through the images.
		/// </summary>
		public IEnumerator<Texture2D> GetEnumerator()
		{
			return Textures.GetEnumerator();
		}

		/// <summary>
		/// Returns an enumerator that iterates through the images.
		/// </summary>
		IEnumerator IEnumerable.GetEnumerator()
		{
			yield return GetEnumerator();
		}

		/// <summary>
		/// Fetches and generates <see cref="UnityEngine.Texture2D"/> objects for all the images in the response.
		/// </summary>
		/// <returns>A list of fetched sprites.</returns>
		public async Task<List<Texture2D>> FetchTextures()
		{
			fetched = true;
			textures = new List<Texture2D>();
			foreach (string data in urls)
			{
				if (data == null)
					continue;
				using (UnityWebRequest webRequest = UnityWebRequestTexture.GetTexture(data))
				{
					UnityWebRequestAsyncOperation asyncOp = webRequest.SendWebRequest();

					while (!asyncOp.isDone)
						await Task.Yield();

					if (webRequest.result != UnityWebRequest.Result.Success)
					{
						Debug.LogWarning(webRequest.error);
					}
					else
					{
						Texture2D texture = ((DownloadHandlerTexture)webRequest.downloadHandler).texture;
						Debug.Log(webRequest.result);
					}
				}
			}
			return textures;
		}

		/// <summary>
		/// Fetches and generates <see cref="UnityEngine.Sprite"/> objects for all the images in the response.
		/// </summary>
		/// <returns>A list of fetched sprites.</returns>
		public async Task<List<Sprite>> FetchSprites()
		{
			if (!fetched || textures == null)
				await FetchTextures();

			List<Sprite> sprites = new List<Sprite>();
			foreach(Texture2D texture in textures)
			{
				Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
				sprites.Add(sprite);
			}
			return sprites;
		}

		/// <summary>
		/// Generates an ImageResponse instance from the provided response content.
		/// </summary>
		public static ImageResponse Generate(ImageResponseContent resp)
		{
			if (resp.data == null)
			{
				Debug.LogError($"No returned data.");
			}
			string[] urls = new string[resp.data != null ? resp.data.Count : 0];
			for (int i = 0; i < urls.Length; i++)
			{
				urls[i] = resp.data[i].url;
			}
			return new ImageResponse(resp.created, urls);
		}
	}
}
