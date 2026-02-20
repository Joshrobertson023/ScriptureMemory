    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    namespace DataAccess.Models;
    public class Collection
    {
        public int CollectionId { get; set; }
        public string AuthorUsername { get; set; }
        public string Username { get; set; }
        public string Title { get; set; }
        public string Visibility { get; set; }
        public DateTime DateCreated { get; set; }
        public int OrderPosition { get; set; }
        public float? ProgressPercent { get; set; }
        public float? AverageProgressPercent { get; set; }
        public int NumVerses
        {
            get
            {
                return UserVerses.Count;
            }
        }
        public List<UserPassage> UserVerses { get; set; } = new();
        public List<CollectionNote> Notes { get; set; } = new();
    }
