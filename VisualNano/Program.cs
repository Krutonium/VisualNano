﻿using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.AccessControl;
using System.Text;
using Atk;
using Eto.Drawing;
using Eto.Forms;
using Gtk;
using Application = Eto.Forms.Application;
using MenuBar = Eto.Forms.MenuBar;
using Newtonsoft.Json;
using AboutDialog = Eto.Forms.AboutDialog;
using Dialog = Eto.Forms.Dialog;

namespace VisualNano
{
    internal class Program
    {
        private static RichTextArea RTB;
        private static SaveFileDialog saveDialog;
        private static Options options;
        private static string toOpen = null;
        [STAThread]
        public static void Main(string[] args)
        {
            if (args.Length > 0)
            {
                toOpen = args[0];
            }
            options = new Options();
            string ConfigPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) +
                                @"/visualnano/config.json";
            Console.WriteLine(Path.GetDirectoryName(ConfigPath));

            if (Directory.Exists(Path.GetDirectoryName(ConfigPath)) == false)
            {
                Directory.CreateDirectory(Path.GetDirectoryName(ConfigPath));
                Console.WriteLine("Created Config Directory");
            }
            
            try
            {
                options = JsonConvert.DeserializeObject<Options>(File.ReadAllText(ConfigPath)); 
                Console.WriteLine("Loaded Config File");
            }
            catch
            {
                options.Width = 800;
                options.Height = 600;
                options.encoding = "UTF-8";
                SaveConfig(options);
                Console.WriteLine("Created Config File");
            }
            //Above code was setting up our config file.
            //It's stored in .config or %appdata% depending on your OS.
            new Eto.Forms.Application().Run(new TestForm());
        }

        public static void SaveConfig(Options config)
        {
            string ConfigPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) +
                                @"/visualnano/config.json";
            File.WriteAllText(ConfigPath, JsonConvert.SerializeObject(options, Formatting.Indented));
        }
        class TestForm : Eto.Forms.Form
        {
            
            public TestForm()
            {
                saveDialog = new SaveFileDialog();
                RTB  = new RichTextArea();
                //Init our Things ^
                
                //TODO: Put everything in tabs
                
                // sets the client (inner) size of the window for your content
                this.ClientSize = new Eto.Drawing.Size(options.Width, options.Height);
                this.Content = RTB;
                
                //Load last document even if it wasn't saved
                if (File.Exists(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) +
                                "/visualnano/~lastdocument"))
                {
                    RTB.Text = File.ReadAllText(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) +
                               "/visualnano/~lastdocument");
                }

                RTB.TextChanged += (sender, args) => File.WriteAllText((Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) +
                                                                       "/visualnano/~lastdocument"), RTB.Text);
                if (toOpen == null == false)
                {
                    RTB.Text = File.ReadAllText(toOpen);
                    saveDialog.FileName = toOpen;
                }//Load command line specified file, if one exists.
                
                this.Title = "Visual Nano";

                var FileMenu = new ButtonMenuItem 
                {
                    Text = "File",
                    Items =
                    {
                        // you can add commands or menu items
                        new NewDocument(),
                        new Open(),
                        new Save(),
                        new About(),
                        new Quit(),
                    }
                };
                var EditMenu = new ButtonMenuItem
                {
                    Text = "Edit",
                    Items =
                    {
                        new FontChooser()
                    }
                };
                
                Menu = new MenuBar
                {
                    Items =
                    {
                       FileMenu,
                       EditMenu
                    }
                };
            }
        }

        public class About : Command
        {
            public About()
            {
                MenuText = "About";
                ToolBarText = "About";
                ToolTip = "About this application";
            }

            protected override void OnExecuted(EventArgs e)
            {
                base.OnExecuted(e);
                AboutDialog about = new AboutDialog();
                about.Title = "About";
                about.License = "This program uses components that are licensed MIT" + Environment.NewLine +
                                "This program uses components that are licensed BSD-3" + Environment.NewLine +
                                "This Project is licensed under MIT as a result.";
                about.Copyright = "Copyright This Programs Authors";
                about.ProgramName = "Visual Nano";
                about.Version = "Version 18.12.26"; //Version is Date - 2018/12/26 becomes 18.12.26
                about.Website = new Uri("https://github.com/Krutonium/VisualNano");
                
                var Authors = new string[] {"Krutonium"}; //Add your name here if you contribute!
                about.Developers = Authors;
                about.ShowDialog(Application.Instance.MainForm);
            }
        }

        public class FontChooser : Command
        {
            public FontChooser()
            {
                MenuText = "Font";
                ToolBarText = "Font Selection";
                ToolTip = "Select a font";
            }

            protected override void OnExecuted(EventArgs e)
            {
                base.OnExecuted(e);
                var FontSelector = new FontDialog();
                var result = FontSelector.ShowDialog(Application.Instance.MainForm);
                if (result == DialogResult.Ok)
                {
                    RTB.Font = FontSelector.Font;
                    //options.font = FontSelector.Font;
                    //SaveConfig(options);
                    //Crashes for some reason
                    //Seems to be a lbrary bug.
                }
            }
        }
        public class NewDocument : Command
        {
            public NewDocument()
            {
                //TODO: Prompt user to Save.
                MenuText = "New Document";
                ToolBarText = "New Document";
                ToolTip = "Creates a new Document";
                Shortcut = Application.Instance.CommonModifier | Keys.N; 
            }

            protected override void OnExecuted(EventArgs e)
            {
                base.OnExecuted(e);
                RTB.Text = "";
            }
        }

        public class Open : Command
        {
            public Open()
            {
                //TODO: Prompt User to save.
                MenuText = "Open File";
                ToolBarText = "Open File";
                ToolTip = "Opens a file";
            }

            protected override void OnExecuted(EventArgs e)
            {
                base.OnExecuted(e);
                var ofd = new OpenFileDialog();
                var result = ofd.ShowDialog(Application.Instance.MainForm);
                if (result == DialogResult.Ok)
                {
                    RTB.Text = File.ReadAllText(ofd.FileName);
                    saveDialog.FileName = ofd.FileName;
                }
            }
        }

        public class Quit : Command
        {
            public Quit()
            {
                //TODO: Prompt user to save.
                MenuText = "Quit";
                ToolBarText = "Quit";
                Shortcut = Application.Instance.CommonModifier | Keys.X;
            } 
            protected override void OnExecuted(EventArgs e)
            {
                base.OnExecuted(e);
                Environment.Exit(0);
            }
        }

        public class Save : Command
        {
            public Save()
            {
                MenuText = "Save";
                ToolBarText = "Save";
                Shortcut = Application.Instance.CommonModifier | Keys.O;
            }

            protected override void OnExecuted(EventArgs e)
            {
                base.OnExecuted(e);
                if (saveDialog.FileName != null)
                {
                    File.WriteAllText(saveDialog.FileName, RTB.Text, Encoding.Unicode);
                }
                else
                {
                    saveDialog.Title = "Save File...";
                    var result = saveDialog.ShowDialog(Application.Instance.MainForm);
                    var encoding = encodingFinder(options.encoding); //Get format specified in options.
                    if (result == DialogResult.Ok)
                    {
                        if (File.Exists(saveDialog.FileName)) //If the file already exists, we should preserve the existing formatting instead.
                        {
                            encoding = GetEncoding(saveDialog.FileName);
                        }
                        File.WriteAllText(saveDialog.FileName, RTB.Text, encoding);
                        //TODO: Some sort of notification that save has completed?
                    }
                }
            }
        }
        private static Encoding GetEncoding(string filename)
        {
            // Read the BOM
            var bom = new byte[4];
            using (var file = new FileStream(filename, FileMode.Open, FileAccess.Read))
            {
                file.Read(bom, 0, 4);
            }

            // Analyze the BOM
            if (bom[0] == 0x2b && bom[1] == 0x2f && bom[2] == 0x76) return Encoding.UTF7;
            if (bom[0] == 0xef && bom[1] == 0xbb && bom[2] == 0xbf) return Encoding.UTF8;
            if (bom[0] == 0xff && bom[1] == 0xfe) return Encoding.Unicode; //UTF-16LE
            if (bom[0] == 0xfe && bom[1] == 0xff) return Encoding.BigEndianUnicode; //UTF-16BE
            if (bom[0] == 0 && bom[1] == 0 && bom[2] == 0xfe && bom[3] == 0xff) return Encoding.UTF32;
            return encodingFinder(options.encoding);
        }

        internal class Options
        {
            public string encoding;
            public int Height;
            public int Width;
            public Font font;
        }

        private static Encoding encodingFinder(string encoding)
        {
            encoding = encoding.ToUpper();
            switch (encoding)
            {
                case "UTF-8":
                    return Encoding.UTF8;    
                case "UTF-32":
                    return Encoding.UTF32;
                case "UTF-7":
                    return Encoding.UTF7;
                case "UNICODE":
                    return Encoding.Unicode;
                case "BIGENDIANUNICODE":
                    return Encoding.BigEndianUnicode;
                case "ASCII":
                    return Encoding.ASCII;
            }
            return Encoding.Default; //System Default format, if the user specifies an invalid format.
        }
    }
}