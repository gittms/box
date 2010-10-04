using System;
using System.IO;
using System.IO.Packaging;
using System.Collections.Generic;
using System.Diagnostics;
using System.Security.Principal;

namespace Definitif.Box.Unbox
{
    /// <summary>
    /// Represents static class for system information retrieval.
    /// </summary>
    public static class System
    {
        /// <summary>
        /// Checks if running under Mono runtime.
        /// </summary>
        public static bool IsMonoRuntime
        {
            get
            {
                Type t = Type.GetType("Mono.Runtime");
                return (t != null);
            }
        }

        /// <summary>
        /// Checks if running on Windows platform.
        /// </summary>
        public static bool IsWindowsOs
        {
            get
            {
                int p = (int)Environment.OSVersion.Platform;
                return !((p == 4) || (p == 6) || (p == 128));
            }
        }

        /// <summary>
        /// Checks is current user is an administrator.
        /// </summary>
        public static bool IsAdministrator
        {
            get
            {
                WindowsIdentity identity = WindowsIdentity.GetCurrent();
                WindowsPrincipal principal = new WindowsPrincipal(identity);
                return principal.IsInRole(WindowsBuiltInRole.Administrator);
            }
        }

        /// <summary>
        /// Installs assembly to Global Assembly Cache.
        /// </summary>
        public static void InstallAssemblyToGac(string path)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.LoadUserProfile = true;

            if (IsWindowsOs)
            {
                startInfo.FileName = @"C:\Program Files\Microsoft SDKs\Windows\v6.0A\bin\gacutil.exe";
                startInfo.Arguments = " /i " + path;
            }
            else
            {
                startInfo.FileName = "gacutil";
                startInfo.Arguments = "-i " + path;
            }

            Process.Start(startInfo);
        }

        /// <summary>
        /// Unzips zip file to directory specified.
        /// </summary>
        public static void Unzip(string zip, string path)
        {
            Package zipPackage = Package.Open(zip, FileMode.Open, FileAccess.Read);
            foreach (PackagePart part in zipPackage.GetParts())
            {
                // Constructing part path.
                string partPath = part.Uri.OriginalString.Replace('/', Path.DirectorySeparatorChar);
                partPath = partPath.TrimStart(Path.DirectorySeparatorChar);
                partPath = Path.Combine(path, partPath);

                // Creating directory for part.
                string partDirectory = Path.GetDirectoryName(partPath);
                if (!Directory.Exists(partDirectory)) Directory.CreateDirectory(partDirectory);

                // Saving part to disk.
                Stream partStream = part.GetStream();
                FileStream fileStream = new FileStream(partPath, FileMode.Create);
                byte[] buffer = new byte[8 * 1024]; int len;
                while ((len = partStream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    fileStream.Write(buffer, 0, len);
                }
                fileStream.Close();
            }
            zipPackage.Close();
        }
    }
}
