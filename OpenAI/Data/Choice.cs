﻿namespace Cosmos.AI.Open_AI
{
	public class Choice
	{
		public string text { get; set; }
		public int index { get; set; }
		public object? logprobs { get; set; }
		public string? finish_reason { get; set; }
	}
}