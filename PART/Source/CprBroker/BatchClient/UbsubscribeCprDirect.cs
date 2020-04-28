using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CprBroker.Utilities.ConsoleApps;
using CprBroker.Providers.CPRDirect;
using System.Net.Sockets;
using System.IO;
using CprBroker.Engine.Local;
using CprBroker.Utilities;
using CprBroker.Engine;
using CprBroker.PartInterface;
using CprBroker.Schemas.Part;

namespace BatchClient
{
    class UbsubscribeCprDirect : ConsoleEnvironment
    {
        public override void Initialize()
        {

            base.Initialize();
            CprBroker.Tests.PartInterface.Utilities.UpdateConnectionString(this.BrokerConnectionString);
            CprBroker.Engine.BrokerContext.Initialize(this.ApplicationToken, "");
        }

        public override string[] LoadCprNumbers()
        {
            return BatchClient.Utilities.LoadCprNumbersOneByOne(SourceFile);
        }

        public override void ProcessPerson(string pnr)
        {
            string error;
            CprBroker.Engine.BrokerContext.Initialize(this.ApplicationToken, "");
            NetworkStream stream = null;
            var prov = new CPRDirectClientDataProvider()
            {
                ConfigurationProperties = new Dictionary<string, string>(),
                Address = "localhost",
                Port = 700,
            };

            using (var callContext = prov.BeginCall("Unsubscribe", pnr))
            {
                try
                {
                    IndividualRequestType request = new IndividualRequestType(false, DataType.NoData, decimal.Parse(pnr));

                    var req = new IndividualRequestType(false, DataType.NoData, decimal.Parse(pnr));

                    TcpClient client = new TcpClient(prov.Address, prov.Port);
                    Byte[] data = CprBroker.Providers.CPRDirect.Constants.TcpClientEncoding.GetBytes(req.Contents);

                    stream = client.GetStream();
                    stream.Write(data, 0, data.Length);

                    data = new Byte[CprBroker.Providers.CPRDirect.Constants.ResponseLengths.MaxResponseLength];

                    int bytes = stream.Read(data, 0, data.Length);
                    var response = CprBroker.Providers.CPRDirect.Constants.TcpClientEncoding.GetString(data, 0, bytes);
                    Log(response);
                    string errorCode = response.Substring(CprBroker.Providers.CPRDirect.Constants.ResponseLengths.ErrorCodeIndex, CprBroker.Providers.CPRDirect.Constants.ResponseLengths.ErrorCodeLength);

                    PartManager pm = new PartManager();
                    BrokerContext bc = BrokerContext.Current;
                    GetUuidOutputType UUIDOutput = pm.GetUuid(bc.UserToken.ToString(), bc.ApplicationToken.ToString(), pnr);
                    string cprBrokerUUID = UUIDOutput.UUID;

                    Admin.LogFormattedSuccess("CPR client: UUID <{0}>, status code <{1}>", cprBrokerUUID, errorCode);

                    if (CprBroker.Providers.CPRDirect.Constants.ErrorCodes.ContainsKey(errorCode))
                    {
                        error = CprBroker.Providers.CPRDirect.Constants.ErrorCodes[errorCode];
                        // We log the call and set the success parameter to false
                        callContext.Fail();
                        throw new Exception("Failed");
                    }
                    else
                    {
                        // We log the call and set the success parameter to true
                        callContext.Succeed();
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }
    }
}
