using UnityEngine; 

public static class TextureModifications
{
	/// <summary>
	/// Trims texture to specified width / height cropping from the edges inward.
	/// </summary>
	/// <param name="tex"></param>
	/// <param name="width"></param>
	/// <param name="height"></param>
	public static Texture2D CopyAndTrimToSize(this Texture2D tex, int width, int height)
	{
		if (width > tex.width || height > tex.height) throw new System.Exception("Cannot trim to be larger");

		tex = CopyTexture(tex);
		TrimToSize(tex, width, height);

		return tex;
	}

	/// <summary>
	/// Trims texture to specified width / height cropping from the edges inward.
	/// </summary>
	/// <param name="tex"></param>
	/// <param name="width"></param>
	/// <param name="height"></param>
	public static void TrimToSize(this Texture2D tex, int width, int height)
    {
		if (width > tex.width || height > tex.height) throw new System.Exception("Cannot trim to be larger");

		Color[] pixels = tex.GetPixels((tex.width - width) / 2, (tex.height - height) / 2, width, height);
		tex.Resize(width, height);
		tex.SetPixels(pixels);
		tex.Apply();
    }


	public static Texture2D CopyAndRescale(this Texture2D tex, int newWidth, int newHeight)
    {
		tex = CopyTexture(tex);
		Rescale(tex, newWidth, newHeight);

		return tex;
	}

	/// <summary>
	/// Uses Nearest-Neighbor scaling to scale to target width and height
	/// </summary>
	/// <param name="tex"></param>
	/// <param name="newWidth"></param>
	/// <param name="newHeight"></param>
	public static void Rescale(this Texture2D tex, int newWidth, int newHeight)
	{
		Color[] texColors = tex.GetPixels();
		Color[] newColors = new Color[newWidth * newHeight];

		float ratioX = ((float)tex.width) / newWidth;
		float ratioY = ((float)tex.height) / newHeight;

		for (var newY = 0; newY < newHeight; newY++)
		{
			var thisY = (int)(ratioY * newY) * tex.width;
			var yWidth = newY * newWidth;
			for (var x = 0; x < newWidth; x++)
			{
				newColors[yWidth + x] = texColors[(int)(thisY + ratioX * x)];
			}
		}

		tex.Resize(newWidth, newHeight);
		tex.SetPixels(newColors);
		tex.Apply();
	}

	public static Texture2D CopyTexture(Texture2D texture)
	{
		Texture2D newTexture = new Texture2D(texture.width, texture.height, TextureFormat.RGBA32, false);
		newTexture.filterMode = FilterMode.Point;

		newTexture.SetPixels32(texture.GetPixels32());

		return newTexture;
	}
}
