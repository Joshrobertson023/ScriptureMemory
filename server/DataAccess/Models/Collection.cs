    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
using static ScriptureMemoryLibrary.Enums;

namespace DataAccess.Models;
    public class Collection
    {
        public int CollectionId { get; set; }
        public int UserId { get; set; }
        public string Title { get; set; } = string.Empty;
        public CollectionVisibility Visibility { get; set; }
        public DateTime DateCreated { get; set; }
        public int OrderPosition { get; set; }
        public bool IsFavorites { get; set; }
        public string? Description { get; set; }
        public int? AuthorId { get; set; }  // If a saved published collection
        public string? Author { get; set; } // If a saved published collection
        public float? ProgressPercent { get; set; }
        //public float? AverageProgressPercent { get; set; }
        public int NumVerses
        {
            get
            {
                return Passages.Count;
            }
        }
        public List<UserPassage> Passages { get; set; } = new();
        public List<CollectionNote> Notes { get; set; } = new();
    }
