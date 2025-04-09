using BackendForChat.Application.Commands.Chats;
using BackendForChat.Models.DatabaseContext;
using BackendForChat.Models.Entities;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackendForChatTests.Commands.Chats
{
    public class CreateGroupChatTests
    {
        private ApplicationDbContext _context;
        private CreateGroupChatHandler _handler;

        private Guid _guidUser1;
        private Guid _guidUser2;
        private Guid _guidUser3;
        private Guid _guidUser4;
        private List<Guid> _guidUsers = new List<Guid>();
        private ChatTypeModel _chatType;

        [SetUp]
        public void SetUp()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            _context = new ApplicationDbContext(options);

            _guidUser1 = Guid.NewGuid();
            _guidUser2 = Guid.NewGuid();
            _guidUser3 = Guid.NewGuid();
            _guidUser4 = Guid.NewGuid();

            _guidUsers.Add(_guidUser1);
            _guidUsers.Add(_guidUser2);
            _guidUsers.Add(_guidUser3);
            _guidUsers.Add(_guidUser4);

            _chatType = new ChatTypeModel { Id = 0, Type = "Group" };


            _context.ChatTypes.Add(_chatType);
            _context.SaveChanges();

            _handler = new CreateGroupChatHandler(_context);
        }
        [Test]
        public async Task CreatePrivateChat_ShouldCreate_PrivateChat_WhenChatTypeExists()
        {
            var command = new CreateGroupChatCommand(_guidUsers);

            var result = await _handler.Handle(command, CancellationToken.None);

            result.Should().NotBeNull();
            result.Success.Should().BeTrue();
            result.Data.Should().NotBeNull();

            var createdChat = await _context.Chats
                .Include(c => c.ChatUsers)
                .FirstOrDefaultAsync(c => c.Id == result.Data.ChatId);

            createdChat.Should().NotBeNull();
            createdChat.ChatUsers.Should().HaveCount(4);
            createdChat.ChatUsers.Should().Contain(x => x.UserId == _guidUser1);
            createdChat.ChatUsers.Should().Contain(x => x.UserId == _guidUser2);
            createdChat.ChatUsers.Should().Contain(x => x.UserId == _guidUser3);
            createdChat.ChatUsers.Should().Contain(x => x.UserId == _guidUser4);
        }

        [Test]
        public async Task CreatePrivateChat_ShouldFailCreate_PrivateChat_WhenChatTypeNotExists()
        {
            _context.ChatTypes.RemoveRange(_context.ChatTypes);
            _context.SaveChanges();

            var command = new CreateGroupChatCommand(_guidUsers);

            var result = await _handler.Handle(command, CancellationToken.None);

            result.Should().NotBeNull();
            result.Success.Should().BeFalse();
            result.ErrorMessage.Should().Be("Chat type not found");
        }
        [TearDown]
        public void TearDown()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }
    }
}
