#ifndef RAYCAST_INCLUDE
#define RAYCAST_INCLUDE



    void GetScreenPosition_float(float3 Position, out float2 ScreenPosition, out float2 ScreenPositionAspectRatio)
    {
        float4 screen = ComputeScreenPos(TransformWorldToHClip(Position), _ProjectionParams.x);
        ScreenPosition = screen.xy / abs(screen.w);

        float aspectRatio = _ScreenParams.y / _ScreenParams.x;
        ScreenPositionAspectRatio = float2(ScreenPosition.x, ScreenPosition.y * aspectRatio);
    }


#endif