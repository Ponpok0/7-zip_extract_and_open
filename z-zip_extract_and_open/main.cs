using Microsoft.Win32;
using System.Reflection;
using System.Diagnostics;

class main {

    static public int Main(string[] args)
    {
        #region Main

        var programPath = Path.GetFullPath(".\\7-zip_extract_and_open.bat");
        var programDir = Path.GetDirectoryName(Path.GetFullPath(programPath));
        var _7zGPath = Path.GetFullPath("..\\7zG.exe");
        #region EnvCheck
        if (!File.Exists(_7zGPath))
        {
            Console.WriteLine("ERROR: The program directory is not correct.");
            Console.WriteLine(_7zGPath);
            Console.ReadKey();
            return 1;
        }

        if (!File.Exists(programPath))
        {
            Console.WriteLine("ERROR: The program does not exist.");
            Console.WriteLine(programPath);
            Console.ReadKey();
            return 1;
        }

        string iconDir = Path.Combine(programDir!, "icons");

        if (!Directory.Exists(iconDir))
        {
            Console.WriteLine("ERROR: The icon directory does not exist.");
            Console.WriteLine(iconDir);
            Console.ReadKey();
            return 1;
        }
        #endregion
        #region prepare extensions list
        // target extensions List
        // key: extension, value: icon path
        var targetExtensions = new List<string> {
              ".7z",
        //    ".ar",
        //    ".arj",
              ".bz2",
        //    ".cab",
        //    ".chm",
        //    ".cpio",
        //    ".cramfs",
        //    ".dmg",
        //    ".ext",
        //    ".fat",
        //    ".gpt",
              ".gz",
        //    ".hfs",
        //    ".ihex",
        //    ".iso",
              ".lzh",
        //    ".lzma",
        //    ".mbr",
        //    ".msi",
        //    ".nsis",
        //    ".ntfs",
        //    ".qcow2",
              ".rar",
        //    ".rpm",
        //    ".suashfs",
              ".tar",
        //    ".udf",
        //    ".uefi",
        //    ".vdi",
        //    ".vhd",
        //    ".vmdk",
              ".wim",
        //    ".xar",
              ".xz",
        //    ".z",
              ".zip"
        };
        Dictionary<string,string> extensionAndPathes= new Dictionary<string,string>();
        foreach(var extension in targetExtensions)
        {
            extensionAndPathes.Add(extension, Path.Combine(programDir!, iconDir, GetIconName(extension)));
        }
        #endregion

        #region main process
        // main process
        foreach (var extension in extensionAndPathes.Keys)
        {
            var iconPath = extensionAndPathes[extension];

            if (!IsExtensionIconAssociated(extension, iconPath))
            {

                // assosiate with the extension and the icon
                string extensionOrProgId = 
                    GetExtensionCurrentUserAssociation(extension, 
                                GetExtensionUserChoice(extension));

                // backup the registory keys to .reg
                int ret = BackupRegistory(extensionOrProgId);
                // When error occurred
                if (ret != 0)
                {
                    Console.WriteLine($"ERROR: Cannot export the registry key.\n{extensionOrProgId}");
                    Console.ReadKey();
                    return 1;
                }

                AssociateExtensionIcon(extensionOrProgId, iconPath);
            }
        }

        // pause before exit
        Console.WriteLine("Process is completed.");
        Console.ReadKey();
        return 0;

        #endregion
        #endregion
        #region functions
        //static string GetExecutablePath()
        //{
        //    var entryAssembly = Assembly.GetEntryAssembly();
        //    if (entryAssembly != null)
        //    {
        //        return entryAssembly.Location;
        //    }

        //    var uri = new UriBuilder(Assembly.GetExecutingAssembly().Location);
        //    return Uri.UnescapeDataString(uri.Path);
        //}

        /// <summary>
        /// Check the file is associated or not
        /// </summary>
        bool IsExtensionIconAssociated(string extension, string defaultIcon)
        {
            // Check extension is associated the program
            var associatedExtensionUserChoiceKey 
                = GetExtensionUserChoice(extension);
            var associatedExtensionCurrentUserKey 
                = GetExtensionCurrentUserAssociation(extension, associatedExtensionUserChoiceKey);

            bool ret;
            if(associatedExtensionUserChoiceKey != string.Empty)
            {
                ret = GetDefaultIcon(associatedExtensionUserChoiceKey) == defaultIcon;
            } else
            {
                ret = GetDefaultIcon(associatedExtensionCurrentUserKey) == defaultIcon;
            }

            return ret;
        }

        string GetExtensionUserChoice(string extension)
        {
            // Get associated value
            using (RegistryKey key = 
                    Registry.ClassesRoot.OpenSubKey(
                    $"HKEY_CURRENT_USER\\SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Explorer\\FileExts\\{extension}\\UserChoice"
                  ))
            {
                string progId = "";
                if (key is not null)
                {
                    progId = (string)key.GetValue("");
                }
                return progId;
            }
        }

        string GetExtensionCurrentUserAssociation(string extension, string userChoiceProgId)
        {
            // Get associated value
            using (RegistryKey progId = Registry.CurrentUser.OpenSubKey($"Software\\Classes\\{userChoiceProgId}"))
            {
                if (progId is not null)
                {
                    return (string)progId.GetValue("");
                }

                using (RegistryKey key = Registry.CurrentUser.OpenSubKey($"Software\\Classes\\{extension}"))
                {
                    if (key is not null)
                    {
                        return (string)key.GetValue("");
                    }
                }
            }
            return string.Empty;
        }
        string GetDefaultIcon(string extensionOrProgId)
        {
            // Get associated value
            using (RegistryKey key = Registry.CurrentUser.OpenSubKey($"Software\\Classes\\{extensionOrProgId}"))
            {
                if (key is not null)
                {
                    return (string)key.GetValue("");
                }
            }
            return string.Empty;
        }

        /// <summary>
        /// associate the extension with the icon
        /// </summary>
        void AssociateExtensionIcon(string extensionOrProgId, string iconPath)
        {
            using (RegistryKey key = Registry.CurrentUser.CreateSubKey("Software\\Classes").CreateSubKey(extensionOrProgId + "\\DefaultIcon"))
            {
                key.SetValue("", iconPath);
            }
        }

        /// <summary>
        /// backup the registory keys to .reg
        /// </summary>
        int BackupRegistory(string extensionOrProgId)
        {
            // backup from the extension key
            string registryKeyPath = $"HKEY_CURRENT_USER\\SOFTWARE\\Classes\\{extensionOrProgId}";

            // add datetime in filename
            var backupFileName = DateTime.Now.ToString("yyyyMMddHHmmss") + extensionOrProgId + ".reg";
            var backupFileDir = Path.Combine(programDir!, "backup");
            if (!Directory.Exists(backupFileDir)) Directory.CreateDirectory(backupFileDir);

            var backupFilePath = Path.Combine(backupFileDir, backupFileName);

            try
            {
                using (Process process = new Process())
                {
                    process.StartInfo.FileName = "regedit.exe";
                    process.StartInfo.Arguments = $"/e \"{backupFilePath}\" \"{registryKeyPath}\"";
                    process.Start();
                    process.WaitForExit();
                    if(process.ExitCode != 0)
                    {
                        throw new IOException("Backup failed");
                    }
                }
                Console.WriteLine($"{extensionOrProgId} backup file was created at {backupFilePath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error is occurred." + "\n" + ex.Message + "\n" + ex.StackTrace);
                Console.ReadKey();
                return 1;
            }

            return 0;
        }

        string GetIconName(string extension) => extension.Substring(1) + ".ico";
        #endregion
    }
}
