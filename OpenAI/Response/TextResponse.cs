using Cosmos.AI.Open_AI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Cosmos.AI
{
	/// <summary>
	/// Represents a response containing text answers.
	/// </summary>
	public class TextResponse : IEnumerable<TextResponse.Answer>
	{
		private readonly long created;
		private readonly List<Answer> responses;
		private readonly string model;
		private readonly Usage usage;

		/// <summary>
		/// Gets the timestamp when the response was created.
		/// </summary>
		public long Created => created;

		/// <summary>
		/// Gets the first response in the list of answers.
		/// </summary>
		public string? Response => (responses.Count > 0 ? responses[0].Response : null);

		/// <summary>
		/// Gets the list of all the answers in the response.
		/// </summary>
		public List<Answer> Responses => responses;

		/// <summary>
		/// Gets the model used for generating the response.
		/// </summary>
		public string Model => model;

		/// <summary>
		/// Gets the usage information associated with the response.
		/// </summary>
		public Usage Usage => usage;

		/// <summary>
		/// Constructs a new TextResponse instance.
		/// </summary>
		public TextResponse(long created, List<Answer> responses, string model, Usage usage)
		{
			this.created = created;
			this.responses = responses;
			this.model = model;
			this.usage = usage;
		}

		/// <summary>
		/// Returns the concatenated output of the response, model, and usage information.
		/// </summary>
		public string Output()
		{
			StringBuilder sb = new StringBuilder();
			sb.Append(this.ToString());
			sb.Append($"{Model}");
			sb.Append($"{Usage}");
			return sb.ToString();
		}

		/// <summary>
		/// Returns a string representation of the TextResponse instance.
		/// </summary>
		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();
			foreach (var response in this)
			{
				sb.AppendLine(response.ToString());
			}
			return sb.ToString();
		}

		/// <summary>
		/// Returns an enumerator that iterates through the answers in the response.
		/// </summary>
		public IEnumerator<Answer> GetEnumerator()
		{
			return responses.GetEnumerator();
		}

		/// <summary>
		/// Returns an enumerator that iterates through the answers in the response.
		/// </summary>
		IEnumerator IEnumerable.GetEnumerator()
		{
			yield return GetEnumerator();
		}

		/// <summary>
		/// Generates a TextResponse instance from the provided response content.
		/// </summary>
		public static TextResponse Generate(TextResponseContent content)
		{
			Answer[] responses = new Answer[content.choices.Length];
			foreach (Choice choice in content.choices)
			{
				responses[choice.index] = new Answer(choice.text, choice.finish_reason);
			}
			return new TextResponse(content.created, responses.ToList(), content.model, content.usage);
		}

		/// <summary>
		/// Represents a single answer in the TextResponse.
		/// </summary>
		public readonly struct Answer
		{
			private readonly string response;
			private readonly string finishReason;

			/// <summary>
			/// Gets the response text.
			/// </summary>
			public string Response => response;

			/// <summary>
			/// Gets the reason indicating the completion or termination of the response.
			/// </summary>
			public string FinishReason => finishReason;

			/// <summary>
			/// Formats the response by replacing periods with periods followed by a new line.
			/// </summary>
			public string Format() => response.Replace(".", "." + Environment.NewLine);

			/// <summary>
			/// Constructs a new Answer instance.
			/// </summary>
			public Answer(string? response, string? finishReason)
			{
				if (response != null)
					this.response = Regex.Replace(response.Trim(), @"^\s+$[\r\n]*", string.Empty, RegexOptions.Multiline);
				else
					this.response = string.Empty;
				if (finishReason != null)
					this.finishReason = finishReason.Trim();
				else
					this.finishReason = string.Empty;
			}

			/// <summary>
			/// Returns a string representation of the Answer instance.
			/// </summary>
			public override string ToString() => response;
		}
	}
}
