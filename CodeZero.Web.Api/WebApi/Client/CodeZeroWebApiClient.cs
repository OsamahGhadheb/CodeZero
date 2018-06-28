﻿//  <copyright file="CodeZeroWebApiClient.cs" project="CodeZero.Web.Api" solution="CodeZero">
//      Copyright (c) 2018 CodeZero Framework.  All rights reserved.
//  </copyright>
//  <author>Nasr Aldin M.</author>
//  <email>nasr2ldin@gmail.com</email>
//  <website>https://nasraldin.com/codezero</website>
//  <github>https://nasraldin.github.io/CodeZero</github>
//  <date>01/01/2018 01:00 AM</date>
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using CodeZero.Dependency;
using CodeZero.Extensions;
using CodeZero.Web.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace CodeZero.WebApi.Client
{
    public class CodeZeroWebApiClient : ITransientDependency, ICodeZeroWebApiClient
    {
        public static TimeSpan DefaultTimeout { get; set; }

        public string BaseUrl { get; set; }

        public TimeSpan Timeout { get; set; }

        public Collection<Cookie> Cookies { get; private set; }

        public ICollection<NameValue> RequestHeaders { get; set; }

        public ICollection<NameValue> ResponseHeaders { get; set; }

        static CodeZeroWebApiClient()
        {
            DefaultTimeout = TimeSpan.FromSeconds(90);
        }

        public CodeZeroWebApiClient()
        {
            Timeout = DefaultTimeout;
            Cookies = new Collection<Cookie>();
            RequestHeaders = new List<NameValue>();
            ResponseHeaders = new List<NameValue>();
        }

        public virtual async Task PostAsync(string url, int? timeout = null)
        {
            await PostAsync<object>(url, timeout);
        }

        public virtual async Task PostAsync(string url, object input, int? timeout = null)
        {
            await PostAsync<object>(url, input, timeout);
        }

        public virtual async Task<TResult> PostAsync<TResult>(string url, int? timeout = null)
            where TResult : class
        {
            return await PostAsync<TResult>(url, null, timeout);
        }

        public virtual async Task<TResult> PostAsync<TResult>(string url, object input, int? timeout = null)
            where TResult : class
        {
            var cookieContainer = new CookieContainer();
            using (var handler = new HttpClientHandler {CookieContainer = cookieContainer})
            {
                using (var client = new HttpClient(handler))
                {
                    client.Timeout = timeout.HasValue ? TimeSpan.FromMilliseconds(timeout.Value) : Timeout;

                    if (!BaseUrl.IsNullOrEmpty())
                    {
                        client.BaseAddress = new Uri(BaseUrl);
                    }

                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    foreach (var header in RequestHeaders)
                    {
                        client.DefaultRequestHeaders.Add(header.Name, header.Value);
                    }
                    
                    using (var requestContent = new StringContent(Object2JsonString(input), Encoding.UTF8, "application/json"))
                    {
                        foreach (var cookie in Cookies)
                        {
                            if (!BaseUrl.IsNullOrEmpty())
                            {
                                cookieContainer.Add(new Uri(BaseUrl), cookie);
                            }
                            else
                            {
                                cookieContainer.Add(cookie);
                            }
                        }

                        using (var response = await client.PostAsync(url, requestContent))
                        {
                            SetResponseHeaders(response);

                            if (!response.IsSuccessStatusCode)
                            {
                                throw new CodeZeroException("Could not made request to " + url + "! StatusCode: " + response.StatusCode + ", ReasonPhrase: " + response.ReasonPhrase);
                            }

                            var ajaxResponse = JsonString2Object<AjaxResponse<TResult>>(await response.Content.ReadAsStringAsync());
                            if (!ajaxResponse.Success)
                            {
                                throw new CodeZeroRemoteCallException(ajaxResponse.Error);
                            }

                            return ajaxResponse.Result;
                        }
                    }
                }
            }
        }

        private void SetResponseHeaders(HttpResponseMessage response)
        {
            ResponseHeaders.Clear();
            foreach (var header in response.Headers)
            {
                foreach (var headerValue in header.Value)
                {
                    ResponseHeaders.Add(new NameValue(header.Key, headerValue));
                }
            }
        }

        private static string Object2JsonString(object obj)
        {
            if (obj == null)
            {
                return "";
            }

            return JsonConvert.SerializeObject(obj,
                new JsonSerializerSettings
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver()
                });
        }

        private static TObj JsonString2Object<TObj>(string str)
        {
            return JsonConvert.DeserializeObject<TObj>(str,
                new JsonSerializerSettings
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver()
                });
        }
    }
}