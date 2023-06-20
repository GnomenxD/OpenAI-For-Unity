using Cosmos.AI;
using Cosmos.AI.Open_AI;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

public class ExampleAI : MonoBehaviour
{
	[SerializeField] private string apiKey = ""; //do not upload this to github.

	[Header("Text Completion")]
	[SerializeField, TextArea(1, 2)] private string textPrompt;
	[SerializeField, TextArea(4, 10)] private string textOutput;

	[Header("Image Generation")]
	[SerializeField, TextArea(1, 2)] private string imagePromt;
	[SerializeField] private SpriteRenderer spriteRenderer;

	private OpenAI ai;

	private async void Start()
	{
		ai = new OpenAI(apiKey);
		await TextGeneration();
		await ImageGeneration();
	}

	private async Task TextGeneration()
	{
		TextRequest textRequest = new TextRequest(
			textPrompt,
			Model.Davinci,
			maxTokens: 100);
		TextResponse textResponse = await ai.TextCompletion.Request(textRequest);

		textOutput = textResponse.Response;
	}

	private async Task ImageGeneration()
	{
		if (spriteRenderer == null)
		{
			Debug.LogWarning($"Assign a SpriteRenderer to see the image.", this);
			return;
		}

		ImageRequest request = new ImageRequest(
			imagePromt,
			1,
			size: ImageSize.p256);
		ImageResponse response = await ai.ImageGeneration.Request(request);
		List<Sprite> sprites = await response.FetchSprites();

		if (sprites.Count > 0)
		{
			spriteRenderer.sprite = sprites[0];
		}
	}
}
