using Jarvis.Framework.Kernel.ProjectionEngine.Composite;
using Jarvis.Framework.Shared.Helpers;
using Jarvis.Framework.Shared.IdentitySupport;
using Jarvis.Framework.Shared.ReadModel;
using Jarvis.Framework.Tests.ProjectionsTests.Composite.Support;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jarvis.Framework.Tests.ProjectionsTests.Composite
{
    public abstract class CompositeReadModelCollectionWrapperTestBase
    {
        protected CompositeReadModelMongoCollectionWrapper<SimpleTestCompositeReadModel> _sut;
        protected IMongoDatabase _db;

        protected IMongoCollection<SimpleTestCompositeReadModel> _collection;
        protected IMongoCollection<BsonDocument> _mongoBsonCollection;
        protected IdentityManager _identityManager;

        protected void Init()
        {
            var url = new MongoUrl(ConfigurationManager.ConnectionStrings["readmodel"].ConnectionString);
            var client = new MongoClient(url);
            _db = client.GetDatabase(url.DatabaseName);
            _collection = _db.GetCollection<SimpleTestCompositeReadModel>(
                CollectionNames.GetCollectionName<SimpleTestCompositeReadModel>());
            _db.Drop();

            _identityManager = new IdentityManager(new CounterService(_db));
            _mongoBsonCollection = _db.GetCollection<BsonDocument>(CollectionNames.GetCollectionName<SimpleTestCompositeReadModel>());
        }

        protected void InitSingleTest()
        {
            _lastCommit = 1;
            _lastPosition = 0;
            _aggregateVersion = 1;
            _aggregateIdSeed++;

            GenerateSut();
        }

        protected void GenerateSut()
        {
            _sut = new CompositeReadModelMongoCollectionWrapper<SimpleTestCompositeReadModel>(_db);
        }

        protected Int64 _lastCommit;
        protected Int32 _lastPosition;

        protected Int64 _aggregateIdSeed = 1;
        protected Int32 _aggregateVersion = 1;
    }
}
