using UnityEditor;
using UnityEngine;

///<summary>
/// 
///</summary>
[AddComponentMenu("SpriteRenderer")]
public class SpriteRenderer : MonoBehaviour
{

    [SerializeField, Tooltip("")]
    private Texture2D _texture;

    [SerializeField, Tooltip("")]
    private Sprite _sprite;

    [SerializeField, Tooltip("")]
    private Material _material;

    private Camera _cameraScene;

    private void Awake()
    {
        _material.mainTexture = _texture;
        
    }

    private void Update()
    {
        
    }

    private void OnDrawGizmos()
    {
        if (_cameraScene == null)
            _cameraScene = Camera.current;

        float sizeTexture = 1 / Vector3.Distance(transform.position, _cameraScene.transform.position);

        _material.mainTextureScale = new Vector2(sizeTexture * 12.8f, sizeTexture * 7.2f);
    }
}
