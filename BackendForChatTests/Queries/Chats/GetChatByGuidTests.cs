using BackendForChat.Application.DTO.Responses;
using BackendForChat.Application.Queries.Chats;
using BackendForChat.Application.Queries.Users;
using BackendForChat.Models.DatabaseContext;
using BackendForChat.Models.Entities;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using System;

namespace BackendForChatTests.Queries.Chats
{
    [TestFixture]
    public class GetChatByGuidTests
    {
        private ApplicationDbContext _context;
        private GetChatByGuidHandler _handler;
        private Guid _chatId;
        private ChatTypeModel _chatType;
        private ChatModel _chat;
        private ResponseChatCreateDto _responseChatCreateDto;

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

            _responseChatCreateDto = new ResponseChatCreateDto
            {
                ChatId = _chatId,
            };

            _context.ChatTypes.Add(_chatType);
            _context.Chats.Add(_chat);
            _context.SaveChanges();

            _handler = new GetChatByGuidHandler(_context);
        }
        [Test]
        public async Task GetChatById_ShouldReturn_SuccessResultAndChatId_WhenChatExists()
        {
            var query = new GetChatByGuidQuery(_chatId);

            var result = await _handler.Handle(query, CancellationToken.None);

            result.Success.Should().BeTrue();
            result.Data.Should().BeEquivalentTo(_responseChatCreateDto);
        }
        [Test]
        public async Task GetChatById_ShouldReturn_FailResultAndErorrMessage_WhenChatNotExists()
        {
            var query = new GetChatByGuidQuery(Guid.NewGuid());

            var result = await _handler.Handle(query, CancellationToken.None);

            result.Success.Should().BeFalse();
            result.ErrorMessage.Should().BeEquivalentTo("Chat with that id does not exist");
        }
        [TearDown]
        public void TearDown()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }
    }
}
