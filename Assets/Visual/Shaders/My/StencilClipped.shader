Shader "Custom/StencilClippedOpaque"
{
    Properties
    {
        _Color ("Fill Color", Color) = (1,1,1,1) // Цвет второго объекта
    }
    SubShader
    {
        Tags { "Queue" = "Geometry+20" }
        Stencil
        {
            Ref 1
            Comp equal    // Проверяем, что значение стэнсила равно 1
        }
        Pass
        {
            // Отрисовка второго объекта
            Color [_Color]
            ColorMask RGBA
            ZWrite On
        }
    }
}
