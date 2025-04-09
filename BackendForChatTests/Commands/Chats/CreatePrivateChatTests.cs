using BackendForChat.Application.Commands.Chats;
using BackendForChat.Application.DTO.Responses;
using BackendForChat.Application.Queries.Chats;
using BackendForChat.Models.DatabaseContext;
using BackendForChat.Models.Entities;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace BackendForChatTests.Commands.Chats
{
    [TestFixture]
    public class CreatePrivateChatTests
    {
        private ApplicationDbContext _context;
        private CreatePrivateChatHandler _handler;

        private Guid _guidUser1;
        private Guid _guidUser2;
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

            _chatType = new ChatTypeModel { Id = 0, Type = "Private" };


            _context.ChatTypes.Add(_chatType);
            _context.SaveChanges();

            _handler = new CreatePrivateChatHandler(_context);
        }
        [Test]
        public async Task CreatePrivateChat_ShouldCreate_PrivateChat_WhenChatTypeExists()
        {
            var command = new CreatePrivateChatCommand(_guidUser1, _guidUser2);

            var result = await _handler.Handle(command, CancellationToken.None);

            result.Should().NotBeNull();
            result.Success.Should().BeTrue();
            result.Data.Should().NotBeNull();

            var createdChat = await _context.Chats
                .Include(c => c.ChatUsers)
                .FirstOrDefaultAsync(c => c.Id == result.Data.ChatId);

            createdChat.Should().NotBeNull();
            createdChat.ChatUsers.Should().HaveCount(2);
            createdChat.ChatUsers.Should().Contain(x => x.UserId == _guidUser1);
            createdChat.ChatUsers.Should().Contain(x => x.UserId == _guidUser2);
        }

        [Test]
        public async Task CreatePrivateChat_ShouldFailCreate_PrivateChat_WhenChatTypeNotExists()
        {
            _context.ChatTypes.RemoveRange(_context.ChatTypes);
            _context.SaveChanges();

            var command = new CreatePrivateChatCommand(_guidUser1, _guidUser2);

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
