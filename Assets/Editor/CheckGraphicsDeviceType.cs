///<summary>
/// Name: CheckGraphicsDeviceType.cs
/// Author: David Azouz
/// Date Created: 03/09/17
/// Date Modified: 03/09/17
/// --------------------------------------------------
/// Brief:
/// viewed: https://docs.unity3d.com/ScriptReference/EditorUtility.html
/// https://docs.unity3d.com/ScriptReference/Rendering.GraphicsDeviceType.html
/// https://docs.unity3d.com/ScriptReference/SystemInfo-graphicsDeviceVersion.html
/// 
/// https://github.com/NvPhysX/UnrealEngine/tree/NvFlow-4.17
/// https://github.com/NVIDIAGameWorks/Flow/releases/latest
/// https://github.com/unity3d-jp/NVIDIAHairWorksIntegration
/// https://github.com/agens-no/PolyglotUnity/blob/master/Assets/Polyglot/Editor/LocalizationEditor.cs
/// --------------------------------------------------
/// Edits:
/// - Script Created - David Azouz 03/09/17
/// - download Flow dll - David Azouz 04/09/17
/// 
/// TODO:
/// 
/// </summary>

using UnityEngine;
using UnityEditor;
using UnityEngine.Networking;
using UnityEngine.Rendering;
using System.IO;
using System.Linq;

public class CheckGraphicsDeviceType : Editor
{
#if UNITY_EDITOR
    [MenuItem("Tools/Setup Graphics Device Type &c")]
    public static void SetupGraphicsDeviceTypePlease()
    {
        Debug.Log(SystemInfo.graphicsDeviceVersion);

        #region Graphics Device Type Setup
        //HACK: this won't suffice if our graphics card can't run Dx12
        // If Nvidia and can run Dx12...
        if (SystemInfo.graphicsDeviceVendor == "NVIDIA" &&
            SystemInfo.operatingSystem.Contains("Windows 10") &&
            SystemInfo.graphicsDeviceType != GraphicsDeviceType.Direct3D12)
        {
            //... set to Dx12
            GraphicsDeviceType[] graphicsTypes =
                new GraphicsDeviceType[(int)GraphicsDeviceType.Direct3D12];
            PlayerSettings.SetGraphicsAPIs(BuildTarget.StandaloneWindows64, graphicsTypes);
            Debug.Log("Win 10 " + SystemInfo.graphicsDeviceType); // Direct3D12
            Debug.Log("mSize " + SystemInfo.graphicsMemorySize); //2GB required for Flow
        }
        // ... otherwise, check if it's the other most common platform used
        else if (SystemInfo.graphicsDeviceVendor == "NVIDIA" &&
            SystemInfo.operatingSystem.Contains("Windows 7") &&
            SystemInfo.graphicsDeviceType != GraphicsDeviceType.Direct3D11)
        {
            //... set to Dx11
            GraphicsDeviceType[] graphicsTypes =
                new GraphicsDeviceType[(int)GraphicsDeviceType.Direct3D11];
            PlayerSettings.SetGraphicsAPIs(BuildTarget.StandaloneWindows64, graphicsTypes);
            Debug.Log("Win 7 " + SystemInfo.graphicsDeviceType); // Direct3D11
        }
        // ... else, get a new PC
        else
        {
            PlayerSettings.SetGraphicsAPIs(BuildTarget.StandaloneWindows64,
                PlayerSettings.GetGraphicsAPIs(BuildTarget.StandaloneWindows64));
            Debug.Log(SystemInfo.graphicsDeviceVendor); // Company = ATI, Nvidia etc.
            Debug.Log(SystemInfo.graphicsDeviceType); // Direct3D11?
            Debug.Log(SystemInfo.graphicsDeviceName); // Name of Graphics Card
        }
        #endregion

        //TODO: Set quality settings/
        switch (SystemInfo.graphicsDeviceType)
        {
            /*case UnityEngine.Rendering.GraphicsDeviceType.Direct3D9:
                break;
            case UnityEngine.Rendering.GraphicsDeviceType.Direct3D11:
                break;
            case UnityEngine.Rendering.GraphicsDeviceType.Null:
                break;
            case UnityEngine.Rendering.GraphicsDeviceType.OpenGLES2:
                break;
            case UnityEngine.Rendering.GraphicsDeviceType.OpenGLES3:
                break;
            case UnityEngine.Rendering.GraphicsDeviceType.Metal:
                break;
            case UnityEngine.Rendering.GraphicsDeviceType.OpenGLCore:
                break;*/
            case GraphicsDeviceType.Direct3D12:
                DownloadFile();
                break;
            case UnityEngine.Rendering.GraphicsDeviceType.Vulkan:
                break;
            default:
                break;
        }
    }

    [MenuItem("Tools/Download File &d")]
    public static void DownloadFile()
    {
        string path = "";
        string bit = "";
        string newFolderPath = "";
        if (SystemInfo.operatingSystem.Contains("64bit"))
        {
            path = "/Assets/Plugins/x86_64";
            bit = "64";
        }
        else
        {
            path = "/Assets/Plugins/x86";
            bit = "32";
        }

        // Update to full path
        path = Path.Combine(Directory.GetCurrentDirectory(), path);
        try
        {
            if (!Directory.Exists(path))
            {
                AssetDatabase.CreateFolder("Assets", "Plugins");
                string guid = AssetDatabase.CreateFolder("Assets/Plugins", path.Split('/').Last());
                // update path
                newFolderPath = AssetDatabase.GUIDToAssetPath(guid);
                AssetDatabase.Refresh();
            }
        }
        catch (System.Exception e)
        {
            Debug.LogErrorFormat("Error path is {0} or {2}. Message = {1}", path, e.Message, newFolderPath);
            throw;
        }
        if (!File.Exists(string.Concat(path, "\\NvFlowLibRelease_win", bit, ".dll")))
        {
            DownloadGithubFile("NVIDIAGameWorks", "Flow",
                    string.Concat("bin/win", bit, "/NvFlowLibRelease_win", bit, ".dll"),
                    "flaimpath", newFolderPath);
            Debug.Log("Downloading");
        }
        else
        {
            Debug.Log("File exists!");
        }
    }

    /// <summary>
    /// Idea from https://github.com/agens-no/PolyglotUnity/blob/master/Assets/Polyglot/Editor/LocalizationEditor.cs
    /// </summary>
    /// <param name="user">NvidiaGameWorks</param>
    /// <param name="repo">Flow</param>
    /// <param name="repoFilePath">remaining path</param>
    /// <param name="prefs">???</param>
    /// <param name="defaultPath">destination</param>
    private static void DownloadGithubFile(string user, string repo, string repoFilePath, string prefs, string defaultPath)
    {
        EditorUtility.DisplayCancelableProgressBar("Download", "Downloading...", 0);
        var url = string.Format("https://github.com/{0}/{1}/raw/master/{2}", user, repo, repoFilePath);
        //"https: //docs.google.com/spreadsheets/d/{0}/export?format={2}&gid={1}", docsId, sheetId, System.Enum.GetName(typeof(LocalizationAssetFormat), format).ToLower());
        Debug.Log(url);
#if UNITY_5_5_OR_NEWER
        var www = UnityWebRequest.Get(url);
        www.Send();
#else
            var www = new WWW(url);
#endif
        while (!www.isDone)
        {
#if UNITY_5_5_OR_NEWER
            var progress = www.downloadProgress;
#else
                var progress = www.progress;
#endif

            if (EditorUtility.DisplayCancelableProgressBar("Download", "Downloading...", progress))
            {
                return;
            }
        }
        EditorUtility.ClearProgressBar();
        var path = GetPrefsString(prefs, defaultPath);

        if (string.IsNullOrEmpty(path))
        {
            path = EditorUtility.SaveFilePanelInProject("Save dll", repoFilePath.Split('/').Last(), "dll", "Please enter a file name to save the dll to", defaultPath);
            SetPrefsString(prefs, path);
        }
        if (string.IsNullOrEmpty(path))
        {
            return;
        }

#if UNITY_5_5_OR_NEWER
        var text = www.downloadHandler.text;
#else
            var text = www.text;
#endif

        if (text.StartsWith("<!"))
        {
            Debug.LogError("File from Repo could not be downloaded\n" + text);
            return;
        }

        File.WriteAllText(path, text);

        Debug.Log("Importing " + path);
        AssetDatabase.ImportAsset(path);

        //LocalizationImporter.Refresh();
    }

    #region Prefs

    private static string GetPrefsString(string key, string defaultString = null)
    {
        return EditorPrefs.GetString(Application.productName + "." + key, defaultString);
    }
    private static void SetPrefsString(string key, string value)
    {
        EditorPrefs.SetString(Application.productName + "." + key, value);
    }
    private static int GetPrefsInt(string key, int defaultInt = 0)
    {
        return EditorPrefs.GetInt(Application.productName + "." + key, defaultInt);
    }
    private static void SetPrefsInt(string key, int value)
    {
        EditorPrefs.SetInt(Application.productName + "." + key, value);
    }
    private static bool HasPrefsKey(string key)
    {
        return EditorPrefs.HasKey(Application.productName + "." + key);
    }
    private static void DeletePrefsKey(string key)
    {
        EditorPrefs.DeleteKey(Application.productName + "." + key);
    }
    #endregion
#endif
}
