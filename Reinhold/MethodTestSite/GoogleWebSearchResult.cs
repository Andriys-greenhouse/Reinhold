using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace MethodTestSite
{
    public class GoogleWebSearchResult
    {
        public string kind { get; set; }
        public Url url { get; set; }
        public Queries queries { get; set; }
        public Context context { get; set; }
        public Searchinformation searchInformation { get; set; }
        public Item[] items { get; set; }
    }

    public class Url
    {
        public string type { get; set; }
        public string template { get; set; }
    }

    public class Queries
    {
        public Request[] request { get; set; }
        public Nextpage[] nextPage { get; set; }
    }

    public class Request
    {
        public string title { get; set; }
        public string totalResults { get; set; }
        public string searchTerms { get; set; }
        public int count { get; set; }
        public int startIndex { get; set; }
        public string inputEncoding { get; set; }
        public string outputEncoding { get; set; }
        public string safe { get; set; }
        public string cx { get; set; }
    }

    public class Nextpage
    {
        public string title { get; set; }
        public string totalResults { get; set; }
        public string searchTerms { get; set; }
        public int count { get; set; }
        public int startIndex { get; set; }
        public string inputEncoding { get; set; }
        public string outputEncoding { get; set; }
        public string safe { get; set; }
        public string cx { get; set; }
    }

    public class Context
    {
        public string title { get; set; }
    }

    public class Searchinformation
    {
        public float searchTime { get; set; }
        public string formattedSearchTime { get; set; }
        public string totalResults { get; set; }
        public string formattedTotalResults { get; set; }
    }

    public class Item
    {
        public string kind { get; set; }
        public string title { get; set; }
        public string htmlTitle { get; set; }
        public string link { get; set; }
        public string displayLink { get; set; }
        public string snippet { get; set; }
        public string htmlSnippet { get; set; }
        public string cacheId { get; set; }
        public string formattedUrl { get; set; }
        public string htmlFormattedUrl { get; set; }
        public Pagemap pagemap { get; set; }
    }

    public class Pagemap
    {
        public Hcard[] hcard { get; set; }
        public Cse_Thumbnail[] cse_thumbnail { get; set; }
        public Person[] person { get; set; }
        public Metatag[] metatags { get; set; }
        public Cse_Image[] cse_image { get; set; }
    }

    public class Hcard
    {
        public string role { get; set; }
        public string bday { get; set; }
        public string fn { get; set; }
        public string nickname { get; set; }
        public string category { get; set; }
    }

    public class Cse_Thumbnail
    {
        public string src { get; set; }
        public string width { get; set; }
        public string height { get; set; }
    }

    public class Person
    {
        public string role { get; set; }
    }

    public class Metatag
    {
        public string referrer { get; set; }
        public string ogimage { get; set; }
        public string ogimagewidth { get; set; }
        [JsonProperty("og:type")]
        public string ogtype { get; set; }
        public string ogtitle { get; set; }
        public string ogimageheight { get; set; }
        public string formatdetection { get; set; }
        public string twittercard { get; set; }
        public string ogsite_name { get; set; }
        public DateTime articlemodified_time { get; set; }
        public string viewport { get; set; }
        public string author { get; set; }
        public string oglocale { get; set; }
        public string ogurl { get; set; }
        public string ogdescription { get; set; }
        public string themecolor { get; set; }
        public string msvalidate01 { get; set; }
        public string twitterlabel1 { get; set; }
        public string twitterdata1 { get; set; }
        public string google { get; set; }
        public string twittertitle { get; set; }
        public string twitterdescription { get; set; }
        public string twitterimage { get; set; }
        public string ogimagetype { get; set; }
        public string fbpages { get; set; }
        public string fbapp_id { get; set; }
        public string twittersite { get; set; }
        public string msapplicationtilecolor { get; set; }
        public string msapplicationconfig { get; set; }
        public string msapplicationtileimage { get; set; }
        public string appleitunesapp { get; set; }
        public string applemobilewebapptitle { get; set; }
        public string twitterappidiphone { get; set; }
        public string twittercreator { get; set; }
        public string applicationname { get; set; }
        public string ver { get; set; }
        public string twitterappidipad { get; set; }
        public string applemobilewebappcapable { get; set; }
        public string ts { get; set; }
    }

    public class Cse_Image
    {
        public string src { get; set; }
    }
}
