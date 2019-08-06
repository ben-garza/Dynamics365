using System;
using System.Net;
using System.ServiceModel.Description;
using System.ServiceModel.Security;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk.Discovery;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Tooling.Connector;
using Microsoft.Xrm.Sdk.Metadata;

namespace CreateAutoNumber
{
    class Program
    {
        public void Run()
        {
            //[ONPREM] string connectionString = @"AuthType=IFD;Url=https://dyncrmsql:444/CRM;Username=contoso\\Administrator;Password=***!";

            // Custom entity required:
            //   Entity name: new_test
            //   Default field: name
            //   Custom fields:
            //      content (multi-line text)       

            string connectionString = @"AuthType=Office365;Url=https://fiscalnet.crm.dynamics.com;Username=ben99@fiscalnet.mx;Password=Itadow29";
            CrmServiceClient proxy = new CrmServiceClient(connectionString);
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            // Creating AutoNumber attribute
            CreateAttributeRequest testAttributeRequest = new CreateAttributeRequest
            {
                EntityName = "new_test",
                Attribute = new StringAttributeMetadata
                {
                    //Define the format of the attribute. Main documentation is here: https://docs.microsoft.com/en-us/dynamics365/customer-engagement/developer/create-auto-number-attributes
                    AutoNumberFormat = "TST2-{SEQNUM:5}",
                    LogicalName = "new_serialnumber2",
                    SchemaName = "new_SerialNumber2",
                    RequiredLevel = new AttributeRequiredLevelManagedProperty(AttributeRequiredLevel.None),
                    MaxLength = 100, // The MaxLength defined for the string attribute must be greater than the length of the AutoNumberFormat value, that is, it should be able to fit in the generated value.
                    DisplayName = new Label("Serial Number 2", 1033),
                    Description = new Label("Serial Number of the test 2.", 1033)
                }
            };
            proxy.Execute(testAttributeRequest);
            Console.Out.WriteLine("AutoNumber Attribute has been created...");

            //Creating a new record
            Random r = new Random();
            Entity e = new Entity("new_test");
            e["new_name"] = "New record " + String.Format("{0:00000}", r.Next(1000, 10000));
            e["new_content"] = "All we need is less...";

            Guid newRecordId = proxy.Create(e);  // When creating the record, the new attribute is auto-populated following the specified pattern
            Console.Out.WriteLine(String.Format("New entity {0} record has been created...", newRecordId.ToString()));

            // Retrieving the just created record using the Guid returned by the create method
            Entity e2 = proxy.Retrieve("new_test", newRecordId, new ColumnSet(true));
            Console.Out.WriteLine("Retrieving new record...");
            Console.Out.WriteLine(String.Format("Name = {0}", e2["new_name"].ToString()));
            Console.Out.WriteLine(String.Format("content = {0}", e2["new_content"].ToString()));
            Console.Out.WriteLine(String.Format("AutoNumber = {0}", e2["new_serialnumber2"].ToString()));

            Console.Out.WriteLine("Press any key to end...");
            Console.ReadKey();
        }

        static void Main(string[] args)
        {
            var c = new Program();
            c.Run();
        }
    }
}
