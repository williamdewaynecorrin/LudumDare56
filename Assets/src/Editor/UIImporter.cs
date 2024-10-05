using UnityEditor;

public class UIImporter : AssetPostprocessor
{
    // -- this is totally not the right way to normally do this, but its a game jam lol 
    void OnPreprocessTexture()
    {
        if (assetPath.Contains("ui_tex"))
        {
            TextureImporter textureImporter = (TextureImporter)assetImporter;
            if (textureImporter.textureType != TextureImporterType.Sprite)
            {
                textureImporter.textureType = TextureImporterType.Sprite;
                textureImporter.spriteImportMode = SpriteImportMode.Single; 
                textureImporter.mipmapEnabled = false;
                textureImporter.textureCompression = TextureImporterCompression.Uncompressed;
            }
        }
    }
}