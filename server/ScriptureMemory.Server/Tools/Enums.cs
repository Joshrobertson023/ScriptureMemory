using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScriptureMemory.Server.Tools
{
    public static class Enums
    {
        public enum UserRole
        {
            User = 0,
            Admin = 1,
            SuperAdmin = 2
        }

        public enum BibleSyncEvent
        {
            Queued,
            Started,
            Stopped,
            Completed,
            Cancelled,
            Progress,
            StartedRemoval,
            CompletedRemoval
        }

        public enum SyncDestination
        {
            Database,
            Redis,
            Files
        }

        public enum AdminActionType
        {
            VodAdded = 3,
            VodDeleted = 4,
        }

        public enum CollectionVisibility
        {
            Private = 0,
            Friends = 1,
            Public = 2
        }

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
            Welcome = 1,
            CollectionSaved = 2,
        }

        public enum ApprovedStatus
        {
            UnderReview = 0,
            Approved = 1,
            Rejected = 2,
            Other = 3
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
            Any,
            Verse,
            Passage,
            Collection,
            PublishedCollection,
            User
        }

        public enum SearchResultType
        {
            ExactPassage,
            SemanticVerse
        }


        // Logging

        public enum SeverityLevel
        {
            Info = 0,
            Warning = 1,
            Error = 2,
            Critical = 3
        }

        public enum SecurityLogType
        {
            Register,
            Login,
            Logout,
            PasswordReset,
            EmailChange,
            TokenIssued,
            TokenLogin,
            
            RoleChange,
            
            View
        }

        public enum AdminLogType
        {
            Delete,
            Add,
            SuspendPublicNote,
            StartSync,
            StopSync,
        }

        public enum ActivityLogType
        {
            Create,
            View,
            Edit,
            Delete,
            Update,
            Save,
            Search,
            LeaveNote,
            Like,
            ReportUser,
            ReportBug,
            Highlight,
            Bookmark,
            
            SendFriendRequest,
            AcceptFriendRequest,
        }

        public enum EntityType // Entity being acted upon for logs
        {
            User,
            Collection,
            Passage,
            Note,
            PublicNote,
            Verse,
            Like,
            Book,
            Page,
            Banner,
            Notification,
            Email
        }
    }
}
