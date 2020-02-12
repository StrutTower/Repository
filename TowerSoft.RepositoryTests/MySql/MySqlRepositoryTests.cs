﻿using Dapper;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using TowerSoft.RepositoryTests.TestObjects;

namespace TowerSoft.RepositoryTests.MySql {
    [TestClass]
    public class MySqlRepositoryTests {
        private static UnitOfWork _uow;

        [ClassInitialize]
        public static void ClassInitialize(TestContext testContext) {
            _uow = new UnitOfWork();
            _uow.DbAdapter.DbConnection.Execute("CREATE TABLE IF NOT EXISTS testobject (" +
                "ID BIGINT(20) AUTO_INCREMENT PRIMARY KEY," +
                "Title VARCHAR(45) NOT NULL UNIQUE," +
                "Description MEDIUMTEXT," +
                "StatusID INT NOT NULL," +
                "InputOn DATETIME NOT NULL," +
                "InputByID INT NOT NULL," +
                "IsActive TINYINT(1) NOT NULL) " +
                "ENGINE=InnoDB;");
            _uow.DbAdapter.DbConnection.Execute("TRUNCATE TABLE testobject");
            _uow.DbAdapter.DbConnection.Execute("CREATE TABLE IF NOT EXISTS counttest (" +
                "ID INT AUTO_INCREMENT PRIMARY KEY," +
                "Name VARCHAR(45) NOT NULL UNIQUE) " +
                "ENGINE=InnoDB;");
            _uow.DbAdapter.DbConnection.Execute("TRUNCATE TABLE counttest");
            CountTestRepository repo = _uow.GetRepo<CountTestRepository>();
            repo.Add(new CountTest { ID = 1, Name = "Object 1" });
            repo.Add(new CountTest { ID = 2, Name = "Object 2" });
            repo.Add(new CountTest { ID = 3, Name = "Object 3" });
            repo.Add(new CountTest { ID = 4, Name = "Object 4" });
        }

        [TestMethod]
        public void Add_TestObject_ShouldAdd() {
            TestObjectRepository repo = _uow.GetRepo<TestObjectRepository>();
            TestObject newObj = new TestObject {
                Title = "Add Test",
                Description = "Add Test Description",
                StatusID = Status.Active,
                InputOn = DateTime.Now,
                InputByID = 1,
                IsActive = true
            };

            repo.Add(newObj);

            TestObject fromDB = repo.GetByTitle(newObj.Title);

            Assert.IsNotNull(fromDB); // Make sure object was returned
            Assert.AreNotEqual(0, fromDB); // Make sure autonumber was assigned
            Assert.IsTrue(newObj.AllPropsEqual(fromDB), "The object returned from the database does not match the original");
        }

        [TestMethod]
        public void Update_TestObject_ShouldUpdate() {
            TestObjectRepository repo = _uow.GetRepo<TestObjectRepository>();
            TestObject newObj = new TestObject {
                Title = "Update Test",
                Description = "Update Test Description",
                StatusID = Status.Active,
                InputOn = DateTime.Now,
                InputByID = 1,
                IsActive = true
            };
            repo.Add(newObj);

            newObj.Title = newObj.Title + " - Updated";
            newObj.StatusID = Status.Closed;
            repo.Update(newObj);
            TestObject fromDB = repo.GetByID(newObj.ID);

            Assert.IsNotNull(fromDB); // Make sure object was returned
            Assert.IsTrue(newObj.AllPropsEqual(fromDB), "The object returned from the database does not match the updated original");
        }

        [TestMethod]
        public void Remove_TestObject_ShouldRemove() {
            TestObjectRepository repo = _uow.GetRepo<TestObjectRepository>();
            TestObject newObj = new TestObject {
                Title = "Remove Test",
                Description = "Remove Test Description",
                StatusID = Status.Pending,
                InputOn = DateTime.Now,
                InputByID = 1,
                IsActive = true
            };
            repo.Add(newObj);

            TestObject fromDbNotNull = repo.GetByID(newObj.ID);
            repo.Remove(newObj);
            TestObject fromDbNull = repo.GetByID(newObj.ID);

            Assert.IsNotNull(fromDbNotNull);
            Assert.IsNull(fromDbNull);
        }

        [TestMethod]
        public void GetAll_CountTest_ShouldGetAll() {
            CountTestRepository repo = _uow.GetRepo<CountTestRepository>();
            List<CountTest> all = repo.GetAll();

            Assert.AreEqual(4, all.Count);
            Assert.AreEqual("Object 2", all.SingleOrDefault(x => x.ID == 2).Name);
        }

        [TestMethod]
        public void GetCount_Count_ShouldGetCount() {
            CountTestRepository repo = _uow.GetRepo<CountTestRepository>();
            long count = repo.GetCount();
            Assert.AreEqual(4, count);
        }
    }
}
