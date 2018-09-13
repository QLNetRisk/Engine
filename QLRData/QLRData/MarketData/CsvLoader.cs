using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QLNet;

namespace QLRData
{
    public class CsvLoader : Loader
    {
        private bool _implyTodaysFixings;
        private Dictionary<Date, List<MarketDatum>> _data;
        private List<Fixing> _fixings;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="marketFileName"></param>
        /// <param name="fixingFileName"></param>
        /// <param name="implyTodaysFixings"></param>
        public CsvLoader(string marketFilename, string fixingFilename, bool implyTodaysFixings = false) 
        {
            _implyTodaysFixings = implyTodaysFixings;

            // load market data
            LoadFile(marketFilename, true);
            // log
            foreach(KeyValuePair<Date, List<MarketDatum>> kvp in _data)
            {
                //LOG("CSVLoader loaded " + kvp.Value.Count + " market data points for " + kvp.Key);
            }

            // load fixings
            LoadFile(fixingFilename, false);
            //LOG("CSVLoader loaded " + _fixings.Count + " fixings");
            //LOG("CSVLoader complete.");
        }

        private void LoadFile(string filename, bool isMarket)
        {
            //LOG("CSVLoader loading from " << filename);

            //Date today = QuantLib::Settings::instance().evaluationDate();

            //ifstream file;
            //file.open(filename.c_str());
            //QL_REQUIRE(file.is_open(), "error opening file " << filename);

            //while (!file.eof())
            //{
            //    string line;
            //    getline(file, line);
            //    // skip blank and comment lines
            //    if (line.size() > 0 && line[0] != '#')
            //    {

            //        vector<string> tokens;
            //        boost::trim(line);
            //        boost::split(tokens, line, boost::is_any_of(",;\t "), boost::token_compress_on);

            //        // TODO: should we try, catch and log any invalid lines?
            //        QL_REQUIRE(tokens.size() == 3, "Invalid CSVLoader line, 3 tokens expected " << line);
            //        Date date = parseDate(tokens[0]);
            //        const string&key = tokens[1];
            //        Real value = parseReal(tokens[2]);

            //        if (isMarket)
            //        {
            //            // process market
            //            // build market datum and add to map
            //            try
            //            {
            //                data_[date].push_back(parseMarketDatum(date, key, value));
            //                TLOG("Added MarketDatum " << data_[date].back()->name());
            //            }
            //            catch (std::exception&e) {
            //                WLOG("Failed to parse MarketDatum " << key << ": " << e.what());
            //            }
            //            } else {
            //                // process fixings
            //                if (date < today || (date == today && !implyTodaysFixings_))
            //                    fixings_.emplace_back(Fixing(date, key, value));
            //            }
            //        }
            //    }
            //    file.close();
            //    LOG("CSVLoader completed processing " << filename);
            }

        /// <summary>
        /// Load market quotes
        /// </summary>
        /// <param name="d"></param>
        /// <returns></returns>
        public override List<MarketDatum> LoadQuotes(Date d)
        {
            Utils.QL_REQUIRE(_data.ContainsKey(d), () => "CSVLoader has no data for date " + d);

            return _data[d];
        }
        /// <summary>
        /// Get a particular quote by its unique name
        /// </summary>
        /// <param name="name"></param>
        /// <param name="date"></param>
        /// <returns></returns>
        public override MarketDatum Get(string name,  Date d)
        {
            foreach(MarketDatum md in LoadQuotes(d))
            {
                if (md.Name() == name) return md;
            }
            Utils.QL_FAIL("No MarketDatum for name " + name + " and date " + d);
            throw new Exception();
        }
        /// <summary>
        /// Load fixings
        /// </summary>
        /// <returns></returns>
        public override List<Fixing> LoadFixings()
        {
            return _fixings;
        }
    }
}
