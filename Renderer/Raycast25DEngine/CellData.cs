using UnityEngine;

namespace UPDB.Renderers.Raycast25DEngine
{
	[CreateAssetMenu(fileName = "CellData", menuName = "UPDB/Renderer/Raycast25DEngine/CellData")]
	public class CellData : ScriptableObject
	{
		[SerializeField]
		private CellID _id;

		[SerializeField]
		private bool _hasGround = false;

        [SerializeField]
        private bool _hasRoof = false;

        [SerializeField]
        private bool _hasWall = false;

        [SerializeField]
        private Texture2D _groundTexture;

        [SerializeField]
        private Texture2D _roofTexture;

        [SerializeField]
		private Texture2D _wallTexture;

        [SerializeField]
        private float _groundHeight;

        [SerializeField]
        private float _roofHeight;

        [SerializeField]
		private float _wallHeight;

		[SerializeField]
		private float _lightSource;

		[SerializeField]
		private Vector3 _lightSourcePosition;

		#region Public API

		public CellID Id => _id; 
		public bool HasGround => _hasGround;
		public bool HasRoof => _hasRoof;
		public bool HasWall => _hasWall;

		public Texture2D GroundTexture => _groundTexture;
		public Texture2D RoofTexture => _roofTexture;
		public Texture2D WallTexture => _wallTexture;

		public float GroundHeight => _groundHeight;
		public float RoofHeight => _roofHeight;
		public float WallHeight => _wallHeight;
		public float LightSource => _lightSource;
		public Vector3 LightSourcePosition => _lightSourcePosition;

        #endregion

        private void OnValidate()
        {
            if(Application.isPlaying)
			{
				GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>().RebuildParameters();
			}
        }
    } 
}
