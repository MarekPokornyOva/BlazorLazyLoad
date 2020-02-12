#region using
using System;
#endregion using

namespace LazyLoadedArea
{
	public class TimeProvider:ITimeProvider
	{
		public DateTime GetTime()
			=> DateTime.Now;
	}

	public interface ITimeProvider
	{
		DateTime GetTime();
	}
}
