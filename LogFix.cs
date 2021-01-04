using BepInEx;
using UnityEngine;
using System.IO;
using System;
using System.Text;

namespace LogFix
{
    [BepInPlugin("pastebee.generic.logfix", "Log Fix", "0.0")]
    public class LogFix : BaseUnityPlugin
    {
        public LogFix()
        {
            // TODO: Send old logs to a backup folder instead of deleting them
            if (File.Exists("exceptionLog.txt"))
            {
                File.Delete("exceptionLog.txt");
            }
            if (File.Exists("consoleLog.txt"))
            {
                File.Delete("consoleLog.txt");
            }
            
            Application.RegisterLogCallback(HandleLog);
            
            Log("[LogFix] Logs registered successfully (+ shortening)");
            LogError("[LogFix] Logs registered successfully (+ shortening)");
        }
        
		public void HandleLog(string logString, string stackTrace, LogType type)
		{
			if (type == LogType.Error || type == LogType.Exception)
			{
                /*
				File.AppendAllText("exceptionLog.txt", logString + Environment.NewLine);
				File.AppendAllText("exceptionLog.txt", stackTrace + Environment.NewLine);
                */
                if (logString == lastExceptionMsg && stackTrace == lastExceptionTrace)
                {
                    exceptionRepeats++;
                    if (exceptionRepeats % 100 == 0)
                    {
                        WriteExceptionRepeats();
                    }
                }
                else
                {
                    if (exceptionRepeats > 1)
                    {
                        WriteExceptionRepeats();
                        LogError("");
                        LogError("");
                    }
                    exceptionRepeats = 0;
                    lastExceptionRepeats = 0;
                    lastExceptionMsg = logString;
                    lastExceptionTrace = stackTrace;
                    LogError(logString);
                    LogError(stackTrace);
                }
				return;
			}
            if (logString == lastConsoleMsg)
            {
                consoleRepeats++;
                if (consoleRepeats % 100 == 0)
                {
                    WriteConsoleRepeats();
                }
            }
            else
            {
                if (consoleRepeats > 1)
                {
                    WriteConsoleRepeats();
                    Log("");
                }
                consoleRepeats = 0;
                lastConsoleRepeats = 0;
                lastConsoleMsg = logString;
                Log(logString);
            }
		}
        
        public void WriteToStream(FileStream fs, string data)
        {
            fs.Write(Encoding.ASCII.GetBytes(data), 0, data.Length);
        }
        
        public void WriteExceptionRepeats()
        {
            int lengthToRemove = (" [x" + lastExceptionRepeats + "]").Length;
            if (lastExceptionRepeats == 0) lengthToRemove = 3;
            using (FileStream fs = new FileStream("exceptionLog.txt", FileMode.Open, FileAccess.ReadWrite))
            {
                fs.Seek(0, SeekOrigin.End);
                fs.SetLength(fs.Length - lengthToRemove);
                WriteToStream(fs, " [x" + exceptionRepeats + "]");
            }
            lastExceptionRepeats = exceptionRepeats;
        }
        
        public void WriteConsoleRepeats()
        {
            int lengthToRemove = (" [x" + lastConsoleRepeats + "]").Length;
            if (lastConsoleRepeats == 0) lengthToRemove = 1;
            using (FileStream fs = new FileStream("consoleLog.txt", FileMode.Open, FileAccess.ReadWrite))
            {
                fs.Seek(0, SeekOrigin.End);
                fs.SetLength(fs.Length - lengthToRemove);
                WriteToStream(fs, " [x" + consoleRepeats + "]");
            }
            lastConsoleRepeats = consoleRepeats;
        }
        
        public void Log(string logString)
        {
            File.AppendAllText("consoleLog.txt", logString + Environment.NewLine);
        }
        
        public void LogError(string logString)
        {
            File.AppendAllText("exceptionLog.txt", logString + Environment.NewLine);
        }
        
        public string lastConsoleMsg = null;
        public string lastExceptionMsg = null;
        public string lastExceptionTrace = null;
        public int consoleRepeats = 0;
        public int exceptionRepeats = 0;
        public int lastConsoleRepeats = 0;
        public int lastExceptionRepeats = 0;
        
        public string updateURL = "http://beestuff.pythonanywhere.com/audb/api/mods/0/22";
        public int version = 0;
        public string keyE = "AQAB";
        public string keyN = "yu7XMmICrzuavyZRGWoknFIbJX4N4zh3mFPOyfzmQkil2axVIyWx5ogCdQ3OTdSZ0xpQ3yiZ7zqbguLu+UWZMfLOBKQZOs52A9OyzeYm7iMALmcLWo6OdndcMc1Uc4ZdVtK1CRoPeUVUhdBfk2xwjx+CvZUlQZ26N1MZVV0nq54IOEJzC9qQnVNgeeHxO1lRUTdg5ZyYb7I2BhHfpDWyTvUp6d5m6+HPKoalC4OZSfmIjRAi5UVDXNRWn05zeT+3BJ2GbKttwvoEa6zrkVuFfOOe9eOAWO3thXmq9vJLeF36xCYbUJMkGR2M5kDySfvoC7pzbzyZ204rXYpxxXyWPP5CaaZFP93iprZXlSO3XfIWwws+R1QHB6bv5chKxTZmy/Imo4M3kNLo5B2NR/ZPWbJqjew3ytj0A+2j/RVwV9CIwPlN4P50uwFm+Mr0OF2GZ6vU0s/WM7rE78+8Wwbgcw6rTReKhVezkCCtOdPkBIOYv3qmLK2S71NPN2ulhMHD9oj4t0uidgz8pNGtmygHAm45m2zeJOhs5Q/YDsTv5P7xD19yfVcn5uHpSzRIJwH5/DU1+aiSAIRMpwhF4XTUw73+pBujdghZdbdqe2CL1juw7XCa+XfJNtsUYrg+jPaCEUsbMuNxdFbvS0Jleiu3C8KPNKDQaZ7QQMnEJXeusdU=";
    }
}