---
name: port-shadertoy-shader
description: Use when converting a Shadertoy or Godot shader into a Unity URP shader.
---

# Port a Shadertoy shader to Unity URP

## Steps
1. Create the shader in `Assets/Shaders/`. Put the original author, title and URL in a comment at the top.
2. Translate GLSL to HLSL:
   - `vec2/vec3/vec4` -> `float2/float3/float4`
   - `mix` -> `lerp`, `fract` -> `frac`, `atan(y,x)` -> `atan2(y,x)`
   - `mod(x,y)` -> `x - y * floor(x / y)` (GLSL mod differs from HLSL fmod)
   - `iTime` -> `_Time.y`
   - `fragCoord / iResolution.xy` -> the mesh/UI UV
   - `iMouse` -> a `_Mouse` float4 property set from a script
   - `mainImage(out fragColor, fragCoord)` -> a fragment function returning `half4`
3. Expose colors and speeds as `Properties` so they match the project palette.
4. For UI (Canvas Image) use a UI-compatible shader (stencil and color properties), or render the effect on a world-space quad.
5. Drive runtime values with `MaterialPropertyBlock` or tweens, not per-frame new materials.

## Rules
- Always credit the original author, and list the shader in the README.
- No new material instance per frame.

## Check before finishing
- Looks correct in both Game view and a build.
- Colors come from the palette, not the original's hardcoded values.
