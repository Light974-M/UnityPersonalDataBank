using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;
using UPDB.CoreHelper.UsableMethods;

namespace UPDB.CoreHelper.Usable
{
    public class EventTrigger : UPDBBehaviour
    {
        #region Serialized API

        [SerializeField, Tooltip("invoked when event is triggered")]
        private UnityEvent _triggerEvent;

        [SerializeField, Tooltip("invoked when event is constantly triggered")]
        private UnityEvent _triggerEventStay;

        [SerializeField, Tooltip("invoked when event is not anymor triggered")]
        private UnityEvent _triggerEventExit;

        [SerializeField, Tooltip("layerMask of layers allowed to trigger event")]
        private LayerMask _triggerLayers = -1;

        [SerializeField, Tooltip("if enabled, script will not interfer with colliders")]
        private bool _freeCollidersPreset = false;

        [SerializeField, Tooltip("type of collider to use")]
        private ColliderType _colliderTypeUsed = ColliderType.BoxCollider;

        [SerializeField, Tooltip("number of colliders")]
        private int _colliderCount = 1;

        [SerializeField, Tooltip("size presets for colliders")]
        private Vector3 _scalePreset = Vector3.one;

        #endregion

        #region Non Serialized API

        private bool _dropDownEnabled = false;
        private List<Component> _colliders = new List<Component>(); 

        #endregion

        #region Public API

        public bool DropDownEnabled
        {
            get => _dropDownEnabled;
            set => _dropDownEnabled = value;
        }
        public LayerMask TriggerLayers
        {
            get => _triggerLayers;
            set => _triggerLayers = value;
        }
        public bool FreeCollidersPreset
        {
            get => _freeCollidersPreset;
            set => _freeCollidersPreset = value;
        }
        public ColliderType ColliderTypeUsed
        {
            get => _colliderTypeUsed;
            set => _colliderTypeUsed = value;
        }
        public int ColliderCount
        {
            get => _colliderCount;
            set => _colliderCount = value;
        }
        public Vector3 ScalePreset
        {
            get => _scalePreset;
            set => _scalePreset = value;
        }

        #endregion

        /*************************BUILT-IN METHODS**************************/

        private void OnTriggerEnter(Collider other)
        {
            if(other.gameObject.layer.IsInLayerMask(_triggerLayers))
            {
                _triggerEvent?.Invoke();
            }
        }

        private void OnTriggerStay(Collider other)
        {
            if (other.gameObject.layer.IsInLayerMask(_triggerLayers))
            {
                _triggerEventStay?.Invoke();
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.layer.IsInLayerMask(_triggerLayers))
            {
                _triggerEventExit?.Invoke();
            }
        }

        /*************************CUSTOM METHODS**************************/

        protected override void OnSceneSelected()
        {
            ManageColliders();
        }

        private void ManageColliders()
        {
            if (_freeCollidersPreset)
                return;

            if (_colliderTypeUsed == ColliderType.BoxCollider)
            {
                if (hasToRebuildColliders<BoxCollider>())
                    BuildColliders<BoxCollider>();

                return;
            }
            if (_colliderTypeUsed == ColliderType.SphereCollider)
            {
                if (hasToRebuildColliders<SphereCollider>())
                    BuildColliders<SphereCollider>();

                return;
            }
            if (_colliderTypeUsed == ColliderType.CapsuleCollider)
            {
                if (hasToRebuildColliders<CapsuleCollider>())
                    BuildColliders<CapsuleCollider>();

                return;
            }
            if (_colliderTypeUsed == ColliderType.MeshCollider)
            {
                if (hasToRebuildColliders<MeshCollider>())
                    BuildColliders<MeshCollider>();

                return;
            }
            if (_colliderTypeUsed == ColliderType.BoxCollider2D)
            {
                if (hasToRebuildColliders<BoxCollider2D>())
                    BuildColliders<BoxCollider2D>();

                return;
            }
            if (_colliderTypeUsed == ColliderType.CapsuleCollider2D)
            {
                if (hasToRebuildColliders<CapsuleCollider2D>())
                    BuildColliders<CapsuleCollider2D>();

                return;
            }
            if (_colliderTypeUsed == ColliderType.CircleCollider2D)
            {
                if (hasToRebuildColliders<CircleCollider2D>())
                    BuildColliders<CircleCollider2D>();

                return;
            }
        }

        private bool hasToRebuildColliders<T>() where T : Component
        {
            Component[] collidersList = GetCollidersList();
            _colliders = collidersList.ToList();

            if (_colliders.Count == 0 || _colliders.Count != _colliderCount)
                return true;

            foreach (Component elem in _colliders)
                if (elem.GetType() != typeof(T))
                    return true;

            return false;
        }

        private void BuildColliders<T>() where T : Component
        {
            int length = _colliders.Count;

            for (int i = 0; i < length; i++)
                IntelliDestroy(_colliders[i]);

            _colliders = new List<Component>();

            for (int i = 0; i < _colliderCount; i++)
                CreateCollider<T>();
        }

        private Component[] GetCollidersList()
        {
            Component[] componentList = GetComponents<Component>();
            List<Component> collidersList = new List<Component>();

            for (int i = 0; i < componentList.Length; i++)
                if (componentList[i].GetType().BaseType == typeof(Collider) || componentList[i].GetType().BaseType == typeof(Collider2D))
                    collidersList.Add(componentList[i]);

            return collidersList.ToArray();
        }

        private void CreateCollider<T>() where T : Component
        {
            T baseCollider = gameObject.AddComponent<T>();

            //set mesh collider convex state
            if (baseCollider.GetType() == typeof(MeshCollider))
            {
                MeshCollider meshCollider = baseCollider as MeshCollider;
                meshCollider.convex = true;
            }

            //set is trigger of collider
            if (baseCollider.GetType().BaseType == typeof(Collider))
            {
                Collider collider = baseCollider as Collider;
                collider.isTrigger = true;
            }
            if (baseCollider.GetType().BaseType == typeof(Collider2D))
            {
                Collider2D collider2D = baseCollider as Collider2D;
                collider2D.isTrigger = true;
            }

            //set size of collider
            if (baseCollider.GetType() == typeof(BoxCollider))
            {
                BoxCollider boxCollider = baseCollider as BoxCollider;
                boxCollider.size = _scalePreset;
            }
            if (baseCollider.GetType() == typeof(CapsuleCollider))
            {
                CapsuleCollider capsuleCollider = baseCollider as CapsuleCollider;
                capsuleCollider.radius = _scalePreset.x;
                capsuleCollider.height = _scalePreset.y;
            }
            if (baseCollider.GetType() == typeof(SphereCollider))
            {
                SphereCollider sphereCollider = baseCollider as SphereCollider;
                sphereCollider.radius = _scalePreset.x;
            }
            if (baseCollider.GetType() == typeof(BoxCollider2D))
            {
                BoxCollider2D boxCollider2D = baseCollider as BoxCollider2D;
                boxCollider2D.size = _scalePreset;
            }
            if (baseCollider.GetType() == typeof(CapsuleCollider2D))
            {
                CapsuleCollider2D capsuleCollider2D = baseCollider as CapsuleCollider2D;
                capsuleCollider2D.size = _scalePreset;
            }
            if (baseCollider.GetType() == typeof(CircleCollider2D))
            {
                CircleCollider2D circleCollider2D = baseCollider as CircleCollider2D;
                circleCollider2D.radius = _scalePreset.x;
            }

            _colliders.Add(baseCollider);
        }
    }
}
