//All credits to the creator of this shader  "ushiostarfish "
//that is only what i know about this owesome guy...,no more details available!!

Shader "Handwritten/water3" { 
	Properties {
     _MainTex ("Texture Image", 2D) = "white" {} 
         _Speed ("Wave Speed", Float) = 5.0
         _Slope ("Slope",Float) = .10 
   		   	  }
	SubShader { 
	Pass { 
         GLSLPROGRAM 
         
         uniform sampler2D _MainTex;
         uniform float _Speed;
         uniform float _Slope;	
         varying vec4 textureCoordinates; 
         #include "UnityCG.glslinc"         
 		 #define PI 3.14159265358979
		 #define N 14 
         #ifdef VERTEX // here begins the vertex shader
         void main() 
         {
            gl_Position = gl_ModelViewProjectionMatrix * gl_Vertex;
           
             
         }
         
         #endif // here ends the definition of the vertex shader
 		 #ifdef FRAGMENT // here begins the fragment shader
 
         void main() 
         {
          vec2 uv = gl_FragCoord.xy / _ScreenParams.xy;
          float w = (0.5 - (uv.x)) * (_ScreenParams.x / _ScreenParams.y);
          float h = 0.5 - uv.y;
	      float distanceFromCenter = sqrt(w * w + h * h);
	      float sinArg = distanceFromCenter * 10.0 - _Time.y * _Speed;
	      float slope = cos(sinArg) ;
	      vec4 color = texture2D(_MainTex, uv + normalize(vec2(w, h)) * slope * _Slope);
          gl_FragColor =color;
          
         }
 
         #endif // here ends the definition of the fragment shader
         
 
         ENDGLSL 
      }
   }
   }
