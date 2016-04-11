﻿using System;
using System.Collections.Generic;
using System.Linq;
using MongoDB.Bson;
using MongoDB.Driver;
using Jarvis.Framework.Shared.Helpers;

namespace Jarvis.Framework.Kernel.Support
{
    public class ProjectionStatusLoader : IProjectionStatusLoader
    {
        private readonly int _pollingIntervalInSeconds;
        private IMongoCollection<BsonDocument> _commitsCollection;
        private IMongoCollection<BsonDocument> _checkpointCollection;

        private DateTime lastPoll = DateTime.MinValue;

        private Dictionary<String, SlotStatus> _lastMetrics;
        private Int64 _lastDelay;

        public ProjectionStatusLoader(
            IMongoDatabase eventStoreDatabase,
            IMongoDatabase readModelDatabase,
            Int32 pollingIntervalInSeconds = 5)
        {
            _pollingIntervalInSeconds = pollingIntervalInSeconds;
            _commitsCollection = eventStoreDatabase.GetCollection<BsonDocument>("Commits");
            _checkpointCollection = readModelDatabase.GetCollection<BsonDocument>("checkpoints");
            _lastMetrics = new Dictionary<string, SlotStatus>();
        }

        public IEnumerable<SlotStatus> GetSlotMetrics()
        {
            GetMetricValue();
            return _lastMetrics.Values.ToList();
        }

        public SlotStatus GetSlotMetric(string slotName)
        {
            GetMetricValue();
            if (!_lastMetrics.ContainsKey(slotName)) return SlotStatus.Null;

            return _lastMetrics[slotName];
        }

        public long GetMaxCheckpointToDispatch()
        {
            GetMetricValue();
            return _lastDelay;
        }

        private void GetMetricValue()
        {
            if (DateTime.Now.Subtract(lastPoll).TotalSeconds > _pollingIntervalInSeconds)
            {
                _lastDelay = 0;
                var lastCommitDoc = _commitsCollection
                    .FindAll()
                    .Sort(Builders<BsonDocument>.Sort.Descending("_id"))
                    .Project(Builders<BsonDocument>.Projection.Include("_id"))
                    .FirstOrDefault();
                if (lastCommitDoc == null) return;

                var lastCommit = lastCommitDoc["_id"].AsInt64;

                //db.checkpoints.aggregate(
                //[
                //    {$match : {"Active": true}}
                //    ,{$project : {"Slot" : 1, "Current" : 1}}
                //    ,{$group : {"_id" : "$Slot", "Current" : {$min : "$Current"}}}
                //])
                BsonDocument[] pipeline =
                {
                    BsonDocument.Parse(@"{$match : {""Active"": true}}"),
                    BsonDocument.Parse(@"{$project : {""Slot"" : 1, ""Current"" : 1}}"),
                    BsonDocument.Parse(@"{$group : {""_id"" : ""$Slot"", ""Current"" : {$min : ""$Current""}}}")
                };
                //AggregateArgs args = new AggregateArgs();
                //args.Pipeline = pipeline;
                //args.AllowDiskUse = true; //important for large file systems
                var options = new AggregateOptions();
                options.AllowDiskUse = true;
                options.UseCursor = true;
                var allCheckpoints = _checkpointCollection.Aggregate(options)
                    .Match(pipeline[0])
                    .Project(pipeline[1])
                    .Group(pipeline[2])
                    .ToList();
                foreach (BsonDocument metric in allCheckpoints)
                {
                    var slotName = metric["_id"].AsString;
                    Int64 current;
                    if (!metric["Current"].IsBsonNull)
                    {
                        current = Int64.Parse(metric["Current"].AsString);
                    }
                    else
                    {
                        current = 0;
                    }

                    var delay = lastCommit - current;
                    if (delay > _lastDelay) _lastDelay = delay;
                    _lastMetrics[slotName] = new SlotStatus(slotName, delay);
                }
                lastPoll = DateTime.Now;
            }
        }


    }
}