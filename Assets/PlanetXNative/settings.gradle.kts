pluginManagement {
    repositories {
        google {
            content {
                includeGroupByRegex("com\\.android.*")
                includeGroupByRegex("com\\.google.*")
                includeGroupByRegex("androidx.*")
            }
        }
        mavenCentral()
        gradlePluginPortal()
    }
}
dependencyResolutionManagement {
    repositoriesMode.set(RepositoriesMode.FAIL_ON_PROJECT_REPOS)
    repositories {
        google()
        mavenCentral()
        flatDir {
            dirs("${rootDir}/../../UnityExports/PlanetXAR/unityLibrary/libs")
        }
    }
}

rootProject.name = "PlanetX Native"
include(":app")

// Include Unity Library module
include(":unityLibrary")
project(":unityLibrary").projectDir = file("${rootDir}/../../UnityExports/PlanetXAR/unityLibrary")

// Include Unity XR Manifest module
include(":unityLibrary:xrmanifest.androidlib")
project(":unityLibrary:xrmanifest.androidlib").projectDir = file("${rootDir}/../../UnityExports/PlanetXAR/unityLibrary/xrmanifest.androidlib")
