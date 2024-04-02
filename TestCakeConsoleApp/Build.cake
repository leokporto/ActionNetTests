#tool nuget:?package=nuget.commandline&version=6.9.1
#tool nuget:?package=MSBuild.StructuredLogger&version=2.2.206

var target = Argument("target", "PublishActionNet");
var configuration = Argument("configuration", "Debug");
var solution = "./TestCakeConsoleApp.sln";
var solutionDir = "./"; // Replace with the path to your solution directory
var newVersion = "1.0.0.4"; // Replace with the new version number1.2.3.4";

void UpdateVersionInAsssemblyFiles(string filePath, string searchText, string replaceText)
{
    var text = System.IO.File.ReadAllText(filePath);
    text = System.Text.RegularExpressions.Regex.Replace(text, searchText, replaceText);
    System.IO.File.WriteAllText(filePath, text);
}

Task("Clean")
  .Does(() =>
{
  CleanDirectory($"./{configuration}");
});

Task("GetLatestVersionSrc");

Task("GetLatestVersionResources");

Task("Update-Assembly-Version")
  .IsDependentOn("GetLatestVersionSrc")
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
      UpdateVersionInAsssemblyFiles(assemblyInfoFile.FullPath, 
      @"AssemblyVersion\(""\d+\.\d+\.\d+\.\d+""\)", 
      $"AssemblyVersion(\"{newVersion}\")");
      
      UpdateVersionInAsssemblyFiles(assemblyInfoFile.FullPath, 
      @"AssemblyFileVersion\(""\d+\.\d+\.\d+\.\d+""\)", 
      $"AssemblyFileVersion(\"{newVersion}\")");
    }
    catch (Exception ex)
    {
      Information($"Erro ao alterar AssemblyVersion: {ex.Message}");
    }
    
  }
});

Task("Restore-NuGet-Packages")  
  .Does(() =>
{
  NuGetRestore(solution);
});

Task("Build")
  .IsDependentOn("Restore-NuGet-Packages")
  .IsDependentOn("Update-Assembly-Version")
  .Does(() =>
{
  MSBuild(solution, settings =>
    settings.SetConfiguration(configuration));
});

Task("CopyTatsoftFiles")
  .IsDependentOn("GetLatestVersionResources");

Task("Copy3rdPartyFiles")
  .IsDependentOn("CopyTatsoftFiles");

Task("CopyBuildFiles")
  .IsDependentOn("Build")
  .IsDependentOn("Copy3rdPartyFiles");

Task("GetHelpFiles");

Task("CopyHelpFiles")
  .IsDependentOn("GetHelpFiles");
  
Task("SignFiles")
  .IsDependentOn("CopyBuildFiles");

Task("SetStagingFiles")    
  .IsDependentOn("Clean")
  .IsDependentOn("CopyHelpFiles")
  .IsDependentOn("SignFiles");

Task("CheckinFiles");

Task("PublishActionNet")
  .IsDependentOn("SetStagingFiles")
  .IsDependentOn("CheckinFiles");

RunTarget(target);
