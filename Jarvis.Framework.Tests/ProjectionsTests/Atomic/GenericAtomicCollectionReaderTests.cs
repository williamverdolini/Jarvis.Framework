using Jarvis.Framework.Kernel.ProjectionEngine.Atomic;
using Jarvis.Framework.Tests.ProjectionsTests.Atomic.Support;
using NSubstitute;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jarvis.Framework.Tests.ProjectionsTests.Atomic
{
    [TestFixture]
    public class GenericAtomicCollectionReaderTests
    {
        private const string _sampleId = "SampleAggregate_1";

        [Test]
        public async Task Can_use_correct_reader()
        {
            var reader1 = Substitute.For<IAtomicCollectionReader<SimpleTestAtomicReadModel>>();
            var reader2 = Substitute.For<IAtomicCollectionReader<AnotherSimpleTestAtomicReadModel>>();

            reader1.ReadModelType.Returns(typeof(SimpleTestAtomicReadModel));
            reader2.ReadModelType.Returns(typeof(AnotherSimpleTestAtomicReadModel));

            var sut = new GenericAtomicCollectionReader(new IAtomicCollectionReader[] { reader1, reader2 });

            SimpleTestAtomicReadModel rm = new SimpleTestAtomicReadModel(_sampleId);
            reader1.FindOneByIdAsync(_sampleId).Returns(Task.FromResult(rm));

            //ok try to exercise the reader, it should delegate to correct reader
            var result = await sut.FindOneByIdAsync<SimpleTestAtomicReadModel>(_sampleId).ConfigureAwait(false);
            Assert.That(Is.ReferenceEquals(result, rm));
        }
    }
}
