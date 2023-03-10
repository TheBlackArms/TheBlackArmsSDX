using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using VRC.Core;

public partial class VRCSdkControlPanel : EditorWindow
{
    private static GUIStyle SettingsImage;

    bool UseDevApi
    {
        get
        {
            return VRC.Core.API.GetApiUrl() == VRC.Core.API.devApiUrl;
        }
    }
    public Color SDKColor = Color.gray;

    string clientVersionDate;
    string sdkVersionDate;
    Vector2 settingsScroll;

    private void Awake()
    {
        GetClientSdkVersionInformation();
    }

    public void GetClientSdkVersionInformation()
    {
        clientVersionDate = VRC.Core.SDKClientUtilities.GetTestClientVersionDate();
        sdkVersionDate = VRC.Core.SDKClientUtilities.GetSDKVersionDate();
    }

    public void OnConfigurationChanged()
    {
        GetClientSdkVersionInformation();
    }




    void ShowSettings()
    {
        SettingsImage = new GUIStyle
        {
            normal =
            {
             background = Resources.Load("SettingsImage") as Texture2D,
            },
            fixedHeight = 100
        };

        GUI.backgroundColor = new Color(
    UnityEditor.EditorPrefs.GetFloat("SDKColor_R"),
    UnityEditor.EditorPrefs.GetFloat("SDKColor_G"),
    UnityEditor.EditorPrefs.GetFloat("SDKColor_B"),
    UnityEditor.EditorPrefs.GetFloat("SDKColor_A")
);

        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        GUILayout.BeginVertical();

        GUI.backgroundColor = Color.white;
        GUILayout.Box("", SettingsImage);
        GUI.backgroundColor = Color.gray;

        settingsScroll = EditorGUILayout.BeginScrollView(settingsScroll, GUILayout.Width(SdkWindowWidth));
        GUI.backgroundColor = new Color(
UnityEditor.EditorPrefs.GetFloat("SDKColor_R"),
UnityEditor.EditorPrefs.GetFloat("SDKColor_G"),
UnityEditor.EditorPrefs.GetFloat("SDKColor_B"),
UnityEditor.EditorPrefs.GetFloat("SDKColor_A")
);
        EditorGUILayout.BeginHorizontal(boxGuiStyle, GUILayout.Height(26));
        if (GUILayout.Button("The Black Arms Website"))
        {
            Application.OpenURL("https://trigon.systems/");
        }
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal(boxGuiStyle, GUILayout.Height(26));
        if (GUILayout.Button("Latest SDX Release Page"))
        {
            Application.OpenURL("https://www.github.com/TheBlackArms/TheBlackArmsSDX/releases/latest");
        }
        if (GUILayout.Button("SDX Support Server"))
        {
            Application.OpenURL("https://discord.gg/A9dca3N");
        }
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginVertical(boxGuiStyle);
        EditorGUILayout.LabelField("Developer", EditorStyles.boldLabel);
#if VRC_SDK_VRCSDK2
        GUI.backgroundColor = Color.gray;
        VRCSettings.Get().DisplayAdvancedSettings = EditorGUILayout.ToggleLeft("Show Extra Options on build page and account page", VRCSettings.Get().DisplayAdvancedSettings);
        bool prevDisplayHelpBoxes = VRCSettings.Get().DisplayHelpBoxes;
        VRCSettings.Get().DisplayHelpBoxes = EditorGUILayout.ToggleLeft("Show Help Boxes on SDK components", VRCSettings.Get().DisplayHelpBoxes);
        if (VRCSettings.Get().DisplayHelpBoxes != prevDisplayHelpBoxes)
            GUI.backgroundColor = new Color(
UnityEditor.EditorPrefs.GetFloat("SDKColor_R"),
UnityEditor.EditorPrefs.GetFloat("SDKColor_G"),
UnityEditor.EditorPrefs.GetFloat("SDKColor_B"),
UnityEditor.EditorPrefs.GetFloat("SDKColor_A")
);
#elif VRC_SDK_VRCSDK3
        VRC.SDK3.Editor.VRCSettings.Get().DisplayAdvancedSettings = EditorGUILayout.ToggleLeft("Show Extra Options on build page and account page", VRC.SDK3.Editor.VRCSettings.Get().DisplayAdvancedSettings);
        bool prevDisplayHelpBoxes = VRC.SDK3.Editor.VRCSettings.Get().DisplayHelpBoxes;
        VRC.SDK3.Editor.VRCSettings.Get().DisplayHelpBoxes = EditorGUILayout.ToggleLeft("Show Help Boxes on SDK components", VRC.SDK3.Editor.VRCSettings.Get().DisplayHelpBoxes);
        if (VRC.SDK3.Editor.VRCSettings.Get().DisplayHelpBoxes != prevDisplayHelpBoxes)
#endif
        {
            Editor[] editors = (Editor[])Resources.FindObjectsOfTypeAll<Editor>();
            for (int i = 0; i < editors.Length; i++)
            {
                editors[i].Repaint();
            }
        }
        EditorGUILayout.EndVertical();

        EditorGUILayout.Separator();
        GUI.backgroundColor = new Color(
UnityEditor.EditorPrefs.GetFloat("SDKColor_R"),
UnityEditor.EditorPrefs.GetFloat("SDKColor_G"),
UnityEditor.EditorPrefs.GetFloat("SDKColor_B"),
UnityEditor.EditorPrefs.GetFloat("SDKColor_A")
);
        ShowSdk23CompatibilitySettings();
        EditorGUILayout.Separator();
        EditorGUILayout.BeginVertical(boxGuiStyle);
        GUILayout.Label("Avatar Options", EditorStyles.boldLabel);
        bool prevShowPerfDetails = showAvatarPerformanceDetails;
        GUI.backgroundColor = Color.gray;
        bool showPerfDetails = EditorGUILayout.ToggleLeft("Show All Avatar Performance Details", prevShowPerfDetails);
        if (showPerfDetails != prevShowPerfDetails)
        {
            showAvatarPerformanceDetails = showPerfDetails;
            ResetIssues();
        }
        EditorGUILayout.EndVertical();
        GUI.backgroundColor = new Color(
           UnityEditor.EditorPrefs.GetFloat("SDKColor_R"),
           UnityEditor.EditorPrefs.GetFloat("SDKColor_G"),
           UnityEditor.EditorPrefs.GetFloat("SDKColor_B"),
           UnityEditor.EditorPrefs.GetFloat("SDKColor_A")
       );
        EditorGUILayout.Separator();
        EditorGUILayout.BeginVertical(boxGuiStyle);
        GUILayout.Label("World Options", EditorStyles.boldLabel);
        EditorGUILayout.BeginHorizontal();
        GUI.backgroundColor = Color.gray;
        int prevLineMode = triggerLineMode;
        int lineMode = System.Convert.ToInt32(EditorGUILayout.EnumPopup("Trigger Lines", (VRC.SDKBase.VRC_Trigger.EditorTriggerLineMode)triggerLineMode, GUILayout.Width(250)));
        if (lineMode != prevLineMode)
        {
            triggerLineMode = lineMode;
            foreach (GameObject t in Selection.gameObjects)
            {
                EditorUtility.SetDirty(t);
            }
        }
        GUILayout.Space(10);
        switch ((VRC.SDKBase.VRC_Trigger.EditorTriggerLineMode)triggerLineMode)
        {
            case VRC.SDKBase.VRC_Trigger.EditorTriggerLineMode.Enabled:
                EditorGUILayout.LabelField("Lines shown for all selected triggers", EditorStyles.miniLabel);
                break;
            case VRC.SDKBase.VRC_Trigger.EditorTriggerLineMode.Disabled:
                EditorGUILayout.LabelField("No trigger lines are drawn", EditorStyles.miniLabel);
                break;
            case VRC.SDKBase.VRC_Trigger.EditorTriggerLineMode.PerTrigger:
                EditorGUILayout.LabelField("Toggle lines directly on each trigger component", EditorStyles.miniLabel);
                break;
        }
        GUI.backgroundColor = new Color(
    UnityEditor.EditorPrefs.GetFloat("SDKColor_R"),
    UnityEditor.EditorPrefs.GetFloat("SDKColor_G"),
    UnityEditor.EditorPrefs.GetFloat("SDKColor_B"),
    UnityEditor.EditorPrefs.GetFloat("SDKColor_A")
);
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.EndVertical();

        // debugging
        if (APIUser.CurrentUser != null && APIUser.CurrentUser.hasSuperPowers)
        {
            EditorGUILayout.Separator();
            EditorGUILayout.BeginVertical(boxGuiStyle);
            EditorGUILayout.LabelField("Logging", EditorStyles.boldLabel);

            // API logging
            {
                bool isLoggingEnabled = UnityEditor.EditorPrefs.GetBool("apiLoggingEnabled");
                bool enableLogging = EditorGUILayout.ToggleLeft("API Logging Enabled", isLoggingEnabled);
                if (enableLogging != isLoggingEnabled)
                {
                    if (enableLogging)
                        VRC.Core.Logger.AddDebugLevel(DebugLevel.API);
                    else
                        VRC.Core.Logger.RemoveDebugLevel(DebugLevel.API);

                    UnityEditor.EditorPrefs.SetBool("apiLoggingEnabled", enableLogging);
                }
            }

            // All logging
            {
                bool isLoggingEnabled = UnityEditor.EditorPrefs.GetBool("allLoggingEnabled");
                bool enableLogging = EditorGUILayout.ToggleLeft("All Logging Enabled", isLoggingEnabled);
                if (enableLogging != isLoggingEnabled)
                {
                    if (enableLogging)
                        VRC.Core.Logger.AddDebugLevel(DebugLevel.All);
                    else
                        VRC.Core.Logger.RemoveDebugLevel(DebugLevel.All);

                    UnityEditor.EditorPrefs.SetBool("allLoggingEnabled", enableLogging);
                }
            }
            EditorGUILayout.EndVertical();
        }
        else
        {
            if (UnityEditor.EditorPrefs.GetBool("apiLoggingEnabled"))
                UnityEditor.EditorPrefs.SetBool("apiLoggingEnabled", false);
            if (UnityEditor.EditorPrefs.GetBool("allLoggingEnabled"))
                UnityEditor.EditorPrefs.SetBool("allLoggingEnabled", false);
        }

        // Future proof upload
        {
            EditorGUILayout.Separator();
            EditorGUILayout.BeginVertical(boxGuiStyle);
            
            EditorGUILayout.LabelField("Publish", EditorStyles.boldLabel);
            GUI.backgroundColor = Color.gray;
            bool futureProofPublish = UnityEditor.EditorPrefs.GetBool("futureProofPublish", DefaultFutureProofPublishEnabled);

            futureProofPublish = EditorGUILayout.ToggleLeft("Future Proof Publish", futureProofPublish);

            if (UnityEditor.EditorPrefs.GetBool("futureProofPublish", DefaultFutureProofPublishEnabled) != futureProofPublish)
            {
                UnityEditor.EditorPrefs.SetBool("futureProofPublish", futureProofPublish);
            }
            EditorGUILayout.LabelField("Client Version Date", clientVersionDate);
            EditorGUILayout.LabelField("SDK Version Date", sdkVersionDate);
            GUI.backgroundColor = new Color(
UnityEditor.EditorPrefs.GetFloat("SDKColor_R"),
UnityEditor.EditorPrefs.GetFloat("SDKColor_G"),
UnityEditor.EditorPrefs.GetFloat("SDKColor_B"),
UnityEditor.EditorPrefs.GetFloat("SDKColor_A")
);
            EditorGUILayout.EndVertical();
        }

        // Custom SDK Settings
        {
            EditorGUILayout.Separator();
            GUI.backgroundColor = new Color(
            UnityEditor.EditorPrefs.GetFloat("SDKColor_R"),
            UnityEditor.EditorPrefs.GetFloat("SDKColor_G"),
            UnityEditor.EditorPrefs.GetFloat("SDKColor_B"),
            UnityEditor.EditorPrefs.GetFloat("SDKColor_A")
            );
            EditorGUILayout.BeginVertical(boxGuiStyle);
            GUI.backgroundColor = Color.gray;

            EditorGUILayout.LabelField("SDK Settings", EditorStyles.boldLabel);

            if (GUILayout.Button("Set Color"))
            {
                UnityEditor.EditorPrefs.SetFloat("SDKColor_R", SDKColor.r);
                UnityEditor.EditorPrefs.SetFloat("SDKColor_G", SDKColor.g);
                UnityEditor.EditorPrefs.SetFloat("SDKColor_B", SDKColor.b);
                UnityEditor.EditorPrefs.SetFloat("SDKColor_A", SDKColor.a);
            }

            SDKColor = EditorGUI.ColorField(new Rect(3, 340, position.width - 6, 15), "SDK Color", SDKColor);
            
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            if (GUILayout.Button("Reset Color"))
            {
                Color SDKColor = Color.gray;

                UnityEditor.EditorPrefs.SetFloat("SDKColor_R", SDKColor.r);
                UnityEditor.EditorPrefs.SetFloat("SDKColor_G", SDKColor.g);
                UnityEditor.EditorPrefs.SetFloat("SDKColor_B", SDKColor.b);
                UnityEditor.EditorPrefs.SetFloat("SDKColor_A", SDKColor.a);
            }

            // SDKGRADIENT = EditorGUI.GradientField(new Rect(3, 290, position.width - 6, 15), "SDK Gradient", SDKGRADIENT);


            EditorGUILayout.EndVertical();
        }

        if (APIUser.CurrentUser != null)
        {
            EditorGUILayout.Separator();
            GUI.backgroundColor = new Color(
            UnityEditor.EditorPrefs.GetFloat("SDKColor_R"),
            UnityEditor.EditorPrefs.GetFloat("SDKColor_G"),
            UnityEditor.EditorPrefs.GetFloat("SDKColor_B"),
            UnityEditor.EditorPrefs.GetFloat("SDKColor_A")
            );
            EditorGUILayout.BeginVertical(boxGuiStyle);
            GUI.backgroundColor = Color.gray;

            // custom vrchat install location
            OnVRCInstallPathGUI();

            EditorGUILayout.EndVertical();
        }

        EditorGUILayout.EndScrollView();

        GUILayout.EndVertical();
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
    }

    static void OnVRCInstallPathGUI()
    {
        EditorGUILayout.LabelField("VRChat Client", EditorStyles.boldLabel);
        EditorGUILayout.LabelField("Installed Client Path: ", clientInstallPath);
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("");
        GUI.backgroundColor = Color.gray;
        if (GUILayout.Button("Edit"))
        {
            string initPath = "";
            if (!string.IsNullOrEmpty(clientInstallPath))
                initPath = clientInstallPath;

            clientInstallPath = EditorUtility.OpenFilePanel("Choose VRC Client Exe", initPath, "exe");
            SDKClientUtilities.SetVRCInstallPath(clientInstallPath);
            window.OnConfigurationChanged();
        }
        if (GUILayout.Button("Revert to Default"))
        {
            clientInstallPath = SDKClientUtilities.LoadRegistryVRCInstallPath();
            window.OnConfigurationChanged();
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Separator();
    }


    static void ShowSdk23CompatibilitySettings()
    {
        return;

//        EditorGUILayout.BeginVertical(boxGuiStyle);
//        EditorGUILayout.LabelField("VRCSDK2 & VRCSDK3 Compatibility", EditorStyles.boldLabel);

//#if !VRC_CLIENT
//        bool sdk2Present = VRCSdk3Analysis.GetSDKInScene(VRCSdk3Analysis.SdkVersion.VRCSDK2).Count > 0;
//        bool sdk3Present = VRCSdk3Analysis.GetSDKInScene(VRCSdk3Analysis.SdkVersion.VRCSDK3).Count > 0;
//        bool sdk2DllActive = VRCSdk3Analysis.IsSdkDllActive(VRCSdk3Analysis.SdkVersion.VRCSDK2);
//        bool sdk3DllActive = VRCSdk3Analysis.IsSdkDllActive(VRCSdk3Analysis.SdkVersion.VRCSDK3);

//        if ( sdk2DllActive && sdk3DllActive)
//        {
//            GUILayout.TextArea("You have not yet configured this project for development with VRCSDK2 and Triggers or VRCSDK3 and Udon. ");
//            if (sdk2Present && sdk3Present)
//            {
//                GUILayout.TextArea("This scene contains both SDK2 and SDK3 elements. " +
//                    "Please modify this scene to contain only one type or the other before completing your configuration.");
//            }
//            else if (sdk2Present)
//            {
//                GUILayout.TextArea("This scene contains SDK2 scripts. " +
//                    "Check below to configure this project for use with VRCSDK2 or remove your VRCSDK2 scripts to upgrade to VRCSDK3");
//                bool downgrade = EditorGUILayout.ToggleLeft("Configure for use with VRCSDK2 and Triggers", false);
//                if (downgrade)
//                    VRCSdk3Analysis.SetSdkVersionActive(VRCSdk3Analysis.SdkVersion.VRCSDK2);
//            }
//            else if (sdk3Present)
//            {
//                GUILayout.TextArea("This scene contains only SDK3 scripts and it ready to upgrade. " +
//                    "Click below to get started.");
//                bool upgrade = EditorGUILayout.ToggleLeft("Configure for use with VRCSDK3 and Udon - Let's Rock!", false);
//                if (upgrade)
//                    VRCSdk3Analysis.SetSdkVersionActive(VRCSdk3Analysis.SdkVersion.VRCSDK3);
//            }
//            else
//            {
//                GUILayout.TextArea("This scene is a blank slate. " +
//                    "Click below to get started.");
//                bool upgrade = EditorGUILayout.ToggleLeft("Configure for use with VRCSDK3 and Udon - Let's Rock!", false);
//                if (upgrade)
//                    VRCSdk3Analysis.SetSdkVersionActive(VRCSdk3Analysis.SdkVersion.VRCSDK3);
//            }
//        }
//        else if (sdk2DllActive)
//        {
//            GUILayout.TextArea("This project has been configured to be built with VRCSDK2. " +
//                "To upgrade, VRCSDK3 must be enabled here.");
//            bool upgrade = EditorGUILayout.ToggleLeft("VRCSDK3 Scripts can be used", false);
//            if (upgrade)
//                VRCSdk3Analysis.SetSdkVersionActive(VRCSdk3Analysis.SdkVersion.VRCSDK3);
//        }
//        else if (sdk3DllActive)
//        {
//            GUILayout.TextArea("This project has been configured to be built with VRCSDK3. " +
//                "Congratulations, you're ready to go. " +
//                "You can still downgrade by activating VRCSDK2 here.");
//            bool downgrade = EditorGUILayout.ToggleLeft("VRCSDK2 Scripts can be used", false);
//            if (downgrade)
//                VRCSdk3Analysis.SetSdkVersionActive(VRCSdk3Analysis.SdkVersion.VRCSDK2);
//        }
//        else
//        {
//            GUILayout.TextArea("Somehow you have disabled both VRCSDK2 and VRCSDK3. Oops. " +
//                "Click here to begin development with VRCSDK3.");
//            bool begin = EditorGUILayout.ToggleLeft("VRCSDK3 Scripts can be used", false);
//            if (begin)
//                VRCSdk3Analysis.SetSdkVersionActive(VRCSdk3Analysis.SdkVersion.VRCSDK3);
//        }
//#else
//        GUILayout.TextArea("I think you're in the main VRChat project. " +
//            "You should not be enabling or disabling SDKs from here.");
//#endif

//        EditorGUILayout.EndVertical();
    }
}
