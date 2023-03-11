using System;
using System.IO;
using System.Collections.Generic;
using Spectre.Console;

namespace Modinstaller
{
    public sealed class Inputs
    {
        public static void Setfolderpaths(out string Basepath, out string Destination)
        {
            bool acceptedpath = false;
            Basepath = string.Empty;
            Destination = string.Empty;

            while (!acceptedpath)
            {
                Basepath = AnsiConsole.Ask<string>("Enter the path to your among us folder(copy paste here):");
                if (Directory.Exists(Basepath + "\\Among Us_Data")) acceptedpath = true;
                else Console.WriteLine($"the path '{Basepath}' is not valid; vanilla files could not be found");
            }

            if (!Directory.Exists(Basepath + "\\.egstore"))
            {
                bool enter = AnsiConsole.Confirm("Would you like to make a copy of the game and put the mod there?");
                if (enter) Destination = AnsiConsole.Ask<string>("Enter the path to a empty folder(copy paste here) or enter to exit:");
                if (!ValidPath(Destination) && Destination != string.Empty)
                {
                    Destination = string.Empty;
                    Console.WriteLine($" '{Destination}' was found as an invalid directory or potentional directory");
                }
            }
        }

        public static bool ValidPath(string path)
        {
            if (Directory.Exists(path) && Directory.GetFiles(path) == Array.Empty<string>()) return true;

            if (!Directory.Exists(path))
            {
                try
                {
                    Directory.CreateDirectory(path).Delete();
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            }
            return false;
        }

        public static string ChooseFromChoice(IEnumerable<string> items, string title)
        {
            bool confirmed = false;
            string choice = string.Empty;

            Console.WriteLine("Use arrow keys to navigate and click enter");
            while (!confirmed)
            {
                choice = AnsiConsole.Prompt(
                                           new SelectionPrompt<string>()
                                               .Title(title)
                                               .PageSize(10)
                                               .AddChoices(items));

                confirmed = AnsiConsole.Confirm($"Confirm your choice of: {choice}");
            }
            return choice;
        }
    }
}