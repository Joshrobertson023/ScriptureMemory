// using ScriptureMemory.Server.Data.DataAccess.Bible;
//
// namespace ScriptureMemory.Server.Services;
//
// public class BibleSyncerAutoScheduler(
//     BibleSyncerQueue queue,
//     BibleSyncerProgressLogger progressLogger,
//     ILogger<BibleSyncerAutoScheduler> logger,
//     BibleApi bibleApi,
//     BibleSyncLogData syncLogData) : BackgroundService
// {
//     private static readonly TimeSpan _syncInterval = TimeSpan.FromDays(29);
//     
//     protected override async Task ExecuteAsync(CancellationToken cancellationToken)
//     {
//         while (!cancellationToken.IsCancellationRequested)
//         {
//             var authorizedBibles = await bibleApi.GetAuthorizedBibles();
//             var lastSyncEntries = await syncLogData.GetLastSyncProgressForBibles();
//
//             var activeBiblesNotSynced = authorizedBibles
//                 .Where(b => b.Active)
//                 .Where(b => !lastSyncEntries.TryGetValue(b.Id, out _))
//                 .ToList();
//
//             var lastSyncEntryBibles = new List<Bible>();
//             foreach (var entry in lastSyncEntries)
//             {
//                 lastSyncEntryBibles.Add(authorizedBibles.First(b => lastSyncEntries.TryGetValue(b.Id, out _)));
//             }
//             
//             List<Bible> biblesOverdueForSync = activeBiblesNotSynced
//                 .AddRange()
//         }
//     }
// }