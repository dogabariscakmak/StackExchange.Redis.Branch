﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StackExchange.Redis.Branch.IntegrationTest.Helpers
{
    public class TestSettings
    {
        public const string Position = "TestSettings";
        public bool IsDockerComposeRequired { get; set; }
        public string RedisConnectionConfiguration { get; set; }
        public string RedisDockerComposeFile { get; set; }
        public string RedisDockerWorkingDir { get; set; }
        public string DockerComposeExePath { get; set; }
        public string TestDataFilePath { get; set; }
        public bool IsGithubAction { get; set; }
    }
}
