///<summary>
/// Name: UnityTools.cs
/// Author: David Azouz
/// Date Created: 04/11/17
/// Date Modified: 04/11/17
/// --------------------------------------------------
/// Brief: Tools to allow for smoother development.
/// viewed: 
/// --------------------------------------------------
/// Edits:
/// - Script Created - David Azouz 04/11/17
/// -  - David Azouz /11/17
/// 
/// TODO:
/// 
/// </summary>

using UnityEngine;
using UnityEditor;
using System.IO;

namespace UnityTools
{
	public class UnityTools : Editor
	{
#if UNITY_EDITOR
        #region Branch Tools
		public const string unityToolsDir = "Tools/";
		public const string toolsDirIISRI = "C:/Users/FLAIM-AIE/Documents/IISRI_Tools/";
        public const string toolsDirHome  = "C:/Users/David/Documents/Game_Projects/IISRI/";
		
		private static bool isAtIISRI = true; // On site at AIE or IISRI, otherwise at home.
		
		// To be used with Bluetooth controller and UDP.
		[MenuItem(unityToolsDir + "Run Branch Visualiser %&v")]
		public static void RunBranchVisualiser()
		{
			RunExe(isAtIISRI ? toolsDirIISRI : toolsDirHome, 
			"FLAIM Branch TFT Battery B2/FLAIM Branch TFT Battery B2.exe"); 			
		}
		
		// This is the one with sliders.
		[MenuItem(unityToolsDir + "Run Branch Debugger %&d")]
		public static void RunBranchDebugger()
		{
			RunExe(isAtIISRI ? toolsDirIISRI : toolsDirHome, 
			"FLAIM Branch V4.1 Protek AIE Build/FLAIM Branch V4.1 Protek.exe");
            if (!isAtIISRI)
		        EditorApplication.playmodeStateChanged = DebugTurnRightHandOn; //DebugTurnRightHandOn();
		}
		
		private static void RunExe(string a_dir, string a_folderExe)
		{
			string path = Path.Combine(a_dir, a_folderExe);
			if(Directory.Exists(a_dir)) // bit hacky
				System.Diagnostics.Process.Start(path);
			else
				Debug.Log("Wrong path. " + path);
		}
        #endregion
		
        #region Ease of Life Tools
		// Purpose of this function is to turn the right hand controller 
		// on while not having a Vive controller connected (e.g. at home.)
        // Instructions: Pause Game and click on the menu item.
		[MenuItem(unityToolsDir + "Debug Turn Right Hand On")]
		private static void DebugTurnRightHandOn()
		{
            // If at home aka no Vive.
            if (!isAtIISRI)
			{
				if(EditorApplication.isPaused)
				{
                    GameObject.FindGameObjectWithTag("Player").transform.GetChild(0).gameObject.SetActive(true);
                    Selection.activeGameObject = GameObject.FindGameObjectWithTag("Branch");
					//EditorApplication.Play(); //This would play every time be pause... not ideal.
				}
			}
		}		
        #endregion
		
		//TODO: Build tools?
		
#endif
    }
}
	