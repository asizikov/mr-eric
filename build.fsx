// include Fake lib
#r @"packages/FAKE/tools/FakeLib.dll"
open Fake

RestorePackages()

let buildDir = "./.build/"
let packagingDir = "./.deploy/"
let nuspecFileName = "MrEric.nuspec"
let version = "1.3"

//NuSpec fileds


Target "Clean" (fun _ ->
    CleanDirs [buildDir; packagingDir]
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
  ==> "BuildApp"
  ==> "CreatePackage"
  ==> "Default"



// start build
RunTargetOrDefault "Default"