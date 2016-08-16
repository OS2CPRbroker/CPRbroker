﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Deployment.WindowsInstaller;
using Microsoft.Deployment.WindowsInstaller.Linq;
using Microsoft.Deployment.WindowsInstaller.Linq.Entities;
using System.Reflection;
using NUnit.Framework;
using System.IO;

namespace CprBroker.Tests.WixInstaller
{
    [TestFixture]
    public class IntegrityTests
    {
        static readonly string SolutionDir = new DirectoryInfo(@"..\..\..\").FullName;

        [Test]
        public void CustomActions_MethodAvailable()
        {
            var path = SolutionDir + @"WixInstaller\bin\Debug\en-US\CprBroker.msi";
            using (var database = new Database(path))
            {
                foreach (CustomAction_ ca in database.AsQueryable().CustomActions)
                {
                    if ((ca.Type & CustomActionTypes.Dll) > 0
                        && ca.Source.Contains("InstallersDll"))
                    {
                        Console.WriteLine("{0}: {1}.{2}", ca.Type.ToString().PadLeft(45), ca.Source, ca.Target);
                        var exists = Exists(ca);
                        Assert.True(exists);
                    }
                }
            }
        }

        public bool Exists(CustomAction_ ca)
        {
            var path = ca.Source == "InstallersDll" ? SolutionDir + @"Output\CprBroker.CustomActions.dll" :
                ca.Source.EndsWith("InstallersDll") ?
                SolutionDir + @"..\EventBroker\Output\CprBroker.Installers.EventBrokerInstallers.dll" :
                "";

            if (string.IsNullOrEmpty(path))
                throw new Exception();

            var asm = Assembly.LoadFile(path);

            if (path.Contains("Event"))
            {
                var asm0 = Assembly.LoadFrom("CprBroker.Installers.dll");
                var methods0 = asm0.GetTypes().SelectMany(t => t.GetMethods()).ToArray();
            }
            var methods = asm.GetTypes().SelectMany(t => t.GetMethods()).ToArray();
            foreach (var method in methods)
            {
                ParameterInfo[] pars = null;
                if (method.GetCustomAttribute(typeof(CustomActionAttribute)) != null)
                {
                    try
                    {
                        pars = method.GetParameters();
                    }
                    catch (Exception ex)
                    {
                        throw;
                    }
                    if (method.Name == ca.Target &&
                            pars.Count() == 1 &&
                            pars.First().ParameterType == typeof(Session)
                            )
                        return true;
                }
            }
            return false;
        }
    }
}
