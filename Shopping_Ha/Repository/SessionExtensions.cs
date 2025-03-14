using Newtonsoft.Json;
using System.Text.Json.Serialization;

namespace Shopping_Ha.Repository
{
	public static	 class SessionExtensions
	{
		public static void SeJson(this ISession session,string key,object value)
		{
			session.SetString(key, JsonConvert.SerializeObject(value));
		}
		public static T GetJson<T>(this ISession session,string key)
		{
			var sessionData = session.GetString(key);
			return sessionData == null ? default(T) : JsonConvert.DeserializeObject<T>(sessionData);
		}
	}

}
