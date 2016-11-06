using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using WixSharp;
using WixSharp.CommonTasks;
using WixSharp.Controls;
using File = WixSharp.File;

namespace Sklad.Installer
{
    class Program
    {
        static void Main(string[] args)
        {
            var configuration = Environment.GetEnvironmentVariable("CONFIGURATION") ?? "Debug";
            var version = Environment.GetEnvironmentVariable("APPVEYOR_BUILD_VERSION") ?? "0.0.1";
            var project = new Project
            {
                Version = new Version(version),
                Name = "Sklad Desktop",
                GUID = new Guid("1EDF5D89-5BB2-4A12-B8FD-9DAA9F363639"),
                Language = "ru-RU",
                SourceBaseDir = @"..\Warehouse.Wpf\bin\" + configuration,
                Dirs = new[]
                {
                    new Dir(@"%ProgramFiles%\Sklad",
                        new DirFiles("*.dll"),
                        new File("Warehouse.Wpf.exe",
                            new FileShortcut("Sklad", @"%Desktop%"),
                            new FileShortcut("Sklad", @"%ProgramMenu%")
                        ),
                        new File("Warehouse.Wpf.exe.config")
                    )
                },
                OutFileName = "Sklad",
                PreserveTempFiles = true,
                ControlPanelInfo =
                {
                    Manufacturer = "Skill",
                    ProductIcon = @"..\Warehouse.Wpf\Icon.ico",
                },
                UI = WUI.WixUI_InstallDir,
                BannerImage = Path.Combine(Environment.CurrentDirectory, "bannrbmp.bmp"),
                BackgroundImage = Path.Combine(Environment.CurrentDirectory, "dlgbmp.bmp"),
                MajorUpgrade = new MajorUpgrade
                {
                    //AllowSameVersionUpgrades = true, //uncomment this if the the upgrade version is different by only the fourth field
                    Schedule = UpgradeSchedule.afterInstallInitialize,
                    DowngradeErrorMessage = "Более новая версия [ProductName] уже установлена."
                },
                Encoding = Encoding.UTF8,
                LocalizationFile = Path.Combine(Environment.CurrentDirectory, "ru-ru.wxl"),
            };

            project.RemoveDialogsBetween(NativeDialogs.WelcomeDlg, NativeDialogs.InstallDirDlg);

            project.SetNetFxPrerequisite("NETFRAMEWORK45", "Пожалуйста установите\nMicrosoft .NET Framework 4.5");

            project.IncludeWixExtension(WixExtension.Util);
            project.WixSourceGenerated += Compiler_WixSourceGenerated;

            project.BuildMsi();
        }

        static void Compiler_WixSourceGenerated(XDocument document)
        {
            var file = document.FindAll("File").FirstOrDefault(
                x => x.HasAttribute("Id", value => value.EndsWith("Warehouse.Wpf.exe.config"))
            );

            var baseAddress = Environment.GetEnvironmentVariable("BaseAddress") ?? "http://localhost:63270";

            file?.Parent?.Add(
                new XElement(WixExtension.Util.ToXNamespace() + "XmlFile",
                    new XAttribute("Id", (string)file.Attribute("Id") + ".XmlFile"),
                    new XAttribute("File", "[#" + (string)file.Attribute("Id") + "]"),
                    new XAttribute("Action", "setValue"),
                    new XAttribute("ElementPath", @"//appSettings/add[\[]@key='BaseAddress'[\]]/@value"),
                    new XAttribute("Value", baseAddress)
                )
            );
        }
    }
}
