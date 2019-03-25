using Jarvis.Framework.Shared.ReadModel.Atomic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jarvis.Framework.Kernel.ProjectionEngine.Atomic
{
    /// <summary>
    /// A generic class that should be registered by castle that
    /// allows accessing all instance registered for every atomic
    /// readmodel. It is used by the composite readmodel projection
    /// to avoid creating too much dependencies.
    /// </summary>
    public class GenericAtomicCollectionReader
    {
        private readonly IDictionary<Type, IAtomicCollectionReader> _allAtomicCollectionReaders;

        public GenericAtomicCollectionReader(IAtomicCollectionReader[] allAtomicCollectionReaders)
        {
            _allAtomicCollectionReaders = allAtomicCollectionReaders
                .ToDictionary(r => r.ReadModelType);
        }

        /// <summary>
        /// Simple wrapper that allows reading ANY Atomic readmodel by a given Id.
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <param name="id"></param>
        /// <returns></returns>
        public Task<TModel> FindOneByIdAsync<TModel>(String id) where TModel : IAtomicReadModel
        {
            return ((IAtomicCollectionReader<TModel>)_allAtomicCollectionReaders[typeof(TModel)]).FindOneByIdAsync(id);
        }
    }
}
