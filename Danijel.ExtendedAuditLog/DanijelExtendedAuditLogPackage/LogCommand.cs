using Danijel.ExtendedAuditLog.DanijelExtendedAuditLogPackage.Extensions;
using Danijel.ExtendedAuditLog.DanijelExtendedAuditLogPackage.Models;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI.WebControls;

namespace Danijel.ExtendedAuditLog.DanijelExtendedAuditLogPackage
{
    public class LogCommand : PluginBase
    {
        private readonly TelemetryClient _telemetryClient;
        private readonly TelemetryConfiguration _telemetryConfiguration;
        public LogCommand(string unsecure, string secure)
            : base(typeof(LogCommand))
        {

            _telemetryConfiguration = TelemetryConfiguration.CreateDefault();
            _telemetryConfiguration.ConnectionString = secure;
            _telemetryClient = new TelemetryClient(_telemetryConfiguration);
            
        }

        protected override void ExecuteCdsPlugin(ILocalPluginContext localContext)
        {
            if (localContext == null)
            {
                throw new InvalidPluginExecutionException(nameof(localContext));
            }
            // Obtain the tracing service
            ITracingService tracingService = localContext.TracingService;

            try
            {

                // Obtain the execution context from the service provider.  
                IPluginExecutionContext context = (IPluginExecutionContext)localContext.PluginExecutionContext;

                if (!context.InputParameters.Contains("Target") || context.InputParameters["Target"] == null)
                {
                    return;
                }

                // Obtain the organization service reference for web service calls.  
                IOrganizationService currentUserService = localContext.CurrentUserService;

                Audit data = null;

                if (context.InputParameters["Target"] is Entity)
                {
                    var target = context.InputParameters["Target"] as Entity;

                    data = new Audit()
                    {
                        UserId = context.UserId.ToString(),
                        CreationTime = DateTime.Now,
                        EntityId = target.Id.ToString(),
                        EntityName = target.LogicalName,
                        Message = context.MessageName
                    };

                }
                else if (context.InputParameters["Target"] is EntityReference)
                {
                    var target = context.InputParameters["Target"] as EntityReference;

                    data = new Audit()
                    {
                        UserId = context.UserId.ToString(),
                        CreationTime = DateTime.Now,
                        EntityId = target.Id.ToString(),
                        EntityName = target.LogicalName,
                        Message = context.MessageName
                    };
                }

                _telemetryClient.Context.Operation.Id = Guid.NewGuid().ToString();

                var evt = new EventTelemetry($"Dynamics CRM");
                var properties = Helper.ToDictionary<string>(data);
                _telemetryClient.TrackEvent(evt.Name, properties);


            }
            // Only throw an InvalidPluginExecutionException. Please Refer https://go.microsoft.com/fwlink/?linkid=2153829.
            catch (Exception ex)
            {
                tracingService?.Trace("An error occurred executing Plugin PowerPlatformVSSolution1.Plugins.PostOperationaccountCreate : {0}", ex.ToString());
                throw new InvalidPluginExecutionException("An error occurred executing Plugin PowerPlatformVSSolution1.Plugins.PostOperationaccountCreate .", ex);
            }
        }
    }
}
