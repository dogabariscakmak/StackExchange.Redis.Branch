using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace StackExchange.Redis.Branch.IntegrationTest.Helpers
{
    public class DockerStarter : IDisposable
    {
        public string DockerComposeExe { get; private set; }
        public string ComposeFile { get; private set; }
        public string WorkingDir { get; private set; }

        public DockerStarter(string dockerComposeExe, string composeFile, string workingDir)
        {
            DockerComposeExe = dockerComposeExe;
            ComposeFile = composeFile;
            WorkingDir = workingDir;
        }

        public void Start()
        {
            var startInfo = generateInfo("up");
            _dockerProcess = Process.Start(startInfo);
            Thread.Sleep(5000);
        }

        private Process _dockerProcess;

        public void Dispose()
        {
            _dockerProcess.Close();

            var stopInfo = generateInfo("down");
            var stop = Process.Start(stopInfo);
            stop.WaitForExit();
        }

        private ProcessStartInfo generateInfo(string argument)
        {
            var procInfo = new ProcessStartInfo
            {
                FileName = DockerComposeExe,
                Arguments = $"-f {ComposeFile} {argument}",
                WorkingDirectory = WorkingDir
            };
            return procInfo;
        }
    }
}
