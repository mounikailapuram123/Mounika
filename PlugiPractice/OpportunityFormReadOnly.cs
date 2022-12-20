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
    public class OpportunityFormReadOnly : IPlugin
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

                    int accessLevel = ((OptionSetValue)entity["sknd_acceslevel"]).Value;
                    string managemnt = "08BDE210-877F-ED11-81AD-000D3A5656E9";
                    string operationnal = "D8283DB1-867F-ED11-81AD-000D3A5656E9";
                    string financial = "662E5CE1-867F-ED11-81AD-000D3A5656E9";

                    if (accessLevel == 1)
                    {
                        GetUserOfTeams(service, financial, entity.Id);
                    }
                    else if (accessLevel == 2)
                    {
                        GetUserOfTeams(service, managemnt, entity.Id);
                    }
                    else if (accessLevel == 3)
                    {
                        GetUserOfTeams(service, operationnal, entity.Id);
                    }
                }
                catch (Exception)
                {

                    throw;
                }
            }

            // throw new NotImplementedException();
        }
        private static void GetUserOfTeams(IOrganizationService service, string team, Guid recordaID)
        {
            string fetch = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='true'>" +
                                            "<entity name='systemuser'>" +
                                              "<attribute name='fullname' />" +
                                              "<attribute name='businessunitid' />" +
                                              "<attribute name='title' />" +
                                              "<attribute name='address1_telephone1' />" +
                                              "<attribute name='positionid' />" +
                                              "<attribute name='systemuserid' />" +
                                              "<order attribute='fullname' descending='false' />" +
                                              "<filter type='and'>" +
                                                "<condition attribute='domainname' operator='not-null' />" +
                                              "</filter>" +
                                              "<link-entity name='teammembership' from='systemuserid' to='systemuserid' visible='false' intersect='true'>" +
                                                "<link-entity name='team' from='teamid' to='teamid' alias='ag'>" +
                                                  "<filter type='and'>" +
                                                    "<condition attribute='teamid' operator='eq' uiname='' uitype='team' value='" + team + "' />" +
                                                  "</filter>" +
                                                "</link-entity>" +
                                              "</link-entity>" +
                                            "</entity>" +
                                          "</fetch>";
            EntityCollection entityCollection = service.RetrieveMultiple(new FetchExpression(fetch));
            if (entityCollection != null && entityCollection.Entities != null && entityCollection.Entities.Count > 0)
            {

                makeFormReadOnly(service, recordaID);
            }
        }
        private static void makeFormReadOnly(IOrganizationService service, Guid recordId)
        {
            Entity opportunity = new Entity("opportunity");
            opportunity["statuscode"] = new OptionSetValue(2);
            opportunity.Id = recordId;
            service.Update(opportunity);
        }

    }
}
