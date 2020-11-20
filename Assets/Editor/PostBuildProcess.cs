using System.IO;
using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;

public class PostBuildProcess {

    [PostProcessBuild(1)]
    public static void OnPostprocessBuild(BuildTarget target, string pathToBuiltProject) {
        Debug.Log(pathToBuiltProject);

        // Need to move any xml files that need to be serialized to the build's "build_Data" folder
        string pathToData = pathToBuiltProject.Replace("Hamster Scramble.exe", "Hamster Scramble_Data/");

        // Created Boards
        string source = Path.Combine(Application.dataPath + '/', "Resources/Text/Created Boards");
        FileUtil.CopyFileOrDirectory(source, pathToData + "Created Boards");

        // Shop data
        source = Path.Combine(Application.dataPath + '/', "Resources/Text/Shop/ShopItemData.xml");
        FileUtil.CopyFileOrDirectory(source, pathToData + "ShopItemData.xml");
        //File.Copy(Path.Combine(Application.dataPath, "Resources/Text/Shop/ShopItemData.xml"), pathToData + "ShopItemData.xml");
    }

}
