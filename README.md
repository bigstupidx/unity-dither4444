unity-dither4444
================

原github地址：https://github.com/keijiro/unity-dither4444

知乎上有篇讲解的文章：https://zhuanlan.zhihu.com/p/28624490

通常来说，对于3D物体的纹理，是可以采用ETC/PVRTC等压缩比很大的算法处理的，但是对于细节要求很高的UI纹理，这样处理造成的失真往往达不到质量要求。对于这类的贴图，我们可以考虑使用失真较小的16位的贴图格式存储。

但是对于颜色数较高的纹理，Unity提供的默认转换方法会呈现明显的色带。针对该问题，keijiro实现了一种dither4444的改进算法。从图1上可以看到，对于画面细节比较平滑的图片，**该算法虽然消除了色带现象，同时带来了肉眼可见的噪点。**

笔者在keijiro的算法基础上进行了改进，提供了一个将RGB24bit图dither之后转RGB565的方法，基本消除了肉眼可见的失真，实际效果见图2。
对于不适合ETC/PVRTC压缩的图片，都采用了该文章中的RGB565或者RGB565+A8的方式。在肉眼基本无失真的基础上，节省了部分资源。

This example shows how to make a high-quality 16-bit color texture in Unity.

Abstract
--------
本人测试的结果是，使用原版dither算法，在放大很多很多倍之后是这样的：
![](image/11.png)

Unity supports 16-bit color as a texture color format, however it introduces significant color banding.

![Image A (original)](http://keijiro.github.io/unity-dither4444/a-original.png)![Image A (default)](http://keijiro.github.io/unity-dither4444/a-default.png)

![Image B (original)](http://keijiro.github.io/unity-dither4444/b-original.png)![Image B (default)](http://keijiro.github.io/unity-dither4444/b-default.png)

*(Left: original image, Right: 16-bit converted image)*

It’s mainly because of lack of dither -- Unity simply quantizes the image to 16-bit without any fancy algorithms. It can be improved by dithering the image before quantization. This example does it in the AssetPostProcessor script.

![Image A (original)](http://keijiro.github.io/unity-dither4444/a-original.png)![Image A (dithered)](http://keijiro.github.io/unity-dither4444/a-dither.png)

![Image B (original)](http://keijiro.github.io/unity-dither4444/b-original.png)![Image B (dithered)](http://keijiro.github.io/unity-dither4444/b-dither.png)

*(Left: original image, Right: 16-bit image with dithering)*

Usage
-----

Add “Dither” to the end of the filename, or import an image with the “Dither” suffix. The AssetPostProcessor script automatically detects and convert it. You can change the behavior by modifying the script. See [the script](https://github.com/keijiro/unity-dither4444/blob/master/Assets/Editor/TextureModifier.cs) for further details.
