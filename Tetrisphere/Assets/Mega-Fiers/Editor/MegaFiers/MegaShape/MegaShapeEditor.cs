
using UnityEditor;
using UnityEngine;
using System.IO;
using System.Collections.Generic;

// TODO: SVG import
// TODO: Button to recalc lengths
// TEST: Build a simple scene in max then have a road, barrier, fence etc

[CustomEditor(typeof(MegaShape))]
public class MegaShapeEditor : Editor
{
	int		selected = -1;
	Vector3 pm = new Vector3();
	Vector3 delta = new Vector3();

	bool showsplines = false;
	bool showknots = false;
	bool showlabels = true;

	float ImportScale = 1.0f;
	static public Vector3 CursorPos = Vector3.zero;
	static public Vector3 CursorSpline = Vector3.zero;
	static public Vector3 CursorTangent = Vector3.zero;

	public virtual bool Params()	{ return false; }

	public bool showcommon = true;

	public override void OnInspectorGUI()
	{
		//undoManager.CheckUndo();
		bool buildmesh = false;
		MegaShape shape = (MegaShape)target;

		EditorGUILayout.BeginHorizontal();

		int curve = shape.selcurve;

		if ( GUILayout.Button("Add Knot") )
		{
			if ( shape.splines == null || shape.splines.Count == 0 )
			{
				MegaSpline spline = new MegaSpline();	// Have methods for these
				shape.splines.Add(spline);
			}

			//Undo.RegisterUndo(target, "Add Knot");

			MegaKnot knot = new MegaKnot();
			// Add a point at CursorPos

			//sp = selected + 1;
			//Debug.Log("CursorPos " + CursorPos + " CursorKnot " + CursorKnot);
			float per = CursorPercent * 0.01f;

			CursorTangent = shape.splines[curve].Interpolate(per + 0.01f, true, ref CursorKnot);	//this.GetPositionOnSpline(i) - p;
			CursorPos = shape.splines[curve].Interpolate(per, true, ref CursorKnot);	//this.GetPositionOnSpline(i) - p;

			knot.p = CursorPos;
			//CursorTangent = 
			//Vector3 t = shape.splines[0].knots[selected].Interpolate(0.51f, shape.splines[0].knots[0]);
			knot.outvec = (CursorTangent - knot.p);
			knot.outvec.Normalize();
			knot.outvec *= shape.splines[curve].knots[CursorKnot].seglength * 0.25f;
			knot.invec = -knot.outvec;
			knot.invec += knot.p;
			knot.outvec += knot.p;

			shape.splines[curve].knots.Insert(CursorKnot + 1, knot);
			shape.CalcLength(10);
			EditorUtility.SetDirty(target);
			buildmesh = true;
		}

		if ( GUILayout.Button("Delete Knot") )
		{
			if ( selected != -1 )
			{
				//Undo.RegisterUndo(target, "Delete Knot");
				shape.splines[curve].knots.RemoveAt(selected);
				selected--;
				shape.CalcLength(10);
			}
			EditorUtility.SetDirty(target);
			buildmesh = true;
		}
		
		EditorGUILayout.EndHorizontal();
		EditorGUILayout.BeginHorizontal();

		if ( GUILayout.Button("Match Handles") )
		{
			if ( selected != -1 )
			{
				//Undo.RegisterUndo(target, "Match Handles");

				Vector3 p = shape.splines[curve].knots[selected].p;
				Vector3 d = shape.splines[curve].knots[selected].outvec - p;
				shape.splines[curve].knots[selected].invec = p - d;
				shape.CalcLength(10);
			}
			EditorUtility.SetDirty(target);
			buildmesh = true;
		}

		if ( GUILayout.Button("Load") )
		{
			// Load a spl file from max, so delete everything and replace
			LoadShape(ImportScale);
			buildmesh = true;
		}

		EditorGUILayout.EndHorizontal();

		showcommon = EditorGUILayout.Foldout(showcommon, "Common Params");

		bool rebuild = false;	//Params();

		if ( showcommon )
		{
			//CursorPos = EditorGUILayout.Vector3Field("Cursor", CursorPos);
			CursorPercent = EditorGUILayout.FloatField("Cursor", CursorPercent);
			CursorPercent = Mathf.Repeat(CursorPercent, 100.0f);

			ImportScale = EditorGUILayout.FloatField("Import Scale", ImportScale);

			MegaAxis av = (MegaAxis)EditorGUILayout.EnumPopup("Axis", shape.axis);
			if ( av != shape.axis )
			{
				shape.axis = av;
				rebuild = true;
			}

			if ( shape.splines.Count > 1 )
			{
				//shape.selcurve = EditorGUILayout.IntField("Curve", shape.selcurve);
				shape.selcurve = EditorGUILayout.IntSlider("Curve", shape.selcurve, 0, shape.splines.Count - 1);
			}

			if ( shape.selcurve < 0 )
				shape.selcurve = 0;

			if ( shape.selcurve > shape.splines.Count - 1 )
				shape.selcurve = shape.splines.Count - 1;

			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("Colors");
			//shape.col1 = EditorGUILayout.ColorField("Col 1", shape.col1);
			//shape.col2 = EditorGUILayout.ColorField("Col 2", shape.col2);
			shape.col1 = EditorGUILayout.ColorField(shape.col1);
			shape.col2 = EditorGUILayout.ColorField(shape.col2);
			EditorGUILayout.EndHorizontal();


			//shape.KnotCol = EditorGUILayout.ColorField("Knot Col", shape.KnotCol);
			//shape.HandleCol = EditorGUILayout.ColorField("Handle Col", shape.HandleCol);
			shape.VecCol = EditorGUILayout.ColorField("Vec Col", shape.VecCol);

			shape.KnotSize = EditorGUILayout.FloatField("Knot Size", shape.KnotSize);
			shape.stepdist = EditorGUILayout.FloatField("Step Dist", shape.stepdist);

			if ( shape.stepdist < 0.01f )
				shape.stepdist = 0.01f;

			shape.normalizedInterp = EditorGUILayout.Toggle("Normalized Interp", shape.normalizedInterp);
			shape.drawHandles = EditorGUILayout.Toggle("Draw Handles", shape.drawHandles);
			shape.drawKnots = EditorGUILayout.Toggle("Draw Knots", shape.drawKnots);
			shape.drawspline = EditorGUILayout.Toggle("Draw Spline", shape.drawspline);
			shape.lockhandles = EditorGUILayout.Toggle("Lock Handles", shape.lockhandles);
			showlabels = EditorGUILayout.Toggle("Labels", showlabels);

			shape.animate = EditorGUILayout.Toggle("Animate", shape.animate);
			if ( shape.animate )
			{
				shape.time = EditorGUILayout.FloatField("Time", shape.time);
				shape.MaxTime = EditorGUILayout.FloatField("Loop Time", shape.MaxTime);
				shape.speed = EditorGUILayout.FloatField("Speed", shape.speed);
				shape.LoopMode = (MegaRepeatMode)EditorGUILayout.EnumPopup("Loop Mode", shape.LoopMode);
			}

			// Mesher
			shape.makeMesh = EditorGUILayout.Toggle("Make Mesh", shape.makeMesh);

			if ( shape.makeMesh )
			{
				//shape.meshType = (MeshShapeType)EditorGUILayout.EnumPopup("Mesh Type", shape.meshType);
				shape.Pivot = EditorGUILayout.Vector3Field("Pivot", shape.Pivot);

				shape.CalcTangents = EditorGUILayout.Toggle("Calc Tangents", shape.CalcTangents);
				shape.GenUV = EditorGUILayout.Toggle("Gen UV", shape.GenUV);
				shape.PhysUV = EditorGUILayout.Toggle("Physical UV", shape.PhysUV);
				shape.UVOffset = EditorGUILayout.Vector2Field("UV Offset", shape.UVOffset);
				shape.UVRotate = EditorGUILayout.Vector2Field("UV Rotate", shape.UVRotate);
				shape.UVScale = EditorGUILayout.Vector2Field("UV Scale", shape.UVScale);
				shape.UVOffset1 = EditorGUILayout.Vector2Field("UV Offset1", shape.UVOffset1);
				shape.UVRotate1 = EditorGUILayout.Vector2Field("UV Rotate1", shape.UVRotate1);
				shape.UVScale1 = EditorGUILayout.Vector2Field("UV Scale1", shape.UVScale1);

				switch ( shape.meshType )
				{
					case MeshShapeType.Fill:
						shape.DoubleSided = EditorGUILayout.Toggle("Double Sided", shape.DoubleSided);
						shape.Height = EditorGUILayout.FloatField("Height", shape.Height);
						//shape.HeightSegs = EditorGUILayout.IntField("HeightSegs", shape.HeightSegs);
						shape.UseHeightCurve = EditorGUILayout.Toggle("Use Height Crv", shape.UseHeightCurve);
						if ( shape.UseHeightCurve )
						{
							shape.heightCrv = EditorGUILayout.CurveField("Height Curve", shape.heightCrv);
							shape.heightOff = EditorGUILayout.Slider("Height Off", shape.heightOff, -1.0f, 1.0f);
						}
						break;

					case MeshShapeType.Line:
						shape.DoubleSided = EditorGUILayout.Toggle("Double Sided", shape.DoubleSided);
						shape.Height = EditorGUILayout.FloatField("Height", shape.Height);
						shape.HeightSegs = EditorGUILayout.IntField("HeightSegs", shape.HeightSegs);
						shape.heightCrv = EditorGUILayout.CurveField("Height Curve", shape.heightCrv);
						shape.Start = EditorGUILayout.FloatField("Start", shape.Start);
						shape.End = EditorGUILayout.FloatField("End", shape.End);
						shape.Rotate = EditorGUILayout.FloatField("Rotate", shape.Rotate);
						break;

					case MeshShapeType.Tube:
						shape.Sides = EditorGUILayout.IntField("Sides", shape.Sides);
						shape.TubeStep = EditorGUILayout.FloatField("TubeStep", shape.TubeStep);
						shape.Start = EditorGUILayout.FloatField("Start", shape.Start);
						shape.End = EditorGUILayout.FloatField("End", shape.End);
						break;
				}
			}
			else
			{
				shape.shapemesh = null;
			}

			showsplines = EditorGUILayout.Foldout(showsplines, "Spline Data");

			if ( showsplines )
			{
				EditorGUILayout.BeginVertical("Box");
				DisplaySpline(shape, shape.splines[shape.selcurve]);
				EditorGUILayout.EndVertical();
#if false
				for ( int i = 0; i < shape.splines.Count; i++ )
				{
					// We should only show the selected curve
					EditorGUILayout.BeginVertical("Box");
					DisplaySpline(shape, shape.splines[i]);
					EditorGUILayout.EndVertical();
				}
#endif
			}

			EditorGUILayout.BeginHorizontal();

			Color col = GUI.backgroundColor;
			GUI.backgroundColor = Color.green;
			if ( GUILayout.Button("Add") )
			{
				// Create a new spline in the shape
				MegaSpline spl = MegaSpline.Copy(shape.splines[shape.selcurve]);

				shape.splines.Add(spl);
				shape.selcurve = shape.splines.Count - 1;
				EditorUtility.SetDirty(shape);
			}

			if ( shape.splines.Count > 1 )
			{
				GUI.backgroundColor = Color.red;
				if ( GUILayout.Button("Delete") )
				{
					// Delete current spline
					shape.splines.RemoveAt(shape.selcurve);
					shape.selcurve--;
					if ( shape.selcurve < 0 )
						shape.selcurve = 0;

					EditorUtility.SetDirty(shape);
				}
			}
			GUI.backgroundColor = col;
			EditorGUILayout.EndHorizontal();
		}

		if ( Params() )
		{
			rebuild = true;
		}

		if ( GUI.changed )
		{
			EditorUtility.SetDirty(target);
			//shape.CalcLength(10);
			buildmesh = true;
		}

		if ( rebuild )
		{
			shape.MakeShape();
			EditorUtility.SetDirty(target);
			buildmesh = true;
		}

		if ( buildmesh )
		{
			shape.BuildMesh();
		}

		//undoManager.CheckDirty();
	}

	void DisplayKnot(MegaShape shape, MegaSpline spline, MegaKnot knot)
	{
		bool recalc = false;

		Vector3 p = EditorGUILayout.Vector3Field("Pos", knot.p);
		delta = p - knot.p;

		knot.invec += delta;
		knot.outvec += delta;

		if ( knot.p != p )
		{
			recalc = true;
			knot.p = p;
		}

		if ( recalc )
		{
			shape.CalcLength(10);
		}
	}

	void DisplaySpline(MegaShape shape, MegaSpline spline)
	{
		bool closed = EditorGUILayout.Toggle("Closed", spline.closed);

		if ( closed != spline.closed )
		{
			spline.closed = closed;
			shape.CalcLength(10);
			EditorUtility.SetDirty(target);
			//shape.BuildMesh();
		}

		EditorGUILayout.LabelField("Length ", spline.length.ToString("0.000"));

		showknots = EditorGUILayout.Foldout(showknots, "Knots");

		if ( showknots )
		{
			for ( int i = 0; i < spline.knots.Count; i++ )
			{
				DisplayKnot(shape, spline, spline.knots[i]);
				//EditorGUILayout.Separator();
			}
		}
	}

#if false
	public void OnSceneGUI()
	{
		//Undo.RegisterUndo(target, "Move Shape Points");
		//undoManager.CheckUndo(target);
		//Undo.CreateSnapshot();

		MegaShape shape = (MegaShape)target;

		Handles.matrix = shape.transform.localToWorldMatrix;

		Quaternion rot = shape.transform.rotation;
		Vector3 trans = shape.transform.position;
		Handles.matrix = Matrix4x4.TRS(trans, rot, Vector3.one);

		if ( shape.selcurve > shape.splines.Count - 1 )
			shape.selcurve = 0;

		bool recalc = false;

		Vector3 dragplane = Vector3.one;

		Color nocol = new Color(0, 0, 0, 0);

		for ( int s = 0; s < shape.splines.Count; s++ )
		{
			for ( int p = 0; p < shape.splines[s].knots.Count; p++ )
			{
				if ( shape.drawKnots && s == shape.selcurve )
				{
					pm = shape.splines[s].knots[p].p;

					if ( showlabels )
					{
						if ( p == selected && s == shape.selcurve )
						{
							Handles.color = Color.white;
							Handles.Label(pm, " Selected\n" + pm.ToString("0.000"));
						}
						else
						{
							Handles.color = shape.KnotCol;
							Handles.Label(pm, " " + p);
						}
					}

					Handles.color = nocol;
					Vector3 newp = Handles.PositionHandle(pm, Quaternion.identity);
					if ( newp != pm )
					{
						Undo.SetSnapshotTarget(shape, "Knot Move");
					}
					shape.splines[s].knots[p].p += Vector3.Scale(newp - pm, dragplane);

					delta = shape.splines[s].knots[p].p - pm;

					shape.splines[s].knots[p].invec += delta;
					shape.splines[s].knots[p].outvec += delta;

					if ( shape.splines[s].knots[p].p != pm )
					{
						selected = p;
						recalc = true;
					}

					pm = shape.splines[s].knots[p].p;
				}

				if ( shape.drawHandles && s == shape.selcurve )
				{
					Handles.color = shape.VecCol;
					pm = shape.splines[s].knots[p].p;

					Vector3 ip = shape.splines[s].knots[p].invec;
					Vector3 op = shape.splines[s].knots[p].outvec;
					Handles.DrawLine(pm, ip);
					Handles.DrawLine(pm, op);

					Handles.color = shape.HandleCol;

					Vector3 invec = shape.splines[s].knots[p].invec;
					Handles.color = nocol;
					Vector3 newinvec = Handles.PositionHandle(shape.splines[s].knots[p].invec, Quaternion.identity);

					if ( newinvec != shape.splines[s].knots[p].invec )
					{
						Undo.SetSnapshotTarget(shape, "Handle Move");
					}
					invec += Vector3.Scale(newinvec - invec, dragplane);
					if ( invec != shape.splines[s].knots[p].invec )
					{
						if ( shape.lockhandles )
						{
							Vector3 d = invec - shape.splines[s].knots[p].invec;
							shape.splines[s].knots[p].outvec -= d;
						}

						shape.splines[s].knots[p].invec = invec;
						selected = p;
						recalc = true;
					}
					Vector3 outvec = shape.splines[s].knots[p].outvec;

					Vector3 newoutvec = Handles.PositionHandle(shape.splines[s].knots[p].outvec, Quaternion.identity);
					if ( newoutvec != shape.splines[s].knots[p].outvec )
					{
						Undo.SetSnapshotTarget(shape, "Handle Move");
					}
					outvec += Vector3.Scale(newoutvec - outvec, dragplane);

					if ( outvec != shape.splines[s].knots[p].outvec )
					{
						if ( shape.lockhandles )
						{
							Vector3 d = outvec - shape.splines[s].knots[p].outvec;
							shape.splines[s].knots[p].invec -= d;
						}

						shape.splines[s].knots[p].outvec = outvec;
						selected = p;
						recalc = true;
					}
					Vector3 hp = shape.splines[s].knots[p].invec;
					if ( selected == p )
						Handles.Label(hp, " " + p);

					hp = shape.splines[s].knots[p].outvec;
					
					if ( selected == p )
						Handles.Label(hp, " " + p);
				}
			}
		}

		// Draw nearest point (use for adding knot)
		CursorPos = Handles.PositionHandle(CursorPos, Quaternion.identity);
		float calpha = 0.0f;
		CursorPos = shape.FindNearestPoint(CursorPos, 5, ref CursorKnot, ref CursorTangent, ref calpha);
		CursorPercent = calpha * 100.0f;
		Handles.Label(CursorPos, "Cursor " + CursorPercent.ToString("0.00") + "% - " + CursorPos);

		if ( recalc )
		{
			shape.CalcLength(10);
			shape.BuildMesh();
		}

		Handles.matrix = Matrix4x4.identity;
		//undoManager.CheckDirty(target);

		if ( GUI.changed )
		{
			//Undo.RegisterCreatedObjectUndo(shape, "plop");
			Undo.CreateSnapshot();
			Undo.RegisterSnapshot(); 
		}

		Undo.ClearSnapshotTarget();
	}
#else
	public void OnSceneGUI()
	{
		//Undo.RegisterUndo(target, "Move Shape Points");
		//undoManager.CheckUndo(target);
		//Undo.CreateSnapshot();

		MegaShape shape = (MegaShape)target;

		Handles.matrix = Matrix4x4.identity;	//shape.transform.localToWorldMatrix;

		//Quaternion rot = shape.transform.rotation;
		//Vector3 trans = shape.transform.position;
		Matrix4x4 tm = shape.transform.localToWorldMatrix;	//Matrix4x4.TRS(trans, rot, Vector3.one);

		if ( shape.selcurve > shape.splines.Count - 1 )
			shape.selcurve = 0;

		bool recalc = false;

		Vector3 dragplane = Vector3.one;

		Color nocol = new Color(0, 0, 0, 0);

		for ( int s = 0; s < shape.splines.Count; s++ )
		{
			for ( int p = 0; p < shape.splines[s].knots.Count; p++ )
			{
				if ( shape.drawKnots && s == shape.selcurve )
				{
					pm = tm.MultiplyPoint(shape.splines[s].knots[p].p);

					if ( showlabels )
					{
						if ( p == selected && s == shape.selcurve )
						{
							Handles.color = Color.white;
							Handles.Label(pm, " Selected\n" + pm.ToString("0.000"));
						}
						else
						{
							Handles.color = shape.KnotCol;
							Handles.Label(pm, " " + p);
						}
					}

					Handles.color = nocol;
					Vector3 newp = Handles.PositionHandle(pm, Quaternion.identity);
					if ( newp != pm )
					{
						Undo.SetSnapshotTarget(shape, "Knot Move");
					}

					Vector3 dl = Vector3.Scale(newp - pm, dragplane);
					shape.splines[s].knots[p].p += dl;	//Vector3.Scale(newp - pm, dragplane);

					//delta = shape.splines[s].knots[p].p - pm;

					shape.splines[s].knots[p].invec += dl;	//delta;
					shape.splines[s].knots[p].outvec += dl;	//delta;

					//if ( shape.splines[s].knots[p].p != pm )
					if ( newp != pm )
					{
						selected = p;
						recalc = true;
					}

					//pm = shape.splines[s].knots[p].p;
				}

#if true
				if ( shape.drawHandles && s == shape.selcurve )
				{
					Handles.color = shape.VecCol;
					pm = tm.MultiplyPoint(shape.splines[s].knots[p].p);

					Vector3 ip = tm.MultiplyPoint(shape.splines[s].knots[p].invec);
					Vector3 op = tm.MultiplyPoint(shape.splines[s].knots[p].outvec);
					Handles.DrawLine(pm, ip);
					Handles.DrawLine(pm, op);

					Handles.color = shape.HandleCol;

					Vector3 invec = tm.MultiplyPoint(shape.splines[s].knots[p].invec);
					Handles.color = nocol;
					Vector3 newinvec = Handles.PositionHandle(invec, Quaternion.identity);

					if ( newinvec != invec )	//shape.splines[s].knots[p].invec )
					{
						Undo.SetSnapshotTarget(shape, "Handle Move");
					}
					Vector3 dl = Vector3.Scale(newinvec - invec, dragplane);
					shape.splines[s].knots[p].invec += dl;	//Vector3.Scale(newinvec - invec, dragplane);
					if ( invec != newinvec )	//shape.splines[s].knots[p].invec )
					{
						if ( shape.lockhandles )
						{
							//Vector3 d = invec - shape.splines[s].knots[p].invec;
							shape.splines[s].knots[p].outvec -= dl;
						}

						//shape.splines[s].knots[p].invec = invec;
						selected = p;
						recalc = true;
					}
					Vector3 outvec = tm.MultiplyPoint(shape.splines[s].knots[p].outvec);

					Vector3 newoutvec = Handles.PositionHandle(outvec, Quaternion.identity);
					if ( newoutvec != outvec )	//shape.splines[s].knots[p].outvec )
					{
						Undo.SetSnapshotTarget(shape, "Handle Move");
					}
					dl = Vector3.Scale(newoutvec - outvec, dragplane);
					//outvec += dl;	//Vector3.Scale(newoutvec - outvec, dragplane);
					shape.splines[s].knots[p].outvec += dl;
					if ( outvec != newoutvec )	//shape.splines[s].knots[p].outvec )
					{
						if ( shape.lockhandles )
						{
							//Vector3 d = outvec - shape.splines[s].knots[p].outvec;
							shape.splines[s].knots[p].invec -= dl;
						}

						//shape.splines[s].knots[p].outvec = outvec;
						selected = p;
						recalc = true;
					}
					Vector3 hp = tm.MultiplyPoint(shape.splines[s].knots[p].invec);
					if ( selected == p )
						Handles.Label(hp, " " + p);

					hp = tm.MultiplyPoint(shape.splines[s].knots[p].outvec);

					if ( selected == p )
						Handles.Label(hp, " " + p);
				}
#endif
			}
		}

		// Draw nearest point (use for adding knot)
		Vector3 wcp = tm.MultiplyPoint(CursorPos);
		Vector3 newCursorPos = Handles.PositionHandle(wcp, Quaternion.identity);
		
		if ( newCursorPos != wcp )
		{
			Vector3 cd = newCursorPos - wcp;

			CursorPos += cd;

			float calpha = 0.0f;
			CursorPos = shape.FindNearestPoint(CursorPos, 5, ref CursorKnot, ref CursorTangent, ref calpha);
			CursorPercent = calpha * 100.0f;
		}

		Handles.Label(tm.MultiplyPoint(CursorPos), "Cursor " + CursorPercent.ToString("0.00") + "% - " + CursorPos);

		if ( recalc )
		{
			shape.CalcLength(10);
			shape.BuildMesh();
		}

		Handles.matrix = Matrix4x4.identity;
		//undoManager.CheckDirty(target);

		if ( GUI.changed )
		{
			//Undo.RegisterCreatedObjectUndo(shape, "plop");
			Undo.CreateSnapshot();
			Undo.RegisterSnapshot();
		}

		Undo.ClearSnapshotTarget();
	}
#endif

	[DrawGizmo(GizmoType.NotInSelectionHierarchy | GizmoType.Pickable)]
	static void RenderGizmo(MegaShape shape, GizmoType gizmoType)
	{
		if ( (gizmoType & GizmoType.NotInSelectionHierarchy) != 0 )
		{
			if ( (gizmoType & GizmoType.Active) != 0 )
			{
				DrawGizmos(shape, new Color(1.0f, 1.0f, 1.0f, 1.0f));
				Color col = Color.yellow;
				col.a = 0.5f;
				Gizmos.color = col;	//Color.yellow;
				CursorPos = shape.InterpCurve3D(0, CursorPercent * 0.01f, true);
				Gizmos.DrawSphere(shape.transform.TransformPoint(CursorPos), shape.KnotSize * 0.01f);
				Handles.color = Color.white;
				//Handles.Label(shape.transform.TransformPoint(CursorPos), "Cursor " + CursorPercent.ToString("0.00") + "% - " + CursorPos);
			}
			else
				DrawGizmos(shape, new Color(1.0f, 1.0f, 1.0f, 0.25f));
		}
		Gizmos.DrawIcon(shape.transform.position, "MegaSpherify icon.png", false);
		Handles.Label(shape.transform.position, " " + shape.name);
	}

	static public int CursorKnot = 0;
	static public float CursorPercent = 0.0f;

	// Dont want this in here, want in editor
	// If we go over a knot then should draw to the knot
#if false
	static void DrawGizmos(MegaShape shape, Color modcol)
	{
		if ( ((1 << shape.gameObject.layer) & Camera.current.cullingMask) == 0 )
			return;

		if ( !shape.drawspline )
			return;

		Matrix4x4 tm = shape.transform.localToWorldMatrix;

		for ( int s = 0; s < shape.splines.Count; s++ )
		{
			float ldist = shape.stepdist * 0.1f;
			if ( ldist < 0.01f )
				ldist = 0.01f;

			if ( shape.splines[s].length / ldist > 500.0f )
			{
				ldist = shape.splines[s].length / 500.0f;
			}

			float ds = shape.splines[s].length / (shape.splines[s].length / ldist);

			if ( ds > shape.splines[s].length )
			{
				ds = shape.splines[s].length;
			}

			int c	= 0;
			int k	= -1;
			int lk	= -1;

			Vector3 first = shape.splines[s].Interpolate(0.0f, shape.normalizedInterp, ref lk);

			for ( float dist = ds; dist < shape.splines[s].length; dist += ds )
			{
				float alpha = dist / shape.splines[s].length;
				Vector3 pos = shape.splines[s].Interpolate(alpha, shape.normalizedInterp, ref k);

				if ( (c & 1) == 1 )
					Gizmos.color = shape.col1 * modcol;
				else
					Gizmos.color = shape.col2 * modcol;

				if ( k != lk )
				{
					for ( lk = lk + 1; lk <= k; lk++ )
					{
						Gizmos.DrawLine(shape.transform.TransformPoint(first), shape.transform.TransformPoint(shape.splines[s].knots[lk].p));
						first = shape.splines[s].knots[lk].p;
					}
				}

				lk = k;

				Gizmos.DrawLine(shape.transform.TransformPoint(first), shape.transform.TransformPoint(pos));

				c++;

				first = pos;
			}

			if ( (c & 1) == 1 )
				Gizmos.color = shape.col1 * modcol;
			else
				Gizmos.color = shape.col2 * modcol;

			Vector3 lastpos;
			if ( shape.splines[s].closed )
				lastpos = shape.splines[s].Interpolate(0.0f, shape.normalizedInterp, ref k);
			else
				lastpos = shape.splines[s].Interpolate(1.0f, shape.normalizedInterp, ref k);

			Gizmos.DrawLine(shape.transform.TransformPoint(first), shape.transform.TransformPoint(lastpos));
		}
	}
#else
	static void DrawGizmos(MegaShape shape, Color modcol)
	{
		if ( ((1 << shape.gameObject.layer) & Camera.current.cullingMask) == 0 )
			return;

		if ( !shape.drawspline )
			return;

		Matrix4x4 tm = shape.transform.localToWorldMatrix;

		for ( int s = 0; s < shape.splines.Count; s++ )
		{
			float ldist = shape.stepdist * 0.1f;
			if ( ldist < 0.01f )
				ldist = 0.01f;

			if ( shape.splines[s].length / ldist > 500.0f )
			{
				ldist = shape.splines[s].length / 500.0f;
			}

			float ds = shape.splines[s].length / (shape.splines[s].length / ldist);

			if ( ds > shape.splines[s].length )
			{
				ds = shape.splines[s].length;
			}

			int c	= 0;
			int k	= -1;
			int lk	= -1;

			Vector3 first = shape.splines[s].Interpolate(0.0f, shape.normalizedInterp, ref lk);

			for ( float dist = ds; dist < shape.splines[s].length; dist += ds )
			{
				float alpha = dist / shape.splines[s].length;
				Vector3 pos = shape.splines[s].Interpolate(alpha, shape.normalizedInterp, ref k);

				if ( (c & 1) == 1 )
					Gizmos.color = shape.col1 * modcol;
				else
					Gizmos.color = shape.col2 * modcol;

				if ( k != lk )
				{
					for ( lk = lk + 1; lk <= k; lk++ )
					{
						Gizmos.DrawLine(tm.MultiplyPoint(first), tm.MultiplyPoint(shape.splines[s].knots[lk].p));
						first = shape.splines[s].knots[lk].p;
					}
				}

				lk = k;

				Gizmos.DrawLine(tm.MultiplyPoint(first), tm.MultiplyPoint(pos));

				c++;

				first = pos;
			}

			if ( (c & 1) == 1 )
				Gizmos.color = shape.col1 * modcol;
			else
				Gizmos.color = shape.col2 * modcol;

			Vector3 lastpos;
			if ( shape.splines[s].closed )
				lastpos = shape.splines[s].Interpolate(0.0f, shape.normalizedInterp, ref k);
			else
				lastpos = shape.splines[s].Interpolate(1.0f, shape.normalizedInterp, ref k);

			Gizmos.DrawLine(tm.MultiplyPoint(first), tm.MultiplyPoint(lastpos));
		}
	}
#endif

	// Load stuff
	string lastpath = "";

	public delegate bool ParseBinCallbackType(BinaryReader br, string id);
	public delegate void ParseClassCallbackType(string classname, BinaryReader br);

	void LoadShape(float scale)
	{
		MegaShape ms = (MegaShape)target;
		//Modifiers mod = mr.GetComponent<Modifiers>();	// Do this at start and store

		string filename = EditorUtility.OpenFilePanel("Shape File", lastpath, "spl");

		if ( filename == null || filename.Length < 1 )
			return;

		lastpath = filename;

		// Clear what we have
		ms.splines.Clear();

		ParseFile(filename, ShapeCallback);

		ms.Scale(scale);

		ms.MaxTime = 0.0f;

		for ( int s = 0; s < ms.splines.Count; s++ )
		{
			if ( ms.splines[s].animations != null )
			{
				for ( int a = 0; a < ms.splines[s].animations.Count; a++ )
				{
					MegaControl con = ms.splines[s].animations[a].con;
					if ( con != null )
					{
						float t = con.Times[con.Times.Length - 1];
						if ( t > ms.MaxTime )
							ms.MaxTime = t;
					}
				}
			}
		}
	}

	public void ShapeCallback(string classname, BinaryReader br)
	{
		switch ( classname )
		{
			case "Shape": LoadShape(br); break;
		}
	}

	public void LoadShape(BinaryReader br)
	{
		//MegaMorphEditor.Parse(br, ParseShape);
		MegaParse.Parse(br, ParseShape);
	}

	public void ParseFile(string assetpath, ParseClassCallbackType cb)
	{
		FileStream fs = new FileStream(assetpath, FileMode.Open, FileAccess.Read, System.IO.FileShare.Read);

		BinaryReader br = new BinaryReader(fs);

		bool processing = true;

		while ( processing )
		{
			string classname = MegaParse.ReadString(br);

			if ( classname == "Done" )
				break;

			int	chunkoff = br.ReadInt32();
			long fpos = fs.Position;

			cb(classname, br);

			fs.Position = fpos + chunkoff;
		}

		br.Close();
	}

	static public Vector3 ReadP3(BinaryReader br)
	{
		Vector3 p = Vector3.zero;

		p.x = br.ReadSingle();
		p.y = br.ReadSingle();
		p.z = br.ReadSingle();

		return p;
	}

	bool SplineParse(BinaryReader br, string cid)
	{
		MegaShape ms = (MegaShape)target;
		MegaSpline ps = ms.splines[ms.splines.Count - 1];

		switch ( cid )
		{
			case "Transform":
				Vector3 pos = ReadP3(br);
				Vector3 rot = ReadP3(br);
				Vector3 scl = ReadP3(br);
				rot.y = -rot.y;
				ms.transform.position = pos;
				ms.transform.rotation = Quaternion.Euler(rot * Mathf.Rad2Deg);
				ms.transform.localScale = scl;
				break;

			case "Flags":
				int count = br.ReadInt32();
				ps.closed = (br.ReadInt32() == 1);
				count = br.ReadInt32();
				ps.knots = new List<MegaKnot>(count);
				ps.length = 0.0f;
				break;

			case "Knots":
				for ( int i = 0; i < ps.knots.Capacity; i++ )
				{
					MegaKnot pk = new MegaKnot();

					pk.p = ReadP3(br);
					pk.invec = ReadP3(br);
					pk.outvec = ReadP3(br);
					pk.seglength = br.ReadSingle();

					ps.length += pk.seglength;
					pk.length = ps.length;
					ps.knots.Add(pk);
				}
				break;
		}
		return true;
	}

	MegaKnotAnim ma;

	bool AnimParse(BinaryReader br, string cid)
	{
		MegaShape ms = (MegaShape)target;

		switch ( cid )
		{
			case "V":
				int v = br.ReadInt32();
				ma = new MegaKnotAnim();
				int s = ms.GetSpline(v, ref ma);	//.s, ref ma.p, ref ma.t);

				if ( ms.splines[s].animations == null )
					ms.splines[s].animations = new List<MegaKnotAnim>();

				ms.splines[s].animations.Add(ma);
				break;

			case "Anim":
				ma.con = MegaParseBezVector3Control.LoadBezVector3KeyControl(br);
				break;
		}
		return true;
	}

	bool ParseShape(BinaryReader br, string cid)
	{
		MegaShape ms = (MegaShape)target;

		switch ( cid )
		{
			case "Num":
				int count = br.ReadInt32();
				ms.splines = new List<MegaSpline>(count);
				break;

			case "Spline":
				MegaSpline spl = new MegaSpline();
				ms.splines.Add(spl);
				MegaParse.Parse(br, SplineParse);
				break;

			case "Anim":
				MegaParse.Parse(br, AnimParse);
				break;
		}

		return true;
	}
}
