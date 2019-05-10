using CprBroker.DBR;
using CprBroker.Providers.CPRDirect;
using CprBroker.Providers.DPR;
using CprBroker.Schemas.Wrappers;
using CprBroker.Tests.DBR.ComparisonResults;
using CprBroker.Tests.DBR.Properties;
using CprBroker.Utilities;

using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using CprBroker.Tests.DBR.ComparisonResults;

namespace CprBroker.Tests.DBR.DiversionComparison
{
    public class DiversionComparisonTest
    {
        static DiversionComparisonTest()
        {
            if (!Directory.Exists(Settings.Default.DprDiversionCacheDirectory))
                Directory.CreateDirectory(Settings.Default.DprDiversionCacheDirectory);

            PartInterface.Utilities.UpdateConnectionString(Settings.Default.CprBrokerConnectionString);
        }

        [TestFixtureSetUp]
        public void InitBrokerContext()
        {
            CprBroker.Engine.BrokerContext.Initialize(CprBroker.Utilities.Constants.BaseApplicationToken.ToString(), "");
        }

        public string GetRealResponse(string request)
        {
            var fileName = string.Format(@"{0}Response.{1}.txt",
                Settings.Default.DprDiversionCacheDirectory,
                request);
            if (File.Exists(fileName))
            {
                return File.ReadAllText(fileName);
            }
            else
            {
                Console.WriteLine("{0}:\"{1}\"", nameof(request), request);

                var inputData = Providers.DPR.Constants.DiversionEncoding.GetBytes(request);

                using (TcpClient client = new TcpClient(Settings.Default.RealDprDiversionAddress, Settings.Default.RealDprDiversionPort))
                {
                    using (NetworkStream stream = client.GetStream())
                    {
                        stream.Write(inputData, 0, inputData.Length);
                        stream.ReadTimeout = 1000;

                        var retData = new Byte[3500];
                        var retBytes = stream.Read(retData, 0, retData.Length);
                        var retText = Providers.DPR.Constants.DiversionEncoding.GetString(retData.Take(retBytes).ToArray());
                        Console.WriteLine("Response:");
                        Console.WriteLine(retText);

                        File.WriteAllText(fileName, retText);
                        return retText;
                    }
                }
            }
        }

        public DiversionRequest ParseRequest(string request)
        {
            var ret = DiversionRequest.Parse(request);

            if (ret is ErrorRequestType)
            {
                //ret = ret;
            }
            else if (ret is ClassicRequestType)
            {
                ret = new ClassicRequestTypeStub(ret.Contents);
            }
            else if (ret is NewRequestType)
            {
                ret = new NewRequestTypeStub(ret.Contents);
            }
            else
            {
                throw new Exception("Unknown request type created");
            }

            return ret;
        }

        public string GetEmulatedResponse(string request, string dprConnectionString)
        {
            var req = ParseRequest(request);
            var resp = req.Process(dprConnectionString, false);
            return resp.ToString();
        }

        public void Compare(string request, Func<string, string> preprocessor = null)
        {
            if (preprocessor == null)
            {
                preprocessor = (s) => s;
            }


            var realResponse = preprocessor(GetRealResponse(request).Trim());
            var emulatedResponse = preprocessor(GetEmulatedResponse(request, Settings.Default.ImitatedDprConnectionString).Trim());
            Assert.AreEqual(realResponse, emulatedResponse);
        }

        #region CPR Numbers
        public string[] GetCprNumbers(int count)
        {
            var minPnr = decimal.Parse("0103000000");
            using (var context = new DPRDataContext(Settings.Default.RealDprConnectionString))
            {
                return context.ExecuteQuery<decimal>(""
                    + "SELECT t.PNR FROM DTTOTAL t "
                    + "INNER JOIN DTPERS p ON p.PNR=t.PNR "
                    + "WHERE t.PNR > {0} "
                    + "AND (INDLAESDTO IS NULL OR INDLAESDTO < {1}) "
                    + "AND (HENTTYP IS NULL OR HENTTYP IN ({2},{3})) "
                    + "AND (INDLAESPGM IS NULL OR INDLAESPGM = {4}) "
                    + "AND PERAJDTO > {5} "
                    + "ORDER BY t.PNR "
                    ,
                    minPnr,
                    new DateTime(2016, 10, 1),
                    DataRetrievalTypes.Extract, DataRetrievalTypes.Extract2,
                    UpdatingProgram.DprUpdate,
                     199701010000 // 1 Jan 1997

                    )
                    .Take(count)
                    .ToArray()
                    .Select(p => p.ToPnrDecimalString())
                    .ToArray();
            }
        }

        public string[] CprNumbers2 { get { return GetCprNumbers(2); } }
        public string[] CprNumbers5 { get { return GetCprNumbers(5); } }
        public string[] CprNumbers10 { get { return GetCprNumbers(10); } }
        public string[] CprNumbers100 { get { return GetCprNumbers(100); } }
        public string[] CprNumbers400 { get { return GetCprNumbers(400); } }
        #endregion
    }

    [TestFixture]
    public class ClassicRequestComparisonTest : DiversionComparisonTest
    {
        [Test]
        public void InvalidRequest_Old12Char(
            [Values("44", "46", "")]string start,
            [ValueSource(nameof(CprNumbers2))]string pnr)
        {
            var request = start + pnr;
            Compare(request);
        }

        [Test]
        public void NonNumericRequest_Old12Char(
            [Values("aa", "bb")]string start,
            [ValueSource(nameof(CprNumbers2))]string pnr)
        {
            var request = start + pnr;
            Compare(request);
        }

        [Test]
        public void RealRequest_Old12Char(
            [Values("1")]string type,
            [Values("0")]string largeData,
            [ValueSource(nameof(CprNumbers5))]string pnr)
        {
            var request = type + largeData + pnr;
            Compare(request);
        }
    }

    public abstract class NewRequestComparisonTest : DiversionComparisonTest, IComparisonType
    {
        public virtual PropertyComparisonResult[] ExcludedPropertiesInformation
        {
            get
            {
                return new PropertyComparisonResult[] {
                    new PropertyComparisonResult("AJF","", ExclusionStatus.CopiedFromNonMatching),
                    new PropertyComparisonResult("MYNKOD","", ExclusionStatus.CopiedFromNonMatching),
                    new PropertyComparisonResult("ADRNVN","", ExclusionStatus.CopiedFromNonMatching),
                    new PropertyComparisonResult("INDRAP","", ExclusionStatus.LocalUpdateRelated),
                    new PropertyComparisonResult("FOEDMYNHAENSTART","", ExclusionStatus.CopiedFromNonMatching),
                    new PropertyComparisonResult("KUNDENR","", ExclusionStatus.LocalUpdateRelated),
                    new PropertyComparisonResult("FARSKABHAENSTART","", ExclusionStatus.CopiedFromNonMatching),
                    new PropertyComparisonResult("AEGTEMRK","", ExclusionStatus.CopiedFromNonMatching),
                    new PropertyComparisonResult("AEGTEDOK","", ExclusionStatus.CopiedFromNonMatching),
                    new PropertyComparisonResult("FARSKABMYNNVN","", ExclusionStatus.CopiedFromNonMatching),
                    new PropertyComparisonResult("TIDLKOMNVN","", ExclusionStatus.CopiedFromNonMatching),
                    new PropertyComparisonResult("CIVMYN","", ExclusionStatus.CopiedFromNonMatching),
                    new PropertyComparisonResult("STILLINGDTO","", ExclusionStatus.CopiedFromNonMatching),

                    new PropertyComparisonResult("MORDOK","", ExclusionStatus.CopiedFromNonMatching),
                    new PropertyComparisonResult("FARDOK","", ExclusionStatus.CopiedFromNonMatching),
                    new PropertyComparisonResult("MORMRK","", ExclusionStatus.CopiedFromNonMatching),
                    new PropertyComparisonResult("FARMRK","", ExclusionStatus.CopiedFromNonMatching),
                    new PropertyComparisonResult("UDLANDADRDTO","", ExclusionStatus.CopiedFromNonMatching),
                    new PropertyComparisonResult("PNRMRKHAENSTART","", ExclusionStatus.LocalUpdateRelated),
                    new PropertyComparisonResult("TIDLPNRMRK", "Fails sometimes because HistoricalPNR's can be older than the 20-year limit for extracts, so they do not appear in the emulated database", ExclusionStatus.InsufficientHistory),
                    new PropertyComparisonResult("KONTAKTADR_KOMKOD","", ExclusionStatus.CopiedFromNonMatching),
                    new PropertyComparisonResult("STARTDATE_FORALD_46", "real DPR returns yyyy-MM-dd HH:mm:ss or dd-MM-yyyy randomly", ExclusionStatus.InconsistentFormatting),
                    new PropertyComparisonResult("STARTDATE_FORALD_35", "real DPR returns yyyy-MM-dd HH:mm:ss or dd-MM-yyyy randomly", ExclusionStatus.InconsistentFormatting),
                    new PropertyComparisonResult("UDRINDRMRK", "Sometimes not set because departures are older than 20 years (so not included in the initial dataset)", ExclusionStatus.InsufficientHistory),
                    new PropertyComparisonResult("FRAFLYKOMKOD", "Sometimes not set because departures are older than 20 years (so not included in the initial dataset)", ExclusionStatus.InsufficientHistory),
                    new PropertyComparisonResult("TILFLYDTOMRK","", ExclusionStatus.CopiedFromNonMatching),
                    new PropertyComparisonResult("MYNTXT_CIV","", ExclusionStatus.CopiedFromNonMatching),
                    new PropertyComparisonResult("PNR_BORN", "Tested - excluded because ordering of children from DPR is inconsistent and causes test failure", ExclusionStatus.InconsistentFormatting),


                    new PropertyComparisonResult("POSTDISTTXT", "", ExclusionStatus.Dead),
                    new PropertyComparisonResult("POSTDISTRICT","", ExclusionStatus.Dead),
                    new PropertyComparisonResult("POSTNR","", ExclusionStatus.Dead),
                    new PropertyComparisonResult("POSTCODE","", ExclusionStatus.Dead),
                    new PropertyComparisonResult("BYNVN","", ExclusionStatus.Dead),
                    new PropertyComparisonResult("STANDARDADR","", ExclusionStatus.Dead),
                    new PropertyComparisonResult("KOMKOD","", ExclusionStatus.Dead),
                    new PropertyComparisonResult("KOMNVN","", ExclusionStatus.Dead),
                    new PropertyComparisonResult("AKTKOMNVN","", ExclusionStatus.Dead),
                    new PropertyComparisonResult("VEJKOD","", ExclusionStatus.Dead),
                    new PropertyComparisonResult("KOMKOD","", ExclusionStatus.Dead),
                    new PropertyComparisonResult("VEJADRNVN","", ExclusionStatus.Dead),
                    new PropertyComparisonResult("STREETNAME","", ExclusionStatus.Dead),
                    new PropertyComparisonResult("HUSNR","", ExclusionStatus.Dead),
                    new PropertyComparisonResult("HOUSENUMBER","", ExclusionStatus.Dead),
                    new PropertyComparisonResult("ETAGE","", ExclusionStatus.Dead),
                    new PropertyComparisonResult("FLOOR","", ExclusionStatus.Dead),
                    new PropertyComparisonResult("SIDEDOER","", ExclusionStatus.Dead),
                    new PropertyComparisonResult("DOOR","", ExclusionStatus.Dead),
                    new PropertyComparisonResult("CONVN","", ExclusionStatus.Dead),
                    new PropertyComparisonResult("CAREOFNAME","", ExclusionStatus.Dead),
                    new PropertyComparisonResult("LOKALITET","", ExclusionStatus.Dead),
                    new PropertyComparisonResult("TILFLYDTO","", ExclusionStatus.Dead),
                    new PropertyComparisonResult("TILFLYKOMDTO","", ExclusionStatus.Dead),
                    new PropertyComparisonResult("FRAFLYKOMDTO","", ExclusionStatus.Dead),
                    new PropertyComparisonResult("FRAFLYKOMKOD","", ExclusionStatus.Dead),
                };
            }
        }

        public virtual Type TargetType
        {
            get
            {
                return null;
            }
        }

        public virtual PropertyInfo[] DataProperties()
        {
            var o = Reflection.CreateInstance(TargetType) as Wrapper;
            return o.PropertyDefinitions.Select(p => TargetType.GetProperty(p.Item1)).ToArray();
        }

        public string SourceName
        {
            get { return TargetType.Name; }
        }

        public void CompareNewRequest(
            char type,
            char largeData,
            string pnr,
            char responseData,
            Func<string, string> preprocessor = null)
        {
            var request = ""
                + type
                + largeData
                + pnr
                + "MMXIII"
                + "0" // Do not force diversion
                + responseData
                + "" // no user
                ;
            Compare(request, preprocessor);
        }

        public string Preprocess<T>(string s) where T : Wrapper, new()
        {
            var propertyDefinitions = new T().PropertyDefinitions;

            var values = s.Substring(7).Split(new char[] { ';' }, propertyDefinitions.Length);

            var valuesAndProps = propertyDefinitions
                .Zip(values, (p, v) => new { Prop = p.Item1.ToUpper(), Pos = p.Item2, Len = p.Item3, Value = v });

            var status = decimal.Parse(valuesAndProps.SingleOrDefault(p => p.Prop == "STATUS")?.Value.TrimEnd(';'));

            var excludedProps = PropertyComparisonResult.ExcludedAlways(ExcludedPropertiesInformation).Select(p => p.PropertyName).ToArray();

            var excluded90 = PropertyComparisonResult.OfStatus(ExcludedPropertiesInformation, ExclusionStatus.Dead).Select(p => p.PropertyName).ToArray();

            var newValues = valuesAndProps
                .Select(p =>
                {
                    var name = p.Prop;
                    var value = p.Value;

                    if (Regex.Match(value, @"\A[0-9]{12}\Z").Success) // 12-digit date
                        value = value.Substring(0, 8).PadRight(12, '0');// Set hour and minute to 0


                    if (Regex.Match(value, @"\d{4}-\d{2}-\d{2}-\d{2}\.\d{2}.\d{2}\.\d{6}").Success)
                        value = value.Substring(0, 17) + "00.000000";

                    //if (Regex.Match(value, @"\A0[0-9]*\Z").Success)
                    value = value.TrimStart('0');

                    value = value.TrimStart();

                    if (value.Equals("0")) //STATUSHAENSTART is null in the database but emulated data has 0 as value.
                        value = "";

                    if (excludedProps.FirstOrDefault(pName => name.Contains(pName)) != null)
                    {
                        value = "";
                    }

                    if (
                        (name.Contains("START") || name.Contains("DTO"))
                        && value.Length >= 12 && value.EndsWith("99")
                    )
                    {
                        value = value.Substring(0, value.Length - 2) + "00";
                    }

                    if (
                        status == 90 || status == 80
                        || status == 20
                        || status == 70
                    )
                    {
                        if (excluded90.Contains(name))
                        {
                            value = "";
                        }
                    }

                    if (name == "PNR_BORN") // Never reachd, excluded above
                    {
                        var childValues = value.Split(';');
                        for (int iDok = 0; iDok < childValues.Length; iDok++)
                        {
                            if (
                                iDok % 3 == 0 // AJFDTO
                                || iDok % 3 == 2 // DOK
                            )
                                childValues[iDok] = "";
                        }
                        value = string.Join(";", childValues);
                    }

                    return string.Format("{0}={1}",
                        p.Prop,
                        value);
                }).ToArray();

            var sRet = s.Substring(0, 7) + string.Join(";", newValues);
            return sRet;
        }
    }

    [TestFixture]
    public class NewRequestComparisonTest_Ingen : NewRequestComparisonTest
    {
        public override Type TargetType
        {
            get
            {
                return typeof(NewResponseNoDataType);
            }
        }

        [Test]
        public void RealRequest_New40Char_Ingen(
            [Values('1')]char type,
            [Values('0')]char largeData,
            [ValueSource(nameof(CprNumbers5))]string pnr)
        {
            CompareNewRequest(type, largeData, pnr, 'I');
        }
    }

    [TestFixture]
    public class NewRequestComparisonTest_Stam : NewRequestComparisonTest
    {
        public override Type TargetType
        {
            get
            {
                return typeof(NewResponseBasicDataType);
            }
        }

        [Test]
        public void RealRequest_New40Char_Stam(
            [Values('1')]char type,
            [Values('0')]char largeData,
            [ValueSource(nameof(CprNumbers100))]string pnr)
        {
            CompareNewRequest(type, largeData, pnr, 'S', Preprocess<NewResponseBasicDataType>);
        }
    }

    [TestFixture]
    public class NewRequestComparisonTest_Udvidet : NewRequestComparisonTest
    {
        public override Type TargetType
        {
            get
            {
                return typeof(NewResponseFullDataType);
            }
        }

        [Test]
        public void RealRequest_New40Char_Udvidet(
            [Values('1')]char type,
            [Values('0')]char largeData,
            [ValueSource(nameof(CprNumbers400))]string pnr)
        {
            CompareNewRequest(type, largeData, pnr, 'U', Preprocess<NewResponseFullDataType>);
        }
    }
}
