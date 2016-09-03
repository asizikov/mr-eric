// include Fake lib
#r @"packages/FAKE/tools/FakeLib.dll"
open Fake
open Fake.AppVeyor

// Project parameters

let buildDir = "./build/"
let packagingDir = "./.deploy/"
let baseVersion = "2016.2.0"

let version = 
        match buildServer with 
        | AppVeyor -> environVar "GitVersion_NuGetVersionV2"
        | _ ->  baseVersion + "-local"

let isMergeRequest = 
    trace "Merge Request Number"
    let mrVal = environVar "APPVEYOR_PULL_REQUEST_NUMBER"
    trace mrVal
    let MrNumber = 
        match buildServer with 
        | AppVeyor -> environVarOrNone "APPVEYOR_PULL_REQUEST_NUMBER"
        | _ ->  None
    match MrNumber with
    | Some(_) -> false
    | None -> true

// NuGet
let projectName = "MrEric"
let nuspecFileName = projectName + ".nuspec"
let packageName = "Sizikov." + projectName
let galleryUri = "https://resharper-plugins.jetbrains.com"

let apiKey = 
        match buildServer with 
        | AppVeyor -> environVar "resharper_nuget_api_key"
        | _ ->  "Nothing here"


Target "Clean" (fun _ ->
    CleanDirs [buildDir; packagingDir]
)

Target "BuildApp" (fun _ ->
    !! "src/**/*.csproj"
        |> MSBuild buildDir "Build" [ "Configuration", "Release" ]
        |> Log "AppBuild-Output: "
)

Target "Default" (fun _ ->
    DoNothing()
)
Target "CreatePackage" (fun _ ->
    NuGet (fun p -> 
        {p with
            OutputPath = packagingDir
            WorkingDir = "."
            Version = version
            Publish = false }) 
            nuspecFileName
)

Target "PublishPackage" (fun _ -> 
    let branch = Fake.Git.Information.getBranchName "."
    if branch = "master" then
        if isLocalBuild then
            trace "Package can only be published from Appveyor build"
        else
            match buildServer with 
            | AppVeyor -> 
                    if isMergeRequest then
                        trace "Skip publish from PR"
                    else 
                        NuGetPublish (fun p ->
                        { p with
                            AccessKey = apiKey
                            PublishUrl = galleryUri
                            NoPackageAnalysis = true
                            Publish = true
                            WorkingDir = packagingDir
                            OutputPath = packagingDir
                            Project = packageName
                            Version = version
                        })
            | _ ->  trace "Do not publish from local build"
    else 
        trace "Package can only be published from master branch"
        DoNothing()

)

"Clean"
  ==> "BuildApp"
  ==> "CreatePackage"
  ==> "PublishPackage"
  ==> "Default"

RunTargetOrDefault "Default"