﻿// Copyright 2015 the original author or authors.
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

using Steeltoe.Extensions.Configuration.CloudFoundry;
using System;
using System.Collections.Generic;

namespace Steeltoe.CloudFoundry.Connector.Services
{
    public class HystrixRabbitServiceInfoFactory : ServiceInfoFactory
    {
        private static string[] _scheme = new string[] { RabbitServiceInfo.AMQP_SCHEME, RabbitServiceInfo.AMQPS_SCHEME };
        public static readonly Tags HYSTRIX_RABBIT_SERVICE_TAGS = new Tags("hystrix-amqp");

        public HystrixRabbitServiceInfoFactory()
            : base(HYSTRIX_RABBIT_SERVICE_TAGS, _scheme)
        {
        }

        public override bool Accept(Service binding)
        {
            return base.TagsMatch(binding) && UriCredentialsMatchesScheme(binding.Credentials);
        }

        private bool UriCredentialsMatchesScheme(Dictionary<string, Credential> credentials)
        {
            if (credentials.ContainsKey("amqp"))
            {
                var amqpDict = credentials["amqp"];
                string uri = GetStringFromCredentials(amqpDict, uriKeys);
                if (uri != null)
                {
                    foreach (string uriScheme in _schemes)
                    {
                        if (uri.StartsWith(uriScheme + "://"))
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        public override IServiceInfo Create(Service binding)
        {
            var amqpCredentials = binding.Credentials["amqp"];
            string uri = GetUriFromCredentials(amqpCredentials);
            bool sslEnabled = GetBoolFromCredentials(amqpCredentials, "ssl");
            if (amqpCredentials.ContainsKey("uris"))
            {
                List<string> uris = GetListFromCredentials(amqpCredentials, "uris");
                return new HystrixRabbitServiceInfo(binding.Name, uri, uris, sslEnabled);
            }

            return new HystrixRabbitServiceInfo(binding.Name, uri, sslEnabled);
        }
    }
}
