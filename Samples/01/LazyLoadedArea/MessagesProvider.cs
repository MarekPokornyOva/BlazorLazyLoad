namespace LazyLoadedArea
{
	public class CircuitMessagesProvider:IMessageProvider
	{
		int _index = 0;
		static string[] _messages = new string[] { "That's great assembly lazy load works.", "It's cool the services registration and injection works too!" };

		public string GetMessage()
		{
			_index++;
			if (_index>=_messages.Length)
				_index=0;
			return _messages[_index];
		}
	}

	public interface IMessageProvider
	{
		string GetMessage();
	}
}
