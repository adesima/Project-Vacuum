Shader "Custom/DarkVoid"
{
    Properties
    {
        // Putem alege culoarea din Inspector (implicit negru pur)
        _Color ("Void Color", Color) = (0, 0, 0, 1)
    }
    SubShader
    {
        // Se va randa la coada normală (Queue 2000), ÎNAINTE de mască și de pământ
        Tags { "RenderType"="Opaque" "Queue"="Geometry" }
        LOD 100

        Pass
        {
            // Ignorăm complet sistemul de lumini din Unity
            Lighting Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
            };

            fixed4 _Color;

            v2f vert (appdata v)
            {
                v2f o;
                // Transformăm coordonatele în spațiul ecranului
                o.vertex = UnityObjectToClipPos(v.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // Returnăm exclusiv culoarea plană, fără umbre sau reflexii
                return _Color;
            }
            ENDCG
        }
    }
}