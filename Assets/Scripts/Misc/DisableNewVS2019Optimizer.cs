#if UNITY_EDITOR_WIN

using UnityEngine;
using UnityEditor;
using UnityEditor.Build;

/// <summary>
/// Disables the new optimizer in Visual Studio 2019 because of bugs that
/// cause Rewired to fail to compile or go into an infinite loop when
/// running the built executable when compiled with the new optimizer.
///
/// Note that you will get a compile error if you use this script and do not
/// have Visual Studio 2019 installed as VS 2017 or other versions do not
/// have this compiler flag option.
///
/// Deleting the script will not remove the compiler option if it has already
/// been set. You can remove it by editing ProjectSettings\ProjectSettings.asset
/// in a text editor under the heading additionalIl2CppArgs.
///
/// Last tested version where issue occurred: Visual Studio 2019 16.7.6.
/// </summary>
[InitializeOnLoad]
public class DisableNewVS2019Optimizer : IActiveBuildTargetChanged {

    const string disableString = " --compiler-flags=-d2ssa-cfg-jt-";

    static DisableNewVS2019Optimizer() {
        Go();
    }

    static void Go() {
        if(BuildTargetUsesVS(EditorUserBuildSettings.activeBuildTarget)) {
            Add();
        } else {
            Remove();
        }
    }

    static void Remove() {
        string args = PlayerSettings.GetAdditionalIl2CppArgs();
        if(!args.Contains(disableString)) return;
        PlayerSettings.SetAdditionalIl2CppArgs(args.Replace(disableString, ""));
        Debug.Log("Removed IL2CPP args: " + disableString);
    }

    static void Add() {
        string args = PlayerSettings.GetAdditionalIl2CppArgs();
        if(args.Contains(disableString)) return;
        PlayerSettings.SetAdditionalIl2CppArgs(args + disableString);
        Debug.Log("Added IL2CPP args: " + disableString);
    }

    static bool BuildTargetUsesVS(BuildTarget target) {
        return
            target == BuildTarget.StandaloneWindows
            || target == BuildTarget.StandaloneWindows64
            || target == BuildTarget.WSAPlayer
#if UNITY_GAMECORE
            || target == BuildTarget.GameCoreScarlett
            || target == BuildTarget.GameCoreXboxOne
#endif
        ;
    }

    int IOrderedCallback.callbackOrder { get { return 0; } }

    void IActiveBuildTargetChanged.OnActiveBuildTargetChanged(BuildTarget previousTarget, BuildTarget newTarget) {
        Go();
    }
}

#endif