using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Path = System.IO.Path;

namespace Flarebook.pages
{
    /// <summary>
    /// clean.xaml 的交互逻辑
    /// </summary>
    public partial class clean : Page
    {
        private List<InstalledProgram> installedPrograms;

        public clean()
        {
            InitializeComponent();
            installedPrograms = new List<InstalledProgram>();
        }

        private void ScanButton_Click(object sender, RoutedEventArgs e)
        {
            ProgramList.Items.Clear();
            installedPrograms = GetInstalledPrograms();
            foreach (var program in installedPrograms)
            {
                ProgramList.Items.Add(program);
            }
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            string searchText = SearchBox.Text.ToLower();
            var filteredPrograms = installedPrograms
                .Where(p => p.Name.ToLower().Contains(searchText))
                .ToList();

            ProgramList.Items.Clear();
            foreach (var program in filteredPrograms)
            {
                ProgramList.Items.Add(program);
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            foreach (var item in ProgramList.SelectedItems)
            {
                var program = (InstalledProgram)item;
                UninstallProgram(program);
                RemoveRegistryEntry(program.RegistryKeyPath);
                RemoveProgramFiles(program);
            }
            MessageBox.Show("Selected programs deleted successfully.");
        }

        private List<InstalledProgram> GetInstalledPrograms()
        {
            var programs = new List<InstalledProgram>();
            programs.AddRange(GetInstalledProgramsFromRegistry());
            programs.AddRange(GetInstalledProgramsFromFileSystem());

            // 去重
            return programs.GroupBy(p => p.Name).Select(g => g.First()).ToList();
        }

        private List<InstalledProgram> GetInstalledProgramsFromRegistry()
        {
            var programs = new List<InstalledProgram>();
            string[] registryKeys = new string[]
            {
                @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall",
                @"SOFTWARE\Wow6432Node\Microsoft\Windows\CurrentVersion\Uninstall"
            };

            foreach (var registryKey in registryKeys)
            {
                using (RegistryKey key = Registry.LocalMachine.OpenSubKey(registryKey))
                {
                    if (key == null) continue;

                    foreach (var subkeyName in key.GetSubKeyNames())
                    {
                        using (RegistryKey subkey = key.OpenSubKey(subkeyName))
                        {
                            var displayName = subkey?.GetValue("DisplayName") as string;
                            var uninstallString = subkey?.GetValue("UninstallString") as string;
                            var installLocation = subkey?.GetValue("InstallLocation") as string;
                            if (!string.IsNullOrEmpty(displayName) && !string.IsNullOrEmpty(uninstallString))
                            {
                                programs.Add(new InstalledProgram
                                {
                                    Name = displayName,
                                    UninstallString = uninstallString,
                                    RegistryKeyPath = $"{registryKey}\\{subkeyName}",
                                    InstallLocation = installLocation
                                });
                            }
                        }
                    }
                }
            }
            return programs;
        }

        private List<InstalledProgram> GetInstalledProgramsFromFileSystem()
        {
            var programs = new List<InstalledProgram>();
            string[] programDirectories = new string[]
            {
                @"C:\Program Files",
                @"C:\Program Files (x86)"
            };

            foreach (var directory in programDirectories)
            {
                if (Directory.Exists(directory))
                {
                    var subDirectories = Directory.GetDirectories(directory);
                    foreach (var subDirectory in subDirectories)
                    {
                        var programName = Path.GetFileName(subDirectory);
                        if (!string.IsNullOrEmpty(programName))
                        {
                            programs.Add(new InstalledProgram
                            {
                                
                                Name = programName,
                                InstallLocation = subDirectory
                            });
                        }
                    }
                }
            }
            return programs;
        }

        private void UninstallProgram(InstalledProgram program)
        {
            try
            {
                if (!string.IsNullOrEmpty(program.UninstallString))
                {
                    Process.Start("cmd.exe", $"/C {program.UninstallString}");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to uninstall {program.Name}: {ex.Message}");
            }
        }

        private void RemoveRegistryEntry(string registryKeyPath)
        {
            try
            {
                if (!string.IsNullOrEmpty(registryKeyPath))
                {
                    using (RegistryKey key = Registry.LocalMachine.OpenSubKey(registryKeyPath, true))
                    {
                        if (key != null)
                        {
                            Registry.LocalMachine.DeleteSubKeyTree(registryKeyPath);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to remove registry entry: {ex.Message}");
            }
        }

        private void RemoveProgramFiles(InstalledProgram program)
        {
            try
            {
                if (!string.IsNullOrEmpty(program.InstallLocation) && Directory.Exists(program.InstallLocation))
                {
                    Directory.Delete(program.InstallLocation, true);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to remove program files: {ex.Message}");
            }
        }
    }

    public class InstalledProgram
    {
        public string Name { get; set; }
        public string UninstallString { get; set; }
        public string RegistryKeyPath { get; set; }
        public string InstallLocation { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}

