using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarDustCommon.Util
{
    public class TimeUtil
    {
        public static DateTimeOffset BeijingTime
        {
            get {
                return new DateTimeOffset(DateTime.UtcNow).ToOffset(TimeSpan.FromHours(+8));
            }
        }


        public static DateTimeOffset BeijingTimeAlignDay
        {
            get
            {
                DateTimeOffset beijingTime = BeijingTime;
                return new DateTimeOffset(beijingTime.Year, beijingTime.Month, beijingTime.Day, 0, 0, 0, TimeSpan.Zero);
            }
        }
    }
}
