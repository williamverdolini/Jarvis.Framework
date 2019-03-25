using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.Framework.Shared.ReadModel.Composite
{
    /// <summary>
    /// <para>
    /// The main difference between atomic and composite readmodel
    /// is that atomic readmodel is a simple class that can project
    /// a stream, while a composite needs a projection class that
    /// orchestrate building of the readmoel.
    /// </para>
    /// <para>
    /// Instead of having a readmodel that can be loaded from a store
    /// and updated passing a <see cref="Changeset"/> with events, we need
    /// a projection object that, given a single id, it can reconstruct
    /// the composite readmodel reading many different atomic readmodels.
    /// </para>
    /// </summary>
    public interface ICompositeReadModelProjection
    {
        /// <summary>
        /// Given a base id it will project a <see cref="ICompositeReadModel"/> in 
        /// memory. It does not need a starting <see cref="ICompositeReadModel"/> to 
        /// modify, but it will compose an instance using various basic <see cref="Atomic.IAtomicReadModel"/>
        /// instances.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        ICompositeReadModel Project(String id);
    }
}
