using UnityEditor;
using UnityEngine;

public class SaveTools
{
    [MenuItem("Tools/Clear PlayerPrefs")]
    public static void ClearSave()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
        Debug.Log("<color=red>Все сохранения стерты!</color>");
    }
}