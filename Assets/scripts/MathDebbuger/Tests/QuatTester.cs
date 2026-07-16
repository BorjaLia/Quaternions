using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CustomMath;

public class QuatTester : MonoBehaviour
{
    [Header("Testing Parameters")]
    public static float tolerance = 0.0001f;

    void Start()
    {
        RunQuatTests();
    }

    public static void RunQuatTests()
    {
        Debug.Log("<color=yellow>--- INICIANDO TESTS DINÁMICOS: QUAT vs QUATERNION ---</color>");

        Quaternion uQ1 = Random.rotation;
        Quaternion uQ2 = Random.rotation;
        Vector3 uEuler = new Vector3(Random.Range(-180f, 180f), Random.Range(-180f, 180f), Random.Range(-180f, 180f));
        Vector3 uVec = Random.onUnitSphere * Random.Range(1f, 10f);
        Vector3 uAxis = Random.onUnitSphere;
        float angle = Random.Range(-360f, 360f);
        float t = Random.Range(-1f, 2f);

        Debug.Log($"<color=cyan><b>[Valores Aleatorios Generados]</b></color>\n" +
                  $"<b>Quat 1:</b> {uQ1:F4}\n" +
                  $"<b>Quat 2:</b> {uQ2:F4}\n" +
                  $"<b>Euler:</b> {uEuler:F4}\n" +
                  $"<b>Vector:</b> {uVec:F4}\n" +
                  $"<b>Ángulo / Eje:</b> {angle:F2} / {uAxis:F4}\n" +
                  $"<b>T (Lerp):</b> {t:F4}");

        Quat vQ1 = new Quat(uQ1.x, uQ1.y, uQ1.z, uQ1.w);
        Quat vQ2 = new Quat(uQ2.x, uQ2.y, uQ2.z, uQ2.w);
        Vec3 vEuler = new Vec3(uEuler);
        Vec3 vVec = new Vec3(uVec);
        Vec3 vAxis = new Vec3(uAxis);

        int failedTests = 0;

        if (!TestMultiplication(uQ1, uQ2, uVec, vQ1, vQ2, vVec)) failedTests++;
        if (!TestDotAngle(uQ1, uQ2, vQ1, vQ2)) failedTests++;
        if (!TestInverse(uQ1, vQ1)) failedTests++;
        if (!TestLerpSlerp(uQ1, uQ2, vQ1, vQ2, t)) failedTests++;
        if (!TestEuler(uEuler, vEuler)) failedTests++;
        if (!TestAngleAxis(angle, uAxis, vAxis)) failedTests++;
        if (!TestLookRotation(uVec, vVec)) failedTests++;
        if (!TestNormalize(uQ1, vQ1)) failedTests++;

        if (failedTests > 0)
        {
            TestManager.error = true;
            Debug.Log($"<color=red>--- NO TODOS LOS TEST PASARON ({failedTests} ERRORES) ---</color>");
        }
        else Debug.Log("<color=green>--- TODOS LOS TESTS QUAT FINALIZADOS CON ÉXITO ---</color>");
    }

    static bool TestMultiplication(Quaternion uQ1, Quaternion uQ2, Vector3 uVec, Quat vQ1, Quat vQ2, Vec3 vVec)
    {
        bool result = true;
        result &= AssertQuatMatch(uQ1 * uQ2, vQ1 * vQ2, "Multiplicación de Rotaciones (*)");
        result &= AssertVectorMatch(uQ1 * uVec, vQ1 * vVec, "Rotación de Vector (*)");
        return result;
    }

    static bool TestDotAngle(Quaternion uQ1, Quaternion uQ2, Quat vQ1, Quat vQ2)
    {
        bool result = true;
        result &= AssertFloatMatch(Quaternion.Dot(uQ1, uQ2), Quat.Dot(vQ1, vQ2), "Producto Punto (Dot)");
        result &= AssertFloatMatch(Quaternion.Angle(uQ1, uQ2), Quat.Angle(vQ1, vQ2), "Ángulo entre Cuaterniones (Angle)");
        return result;
    }

    static bool TestInverse(Quaternion uQ, Quat vQ)
    {
        return AssertQuatMatch(Quaternion.Inverse(uQ), Quat.Inverse(vQ), "Cuaternión Inverso (Inverse)");
    }

    static bool TestLerpSlerp(Quaternion uQ1, Quaternion uQ2, Quat vQ1, Quat vQ2, float t)
    {
        bool result = true;
        result &= AssertQuatMatch(Quaternion.Lerp(uQ1, uQ2, t), Quat.Lerp(vQ1, vQ2, t), "Interpolación Lineal (Lerp Clamped)");
        result &= AssertQuatMatch(Quaternion.LerpUnclamped(uQ1, uQ2, t), Quat.LerpUnclamped(vQ1, vQ2, t), "Interpolación Lineal (LerpUnclamped)");
        result &= AssertQuatMatch(Quaternion.Slerp(uQ1, uQ2, t), Quat.Slerp(vQ1, vQ2, t), "Interpolación Esférica (Slerp Clamped)");
        result &= AssertQuatMatch(Quaternion.SlerpUnclamped(uQ1, uQ2, t), Quat.SlerpUnclamped(vQ1, vQ2, t), "Interpolación Esférica (SlerpUnclamped)");
        return result;
    }

    static bool TestEuler(Vector3 uEuler, Vec3 vEuler)
    {
        return AssertQuatMatch(Quaternion.Euler(uEuler), Quat.Euler(vEuler), "Creación por Euler (Euler)");
    }

    static bool TestAngleAxis(float angle, Vector3 uAxis, Vec3 vAxis)
    {
        return AssertQuatMatch(Quaternion.AngleAxis(angle, uAxis), Quat.AngleAxis(angle, vAxis), "Creación por Eje/Ángulo (AngleAxis)");
    }

    static bool TestLookRotation(Vector3 uFwd, Vec3 vFwd)
    {
        return AssertQuatMatch(Quaternion.LookRotation(uFwd), Quat.LookRotation(vFwd), "Rotación de Mirada (LookRotation)");
    }

    static bool TestNormalize(Quaternion uQ, Quat vQ)
    {
        return AssertQuatMatch(Quaternion.Normalize(uQ), Quat.Normalize(vQ), "Normalización Estática (Normalize)");
    }

    private static bool AssertQuatMatch(Quaternion expected, Quat actual, string testName)
    {
        Quaternion actualQ = new Quaternion(actual.x, actual.y, actual.z, actual.w);

        float dot = Quaternion.Dot(expected, actualQ);
        if (Mathf.Abs(dot) < 1.0f - tolerance)
        {
            Debug.LogError($"[FAILED] {testName}: Esperado {expected:F4}, pero se obtuvo {actualQ:F4} (Dot: {dot})");
            return false;
        }

        Debug.Log($"[PASSED] {testName}");
        return true;
    }

    private static bool AssertVectorMatch(Vector3 expected, Vec3 actual, string testName)
    {
        Vector3 actualV3 = new Vector3(actual.x, actual.y, actual.z);
        if (Vector3.Distance(expected, actualV3) > tolerance)
        {
            Debug.LogError($"[FAILED] {testName}: Esperado {expected:F4}, pero se obtuvo {actualV3:F4}");
            return false;
        }
        Debug.Log($"[PASSED] {testName}");
        return true;
    }

    private static bool AssertFloatMatch(float expected, float actual, string testName)
    {
        if (Mathf.Abs(expected - actual) > tolerance)
        {
            Debug.LogError($"[FAILED] {testName}: Esperado {expected:F4}, pero se obtuvo {actual:F4}");
            return false;
        }
        Debug.Log($"[PASSED] {testName}");
        return true;
    }
}