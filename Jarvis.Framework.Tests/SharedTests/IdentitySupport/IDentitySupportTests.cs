﻿using Jarvis.Framework.Shared.Domain.Serialization;
using Jarvis.Framework.Shared.IdentitySupport;
using Jarvis.Framework.Shared.IdentitySupport.Serialization;
using Jarvis.Framework.Tests.Support;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Jarvis.Framework.Shared.Helpers;
using Jarvis.Framework.Tests.SharedTests.IdentitySupport;

namespace Jarvis.Framework.Tests.SharedTests.IdentitySupport
{
    [TestFixture]
    [Category("mongo_serialization")]
    public class IDentitySupportTests
    {
        private TestMapper sut;
        private TestFlatMapper sutFlat;
        private IMongoCollection<BsonDocument> _mappingCollection;
        private IMongoCollection<BsonDocument> _mappingFlatCollection;

        [TestFixtureSetUp]
        public void TestFixtureSetup()
        {

            TestHelper.RegisterSerializerForFlatId<TestId>();

            TestHelper.RegisterSerializerForFlatId<TestFlatId>();

        }

        [SetUp]
        public void SetUp()
        {
           var db = TestHelper.CreateNew(ConfigurationManager.ConnectionStrings["system"].ConnectionString);
            IdentityManager manager = new IdentityManager(new CounterService(db));
            manager.RegisterIdentitiesFromAssembly(Assembly.GetExecutingAssembly());
            _mappingCollection = db.GetCollection<BsonDocument>("map_testid");
            _mappingFlatCollection = db.GetCollection<BsonDocument>("map_testflatid");
            sut = new TestMapper(db, manager);
            sutFlat = new TestFlatMapper(db, manager);
        }

        [Test]
        public void Verify_delete_of_mapping()
        {
            var id = sut.Map("TEST");
            var mapCount = _mappingCollection.FindAll();
            Assert.That(mapCount.Count(), Is.EqualTo(1));

            sut.DeleteAliases(id);
            mapCount = _mappingCollection.FindAll();
            Assert.That(mapCount.Count(), Is.EqualTo(0));
        }


        [Test]
        public void Verify_delete_of_flat_mapping()
        {
            var id = sutFlat.Map("TEST");
            var mapCount = _mappingFlatCollection.FindAll();
            Assert.That(mapCount.Count(), Is.EqualTo(1));

            sutFlat.DeleteAliases(id);
            mapCount = _mappingFlatCollection.FindAll();
            Assert.That(mapCount.Count(), Is.EqualTo(0));
        }

        [Test]
        public void Verify_replace_alias_of_flat_mapping()
        {
            var id = sutFlat.Map("TEST");
            var mapCount = _mappingFlatCollection.FindAll();
            Assert.That(mapCount.Count(), Is.EqualTo(1));

            sutFlat.ReplaceAlias(id, "TEST2");
            mapCount = _mappingFlatCollection.FindAll();
            Assert.That(mapCount.Count(), Is.EqualTo(1));
        }

        [Test]
        public void Verify_exception_of_multiple_map()
        {
            sut.Addalias(new TestId(2), "Alias2");
            try
            {
                sut.Addalias(new TestId(3), "Alias2");
                Assert.Fail("Expect exception for invalid alias");
            }
            catch (Exception ex)
            {
                Assert.That(ex.Message, Contains.Substring("Alias alias2 already mapped to Test_2"));
            }
           
        }
    }

 


}