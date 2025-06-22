using UnityEngine;

namespace UPDB.Renderers.Raycast25DEngine
{
	[CreateAssetMenu(fileName = "CellData", menuName = "UPDB/Renderer/Raycast25DEngine/CellData")]
	public class CellData : ScriptableObject
	{
		[SerializeField]
		private CellID _id;

		[SerializeField]
		private Texture2D _texture;

		[SerializeField]
		private float _height;

		#region Public API

		public CellID Id => _id; 
		public Texture2D Texture => _texture;

		public float Height => _height;

		#endregion
	} 
}
