#addin nuget:?package=Cake.FileHelpers
#tool nuget:?package=nuget.commandline
#tool nuget:?package=MSBuild.StructuredLogger

var target = Argument("target", "Default");
var configuration = Argument("configuration", "Debug");
var solution = "./TestCakeConsoleApp.sln";
var solutionDir = "./"; // Replace with the path to your solution directory
var newVersion = "1.0.0.1"; // Replace with the new version number1.2.3.4";

Task("Update-Assembly-Version")
  .Does(() =>
{
  //Information($"./**/AssemblyInfo.cs");
  // Get all AssemblyInfo.cs files in the solution directory
  var assemblyInfoFiles = GetFiles($"./**/AssemblyInfo.cs");
  
  Information($"Encontrados {assemblyInfoFiles.Count()} AssemblyFiles nesta pasta.");
  foreach(var assemblyInfoFile in assemblyInfoFiles)
  {
    Information($"Alterando versão de {assemblyInfoFile.FullPath} para {newVersion}.");    

    try
    {
    // Update AssemblyVersion
    ReplaceTextInFiles(assemblyInfoFile.FullPath, 
      @"AssemblyVersion\(""\d+\.\d+\.\d+\.\d+""\)", 
      $"AssemblyVersion(\"{newVersion}\")");
    }
    catch (Exception ex)
    {
      Information($"Erro ao alterar AssemblyVersion: {ex.Message}");
    }
    // Update AssemblyFileVersion
    ReplaceTextInFiles(assemblyInfoFile.FullPath, 
      @"AssemblyFileVersion\(""\d+.\d+.\d+.\d+""\)", 
      $"AssemblyFileVersion(\"{newVersion}\")");
  }
});

Task("Clean")
  .Does(() =>
{
  CleanDirectory($"./{configuration}");
});

Task("Restore-NuGet-Packages")
  .IsDependentOn("Clean")
  .Does(() =>
{
  NuGetRestore(solution);
});

Task("Build")
  .IsDependentOn("Restore-NuGet-Packages")
  .Does(() =>
{
  MSBuild(solution, settings =>
    settings.SetConfiguration(configuration));
});

Task("Default")
  .IsDependentOn("Update-Assembly-Version");
  //.IsDependentOn("Build");

RunTarget(target);
