﻿using System;
using System.Threading;
using NUnit.Framework;
using Jarvis.Framework.Tests.BusTests.MessageFolder;
using Jarvis.Framework.Kernel.Commands;
using Jarvis.Framework.Shared.Commands;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Jarvis.Framework.Tests.BusTests.Handlers
{
    public class AnotherSampleCommandHandler : ICommandHandler<AnotherSampleTestCommand>
    {
        /// <summary>
        /// This is used to simply test, you can decide how to handle a command.
        /// </summary>
        /// <param name="messageId"></param>
        /// <param name="numOfFailure"></param>
        /// <param name="exceptionToThrow"></param>
        /// <param name="setEventOnThrow"></param>
        public static void SetFixtureData(Guid messageId, Int32 numOfFailure, Exception exceptionToThrow, Boolean setEventOnThrow)
        {
            commandRunDatas[messageId] = new RunData(numOfFailure, setEventOnThrow, exceptionToThrow);
        }

        private static Dictionary<Guid, RunData> commandRunDatas = new Dictionary<Guid, RunData>();

        public AnotherSampleCommandHandler()
        {

        }

        public readonly ManualResetEvent Reset = new ManualResetEvent(false);

        public Task HandleAsync(AnotherSampleTestCommand cmd)
        {
            RunData runData;
            if (!commandRunDatas.TryGetValue(cmd.MessageId, out runData))
            {
                Assert.Fail($"TEST FIXTURE WRONG: No run data for command id {cmd.MessageId}");
            }

            if (runData.ShouldFail())
            {
                runData.Fail(Reset);
            }
            this.ReceivedCommand = cmd;
            Reset.Set();
            return Task.CompletedTask;
        }

        public Task HandleAsync(ICommand command)
        {
            return HandleAsync(command as AnotherSampleTestCommand);
        }

        public AnotherSampleTestCommand ReceivedCommand { get; private set; }

        private class RunData
        {
            public RunData(int numOfFailures, bool setEvent, Exception ex)
            {
                NumOfFailures = numOfFailures;
                SetEvent = setEvent;
                Ex = ex;
                _failCount = 0;
            }

            public Int32 NumOfFailures { get; private set; }

            public Boolean SetEvent { get; private set; }

            public Exception Ex { get; private set; }

            private Int32 _failCount;

#pragma warning disable S3242 // Method parameters should be declared with base types
            internal void Fail(ManualResetEvent reset)
#pragma warning restore S3242 // Method parameters should be declared with base types
            {
                _failCount++;
                if (SetEvent) reset.Set();
                throw Ex;
            }

            internal bool ShouldFail()
            {
                return _failCount < NumOfFailures;
            }
        }
    }
}
