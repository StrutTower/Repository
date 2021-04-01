﻿using Dapper;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using TowerSoft.RepositoryTests.Interfaces;
using TowerSoft.RepositoryTests.MicrosoftSql;
using TowerSoft.RepositoryTests.TestObjects;

namespace TowerSoft.RepositoryTests.DbRepository {
    [TestClass]
    public class MicrosoftSqlRepositoryTests : DbRepositoryTests {
        private static UnitOfWork uow;

        [ClassInitialize]
        public static void ClassInitialize(TestContext testContext) {
            uow = new UnitOfWork();
            //uow.DbAdapter.DbConnection.Execute("DROP TABLE testobject;");
            //uow.DbAdapter.DbConnection.Execute("DROP TABLE counttest;");
            uow.DbAdapter.DbConnection.Execute("" +
                "IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='testobject' AND xtype='U')" +
                    "CREATE TABLE testobject (" +
                    "ID BIGINT IDENTITY(1,1) PRIMARY KEY," +
                    "Title NVARCHAR(45) NOT NULL UNIQUE," +
                    "Description NVARCHAR(MAX)," +
                    "StatusID INT NOT NULL," +
                    "InputOn DATETIME NOT NULL," +
                    "InputByID INT NOT NULL," +
                    "IsActive TINYINT NOT NULL)");
            uow.DbAdapter.DbConnection.Execute("TRUNCATE TABLE testobject");
            uow.DbAdapter.DbConnection.Execute("" +
                "IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='counttest' AND xtype='U')" +
                    "CREATE TABLE counttest " +
                    "(" +
                    "Number INT PRIMARY KEY," +
                    "Name VARCHAR(45) NOT NULL UNIQUE)");
            uow.DbAdapter.DbConnection.Execute("TRUNCATE TABLE counttest");
            CountTestRepository repo = uow.GetRepo<CountTestRepository>();
            repo.Add(new CountTest { Number = 1, Name = "Object 1" });
            repo.Add(new CountTest { Number = 2, Name = "Object 2" });
            repo.Add(new CountTest { Number = 3, Name = "Object 3" });
            repo.Add(new CountTest { Number = 4, Name = "Object 4" });
        }

        protected override ITestObjectRepository GetTestObjectRepository() {
            return uow.GetRepo<TestObjectRepository>();
        }

        protected override ICountTestRepository GetCountTestRepository() {
            return uow.GetRepo<CountTestRepository>();
        }
    }
}
