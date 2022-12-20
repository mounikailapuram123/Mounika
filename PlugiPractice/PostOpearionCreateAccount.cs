

namespace ClassLibrary2
{
    using Microsoft.Xrm.Sdk;
    using System;
    using System.IO;
    using System.Net;
    using System.Runtime.Serialization.Json;
    using System.Text;
    public class PostOpearionCreateAccount : IPlugin
    {
        private readonly string _unsecureString;
        private readonly string _secureString;

        public PostOpearionCreateAccount(string unsecureString, string secureString)
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
            IOrganizationServiceFactory organizationService = serviceProvider.GetService(typeof(IOrganizationServiceFactory)) as IOrganizationServiceFactory;
            IOrganizationService service = organizationService.CreateOrganizationService(context.UserId);
            if (context.InputParameters.Contains("Target") && context.InputParameters["Target"] is Entity)
            {
                Entity entity = (Entity)context.InputParameters["Target"];
                try
                {
                    using (WebClient client = new WebClient())
                    {
                        var account = new AccountDetails();
                        account.FirstName = entity.Attributes["name"].ToString();
                        account.LastName = entity.Attributes["accountnumber"].ToString();
                        DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(AccountDetails));
                        MemoryStream memoryStream = new MemoryStream();
                        serializer.WriteObject(memoryStream, account);
                        var jsonObject = Encoding.Default.GetString(memoryStream.ToArray());
                        var webClient = new WebClient();
                        webClient.Headers[HttpRequestHeader.ContentType] = "application/json";
                        var serviceUrl = _secureString;
                        // upload the data using Post mehtod
                        string response = webClient.UploadString(serviceUrl, jsonObject);
                        Entity acount = new Entity("account");
                        acount.Id = entity.Id;
                        acount["address1_line1"] = response;
                        acount["telephone1"] = _unsecureString;

                        service.Update(acount);
                    }
                }
                catch (Exception ex)
                {
                    throw new InvalidPluginExecutionException(ex.Message);
                }
            }
        }
        public class AccountDetails
        {
            public string FirstName { get; set; }
            public string LastName { get; set; }
        }
    }
}