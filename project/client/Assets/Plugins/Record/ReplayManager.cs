/**
 	录像管理器,负责录像的处理
 	Added by Teng.
 */
using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;

public class ReplayManager 
{

	[DllImport("__Internal")]
	private static extern void startRecording();
	public static void StartRecording()
	{
		if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			startRecording();
		}
	}

	[DllImport("__Internal")]
	private static extern void stopRecording();
	public static void StopRecording()
	{
		if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			stopRecording();
		}
	}

	[DllImport("__Internal")]
	private static extern void discardRecording();
	public static void DiscardRecording()
	{
		if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			discardRecording();
		}
	}

	[DllImport("__Internal")]
	private static extern void displayRecordingContent();
	public static void DisplayRecordingContent()
	{
		if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			displayRecordingContent();
		}
	}
}