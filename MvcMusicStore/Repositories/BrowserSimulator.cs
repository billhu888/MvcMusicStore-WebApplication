namespace MvcMusicStore.Repositories
{
    public class BrowserSimulator
    {
        class Session
        {
            public Dictionary <string, string> KeyValuePairs = new Dictionary <string, string> ();

            public void SetString(string key, string value)
            {
                KeyValuePairs.Add(key, value);
            }

            public string GetString(string key)
            {
                if (KeyValuePairs.ContainsKey(key))
                {
                    return KeyValuePairs[key];
                }   
                else
                {
                    return null;
                }
            }
        }

        class HttpContext
        {
            public Session Session { get; set; } = new Session ();
        }

        class Browser
        {
            public HttpContext Client { get; set; } = new HttpContext();
            public String Url { get; set; }
        }

        public void Pack()
        {
            Browser me = new Browser ();
            me.Url = "www.google.com";
        }
    }
}