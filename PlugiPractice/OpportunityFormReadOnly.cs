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
                    if (accessLevel == 1)
                    {
                        GetUserOfTeams(service, context.UserId, entity);
                    }
                }
                catch (Exception)
                {

                    throw;
                }
            }
        } 
        private static void GetUserOfTeams(IOrganizationService service, Guid SystemUserid, Entity entity)
        {
            // bool isRole = false;
            QueryExpression team = new QueryExpression("team");
            team.ColumnSet.AddColumns(new string[] { "teamid", "name" });
            EntityCollection entityCollection = service.RetrieveMultiple(team);
            if (entityCollection != null && entityCollection.Entities != null && entityCollection.Entities.Count > 0)
            {
                foreach (Entity item in entityCollection.Entities)
                {
                    Guid teamid = item.Id;
                    string TeamName = item.Contains("name") ? Convert.ToString(item["name"]) : string.Empty;
                    if (TeamName == "Financial")
                    {
                        String FetchXml = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='true'>" +
                                           "<entity name='systemuser'>" +
                                           "<attribute name='fullname' />" +
                                           "<attribute name='businessunitid' />" +
                                           "<attribute name='title' />" +
                                           "<attribute name='address1_telephone1' />" +
                                           "<attribute name='positionid' />" +
                                           "<attribute name='systemuserid' />" +
                                           "<order attribute='fullname' descending='false' />" +
                                           "<filter type='and'>" +
                                           "<condition attribute='systemuserid' operator='not-null' />" +
                                           "</filter>" +
                                          "<link-entity name='teammembership' from='systemuserid'                                          to='systemuserid' visible='false' intersect='true'>" +
                                          "<link-entity name='team' from='teamid' to='teamid' alias='ab'>" +
                                          "<filter type='and'>" +
                                          "<condition attribute='teamid' operator='eq' uiname='Finance'                                  uitype='team' value='" + teamid + "' />" +
                                          "</filter>" +
                                          "</link-entity>" +
                                          "</link-entity>" +
                                          "</entity>" +
                                          "</fetch>";
                        EntityCollection UserCollection = service.RetrieveMultiple(new FetchExpression(FetchXml));
                        if (UserCollection != null && UserCollection.Entities != null && UserCollection.Entities.Count > 0)
                        {
                            foreach (var user in UserCollection.Entities)
                            {
                                string fullname = user.Contains("fullname") ? Convert.ToString(user.Attributes["fullname"]) : string.Empty;
                                if (fullname != null)
                                {
                                          
                                               
                                }
                            }
                        }
                    }
                }
            }
        }
     
    }
}
