Shader "Fisheye"
{
	Properties
	{
		_MainTex("_MainTex", CUBE) = "" {}
	}
    SubShader
    {
        Pass
        {
            CGPROGRAM
            #include "UnityCG.cginc"

            #pragma vertex vert_img
            #pragma fragment frag

			samplerCUBE _MainTex;
            float _FOV;
			float4x4 _CameraRotation;
        
            fixed4 frag(v2f_img i) : SV_Target
            {
                // Resize the UVs to -1 to 1.
                i.uv = i.uv * 2.0 - 1.0;

                // If we're outside the unit circle, mask to black, as this will never be displayed.
				float r = length(i.uv);
				if (r > 1.0) return float4(0, 0, 0, 1);

                // Calculate the angle into the cubemap
				float theta = r * radians(_FOV / 2);
				float phi = atan2(i.uv.y, i.uv.x);
				float3 dir;
				dir.x = sin(theta) * cos(phi);
				dir.y = sin(theta) * sin(phi);
				dir.z = cos(theta);

                // Apply the camera rotation matrix.
                float3x3 rot = _CameraRotation;
				dir = mul(rot, dir);
            
                // Apply some budget MSAA by slighly changing the rotation to sample the cubemap.
                float3 off1 = mul(rot, float3(0.002, 0, 0));
                float3 off2 = mul(rot, float3(-0.002, 0, 0));
                float3 off3 = mul(rot, float3(0, 0.002, 0));
                float3 off4 = mul(rot, float3(0, -0.002, 0));

                fixed4 color = texCUBE(_MainTex, dir);
                color += texCUBE(_MainTex, normalize(dir + off1));
                color += texCUBE(_MainTex, normalize(dir + off2));
                color += texCUBE(_MainTex, normalize(dir + off3));
                color += texCUBE(_MainTex, normalize(dir + off4));
                color /= 5.0;

				return color;
            }
            ENDCG
        }
    }
}
