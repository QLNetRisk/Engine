using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QLNet;

namespace QLRData
{
    public class PeriodParser
    {
        public static Period Parse(string s)
        {
            Utils.QL_REQUIRE(s.Length > 1, () => "period string length must be at least 2");

            List<string> subStrings = new List<string>();
            string reducedString = s;

            int iPos, reducedStringDim = 100000, max_iter = 0;
            while (reducedStringDim > 0)
            {
                iPos = reducedString.IndexOfAny(new char[] { 'D', 'd', 'W', 'w', 'M', 'm', 'Y', 'y' } );
                int subStringDim = iPos + 1;
                reducedStringDim = reducedString.Length - subStringDim;
                subStrings.Add(reducedString.Substring(0, subStringDim));
                reducedString = reducedString.Substring(iPos + 1, reducedStringDim);
                ++max_iter;
                Utils.QL_REQUIRE(max_iter < s.Length, () => "unknown '" + s + "' unit");
            }

            Period result = parseOnePeriod(subStrings[0]);
            for (int i = 1; i < subStrings.Count; ++i)
            {
                result += parseOnePeriod(subStrings[i]);
            }

            return result;
        }

        private static Period parseOnePeriod(string s)
        {
            Utils.QL_REQUIRE(s.Length > 1, () => "single period require a string of at least 2 characters");

            int iPos = s.IndexOfAny(new char[] { 'D', 'd', 'W', 'w', 'M', 'm', 'Y', 'y' }); 
            Utils.QL_REQUIRE(iPos == s.Length - 1, () => "unknown '" + s.Substring(s.Length - 1, s.Length) + "' unit");

            TimeUnit units = TimeUnit.Days;
            string abbr = s[iPos].ToString().ToUpper();
            if (abbr == "D") units = TimeUnit.Days;
            else if (abbr == "W") units = TimeUnit.Weeks;
            else if (abbr == "M") units = TimeUnit.Months;
            else if (abbr == "Y") units = TimeUnit.Years;

            int nPos = s.IndexOfAny(new char[] { '+', '-', '1', '2', '3', '4', '5', '6', '7', '8', '9' }); //IndexOf("-+0123456789");
            Utils.QL_REQUIRE(nPos < iPos, () => "no numbers of " + units + " provided");
            int n;
            try
            {
                n = Convert.ToInt32(s.Substring(nPos, iPos));
            }
            catch (Exception ex)
            {
                Utils.QL_FAIL("unable to parse the number of units of " + units + " in '" + s + "'. Error:" + ex.ToString());
                throw;
            }

            return new Period(n, units);
        }
    }

    public class DateParsers
    {
        /// <summary>
        /// Parses a string in a used-defined format.
        /// This method uses the parsing functions from
        /// Boost.Date_Time and supports the same formats.
        /// </summary>
        /// <param name="s"></param>
        /// <param name=""></param>
        /// <returns></returns>
        public static Date ParseFormatted(string s, string fmt)
        {
            return new Date(); // TODO!!
        }
                                   
        public static Date ParseISO(string s)
        {
            return new Date(); // TODO!!
        }
    }
}
