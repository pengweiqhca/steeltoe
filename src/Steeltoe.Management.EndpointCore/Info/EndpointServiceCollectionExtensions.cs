﻿// Copyright 2017 the original author or authors.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Steeltoe.Management.Endpoint.Discovery;
using Steeltoe.Management.Endpoint.Info.Contributor;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Steeltoe.Management.Endpoint.Info
{
    public static class EndpointServiceCollectionExtensions
    {
        /// <summary>
        /// Adds components of the Info actuator to Microsoft-DI
        /// </summary>
        /// <param name="services">Service collection to add info to</param>
        /// <param name="config">Application configuration (this actuator looks for a settings starting with management:endpoints:info)</param>
        /// <param name="addToDiscovery"></param>
        public static void AddInfoActuator(this IServiceCollection services, IConfiguration config, bool addToDiscovery = false)
        {
            services.AddInfoActuator(config, addToDiscovery, new GitInfoContributor(), new AppSettingsInfoContributor(config));
        }

        /// <summary>
        /// Adds components of the info actuator to Microsoft-DI
        /// </summary>
        /// <param name="services">Service collection to add info to</param>
        /// <param name="config">Application configuration (this actuator looks for a settings starting with management:endpoints:info)</param>
        /// <param name="addToDiscovery">Add to Discovery Actuators</param>
        /// <param name="contributors">Contributors to application information</param>
        public static void AddInfoActuator(this IServiceCollection services, IConfiguration config, bool addToDiscovery, params IInfoContributor[] contributors)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (config == null)
            {
                throw new ArgumentNullException(nameof(config));
            }

            services.TryAddEnumerable(ServiceDescriptor.Singleton<IManagementOptions>(new ActuatorManagementOptions(config)));
            var options = new InfoEndpointOptions(config);
            services.TryAddSingleton<IInfoOptions>(options);
            services.RegisterEndpointOptions(options, addToDiscovery);
            AddContributors(services, contributors);
            services.TryAddSingleton<InfoEndpoint>();
        }

        private static void AddContributors(IServiceCollection services, params IInfoContributor[] contributors)
        {
            List<ServiceDescriptor> descriptors = new List<ServiceDescriptor>();
            foreach (var instance in contributors)
            {
                descriptors.Add(ServiceDescriptor.Singleton<IInfoContributor>(instance));
            }

            services.TryAddEnumerable(descriptors);
        }
    }
}