using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScriptureMemory.Server.Tools;
public class Bibles
{
    /// <summary>
    /// Bibles authorized to use from API.Bible
    /// Add a new one here and run the startup tasks to add the new Bible to my database
    /// </summary>
    public static Bible[] authorizedBibles =
    {
        new Bible()
        {
            Id = "de4e12af7f28f599-01",
            Abbreviation = "kjv",
            Name = "King James (Authorized) Version",
            Copyright = "PUBLIC DOMAIN except in the United Kingdom, where a Crown Copyright applies to printing the KJV. See http://www.cambridge.org/about-us/who-we-are/queens-printers-patent",
            Info = "This historical translation of the Holy Bible is brought to you in quality digital format by the Crosswire Bible Society (https://crosswire.org) and eBible.org (https://eBible.org).",
        },
        new Bible()
        {
            Id = "63097d2a0a2f7db3-01",
            Abbreviation = "nkjv",
            Name = "New King James Version",
            Copyright = "New King James Version®, Copyright© 1982, Thomas Nelson. All rights reserved.",
            Info = "Scripture taken from the New King James Version®. Copyright © 1982 by Thomas Nelson. Used by permission. All rights reserved.",
        },
        new Bible()
        {
            Id = "78a9f6124f344018-01",
            Abbreviation = "niv",
            Name = "New International Version 2011",
            Copyright = "The Holy Bible, New International Version® NIV® Copyright © 1973, 1978, 1984, 2011 by Biblica, Inc.® Used by Permission of Biblica, Inc.® All rights reserved worldwide.",
            Info = "<h3>About Biblica</h3> <p>Biblica is a global ministry innovating new ways to reach people with the Bible. Now in its third century of mission, Biblica continues to produce Bible translations and resources that power hundreds of Gospel ministries worldwide — all so that more people can discover the love of Jesus Christ.</p> <p>To learn more, visit <a href=\\\"http://biblica.com\\\">biblica.com</a> and <a href=\\\"http://facebook.com/Biblica\\\">facebook.com/Biblica</a>.</p>"
        },
        new Bible()
        {
            Id = "b8ee27bcd1cae43a-01",
            Abbreviation = "nasb",
            Name = "New American Standard Bible 1995",
            Copyright = "NEW AMERICAN STANDARD BIBLE® NASB® Copyright © 1960,1962,1963,1968,1971,1972,1973,1975,1977,1995 by The Lockman Foundation A Corporation Not for Profit La Habra, CA All Rights Reserved www.lockman.org",
            Info = "<p>The New American Standard Bible has been produced with the conviction that the words of Scripture, as originally penned in the Hebrew, Aramaic, and Greek, were inspired by God. Since they are the eternal Word of God, the Holy Scriptures speak with fresh power to each generation, to give wisdom that leads to salvation so that men and women may serve Christ for the glory of God.</p>"
        }
    };

    // Todo: Refactor to use data access in authorized Bibles in db, checking cache first
    public static Bible GetBible(string version)
    {
        return authorizedBibles.FirstOrDefault(b => b.Abbreviation == version)
            ?? throw new InvalidOperationException($"Bible {version} not found.");
    }

    public static Bible? TryGetBible(string version)
    {
        return authorizedBibles.FirstOrDefault(b => b.Abbreviation == version);
    }
}
