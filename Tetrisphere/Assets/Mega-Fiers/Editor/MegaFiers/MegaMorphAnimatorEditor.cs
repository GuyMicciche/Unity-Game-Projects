
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MegaMorphAnimator))]
public class MegaMorphAnimatorEditor : Editor
{
	// TODO: Need none in the popup to clear a channel
	public override void OnInspectorGUI()
	{
		MegaMorphAnimator anim = (MegaMorphAnimator)target;

		string[] clips = anim.GetClipNames();
		anim.LinkedUpdate = EditorGUILayout.Toggle("Linked Update", anim.LinkedUpdate);
		anim.PlayOnStart = EditorGUILayout.Toggle("Play On Start", anim.PlayOnStart);
		anim.current = EditorGUILayout.Popup("Playing Clip", anim.current, clips);

		//anim.t = EditorGUILayout.FloatField("t", anim.t);
		//anim.at = EditorGUILayout.FloatField("at", anim.at);

		if ( GUILayout.Button("Add Clip") )
			anim.AddClip("Clip " + anim.clips.Count, 0.0f, 1.0f, MegaRepeatMode.Loop);

		EditorGUILayout.BeginVertical();
		for ( int i = 0; i < anim.clips.Count; i++ )
		{
			EditorGUILayout.BeginHorizontal();

			//EditorGUILayout.TextArea("" + i + " - ");
			anim.clips[i].name = EditorGUILayout.TextField(anim.clips[i].name);
			anim.clips[i].start = EditorGUILayout.FloatField(anim.clips[i].start, GUILayout.Width(40));
			anim.clips[i].end = EditorGUILayout.FloatField(anim.clips[i].end, GUILayout.Width(40));
			anim.clips[i].loop = (MegaRepeatMode)EditorGUILayout.EnumPopup(anim.clips[i].loop);

			if ( GUILayout.Button("-") )
			{
				anim.clips.Remove(anim.clips[i]);
			}

			EditorGUILayout.EndHorizontal();
		}

		//if ( GUILayout.Button("Debug") )
		//{
		//	DisplayClipInfo();
		//}
	}

	void DisplayClipInfo()
	{
		MegaMorphAnimator mod = (MegaMorphAnimator)target;
		AnimationClip[] clips = AnimationUtility.GetAnimationClips(mod.GetComponent<Animation>());
		Debug.Log("Found " + clips.Length + " clips");

		for ( int i = 0; i < clips.Length; i++ )
		{
			AnimationClipCurveData[] curves = AnimationUtility.GetAllCurves(clips[i], true);

			Debug.Log("Clip[" + clips[i].name + "] has " + curves.Length + " curves");

			for ( int c = 0; c < curves.Length; c++ )
			{
				AnimationCurve acurve = curves[c].curve;

				for ( int k = 0; k < acurve.keys.Length; k++ )
				{
					Debug.Log("Key[" + k + "] ");
					Debug.Log("Time " + acurve.keys[k].time + " val " + acurve.keys[k].value);
				}
			}
		}
	}
}