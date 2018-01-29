using UnityEngine;
using UnityEditor;
using System.Collections;


/// <summary>
/// https://zhuanlan.zhihu.com/p/28624490
/// <br>
/// 对keijiro原版基础上的改进，提供了一个将RGB24bit图dither之后转RGB565的方法，基本消除了肉眼可见的失真
/// 实际在我的项目的应用中，对于不适合ETC/PVRTC压缩的图片，都采用了该文章中的RGB565或者RGB565+A8的方式。在肉眼基本无失真的基础上，节省了部分资源。
/// </summary>
/// 
class TextureModifier2 : AssetPostprocessor
{
	void OnPreprocessTexture ()
	{
		var importer = (assetImporter as TextureImporter);

//		importer.textureType = TextureImporterType.GUI;

		if (assetPath.EndsWith ("Dither.png")) {
			importer.textureFormat = TextureImporterFormat.RGBA32;
		}
	}

	void OnPostprocessTexture (Texture2D texture)
	{
		if (assetPath.Contains ("_dither565")) {
			var texw = texture.width;
			var texh = texture.height;

			var pixels = texture.GetPixels ();
			var offs = 0;

			var k1Per31 = 1.0f / 31.0f;

			var k1Per32 = 1.0f / 32.0f;
			var k5Per32 = 5.0f / 32.0f;
			var k11Per32 = 11.0f / 32.0f;
			var k15Per32 = 15.0f / 32.0f;

			var k1Per63 = 1.0f / 63.0f;

			var k3Per64 = 3.0f / 64.0f;
			var k11Per64 = 11.0f / 64.0f;
			var k21Per64 = 21.0f / 64.0f;
			var k29Per64 = 29.0f / 64.0f;

			var k_r = 32; //R&B压缩到5位，所以取2的5次方
			var k_g = 64; //G压缩到6位，所以取2的6次方

			for (var y = 0; y < texh; y++) {
				for (var x = 0; x < texw; x++) {
					float r = pixels [offs].r;
					float g = pixels [offs].g;
					float b = pixels [offs].b;

					var r2 = Mathf.Clamp01 (Mathf.Floor (r * k_r) * k1Per31);
					var g2 = Mathf.Clamp01 (Mathf.Floor (g * k_g) * k1Per63);
					var b2 = Mathf.Clamp01 (Mathf.Floor (b * k_r) * k1Per31);

					var re = r - r2;
					var ge = g - g2;
					var be = b - b2;

					var n1 = offs + 1;
					var n2 = offs + texw - 1;
					var n3 = offs + texw;
					var n4 = offs + texw + 1;

					if (x < texw - 1) {
						pixels [n1].r += re * k15Per32;
						pixels [n1].g += ge * k29Per64;
						pixels [n1].b += be * k15Per32;
					}

					if (y < texh - 1) {
						pixels [n3].r += re * k11Per32;
						pixels [n3].g += ge * k21Per64;
						pixels [n3].b += be * k11Per32;

						if (x > 0) {
							pixels [n2].r += re * k5Per32;
							pixels [n2].g += ge * k11Per64;
							pixels [n2].b += be * k5Per32;
						}

						if (x < texw - 1) {
							pixels [n4].r += re * k1Per32;
							pixels [n4].g += ge * k3Per64;
							pixels [n4].b += be * k1Per32;
						}
					}

					pixels [offs].r = r2;
					pixels [offs].g = g2;
					pixels [offs].b = b2;

					offs++;
				}
			}

			texture.SetPixels (pixels);
			EditorUtility.CompressTexture (texture, TextureFormat.RGB565, TextureCompressionQuality.Best);
		}
	}
}
