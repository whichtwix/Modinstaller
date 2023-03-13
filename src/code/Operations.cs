using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Diagnostics;

namespace Modinstaller
{
    public sealed class ActionsFromChoice
    {
        public static async Task InstallByUser()
        {
            Inputs.Setfolderpaths(out string Basepath, out string Destinationpath);
            string mod = Inputs.ChooseFromChoice(Constants.Mods.Keys, "Select which mod you want to install:");

            await ModZip.Install(Basepath, Destinationpath, mod);
        }

        public static async Task InstallByJson()
        {
            if (!File.Exists(Constants.Jsonpath))
            {
                Console.WriteLine("Presets file does not exist");
                return;
            }

            var presets = Presetfile.GetPresets();
            List<string> items = Presetfile.GetPresets().ConvertAll(x => x.Mod);

            //check if multiple destination files are the same, or base folders and destinations are empty
            if ((!Presetfile.ClashingPaths(presets, true) && Presetfile.AllEmptyDestinations(presets)) ||
                 !Presetfile.ClashingPaths(presets, false))
            {
                items.Add("All presets");
            }

            string mod = Inputs.ChooseFromChoice(items, "Select which mod you want to install:");

            if (mod == "All presets")
            {
                await Parallel.ForEachAsync(presets, async (preset, _) => await ModZip.Install(preset.BaseFolder, preset.DestinationFolder, preset.Mod));
            }
            else
            {
                var preset = presets.Find(x => x.Mod == mod);
                await ModZip.Install(preset.BaseFolder, preset.DestinationFolder, preset.Mod);
            }
        }

        public static async Task AddToJson()
        {
            Console.WriteLine("WARNING: only 1 preset per mod is allowed and selecting again will override the existing one");
            Inputs.Setfolderpaths(out string Basepath, out string Destinationpath);
            string mod = Inputs.ChooseFromChoice(Constants.Mods.Keys, "Select which mod you want to install at the folder:");

            PresetsJson preset = new()
            {
                Mod = mod,
                BaseFolder = Basepath,
                DestinationFolder = Destinationpath
            };

            if (!File.Exists(Constants.Jsonpath))
            {
                Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\\Modinstaller");
                List<PresetsJson> presets = new() { preset };
                string serial = JsonSerializer.Serialize(presets, Constants.opts);
                await File.WriteAllTextAsync(Constants.Jsonpath, serial);
                return;
            }
            else
            {
                var currentfile = Presetfile.GetPresets();
                currentfile = currentfile.FindAll(x => x.Mod != preset.Mod);
                currentfile.Add(preset);
                string serial = JsonSerializer.Serialize(currentfile, Constants.opts);
                await File.WriteAllTextAsync(Constants.Jsonpath, serial);
                return;
            }
        }

        public static async Task RemoveFromJson()
        {
            if (!File.Exists(Constants.Jsonpath))
            {
                Console.WriteLine("Presets file does not exist");
                return;
            }

            var presets = Presetfile.GetPresets();
            var list = Presetfile.GetPresets().ConvertAll(x => x.Mod);
            list.Add("Cancel");
            string mod = Inputs.ChooseFromChoice(list, "Choose the mod you want to remove:");

            if (mod == "Cancel") return;

            if (mod != "Cancel" && presets.Count == 1)
            {
                File.Delete(Constants.Jsonpath);
                return;
            }

            presets = presets.FindAll(x => x.Mod != mod);
            string serial = JsonSerializer.Serialize(presets, Constants.opts);
            await File.WriteAllTextAsync(Constants.Jsonpath, serial);
        }

        public static async Task ViewPresets()
        {
            if (!File.Exists(Constants.Jsonpath))
            {
                Console.WriteLine("Presets file does not exist");
                return;
            }

            Process.Start("notepad.exe", Constants.Jsonpath);
        }
    }
}