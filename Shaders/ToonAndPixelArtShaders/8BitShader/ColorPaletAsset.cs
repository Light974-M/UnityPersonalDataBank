using UnityEngine;
using System.Collections.Generic;

namespace UPDB.Renderers.HeightBitRenderer
{
	[CreateAssetMenu(fileName = "ColorPaletAsset", menuName = "UPDB/Renderer/8BitRenderer/ColorPaletAsset")]
	public class ColorPaletAsset : ScriptableObject
	{
        [SerializeField]
        private List<Color> _colorPalet;

		#region Public API

		public List<Color> ColorsPalet => _colorPalet;

		#endregion
	} 
}
