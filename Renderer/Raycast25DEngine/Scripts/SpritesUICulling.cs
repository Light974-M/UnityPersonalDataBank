using System.Collections.Generic;
using UnityEngine;
using UPDB.CoreHelper.UsableMethods;

namespace UPDB.Renderers.Raycast25DEngine
{
	public class SpritesUICulling : UPDBBehaviour
	{
        [SerializeField]
        private GameObject _spritesDatasParentObj;

        private PlayerController _controller;

        private void Awake()
        {
            _controller = FindFirstObjectByType<PlayerController>();
        }

        private void Update()
        {
            SortRenderersByDistance(_controller.transform, transform, _spritesDatasParentObj.transform, true);
        }

        public static void SortRenderersByDistance(Transform target, Transform renderersParent, Transform enemiesParent, bool descendingOrder)
        {
            // 1. Récupérer la liste des EnemyControllers depuis le parent
            EnemyController[] enemies = enemiesParent.GetComponentsInChildren<EnemyController>();

            // 2. Trier par distance à la cible
            System.Array.Sort(enemies, (a, b) =>
            {
                float distA = Vector3.SqrMagnitude(a.transform.position - target.position);
                float distB = Vector3.SqrMagnitude(b.transform.position - target.position);
                return descendingOrder ? distB.CompareTo(distA) : distA.CompareTo(distB);
            });

            // 3. Réordonner les renderers dans le parent des renderers
            for (int i = 0; i < enemies.Length; i++)
            {
                if (enemies[i].RendererObj != null)
                {
                    enemies[i].RendererObj.transform.SetParent(renderersParent);
                    enemies[i].RendererObj.transform.SetSiblingIndex(i);
                }
            }
        }
    } 
}
