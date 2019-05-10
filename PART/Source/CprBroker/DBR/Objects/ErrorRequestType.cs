using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CprBroker.DBR
{
    public class ErrorRequestType : NewRequestType
    {
        public ErrorRequestType(string contents)
            : base(contents)
        {

        }

        public override DiversionResponseType Process(string dprConnectionString, bool skipAddressIfDead = false)
        {
            return Process(dprConnectionString, skipAddressIfDead, false);
        }

        public DiversionResponseType Process(string dprConnectionString, bool skipAddressIfDead, bool nullOnUnknownError)
        {
            var myContents = Contents.Trim();

            DiversionResponseType ret = null;
            Func<string, ClassicResponseType> classicResponseFromContents = (string s) =>
            {
                var r = new ClassicResponseType();
                r.Contents = s.PadRight(r.Length);
                return r;
            };

            if (myContents.Length < 12)
            {
                ret = classicResponseFromContents(
                    string.Join("", myContents.Take(2))
                    + "99"
                    + string.Join("", myContents.Skip(2))
                    + "Fejl i kaldet - færre end 12 tegn"
                    );
            }
            else if (!Regex.Match(myContents, @"\A[0-9]{12}").Success)
            {
                ret = classicResponseFromContents(
                    string.Join("", myContents.Take(2))
                    + "99"
                    + string.Join("", myContents.Skip(2))
                    + "Fejl i kaldet - første 12 tegn ikke numeriske."
                    );
            }
            else if (!PartInterface.Strings.IsValidPersonNumber(this.PNR))
            {
                ret = new ClassicResponseType()
                {
                    Type = this.Type,
                    LargeData = this.LargeData,
                    ErrorNumber = "40",
                    PNR = this.PNR,
                    Data = " Personnummer forkert opbygget. Kald afvist."
                };

            }
            else if (!Regex.Match(myContents, @"\A[013]").Success)
            {
                ret = new ClassicResponseType()
                {
                    Type = this.Type,
                    LargeData = this.LargeData,
                    ErrorNumber = "10",
                    PNR = this.PNR,
                    Data = " ABON_TYPE ukendt"
                };
            }
            else if (!Regex.Match(myContents, @"\A[013][01]").Success)
            {
                ret = classicResponseFromContents("Tom Codepath i sSvarGammelType. Stordata = " + (int)this.LargeData);
            }
            else if (!nullOnUnknownError)
            {
                // This case should not be reached if the request has no errors
                return new ClassicResponseType()
                {
                    Type = this.Type,
                    LargeData = this.LargeData,
                    PNR = this.PNR,
                    ErrorNumber = "31",
                    Data = "Kommunikationsfejl"
                };
            }

            return ret;
        }
    }
}
