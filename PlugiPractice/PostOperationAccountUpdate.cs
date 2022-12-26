using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlugiPractice
{
    public class PostOperationAccountUpdate : IPlugin
    {

        private readonly string _unsecureString;
        private readonly string _secureString;

        public PostOperationAccountUpdate(string unsecureString, string secureString)
        {
            if (String.IsNullOrWhiteSpace(unsecureString) &&
                String.IsNullOrWhiteSpace(secureString))
            {
                throw new InvalidPluginExecutionException("Unsecure and secure strings are required for this plugin to execute.");
            }

            _unsecureString = unsecureString;
            _secureString = secureString;
        }


        public void Execute(IServiceProvider serviceProvider)
        {
            IPluginExecutionContext context = serviceProvider.GetService(typeof(IPluginExecutionContext)) as IPluginExecutionContext;
            IOrganizationServiceFactory organizationServiceFactory = serviceProvider.GetService(typeof(IOrganizationServiceFactory)) as IOrganizationServiceFactory;
            IOrganizationService service = organizationServiceFactory.CreateOrganizationService(context.UserId);
            if (context.InputParameters.Contains("Target") && context.InputParameters["Target"] is Entity)
            {
                Entity entity = (Entity)context.InputParameters["Target"];

                try
                {
                    Entity contact = new Entity("contact");
                    contact["parentcustomerid"] = new EntityReference("account", entity.Id);
                    contact["firstname"] = _unsecureString;
                    contact["description"] = _secureString;
                    
                    service.Create(contact);
                }
                catch (Exception)
                {

                    throw;
                }
            }

        }
    }
}
