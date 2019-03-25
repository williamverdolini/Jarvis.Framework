using Jarvis.Framework.Shared.ReadModel.Composite;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Jarvis.Framework.Kernel.ProjectionEngine.Composite
{
    /// <summary>
    /// Simple wrapper to query a <see cref="ICompositeReadModel"/> collection.
    /// </summary>
    /// <typeparam name="TModel"></typeparam>
    /// <typeparam name="TKey"></typeparam>
    public interface ICompositeReadModelCollectionReader<TModel>
         where TModel : ICompositeReadModel
    {
        IQueryable<TModel> AsQueryable();

        /// <summary>
        /// Differently from <see cref="Atomic.IAtomicCollectionReader{TModel}.FindOneByIdAsync(string)"/>
        /// method, in this first version this does not autofix the readmodel, if an error occurred
        /// the error is returned to the caller.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<TModel> FindOneByIdAsync(String id);
    }

    public interface ICompositeReadModelCollectionWrapperFactory
    {
        ICompositeReadModelCollectionWrapper<TModel> CreateCollectionWrappper<TModel>()
             where TModel : ICompositeReadModel;
    }

    public interface ICompositeReadModelCollectionWrapper<TModel> :
        ICompositeReadModelCollectionReader<TModel>
        where TModel : ICompositeReadModel
    {
        /// <summary>
        /// Insert or update a composite readmodel, if readmodel on database is newer
        /// or it has an older version, the record will be overwritten.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task UpsertAsync(TModel model);
    }
}
