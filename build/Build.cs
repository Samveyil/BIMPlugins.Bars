using BIMPlugins.Nuke;
using Nuke.Common;
using Nuke.Common.ProjectModel;

public class Build : BIMPluginsBuild
{
    protected override int MajorVersion => 2;
    protected override int MinorVersion => 1;
    protected override int MaintenanceVersion => 1;


    [Solution("BIMPlugins.Bars.sln")]
    protected override Solution Solution { get; }

    [Parameter("Project name to build")]
    protected override string ProjectName { get; } = "BIMPlugins.Bars";

    [Parameter("Build configurations to run sequentially")]
    protected override string[] Configurations { get; } = new[] { "Release" };

    public static int Main() => Execute<Build>(x => x.Compile);
}