// Custom/Distorted/FlyingCarpet
// Ported from the 1.56 original's GLES program (sharedassets1, path_id 374),
// decompressed with LZ4 (decompressedSize 1728). Source is verbatim.
Shader "Custom/Distorted/FlyingCarpet" {
Properties {
    _MainTex ("Base (RGB)", 2D) = "white" {}
    _Color ("Color (RGBA)", Color) = (1,1,1,1)
    _Frequency ("Frequency", Range(0.01,5)) = 4
    _Distort ("Distort", Color) = (0,0,0,0)
}
SubShader {
    Pass {
        Cull Off

        GLSLPROGRAM
        #version 100

        #ifdef VERTEX
        attribute vec4 _glesVertex;
        attribute vec4 _glesColor;
        attribute vec4 _glesMultiTexCoord0;
        uniform highp vec4 _Time;
        uniform highp mat4 glstate_matrix_mvp;
        uniform highp vec4 _MainTex_ST;
        uniform highp float _Frequency;
        uniform highp vec2 _Distort;
        varying highp vec2 xlv_TEXCOORD0;
        void main ()
        {
          highp vec4 tmpvar_1;
          tmpvar_1 = _glesVertex;
          lowp vec4 tmpvar_2;
          tmpvar_2 = _glesColor;
          highp vec4 tmpvar_3;
          tmpvar_3.w = tmpvar_1.w;
          highp vec4 tmpvar_4;
          highp float intensity_5;
          intensity_5 = tmpvar_2.x;
          highp vec3 off_6;
          highp vec3 tmpvar_7;
          tmpvar_7.z = 0.0;
          tmpvar_7.xy = tmpvar_1.xz;
          highp vec3 tmpvar_8;
          tmpvar_8 = abs(((
            fract((((
              ((_Time.x * vec3(50.0, 100.0, 0.0)) * _Frequency)
             - 
              dot (tmpvar_7, vec3(1.0, 1.0, 1.0))
            ) / 9.42477) + 0.5))
           * 2.0) - 1.0));
          off_6.x = 0.0;
          off_6.y = ((intensity_5 * (
            (((tmpvar_8 * tmpvar_8) * (3.0 - (2.0 * tmpvar_8))) * 2.0)
           - 1.0)).y * 0.3);
          off_6.z = 0.0;
          tmpvar_3.xyz = (_glesVertex.xyz + off_6);
          tmpvar_4 = (glstate_matrix_mvp * tmpvar_3);
          tmpvar_4.x = (tmpvar_4.x + ((tmpvar_4.z * tmpvar_4.z) * _Distort.x));
          tmpvar_4.y = (tmpvar_4.y + ((tmpvar_4.z * tmpvar_4.z) * _Distort.y));
          gl_Position = tmpvar_4;
          xlv_TEXCOORD0 = ((_glesMultiTexCoord0.xy * _MainTex_ST.xy) + _MainTex_ST.zw);
        }
        #endif
        #ifdef FRAGMENT
        uniform sampler2D _MainTex;
        uniform mediump vec4 _Color;
        varying highp vec2 xlv_TEXCOORD0;
        void main ()
        {
          lowp vec4 tmpvar_1;
          tmpvar_1 = (texture2D (_MainTex, xlv_TEXCOORD0) * _Color);
          gl_FragData[0] = tmpvar_1;
        }
        #endif
        ENDGLSL
    }
}
}
