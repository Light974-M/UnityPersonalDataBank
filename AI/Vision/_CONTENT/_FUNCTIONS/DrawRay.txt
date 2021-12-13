private void DrawRay()
{

    #region full raycast vision
    float yFov = -yFOV;

    for (int i = 0; i < rayNumber / 4; i++)
    {
        float xFov = -xFOV;

        for (int j = 0; j < rayNumber; j++)
        {
            RaycastHit hit;
            RaycastHit mirrorHit;
            RaycastHit reflectedHit;
            Vector3 playerVisionInit = new Vector3(transform.position.x, transform.position.y + 2.5f, transform.position.z);
            Vector3 direction = transform.TransformDirection(new Vector3(xFov, yFov, 1).normalized);

            bool isCollided = Physics.Raycast(playerVisionInit, direction, out hit, maxDistance);

            if (Physics.Raycast(playerVisionInit, direction, out mirrorHit, maxDistance, 1 << 9))
            {
                float x = playerVisionInit.x + (2 * (hit.point.x - playerVisionInit.x));
                float y = playerVisionInit.y + (2 * (hit.point.y - playerVisionInit.y));

                if (Physics.Raycast(hit.point, (new Vector3(x, y, playerVisionInit.z) - hit.point).normalized, out reflectedHit))
                {
                    if (reflectedHit.collider == capsuleCollider)
                    {
                        Debug.DrawRay(hit.point, (new Vector3(x, y, playerVisionInit.z) - hit.point).normalized * reflectedHit.distance, Color.green);
                        visibleDetectedPhase = true;
                    }
                    else
                    {
                        Debug.DrawRay(hit.point, (new Vector3(x, y, playerVisionInit.z) - hit.point).normalized * reflectedHit.distance, Color.blue);
                    }

                }

            }
            if (isCollided)
            {
                if (hit.collider == capsuleCollider)
                    Debug.DrawRay(playerVisionInit, direction * hit.distance, Color.green);
                else
                    Debug.DrawRay(playerVisionInit, direction * hit.distance, Color.blue);
            }
            else
                Debug.DrawRay(playerVisionInit, direction * maxDistance, Color.red);

            xFov += 2 * xFOV / rayNumber;
        }

        yFov += 2 * yFOV / (rayNumber / 4);
    }
    #endregion




    #region raycast line by line vision
    if (i < rayNumber / 4)
    {
        float xFov = -xFOV;
        visibleDetectedPhase = false;

        for (int j = 0; j < rayNumber; j++)
        {
            RaycastHit hit;
            RaycastHit mirrorHit;
            RaycastHit reflectedHit;
            Vector3 playerVisionInit = new Vector3(transform.position.x, transform.position.y + 2.5f, transform.position.z);
            Vector3 direction = transform.TransformDirection(new Vector3(xFov, yFov, 1).normalized);

            bool isCollided = Physics.Raycast(playerVisionInit, direction, out hit, maxDistance);

            if (Physics.Raycast(playerVisionInit, direction, out mirrorHit, maxDistance, 1 << 9))
            {
                float x = playerVisionInit.x + (2 * (hit.point.x - playerVisionInit.x));
                float y = playerVisionInit.y + (2 * (hit.point.y - playerVisionInit.y));

                if (Physics.Raycast(hit.point, (new Vector3(x, y, playerVisionInit.z) - hit.point).normalized, out reflectedHit))
                {
                    if (reflectedHit.collider == capsuleCollider)
                    {
                        Debug.DrawRay(hit.point, (new Vector3(x, y, playerVisionInit.z) - hit.point).normalized * reflectedHit.distance, Color.green);
                        visibleDetectedPhase = true;
                    }
                    else
                    {
                        Debug.DrawRay(hit.point, (new Vector3(x, y, playerVisionInit.z) - hit.point).normalized * reflectedHit.distance, Color.blue);
                    }

                }

            }

            if (isCollided)
            {
                if (hit.collider == capsuleCollider)
                {
                    Debug.DrawRay(playerVisionInit, direction * hit.distance, Color.green);
                    detectedPhase = true;
                }
                else
                    Debug.DrawRay(playerVisionInit, direction * hit.distance, Color.blue);
            }
            else
                Debug.DrawRay(playerVisionInit, direction * maxDistance, Color.red);

            xFov += 2 * xFOV / rayNumber;
        }

        yFov += 2 * yFOV / (rayNumber / 4);
        i++;
    }
    else
    {
        yFov = -yFOV;
        i = 0;
    }
    #endregion




    #region single raycasted vision
    float xFov = -xFOV;
    visibleDetectedPhase = false;

    RaycastHit hit;
    RaycastHit mirrorHit;
    RaycastHit reflectedHit;
    Vector3 playerVisionInit = new Vector3(transform.position.x, transform.position.y + 2.5f, transform.position.z);
    Vector3 playerMirrorVisionInit = new Vector3(transform.position.x, transform.position.y + 1f, transform.position.z);
    Vector3 direction = (player.position - playerVisionInit).normalized;

    bool isCollided = Physics.Raycast(playerVisionInit, direction, out hit, maxDistance);

    for (int j = 0; j < rayNumber; j++)
    {
        Vector3 originalDir = transform.TransformDirection(new Vector3(xFov, 0, 1).normalized);

        if (Physics.Raycast(playerMirrorVisionInit, originalDir, out mirrorHit, maxDistance, 1 << 9))
        {
            float x = playerMirrorVisionInit.x + (2 * (mirrorHit.point.x - playerMirrorVisionInit.x));
            float y = playerMirrorVisionInit.y + (2 * (mirrorHit.point.y - playerMirrorVisionInit.y));

            if (Physics.Raycast(mirrorHit.point, (new Vector3(x, y, playerMirrorVisionInit.z) - mirrorHit.point).normalized, out reflectedHit))
            {
                if (reflectedHit.collider == capsuleCollider)
                {
                    Debug.DrawRay(mirrorHit.point, (new Vector3(x, y, playerMirrorVisionInit.z) - mirrorHit.point).normalized * reflectedHit.distance, Color.green);
                    visibleDetectedPhase = true;
                }
                else
                {
                    Debug.DrawRay(mirrorHit.point, (new Vector3(x, y, playerMirrorVisionInit.z) - mirrorHit.point).normalized * reflectedHit.distance, Color.blue);
                }

            }
            else
                Debug.DrawRay(mirrorHit.point, (new Vector3(x, y, playerMirrorVisionInit.z) - mirrorHit.point).normalized * maxDistance, Color.red);

        }
        xFov += 2 * xFOV / rayNumber;
    }

    if (isCollided)
    {
        if (hit.collider == capsuleCollider)
        {
            Debug.DrawRay(playerVisionInit, direction * hit.distance, Color.green);
            detectedPhase = true;
        }
        else
            Debug.DrawRay(playerVisionInit, direction * hit.distance, Color.blue);
    }
    else
        Debug.DrawRay(playerVisionInit, direction * maxDistance, Color.red); 
    #endregion
}