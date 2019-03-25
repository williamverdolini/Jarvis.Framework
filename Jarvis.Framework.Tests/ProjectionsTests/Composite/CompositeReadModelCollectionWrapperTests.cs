using Jarvis.Framework.Tests.EngineTests;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jarvis.Framework.Tests.ProjectionsTests.Composite.Support
{
    [TestFixture]
    public class CompositeReadModelCollectionWrapperTests : CompositeReadModelCollectionWrapperTestBase
    {
        [OneTimeSetUp]
        public void TestFixtureSetUp()
        {
            Init();
        }

        [SetUp]
        public void SetUp()
        {
            InitSingleTest();
            SimpleTestCompositeReadModel.FakeVersion = 1;
        }

        [Test]
        public async Task Insert_composite_readmodel()
        {
            var rm = new SimpleTestCompositeReadModel(new SampleAggregateId(_aggregateIdSeed++));
            rm.MyProperty = "this is a test";
            rm.SetProjectionInfo(10);

            await _sut.UpsertAsync(rm).ConfigureAwait(false);

            var reloaded = await _sut.FindOneByIdAsync(rm.Id).ConfigureAwait(false);
            Assert.That(reloaded.MyProperty, Is.EqualTo(rm.MyProperty));
            Assert.That(reloaded.ProjectedPosition, Is.EqualTo(10));
        }

        [Test]
        public async Task Update_composite_readmodel_with_higher_version()
        {
            var rm = new SimpleTestCompositeReadModel(new SampleAggregateId(_aggregateIdSeed++));
            rm.MyProperty = "this is a test";
            rm.SetProjectionInfo(10);

            await _sut.UpsertAsync(rm).ConfigureAwait(false);

            //now update the version
            rm.MyProperty = "This is modified";
            rm.SetProjectionInfo(11);
            await _sut.UpsertAsync(rm).ConfigureAwait(false);

            var reloaded = await _sut.FindOneByIdAsync(rm.Id).ConfigureAwait(false);
            Assert.That(reloaded.MyProperty, Is.EqualTo(rm.MyProperty));
            Assert.That(reloaded.ProjectedPosition, Is.EqualTo(11));
        }

        [Test]
        public async Task Update_composite_readmodel_with_equal_Version()
        {
            var rm = new SimpleTestCompositeReadModel(new SampleAggregateId(_aggregateIdSeed++));
            rm.MyProperty = "this is a test";
            rm.SetProjectionInfo(10);

            await _sut.UpsertAsync(rm).ConfigureAwait(false);

            //now update the version
            rm.MyProperty = "This is modified";
            rm.SetProjectionInfo(10);
            await _sut.UpsertAsync(rm).ConfigureAwait(false);

            var reloaded = await _sut.FindOneByIdAsync(rm.Id).ConfigureAwait(false);
            Assert.That(reloaded.MyProperty, Is.EqualTo(rm.MyProperty));
            Assert.That(reloaded.ProjectedPosition, Is.EqualTo(10));
        }

        [Test]
        public async Task Update_composite_readmodel_with_lower_version()
        {
            var rm = new SimpleTestCompositeReadModel(new SampleAggregateId(_aggregateIdSeed++));
            const string originalPropertyValue = "this is a test";
            rm.MyProperty = originalPropertyValue;
            rm.SetProjectionInfo(10);

            await _sut.UpsertAsync(rm).ConfigureAwait(false);

            //now update the version
            rm.MyProperty = "This is modified";
            rm.SetProjectionInfo(9);
            await _sut.UpsertAsync(rm).ConfigureAwait(false);

            var reloaded = await _sut.FindOneByIdAsync(rm.Id).ConfigureAwait(false);
            Assert.That(reloaded.MyProperty, Is.EqualTo(originalPropertyValue));
            Assert.That(reloaded.ProjectedPosition, Is.EqualTo(10));
        }

        [Test]
        public async Task Update_composite_readmodel_with_higher_version_but_older_signature()
        {
            var rm = new SimpleTestCompositeReadModel(new SampleAggregateId(_aggregateIdSeed));
            const string originalPropertyValue = "this is a test";
            rm.MyProperty = originalPropertyValue;
            rm.SetProjectionInfo(10);
            SimpleTestCompositeReadModel.FakeVersion = 2;
            await _sut.UpsertAsync(rm).ConfigureAwait(false);

            //now update the version
            rm = new SimpleTestCompositeReadModel(new SampleAggregateId(_aggregateIdSeed++));
            rm.MyProperty = "This is modified";
            rm.SetProjectionInfo(12); //higher projected position
            SimpleTestCompositeReadModel.FakeVersion = 1; //but older version
            await _sut.UpsertAsync(rm).ConfigureAwait(false);

            var reloaded = await _sut.FindOneByIdAsync(rm.Id).ConfigureAwait(false);
            Assert.That(reloaded.MyProperty, Is.EqualTo(originalPropertyValue));
            Assert.That(reloaded.ProjectedPosition, Is.EqualTo(10));
        }
    }
}
