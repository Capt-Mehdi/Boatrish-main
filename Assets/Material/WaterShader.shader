Shader "Custom/WaterShader"
{
    Properties
    {
        _MainTex ("Main Texture", 2D) = "white" {}
        _WaveSpeed ("Wave Speed", Range(0.1, 1)) = 0.5
        _WaveHeight ("Wave Height", Range(0.1, 1)) = 0.1
        _WaveFrequency ("Wave Frequency", Range(0.1, 10)) = 1.0
        _RippleSpeed ("Ripple Speed", Range(0.1, 1)) = 0.2
        _RippleStrength ("Ripple Strength", Range(0.1, 1)) = 0.2
        _RippleFrequency ("Ripple Frequency", Range(0.1, 10)) = 1.0
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        LOD 200

        CGPROGRAM
        #pragma surface surf Lambert vertex:vert

        sampler2D _MainTex;
        float _WaveSpeed;
        float _WaveHeight;
        float _WaveFrequency;
        float _RippleSpeed;
        float _RippleStrength;
        float _RippleFrequency;

        struct Input
        {
            float2 uv_MainTex;
            float4 vertex : POSITION;
            float3 normal : NORMAL;
        };

        void vert(inout appdata_full v)
        {
            float wave = sin(v.vertex.x * _WaveFrequency + _Time.y * _WaveSpeed) * _WaveHeight;
            float ripple = sin(v.vertex.z * _RippleFrequency + _Time.y * _RippleSpeed) * _RippleStrength;
            v.vertex.y += wave + ripple;
        }

        void surf(Input IN, inout SurfaceOutput o)
        {
            half4 c = tex2D(_MainTex, IN.uv_MainTex);
            o.Albedo = c.rgb;
            o.Alpha = c.a;
        }
        ENDCG
    }
    FallBack "Transparent/VertexLit"
}
