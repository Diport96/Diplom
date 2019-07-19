using DiplomApp.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using static DiplomApp.App;

namespace DiplomApp.Core.Tests.IntegrateTests
{
    [TestClass]
    public class AccountManagerTests
    {
        [TestMethod]
        public void CreateAccount_IntegrateTestAccount_NotNullReturned()
        {
            string userName = "TestAcc@bk.ru";
            string password = "123456798AbCd";

            var account = UserAccountManager.CreateAccount(userName, password);

            Assert.IsNotNull(account);

            using (var database = new UserAccountContext())
            {
                var acc = database.UserAccounts.FirstOrDefault(x => x.Login == userName);
                if (acc != null)
                {
                    database.UserAccounts.Remove(acc);
                    database.SaveChanges();
                }
            }
        }

        [TestMethod]
        public void CrateAccount_IntegrateTestAccount_CurrentUserNameAccountReturned()
        {
            // Expected
            string userName = "TestAcc@bk.ru";
            string password = "123456798AbCd";

            // Actual
            var account = UserAccountManager.CreateAccount(userName, password);

            Assert.AreEqual(userName, account.Login);

            using (var database = new UserAccountContext())
            {
                var acc = database.UserAccounts.FirstOrDefault(x => x.Login == userName);
                if (acc != null)
                {
                    database.UserAccounts.Remove(acc);
                    database.SaveChanges();
                }
            }
        }

        [TestMethod]
        public void Login_ExistingUser_TrueReturned()
        {
            string userName = "TestAcc@bk.ru";
            string password = "123456798AbCd";
            UserAccountManager.CreateAccount(userName, password);

            // Actual
            bool actual = UserAccountManager.Login(userName, password);

            Assert.IsTrue(actual);

            using (var database = new UserAccountContext())
            {
                var acc = database.UserAccounts.FirstOrDefault(x => x.Login == userName);
                if (acc != null)
                {
                    database.UserAccounts.Remove(acc);
                    database.SaveChanges();
                }
            }
        }

        [TestMethod]
        public void Login_NotExistingUser_FalseReturned()
        {
            string userName = "TestAcc@bk.ru";
            string password = "123456798AbCd";

            // Actual
            bool actual = UserAccountManager.Login(userName, password);

            Assert.IsFalse(actual);
        }

        [TestMethod]
        public void Login_CurrentUserLoginIsEqualLoginedUserLogin_TrueReturned()
        {
            // Expected
            string userName = "TestAcc@bk.ru";
            string password = "123456798AbCd";
            UserAccountManager.CreateAccount(userName, password);
            UserAccountManager.Login(userName, password);

            // Actual
            string actual = UserAccountManager.CurrentUser.Login;

            Assert.AreEqual(userName, actual);

            using (var database = new UserAccountContext())
            {
                var acc = database.UserAccounts.FirstOrDefault(x => x.Login == userName);
                if (acc != null)
                {
                    database.UserAccounts.Remove(acc);
                    database.SaveChanges();
                }
            }
        }
    }
}
