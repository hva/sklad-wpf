using System;
using WixSharp;

namespace Sklad.Installer
{
    class Program
    {
        static void Main(string[] args)
        {
            var project = new Project("MyProduct",
                                      new Dir(@"%ProgramFiles%\My Company\My Product",
                                          new File("packages.config")));

            project.GUID = new Guid("6f330b47-2577-43ad-9095-1861ba25889b");

            Compiler.BuildMsi(project);
        }
    }
}
