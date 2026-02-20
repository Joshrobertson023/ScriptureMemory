using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScriptureMemoryLibrary
{
    public static class Enums
    {
        public enum Status
        {
            Active = 0,
            Inactive = 1,
            Deleted = 2
        };

        public enum CollectionsSort
        {
            Newest = 0,
            Title = 1,
            LastPracticed = 2,
            Completion = 3,
            Custom = 4
        };

        public enum NotificationType
        {
            System = 0,
            Welcome = 1
        }

        public enum PracticeStyle
        {
            FirstLetter = 0,
            FullText = 1,
            DragDrop = 2,
            Flashcards = 3
        }

        public enum ThemePreference
        {
            SystemDefault = 0,
            Light = 1,
            Dark = 2
        }

        public enum BibleVersion
        {
            Kjv = 0
        }

        public enum RelationshipStatus
        {
            Pending = 0,
            Accepted = 1,
            Blocked = 2
        }

        public enum SearchType
        {
            Verse,
            Passage,
            Collection,
            PublishedCollection,
            User
        }


        // Logging

        public enum SeverityLevel
        {
            Info = 0,
            Warning = 1,
            Error = 2,
            Critical = 3
        }

        public enum ActionType
        {
            Register,
            Login,
            Logout,

            Create,
            Edit,
            Delete,
            Publish,
            Update,

            View,
            Like,
            Report,

            CheckedUsernameAvailability,
            PracticeSession,
            ReadChapter,
            Search,
        }

        public enum EntityType // Entity being acted upon
        {
            User = 0,
            Collection = 1,
            Passage = 2,
            Note = 3,
            PublishedCollection = 4,
            PublishedPassage = 5,
            PublishedNote = 6,
            Verse = 7,
            Like = 8,
            Book = 9,
            Page = 10
        }
    }
}
