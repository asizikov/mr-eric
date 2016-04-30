// include Fake lib
#r @"packages/FAKE/tools/FakeLib.dll"
open Fake
open Fake.AppVeyor

let buildDir = "./.build/"
let packagingDir = "./.deploy/"
let nuspecFileName = "MrEric.nuspec"
let baseVersion = "1.3.0"


TraceEnvironmentVariables()

Target "Clean" (fun _ ->
    CleanDirs [buildDir; packagingDir]
)


Target "RestorePackages" (fun _ ->
  let packagesDir = @"./src/packages"
  !! "./**/packages.config"
  |> Seq.iter (RestorePackage (fun p ->
      { p with
          OutputPath = packagesDir }))
)

Target "InstallGitVersion" (fun _ ->
    "gitversion.portable" |> Choco.Install id
    let args = match buildServer with 
                  | AppVeyor -> "/l console /output buildserver"
                  | _ ->  "/l console"
    "Running GitVersion with " + args |> trace
    let result = Shell.Exec("gitversion", args ) 
    if result <> 0 then failwithf "%s exited with error %d" "gitversion" result
)


Target "BuildApp" (fun _ ->
    !! "src/**/*.csproj"
        |> MSBuildRelease buildDir "Build"
        |> Log "AppBuild-Output: "
)

Target "Default" (fun _ ->
    trace "Building Mr.Eric"
)
Target "CreatePackage" (fun _ ->
    TraceEnvironmentVariables()
    let version = 
        match buildServer with 
        | AppVeyor -> environVar "GitVersion_SemVer"
        | _ ->  baseVersion + "-local"
    //let x = Shell.Exec("powershell", "(get-item env:GitVersion_InformationalVersion).Value"))// |> trace value
    appSetting "GitVersion_InformationalVersion" |> trace
    getBuildParam "GitVersion_InformationalVersion" |> trace
    NuGet (fun p -> 
        {p with
            OutputPath = packagingDir
            WorkingDir = "."
            Version = version
            Publish = false }) 
            nuspecFileName
)



"Clean"
  =?> ("InstallGitVersion", Choco.IsAvailable)
  ==> "RestorePackages"
  ==> "BuildApp"
  ==> "CreatePackage"
  ==> "Default"

RunTargetOrDefault "Default"