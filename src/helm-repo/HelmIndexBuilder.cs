using CliWrap;
using Serilog;

namespace HelmRepo;

public class HelmIndexBuilder
{
    private CommandTask<CommandResult> currentTask;
    public async Task BuildIndex()
    {
        try
        {
            if (currentTask == null || currentTask?.GetAwaiter().IsCompleted == true)
            {
                currentTask = Cli.Wrap("helm")
                    .WithArguments("repo index")
                    .ExecuteAsync();
            }
        }
        catch (Exception e)
        {
            Log.Error(e, "An error occured whilst helm was building the index.");
        }
    }
}