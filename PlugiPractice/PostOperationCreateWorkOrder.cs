using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlugiPractice
{
    public class PostOperationCreateWorkOrder : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {

            IPluginExecutionContext context = serviceProvider.GetService(typeof(IPluginExecutionContext)) as IPluginExecutionContext;
            IOrganizationServiceFactory organizationService = serviceProvider.GetService(typeof(IOrganizationServiceFactory)) as IOrganizationServiceFactory;
            IOrganizationService service = organizationService.CreateOrganizationService(context.UserId);
            if (context.InputParameters.Contains("Target") && context.InputParameters["Target"] is Entity)
            {
                Entity entity = (Entity)context.InputParameters["Target"];
                try
                {
                    EntityReference opportunity = (EntityReference)entity.Attributes["sknd_opportunity"];

                    Entity entity1 = service.Retrieve("opportunity", opportunity.Id, new ColumnSet(new string[] { "sknd_opportunityvalue", "sknd_totalworkordervalue", "sknd_remainingvalue" }));   // service.Retrieve()

                    int opportunityvalu = entity1.Contains("sknd_opportunityvalue") ? Convert.ToInt32(entity1["sknd_opportunityvalue"]) : 0;
                   int totalworkorder = entity1.Contains("sknd_totalworkordervalue") ? Convert.ToInt32(entity1["sknd_totalworkordervalue"]) : 0;
                    int remainingvalue = entity1.Contains("sknd_remainingvalue") ? Convert.ToInt32(entity1["sknd_remainingvalue"]) : 0;
                   
                    //Declaring Variables Which Contains Rollup Field Schema Name, CRM Service and Parent Entity Entity Refrence ID.
                    string FieldName = "sknd_totalworkordervalue";
                    var calcularRollup = new CalculateRollupFieldRequest
                    {
                        Target = new EntityReference("opportunity", opportunity.Id),
                        FieldName = FieldName
                    };

                    var calcularRollupResult = ((CalculateRollupFieldResponse)service.Execute(calcularRollup));
                    Entity classRecord = calcularRollupResult.Entity;
                    int totalworkordervalue = classRecord.GetAttributeValue<int>("sknd_totalworkordervalue");

                    if (totalworkordervalue > opportunityvalu)
                    {
                        throw new InvalidPluginExecutionException("You can not Create Work order Record");
                    }
                }
                catch (Exception)
                {

                    throw;
                }
            }
        }
    }
}
