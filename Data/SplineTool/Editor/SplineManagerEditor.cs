using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace UPDB.Data.SplineTool
{
	/// <summary>
	/// 
	/// </summary>
	[CustomEditor(typeof(SplineManager))]
	public class SplineManagerEditor : Editor
	{
		public override void OnInspectorGUI()
		{
			SplineManager myTarget = (SplineManager)target;

			base.OnInspectorGUI();
		}
	} 
}
