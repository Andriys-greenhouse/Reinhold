using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MethodTestSite
{
    public class WebModule
    {
        public static async Task<string> WebSearch(string KeyWord)
        {
            using (HttpClient client = new HttpClient())
            {
                string searchTerm = KeyWord.Replace(" ", "+");
                HttpResponseMessage response = await client.GetAsync("https://www.googleapis.com/customsearch/v1?key=AIzaSyD1zMkt3fkpb5-AArVhyLTEX4vQzgM_zl4&cx=f0aa652047963924f&q=" + searchTerm);
                return await response.Content.ReadAsStringAsync();
            }
        }

        public static List<MyWebSearchResult> GoogleWebSearchResultConvert(string GWSR)
        {
            List<MyWebSearchResult> final = new List<MyWebSearchResult>();
            GoogleWebSearchResult input = JsonConvert.DeserializeObject<GoogleWebSearchResult>(GWSR);
            foreach (Item item in input.items)
            {
                try
                {
                    final.Add(new MyWebSearchResult(item.title, item.pagemap.metatags[0].ogtype, item.snippet, item.link));
                }
                catch (Exception e)
                {
                    final.Add(new MyWebSearchResult(item.title, null, item.snippet, item.link));
                }
            }
            return final;
        }

        public static async Task<string> News(string KeyWord)
        {
            News data;
            StringBuilder sb = new StringBuilder("Articles found:");
            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = await client.GetAsync(@"https://newsapi.org/v2/everything?apiKey=63b3a441735f47609eb041ac16104b8b&language=en&sortBy-popularity&q=" + KeyWord.Replace(" ", "+"));
                data = JsonConvert.DeserializeObject<News>(await response.Content.ReadAsStringAsync());
            }
            for (int i = 0; i < 5; i++)
            {
                sb.Append($"\n{data.articles[i].title}\n{data.articles[i].description}\n({data.articles[i].url})\n");
            }
            return sb.ToString();
        }

        //method untested and unusable due to time required for completing of a given task
        public static async Task<string> WebsiteSearch(string StartUrl, string RegexToSearch, int Dept)//dept from 1 to 10
        {
            if (Dept < 1 || Dept > 10) { throw new ArgumentException("Search dept must be between 1 and 10."); }
            Regex rxSublinks = new Regex(@"<a.*href\=(\{0}|\')(?<ImgLink>.[^{0}']*).*alt\=(\{0}|\')(?<ImgDescr>.[^'{0}]*).*><\/a>|<a.*href\=(\{0}|\')(?<TextLink>.[^{0}']*).*>(?<TextContent>.*)<\/a>|<button.*onclick\=(\{0}|\')(.*(\{0}|\')(?<ButtLocation>.[^{0}']*)(\{0}|\')).*>(?<ButtContent>.*)<\/button>".Replace("{0}", "\""));
            List<WebsiteSearchResult> final = new List<WebsiteSearchResult>();
            List<WebsiteSearchResult> operational = new List<WebsiteSearchResult>();
            List<WebsiteSearchResult> last = new List<WebsiteSearchResult>() { new WebsiteSearchResult("", StartUrl, WebsiteSearchResultType.Text) };
            List<WebsiteSearchResult> faulty = new List<WebsiteSearchResult>();
            try
            {
                WebsiteSearchResult found = new WebsiteSearchResult();
                string content;
                using (HttpClient client = new HttpClient())
                {
                    for (int i = 0; i < Dept + 1; i++)
                    {
                        foreach (WebsiteSearchResult result in last)
                        {
                            if (result.Link.Substring(0,7) == "mailto:") { result.Code = result.Link; }
                            else
                            {
                                try
                                {
                                    HttpResponseMessage response = await client.GetAsync(result.Link);
                                    content = await response.Content.ReadAsStringAsync();
                                    result.Code = content;
                                    foreach (Match match in rxSublinks.Matches(content))
                                    {
                                        found.Update(match.Groups["ImgDescr"].Value, match.Groups["ImgLink"].Value, WebsiteSearchResultType.Image);
                                        found.Update(match.Groups["TextContent"].Value, match.Groups["TextLink"].Value, WebsiteSearchResultType.Text);
                                        found.Update(match.Groups["ButtContent"].Value, match.Groups["ButtLocation"].Value, WebsiteSearchResultType.Button);
                                        operational.Add(new WebsiteSearchResult(found.Text, found.Link, found.Type));
                                    }
                                }
                                catch (HttpRequestException)
                                {
                                    faulty.Add(result);
                                }
                                catch (InvalidOperationException)
                                {
                                    faulty.Add(result);
                                }
                            }
                        }
                        foreach (WebsiteSearchResult res in last)
                        {
                            final.Add(res);
                        }
                        last.Clear();
                        operational = operational.Distinct().ToList();
                        foreach (WebsiteSearchResult res in operational)
                        {
                            last.Add(res);
                        }
                        operational.Clear();
                    }
                    foreach (WebsiteSearchResult res in last)
                    {
                        final.Add(res);
                    }
                    last.Clear();
                }
                final = final.Distinct().ToList();
                foreach (WebsiteSearchResult item in faulty)
                {
                    final.Remove(item);
                }


                Regex rxUser = new Regex(RegexToSearch);
                StringBuilder sb = new StringBuilder("Results from web:");
                int count;
                foreach (WebsiteSearchResult result in final)
                {
                    count = rxUser.Matches(result.Code).Count;
                    if (count > 0) { sb.Append($"\n({count}) {result.Link}"); }
                }
                return sb.ToString();
            }
            catch (Exception)
            {
                throw new ArgumentException("Invalid URL.");
            }
        }
    }

    public enum WebsiteSearchResultType { Text, Image, Button }
    public class WebsiteSearchResult
    {
        public string Link { get; set; }
        public string Text { get; set; }
        public WebsiteSearchResultType Type { get; set; }
        public string Code { get; set; }
        public WebsiteSearchResult(string aText, string aLink, WebsiteSearchResultType aType)
        {
            Link = aLink;
            Text = aText;
            Type = aType;
        }
        public WebsiteSearchResult() { }

        public void Update(string aText, string aLink, WebsiteSearchResultType aType)
        {
            if (aText == "" || aLink == "") { return; }
            else
            {
                Link = aLink;
                Text = aText;
                Type = aType;
            }
        }
    }

    public class MyWebSearchResult
    {
        public string Title { get; set; }
        public string Kind { get; set; }
        public string Snippet { get; set; }
        public string Link { get; set; }
        public MyWebSearchResult(string aTitle, string aKind, string aSnippet, string aLink)
        {
            Title = aTitle;
            Kind = aKind;
            Snippet = aSnippet;
            Link = aLink;
        }
        public string OutputText(int aIndexOfResultToDisplay = 0) //zero for none
        {
            if (aIndexOfResultToDisplay < 0) { throw new ArgumentException("Index in list can't be lower than zero."); }
            string sth;
            if (aIndexOfResultToDisplay == 0 && Kind != null) { sth = $"({Kind}) "; }
            else if (aIndexOfResultToDisplay != 0 && Kind != null) { sth = $"({aIndexOfResultToDisplay}, {Kind}) "; }
            else if (aIndexOfResultToDisplay != 0 && Kind == null) { sth = $"({aIndexOfResultToDisplay}) "; }
            else { sth = ""; }
            return $"{sth}{Title}\n{Snippet}";
        }
    }
}