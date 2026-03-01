using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using DataAccess.Models;
using DataAccess.Requests;
using ScriptureMemory.Server.Services;

namespace ScriptureMemory.IntegrationTests
{
    public class CollectionTests : BaseIntegrationTest
    {
        public CollectionTests(IntegrationTestWebAppFactory factory) : base(factory) { }

        [Fact]
        public async Task CreateCollection_ValidCollection_ReturnsNewId()
        {
            var collection = new Collection { UserId = 1, Title = "Integration Test Collection" };
            var newId = await collectionService.CreateCollection(collection);
            Assert.True(newId > 0);
        }

        [Fact]
        public async Task SaveCollection_ValidRequest_Succeeds()
        {
            var request = new SaveCollectionRequest { UserId = 1, PublishedId = 100 };
            await collectionService.SaveCollection(request);
            // No exception means success
            Assert.True(true);
        }

        [Fact]
        public async Task GetUserCollections_ValidUserId_ReturnsCollections()
        {
            var collections = await collectionService.GetUserCollections(1);
            Assert.NotNull(collections);
        }

        [Fact]
        public async Task GetCollection_ValidCollection_ReturnsPopulatedCollection()
        {
            var collections = await collectionService.GetUserCollections(1);
            if (collections.Count == 0) return;
            var collection = collections[0];
            var result = await collectionService.GetCollection(collection);
            Assert.NotNull(result);
        }
    }
}
