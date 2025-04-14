using BackendForChat.Application.DTO.Responses;
using BackendForChat.Application.Queries.Users;
using BackendForChat.Models.DatabaseContext;
using BackendForChat.Models.Entities;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.ApplicationInsights.MetricDimensionNames.TelemetryContext;

namespace BackendForChatTests.Queries.Users
{
    public class GetUserByGuidTests
    {
        private ApplicationDbContext _context;
        private GetUserByGuidHandler _handler;
        private Guid _id;
        private string _name;
        private UserModel _user;
        private ResponseUserDto _responseUser;
        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            _context = new ApplicationDbContext(options);

            _id = Guid.NewGuid();
            _name = "test_user";
            _user = new UserModel 
            { 
                Id = _id, 
                Username = _name, 
                PasswordHash = "pass" 
            };
            _responseUser = new ResponseUserDto
            {
                Id = _user.Id,
                Username = _user.Username
            };
            _context.Users.Add(_user);
            _context.SaveChanges();

            _handler = new GetUserByGuidHandler(_context);
        }
        [Test]
        public async Task GetUserByGuid_ShouldReturn_SuccessResultAndUserDTO_WhenUserExists()
        {
            var query = new GetUserByGuidQuery(_id);

            var result = await _handler.Handle(query, CancellationToken.None);
            
            result.Success.Should().BeTrue();
            result.Data.Should().BeEquivalentTo(_responseUser);
        }

        [Test]
        public async Task GetUserByGuid_ShouldReturn_FailResultAndErrorMessage_WhenUserNotExist()
        {
            var query = new GetUserByGuidQuery(Guid.NewGuid());

            var result = await _handler.Handle(query, CancellationToken.None);

            result.Success.Should().BeFalse();
            result.ErrorMessage.Should().BeEquivalentTo("User with that id doesn't exist");
        }
        [TearDown]
        public void TearDown()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }
    }
}
