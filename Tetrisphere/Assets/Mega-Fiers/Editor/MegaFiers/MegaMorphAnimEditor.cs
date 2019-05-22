
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MegaMorphAnim))]
public class MegaMorphAnimEditor : Editor
{
	int GetIndex(string name, string[] channels)
	{
		int index = -1;
		for ( int i = 0; i < channels.Length; i++ )
		{
			if ( channels[i] == name )
			{
				index = i;
				break;
			}
		}
		return index;
	}

	// TODO: Need none in the popup to clear a channel
	public override void OnInspectorGUI()
	{
		MegaMorphAnim anim = (MegaMorphAnim)target;

		MegaMorph morph = anim.gameObject.GetComponent<MegaMorph>();

		if ( morph != null )
		{
			string[] channels = morph.GetChannelNames();

			int index = GetIndex(anim.SrcChannel, channels);
			index = EditorGUILayout.Popup("Source Channel", index, channels);

			if ( index != -1 )
			{
				anim.SrcChannel = channels[index];
				anim.SetChannel(morph, 0);
			}

			float min = 0.0f;
			float max = 100.0f;
			anim.GetMinMax(morph, 0, ref min, ref max);

			anim.Percent = EditorGUILayout.Slider("Percent", anim.Percent, min, max);

			if ( index != -1 )
			{
				index = GetIndex(anim.SrcChannel1, channels);
				index = EditorGUILayout.Popup("Source Channel", index, channels);
				if ( index != -1 )
				{
					anim.SrcChannel1 = channels[index];
					anim.SetChannel(morph, 1);
				}
				anim.GetMinMax(morph, 1, ref min, ref max);
				anim.Percent1 = EditorGUILayout.Slider("Percent", anim.Percent1, min, max);
			}

			if ( index != -1 )
			{
				index = GetIndex(anim.SrcChannel2, channels);
				index = EditorGUILayout.Popup("Source Channel", index, channels);
				if ( index != -1 )
				{
					anim.SrcChannel2 = channels[index];
					anim.SetChannel(morph, 2);
				}
				anim.GetMinMax(morph, 2, ref min, ref max);
				anim.Percent2 = EditorGUILayout.Slider("Percent", anim.Percent2, min, max);
			}

			if ( index != -1 )
			{
				index = GetIndex(anim.SrcChannel3, channels);
				index = EditorGUILayout.Popup("Source Channel", index, channels);
				if ( index != -1 )
				{
					anim.SrcChannel3 = channels[index];
					anim.SetChannel(morph, 3);
				}
				anim.GetMinMax(morph, 3, ref min, ref max);
				anim.Percent3 = EditorGUILayout.Slider("Percent", anim.Percent3, min, max);
			}

			if ( index != -1 )
			{
				index = GetIndex(anim.SrcChannel4, channels);
				index = EditorGUILayout.Popup("Source Channel", index, channels);
				if ( index != -1 )
				{
					anim.SrcChannel4 = channels[index];
					anim.SetChannel(morph, 4);
				}
				anim.GetMinMax(morph, 4, ref min, ref max);
				anim.Percent4 = EditorGUILayout.Slider("Percent", anim.Percent4, min, max);
			}

			if ( index != -1 )
			{
				index = GetIndex(anim.SrcChannel5, channels);
				index = EditorGUILayout.Popup("Source Channel", index, channels);
				if ( index != -1 )
				{
					anim.SrcChannel5 = channels[index];
					anim.SetChannel(morph, 5);
				}
				anim.GetMinMax(morph, 5, ref min, ref max);
				anim.Percent5 = EditorGUILayout.Slider("Percent", anim.Percent5, min, max);
			}

			if ( index != -1 )
			{
				index = GetIndex(anim.SrcChannel6, channels);
				index = EditorGUILayout.Popup("Source Channel", index, channels);
				if ( index != -1 )
				{
					anim.SrcChannel6 = channels[index];
					anim.SetChannel(morph, 6);
				}
				anim.GetMinMax(morph, 6, ref min, ref max);
				anim.Percent6 = EditorGUILayout.Slider("Percent", anim.Percent6, min, max);
			}

			if ( index != -1 )
			{
				index = GetIndex(anim.SrcChannel7, channels);
				index = EditorGUILayout.Popup("Source Channel", index, channels);
				if ( index != -1 )
				{
					anim.SrcChannel7 = channels[index];
					anim.SetChannel(morph, 7);
				}
				anim.GetMinMax(morph, 7, ref min, ref max);
				anim.Percent7 = EditorGUILayout.Slider("Percent", anim.Percent7, min, max);
			}

			if ( index != -1 )
			{
				index = GetIndex(anim.SrcChannel8, channels);
				index = EditorGUILayout.Popup("Source Channel", index, channels);
				if ( index != -1 )
				{
					anim.SrcChannel8 = channels[index];
					anim.SetChannel(morph, 8);
				}
				anim.GetMinMax(morph, 8, ref min, ref max);
				anim.Percent8 = EditorGUILayout.Slider("Percent", anim.Percent8, min, max);
			}

			if ( index != -1 )
			{
				index = GetIndex(anim.SrcChannel9, channels);
				index = EditorGUILayout.Popup("Source Channel", index, channels);
				if ( index != -1 )
				{
					anim.SrcChannel9 = channels[index];
					anim.SetChannel(morph, 9);
				}
				anim.GetMinMax(morph, 9, ref min, ref max);
				anim.Percent9 = EditorGUILayout.Slider("Percent", anim.Percent9, min, max);
			}

			if ( index != -1 )
			{
				index = GetIndex(anim.SrcChannel10, channels);
				index = EditorGUILayout.Popup("Source Channel", index, channels);

				if ( index != -1 )
				{
					anim.SrcChannel10 = channels[index];
					anim.SetChannel(morph, 10);
				}
				anim.GetMinMax(morph, 10, ref min, ref max);
				anim.Percent10 = EditorGUILayout.Slider("Percent", anim.Percent10, min, max);
			}

			if ( index != -1 )
			{
				index = GetIndex(anim.SrcChannel11, channels);
				index = EditorGUILayout.Popup("Source Channel", index, channels);
				if ( index != -1 )
				{
					anim.SrcChannel11 = channels[index];
					anim.SetChannel(morph, 11);
				}
				anim.GetMinMax(morph, 11, ref min, ref max);
				anim.Percent11 = EditorGUILayout.Slider("Percent", anim.Percent11, min, max);
			}

			if ( index != -1 )
			{
				index = GetIndex(anim.SrcChannel12, channels);
				index = EditorGUILayout.Popup("Source Channel", index, channels);
				if ( index != -1 )
				{
					anim.SrcChannel12 = channels[index];
					anim.SetChannel(morph, 12);
				}
				anim.GetMinMax(morph, 12, ref min, ref max);
				anim.Percent12 = EditorGUILayout.Slider("Percent", anim.Percent12, min, max);
			}

			if ( index != -1 )
			{
				index = GetIndex(anim.SrcChannel13, channels);
				index = EditorGUILayout.Popup("Source Channel", index, channels);
				if ( index != -1 )
				{
					anim.SrcChannel13 = channels[index];
					anim.SetChannel(morph, 13);
				}
				anim.GetMinMax(morph, 13, ref min, ref max);
				anim.Percent13 = EditorGUILayout.Slider("Percent", anim.Percent13, min, max);
			}

			if ( index != -1 )
			{
				index = GetIndex(anim.SrcChannel14, channels);
				index = EditorGUILayout.Popup("Source Channel", index, channels);
				if ( index != -1 )
				{
					anim.SrcChannel14 = channels[index];
					anim.SetChannel(morph, 14);
				}
				anim.GetMinMax(morph, 14, ref min, ref max);
				anim.Percent14 = EditorGUILayout.Slider("Percent", anim.Percent14, min, max);
			}

			if ( index != -1 )
			{
				index = GetIndex(anim.SrcChannel15, channels);
				index = EditorGUILayout.Popup("Source Channel", index, channels);
				if ( index != -1 )
				{
					anim.SrcChannel15 = channels[index];
					anim.SetChannel(morph, 15);
				}
				anim.GetMinMax(morph, 15, ref min, ref max);
				anim.Percent15 = EditorGUILayout.Slider("Percent", anim.Percent15, min, max);
			}

			if ( index != -1 )
			{
				index = GetIndex(anim.SrcChannel16, channels);
				index = EditorGUILayout.Popup("Source Channel", index, channels);
				if ( index != -1 )
				{
					anim.SrcChannel16 = channels[index];
					anim.SetChannel(morph, 16);
				}
				anim.GetMinMax(morph, 16, ref min, ref max);
				anim.Percent16 = EditorGUILayout.Slider("Percent", anim.Percent16, min, max);
			}

			if ( index != -1 )
			{
				index = GetIndex(anim.SrcChannel17, channels);
				index = EditorGUILayout.Popup("Source Channel", index, channels);
				if ( index != -1 )
				{
					anim.SrcChannel17 = channels[index];
					anim.SetChannel(morph, 17);
				}
				anim.GetMinMax(morph, 17, ref min, ref max);
				anim.Percent17 = EditorGUILayout.Slider("Percent", anim.Percent17, min, max);
			}

			if ( index != -1 )
			{
				index = GetIndex(anim.SrcChannel18, channels);
				index = EditorGUILayout.Popup("Source Channel", index, channels);
				if ( index != -1 )
				{
					anim.SrcChannel18 = channels[index];
					anim.SetChannel(morph, 18);
				}
				anim.GetMinMax(morph, 18, ref min, ref max);
				anim.Percent18 = EditorGUILayout.Slider("Percent", anim.Percent18, min, max);
			}

			if ( index != -1 )
			{
				index = GetIndex(anim.SrcChannel19, channels);
				index = EditorGUILayout.Popup("Source Channel", index, channels);
				if ( index != -1 )
				{
					anim.SrcChannel19 = channels[index];
					anim.SetChannel(morph, 19);
				}
				anim.GetMinMax(morph, 19, ref min, ref max);
				anim.Percent19 = EditorGUILayout.Slider("Percent", anim.Percent19, min, max);
			}

			if ( GUI.changed )
			{
				EditorUtility.SetDirty(target);
			}
		}
	}
}
