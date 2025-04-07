using BackendForChat.Application.DTO.Requests;
using BackendForChat.Application.Interfaces;
using BackendForChat.Application.Queries.Auth;
using BackendForChat.Application.Queries.Users;
using BackendForChat.Application.Services;
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

namespace BackendForChatTests.Queries.Auth
{
    [TestFixture]
    public class LoginTests
    {
        private ApplicationDbContext _context;
        private Mock<IPasswordHasher<UserModel>> _passwordHasherMock;
        private Mock<IJwtService> _jwtServiceMock;
        private LoginHandler _handler;

        [SetUp]
        public void SetUp()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            _context = new ApplicationDbContext(options);

            _context.Users.Add(new UserModel { Username = "test_user", PasswordHash = "pass" });
            _context.SaveChanges();

            _passwordHasherMock = new Mock<IPasswordHasher<UserModel>>();
            _jwtServiceMock = new Mock<IJwtService>();

            _handler = new LoginHandler(_context, _passwordHasherMock.Object, _jwtServiceMock.Object);
        }

        [Test]
        public async Task Login_ShouldReturn_SuccessResultAndToken_WhenCredentialsAreValid()
        {
            var user = await _context.Users.FirstAsync();
            _passwordHasherMock
                .Setup(h => h.VerifyHashedPassword(user, user.PasswordHash, "correct_password"))
                .Returns(PasswordVerificationResult.Success);

            _jwtServiceMock
                .Setup(j => j.GenerateJwtToken(user))
                .Returns("test_token");
            var requestLogin = new RequestLoginDto
            {
                Username = "test_user",
                Password = "correct_password"
            };
            var query = new LoginQuery(requestLogin);

            var result = await _handler.Handle(query, CancellationToken.None);

            result.Success.Should().BeTrue();
            result.Data.Token.Should().BeEquivalentTo("test_token");
        }

        [Test]
        public async Task Login_ShouldReturn_FailResultAndErorrMessage_WhenUserDoesNotExist()
        {
            var requestLogin = new RequestLoginDto
            {
                Username = "not_exist",
                Password = "any"
            };
            var query = new LoginQuery(requestLogin);

            var result = await _handler.Handle(query, CancellationToken.None);

            result.Success.Should().BeFalse();
            result.ErrorMessage.Should().BeEquivalentTo("Invalid credentials");
        }

        [Test]
        public async Task Login_ShouldReturn_FailResultAndErrorMessage_WhenPasswordIsIncorrect()
        {
            var user = await _context.Users.FirstAsync();
            _passwordHasherMock
                .Setup(h => h.VerifyHashedPassword(user, user.PasswordHash, "wrong_password"))
                .Returns(PasswordVerificationResult.Failed);

            var requestLogin = new RequestLoginDto
            {
                Username = "test_user",
                Password = "wrong_password"
            };
            var query = new LoginQuery(requestLogin);

            var result = await _handler.Handle(query, CancellationToken.None);

            result.Success.Should().BeFalse();
            result.ErrorMessage.Should().BeEquivalentTo("Invalid credentials");
        }

        [TearDown]
        public void TearDown()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }
    }
}
