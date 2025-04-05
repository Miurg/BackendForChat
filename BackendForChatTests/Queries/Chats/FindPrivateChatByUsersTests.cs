using BackendForChat.Application.Queries.Chats;
using BackendForChat.Models.DatabaseContext;
using BackendForChat.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackendForChatTests.Queries.Chats
{
    [TestFixture]
    public class FindPrivateChatByUsersTests
    {
        private ApplicationDbContext _context;
        private FindPrivateChatByUsersHandler _handler;
        private Guid _chatId;
        private ChatTypeModel _chatType;
        private ChatModel _chat;
        private ChatUserModel _chatUser1;
        private ChatUserModel _chatUser2;
        private Guid _user1Id;
        private Guid _user2Id;
        private Guid _user3Id;
        private UserModel _user1;
        private UserModel _user2;
        private UserModel _user3;

        [SetUp]
        public void SetUp()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            _context = new ApplicationDbContext(options);

            _chatId = Guid.NewGuid();
            _user1Id = Guid.NewGuid();
            _user2Id = Guid.NewGuid();
            _user3Id = Guid.NewGuid();
            _chatType = new ChatTypeModel { Id = 1, Type = "Private" };
            _chat = new ChatModel { Id = _chatId, ChatTypeId = _chatType.Id};
            _user1 = new UserModel { Id = _user1Id, Username = "User1", PasswordHash = "Password1" };
            _user2 = new UserModel { Id = _user2Id, Username = "User2", PasswordHash = "Password2" };
            _user3 = new UserModel { Id = _user3Id, Username = "User3", PasswordHash = "Password3" };
            _chatUser1 = new ChatUserModel { ChatId = _chatId, UserId = _user1Id };
            _chatUser2 = new ChatUserModel { ChatId = _chatId, UserId = _user2Id };

            _context.ChatTypes.Add(_chatType);
            _context.Chats.Add(_chat);
            _context.Users.Add(_user1);
            _context.Users.Add(_user2);
            _context.Users.Add(_user3);
            _context.ChatUsers.Add(_chatUser1);
            _context.ChatUsers.Add(_chatUser2);
            _context.SaveChanges();

            _handler = new FindPrivateChatByUsersHandler(_context);
        }
        [Test]
        public async Task FindPrivateChatByUsers_Should_ReturnSuccessResultAndChatId_WhenChatAndUsersExists()
        {
            var query = new FindPrivateChatByUsersQuery(_user1Id, _user2Id);

            var result = await _handler.Handle(query, CancellationToken.None);

            Assert.That(result.Success, Is.True);
            Assert.That(result.Data.ChatId, Is.EqualTo(_chatId));
        }
        [Test]
        public async Task FindPrivateChatByUsers_Should_ReturnFailResultAndErrorMessage_WhenChatNotExist()
        {
            var query = new FindPrivateChatByUsersQuery(_user1Id, _user2Id);

            var result = await _handler.Handle(query, CancellationToken.None);

            Assert.That(result.Success, Is.False);
            Assert.That(result.ErrorMessage, Is.EqualTo("Chat with this user does not exist"));
        }
        [TearDown]
        public void TearDown()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }
    }
}
