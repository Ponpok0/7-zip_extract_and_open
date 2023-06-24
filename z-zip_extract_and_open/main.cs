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

        // target extensions List
        // key: extension, value: icon path
        var targetExtensions = new List<string> {
            ".7z",
        //    ".ar",
        //    ".arj",
        //    ".bzip2",
        //    ".cab",
        //    ".chm",
        //    ".cpio",
        //    ".cramfs",
        //    ".dmg",
        //    ".ext",
        //    ".fat",
        //    ".gpt",
              ".gzip",
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
        //    ".wim",
        //    ".xar",
        //    ".xz",
        //    ".z",
              ".zip"
        };
        Dictionary<string,string> extensionAndPathes= new Dictionary<string,string>();
        foreach(var extension in targetExtensions)
        {
            extensionAndPathes.Add(extension, Path.Combine(programDir!, iconDir, GetIconName(extension)));
        }

        // main process
        foreach(var extension in extensionAndPathes.Keys)
        {
            if (!IsExtensionAssociated(extension, programPath))
            {
                // backup the registory keys to .reg
                int ret = BackupRegistory(extension);
                // When error occurred
                if(ret != 0) return 1;

                // assosiate with the extension and the icon
                var iconPath = extensionAndPathes[extension];
                AssociateExtension(extension, programPath, iconPath);
            }
        }

        // pause before exit
        Console.WriteLine("Process is completed.");
        Console.ReadKey();

        return 0;

        #endregion
        #region functions
        static string GetExecutablePath()
        {
            var entryAssembly = Assembly.GetEntryAssembly();
            if (entryAssembly != null)
            {
                return entryAssembly.Location;
            }

            var uri = new UriBuilder(Assembly.GetExecutingAssembly().CodeBase);
            return Uri.UnescapeDataString(uri.Path);
        }
        /// <summary>
        /// Check the file is associated or not
        /// </summary>
        bool IsExtensionAssociated(string extension, string programPath)
        {
            // Get associated value
            using (RegistryKey key = Registry.CurrentUser.OpenSubKey("Software\\Classes\\" + extension + "\\shell\\open\\command\\"))
            {
                string currentKeyValue = "";
                if (key is not null)
                {
                    currentKeyValue = (string) key.GetValue("");
                }

                // Check extension is associated the program
                return currentKeyValue == $"\"{programPath}\" \"%1\"";
            }
        }

        /// <summary>
        /// associate the extension with the icon
        /// </summary>
        void AssociateExtension(string extension, string programPath, string iconPath)
        {
            using (RegistryKey key = Registry.CurrentUser.CreateSubKey("Software\\Classes").CreateSubKey(extension + "\\DefaultIcon"))
            {
                key.SetValue("", iconPath);
            }

            //creating HKEY_CURRENT_USER\Software\Classes\{extension}\shell\open\command
            using (RegistryKey key = Registry.CurrentUser.CreateSubKey("Software\\Classes").CreateSubKey(extension + "\\shell\\open\\command"))
            {
                key.SetValue("", $"\"{programPath}\" \"%1\"");
            }
        }

        /// <summary>
        /// backup the registory keys to .reg
        /// </summary>
        int BackupRegistory(string extension)
        {
            // backup from the extension key
            string registryKeyPath = $"HKEY_CURRENT_USER\\SOFTWARE\\Classes\\{extension}";

            // add datetime in filename
            var backupFileName = DateTime.Now.ToString("yyyyMMddHHmmss") + extension + ".reg";
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
                Console.WriteLine($"{extension} backup file was created. {backupFilePath}");
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
