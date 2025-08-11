using SistemSekolahSMA.Models;
using System.Collections.Generic;

namespace SistemSekolahSMA.ViewModels
{
    public class MasterDataViewModel<T> where T : class
    {
        public List<T> Data { get; set; } = new List<T>();
        public int TotalCount { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string SearchTerm { get; set; }
        public string SortBy { get; set; }
        public string SortDirection { get; set; } = "asc";

        public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
        public bool HasPreviousPage => PageNumber > 1;
        public bool HasNextPage => PageNumber < TotalPages;
    }
}