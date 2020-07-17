#ifndef __PROCOOL_HEXAGON_HELPER__
#define __PROCOOL_HEXAGON_HELPER__

#define HEXAGON_POINT_TOP
// #define HEXAGON_FLAT_TOP

#define SQRT_3 (1.732050807568877f)

float2 world_to_hexagon(float2 pos, float size)
{
#ifdef HEXAGON_POINT_TOP
    float2x2 mat = {
        SQRT_3 / 3, -1.0f / 3.0f,
        0,           2.0f / 3.0f
    };
#endif
#ifdef HEXAGON_FLAT_TOP
    float2x2 mat = {
         2.0f / 3.0f,   0,
        -1.0f/3.0f,     SQRT_3 / 3.0f
    };
#endif
    return mul(mat, pos) / size;
}

float2 hexagon_to_world(float2 pos, float size)
{
#ifdef HEXAGON_POINT_TOP
    float2x2 mat = {
        SQRT_3, SQRT_3 / 2.0f,
        0,      3.0f / 2.0f,
    };
#endif
#ifdef HEXAGON_FLAT_TOP
    float2x2 mat = {
        3.0f / 2.0f,    0,
        SQRT_3, SQRT_3 / 2.0f,
    };
#endif
    return mul(mat, pos) * size;
}

float3 axial_to_cube(float2 axial)
{
    return float3(axial.x, axial.y, 0 - axial.x - axial.y);
}

float2 cube_to_axial(float3 cube)
{
    return cube.xy;
}

int3 hexagon_round(float3 cube)
{
    int3 r = round(cube);

    float3 diff = abs(r - cube);

    if (diff.x > diff.y && diff.x > diff.z)
        r.x = -r.y - r.z;
    else if (diff.y > diff.z)
        r.y = -r.x - r.z;
    else
        r.z = -r.x - r.y;

    return r;
}
int2 hexagon_round(float2 axial)
{
    return hexagon_round(axial_to_cube(axial)).xy;
}

bool hexagon_equal(float3 a, float3 b)
{
    return a.x == b.x && a.y == b.y && a.z == b.z;
}
bool hexagon_equal(int3 a, int3 b)
{
    return a.x == b.x && a.y == b.y && a.z == b.z;
}
bool hexagon_equal(int2 a, int2 b)
{
    return a.x == b.x && a.y == b.y;
}
bool hexagon_equal(float2 a, float2 b)
{
    return a.x == b.x && a.y == b.y;
}

#endif