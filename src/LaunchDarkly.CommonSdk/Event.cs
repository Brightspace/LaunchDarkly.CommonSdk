﻿using System;
using Newtonsoft.Json.Linq;

namespace LaunchDarkly.Client
{
    /// <summary>
    /// An analytics event that may be sent to LaunchDarkly.
    /// 
    /// This class and its subclasses are public so as to be usable by custom implementations of
    /// <see cref="IEventProcessor"/>. Application code should not construct or modify events; they
    /// are generated by the client.
    /// </summary>
    public abstract class Event
    {
        /// <summary>
        /// Attributes of the user who generated the event. Some attributes may not be sent
        /// to LaunchDarkly if they are private.
        /// </summary>
        public User User { get; private set; }

        /// <summary>
        /// Date/timestamp of the event.
        /// </summary>
        public long CreationDate { get; private set; }

        /// <summary>
        /// The unique key of the feature flag involved in the event.
        /// </summary>
        public string Key { get; private set; }
        
        internal Event(long creationDate, string key, User user)
        {
            CreationDate = creationDate;
            Key = key;
            User = user;
        }
    }

    /// <summary>
    /// An analytics event generated by feature flag evaluation.
    /// </summary>
    public class FeatureRequestEvent : Event
    {
        /// <summary>
        /// The variation index for the computed value of the flag.
        /// </summary>
        public int? Variation { get; private set; }

        /// <summary>
        /// The computed value of the flag.
        /// </summary>
        public JToken Value { get; private set; }

        /// <summary>
        /// The default value of the flag.
        /// </summary>
        public JToken Default { get; private set; }

        /// <summary>
        /// The version of the flag.
        /// </summary>
        public int? Version { get; private set; }
        
        /// <summary>
        /// The key of the flag that this flag is a prerequisite of, if any.
        /// </summary>
        public string PrereqOf { get; private set; }

        /// <summary>
        /// True if full-fidelity analytics events should be sent for this flag.
        /// </summary>
        public bool TrackEvents { get; private set; }

        /// <summary>
        /// If set, debug events are being generated until this date/time.
        /// </summary>
        public long? DebugEventsUntilDate { get; private set; }

        /// <summary>
        /// If set, this is a debug event.
        /// </summary>
        public bool Debug { get; private set; }

        /// <summary>
        /// An explanation of how the value was calculated, or null if the reason was not requested.
        /// </summary>
        public EvaluationReason Reason { get; private set; }

        internal FeatureRequestEvent(long creationDate, string key, User user, int? variation,
            JToken value, JToken defaultValue, int? version, string prereqOf, bool trackEvents, long? debugEventsUntilDate,
            bool debug, EvaluationReason reason) : base(creationDate, key, user)
        {
            Variation = variation;
            Value = value;
            Default = defaultValue;
            Version = version;
            PrereqOf = prereqOf;
            TrackEvents = trackEvents;
            DebugEventsUntilDate = debugEventsUntilDate;
            Debug = debug;
            Reason = reason;
        }
    }

    /// <summary>
    /// An analytics event generated by the <c>Track</c> method.
    /// </summary>
    public class CustomEvent : Event
    {
        /// <summary>
        /// Custom data provided for the event.
        /// </summary>
        [Obsolete("Use JsonData.")]
        public string Data
        {
            get
            {
                return JsonData == null ? null : JsonData.ToString();
            }
        }

        /// <summary>
        /// Custom data provided for the event.
        /// </summary>
        public JToken JsonData { get; private set; }

        /// <summary>
        /// An optional numeric value that can be used in analytics.
        /// </summary>
        public double? MetricValue { get; private set; }

        internal CustomEvent(long creationDate, string key, User user, JToken data, double? metricValue) :
            base(creationDate, key, user)
        {
            JsonData = data;
            MetricValue = metricValue;
        }
    }

    /// <summary>
    /// An analytics event generated by the <c>Identify</c> method.
    /// </summary>
    public class IdentifyEvent : Event
    {
        internal IdentifyEvent(long creationDate, User user) :
            base(creationDate, user == null ? null : user.Key, user)
        {
        }
    }

    /// <summary>
    /// An analytics event generated internally to capture user details from another event.
    /// </summary>
    internal class IndexEvent : Event
    {
        internal IndexEvent(long creationDate, User user) :
            base(creationDate, user.Key, user)
        { }
    }

}