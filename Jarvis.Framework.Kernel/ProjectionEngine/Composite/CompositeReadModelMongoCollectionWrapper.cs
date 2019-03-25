using Castle.Core.Logging;
using Jarvis.Framework.Shared.Helpers;
using Jarvis.Framework.Shared.ReadModel;
using Jarvis.Framework.Shared.ReadModel.Composite;
using MongoDB.Driver;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Jarvis.Framework.Kernel.ProjectionEngine.Composite
{
    public class CompositeReadModelMongoCollectionWrapper<TModel> :
          ICompositeReadModelCollectionWrapper<TModel>
        where TModel : class, ICompositeReadModel
    {
        private const string ConcurrencyException = "E11000";

        private readonly IMongoCollection<TModel> _collection;
        private readonly Int32 _actualVersion;

        public CompositeReadModelMongoCollectionWrapper(
            IMongoDatabase readmodelDb)
        {
            var collectionName = CollectionNames.GetCollectionName<TModel>();
            _collection = readmodelDb.GetCollection<TModel>(collectionName);

            Logger = NullLogger.Instance;

            //Auto create the basic index you need
            _collection.Indexes.CreateOne(
                new CreateIndexModel<TModel>(
                    Builders<TModel>.IndexKeys
                        .Ascending(_ => _.ProjectedPosition),
                    new CreateIndexOptions()
                    {
                        Name = "ProjectedPosition",
                        Background = false
                    }
                )
             );
        }

        /// <summary>
        /// Initialized by castle
        /// </summary>
        public ILogger Logger { get; set; }

        public IQueryable<TModel> AsQueryable()
        {
            return _collection.AsQueryable();
        }

        public async Task<TModel> FindOneByIdAsync(String id)
        {
            return await _collection.FindOneByIdAsync(id).ConfigureAwait(false);
        }

        public async Task UpsertAsync(TModel model)
        {
            if (String.IsNullOrEmpty(model.Id))
            {
                throw new CollectionWrapperException("Cannot save readmodel, Id property not initialized");
            }

            try
            {
                await _collection.FindOneAndReplaceAsync<TModel>(
               Builders<TModel>.Filter.And(
                   Builders<TModel>.Filter.Eq(rm => rm.Id, model.Id),
                   Builders<TModel>.Filter.Lte(rm => rm.ProjectedPosition, model.ProjectedPosition),
                   Builders<TModel>.Filter.Lte(rm => rm.ReadModelVersion, model.ReadModelVersion)
               ),
               model,
               new FindOneAndReplaceOptions<TModel, TModel>()
               {
                   IsUpsert = true,
               }).ConfigureAwait(false);
            }
            catch (MongoException mex)
            {
                //we can have duplicate index exception, it happens when you are trying
                //to upsert a document with an older version, the condition does not match
                //the routine tries an upsert, and the upsert fails.
                if (!mex.Message.Contains(ConcurrencyException))
                    throw;
            }
        }
    }
}
