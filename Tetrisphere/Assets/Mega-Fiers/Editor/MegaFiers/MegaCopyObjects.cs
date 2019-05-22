
using UnityEngine;
using UnityEditor;
using System.Reflection;

#if !UNITY_FLASH
public class MegaCopyObjects : MonoBehaviour
{
	[MenuItem("GameObject/Mega Copy Object")]
	static void DoCopyObjects()
	{
		GameObject from = Selection.activeGameObject;
		MegaCopyObject.DoCopyObjects(from);
	}

	[MenuItem("GameObject/Mega Copy Hier")]
	static void DoCopyObjectsHier()
	{
		GameObject from = Selection.activeGameObject;
		MegaCopyObject.DoCopyObjectsChildren(from);
	}
}
#endif