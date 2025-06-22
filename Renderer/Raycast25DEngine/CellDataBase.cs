using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEditor.PackageManager;
using UnityEditorInternal;
using UnityEngine;

namespace UPDB.Renderers.Raycast25DEngine
{
	[CreateAssetMenu(fileName = "CellDataBase", menuName = "UPDB/Renderer/Raycast25DEngine/CellDataBase")]
	public class CellDataBase : ScriptableObject
	{
		[SerializeField]
		private List<CellData> _cellsList;

		private Dictionary<CellID, CellData> _cellsDictionary = new Dictionary<CellID, CellData> ();

		public Dictionary<CellID, CellData> CellsDictionary
		{
			get
			{
				if(_cellsDictionary == null || _cellsDictionary.Count == 0)
				{
                    _cellsDictionary = new Dictionary<CellID, CellData>();

                    for (int i = 0; i < _cellsList.Count; i++)
						_cellsDictionary.Add(_cellsList[i].Id, _cellsList[i]);
				}

				return _cellsDictionary;
            }
		}
	}

    public enum CellID
    {
        BlankGround,
        BrickWall,
		BrickWallTall,
    }
}
