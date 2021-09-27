Shader "Crying Suns/Planet"
{
    Properties
    {
        _MainTex ("Ground Texture", 2D) = "white" {}
        _GroundTint ("Ground Tint", Color) = (1, 1, 1, 1)
        [NoScaleOffset] _CityTex ("Ground Lights Texture", 2D) = "black" {}
        _CityTint ("Cities Tint", Color) = (1, 1, 1, 1)
        [NoScaleOffset] _CloudsTex ("Clouds Texture", 2D) = "black" {}
        _AtmosDistance ("Atmosphere Distance", Float) = 0.1
        _AtmosColor ("Atmosphere Tint", Color) = (1, 1, 1, 1)
        _AtmosFresnelLimit ("Atmosphere Fresnel Limit", Range(0, 1)) = 0.35
        _AtmosFresnelExponent ("Atmosphere Fresnel Exponent", Float) = 3
        _RotationSpeed ("Rotation Speed", Float) = 0.001
        _CloudsRelativeSpeed ("Cloud Relative Speed", Float) = 0.001
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

    float4 ComputeLight1(float3 worldPosition, float3 worldNormal)
    {
        float3 direction = normalize(_WorldSpaceLightPos0 - worldPosition);

        half nl = max(0, dot(worldNormal, direction));
        float4 diff = nl * _LightColor0 * 0.5;
        diff.rgb += ShadeSH9(half4(worldNormal, 1));
        return diff;
    }

    float FresnelDotProduct(v2f i, float limit, float exponent) {
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
        Tags { "Queue"="Transparent" }

        Pass {
            Tags { "LightMode" = "ForwardAdd" }

            Name "OUTER_ATMOSPHERE"

            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            fixed _AtmosDistance;

            v2f vert (appdata v)
            {
                v2f o;
                o.normal = UnityObjectToWorldNormal(v.normal);
                o.vertex = UnityObjectToClipPos(v.vertex + v.normal * _AtmosDistance);
                o.viewDir = normalize(WorldSpaceViewDir(v.vertex));
                o.uv = float2(0, 0);
                o.diff = ComputeLight1(mul(unity_ObjectToWorld, v.vertex), o.normal);
                return o;
            }

            fixed4 _AtmosColor;
            fixed _AtmosFresnelLimit;
            half _AtmosFresnelExponent;

            fixed4 frag (v2f i) : SV_Target
            {
                float dotProduct = FresnelDotProduct(i, _AtmosFresnelLimit, _AtmosFresnelExponent);
                fixed4 col = fixed4(_AtmosColor.rgb, dotProduct);
                col.rgb *= i.diff.rgb;
                return col;
            }
            ENDCG
        }

        Pass
        {
            Tags { "LightMode" = "ForwardAdd" }

            Name "GROUND"

            ZWrite On

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            sampler2D _MainTex;
            float4 _MainTex_ST;

            float _RotationSpeed;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                float2 uv = v.uv;
                uv.x += _RotationSpeed * _Time[1];
                o.uv = TRANSFORM_TEX(uv, _MainTex);
                o.normal = UnityObjectToWorldNormal(v.normal);
                o.viewDir = normalize(WorldSpaceViewDir(v.vertex));

                o.diff = ComputeLight1(mul(unity_ObjectToWorld, v.vertex), o.normal);
                return o;
            }

            fixed4 _GroundTint;

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);
                col *= _GroundTint;
                col *= i.diff;
                return col;
            }
            ENDCG
        }

        Pass {
            Name "CLOUDS_SHADOW"

            Blend SrcAlpha OneMinusSrcAlpha
            ZTest LEqual
            ZWrite Off

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            sampler2D _CloudsTex;
            float4 _MainTex_ST;

            float _RotationSpeed;
            float _CloudsRelativeSpeed;
            fixed _AtmosDistance;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.normal = v.normal;
                float2 uv = v.uv;
                uv.x += (_RotationSpeed + _CloudsRelativeSpeed) * _Time[1];
                o.uv = TRANSFORM_TEX(uv, _MainTex);
                o.viewDir = float3(0, 0, 0);
                o.diff = float4(0, 0, 0, 0);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_CloudsTex, i.uv);
                col.rgb = fixed3(0, 0, 0);
                return col;
            }
            ENDCG
        }

        Pass
        {
            Tags { "LightMode" = "ForwardAdd" }

            Name "CITIES"

            Blend SrcAlpha OneMinusSrcAlpha
            ZTest LEqual
            ZWrite Off

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            sampler2D _CityTex;
            float4 _MainTex_ST;

            float _RotationSpeed;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                float2 uv = v.uv;
                uv.x += _RotationSpeed * _Time[1];
                o.uv = TRANSFORM_TEX(uv, _MainTex);
                o.normal = UnityObjectToWorldNormal(v.normal);
                o.viewDir = float3(0, 0, 0);
                o.diff = ComputeLight1(mul(unity_ObjectToWorld, v.vertex), -o.normal);
                return o;
            }

            fixed4 _CityTint;

            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 col = tex2D(_CityTex, i.uv);
                col *= _CityTint;
                col.a = lerp(0, col.a, i.diff.a);

                return col;
            }
            ENDCG
        }

        Pass {
            Tags { "LightMode" = "ForwardAdd" }

            Name "CLOUDS"

            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            sampler2D _CloudsTex;
            float4 _MainTex_ST;

            float _RotationSpeed;
            float _CloudsRelativeSpeed;
            fixed _AtmosDistance;

            v2f vert (appdata v)
            {
                v2f o;
                o.normal = UnityObjectToWorldNormal(v.normal);
                o.vertex = UnityObjectToClipPos(v.vertex + v.normal * _AtmosDistance * 0.25);
                float2 uv = v.uv;
                uv.x += (_RotationSpeed + _CloudsRelativeSpeed) * _Time[1];
                o.uv = TRANSFORM_TEX(uv, _MainTex);
                o.viewDir = normalize(WorldSpaceViewDir(v.vertex));

                o.diff = ComputeLight1(mul(unity_ObjectToWorld, v.vertex), o.normal);
                return o;
            }

            fixed4 _AtmosColor;
            fixed _AtmosFresnelLimit;
            fixed _AtmosFresnelExponent;

            fixed4 frag (v2f i) : SV_Target
            {
                float dotProduct = FresnelDotProduct(i, _AtmosFresnelLimit, _AtmosFresnelExponent);

                fixed4 col = tex2D(_CloudsTex, i.uv);
                col.rgb = lerp(_AtmosColor.rgb, col.rgb, dotProduct);
                return fixed4(col.rgb * i.diff.rgb, col.a);
                return col;
            }
            ENDCG
        }

        Pass {
            Tags { "LightMode" = "ForwardAdd" }

            Name "INNER_ATMOSPHERE"

            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            fixed _AtmosDistance;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.normal = UnityObjectToWorldNormal(v.normal);
                o.viewDir = normalize(WorldSpaceViewDir(v.vertex));
                o.uv = float2(0, 0);

                o.diff = ComputeLight1(mul(unity_ObjectToWorld, v.vertex), o.normal);
                return o;
            }

            fixed4 _AtmosColor;

            fixed4 frag (v2f i) : SV_Target
            {
                float dotProduct = FresnelDotProduct(i, 1, 1);
                _AtmosColor.a *= 1.0 - dotProduct;
                _AtmosColor.rgb *= i.diff.rgb;
                return _AtmosColor;
            }
            ENDCG
        }
    }
}
