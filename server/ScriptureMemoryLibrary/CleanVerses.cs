//using System.Diagnostics;

//namespace ScriptureMemoryLibrary;

//public class CleanVerses
//{
//    private readonly IVerseData _verseData;

//    public CleanVerses(IVerseData verseData)
//    {
//        _verseData = verseData;
//    }

//    public async Task StartAsync(CancellationToken cancellationToken)
//    {
//        int verseId = 62331;
//        for (verseId = 62331; verseId <= 94887; verseId++)
//        {
//            try
//            {
//                Verse verse = await _verseData.GetVerseFromRowNum(verseId);
//                if (verse != null)
//                {
//                    var oldText = verse.Text;
//                    verse.Text = verse.Text.Replace("\n", " ");
//                    await _verseData.UpdateVerseText(verse.Text, verseId);
//                    double percentFinished = ((double)verseId - 62331) / (94887.0 - 62331.0);
//                    string finished = (percentFinished * 100).ToString("F2") + "%";
//                    Debug.WriteLine("\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n");
//                    Debug.WriteLine($"\n\nCleaned verse {verseId} from \n{oldText}\nto\n{verse.Text}\n\n --- {finished}% ---");
//                }
//            }
//            catch (Exception ex)
//            {
//                Debug.WriteLine(ex.Message);
//            }
//        }

//        return;
//    }

//    public async Task StopAsync(CancellationToken cancellationToken)
//    {
//        return;
//    }
//}
