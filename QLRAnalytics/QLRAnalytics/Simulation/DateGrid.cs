using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QLNet;

namespace QLRAnalytics
{
    public class DateGrid
    {
        private List<Date> _dates = new List<Date>();
        private List<Period> _tenors = new List<Period>();
        private List<double> _times = new List<double>();
        private TimeGrid _timeGrid;

        public DateGrid(string grid, QLNet.Calendar gridCalendar, DayCounter dayCounter)
        {
            if (grid == "ALPHA")
            {
                // ALPHA is
                // quarterly up to 10Y,
                // annual up to 30Y,
                // quinquennial up to 100Y
                for (int i = 1; i < 40; i++)
                { // 3M up to 39*3M = 117M = 9Y9M
                    Period p = new Period(i * 3, TimeUnit.Months);
                    p.normalize();
                    _tenors.Add(p);
                }
                for (int i = 10; i < 30; i++) // 10Y up to 29Y
                {
                    _tenors.Add(new Period(i, TimeUnit.Years));
                }

                for (int i = 30; i < 105; i += 5) // 30Y up to 100Y
                {
                    _tenors.Add(new Period(i, TimeUnit.Years));
                }
            }
            else if (grid == "BETA")
            {
                // BETA is
                // monthly up to 10Y,
                // quarterly up to 20Y,
                // annually up to 50Y,
                // quinquennial up to 100Y
                for (int i = 1; i < 119; i++)
                {
                    Period p = new Period(i, TimeUnit.Months);                        
                    p.normalize();
                    _tenors.Add(p);
                }
                for (int i = 40; i < 80; i++)
                {
                    Period p = new Period(3 * i, TimeUnit.Months);
                    p.normalize();
                    _tenors.Add(p);
                }
                for (int i = 20; i < 50; i++)
                {
                    _tenors.Add(new Period(i, TimeUnit.Years));
                }                
                for (int i = 50; i <= 100; i += 5)
                {
                    _tenors.Add(new Period(i, TimeUnit.Years));
                }                
            }
            else
            { // uniform grid of format "numPillars,spacing" (e.g. 40,1M)
                List<string> tokens = new List<string>();                
                //boost::split(tokens, grid, boost::is_any_of(","));
                if (tokens.Count <= 2)
                {
                    // uniform grid of format "numPillars,spacing" (e.g. 40,1M)
                    Period gridTenor = new Period(1, TimeUnit.Years); // default
                    int gridSize = 1; // atoi(tokens[0].c_str());
                    QLNet.Utils.QL_REQUIRE(gridSize > 0, () => "Invalid DateGrid string " + grid);
                    if (tokens.Count == 2)
                    {
                        //gridTenor = data::parsePeriod(tokens[1]);
                    }                        
                    if (gridTenor == new Period(1, TimeUnit.Days))
                    {
                        // we have a daily grid. Period and Calendar are not consistant with
                        // working & actual days, so we set the tenor grid
                        Date today = Settings.evaluationDate();
                        Date d = today;
                        for (int i = 0; i < gridSize; i++)
                        {
                            d = gridCalendar.advance(d, new Period(1, TimeUnit.Days), BusinessDayConvention.Following); // next working day
                            int n = d - today;
                            _tenors.Add(new Period(n, TimeUnit.Days));
                        }
                    }
                    else
                    {
                        for (int i = 0; i < gridSize; i++)
                        {
                            _tenors.Add((i + 1) * gridTenor);
                        }                            
                    }
                }
                else
                {
                    // New style : 1D,2D,1W,2W,3Y,5Y,....
                    for (int i = 0; i < tokens.Count; i++)
                    {
                        //_tenors.Add(parsePeriod(tokens[i]));
                    }                        
                }
            }
            BuildDates(gridCalendar, dayCounter);
        }

        public void Truncate(int len)
        {
            if(_dates.Count > len)
            {
                //DLOG("Truncating DateGrid, removing elements " << dates_[len] << " to " << dates_.back());
                _dates.Resize(len);
                _tenors.Resize(len);
                _times.Resize(len);
                _timeGrid = new TimeGrid(_times, 0);  // TODO: Is this correct??
                //DLOG("DateGrid size now " << dates_.size());
            }
        }

        public int Size()
        {
            return _dates.Count;
        }

        public List<Date> Dates()
        {
            return _dates;
        }

        public List<Period> Tenors()
        {
            return _tenors;
        }

        public List<double> Times()
        {
            return _times;
        }        

        public TimeGrid TimeGrid()
        {
            return _timeGrid;
        }
             
        public void BuildDates(QLNet.Calendar calendar, QLNet.DayCounter dc)
        {
            _dates.Resize(_tenors.Count);
            Date today = Settings.evaluationDate();
            for (int i = 0; i < _tenors.Count; i++)
            {
                if(_tenors[i].units() == TimeUnit.Days)
                {
                    _dates[i] = calendar.adjust(today + _tenors[i]);
                }
                else
                {
                    _dates[i] = calendar.advance(today, _tenors[i], BusinessDayConvention.Following, true);
                }                
            }
            QLNet.Utils.QL_REQUIRE(_dates.Count == _tenors.Count, () => "Date/Tenor mismatch");

            // Build times
            _times.Resize(_dates.Count);
            for (int i = 0; i < _dates.Count; i++)
            {
                _times[i] = dc.yearFraction(today, _dates[i]);
            }                

            _timeGrid = new TimeGrid(_times, _times.Count);

            // Log the date grid
            //log();
        }

        ////! Build a date grid from the given vector of tenors.
        //DateGrid(const std::vector<QuantLib::Period>& tenors = std::vector<QuantLib::Period>(),
        //         const QuantLib::Calendar& gridCalendar = QuantLib::TARGET(),
        //         const QuantLib::DayCounter& dayCounter = QuantLib::ActualActual());

        ////! Build a date grid from an explicit set of dates, sorted in ascending order.
        //DateGrid(const std::vector<QuantLib::Date>& dates,
        //         const QuantLib::DayCounter& dayCounter = QuantLib::ActualActual());

    
    }
}
