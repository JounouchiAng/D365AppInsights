﻿using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Workflow;
using System;
using System.Activities;
using System.Collections.Generic;

namespace AppInsightsWorkflowLogger
{
    public sealed class LogMetric : WorkFlowActivityBase
    {
        [RequiredArgument]
        [Input("Name")]
        public InArgument<string> Name { get; set; }

        [RequiredArgument]
        [Input("Kind (Valid values: 0 Measurement or 1 Aggregation)")]
        public InArgument<int?> Kind { get; set; }

        [RequiredArgument]
        [Input("Metric Value")]
        public InArgument<int?> MetricValue { get; set; }

        [Input("Count")]
        public InArgument<int?> Count { get; set; }

        [Input("Min")]
        public InArgument<int?> Min { get; set; }

        [Input("Max")]
        public InArgument<int?> Max { get; set; }

        [Output("Log Success")]
        public OutArgument<bool> LogSuccess { get; set; }

        public LogMetric() : base(typeof(LogMetric)) { }
        protected override void ExecuteCrmWorkFlowActivity(CodeActivityContext context, LocalWorkflowContext localContext)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));
            if (localContext == null)
                throw new ArgumentNullException(nameof(localContext));

            string name = Name.Get(context);
            int? kind = Kind.Get(context);
            int? metricValue = MetricValue.Get(context);
            int? count = Count.Get(context);
            int? min = Min.Get(context);
            int? max = Max.Get(context);

            OrganizationRequest request = new OrganizationRequest
            {
                RequestName = "lat_ApplicationInsightsLogMetric",
                Parameters = new ParameterCollection
                {
                    new KeyValuePair<string, object>("name", name),
                    new KeyValuePair<string, object>("kind", kind),
                    new KeyValuePair<string, object>("metricValue", metricValue),
                    new KeyValuePair<string, object>("count", count),
                    new KeyValuePair<string, object>("min", min),
                    new KeyValuePair<string, object>("max", max)
                }
            };

            OrganizationResponse response = localContext.OrganizationService.Execute(request);

            bool hasLogSuccess = response.Results.TryGetValue("logsuccess", out object objLogSuccess);
            if (hasLogSuccess)
            {
                LogSuccess.Set(context, (bool)objLogSuccess);
                return;
            }

            LogSuccess.Set(context, false);
        }
    }
}