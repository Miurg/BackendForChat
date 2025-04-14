using BackendForChat.Application.Queries.Users;
using BackendForChat.Models.DatabaseContext;
using BackendForChat.Models.Entities;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace BackendForChatTests.Queries.Users
{
    public class UserExistByGuidTests
    {
        private ApplicationDbContext _context;
        private UserExistByGuidHandler _handler;
        private Guid _id;
        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            _context = new ApplicationDbContext(options);

            _id = Guid.NewGuid();
            _context.Users.Add(new UserModel { Id = _id, Username = "test_user", PasswordHash = "pass" });
            _context.SaveChanges();

            _handler = new UserExistByGuidHandler(_context);
        }

        [Test]
        public async Task UserExistByGuid_ShouldReturn_TrueResult_WhenUserExists()
        {
            var query = new UserExistByGuidQuery(_id);

            var result = await _handler.Handle(query, CancellationToken.None);

            result.Should().BeTrue();
        }

        [Test]
        public async Task UserExistByGuid_ShouldReturn_FalseResult_WhenUserNotExist()
        {
            var query = new UserExistByGuidQuery(Guid.NewGuid());

            var result = await _handler.Handle(query, CancellationToken.None);

            result.Should().BeFalse();
        }
        [TearDown]
        public void TearDown()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }
    }
}
