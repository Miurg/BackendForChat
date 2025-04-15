using Azure.Messaging;
using BackendForChat.Application.DTO.Responses;
using BackendForChat.Application.Interfaces;
using BackendForChat.Application.Queries.Messages;
using BackendForChat.Models.DatabaseContext;
using BackendForChat.Models.Entities;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackendForChatTests.Queries.Messages
{
    public class GetMessagesPagedTests
    {
        private ApplicationDbContext _context;
        private GetMessagesPagedHandler _handler;
        private Mock<IEncryptionService> _encryptionServiceMock;
        private Mock<ICurrentUserService> _currentUserMock;

        private Guid _chatId1;
        private Guid _chatId2;
        private ChatTypeModel _chatType;
        private ChatModel _chat1;
        private ChatModel _chat2;
        private ChatUserModel _chatUser1;
        private ChatUserModel _chatUser2;
        private ChatUserModel _chatUser3;
        private ChatUserModel _chatUser4;
        private Guid _userGuid1;
        private Guid _userGuid2;
        private Guid _userGuid3;
        private UserModel _user1;
        private UserModel _user2;
        private UserModel _user3;
        private MessageModel _message1;
        private int _messageId1;
        private MessageModel _message2;
        private int _messageId2;
        private MessageModel _message3;
        private int _messageId3;
        ResponseMessageDto _responseMessage1;
        ResponseMessageDto _responseMessage2;



        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            _context = new ApplicationDbContext(options);

            _chatId1 = Guid.NewGuid();
            _chatId2 = Guid.NewGuid();
            _userGuid1 = Guid.NewGuid();
            _userGuid2 = Guid.NewGuid();
            _userGuid3 = Guid.NewGuid();
            _chatType = new ChatTypeModel { Id = 1, Type = "Private" };
            _chat1 = new ChatModel { Id = _chatId1, ChatTypeId = _chatType.Id };
            _chat2 = new ChatModel { Id = _chatId2, ChatTypeId = _chatType.Id };
            _user1 = new UserModel { Id = _userGuid1, Username = "User1", PasswordHash = "Password1" };
            _user2 = new UserModel { Id = _userGuid2, Username = "User2", PasswordHash = "Password2" };
            _user3 = new UserModel { Id = _userGuid3, Username = "User3", PasswordHash = "Password3" };
            _chatUser1 = new ChatUserModel { ChatId = _chatId1, UserId = _userGuid1 };
            _chatUser2 = new ChatUserModel { ChatId = _chatId1, UserId = _userGuid2 };
            _chatUser3 = new ChatUserModel { ChatId = _chatId2, UserId = _userGuid3 };
            _chatUser4 = new ChatUserModel { ChatId = _chatId2, UserId = _userGuid2 };
            _messageId3 = 3;
            _message3 = new MessageModel { Id = _messageId3, ChatId = _chatId2, Content = "Random", SenderId = _userGuid2 };
            _messageId2 = 2;
            _message2 = new MessageModel { Id = _messageId2, ChatId = _chatId1, Content = "Random", SenderId = _userGuid2 };
            _messageId1 = 1;
            _message1 = new MessageModel { Id = _messageId1, ChatId = _chatId1, Content = "Random", SenderId = _userGuid1 };

            _responseMessage1 = new ResponseMessageDto
            {
                Id = _message1.Id,
                ChatId = _message1.ChatId,
                Content = _message1.Content,
                SenderId = _message1.SenderId,
                CreatedAt = _message1.CreatedAt
            };
            _responseMessage2 = new ResponseMessageDto
            {
                Id = _message2.Id,
                ChatId = _message2.ChatId,
                Content = _message2.Content,
                SenderId = _message2.SenderId,
                CreatedAt = _message2.CreatedAt
            };


            _context.ChatTypes.Add(_chatType);
            _context.Chats.Add(_chat1);
            _context.Chats.Add(_chat2);
            _context.Users.Add(_user1);
            _context.Users.Add(_user2);
            _context.Users.Add(_user3);
            _context.ChatUsers.Add(_chatUser1);
            _context.ChatUsers.Add(_chatUser2);
            _context.Messages.Add(_message1);
            _context.Messages.Add(_message2);
            _context.Messages.Add(_message3);
            _context.SaveChanges();

            _encryptionServiceMock = new Mock<IEncryptionService>();
            _currentUserMock = new Mock<ICurrentUserService>();

            _handler = new GetMessagesPagedHandler(_context, _encryptionServiceMock.Object, _currentUserMock.Object);

            _currentUserMock.Setup(x => x.UserId).Returns(_userGuid1);
        }
        [Test]
        public async Task GetMessagesPaged_ShouldReturn_SuccesResultAndMessages_WhenMessagesExistAndUserInChat()
        {
            _encryptionServiceMock
                .Setup(j => j.Decrypt("Random"))
                .Returns("Random");

            var query = new GetMessagesPagedQuery(0,2,_chatId1);

            var result = await _handler.Handle(query, CancellationToken.None);

            result.Success.Should().BeTrue();
            result.Data[0].Should().BeEquivalentTo(_responseMessage1);
            result.Data[1].Should().BeEquivalentTo(_responseMessage2);
        }
        [Test]
        public async Task GetMessagesPaged_ShouldReturn_FailResultAndErrorMessage_WhenUserDoesNotBelongToChat()
        {
            _encryptionServiceMock
               .Setup(j => j.Decrypt("Random"))
               .Returns("Random");

            var query = new GetMessagesPagedQuery(0, 2, _chatId2);

            var result = await _handler.Handle(query, CancellationToken.None);

            result.Success.Should().BeFalse();
            result.ErrorMessage.Should().BeEquivalentTo("User doesn't belong to chat");
        }
        [Test]
        public async Task GetMessagesPaged_ShouldReturn_FailResultAndErrorMessage_WhenInvalidPagedParametersAreRequested()
        {
            _encryptionServiceMock
               .Setup(j => j.Decrypt("Random"))
               .Returns("Random");

            var query = new GetMessagesPagedQuery(-1, -1, _chatId1);

            var result = await _handler.Handle(query, CancellationToken.None);

            result.Success.Should().BeFalse();
            result.ErrorMessage.Should().BeEquivalentTo("Messages with that paged parameters doesn't exist");
        }
        [Test]
        public async Task GetMessagesPaged_ShouldReturn_FailResultAndErrorMessage_WhenPagedParametersAreRequestedForNonExistentMessages()
        {
            _encryptionServiceMock
               .Setup(j => j.Decrypt("Random"))
               .Returns("Random");

            var query = new GetMessagesPagedQuery(10, 10, _chatId1);

            var result = await _handler.Handle(query, CancellationToken.None);

            result.Success.Should().BeFalse();
            result.ErrorMessage.Should().BeEquivalentTo("Messages with that paged parameters doesn't exist");
        }
        [TearDown]
        public void TearDown()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }
    }
}
