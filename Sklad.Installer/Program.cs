using System;
using System.IO;
using WixSharp;

namespace Sklad.Installer
{
    class Program
    {
        static void Main(string[] args)
        {
            var configuration = Environment.GetEnvironmentVariable("CONFIGURATION") ?? "Debug";
            var project = new Project
            {
                Name = "Sklad Desktop",
                GUID = new Guid("1EDF5D89-5BB2-4A12-B8FD-9DAA9F363639"),
                UpgradeCode = new Guid("E1E7E6A8-D96D-4D26-926E-8F638B9A3992"),
                Language = "ru-RU",
                SourceBaseDir = @"..\Warehouse.Wpf\bin\" + configuration,
                Dirs = new[]
                {
                    new Dir(@"%ProgramFiles%\Sklad",
                        new DirFiles("*.dll")
                    )
                },
                OutFileName = "Sklad",
                PreserveTempFiles = true,
                ControlPanelInfo =
                {
                    Manufacturer = "Skill",
                    ProductIcon = @"..\Warehouse.Wpf\Icon.ico",
                },
                
            };

            project.ResolveWildCards();

            project.BuildMsi();
        }
    }
}
