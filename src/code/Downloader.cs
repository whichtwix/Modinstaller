using System;
using System.IO;
using Spectre.Console;
using System.Threading.Tasks;

namespace Modinstaller
{
    public sealed class Modinstaller
    {
        private static async Task Main()
        {
            if (File.Exists(Constants.ErrorLog)) File.Delete(Constants.ErrorLog);

            bool useagain = true;
            while (useagain)
            {
                string choice = Inputs.ChooseFromChoice(Constants.Actions.Keys, "Select the operation to do:");
                try
                {
                    await Constants.Actions[choice].Invoke();
                }
                catch (Exception e)
                {
                    Console.WriteLine("An error occured, sending to log file");
                    File.AppendAllText(Constants.ErrorLog, $"\n{e}");
                }
                useagain = AnsiConsole.Confirm("Want to do another operation?");
            }
        }
    }
}
