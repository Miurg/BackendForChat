using BackendForChat.Application.Queries.Users;
using BackendForChat.Models.DatabaseContext;
using BackendForChat.Models.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackendForChatTests.Queries.Users
{
    public class GetUserByGuidTests
    {
        private ApplicationDbContext _context;
        private GetUserByGuidHandler _handler;
        private Guid _id;
        private string _name;
        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            _context = new ApplicationDbContext(options);

            _id = Guid.NewGuid();
            _name = "test_user";
            _context.Users.Add(new UserModel { Id = _id, Username = _name, PasswordHash = "pass" });
            _context.SaveChanges();

            _handler = new GetUserByGuidHandler(_context);
        }
        [Test]
        public async Task GetUserByGuid_ShouldReturn_SuccessResultAndUserDTO_WhenUserExists()
        {
            var query = new GetUserByGuidQuery(_id);

            var result = await _handler.Handle(query, CancellationToken.None);

            Assert.That(result.Success, Is.True);
            Assert.That(result.Data.Id, Is.EqualTo(_id));
            Assert.That(result.Data.Username, Is.EqualTo(_name));
        }

        [Test]
        public async Task GetUserByGuid_ShouldReturn_FailResultAndErrorMessage_WhenUserDoesNotExist()
        {
            var query = new GetUserByGuidQuery(Guid.NewGuid());

            var result = await _handler.Handle(query, CancellationToken.None);

            Assert.That(result.Success, Is.False);
            Assert.That(result.ErrorMessage, Is.EqualTo("User with that id doesn't exist"));
        }
        [TearDown]
        public void TearDown()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }
    }
}
