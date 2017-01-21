﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using Microsoft.Azure.Management.Fluent;
using Microsoft.Azure.Management.Network.Fluent;
using Microsoft.Azure.Management.Resource.Fluent;
using Microsoft.Azure.Management.Resource.Fluent.Authentication;
using Microsoft.Azure.Management.Resource.Fluent.Core;
using Microsoft.Azure.Management.Samples.Common;
using System.Diagnostics;
using System.IO;

namespace ManageSimpleApplicationGateway
{
    /**
     * Azure network sample for managing application gateways.
     *
     *  - CREATE an application gateway for load balancing
     *    HTTP/HTTPS requests to backend server pools of virtual machines
     *
     *    This application gateway serves traffic for multiple
     *    domain names
     *
     *    Routing Rule 1
     *    Hostname 1 = None
     *    Backend server pool 1 = 4 virtual machines with IP addresses
     *    Backend server pool 1 settings = HTTP:8080
     *    Front end port 1 = HTTP:80
     *    Listener 1 = HTTP
     *    Routing rule 1 = HTTP listener 1 => backend server pool 1
     *    (round-robin load distribution)
     *
     *  - MODIFY the application gateway - re-configure the Routing Rule 1 for SSL offload &
     *    add a host name, www.contoso.com
     *
     *    Change listener 1 from HTTP to HTTPS
     *    Add SSL certificate to the listener
     *    Update front end port 1 to HTTPS:1443
     *    Add a host name, www.contoso.com
     *    Enable cookie-based affinity
     *
     *    Modified Routing Rule 1
     *    Hostname 1 = www.contoso.com
     *    Backend server pool 1 = 4 virtual machines with IP addresses
     *    Backend server pool 1 settings = HTTP:8080
     *    Front end port 1 = HTTPS:1443
     *    Listener 1 = HTTPS
     *    Routing rule 1 = HTTPS listener 1 => backend server pool 1
     *    (round-robin load distribution)
     *
     */
    public class Program
    {
        private static readonly string rgName = SharedSettings.RandomResourceName("rgNEAGS", 15);
        private static readonly string pipName = SharedSettings.RandomResourceName("pip" + "-", 18);

        public static void Main(string[] args)
        {
            try
            {
                //=================================================================
                // Authenticate
                var credentials = SharedSettings.AzureCredentialsFactory.FromFile(Environment.GetEnvironmentVariable("AZURE_AUTH_LOCATION"));

                var azure = Azure
                    .Configure()
                    .WithLogLevel(HttpLoggingDelegatingHandler.Level.BASIC)
                    .Authenticate(credentials)
                    .WithDefaultSubscription();

                // Print selected subscription
                Console.WriteLine("Selected subscription: " + azure.SubscriptionId);

                try
                {
                    //=======================================================================
                    // Create an application gateway

                    Console.WriteLine("================= CREATE ======================");
                    Console.WriteLine("Creating an application gateway... (this can take about 20 min)");
                    Stopwatch t = Stopwatch.StartNew();

                    IApplicationGateway applicationGateway = azure.ApplicationGateways.Define("myFirstAppGateway")
                            .WithRegion(Region.US_EAST)
                            .WithNewResourceGroup(rgName)
                            // Request routing rule for HTTP from public 80 to public 8080
                            .DefineRequestRoutingRule("HTTP-80-to-8080")
                                .FromPublicFrontend()
                                .FromFrontendHttpPort(80)
                                .ToBackendHttpPort(8080)
                                .ToBackendIpAddress("11.1.1.1")
                                .ToBackendIpAddress("11.1.1.2")
                                .ToBackendIpAddress("11.1.1.3")
                                .ToBackendIpAddress("11.1.1.4")
                                .Attach()
                            .WithNewPublicIpAddress()
                            .Create();

                    t.Stop();

                    Console.WriteLine("Application gateway created: (took " + (t.ElapsedMilliseconds / 1000) + " seconds)");
                    Utilities.PrintAppGateway(applicationGateway);


                    //=======================================================================
                    // Update an application gateway
                    // configure the first routing rule for SSL offload

                    Console.WriteLine("================= UPDATE ======================");
                    Console.WriteLine("Updating the application gateway");

                    t = Stopwatch.StartNew();

                    applicationGateway.Update()
                            .WithoutRequestRoutingRule("HTTP-80-to-8080")
                            .DefineRequestRoutingRule("HTTPs-1443-to-8080")
                                .FromPublicFrontend()
                                .FromFrontendHttpsPort(1443)
                                .WithSslCertificateFromPfxFile(new FileInfo("myTest._pfx"))
                                .WithSslCertificatePassword("Abc123")
                                .ToBackendHttpPort(8080)
                                .ToBackendIpAddress("11.1.1.1")
                                .ToBackendIpAddress("11.1.1.2")
                                .ToBackendIpAddress("11.1.1.3")
                                .ToBackendIpAddress("11.1.1.4")
                                .WithHostName("www.contoso.com")
                                .WithCookieBasedAffinity()
                                .Attach()
                            .Apply();

                    t.Stop();

                    Console.WriteLine("Application gateway updated: (took " + (t.ElapsedMilliseconds / 1000) + " seconds)");
                    Utilities.PrintAppGateway(applicationGateway);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    Console.WriteLine(e.StackTrace);
                }
                finally
                {
                    try
                    {
                        Console.WriteLine("Deleting Resource Group: " + rgName);
                        azure.ResourceGroups.DeleteByName(rgName);
                        Console.WriteLine("Deleted Resource Group: " + rgName);
                    }
                    catch (NullReferenceException)
                    {
                        Console.WriteLine("Did not create any resources in Azure. No clean up is necessary");
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                        Console.WriteLine(e.StackTrace);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
            }
        }
    }
}