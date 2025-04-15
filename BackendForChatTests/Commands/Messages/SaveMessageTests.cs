using BackendForChat.Application.Commands.Chats;
using BackendForChat.Application.Commands.Messages;
using BackendForChat.Application.DTO.Requests;
using BackendForChat.Application.DTO.Responses;
using BackendForChat.Application.Interfaces;
using BackendForChat.Application.Queries.Chats;
using BackendForChat.Application.Queries.Users;
using BackendForChat.Models.DatabaseContext;
using BackendForChat.Models.Entities;
using FluentAssertions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackendForChatTests.Commands.Messages
{
    [TestFixture]
    public class SaveMessageTests
    {
        private ApplicationDbContext _context;
        private SaveMessageHandler _handler;
        private Mock<IEncryptionService> _encryptionServiceMock;
        private Mock<ICurrentUserService> _currentUserMock;

        private Guid _userGuid1;
        private Guid _userGuid2;
        private Guid _userGuid3;
        private Guid _chatId1;
        private Guid _chatId2;
        private ChatTypeModel _chatType;
        private ChatModel _chat1;
        private ChatModel _chat2;
        private ChatUserModel _chatUser1;
        private ChatUserModel _chatUser2;
        private ChatUserModel _chatUser3;
        private ChatUserModel _chatUser4;
        private ResponseMessageDto _responseMessage;

        [SetUp]
        public void SetUp()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            _context = new ApplicationDbContext(options);

            _userGuid1 = Guid.NewGuid();
            _userGuid2 = Guid.NewGuid();
            _userGuid3 = Guid.NewGuid();

            _chatId1 = Guid.NewGuid();
            _chatId2 = Guid.NewGuid();
            _chatType = new ChatTypeModel { Id = 0, Type = "Private" };

            _chat1 = new ChatModel { Id = _chatId1, ChatTypeId = 0 };
            _chat2 = new ChatModel { Id = _chatId2, ChatTypeId = 0  };

            _chatUser1 = new ChatUserModel { ChatId = _chatId1, UserId = _userGuid1 };
            _chatUser2 = new ChatUserModel { ChatId = _chatId1, UserId = _userGuid2 };
            _chatUser3 = new ChatUserModel { ChatId = _chatId2, UserId = _userGuid2 };
            _chatUser4 = new ChatUserModel { ChatId = _chatId2, UserId = _userGuid3 };

            _responseMessage = new ResponseMessageDto()
            {
                Id = 1,
                Content = "Random",
                ChatId = _chatId1,
                CreatedAt = DateTime.UtcNow,
                SenderId = _userGuid1,
            };


            _context.ChatUsers.Add(_chatUser1);
            _context.ChatUsers.Add(_chatUser2);
            _context.ChatUsers.Add(_chatUser3);
            _context.ChatUsers.Add(_chatUser4);
            _context.ChatTypes.Add(_chatType);
            _context.Chats.Add(_chat1);
            _context.Chats.Add(_chat2);
            _context.SaveChanges();

            _encryptionServiceMock = new Mock<IEncryptionService>();


            _currentUserMock = new Mock<ICurrentUserService>();

            _handler = new SaveMessageHandler(_context, _encryptionServiceMock.Object, _currentUserMock.Object);
            _encryptionServiceMock
                .Setup(j => j.Encrypt("Random"))
                .Returns("Random");
            _encryptionServiceMock
                .Setup(j => j.Decrypt("Random"))
                .Returns("Random");
            _currentUserMock.Setup(x => x.UserId).Returns(_userGuid1);
        }
        [Test]
        public async Task SaveMessage_ShouldSaveMessageAndReturn_SuccessResultAndResponseMessageDto_WhenValidChatAndUser()
        {

            RequestMessageDto requestMessage = new RequestMessageDto
            { 
                ChatId = _chatId1,
                Content = "Random"
            };
            var query = new SaveMessageCommand(requestMessage);

            var result = await _handler.Handle(query, CancellationToken.None);

            result.Success.Should().BeTrue();
            result.Data.Should().BeEquivalentTo(_responseMessage, options => options
                .Excluding(x => x.CreatedAt)
            );
            var createdChat = await _context.Messages
                .FirstOrDefaultAsync(c => c.Id == result.Data.Id);
            createdChat.Should().NotBeNull();
        }
        [Test]
        public async Task SaveMessage_ShouldFailSaveMessageAndReturn_FailResultAndErrorMessage_WhenChatNotExist()
        {
            RequestMessageDto requestMessage = new RequestMessageDto
            {
                ChatId = Guid.NewGuid(),
                Content = "Random"
            };
            var query = new SaveMessageCommand(requestMessage);

            var result = await _handler.Handle(query, CancellationToken.None);

            result.Success.Should().BeFalse();
            result.ErrorMessage.Should().BeEquivalentTo("User doesn't belong to chat");
        }
        [Test]
        public async Task SaveMessage_ShouldFailSaveMessageAndReturn_FailResultAndErrorMessage_WhenUserNotBelongToChat()
        {
            RequestMessageDto requestMessage = new RequestMessageDto
            {
                ChatId = _chatId2,
                Content = "Random"
            };
            var query = new SaveMessageCommand(requestMessage);

            var result = await _handler.Handle(query, CancellationToken.None);

            result.Success.Should().BeFalse();
            result.ErrorMessage.Should().BeEquivalentTo("User doesn't belong to chat");
        }
        [TearDown]
        public void TearDown()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }
    }
}
