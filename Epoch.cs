#region License
//   Copyright 2012 Andrius Bentkus
//   without provided license assumed public without limitations
//   source: https://gist.github.com/txdv/1095252
#endregion
using System;
 
namespace Epoch
{
	public class Epoch
	{
		static readonly DateTime epochStart = new DateTime(1970, 1, 1, 0, 0, 0);
		
		static readonly DateTimeOffset epochDateTimeOffset = new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero);
		
		public static DateTime FromUnix(int secondsSinceepoch)
		{
			return epochStart.AddSeconds(secondsSinceepoch);
		}
		
		public static DateTimeOffset FromUnix(int secondsSinceEpoch, int timeZoneOffsetInMinutes)
		{
			var utcDateTime = epochDateTimeOffset.AddSeconds(secondsSinceEpoch);
			var offset = TimeSpan.FromMinutes(timeZoneOffsetInMinutes);
			return new DateTimeOffset(utcDateTime.DateTime.Add(offset), offset);
		}
		
		public static int ToUnix(DateTime dateTime)
		{
			return (int)(dateTime - epochStart).TotalSeconds;
		}
		
		public static int Now {
			get {
				return (int)(DateTime.UtcNow - epochStart).TotalSeconds;
			}
		}
	}
	
	namespace Extensions
	{
		public static class EpochExtensions
		{
			public static int ToUnix(this DateTime dateTime)
			{
				return Epoch.ToUnix(dateTime);
			}
			
			public static DateTime FromUnix(this int secondsSinceEpoch)
			{
				return Epoch.FromUnix(secondsSinceEpoch);
			}
			
			public static DateTimeOffset FromUnix(this int secondsSinceEpoch, int timeZoneOffsetInMinutes)
			{
				return Epoch.FromUnix(secondsSinceEpoch, timeZoneOffsetInMinutes);
			}
			
		}
	}
}
