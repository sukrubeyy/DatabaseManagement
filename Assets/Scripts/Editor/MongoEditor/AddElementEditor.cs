using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class AddElementEditor : EditorWindow
{
    private static AddElementEditor Window;

    public static void Initialize()
    {
        Window = GetWindow<AddElementEditor>("Add Element");
        Window.Show();
    }
}
