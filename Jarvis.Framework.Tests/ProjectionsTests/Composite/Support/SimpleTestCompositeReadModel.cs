using Jarvis.Framework.Shared.ReadModel.Composite;
using System;

namespace Jarvis.Framework.Tests.ProjectionsTests.Composite.Support
{
    public class SimpleTestCompositeReadModel : AbstractCompositeReadModel
    {
        public SimpleTestCompositeReadModel(string id) : base(id)
        {
        }

        public static Int32 FakeVersion = 1;

        protected override int GetVersion()
        {
            return FakeVersion;
        }

        public string MyProperty { get; set; }
    }
}
