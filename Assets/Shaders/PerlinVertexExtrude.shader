Shader "Perlin Normal Extrusion" 
{
    Properties 
    {
      //_MainTex      ("Texture", 2D) = "white" {}
      _MainTex      ("Color (RGB) Alpha (A)", 2D) = "white"
      _SpecMap      ("SpecMap", 2D) = "white" {}
      _Amount       ("Extrusion Amount", Range(-0.03,0.1)) = 0.5
      _AzimSpeed    ("Azimuth Deformation Speed", Range (-100, 100)) = 1
      _InclSpeed    ("Inclination Deformation Speed", Range (-100, 100)) = 1
      _AzimFreq     ("Azimuth Frequency", Range (1, 10000)) = 1
      _InclFreq     ("Inclination Frequency", Range (1, 10000)) = 1
      _AzimAmount   ("Azimuth Deformation Amount", Range (0, 1)) = 0.2
      _InclAmount   ("Inclination Deformation Amount", Range (0, 1)) = 0.2
      _NAmount		("Bump Mapping Amount", Range (0, 1)) = 0.5
      _Granularity  ("Bump Mapping Granularity", Range (0, 10)) = 1.0
      _NSpeed       ("Bump Mapping Animation Speed", Range (0, 10)) = 1.0
      _SpecularRad  ("Specular Radius", Range(0,300)) = 30
      _AmbientCol   ("Ambient Colour", Vector) = (1, 1, 1)
      _Alpha 		("Opacity", Range (0, 1)) = 0.5
      _Brightness   ("Brightness", Range (0, 1)) = 0.5
    }

    SubShader 
    {
    	
    
        Tags { "RenderType" = "Transparent" }
        LOD 200
        //Tags { "Queue"="Transparent" "RenderType"="Transparent" }

        CGPROGRAM
        #pragma target 3.0
        #pragma surface surf ColoredSpecular vertex:vert
        //#pragma surface surf Lambert alpha vertex:vert
        //uniform sampler2D permutation;
      
        int perm(int d)
        {
            d = d % 256;
            float2 t = float2(d%16,d/16)/15.0;
            //return tex2D(permutation,t).r *255;
            return t * 255;
        }

        float fade(float t) { return t * t * t * (t * (t * 6.0 - 15.0) + 10.0); }

        float lerp(float t,float a,float b) { return a + t * (b - a); }

        float grad(int hash,float x,float y,float z)
        {
        	int h	= hash % 16;										// & 15;
        	float u = h<8 ? x : y;
        	float v = h<4 ? y : (h==12||h==14 ? x : z);
        	return ((h%2) == 0 ? u : -u) + (((h/2)%2) == 0 ? v : -v); 	// h&1, h&2 
        }

        float noise(float x, float y,float z)
        {	
        	int X = (int)floor(x) % 256;	// & 255;
        	int Y = (int)floor(y) % 256;	// & 255;
        	int Z = (int)floor(z) % 256;	// & 255;
        	
        	x -= floor(x);
        	y -= floor(y);
        	z -= floor(z);
              
        	float u = fade(x);
        	float v = fade(y);
        	float w = fade(z);
        	
            int A	= perm(X  	)+Y;
            int AA	= perm(A	)+Z;
        	int AB	= perm(A+1	)+Z; 
        	int B	= perm(X+1	)+Y;
        	int BA	= perm(B	)+Z;
        	int BB	= perm(B+1	)+Z;

	        return lerp(w, lerp(v, lerp(u, grad(perm(AA  ), x  , y  , z   ),
		                                    grad(perm(BA  ), x-1, y  , z   )),
		                            lerp(u, grad(perm(AB  ), x  , y-1, z   ),
		                                    grad(perm(BB  ), x-1, y-1, z   ))),
		                    lerp(v, lerp(u, grad(perm(AA+1), x  , y  , z-1 ),
		                                    grad(perm(BA+1), x-1, y  , z-1 )),
		                            lerp(u, grad(perm(AB+1), x  , y-1, z-1 ),
		                                    grad(perm(BB+1), x-1, y-1, z-1 ))));
	    }
        
        struct Input 
        {
            float2 uv_MainTex;
            float2 uv_SpecMap;
        };

        //vertex deformation
        float _Amount;
      	float _AzimSpeed;
      	float _InclSpeed;
      	float _AzimFreq;
      	float _InclFreq;
        float _AzimAmount;
        float _InclAmount;
      	//bump mapping (normal deformation)
      	float _NAmount;
      	float _Granularity;
      	float _NSpeed;
        
        float3 getSphericalCoordinates (float3 xyz)
        {
        	float rad = sqrt ((xyz.x * xyz.x) + (xyz.y * xyz.y) + (xyz.z * xyz.z));
        	float inclination = acos (xyz.z / rad);
        	float azimuth = atan (xyz.y / xyz.x);
        	return float3 (rad, inclination, azimuth);
        }
        void vert (inout appdata_full v) 
        {
        	const float pi = 3.14159265359;
        	const float pi2 = 2.0 * pi;
        	float3 radIncAzim = getSphericalCoordinates (v.vertex.xyz);
        	// (n for normed)
        	float nAzim = radIncAzim.z / pi2;
        	float nIncl = radIncAzim.y / pi2;
            
            //deform three points
            float azimOffset = _Time * _AzimSpeed;//floor((_Time * _AzimSpeed) / pi2) * pi2;
            float inclOffset = _Time * _InclSpeed;
            float azimFunc = azimOffset + nAzim * (_AzimFreq);
            float inclFunc = inclOffset + nIncl * (_InclFreq);
        	v.vertex.xyz += _Amount * (sin (azimFunc) * _AzimAmount - sin (inclFunc) * _InclAmount) * v.normal.xyz;  
        	v.normal.xyz += noise ((sin(v.vertex.x * _Granularity) + _Time * _NSpeed), 
        						   (sin(v.vertex.y * _Granularity) + _Time * _NSpeed), 
        						   (sin(v.vertex.z * _Granularity) + _Time * _NSpeed)) * _NAmount;

        }
      
        struct MySurfaceOutput 
        {
	        half3 Albedo;
	        half3 Normal;
	        half3 Emission;
	        half Specular;
	        half3 GlossColor;
	        half Alpha;
	    };
      
        float _SpecularRad;
        
        inline half4 LightingColoredSpecular (MySurfaceOutput s, half3 lightDir, half3 viewDir, half atten)
   	    {	
	        half3 h = normalize (lightDir + viewDir);
 	 
	        half diff = max (0, dot (s.Normal, lightDir));
	 
  	        float nh = max (0, dot (s.Normal, h));
	        float spec = pow (nh, _SpecularRad);
	        half3 specCol = spec * s.GlossColor;
	 
	        half4 c;
	        c.rgb = (s.Albedo * _LightColor0.rgb * diff + _LightColor0.rgb * specCol) * (atten * 2);
	        c.a = s.Alpha;
	        return c;
	    }
	 
	    inline half4 LightingColoredSpecular_PrePass (MySurfaceOutput s, half4 light)
 	    {
	        half3 spec = light.a * s.GlossColor;
	    
	        half4 c;
	        c.rgb = (s.Albedo * light.rgb + light.rgb * spec);
	        c.a = s.Alpha;// + spec * _SpecColor.a;
	        return c;
	    }
		
		float3 Hue(float H)
        {
            half R = abs(H * 6 - 3) - 1;
            half G = 2 - abs(H * 6 - 2);
            half B = 2 - abs(H * 6 - 4);
            return saturate(half3(R,G,B));
        }

        float3 HSVtoRGB(in half3 HSV)
        {
            return float3 (((Hue(HSV.x) - 1) * HSV.y + 1) * HSV.z);
        }
        
        sampler2D _MainTex;
        sampler2D _SpecMap;
        float3 _AmbientCol;
		float _Brightness;
		float _Hue;
		float _Saturation;
		float _Alpha;

        void surf (Input IN, inout MySurfaceOutput o) 
        {
            //o.Albedo = tex2D (_MainTex, IN.uv_MainTex).rgb;
            float3 surfaceColour = _AmbientCol;//tex2D (_MainTex, IN.uv_MainTex).rgb;
            o.Albedo = surfaceColour * (0.3 + _Brightness);//tex2D (_MainTex, IN.uv_MainTex).rgb * 0.3;
            half4 spec = tex2D (_SpecMap, IN.uv_SpecMap);
            o.GlossColor = float3 (0.5f, 0.5f, 0.5f) + spec.rgb * HSVtoRGB (half3 (_Hue, _Saturation, _Brightness));
            o.Specular = _SpecularRad;//32.0/128.0;
            o.Alpha = _Alpha;
        }

	    ENDCG
     
    }
    Fallback "Diffuse"
}
