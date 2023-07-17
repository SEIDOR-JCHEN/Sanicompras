using Newtonsoft.Json;

namespace Sanicompras.Models
{
    public struct DefaultFilterValues
    {
        public const int Page = 1;
        public const int Limit = 10;
    }
    public class Filter
    {
        //public string? StartDate { get; set; }
        //public string? EndDate { get; set; }
        public int? Page { get; set; } = DefaultFilterValues.Page;
        public int? Limit { get; set; } = DefaultFilterValues.Limit;

        
        public Uri GetNextPage(HttpRequest Request)
        {
            
            string BaseURL = $"{Request.Scheme}://{Request.Host}{Request.Path}";
            string filters = string.Empty;
            return new Uri($"{BaseURL}?Page={(int)(Page + 1)}{GetAllParameters(Request)}");
        }

        public Uri GetPreviousPage(HttpRequest Request)
        {
            string BaseURL = $"{Request.Scheme}://{Request.Host}{Request.Path}";

            if (Page == 1)
            {
                return null;
            }
            else
            {
                return new Uri($"{BaseURL}?Page={(int)(Page - 1)}{GetAllParameters(Request)}");
            }
        }

        private string GetAllParameters(HttpRequest request)
        {
            string result = string.Empty;

            foreach(string key in request.Query.Keys)
            {
                if(key != "Page")
                {
                    result += $"&{key}={request.Query[key]}";
                }
            }
            return result;
        }

    }
}
