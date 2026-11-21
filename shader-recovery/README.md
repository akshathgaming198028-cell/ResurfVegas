# Shader recovery (from original APK, Unity 5.3.4f1 data)

Source: original APK (release The_1.56), assets/bin/Data.

- shaderlab/*.shader: 73 ShaderLab blocks copied verbatim from the data (names, properties, passes). The sky block is incomplete: it stops at the first closing brace inside Properties. Re-extract with the fixed end rule (end at the Fallback line / next top-level Shader).
- glsl-raw/*.glsl: 65 GLES 2.0 program fragments (#version 100). Text is interleaved with binary length bytes, so they are NOT clean; clean each one by hand against its ShaderLab pass.
- extract_*.py: the extraction scripts.

Next: sky (Custom/Sky) -> replace Assets/Shader/Sky.shader, then Bend/Flag and Bend/UV Scroll, then the distorted variants in groups. Check each against reference screenshots on a green build.
