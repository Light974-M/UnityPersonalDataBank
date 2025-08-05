using System.Collections.Generic;
using UnityEngine;

namespace UPDB.Renderers.AsciiEngine
{
	[CreateAssetMenu(fileName = "AsciiTableAsset", menuName = "UPDB/Renderer/AsciiEngine/AsciiTableAsset")]
	public class AsciiTableAsset : ScriptableObject
	{
        [SerializeField]
        private Vector2Int _characterSize;

		[SerializeField]
		private AsciiTableBuildMode _buildMode;

        [SerializeField]
		private Texture2D _asciiTextureBuildable;

        [SerializeField]
        private Vector2Int _textureBuildableCharacterSize;

        [SerializeField]
		private List<Texture2D> _charactersList;

		#region Public API

		public Vector2Int CharacterSize => _characterSize;
		public AsciiTableBuildMode BuildMode => _buildMode;
		public Texture2D AsciiTextureBuildable => _asciiTextureBuildable;
		public Vector2Int TextureBuildableCharacterSize => _textureBuildableCharacterSize;
		public List<Texture2D> CharactersList
		{
			get => _charactersList;
			set => _charactersList = value;
        }

        #endregion
    } 

	public enum AsciiTableBuildMode
	{
		SingleTexture,
		TextureList,
	}
}
