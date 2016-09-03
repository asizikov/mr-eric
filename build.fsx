// include Fake lib
#r @"packages/FAKE/tools/FakeLib.dll"
open Fake
open Fake.AppVeyor

let buildDir = "./.build/"
let packagingDir = "./.deploy/"
let nuspecFileName = "MrEric.nuspec"
let baseVersion = "1.3.0"
let slnFile = "./src/MrEric.sln"


Target "Clean" (fun _ ->
    CleanDirs [buildDir; packagingDir]
)

Target "BuildApp" (fun _ ->
    !! "src/**/*.csproj"
        |> MSBuild buildDir "Build" [ "Configuration", "Release" ]
        |> Log "AppBuild-Output: "
)

Target "Default" (fun _ ->
    trace "Building Mr.Eric"
)
Target "CreatePackage" (fun _ ->
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
  ==> "BuildApp"
  ==> "CreatePackage"
  ==> "Default"

RunTargetOrDefault "Default"