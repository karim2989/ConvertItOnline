using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Net.Mail;
namespace ConvertItOnline
{
    public static class StatsController
    {
        private static List<DailyReport> dailyReports = new List<DailyReport>();
        public static DailyReport CurrentReport => dailyReports[dailyReports.Count - 1];
        public static void RegisterHomePageVisit()
        {
            checkCurrentReportDate();
            CurrentReport.HomePageVisits++;
        }
        public static void RegisterProcessStart()
        {
            checkCurrentReportDate();
            CurrentReport.ProcessesStarted++;
        }
        public static void RegisterProcessComplete()
        {
            checkCurrentReportDate();
            CurrentReport.ProcessesComplete++;
        }
        public static void RegisterProcessFailed()
        {
            checkCurrentReportDate();
            CurrentReport.ProcessesFailed++;
        }
        private static void checkCurrentReportDate()
        {
            var today = DateTime.Today;
            if (dailyReports.Count == 0 || CurrentReport.Day != today)
            {
                System.IO.File.AppendAllText("OuputLog.txt", "Stats :: New day" + DateTime.Now);
                dailyReports.Add(new DailyReport() { Day = today });
            }
        }
        public static string PrintReport => JsonSerializer.Serialize(dailyReports);
    }
    public class DailyReport
    {
        public DateTime Day { get; set; }
        public long HomePageVisits { get; set; }
        public long ProcessesStarted { get; set; }
        public long ProcessesComplete { get; set; }
        public long ProcessesFailed { get; set; }
    }
}