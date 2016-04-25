// include Fake lib
#r @"packages/FAKE/tools/FakeLib.dll"
open Fake
open Fake.AppVeyor

//RestorePackages()

let buildDir = "./.build/"
let packagingDir = "./.deploy/"
let nuspecFileName = "MrEric.nuspec"
let baseVersion = "1.3.0"
let version = 
    match buildServer with 
    | AppVeyor ->  baseVersion + "-rc" + AppVeyorEnvironment.BuildNumber
    | _ ->  baseVersion + "-local"

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
    // Copy all the package files into a package folder
    //CopyFiles packagingDir allPackageFiles

    NuGet (fun p -> 
        {p with
            OutputPath = packagingDir
            WorkingDir = "."
            Version = version
            Publish = false }) 
            nuspecFileName
)
// Dependencies
"Clean"
  =?> ("InstallGitVersion", Choco.IsAvailable)
  ==> "RestorePackages"
  ==> "BuildApp"
  ==> "CreatePackage"
  ==> "Default"



// start build
RunTargetOrDefault "Default"