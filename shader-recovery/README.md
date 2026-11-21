# Shader recovery (from original APK, Unity 5.3.4f1 data)

Source: original APK (assets/bin/Data), extracted in the working chat.

## Status
- shaderlab/*.shader: 73 ShaderLab definitions (names, properties, passes). Complete for Custom/Sky (the earlier truncated copy is fixed).
- glsl-raw/*.glsl: 65 GLES 2.0 program bodies (#version 100). These are NOT clean source. Binary length bytes are mixed into names and code, so lines are broken or garbled. Treat them as reference only, not code to compile.
- The sky's exact blend formula is not recoverable from this text. Readable pieces suggest: vertex computes a height term from the view direction, and the fragment mixes _SkyGradientBottomColor and _SkyGradientTopColor with clamp(height * fade + _SkyGradientOffset, 0, 1), with _FogSilhouetteColor near the horizon.

## Next
1. Write Assets/Shader/Sky.shader from the ShaderLab structure above (two passes, background queue, ZWrite Off), using the readable formula.
2. Build on main (green), compare against the original sky screenshot, and tune the horizon band.
3. Then Bend/Flag and Bend/UV Scroll, then the distorted variants in groups, each checked against screenshots.
