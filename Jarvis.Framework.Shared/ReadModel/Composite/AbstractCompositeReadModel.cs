using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.Framework.Shared.ReadModel.Composite
{
    public abstract class AbstractCompositeReadModel : ICompositeReadModel
    {
        public string Id { get; private set; }

        public long ProjectedPosition { get; private set; }

        protected abstract Int32 GetVersion();
        private Int32? _readModelVersion;

        /// <summary>
        /// This is a read / write property because we want this property
        /// to be serialized in mongo and retrieved to check if the readmodel
        /// in mongo has a different signature. Lower signature is the sign 
        /// of an older readmodel. Higher signature is the sign of an higher 
        /// and newer readmodel.
        /// </summary>
        public Int32 ReadModelVersion
        {
            get
            {
                return (_readModelVersion ?? (_readModelVersion = GetVersion())).Value;
            }
            set
            {
                _readModelVersion = value;
            }
        }

        public bool Faulted { get; private set; }

        public void MarkAsFaulted(long projectedPosition)
        {
            Faulted = true;
            ProjectedPosition = projectedPosition;
        }

        protected AbstractCompositeReadModel(String id)
        {
            Id = id;
        }

        public void SetProjectionInfo(
            Int64 projectedPosition)
        {
            ProjectedPosition = projectedPosition;
        }
    }
}
