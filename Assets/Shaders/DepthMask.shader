Shader "Custom/DepthMask" {
    SubShader {
        // Îi spunem să se randeze imediat DUPĂ obiectele normale (Queue 2001)
        Tags { "Queue" = "Geometry+1" }
        
        // Nu desena nicio culoare (va fi invizibil)
        ColorMask 0
        
        // Dar blochează obiectele care se randează după el
        ZWrite On
        
        Pass {}
    }
}