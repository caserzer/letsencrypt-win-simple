﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using Serilog;
using Microsoft.WindowsAzure.Storage;

namespace LetsEncrypt.ACME.Simple
{
    public class AzureContainerPlugin : Plugin
    {
        private string _conStr;

        private string _container;

        public override string Name => "Azure";

        public override List<Target> GetTargets()
        {
            var result = new List<Target>();

            return result;
        }

        public override List<Target> GetSites()
        {
            var result = new List<Target>();

            return result;
        }

        public override void Install(Target target, string pfxFilename, X509Store store, X509Certificate2 certificate)
        {
            if (!string.IsNullOrWhiteSpace(Program.Options.Script) &&
                !string.IsNullOrWhiteSpace(Program.Options.ScriptParameters))
            {
                var parameters = string.Format(Program.Options.ScriptParameters, target.Host,
                    Properties.Settings.Default.PFXPassword,
                    pfxFilename, store.Name, certificate.FriendlyName, certificate.Thumbprint);
                Log.Information("Running {Script} with {parameters}", Program.Options.Script, parameters);
                Process.Start(Program.Options.Script, parameters);
            }
            else if (!string.IsNullOrWhiteSpace(Program.Options.Script))
            {
                Log.Information("Running {Script}", Program.Options.Script);
                Process.Start(Program.Options.Script);
            }
            else
            {
                Console.WriteLine(" WARNING: Unable to configure server software.");
            }
        }

        public override void Install(Target target)
        {
            // This method with just the Target paramater is currently only used by Centralized SSL
            if (!string.IsNullOrWhiteSpace(Program.Options.Script) &&
                !string.IsNullOrWhiteSpace(Program.Options.ScriptParameters))
            {
                var parameters = string.Format(Program.Options.ScriptParameters, target.Host,
                    Properties.Settings.Default.PFXPassword, Program.Options.CentralSslStore);
                Log.Information("Running {Script} with {parameters}", Program.Options.Script, parameters);
                Process.Start(Program.Options.Script, parameters);
            }
            else if (!string.IsNullOrWhiteSpace(Program.Options.Script))
            {
                Log.Information("Running {Script}", Program.Options.Script);
                Process.Start(Program.Options.Script);
            }
            else
            {
                Console.WriteLine(" WARNING: Unable to configure server software.");
            }
        }

        public override void Renew(Target target)
        {
            Console.WriteLine(" WARNING: Unable to renew.");
        }

        public override void PrintMenu()
        {
            if (!String.IsNullOrEmpty(Program.Options.ConStr) && !String.IsNullOrEmpty(Program.Options.Container))
            { 
                _conStr = Program.Options.ConStr;
                _container = Program.Options.Container;
                var target = new Target()
                {
                    Host = Program.Options.ManualHost,
                    PluginName = Name
                };
                Auto(target);
                Environment.Exit(0);
            }

            Console.WriteLine(" M: Generate a certificate manually.");
        }

        public override void HandleMenuResponse(string response, List<Target> targets)
        {
            if (response == "m")
            {
                Console.Write("Enter a host name: ");
                var hostName = Console.ReadLine();
                string[] alternativeNames = null;
                List<string> sanList = null;

                if (Program.Options.San)
                {
                    Console.Write("Enter all Alternative Names seperated by a comma ");

                    // Copied from http://stackoverflow.com/a/16638000
                    int BufferSize = 16384;
                    Stream inputStream = Console.OpenStandardInput(BufferSize);
                    Console.SetIn(new StreamReader(inputStream, Console.InputEncoding, false, BufferSize));

                    var sanInput = Console.ReadLine();
                    alternativeNames = sanInput.Split(',');
                    sanList = new List<string>(alternativeNames);
                }

                Console.Write("Enter a site path (the web root of the host for http authentication): ");
                var physicalPath = Console.ReadLine();

                if (sanList == null || sanList.Count <= 100)
                {
                    var target = new Target()
                    {
                        Host = hostName,
                        WebRootPath = physicalPath,
                        PluginName = Name,
                        AlternativeNames = sanList
                    };
                    Auto(target);
                }
                else
                {
                    Log.Error(
                        "You entered too many hosts for a San certificate. Let's Encrypt currently has a maximum of 100 alternative names per certificate.");
                }
            }
        }

        public override void CreateAuthorizationFile(string answerPath, string fileContents)
        {
            //Log.Information("Writing challenge answer to {answerPath}", answerPath);
            //var directory = Path.GetDirectoryName(answerPath);
            //Directory.CreateDirectory(directory);
            var fileName = Path.GetFileName(answerPath);
            var storageAccount = CloudStorageAccount.Parse(_conStr);
            var container = storageAccount.CreateCloudBlobClient().GetContainerReference(_container);
            container.CreateIfNotExists();
            var blob = container.GetBlockBlobReference(fileName);
            blob.UploadText(fileContents);
        }
    }

}
