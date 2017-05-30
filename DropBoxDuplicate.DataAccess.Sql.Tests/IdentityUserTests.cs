using System;
using System.ComponentModel.Design;
using System.Configuration;
using DropBoxDuplicate.Model;
using Microsoft.AspNet.Identity;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data.SqlClient;
using Ninject;
using Ninject.Selection.Heuristics;


namespace DropBoxDuplicate.DataAccess.Sql.Tests
{
    [TestClass]
    public class IdentityUserTests
    {
        private static string connectionString;//= ConfigurationManager.ConnectionStrings["DBD"].ConnectionString;

        public IUserStore<IdentityUser, Guid> _userRepository;// = new IdentityUserRepository(connectionString);

        //var _userRepository = new IdentityUserRepository();

        public IdentityUser TestUser { get; set; }
        
        //[TestInitialize]
        /*public void Init()
        {
            //_userRepository ;
           
            //IKernel kernel = new StandardKernel();
            //kernel.Components.Add<IInjectionHeuristic, StandardInjectionHeuristic>();
            //kernel.Bind<IUserStore<IdentityUser, Guid>, IdentityUserRepository>();
            /*kernel.Bind<SqlConnectionStringBuilder>()
                .ToConstant(new SqlConnectionStringBuilder() { ConnectionString = connectionString });*/

            //            TestUser.Id = Guid.NewGuid();
            //            TestUser.UserName = "testName";
            //            TestUser.PasswordHash = "test_password";
            //            TestUser.Email = "test@email.com";
            //            TestUser.EmailConfirmed = true;
            //            TestUser.BirthDate = DateTimeOffset.Now;
            //            TestUser.City = "TestCity";
            //            TestUser.FirstName = "FirstNameTest";
            //            TestUser.SecondName = "SecondNameTest";
            //            TestUser.RegDate = DateTimeOffset.Now;
            //            TestUser.SecurityStamp = Guid.NewGuid();

            //            _userRepository.CreateAsync(TestUser);
        //}

        [TestCleanup]
        public void Clean()
        {
            if (TestUser != null)
            {
               _userRepository.DeleteAsync(TestUser);
            }
        }

        [TestMethod]
        public void Should_create_and_get_user()
        {
            //arrange
            var user = new IdentityUser
            {
                Id = Guid.NewGuid(),
                UserName = "testName",
                PasswordHash = "test_password",
                Email = "test@email.com",
                EmailConfirmed = true,
                BirthDate = DateTimeOffset.Now,
                City = "TestCity",
                FirstName = "FirstNameTest",
                SecondName = "SecondNameTest",
                RegDate = DateTimeOffset.Now,
                SecurityStamp = Guid.NewGuid()
            };
            
            //act
            _userRepository.CreateAsync(user);

            var result = _userRepository.FindByIdAsync(user.Id).Result;
            
            //asserts
            Assert.AreEqual(user.Id, result.Id);
            Assert.AreEqual(user.UserName, result.UserName);
            //Assert.AreEqual(user.PasswordHash, result.PasswordHash);
            Assert.AreEqual(user.Email, result.Email);
            Assert.AreEqual(user.EmailConfirmed, result.EmailConfirmed);
            Assert.AreEqual(user.BirthDate, result.BirthDate);
            Assert.AreEqual(user.City, result.City);
            Assert.AreEqual(user.FirstName, result.FirstName);
            Assert.AreEqual(user.SecondName, result.SecondName);
            Assert.AreEqual(user.RegDate, result.RegDate);
            Assert.AreEqual(user.SecurityStamp, result.SecurityStamp);
            

        }
    }
}
