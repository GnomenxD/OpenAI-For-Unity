namespace Cosmos.AI.Open_AI
{
	/// <summary>
	/// Represents a base class for OpenAI tools.
	/// </summary>
	public abstract class BaseTool
	{
		private readonly OpenAI ai;

		/// <summary>
		/// Gets the API key associated with the OpenAI instance.
		/// </summary>
		protected string ApiKey => ai.ApiKey;

		/// <summary>
		/// Constructs a new instance of the BaseTool class.
		/// </summary>
		/// <param name="ai">The OpenAI instance to associate with the tool.</param>
		public BaseTool(OpenAI ai)
		{
			this.ai = ai;
		}
	}
}
