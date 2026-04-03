using DataAccess.Models;
using DataAccess.Requests;
using System.Net.Http.Json;
using static ScriptureMemory.Server.Tools.Enums;

namespace ScriptureMemory.IntegrationTests;

public class VerseTests : BaseIntegrationTest
{
    public VerseTests(IntegrationTestWebAppFactory factory) : base(factory) { }

    // No endpoint for GetAllVersesFromReferenceList, so this test is skipped or would need a new endpoint.
}
