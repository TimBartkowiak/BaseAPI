using System.Collections.Generic;

namespace BaseAPI.Utils
{
    public class Paged<T> where T : class
    {
        private const int MaxPageSize = 500;
        private int _pageSize;

        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = (value > MaxPageSize) ? MaxPageSize : value;
        }
        
        public int CurrentPage { get; set; }
        public int TotalItems { get; set; }
        public int TotalPages { get; set; }
        public IList<T> Items { get; set; }

        public Paged()
        {
            Items = new List<T>();
        }
    }
}