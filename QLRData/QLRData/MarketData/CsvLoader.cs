using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;
using QLNet;

namespace QLRData
{
    public class CsvLoader : Loader
    {
        private bool _implyTodaysFixings;
        private Dictionary<Date, List<MarketDatum>> _data = new Dictionary<Date, List<MarketDatum>>();
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

            Date today = Settings.evaluationDate();

            using (var reader = new StreamReader(filename))
            {

                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine();

                    if (line.Length > 0 && line[0] != '#')
                    {
                        line = line.Trim();
                        string[] tokens = line.Split(new[] { " " }, StringSplitOptions.None);
                        //string[] tokens = Regex.Split(line, ",;\t "); //line.Split(',').ToList();

                        // TODO: should we try, catch and log any invalid lines?
                        Utils.QL_REQUIRE(tokens.ToList().Count == 3, () => "Invalid CSVLoader line, 3 tokens expected " + line);
                        Date date = Parsers.ParseDateExact(tokens[0], "yyyyMMdd");
                        string key = tokens[1];
                        double value = Parsers.ParseDouble(tokens[2]);

                        if (isMarket)
                        {
                            // process market
                            // build market datum and add to map
                            try
                            {
                                if(!_data.ContainsKey(date))
                                {
                                    List<MarketDatum> datum = new List<MarketDatum>();
                                    datum.Add(MarketDatumParser.ParseMarketDatum(date, key, value));
                                    _data.Add(date, datum);
                                }
                                else
                                {
                                    _data[date].Add(MarketDatumParser.ParseMarketDatum(date, key, value));
                                }                                
                                //TLOG("Added MarketDatum " << data_[date].back()->name());
                            }
                            catch (Exception e)
                            {
                                //WLOG("Failed to parse MarketDatum " << key << ": " << e.ToString());
                            }
                        }
                        else
                        {
                            // process fixings
                            if (date < today || (date == today && !_implyTodaysFixings))
                            {
                                _fixings.Add(new Fixing(date, key, value));
                            }
                        }
                    }
                }
                //    LOG("CSVLoader completed processing " << filename);
            }
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
