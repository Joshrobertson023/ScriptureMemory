namespace ScriptureMemory.Server.Services;

public static class BibleHelper
{
    /// <summary>
    /// Merges Bibles in db and Bibles authorized to use without duplicates, conserving bool Authorized and other
    /// properties
    /// </summary>
    /// <param name="dbBibles"></param>
    /// <param name="authorizedBibles"></param>
    /// <returns></returns>
    public static (List<Bible>, List<Bible>) MergeBiblesToSet(List<Bible> dbBibles, List<Bible> authorizedBibles)
    {
        HashSet<string> authorizedBibleIds = authorizedBibles.Select(b => b.Id).ToHashSet();
        HashSet<string> dbBibleIds = dbBibles.Select(b => b.Id).ToHashSet();

        List<Bible> allBibles = dbBibles;
        HashSet<string> allBibleIds = dbBibles.Select(b => b.Id).ToHashSet();
        foreach (var bible in authorizedBibles)
        {
            if (!allBibleIds.Contains(bible.Id))
            {
                allBibles.Add(bible);
            }
        }
                
        List<Bible> biblesToSet = new();
        List<Bible> biblesNotAuthorizedButActive = new();
        HashSet<string> biblesToSetIds = new();

        foreach (var bible in allBibles)
        {
            if (!biblesToSetIds.Contains(bible.Id))
            {
                bool authorized = authorizedBibleIds.Contains(bible.Id);
                
                if (!authorized && bible.Active)
                    biblesNotAuthorizedButActive.Add(bible);
                
                bible.Authorized = authorized;
                        
                biblesToSet.Add(bible);
                biblesToSetIds.Add(bible.Id);
            }
        }

        return (biblesToSet, biblesNotAuthorizedButActive);
    }
}