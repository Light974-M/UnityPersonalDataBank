using UnityEngine;
using UnityEngine.UI;
using UPDB.CoreHelper.UsableMethods;

namespace UPDB.Renderers.Raycast25DEngine
{
    public class EnemyController : UPDBBehaviour
    {
        [SerializeField]
        private GameObject _rendererPrefab;

        [SerializeField]
        private Texture2D _sprite;

        private Material _enemyRaycastRendererMaterial;

        private PlayerController _controller;
        private float _calculatedXPos;

        private Vector2 _canvasReferenceResolution;

        private GameObject _rendererObj;

        public GameObject RendererObj => _rendererObj;

        private void Awake()
        {
            _rendererObj = Instantiate(_rendererPrefab, GameObject.FindGameObjectWithTag("RaycasterSpritesUICanvas").transform);

            _enemyRaycastRendererMaterial = new Material(Shader.Find("Shader Graphs/SpriteRaycastRenderer"));
            _enemyRaycastRendererMaterial.SetTexture("_MainTexture", _sprite);

            _rendererObj.GetComponent<Image>().material = _enemyRaycastRendererMaterial;

            _controller = FindFirstObjectByType<PlayerController>();
            _enemyRaycastRendererMaterial.SetTexture("_textureToDrawCoords", _controller.TextureToDrawCoordsArray);
            _enemyRaycastRendererMaterial.SetTexture("_raycastAndHorizonParameters", _controller.RaycastAndHorizonParameters);
            _enemyRaycastRendererMaterial.SetFloat("_renderDistance", _controller.RenderDistance);

            _canvasReferenceResolution = _rendererObj.GetComponent<Image>().canvas.transform.GetComponent<CanvasScaler>().referenceResolution;

        }

        // Update is called once per frame
        private void Update()
        {
            RenderEnemySprite();
        }

        private void RenderEnemySprite()
        {
            float enemyDistance = Vector2.Distance(transform.position, _controller.RaycastOrigin);
            _enemyRaycastRendererMaterial.SetFloat("_playerDist", enemyDistance);

            _enemyRaycastRendererMaterial.SetTexture("_textureToDrawCoords", _controller.TextureToDrawCoordsArray);
            _enemyRaycastRendererMaterial.SetTexture("_raycastAndHorizonParameters", _controller.RaycastAndHorizonParameters);

            Vector2 minRayAngle = RotateVector(_controller.transform.up, _controller.FieldOfView.x / 2f);
            Vector2 maxRayAngle = RotateVector(_controller.transform.up, -_controller.FieldOfView.x / 2f);
            Vector2 currentAngle = (Vector2)transform.position - _controller.RaycastOrigin;

            RectTransform rectTransform = _rendererObj.GetComponent<RectTransform>();

            _calculatedXPos = ((GetRelativePosition(minRayAngle, maxRayAngle, currentAngle) * _canvasReferenceResolution.x) - (_canvasReferenceResolution.x / 2));

            float size = _canvasReferenceResolution.y / enemyDistance;

            float enemyHeight = ((100f / _controller.FieldOfView.y) / enemyDistance);
            float heightoffset = enemyHeight * -_controller.PlayerHeight;

            rectTransform.anchoredPosition = new Vector2(_calculatedXPos, ((-_controller.VerticalLookingValue / 25f) * (_canvasReferenceResolution.y / 2)) + (heightoffset * 1000));
            rectTransform.sizeDelta = Vector2.one * size;
        }

        public float GetRelativePosition(Vector2 left, Vector2 right, Vector2 point)
        {
            // Angle total entre left et right
            float totalAngle = Vector2.SignedAngle(left, right);

            // Angle entre left et point
            float angleToPoint = Vector2.SignedAngle(left, point);

            // Normalisation entre 0 et 1
            float t = angleToPoint / totalAngle;

            return t;
        }
    } 
}
