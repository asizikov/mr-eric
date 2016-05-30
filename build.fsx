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
        | AppVeyor -> environVar "GitVersion_NuGetVersion"
        | _ ->  baseVersion + "-local"

    NuGet (fun p -> 
        {p with
            OutputPath = packagingDir
            WorkingDir = "."
            Version = version
            Publish = false }) 
            nuspecFileName
)



"Clean"
  //=?> ("InstallGitVersion", Choco.IsAvailable)
  ==> "RestorePackages"
  ==> "BuildApp"
  ==> "CreatePackage"
  ==> "Default"

RunTargetOrDefault "Default"