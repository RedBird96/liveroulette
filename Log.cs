using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LiveRoulette
{
    class Log
    {
        private FrmMain frm;
        public List<string> logArray;
        string LOGFILENAME = @"Roulette.log";
        public Log(FrmMain frmParam)
        {
            frm = frmParam;
            logArray = new List<string>();
            Thread th = new Thread(WriteLogFile);
            th.IsBackground = true;
            th.Start();
        }

        public void addLog(string log, int level = 0)
        {
            DateTime localDate = DateTime.Now;
            string logLine = "", logfileLine = ""; ;
            string time_st = localDate.ToString("HH:mm:ss") + "   ";
            string log_time_st = localDate.ToString("yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture) + "   ";
            logLine = time_st + log + "\r\n";
            logfileLine = log_time_st + log + "\r\n";
            if (level != Global.LOG_LEVEL_ONLYFILE)
                frm.SetText(logLine);
            lock (logArray)
            {
                logArray.Add(logfileLine);
            }
            Console.WriteLine(logfileLine);
        }
        private void WriteLogFile()
        {
            string logFilePath = Path.Combine(Directory.GetCurrentDirectory(), LOGFILENAME);
            while (true)
            {
                lock (logArray)
                {
                    for (int index = 0; index < logArray.Count; index++)
                    {
                        string log_temp = logArray[index];
                        try
                        {
                            File.AppendAllText(logFilePath, log_temp);
                        }
                        catch
                        {
                            Console.WriteLine("Log File Writing Exception");
                        }
                    }
                    logArray.Clear();
                }
                Thread.Sleep(100);
            }
        }
    }
}
