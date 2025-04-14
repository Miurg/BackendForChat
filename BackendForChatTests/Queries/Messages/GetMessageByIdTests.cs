using Azure.Messaging;
using BackendForChat.Application.DTO.Responses;
using BackendForChat.Application.Interfaces;
using BackendForChat.Application.Queries.Chats;
using BackendForChat.Application.Queries.Messages;
using BackendForChat.Application.Queries.Users;
using BackendForChat.Models.DatabaseContext;
using BackendForChat.Models.Entities;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackendForChatTests.Queries.Messages
{
    public class GetMessageByIdTests
    {
        private ApplicationDbContext _context;
        private GetMessageByIdHandler _handler;
        private Mock<IEncryptionService> _encryptionServiceMock;

        private Guid _chatId1;
        private Guid _chatId2;
        private ChatTypeModel _chatType;
        private ChatModel _chat1;
        private ChatModel _chat2;
        private ChatUserModel _chatUser1;
        private ChatUserModel _chatUser2;
        private ChatUserModel _chatUser3;
        private ChatUserModel _chatUser4;
        private Guid _user1Id;
        private Guid _user2Id;
        private Guid _user3Id;
        private UserModel _user1;
        private UserModel _user2;
        private UserModel _user3;
        private ResponseMessageDto _responseMessage1;
        private MessageModel _message1;
        private int _messageId1;
        private string _messageContent;
        private MessageModel _message2;
        private int _messageId2;


        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            _context = new ApplicationDbContext(options);

            _chatId1 = Guid.NewGuid();
            _chatId2 = Guid.NewGuid();
            _user1Id = Guid.NewGuid();
            _user2Id = Guid.NewGuid();
            _user3Id = Guid.NewGuid();
            _chatType = new ChatTypeModel { Id = 1, Type = "Private" };
            _chat1 = new ChatModel { Id = _chatId1, ChatTypeId = _chatType.Id };
            _chat2 = new ChatModel { Id = _chatId2, ChatTypeId = _chatType.Id };
            _user1 = new UserModel { Id = _user1Id, Username = "User1", PasswordHash = "Password1" };
            _user2 = new UserModel { Id = _user2Id, Username = "User2", PasswordHash = "Password2" };
            _user3 = new UserModel { Id = _user3Id, Username = "User3", PasswordHash = "Password3" };
            _chatUser1 = new ChatUserModel { ChatId = _chatId1, UserId = _user1Id };
            _chatUser2 = new ChatUserModel { ChatId = _chatId1, UserId = _user2Id };
            _chatUser3 = new ChatUserModel { ChatId = _chatId2, UserId = _user3Id };
            _chatUser4 = new ChatUserModel { ChatId = _chatId2, UserId = _user2Id };
            _messageId2 = 2;
            _message2 = new MessageModel { Id = _messageId2, ChatId = _chatId2, Content = "Random", SenderId = _user2Id };
            _messageId1 = 1;
            _messageContent = "Message1";
            _message1 = new MessageModel { Id = _messageId1, ChatId = _chatId1, Content = _messageContent, SenderId = _user1Id };

            _responseMessage1 = new ResponseMessageDto
            {
                Id = _message1.Id,
                ChatId = _message1.ChatId,
                Content = _message1.Content,
                SenderId = _message1.SenderId,
                CreatedAt = _message1.CreatedAt
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
            _context.SaveChanges();

            _encryptionServiceMock = new Mock<IEncryptionService>();

            _handler = new GetMessageByIdHandler(_context, _encryptionServiceMock.Object);
        }
        [Test]
        public async Task GetMessageById_ShouldReturn_SuccesResultAndMessage_WhenMessageExistAndUserInChat()
        {
            _encryptionServiceMock
                .Setup(j => j.Decrypt(_messageContent))
                .Returns(_messageContent);

            var query = new GetMessageByIdQuery(_messageId1, _user1Id);

            var result = await _handler.Handle(query, CancellationToken.None);

            result.Success.Should().BeTrue();
            result.Data.Should().BeEquivalentTo(_responseMessage1);
        }
        [Test]
        public async Task GetMessageById_ShouldReturn_FailResultAndErrorMessage_WhenMessageNotExist()
        {
            var query = new GetMessageByIdQuery(10, _user1Id);

            var result = await _handler.Handle(query, CancellationToken.None);

            result.Success.Should().BeFalse();
            result.ErrorMessage.Should().BeEquivalentTo("Message with that id doesn't exist");
        }
        [Test]
        public async Task GetMessageById_ShouldReturn_FailResultAndErrorMessage_WhenUserNotBelondToChat()
        {
            var query = new GetMessageByIdQuery(_messageId2, _user1Id);

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
