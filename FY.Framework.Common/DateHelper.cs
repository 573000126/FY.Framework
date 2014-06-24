using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FY.Framework.Common
{
    public class DateHelper
    {
        /// <summary>
        /// 当前时间是一年中的第几周
        /// </summary>
        /// <param name="Date"></param>
        /// <returns></returns>
        public int GetWeekOrderOfDate(DateTime Date)
        {
            //当天所在的年份
            int year = Date.Year;
            //当年的第一天
            DateTime firstDay = new DateTime(year, 1, 1);
            //当年的第一天是星期几
            int firstOfWeek = Convert.ToInt32(firstDay.DayOfWeek);
            //当年第一周的天数
            int firstWeekDayNum = 7 - firstOfWeek;

            //传入日期在当年的天数与第一周天数的差
            int otherDays = Date.DayOfYear - firstWeekDayNum;
            //传入日期不在第一周内
            if (otherDays > 0)
            {
                int weekNumOfOtherDays;
                if (otherDays % 7 == 0)
                {
                    weekNumOfOtherDays = otherDays / 7;
                }
                else
                {
                    weekNumOfOtherDays = otherDays / 7 + 1;
                }
                return weekNumOfOtherDays + 1;
            }
            //传入日期在第一周内
            else
            {
                return 1;
            }
        }
    }
}
