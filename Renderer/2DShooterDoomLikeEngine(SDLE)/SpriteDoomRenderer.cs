using UnityEngine;

namespace UPDB.Renderer.ShooterDoomLikeRenderer
{
    ///<summary>
    /// 
    ///</summary>
    [AddComponentMenu("UPDB/Renderer/ShooterDoomLikeRenderer/SpriteDoomRenderer")]
    public class SpriteDoomRenderer : UPDBBehaviour
    {

        [SerializeField]
        private GameObject _gameRenderer;

        private void Awake()
        {
            
        }

        private void Update()
        {
            
        }

        private void OnDrawGizmos()
        {
            transform.LookAt(Camera.current.transform.position);
            _gameRenderer.transform.LookAt(Camera.main.transform.position);
        }
    }
}

