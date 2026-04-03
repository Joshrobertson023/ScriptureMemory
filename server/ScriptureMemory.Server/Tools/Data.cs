using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScriptureMemory.Server.Tools;
public class Data
{
    public static string[] adminUsernames =
    {
        "Joshrobertson023",
    };

    public static string emailFromAddress = "therealjoshrobertson@gmail.com";

    public static int NOTIFICATION_SYSTEM_ID = 1;
    public static string notificationSystemName = "Scripture Memory Staff";
    public static string welcomeNotificationBody = "To interact with people you know, visit the search page to send them a friend request.";
    public static string savedNotificationBody = "Someone saved your collection, ";

    public static int NOTIFICATION_SYSTEM_SENDER_ID = 1;

    public static int MIN_PASSWORD_LENGTH = 11;

    public const int MAX_PASSAGES_PER_COLLECTION = 50;

    public const int MAX_COLLECTIONS_PER_USER_FREE = 5;
    public const int MAX_COLLECTIONS_PER_USER_PAID = 255;

    public const int MAX_PUBLISHED_COLLECTIONS_FREE = 5;
    public const int MAX_PUBLISHED_COLLECTIONS_PAID = 255;

    public const int MAX_SAVED_PUBLISHED_COLLECTIONS_FREE = 5;
    public const int MAX_SAVED_PUBLISHED_COLLECTIONS_PAID = 255;
}
