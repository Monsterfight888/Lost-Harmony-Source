// Unity built-in shader source. Copyright (c) 2016 Unity Technologies. MIT license (see license.txt)

Shader "Sprites/GrayScaleSprite"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)
        [MaterialToggle] PixelSnap ("Pixel snap", Float) = 0
        [HideInInspector] _RendererColor ("RendererColor", Color) = (1,1,1,1)
        [HideInInspector] _Flip ("Flip", Vector) = (1,1,1,1)
        [PerRendererData] _AlphaTex ("External Alpha", 2D) = "white" {}
        [PerRendererData] _EnableExternalAlpha ("Enable External Alpha", Float) = 0
        _UnEffectedRaduis ("UnEffectedRaduis", float) = 3
        _Offset ("Offset", Vector) = (.25, .5, .5, 1)
         //_Strength ("Strength", float) = 3
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
        }

        Cull Off
        Lighting Off
        ZWrite Off
        Blend One OneMinusSrcAlpha

        Pass
        {
        CGPROGRAM
            #pragma vertex MySpriteVert
            //#pragma fragment SpriteFrag
            #pragma fragment MyGreyFrag
            #pragma target 3.0
            #pragma multi_compile_instancing
            #pragma multi_compile_local _ PIXELSNAP_ON
            #pragma multi_compile _ ETC1_EXTERNAL_ALPHA
            #include "UnitySprites.cginc" //like c++ - is including v2f built in struct & appdata & all the functions like SpriteVert - can find unitysprite.cginc file online
            
		    uniform float _UnEffectedRaduis;
		    //uniform float _Strength;
            uniform float4 _Offset;
            struct Myv2f //modified v2f struct so can carry world position into fragment shader
            {
                float4 vertex   : SV_POSITION;
                fixed4 color : COLOR;
                float2 texcoord : TEXCOORD0;
                float4 worldSpacePos : TEXCOORD1;
                UNITY_VERTEX_OUTPUT_STEREO
            };
            Myv2f MySpriteVert(appdata_t IN) //modified version of SpriteVert - just so could pass through the worldposition
            {
                Myv2f OUT;

                UNITY_SETUP_INSTANCE_ID(IN);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);

                OUT.vertex = UnityFlipSprite(IN.vertex, _Flip);
                OUT.vertex = UnityObjectToClipPos(OUT.vertex);
                OUT.texcoord = IN.texcoord;
                OUT.color = IN.color * _Color * _RendererColor;
                OUT.worldSpacePos = mul(unity_ObjectToWorld, IN.vertex);

                #ifdef PIXELSNAP_ON
                OUT.vertex = UnityPixelSnap(OUT.vertex);
                #endif

                return OUT;
            }

            fixed4 MyGreyFrag(Myv2f IN) : SV_Target
            {
                fixed4 c = SampleSpriteTexture (IN.texcoord) * IN.color/* *sin(IN.worldSpacePos)*/;
                fixed4 ogc = c;
                float4 thisPos =  IN.worldSpacePos - _Offset;

                float intensity = c.x*.299 + c.y * .587 + c.z *.114;
                float dist = sqrt(thisPos.x * thisPos.x + thisPos.y * thisPos.y);
                dist = clamp((dist / _UnEffectedRaduis), 0, 1);

                c = fixed4(lerp(ogc.x, intensity, dist), lerp(ogc.y, intensity, dist), lerp(ogc.z, intensity * 1.05, dist), c.w);

                

                c.rgb *= c.a;
                //c.z = c.z * 1.05; //slight blue tint
                return c;
            }
        ENDCG



        }
    }
}