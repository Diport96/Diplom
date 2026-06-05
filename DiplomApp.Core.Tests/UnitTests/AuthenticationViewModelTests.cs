using ClientApp;
using DiplomApp;
using DiplomApp.Accounts;
using DiplomApp.ViewModels;
using DiplomApp.ViewModels.Services;
using Moq;
using NUnit.Framework;
using System;
using System.Threading.Tasks;

namespace DiplomApp.Core.Tests.UnitTests
{
    [TestFixture]
    [NonParallelizable] // использует глобальный App.UserAccountManager
    public class AuthenticationViewModelTests
    {
        private Mock<IWindowService> windowService = null!;
        private Mock<IWebApiClient> webApi = null!;
        private Mock<IAccountManager> accountManager = null!;
        private AuthenticationViewModel vm = null!;

        [SetUp]
        public void SetUp()
        {
            windowService = new Mock<IWindowService>();
            webApi = new Mock<IWebApiClient>();
            accountManager = new Mock<IAccountManager>();
            App.UserAccountManager = accountManager.Object;
            vm = new AuthenticationViewModel(windowService.Object, webApi.Object);
        }

        [Test]
        public async Task SignIn_EmptyLogin_ShowsErrorAndDoesNotCallApi()
        {
            await vm.SignIn("", "password");

            Assert.That(vm.AttemptShow, Is.True);
            webApi.Verify(x => x.LoginAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
            windowService.Verify(
                x => x.OpenMainWindow(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<Func<Task<bool>>>()),
                Times.Never);
        }

        [Test]
        public async Task SignIn_ApiSuccess_AccountMissing_CreatesAccountThenRedirects()
        {
            webApi.Setup(x => x.LoginAsync("alice", "pwd")).ReturnsAsync(true);
            accountManager.Setup(x => x.CheckIfAccountExists("alice")).ReturnsAsync(false);
            accountManager.Setup(x => x.CreateAccount("alice", "pwd")).Returns(Mock.Of<IAccount>());
            accountManager.Setup(x => x.LoginAsync("alice", "pwd")).ReturnsAsync(true);

            await vm.SignIn("alice", "pwd");

            accountManager.Verify(x => x.CreateAccount("alice", "pwd"), Times.Once);
            windowService.Verify(
                x => x.OpenMainWindow("alice", false, It.Is<Func<Task<bool>>>(f => f == null)),
                Times.Once);
        }

        [Test]
        public async Task SignIn_ApiSuccess_AccountExists_DoesNotCreateAndRedirects()
        {
            webApi.Setup(x => x.LoginAsync("bob", "pwd")).ReturnsAsync(true);
            accountManager.Setup(x => x.CheckIfAccountExists("bob")).ReturnsAsync(true);
            accountManager.Setup(x => x.LoginAsync("bob", "pwd")).ReturnsAsync(true);

            await vm.SignIn("bob", "pwd");

            accountManager.Verify(x => x.CreateAccount(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
            windowService.Verify(
                x => x.OpenMainWindow("bob", false, It.Is<Func<Task<bool>>>(f => f == null)),
                Times.Once);
        }

        [Test]
        public async Task SignIn_ApiFails_LocalLoginSuccess_RedirectsLocalSessionWithCallback()
        {
            webApi.Setup(x => x.LoginAsync("carol", "pwd")).ReturnsAsync(false);
            accountManager.Setup(x => x.LoginAsync("carol", "pwd")).ReturnsAsync(true);

            await vm.SignIn("carol", "pwd");

            windowService.Verify(
                x => x.OpenMainWindow("carol", true, It.IsNotNull<Func<Task<bool>>>()),
                Times.Once);
        }

        [Test]
        public async Task SignIn_ApiFails_LocalLoginFails_ShowsInvalidCredentials()
        {
            webApi.Setup(x => x.LoginAsync("dave", "bad")).ReturnsAsync(false);
            accountManager.Setup(x => x.LoginAsync("dave", "bad")).ReturnsAsync(false);

            await vm.SignIn("dave", "bad");

            Assert.That(vm.AttemptShow, Is.True);
            Assert.That(vm.AttemptMessage, Is.EqualTo("Неправильные логин или пароль"));
            windowService.Verify(
                x => x.OpenMainWindow(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<Func<Task<bool>>>()),
                Times.Never);
        }
    }
}
