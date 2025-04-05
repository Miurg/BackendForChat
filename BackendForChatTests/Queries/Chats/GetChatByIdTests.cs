using BackendForChat.Application.Queries.Chats;
using BackendForChat.Application.Queries.Users;
using BackendForChat.Models.DatabaseContext;
using BackendForChat.Models.Entities;
using Microsoft.EntityFrameworkCore;
using System;

namespace BackendForChatTests.Queries.Chats
{
    [TestFixture]
    public class GetChatByIdTests
    {
        private ApplicationDbContext _context;
        private GetChatByIdHandler _handler;
        private Guid _chatId;
        private ChatTypeModel _chatType;
        private ChatModel _chat;

        [SetUp]
        public void SetUp()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            _context = new ApplicationDbContext(options);

            _chatId = Guid.NewGuid();
            _chatType = new ChatTypeModel { Id = 0, Type = "Private" };
            _chat = new ChatModel { Id = _chatId, ChatTypeId = 0 };

            _context.ChatTypes.Add(_chatType);
            _context.Chats.Add(_chat);
            _context.SaveChanges();

            _handler = new GetChatByIdHandler(_context);
        }
        [Test]
        public async Task GetChatById_Should_ReturnSuccessResultAndChatId_WhenChatExists()
        {
            var query = new GetChatByIdQuery(_chatId);

            var result = await _handler.Handle(query, CancellationToken.None);

            Assert.That(result.Success, Is.True);
            Assert.That(result.Data.ChatId, Is.EqualTo(_chatId));
        }
        [Test]
        public async Task GetChatById_Should_ReturnFailResultAndErorrMessage_WhenChatDoesNotExists()
        {
            var query = new GetChatByIdQuery(Guid.NewGuid());

            var result = await _handler.Handle(query, CancellationToken.None);

            Assert.That(result.Success, Is.False);
            Assert.That(result.ErrorMessage, Is.EqualTo("Chat with that id does not exist"));
        }
        [TearDown]
        public void TearDown()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }
    }
}
