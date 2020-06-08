﻿// Copyright 2017 the original author or authors.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// https://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using Microsoft.Extensions.Configuration;
using Steeltoe.Common;
using System;
using System.Linq;

namespace Steeltoe.Management.Endpoint.SpringBootAdminClient
{
    public class SpringBootAdminClientOptions
    {
        private const string PREFIX = "spring:boot:admin:client";
        private const string URLS = "URLS";

        public string Url { get; set; }

        public string ApplicationName { get; set; }

        public string BasePath { get; set; }

        public SpringBootAdminClientOptions(IConfiguration config, IApplicationInstanceInfo appInfo)
        {
            if (config == null)
            {
                throw new ArgumentNullException(nameof(config));
            }

            if (appInfo is null)
            {
                throw new ArgumentNullException(nameof(appInfo));
            }

            var section = config.GetSection(PREFIX);
            if (section != null)
            {
                section.Bind(this);
            }

            // Require base path to be supplied directly, in the config, or in the app instance info
            BasePath ??= GetBasePath(config) ?? appInfo?.Uris?.FirstOrDefault() ?? throw new NullReferenceException($"Please set {PREFIX}:BasePath in order to register with Spring Boot Admin");
            ApplicationName ??= appInfo.ApplicationName;
        }

        private string GetBasePath(IConfiguration config)
        {
            var urlString = config.GetValue<string>(URLS);
            var urls = urlString?.Split(';');
            if (urls?.Length > 0)
            {
                return urls[0];
            }

            return null;
        }
    }
}