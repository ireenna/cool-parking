using System;
using System.IO;
using CoolParking.BL.Interfaces;
using CoolParking.BL.Models;

namespace CoolParking.BL.Services
{
    public class LogService : ILogService
    {
        public string LogPath { get; }

        public LogService(string logFilePath)
        {
            LogPath = logFilePath;
        }

        public void Write(string logInfo)
        {
            using (StreamWriter sw = new StreamWriter(LogPath, true))
            {
                sw.WriteLine(logInfo);
            }
        }

        public string Read()
        {
            if(!File.Exists(LogPath))
                throw new InvalidOperationException("There is no such file.");
            using (StreamReader streamReader = new StreamReader(LogPath))
            {
                if (streamReader == null) throw new InvalidOperationException();
                return streamReader.ReadToEnd();
            }
        }
    }

}
// TODO: implement the LogService class from the ILogService interface.
//       One explicit requirement - for the read method, if the file is not found, an InvalidOperationException should be thrown
//       Other implementation details are up to you, they just have to match the interface requirements
//       and tests, for example, in LogServiceTests you can find the necessary constructor format.
