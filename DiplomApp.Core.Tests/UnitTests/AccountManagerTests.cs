using DiplomApp.Accounts;
using DiplomApp.Data;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System;
using System.Text;
using System.Threading.Tasks;

namespace DiplomApp.Core.Tests.UnitTests
{
    [TestFixture]
    public class AccountManagerTests
    {
        private static AccountManager CreateManager()
        {
            var options = new DbContextOptionsBuilder<UserAccountContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            var context = new UserAccountContext(options);
            return new AccountManager(context);
        }

        [Test]
        public void CreateAccount_NewLogin_StoresHashedKeyNotPlaintext()
        {
            var manager = CreateManager();

            var account = (UserAccount)manager.CreateAccount("alice", "password123");

            Assert.That(account.Login, Is.EqualTo("alice"));
            Assert.That(account.Salt, Is.Not.Null.And.Not.Empty);
            Assert.That(account.Key, Is.Not.Null.And.Not.Empty);
            Assert.That(account.Key, Is.Not.EqualTo(Encoding.UTF8.GetBytes("password123")));
        }

        [Test]
        public void CreateAccount_ExistingLogin_ReturnsExistingWithoutDuplicate()
        {
            var manager = CreateManager();
            var first = (UserAccount)manager.CreateAccount("bob", "secret");

            var second = (UserAccount)manager.CreateAccount("bob", "different");

            Assert.That(second.ID, Is.EqualTo(first.ID));
            Assert.That(second.Key, Is.EqualTo(first.Key));
        }

        [Test]
        public void Login_CorrectPassword_ReturnsTrueAndSetsCurrentUser()
        {
            var manager = CreateManager();
            manager.CreateAccount("carol", "qwerty");

            var result = manager.Login("carol", "qwerty");

            Assert.That(result, Is.True);
            Assert.That(manager.CurrentUser, Is.Not.Null);
            Assert.That(manager.CurrentUser!.Login, Is.EqualTo("carol"));
        }

        [Test]
        public void Login_WrongPassword_ReturnsFalse()
        {
            var manager = CreateManager();
            manager.CreateAccount("dave", "correct");

            var result = manager.Login("dave", "wrong");

            Assert.That(result, Is.False);
            Assert.That(manager.CurrentUser, Is.Null);
        }

        [Test]
        public void Login_UnknownLogin_ReturnsFalse()
        {
            var manager = CreateManager();

            var result = manager.Login("ghost", "whatever");

            Assert.That(result, Is.False);
        }

        [Test]
        public async Task LoginAsync_CorrectPassword_ReturnsTrue()
        {
            var manager = CreateManager();
            manager.CreateAccount("erin", "p@ss");

            var result = await manager.LoginAsync("erin", "p@ss");

            Assert.That(result, Is.True);
            Assert.That(manager.CurrentUser!.Login, Is.EqualTo("erin"));
        }

        [Test]
        public async Task CheckIfAccountExists_ReturnsTrueForExistingAndFalseForMissing()
        {
            var manager = CreateManager();
            manager.CreateAccount("frank", "pwd");

            Assert.That(await manager.CheckIfAccountExists("frank"), Is.True);
            Assert.That(await manager.CheckIfAccountExists("nobody"), Is.False);
        }

        [Test]
        public void Logout_ClearsCurrentUser()
        {
            var manager = CreateManager();
            manager.CreateAccount("gina", "1234");
            manager.Login("gina", "1234");
            Assert.That(manager.CurrentUser, Is.Not.Null);

            manager.Logout();

            Assert.That(manager.CurrentUser, Is.Null);
        }
    }
}
