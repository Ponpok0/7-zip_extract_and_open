using Microsoft.Win32;
using System.Reflection;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.ComponentModel;
using static System.Net.Mime.MediaTypeNames;

class main {

    static public int Main(string[] args)
    {
        #region CONST
        const string DEFAULT_ICON = "DefaultIcon";
        #endregion
        #region Main

        var programName = "7-zip_extract_and_open.bat";
        var programPath = Path.GetFullPath($".\\{programName}");
        var programDir = Path.GetDirectoryName(Path.GetFullPath(programPath));
        var programCommand = $"\"{programPath}\" %1";
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

        // Set default icon for the .bat
        SetDefalaultIconOf7zip_extract_and_open_bat();

        // target extensions List
        // commented out i don't know what it is or think it should not be in
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
            // create the extension class if it does'nt exist
            CreateClassIfNotExists(extension);

            var iconPath = extensionAndPathes[extension];
            if (!IsExtensionIconAssociated(extension, iconPath))
            {
                // assosiate with the extension and the icon
                string progId = GetExtensionUserChoiceProgId(extension);

                // backup the registry keys to .reg
                int ret = BackupRegistry(progId);
                // When error occurred
                if (ret != 0)
                {
                    Console.WriteLine($"ERROR: Cannot export the registry key.\n{progId}");
                    Console.ReadKey();
                    return 1;
                }

                AssociateExtensionIcon(progId, iconPath);
                if(!HasSetExtensionCommand(progId) || IsSet_7zip_extract_and_open_bat(progId))
                {
                    AssociateExtensionCommand(progId, programCommand);
                }
            }
        }

        // pause before exit
        Console.WriteLine("\nProcess is completed.");
        Console.ReadKey();
        return 0;

        #endregion
        #endregion
        #region functions

        /// <summary>
        /// set default icon for 7-zip_extract_and_open.bat
        /// </summary>
        void SetDefalaultIconOf7zip_extract_and_open_bat()
        {
            using (RegistryKey key = Registry.CurrentUser.OpenSubKey($"Software\\Classes\\Applications\\{programName}\\{DEFAULT_ICON}", true))
            {
                if(key != null)
                {
                    key.SetValue("", Path.Combine(programDir!, iconDir, "icon.ico"));
                } else
                {
                    Registry.CurrentUser.CreateSubKey($"Software\\Classes\\Applications\\{programName}\\{DEFAULT_ICON}");
                    using (RegistryKey newKey = Registry.CurrentUser.OpenSubKey($"Software\\Classes\\Applications\\{programName}", true))
                    {
                        if (newKey != null)
                        {
                            newKey.SetValue("", Path.Combine(programDir!, iconDir, "icon.ico"));
                        }
                    }
                }
            }
        }

        /// <summary>
        /// create an extension class if it does'nt exist
        /// </summary>
        void CreateClassIfNotExists(string extension)
        {
            using (RegistryKey key = Registry.CurrentUser.OpenSubKey($"Software\\Classes\\{extension}"))
            {
                if (key is null)
                {
                    using (RegistryKey newKey = Registry.CurrentUser.CreateSubKey($"Software\\Classes\\{extension}"))
                    {
                        newKey.SetValue("", Get_extension_auto_file(extension));
                    }
                }
            }
        }

        /// <summary>
        /// check default icon is associated or not
        /// </summary>
        bool IsExtensionIconAssociated(string extension, string iconPath)
        {
            // Check extension is associated the program
            var currentDefaultIconClass = GetExtensionUserChoiceProgId(extension);

            return GetDefaultIconPath(currentDefaultIconClass) == iconPath;
        }

        /// <summary>
        /// get associated progId
        /// </summary>
        string GetExtensionUserChoiceProgId(string extension)
        {
            // get associated value tha user choiced.
            // if it exists and named tar_auto_file,
            // then the target is HKEY_CURRENT_USER\Software\Classes\tar_auto_file
            // else it is named 7zip.zip then
            // the target is HKEY_CURRENT_USER\Software\Classes\7zip.zip
            // but, 
            // HKEY_CURRENT_USER\SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\FileExts\.xz\UserChoice
            // key value is "Application\7-zip_extract_and_open.bat" then
            // the target is HKEY_CURRENT_USER\Software\Classes\xz_auto_file and It should be created if it does'nt exist
            using (RegistryKey key = 
                    Registry.CurrentUser.OpenSubKey($"SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Explorer\\FileExts\\{extension}\\UserChoice"))
            {
                string? progId = "";
                string extension_auto_file = Get_extension_auto_file(extension);
                if (key is not null)
                {
                    progId = (string)key.GetValue("ProgID");

                    if (progId is not null && progId.StartsWith("Applications\\7-zip_extract_and_open"))
                    {
                        progId = extension_auto_file;
                    } else if (progId is null) {
                        // create {extension}_auto_file
                        return extension_auto_file;
                    } else {
                        // if it has been set something already
                        return progId;
                    }
                }

                // create {extension}_auto_file
                return extension_auto_file;
            }
        }

        /// <summary>
        /// get default icon path
        /// </summary>
        string GetDefaultIconPath(string progId)
        {
            // Get associated value
            using (RegistryKey key = Registry.CurrentUser.OpenSubKey($"Software\\Classes\\{progId}"))
            {
                if (key is not null)
                {
                    var ret = (string)key.GetValue(DEFAULT_ICON);
                    if (ret is null) return "";
                    return ret;
                }
            }
            return "";
        }

        /// <summary>
        /// associate the extension with the icon
        /// </summary>
        void AssociateExtensionIcon(string extensionOrProgId, string iconPath)
        {
            using (RegistryKey key = Registry.CurrentUser.CreateSubKey("Software\\Classes").CreateSubKey(extensionOrProgId + $"\\{DEFAULT_ICON}"))
            {
                key.SetValue("", iconPath);
            }
        }
        /// <summary>
        /// associate the extension with the icon
        /// </summary>
        void AssociateExtensionCommand(string progId, string command)
        {
            using (RegistryKey key = Registry.CurrentUser.CreateSubKey($"Software\\Classes\\{progId}\\shell\\open\\command"))
            {
                key.SetValue("", command);
            }
        }

        /// <summary>
        /// check the extension open command has been set
        /// </summary>
        bool HasSetExtensionCommand(string progId)
        {
            using (RegistryKey key = Registry.CurrentUser.OpenSubKey($"Software\\Classes\\{progId}\\shell\\open\\command"))
            {
                if(key is not null)
                {
                    object? v = key.GetValue("");
                    return v is not null && (string) v != "";
                }
                return false;
            }
        }

        /// <summary>
        /// check the extension open command is 7-zip_extract_and_open.bat
        /// </summary>
        bool IsSet_7zip_extract_and_open_bat(string progId)
        {
            using (RegistryKey key = Registry.CurrentUser.OpenSubKey($"Software\\Classes\\{progId}\\shell\\open\\command"))
            {
                if (key is not null)
                {
                    object? v = key.GetValue("");
                    return v is not null && ((string) v).EndsWith(programName);
                }
                return false;
            }
        }

        /// <summary>
        /// backup the registory keys to .reg
        /// </summary>
        int BackupRegistry(string progId)
        {
            // backup from the extension key
            string classesRegistryPath = "HKEY_CURRENT_USER\\SOFTWARE\\Classes";
            string registryKeyPath = $"{classesRegistryPath}\\{progId}";

            if (Registry.CurrentUser.OpenSubKey($"SOFTWARE\\Classes\\{progId}") == null) {
                Console.WriteLine($"{registryKeyPath} does not exist in registry. then it will be passed.");
                return 0;
            }

            // add datetime in filename
            var backupFileName = DateTime.Now.ToString("yyyyMMddHHmmss") + "_" + Path.GetFileName(progId) + ".reg";
            var backupFileDir = Path.Combine(programDir!, "backup");
            if (!Directory.Exists(backupFileDir)) Directory.CreateDirectory(backupFileDir);

#pragma warning disable CS8604
                var backupFilePath = Path.Combine(backupFileDir, backupFileName);
#pragma warning restore CS8604

            try
            {
                using (Process process = new Process())
                {
                    process.StartInfo.FileName = "regedit.exe";
                    Console.WriteLine($"/e \"{backupFilePath}\" \"{registryKeyPath}\\shell\\open\\command\"");
                    process.StartInfo.Arguments = $"/e \"{backupFilePath}\" \"{registryKeyPath}\"";
                    process.Start();
                    process.WaitForExit();
                    if (process.ExitCode != 0)
                    {
                        throw new IOException("Backup failed");
                    }
                    if (!File.Exists(backupFilePath))
                    {
                        Console.WriteLine("ERROR: Cannot export registry. " + registryKeyPath);
                    }
                    else
                    {
                        Console.WriteLine($"{progId} backup file was created at {backupFilePath}");
                    }
                }
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
        string Get_extension_auto_file(string extension) => extension.Replace(".", "") + "_auto_file";
        #endregion
    }
}
