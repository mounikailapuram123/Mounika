using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlugiPractice
{
    public class PostOperationAccountConfiguration : IPlugin
    {
        private readonly string _unsecureString;
        private readonly string _secureString;

        public PostOperationAccountConfiguration(string unsecureString, string secureString)
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
                    contact.Id = entity.Id;
                    contact["telephone1"] = _unsecureString;
                    contact["emailaddress1"] = _secureString;
                    service.Update(contact);
                }
                catch (Exception ex)
                {
                    throw new InvalidPluginExecutionException(ex.Message);
                }
            }



        }
    }
}
