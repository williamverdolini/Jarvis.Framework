
using System;

namespace Jarvis.Framework.Shared.ReadModel.Composite
{
    /// <summary>
    /// Composite readmodel is a reamodel that is the result
    /// of a composition of more than one atomic readmodel
    /// and is used to create readmodel that "join" data between
    /// different aggregate. 
    /// </summary>
    public interface ICompositeReadModel : IReadModel
    {
        /// <summary>
        /// <para>
        /// This is the id of the readmodel, usually it is the id of a root
        /// aggregate that represent the logical base aggregate to project.
        /// </para>
        /// <para>
        /// In this first version, each composite aggregate as an aggregate instance
        /// that is the root of the readmodel. Related projection should be able
        /// to generate an instance of composite readmodel starting from
        /// basic atomic readmodels.
        /// </para>
        /// </summary>
        String Id { get;  }

        /// <summary>
        /// <para>
        /// This is the higher <see cref="Atomic.IAtomicReadmodel.ProjectedPosition"/> of all
        /// the basic <see cref="Atomic.IAtomicReadModel"/> readmodels used to build this
        /// composite readmodel.
        /// </para>
        /// <para>
        /// This is the value used to guarantee idempotency and thread safe save of the
        /// readmodel, lower version will always be discharded if the database has an higher version.
        /// </para>
        /// </summary>
        Int64 ProjectedPosition { get; }

        /// <summary>
        /// This property is set to true if there were some error projecting the readmodel, this
        /// means that this specific readmodel is not up to date anymore.
        /// </summary>
        Boolean Faulted { get; }

        /// <summary>
        /// Needed to understand if this readmodel is old and should be rebuilded
        /// </summary>
        Int32 ReadModelVersion { get; }

        /// <summary>
        /// Mark the readmodel as faulted.
        /// </summary>
        /// <returns></returns>
        void MarkAsFaulted(Int64 projectedPosition);
    }
}
