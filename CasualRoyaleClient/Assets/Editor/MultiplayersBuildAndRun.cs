using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class MultiplayersBuildAndRun
{
    #region WindowsBuilds

    [MenuItem("Tools/Run Multiplayer/Windows/1 Players")]
	static void PerformWin64Build1()
	{
		PerformWin64Build(1);
	}

	[MenuItem("Tools/Run Multiplayer/Windows/2 Players")]
	static void PerformWin64Build2()
	{
		PerformWin64Build(2);
	}

	[MenuItem("Tools/Run Multiplayer/Windows/3 Players")]
	static void PerformWin64Build3()
	{
		PerformWin64Build(3);
	}

	[MenuItem("Tools/Run Multiplayer/Windows/4 Players")]
	static void PerformWin64Build4()
	{
		PerformWin64Build(4);
	}

	#endregion

	#region AndroidBuilds

	[MenuItem("Tools/Run Multiplayer/Android/1 Players")]
	static void PerformAndroidBuild1()
	{
		PerformAndroidBuild(1);
	}

	[MenuItem("Tools/Run Multiplayer/Android/2 Players")]
	static void PerformAndroidBuild2()
	{
		PerformAndroidBuild(2);
	}

	[MenuItem("Tools/Run Multiplayer/Android/3 Players")]
	static void PerformAndroidBuild3()
	{
		PerformAndroidBuild(3);
	}

	[MenuItem("Tools/Run Multiplayer/Android/4 Players")]
	static void PerformAndroidBuild4()
	{
		PerformAndroidBuild(4);
	}

    #endregion

    static void PerformWin64Build(int playerCount)
	{
		EditorUserBuildSettings.SwitchActiveBuildTarget(
			BuildTargetGroup.Standalone, BuildTarget.StandaloneWindows);

		for (int i = 1; i <= playerCount; i++)
		{
			BuildPipeline.BuildPlayer(GetScenePaths(),
				"Builds/Win64/" + GetProjectName() + i.ToString() + "/" + GetProjectName() + i.ToString() + ".exe",
				BuildTarget.StandaloneWindows64, BuildOptions.AutoRunPlayer);
		}
	}

	static void PerformAndroidBuild(int playerCount)
	{
		EditorUserBuildSettings.SwitchActiveBuildTarget(
			BuildTargetGroup.Android, BuildTarget.Android);

		for (int i = 1; i <= playerCount; i++)
		{
			BuildPipeline.BuildPlayer(GetScenePaths(),
				"Builds/Android/" + GetProjectName() + i.ToString() + "/" + GetProjectName() + i.ToString() + ".apk",
				BuildTarget.Android, BuildOptions.AutoRunPlayer);
		}
	}

	static string GetProjectName()
	{
		string[] s = Application.dataPath.Split('/');
		return s[s.Length - 2];
	}

	static string[] GetScenePaths()
	{
		string[] scenes = new string[EditorBuildSettings.scenes.Length];

		for (int i = 0; i < scenes.Length; i++)
		{
			scenes[i] = EditorBuildSettings.scenes[i].path;
		}

		return scenes;
	}
}
