using System.Collections.Generic;
using System.Linq;

using Yuzu.Metadata;

namespace Yuzu.DictOfObjects
{
	public static class DictOfObjects
	{
		public static CommonOptions Options = new CommonOptions();

		public static Dictionary<string, object> Pack<T>(T obj) =>
			Meta.Get(obj.GetType(), Options).Items.ToDictionary(yi => yi.Tag(Options), yi => yi.GetValue(obj));

		public static T Unpack<T>(Dictionary<string, object> d)
		{
			var meta = Meta.Get(typeof(T), Options);
			var result = meta.Factory();
			foreach (var yi in meta.Items) {
				if (d.TryGetValue(yi.Tag(Options), out object itemValue))
					yi.SetValue(result, itemValue);
			}
			return (T)result;
		}
	}
}