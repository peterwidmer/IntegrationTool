using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationTool.Module.WriteToDynamicsCrm.Execution
{
    public class DateTimeHelper
    {
        public static DateTime? ConvertStringToDateTime(string stringDateTime, string valueFormat)
        {
            DateTime dateTime = DateTime.MinValue;
            int year = -1;
            int month = -1;
            int day = -1;
            int hour = -1;
            int minute = -1;
            int second = -1;

            try
            {
                if (String.IsNullOrEmpty(valueFormat))
                {
                    try
                    {
                        dateTime = DateTime.Parse(stringDateTime);
                    }
                    catch (Exception ex)
                    {
                        dateTime = DateTime.Parse(stringDateTime, System.Threading.Thread.CurrentThread.CurrentCulture.DateTimeFormat);
                    }
                }
                else
                {
                    //Parse the DateTime by provided format
                    year = GetYear(stringDateTime, valueFormat);
                    month = GetDatePart(stringDateTime, valueFormat, "MM");
                    day = GetDatePart(stringDateTime, valueFormat, "dd");
                    hour = GetDatePart(stringDateTime, valueFormat, "hh");
                    minute = GetDatePart(stringDateTime, valueFormat, "mm");
                    second = GetDatePart(stringDateTime, valueFormat, "ss");
                    if (second == -1)
                        second = 0;

                    if (year != -1 && month != -1 && day != -1 && hour != -1 && minute != -1)
                    {
                        dateTime = new DateTime(year, month, day, hour, minute, second);
                    }
                    else
                    {
                        if (year != -1 && month != -1 && day != -1)
                        {
                            dateTime = new DateTime(year, month, day);
                        }
                        else
                        {
                            if (year != -1)
                            {
                                dateTime = new DateTime(year, 1, 1);
                            }
                        }
                    }


                }
            }
            catch (Exception ex)
            {
                try
                {
                    year = Convert.ToInt32(stringDateTime);
                    if (year > 1960 && year < 3000)
                    {
                        dateTime = new DateTime(year, 1, 1);
                    }
                }
                catch (Exception ex2)
                {
                }
            }

            if (!dateTime.Equals(DateTime.MinValue))
            {
                return dateTime;
            }
            else
            {
                return null;
            }
        }

        public static int GetYear(string strDateTime, string strFormat)
        {
            try
            {
                int iResult = -1;
                int iStart = -1;
                string strSub = string.Empty;

                //Get Year
                if (strFormat.Contains("yyyy"))
                {
                    iStart = strFormat.IndexOf("yyyy");
                    strSub = strDateTime.Substring(iStart, 4);
                }
                else
                {
                    if (strFormat.Contains("yy"))
                    {
                        iStart = strFormat.IndexOf("yy");
                        strSub = strDateTime.Substring(iStart, 2);
                    }
                }

                iResult = Convert.ToInt32(strSub);
                return iResult;
            }
            catch (Exception ex)
            {
                return -1;
            }
        }


        public static int GetDatePart(string strDateTime, string strFormat, string strPart)
        {
            try
            {
                int iResult = -1;
                int iStart = -1;
                string strSub = string.Empty;

                if (strFormat.Contains(strPart))
                {
                    iStart = strFormat.IndexOf(strPart);
                    strSub = strDateTime.Substring(iStart, strPart.Length);
                }

                iResult = Convert.ToInt32(strSub);
                return iResult;
            }
            catch (Exception ex)
            {
                return -1;
            }
        }
    }
}
