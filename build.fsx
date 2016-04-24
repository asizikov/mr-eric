// include Fake lib
#r @"packages/FAKE/tools/FakeLib.dll"
open Fake

RestorePackages()

let buildDir = "./.build/"

Target "Clean" (fun _ ->
    CleanDir buildDir
)


Target "BuildApp" (fun _ ->
    !! "src/**/*.csproj"
        |> MSBuildRelease buildDir "Build"
        |> Log "AppBuild-Output: "
)

Target "Default" (fun _ ->
    trace "Building Mr.Eric"
)

// Dependencies
"Clean"
  ==> "BuildApp"
  ==> "Default"


// start build
RunTargetOrDefault "Default"