using Jarvis.Framework.Kernel.ProjectionEngine.Composite;
using Jarvis.Framework.Shared.ReadModel.Composite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jarvis.Framework.Tests.ProjectionsTests.Composite.Support
{
    internal class SimpleTestCompositeReadModelProjection<TModel> : AbstractCompositeReadmodelProjection<TModel>
        where TModel : ICompositeReadModel
    {
        /// <summary>
        /// Project a readmodel, should composite all the readmodel.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        protected override TModel OnProject(string id)
        {
            throw new NotImplementedException(); 
        }
    }
}
