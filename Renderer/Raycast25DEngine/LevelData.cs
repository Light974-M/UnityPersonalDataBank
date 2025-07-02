using NUnit.Framework;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace UPDB.Renderers.Raycast25DEngine
{
    [CreateAssetMenu(fileName = "LevelData", menuName = "UPDB/Renderer/Raycast25DEngine/LevelData")]
    public class LevelData : ScriptableObject
    {
        [SerializeField]
        private Vector2Int _levelSize;

        [SerializeField]
        private Cell[,] _levelArray;

        [SerializeField, HideInInspector]
        private List<CellSavable> _levelArraySavable;

        [SerializeField, HideInInspector]
        private Vector2Int _savedSize;

        private Vector2Int _levelSizeMemo = Vector2Int.zero;

        public Cell[,] LevelArray
        {
            get
            {
                if (_levelArray == null || _levelArray.Length != _levelSize.x * _levelSize.y)
                {
                    _levelArray = new Cell[_levelSize.x, _levelSize.y];

                    for (int y = 0; y < _levelSize.y; y++)
                    {
                        for (int x = 0; x < _levelSize.x; x++)
                        {
                            _levelArray[x, y] = new Cell(new Vector2Int(x, y), CellID.BlankGround);
                        }
                    }
                }

                return _levelArray;
            }
        }

        public Vector2Int LevelSize
        {
            get => _levelSize;
        }

        public void Save()
        {
            _levelArraySavable = new List<CellSavable>();
            _savedSize = _levelSize;

            for (int y = 0; y < _levelSize.y; y++)
                for (int x = 0; x < _levelSize.x; x++)
                    _levelArraySavable.Add(new CellSavable(LevelArray[x, y]));

            EditorUtility.SetDirty(this);
        }

        public void Load()
        {
            _levelSize = _savedSize;
            _levelArray = new Cell[_levelSize.x, _levelSize.y];

            for (int i = 0; i < _levelArraySavable.Count; i++)
            {
                int x = i % _levelSize.x;
                int y = i / _levelSize.x;
                _levelArray[x, y] = new Cell(_levelArraySavable[i]);
            }
        }

        private void OnValidate()
        {
            if (_levelSizeMemo != _levelSize && _levelArray != null)
                Save();

            Load();

            _levelSizeMemo = _levelSize;
        }
    }
}
