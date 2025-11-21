plugins {
    alias(libs.plugins.android.application)
    alias(libs.plugins.kotlin.android)
    alias(libs.plugins.kotlin.compose)
}

android {
    namespace = "com.yourcompany.planetxnative"
    compileSdk = 36

    defaultConfig {
        applicationId = "com.yourcompany.planetxnative"
        minSdk = 24  // Android 7.0 Nougat - Matches Unity export API level
        targetSdk = 36
        versionCode = 1
        versionName = "1.0"

        testInstrumentationRunner = "androidx.test.runner.AndroidJUnitRunner"
        
        // NDK configuration for Unity - only arm64-v8a for now
        ndk {
            abiFilters.addAll(listOf("arm64-v8a"))
        }
    }

    buildTypes {
        release {
            isMinifyEnabled = false
            proguardFiles(
                getDefaultProguardFile("proguard-android-optimize.txt"),
                "proguard-rules.pro"
            )
        }
    }
    compileOptions {
        sourceCompatibility = JavaVersion.VERSION_17
        targetCompatibility = JavaVersion.VERSION_17
    }
    kotlinOptions {
        jvmTarget = "17"
    }
    buildFeatures {
        compose = true
    }
    
    // Exclude Unity .meta files from resources
    androidResources {
        ignoreAssetsPattern = "*.meta"
    }
    
    packaging {
        resources {
            excludes += "**/*.meta"
        }
    }
}

dependencies {

    implementation(libs.androidx.core.ktx)
    implementation(libs.androidx.lifecycle.runtime.ktx)
    implementation(libs.androidx.activity.compose)
    implementation(platform(libs.androidx.compose.bom))
    implementation(libs.androidx.ui)
    implementation(libs.androidx.ui.graphics)
    implementation(libs.androidx.ui.tooling.preview)
    implementation(libs.androidx.material3)
    
    // Navigation
    implementation("androidx.navigation:navigation-compose:2.7.7")
    
    // Icons Extended
    implementation("androidx.compose.material:material-icons-extended:1.6.3")
    
    // AppCompat for UnityHolderActivity
    implementation("androidx.appcompat:appcompat:1.6.1")
    
    // Google Games SDK for Unity 6
    implementation("androidx.games:games-activity:3.0.5")
    
    testImplementation(libs.junit)
    androidTestImplementation(libs.androidx.junit)
    androidTestImplementation(libs.androidx.espresso.core)
    androidTestImplementation(platform(libs.androidx.compose.bom))
    androidTestImplementation(libs.androidx.ui.test.junit4)
    debugImplementation(libs.androidx.ui.tooling)
    debugImplementation(libs.androidx.ui.test.manifest)
    
    // Unity Library dependency - use api to expose Unity classes
    api(project(":unityLibrary"))
    implementation(files("C:/Users/ankur/Planet X - AR/UnityExports/unityLibrary/libs/unity-classes.jar"))
}