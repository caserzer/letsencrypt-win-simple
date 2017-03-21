# letsencrypt-win-simple
A Simple ACME Client for Windows folked from https://github.com/Lone-Coder/letsencrypt-win-simple

# Overview

This is a ACME windows CLI client built in native .net and aims to be as simple as possible to use.

It's built on top of the [.net ACME protocol library](https://github.com/ebekker/ACMESharp).

Add Azure plugin for the Azure storage container.

For asp.net core application hosting on Azure cloud service / Service Fabric which have multi instances it's difficult to 
put the challenge file to the /.well-known/acme-challenge folder. So this project's intend is to put the challenge file
into the an Azure storage container. And redirect the challenge requests to the container. 

# How to 
1. Download the files in AzureFileProvider, add them to your asp.net core , modify the namespace and AzureFileProvider.cs 

2. Register the challenge url in Startup and publish your asp.net application
<code>
            app.UseDirectoryBrowser(new DirectoryBrowserOptions
            {
                FileProvider = new AzureFileProvider(Configuration),
                RequestPath = new PathString("/.well-known/acme-challenge"),
            });



            app.UseStaticFiles( new StaticFileOptions {  ServeUnknownFileTypes = true
                ,FileProvider = new AzureFileProvider(Configuration)
                ,
                RequestPath = new PathString("/.well-known/acme-challenge")
            });
</code>

3. complie this solution and run the app with
<code>
letsencrypt --accepttos --azurehost YOURDOMAIN --constr YOURAZURESTORAGECONNECTIONSTRING --container STORAGECONTAINERNAME
</code>
# Settings

Some of the applications' settings can be updated in the app's settings or configuration file. the file is in the application root and is called letsencrypt.exe.config.

### FileDateFormat

The FileDateFormat is a string that is used to format the date of the pfx file's friendly name.
Default: ```yyyy/M/d/ h:m:s tt``` ex. 2016/1/21 2:58:12 PM
See https://msdn.microsoft.com/en-us/library/8kb3ddd4(v=vs.110).aspx to create your own custom date format.

### PFXPassword

The password to sign all PFX files with. Default is empty.

### RSAKeyBits

The key size to sign the certificate with. Default is 2048, Minimum is 1024.

### HostsPerPage

The number of hosts to display per page. Default is 50.

### CertificatePath

The path where certificates and request files are stored. 
Default is empty which resolves to `%appdata%\letsencrypt-win-simple\[BaseUri]`. 
All directories and subdirectories in the specified path are created unless they already exist.
The default path is used when the specified path is invalid.

### RenewalDays

The number of days to renew a certificate after.
The default is 60. Let's Encrypt certificates are currently valid for a max of 90 days so it is advised to not increase the days much.
If you increase the days, please note that you will have less than 30 days to fix any issues if the certificate doesn't renew correctly.

### CertificateStore

The certificate store to save the certificates in.

### CleanupFolders

If set to True, it will cleanup the folder structure and files it creates under the site for authorization.

# Wiki

Please head to the [Wiki](https://github.com/Lone-Coder/letsencrypt-win-simple/wiki) to learn more.

## Settings

See the [Application Settings](https://github.com/Lone-Coder/letsencrypt-win-simple/wiki/Application-Settings) page on the wiki for settings such as how to change the location where certificates are stored, how they're generated, and how often they are renewed among other settings.

