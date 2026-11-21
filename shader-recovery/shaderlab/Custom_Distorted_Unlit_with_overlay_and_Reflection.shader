Shader "Custom/Distorted/Unlit with overlay and Reflection" {
Properties {
 _MainTex ("Base (RGB)", 2D) = "white" { }
 _Color ("Color Tint", Color) = (0,0,0,0)
 _ReflectColor ("Reflection Color", Color) = (1,1,1,0.5)
 _Cube ("Reflection Cubemap", CUBE) = "_Skybox" { }
}
