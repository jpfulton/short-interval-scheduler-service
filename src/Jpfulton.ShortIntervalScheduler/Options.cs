using System.Text;

namespace Jpfulton.ShortIntervalScheduler;

public class Options {

  public string Command { get; private set; }
  public int DelayInSeconds { get; private set; }
  public Options(string cmd, int delayInSeconds) {
    Command = cmd;
    DelayInSeconds = delayInSeconds;
  }

  public static Options GetOptionsFromArgs(string[] args) {
    if (args.Length != 2) {
      var msgBuilder = new StringBuilder();
      msgBuilder.AppendLine($"Expected two command line arguments: command, delayInSeconds. Found {args.Length} arguments.");
      foreach (var arg in args) {
        msgBuilder.AppendLine($"Found arg: \"{arg}\"");
      }
      
      throw new Exception(msgBuilder.ToString());
    }

    string command = args[0];

    int delay = 0;
    try
    {
      delay = int.Parse(args[1]);
    }
    catch (FormatException fe) {
      throw new Exception("Delay could not be parsed from command line argument.", fe);
    }

    return new Options(command, delay);
  }
}