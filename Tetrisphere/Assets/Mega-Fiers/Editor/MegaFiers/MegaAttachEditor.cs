
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MegaAttach))]
public class MegaAttachEditor : Editor
{
	public override void OnInspectorGUI()
	{
		MegaAttach mod = (MegaAttach)target;

		EditorGUIUtility.LookLikeControls();
		DrawDefaultInspector();

		if ( GUILayout.Button("Attach") )
		{
			mod.AttachIt();
		}

		if ( GUILayout.Button("Detach") )
		{
			mod.DetachIt();
		}
	}
}