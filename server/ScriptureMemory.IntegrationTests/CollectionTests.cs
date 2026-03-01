using DataAccess.Data;
using DataAccess.Models;
using DataAccess.Requests;
using ScriptureMemory.Server.Services;
using ScriptureMemoryLibrary;
using System.Net.Http.Json;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using static ScriptureMemoryLibrary.Enums;

namespace ScriptureMemory.IntegrationTests
{
    public class CollectionTests : BaseIntegrationTest
    {
        public CollectionTests(IntegrationTestWebAppFactory factory) : base(factory) { }

        private async Task<User> CreateAndLoginUserAsync(string firstName, string lastName)
        {
            var request = new CreateUserRequest
            {
                Username = $"{firstName.ToLower()}_{Guid.NewGuid().ToString()[..8]}",
                FirstName = firstName,
                LastName = lastName,
                Email = $"{firstName.ToLower()}@gmail.com",
                Password = "password123456",
                BibleVersion = BibleVersion.Kjv
            };

            await Api.PostAsJsonAsync("/users", request);

            var loginResponse = await Api.PostAsJsonAsync("/users/login/username",
                new { request.Username, request.Password });

            loginResponse.EnsureSuccessStatusCode();

            var user = await loginResponse.Content.ReadFromJsonAsync<User>();
            Assert.NotNull(user);
            return user;
        }

        [Fact]
        public async Task CreateCollection_ValidCollection_ReturnsNewId()
        {
            var user = await CreateAndLoginUserAsync("Test", "User");

            var collection = new Collection { UserId = user.Id, Title = "Integration Test Collection" };
            var newId = await collectionService.CreateCollection(collection);

            Assert.True(newId > 0);
        }

        /// <summary>
        /// John publishes a collection. Jane saves it.
        /// Verifies NumSaves increments and John receives a notification.
        /// </summary>
        [Fact]
        public async Task SaveCollection_AfterInsertingPublishedCollection_IncrementsNumSavedAndSendsNotification()
        {
            // Arrange
            var john = await CreateAndLoginUserAsync("John", "Doe");
            var jane = await CreateAndLoginUserAsync("Jane", "Doe");

            int publishedId = await publishedContext.Insert(new PublishedCollection
            {
                AuthorId = john.Id,
                Title = "John's Scripture Collection",
                Status = ApprovedStatus.Approved,
                NumSaves = 0,
            });

            // Act — Jane saves John's collection
            await collectionService.SaveCollection(new SaveCollectionRequest
            {
                UserId = jane.Id,
                PublishedId = publishedId,
            });

            // Assert — NumSaves went from 0 to 1
            var updated = await publishedContext.Get(publishedId);
            Assert.Equal(1, updated.NumSaves);

            // Assert — John got a notification that Jane saved his collection
            var notifications = await notificationContext.GetUserNotifications(john.Id);
            var notification = notifications.FirstOrDefault(n =>
                n.SenderId == jane.Id &&
                n.ReceiverId == john.Id &&
                n.NotificationType == NotificationType.CollectionSaved);

            Assert.NotNull(notification);
            Assert.Contains("John's Scripture Collection", notification.Message);
        }

        /// <summary>
        /// Jane tries to save John's collection twice — the second attempt should be rejected.
        /// </summary>
        [Fact]
        public async Task SaveCollection_WhenAlreadySaved_ThrowsInvalidOperationException()
        {
            // Arrange
            var john = await CreateAndLoginUserAsync("John", "Smith");
            var jane = await CreateAndLoginUserAsync("Jane", "Smith");

            int publishedId = await publishedContext.Insert(new PublishedCollection
            {
                AuthorId = john.Id,
                Title = "John's Epistles Collection",
                Status = ApprovedStatus.Approved,
            });

            var saveRequest = new SaveCollectionRequest
            {
                UserId = jane.Id,
                PublishedId = publishedId,
            };

            // First save — should succeed
            await collectionService.SaveCollection(saveRequest);

            // Act & Assert — second save should be rejected
            await Assert.ThrowsAsync<InvalidOperationException>(
                () => collectionService.SaveCollection(saveRequest));

            // NumSaves should still be 1, not 2
            var updated = await publishedContext.Get(publishedId);
            Assert.Equal(1, updated.NumSaves);
        }

        [Fact]
        public async Task GetUserCollections_ValidUserId_ReturnsCollections()
        {
            var user = await CreateAndLoginUserAsync("Test", "User");
            var collections = await collectionService.GetUserCollections(user.Id);
            Assert.NotNull(collections);
        }

        [Fact]
        public async Task GetCollection_ValidCollection_ReturnsPopulatedCollection()
        {
            var user = await CreateAndLoginUserAsync("Test", "User");

            await collectionService.CreateCollection(new Collection { UserId = user.Id, Title = "Test" });

            var collections = await collectionService.GetUserCollections(user.Id);
            Assert.NotEmpty(collections);

            var result = await collectionService.GetCollection(collections[0]);
            Assert.NotNull(result);
        }
    }
}