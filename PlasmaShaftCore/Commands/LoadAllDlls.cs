using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;

namespace PlasmaShaft
{
    public static class LoadAllDlls
    {
        public static void Init()
        {
            Server.Log("Initializing Commands", LogMessage.INFO);
            InitCommands();

        }
        public static Assembly LoadFile(string file)
        {
            try
            {
                Assembly lib = null;
                using (FileStream fs = File.Open(file, FileMode.Open))
                {
                    using (MemoryStream ms = new MemoryStream())
                    {
                        byte[] buffer = new byte[1024];
                        int read = 0;
                        while ((read = fs.Read(buffer, 0, 1024)) > 0)
                            ms.Write(buffer, 0, read);
                        lib = Assembly.Load(ms.ToArray());
                        ms.Close();
                        ms.Dispose();
                    }
                    fs.Close();
                    fs.Dispose();
                }
                return lib;
            }
            catch { return null; }
        }
        /// <summary>
        /// Load a DLL
        /// </summary>
        /// <param name="s">The filepath of the DLL</param>
        /// <param name="args">The args to passed to the plugin OnLoad method.</param>
        public static void LoadDLL(string s, string[] args)
        {
            Assembly DLLAssembly = LoadFile(s); //Prevents the dll from being in use inside windows
            try
            {
                foreach (Type ClassType in DLLAssembly.GetTypes())
                {
                    if (ClassType.IsPublic)
                    {
                        if (!ClassType.IsAbstract)
                        {
                            Type typeInterface = ClassType.GetInterface("ICommand", true);
                            if (typeInterface != null)
                            {

                                ICommand instance = (ICommand)Activator.CreateInstance(DLLAssembly.GetType(ClassType.ToString()));
                                instance.Initialize();
                                Server.Log("[Command]: " + instance.Name + " Initialized!", LogMessage.INFO);
                            }
                        }
                    }
                }
            }
            catch
            {
            } //Stops loading bad DLL files
        }
        private static void LoadCommand(Type classType, Assembly DLLAssembly)
        {
                ICommand instance = (ICommand)Activator.CreateInstance(DLLAssembly.GetType(classType.ToString()));
                instance.Initialize();
                Server.Log("[Command]: " + instance.Name + " Initialized!", LogMessage.INFO);
        }
        public static void LoadDLL(string s, string[] args, Type i = null)
        {
            Assembly DLLAssembly = LoadFile(s); //Prevents the dll from being in use inside windows
            try
            {
                foreach (Type ClassType in DLLAssembly.GetTypes())
                {
                    if (ClassType.IsPublic)
                    {
                        if (!ClassType.IsAbstract)
                        {
                            if (i != null && i.Name == "ICommand")
                            {
                                LoadCommand(ClassType, DLLAssembly);
                            }
                            else
                            {
                                if (ClassType == typeof(ICommand))
                                    LoadCommand(ClassType, DLLAssembly);
                            }
                        }
                    }
                }
            }
            catch { } //Stops loading bad DLL files
        }

        public static void InitCommands()
        {
            string path = Directory.GetCurrentDirectory();
            string[] DLLFiles = Directory.GetFiles(path, "*.dll");
            foreach (string s in DLLFiles)
                LoadDLL(s, new string[] { "-normal" });
        }

    }
}