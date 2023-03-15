using System;
using System.IO;
using Spectre.Console;
using System.Text.Json;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Modinstaller
{
    public sealed class ActionsFromChoice
    {
        public static async Task InstallByUser()
        {
            Inputs.Setfolderpaths(out string Basepath, out string Destinationpath);
            List<string> items = new(Constants.Mods.Keys)
            {
                "Cancel"
            };
            string mod = Inputs.ChooseFromChoice(items, "Select which mod you want to install:");
            if (mod == "Cancel") return;

            await ModZip.Install(Basepath, Destinationpath, mod);

            if (!File.Exists(Constants.Jsonpath) || !Presetfile.GetPresets().ConvertAll(x => x.Mod).Contains(mod))
            {
                bool save = AnsiConsole.Confirm("Do you want to save these details as a preset now for future use?");
                if (save)
                {
                    PresetsJson preset = new()
                    {
                        BaseFolder = Basepath,
                        DestinationFolder = Destinationpath,
                        Mod = mod
                    };
                    Presetfile.WriteJson(preset);
                }
            }
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
            items.Add("Cancel");

            string mod = Inputs.ChooseFromChoice(items, "Select which mod you want to install:");
            if (mod == "Cancel") return;

            if (mod == "All presets")
            {
                await Parallel.ForEachAsync(presets, async (preset, _) => await ModZip.Install(preset.BaseFolder, preset.DestinationFolder, preset.Mod));
                return;
            }
            var preset = presets.Find(x => x.Mod == mod);
            await ModZip.Install(preset.BaseFolder, preset.DestinationFolder, preset.Mod);
        }

        public static async Task AddToJson()
        {
            Console.WriteLine("WARNING: only 1 preset per mod is allowed and selecting again will override the existing one");
            Inputs.Setfolderpaths(out string Basepath, out string Destinationpath);
            List<string> items = new(Constants.Mods.Keys)
            {
                "Cancel"
            };
            string mod = Inputs.ChooseFromChoice(items, "Select which mod you want to install at the folder:");
            if (mod == "Cancel") return;

            PresetsJson preset = new()
            {
                Mod = mod,
                BaseFolder = Basepath,
                DestinationFolder = Destinationpath
            };
            Presetfile.WriteJson(preset);
        }

        public static async Task RemoveFromJson()
        {
            if (!File.Exists(Constants.Jsonpath))
            {
                Console.WriteLine("Presets file does not exist");
                return;
            }

            var presets = Presetfile.GetPresets();
            var items = Presetfile.GetPresets().ConvertAll(x => x.Mod);
            items.Add("Cancel");
            string mod = Inputs.ChooseFromChoice(items, "Choose the mod you want to remove:");

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