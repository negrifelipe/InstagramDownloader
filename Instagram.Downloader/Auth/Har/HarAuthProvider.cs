using Newtonsoft.Json.Linq;
using System.IO;
using System.Linq;

namespace Feli.Instagram.Downloader.Auth.Har
{
    public class HarAuthProvider
    {
        public static string GetAuthCredential(params string[] args)
        {
            var filePath = args[0];

            var text = File.ReadAllText(filePath);

            var @object = JObject.Parse(text);

            foreach(var entry in @object["log"]["entries"] as JArray)
            {
                var headers = entry["request"]["headers"] as JArray;

                return headers.FirstOrDefault(h => h["name"].ToString() == "cookie")["value"].ToString();
            }

            return null;
        }
    }
}
