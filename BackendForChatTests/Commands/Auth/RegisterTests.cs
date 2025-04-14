using BackendForChat.Application.Commands.Auth;
using BackendForChat.Application.Commands.Chats;
using BackendForChat.Application.DTO.Requests;
using BackendForChat.Application.Queries.Users;
using BackendForChat.Models.DatabaseContext;
using BackendForChat.Models.Entities;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackendForChatTests.Commands.Auth
{
    [TestFixture]
    public class RegisterTests
    {
        private ApplicationDbContext _context;
        private RegisterHandler _handler;
        private Mock<IPasswordHasher<UserModel>> passwordHasher;

        private Guid _guidUser1;
        private RequestRegisterDto requestRegister;

        [SetUp]
        public void SetUp()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            _context = new ApplicationDbContext(options);

            _guidUser1 = Guid.NewGuid();
            requestRegister = new RequestRegisterDto
            {
                Username = "Random",
                Password = "password"
            };

            _context.SaveChanges();

            passwordHasher = new Mock<IPasswordHasher<UserModel>>();

            _handler = new RegisterHandler(_context, passwordHasher.Object);
        }
        [Test]
        public async Task Register_ShouldRegisterAndReturn_SuccesResultAndUser_WhenDataValid()
        {
            passwordHasher
                .Setup(h => h.HashPassword(It.IsAny<UserModel>(), "password"))
                .Returns("password");
            var command = new RegisterCommand(requestRegister);

            var result = await _handler.Handle(command, CancellationToken.None);

            result.Should().NotBeNull();
            result.Success.Should().BeTrue();
            result.Data.Should().NotBeNull();
        }

        [TearDown]
        public void TearDown()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }
    }
}
