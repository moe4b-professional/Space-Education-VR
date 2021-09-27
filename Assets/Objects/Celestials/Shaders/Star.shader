Shader "Crying Suns/Star"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
        [HDR] _Tint("Tint", Color) = (1, 1, 1, 1)
        _AtmosDistance("Atmosphere Distance", Float) = 0.1
        _AtmosColor("Atmosphere Tint", Color) = (1, 1, 1, 1)
        _AtmosFresnelLimit("Atmosphere Fresnel Limit", Range(0, 1)) = 0.35
        _AtmosFresnelExponent("Atmosphere Fresnel Exponent", Float) = 3
        _RotationSpeed("Rotation Speed", Float) = 0.001
    }

    CGINCLUDE
    #include "UnityCG.cginc"
    #include "UnityLightingCommon.cginc"

    struct appdata
    {
        float4 vertex : POSITION;
        float2 uv : TEXCOORD0;
        float3 normal : NORMAL0;
    };

    struct v2f
    {
        float4 vertex : SV_POSITION;
        float2 uv : TEXCOORD0;
        float3 normal : TEXCOORD1;
        float3 viewDir : TEXCOORD2;
        float4 diff : COLOR0;
    };

    float4 ComputeLight0(float3 worldNormal)
    {
        half nl = max(0, dot(worldNormal, _WorldSpaceLightPos0.xyz));
        float4 diff = nl * _LightColor0 * 0.5;
        diff.rgb += ShadeSH9(half4(worldNormal, 1));
        return diff;
    }

    float FresnelDotProduct(v2f i, float limit, float exponent)
    {
        float dotProduct = dot(i.normal, i.viewDir);
        dotProduct = clamp(dotProduct, 0, 1);
        if (dotProduct > limit) {
            dotProduct = 1;
        }
        else {
            dotProduct /= limit;
        }
        return pow(dotProduct, exponent);
    }

    ENDCG

    SubShader
    {
        Tags { "Queue" = "Transparent" }

        Pass
        {
            Tags { "LightMode" = "ForwardBase" }

            Name "OUTER_ATMOSPHERE"

            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            fixed _AtmosDistance;

            v2f vert(appdata v)
            {
                v2f o;
                o.normal = UnityObjectToWorldNormal(v.normal);
                o.vertex = UnityObjectToClipPos(v.vertex + v.normal * _AtmosDistance);
                o.viewDir = normalize(WorldSpaceViewDir(v.vertex));
                o.uv = float2(0, 0);
                o.diff = ComputeLight0(o.normal);
                return o;
            }

            fixed4 _AtmosColor;
            fixed _AtmosFresnelLimit;
            half _AtmosFresnelExponent;

            fixed4 frag(v2f i) : SV_Target
            {
                float dotProduct = FresnelDotProduct(i, _AtmosFresnelLimit, _AtmosFresnelExponent);
                fixed4 col = fixed4(_AtmosColor.rgb, dotProduct);
                //col.rgb *= i.diff.rgb;
                return col;
            }
            ENDCG
        }

        Pass
        {
            Tags { "LightMode" = "ForwardBase" }

            Name "GROUND"

            ZWrite On

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            sampler2D _MainTex;
            float4 _MainTex_ST;

            float _RotationSpeed;

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                float2 uv = v.uv;
                uv.x += _RotationSpeed * _Time[1];
                o.uv = TRANSFORM_TEX(uv, _MainTex);
                o.normal = UnityObjectToWorldNormal(v.normal);
                o.viewDir = normalize(WorldSpaceViewDir(v.vertex));

                o.diff = ComputeLight0(o.normal);
                return o;
            }

            fixed4 _Tint;

            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);
                col *= _Tint;
                //col *= i.diff;
                return col;
            }
            ENDCG
        }

        Pass
        {
            Tags { "LightMode" = "ForwardBase" }

            Name "INNER_ATMOSPHERE"

            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            fixed _AtmosDistance;

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.normal = UnityObjectToWorldNormal(v.normal);
                o.viewDir = normalize(WorldSpaceViewDir(v.vertex));
                o.uv = float2(0, 0);
                o.diff = ComputeLight0(o.normal);
                return o;
            }

            fixed4 _AtmosColor;

            fixed4 frag(v2f i) : SV_Target
            {
                float dotProduct = FresnelDotProduct(i, 1, 1);
                _AtmosColor.a *= 1.0 - dotProduct;
                //_AtmosColor.rgb *= i.diff.rgb;
                return _AtmosColor;
            }
            ENDCG
        }
    }
}