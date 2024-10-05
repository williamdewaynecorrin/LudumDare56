Shader "LD56/shader_slime"
{
    Properties
    {
        [Header(Default)]
        [HDR]_Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _BumpMap("Normal Map", 2D) = "bump" {}
        _BumpStrength("Bump Strength", Range(0, 10)) = 1
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0

        [Header(Rim)]
        [HDR]_RimColor ("Rim Color", Color) = (1,1,1,1)
        _RimPower ("Rim Power", Range(0.1, 10.0)) = 5 
        _RimWidth ("Rim Width", Range(0.01, 5.0)) = 1

        [Header(Animation)]
        _ScrollX("Scroll X", Range(0, 10)) = 0.1
        _ScrollY("Scroll Y", Range(0, 10)) = 0.1

    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard fullforwardshadows

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        struct Input
        {
            float2 uv_MainTex;
            float2 uv_BumpMap;
            float3 viewDir;
        };

        // -- standard
        sampler2D _MainTex;
        sampler2D _BumpMap;
        half _BumpStrength;
        fixed4 _Color;
        half _Glossiness;
        half _Metallic;

        // -- rim
        fixed4 _RimColor;
        half _RimPower;
        half _RimWidth;

        // -- animation
        half _ScrollX;
        half _ScrollY;

        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            half2 currentscroll = half2(_ScrollX, _ScrollY) * _Time.y;

            fixed4 c = tex2D (_MainTex, IN.uv_MainTex + currentscroll) * _Color;
            o.Albedo = c.rgb;
            o.Normal = UnpackScaleNormal (tex2D(_BumpMap, IN.uv_BumpMap + currentscroll), _BumpStrength);
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Alpha = c.a;

            // -- saturate to clamp between [0,1]
            float sat = saturate(dot(o.Normal, normalize(IN.viewDir)));
            float rim = max(0, _RimWidth - sat);

            o.Emission = pow(rim, _RimPower) * _RimColor;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
