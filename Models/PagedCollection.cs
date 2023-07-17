namespace Sanicompras.Models
{
    public class PagedCollection<T> where T : class
    {
        public List<T> Data { get; set; }  = new List<T>();
        public Uri NextPage { get; set; }
        public Uri PreviousPage { get; set; }

    }
}
