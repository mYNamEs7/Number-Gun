Shader "Custom/StencilMask"
{
    Properties
    {
        _Color ("Mask Color", Color) = (1,1,1,1) // Цвет маски
    }
    SubShader
    {
        Tags { "Queue" = "Geometry+10" }
        Stencil
        {
            Ref 1
            Comp always
            Pass replace
        }
        Pass
        {
            // Маска записывается только в буфер стэнсила
            ColorMask 0   // Не рендерим цвет маски
            ZWrite Off    // Не записываем глубину, чтобы не перекрывать объекты
        }
    }
}
