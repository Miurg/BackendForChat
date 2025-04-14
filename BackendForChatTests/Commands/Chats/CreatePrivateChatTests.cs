using BackendForChat.Application.Commands.Chats;
using BackendForChat.Application.DTO.Responses;
using BackendForChat.Application.Queries.Chats;
using BackendForChat.Application.Queries.Users;
using BackendForChat.Models.DatabaseContext;
using BackendForChat.Models.Entities;
using FluentAssertions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace BackendForChatTests.Commands.Chats
{
    [TestFixture]
    public class CreatePrivateChatTests
    {
        private ApplicationDbContext _context;
        private CreatePrivateChatHandler _handler;
        private Mock<IMediator> mediatorMock;

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

            mediatorMock = new Mock<IMediator>();

            _handler = new CreatePrivateChatHandler(_context, mediatorMock.Object);
        }
        [Test]
        public async Task CreatePrivateChat_ShouldCreatePrivateChatAndReturn_ChatId_WhenChatTypeAndUsersExists()
        {
            mediatorMock
                .Setup(m => m.Send(
                    It.Is<UserExistByGuidQuery>(q => q.Id == _guidUser1),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);
            mediatorMock
                .Setup(m => m.Send(
                    It.Is<UserExistByGuidQuery>(q => q.Id == _guidUser2),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);
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
        public async Task CreatePrivateChat_ShouldFailCreateAndReturn_FailResultAndErrorMessage_WhenChatTypeNotExists()
        {
            mediatorMock
                .Setup(m => m.Send(
                    It.Is<UserExistByGuidQuery>(q => q.Id == _guidUser1),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);
            mediatorMock
                .Setup(m => m.Send(
                    It.Is<UserExistByGuidQuery>(q => q.Id == _guidUser2),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);
            _context.ChatTypes.RemoveRange(_context.ChatTypes);
            _context.SaveChanges();
            var command = new CreatePrivateChatCommand(_guidUser1, _guidUser2);

            var result = await _handler.Handle(command, CancellationToken.None);

            result.Should().NotBeNull();
            result.Success.Should().BeFalse();
            result.ErrorMessage.Should().Be("Chat type not found");
        }
        [Test]
        public async Task CreatePrivateChat_ShouldFailCreateAndReturn_FailResultAndErrorMessage_WhenFirstUserNotExists()
        {
            mediatorMock
                .Setup(m => m.Send(
                    It.Is<UserExistByGuidQuery>(q => q.Id == _guidUser1),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);
            mediatorMock
                .Setup(m => m.Send(
                    It.Is<UserExistByGuidQuery>(q => q.Id == _guidUser2),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);
            var command = new CreatePrivateChatCommand(_guidUser1, _guidUser2);

            var result = await _handler.Handle(command, CancellationToken.None);

            result.Should().NotBeNull();
            result.Success.Should().BeFalse();
            result.ErrorMessage.Should().Be("User with that id doesn't exist");
        }
        [Test]
        public async Task CreatePrivateChat_ShouldFailCreateAndReturn_FailResultAndErrorMessage_WhenSecondUserNotExists()
        {
            mediatorMock
                .Setup(m => m.Send(
                    It.Is<UserExistByGuidQuery>(q => q.Id == _guidUser1),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);
            mediatorMock
                .Setup(m => m.Send(
                    It.Is<UserExistByGuidQuery>(q => q.Id == _guidUser2),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);
            var command = new CreatePrivateChatCommand(_guidUser1, _guidUser2);

            var result = await _handler.Handle(command, CancellationToken.None);

            result.Should().NotBeNull();
            result.Success.Should().BeFalse();
            result.ErrorMessage.Should().Be("User with that id doesn't exist");
        }
        [TearDown]
        public void TearDown()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }
    }
}
