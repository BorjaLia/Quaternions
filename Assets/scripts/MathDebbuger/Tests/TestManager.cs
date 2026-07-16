using UnityEngine;

public class TestManager : MonoBehaviour
{
    public static bool error = false;

    void Update()
    {
        if ((Input.GetKeyDown(KeyCode.R) || Input.GetKey(KeyCode.Space)) && !error)
        {
            ClearConsole();

            QuatTester.RunQuatTests();
            Mat4x4Tester.RunMatTests();
        }
    }

    private void ClearConsole()
    {
#if UNITY_EDITOR
        var assembly = System.Reflection.Assembly.GetAssembly(typeof(UnityEditor.Editor));
        var type = assembly.GetType("UnityEditor.LogEntries");
        var method = type.GetMethod("Clear");
        method?.Invoke(new object(), null);
#endif
    }
}