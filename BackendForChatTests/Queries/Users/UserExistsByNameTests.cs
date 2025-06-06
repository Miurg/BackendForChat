﻿using BackendForChat.Application.Queries.Users;
using BackendForChat.Models.DatabaseContext;
using Microsoft.EntityFrameworkCore;
using BackendForChat.Models.Entities;
using NUnit.Framework;
using FluentAssertions;

namespace BackendForChatTests
{
    [TestFixture]
    public class UserExistsByNameTests
    {
        private ApplicationDbContext _context;
        private UserExistsByNameHandler _handler;
        private string _username;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            _context = new ApplicationDbContext(options);

            _username = "test_user";
            _context.Users.Add(new UserModel { Username = _username, PasswordHash = "pass" });
            _context.SaveChanges();

            _handler = new UserExistsByNameHandler(_context);
        }

        [Test]
        public async Task UserExistsByName_ShouldReturn_TrueResult_WhenUserExists()
        {
            var query = new UserExistsByNameQuery(_username);

            var result = await _handler.Handle(query, CancellationToken.None);

            result.Should().BeTrue();
        }

        [Test]
        public async Task UserExistsByName_ShouldReturn_FalseResult_WhenUserNotExist()
        {
            var query = new UserExistsByNameQuery("non_existing_user");

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
