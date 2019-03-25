using Jarvis.Framework.Kernel.ProjectionEngine.Atomic;
using Jarvis.Framework.Shared.ReadModel.Composite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jarvis.Framework.Kernel.ProjectionEngine.Composite
{
    public abstract class AbstractCompositeReadmodelProjection<TModel>
        : ICompositeReadModelProjection<TModel>
        where TModel : ICompositeReadModel
    {
        public GenericAtomicCollectionReader GenericAtomicCollectionReader { get; set; }

        public TModel Project(string id)
        {
            return OnProject(id);
        }

        /// <summary>
        /// This is the real method that should be implemented to project the
        /// aggregate.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        protected abstract TModel OnProject(string id);
    }
}
