using UnityEngine;
using UPDB.CoreHelper.UsableMethods;

namespace UPDB.Physic.SoftLockPrevention
{
    public class SoftLockDeadZone : UPDBBehaviour
    {
        /// <summary>
        /// called when object collider with trigger mode overlap
        /// </summary>
        /// <param name="other">triggered object</param>
        private void OnTriggerEnter(Collider other)
        {
            if(other.TryGetComponent(out SoftLockInitialState initialState))
            {
                initialState.transform.position = initialState.InitialPos;
                initialState.transform.rotation = initialState.InitialRot;
                initialState.transform.localScale = initialState.InitialScale;

                if(other.TryGetComponent(out Rigidbody rb))
                {
                    rb.linearVelocity = Vector3.zero;
                    rb.angularVelocity = Vector3.zero;
                }
            }
        }

        /// <summary>
        /// called when object collider 2D with trigger mode overlap
        /// </summary>
        /// <param name="collision">triggered object</param>
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.TryGetComponent(out SoftLockInitialState initialState))
            {
                initialState.transform.position = initialState.InitialPos;
                initialState.transform.rotation = initialState.InitialRot;
                initialState.transform.localScale = initialState.InitialScale;

                if (collision.TryGetComponent(out Rigidbody2D rb))
                {
                    rb.linearVelocity = Vector2.zero;
                    rb.angularVelocity = 0;
                }
            }
        }
    } 
}
