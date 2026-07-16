using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CustomMath;

public class Mat4x4Tester : MonoBehaviour
{
    [Header("Testing Parameters")]
    public static float tolerance = 0.001f;

    void Start()
    {
        RunMatTests();
    }

    public static void RunMatTests()
    {
        Debug.Log("<color=yellow>--- INICIANDO TESTS DINÁMICOS: MAT4x4 vs MATRIX4x4 ---</color>");

        Vector3 uPos1 = Random.insideUnitSphere * 10f;
        Quaternion uRot1 = Random.rotation;
        Vector3 uScale1 = new Vector3(Random.Range(0.5f, 2f), Random.Range(0.5f, 2f), Random.Range(0.5f, 2f));
        Matrix4x4 uM1 = Matrix4x4.TRS(uPos1, uRot1, uScale1);

        Vector3 uPos2 = Random.insideUnitSphere * 10f;
        Quaternion uRot2 = Random.rotation;
        Vector3 uScale2 = new Vector3(Random.Range(0.5f, 2f), Random.Range(0.5f, 2f), Random.Range(0.5f, 2f));
        Matrix4x4 uM2 = Matrix4x4.TRS(uPos2, uRot2, uScale2);

        Vector3 uPt = Random.insideUnitSphere * 5f;
        Vector4 uVec = new Vector4(Random.Range(-5f, 5f), Random.Range(-5f, 5f), Random.Range(-5f, 5f), Random.Range(0f, 1f));

        Debug.Log($"<color=cyan><b>[Valores Aleatorios Generados]</b></color>\n" +
                  $"<b>Matrix 1 (Pos,Rot,Scale):</b> {uPos1:F2} | {uScale1:F2}\n" +
                  $"<b>Matrix 2 (Pos,Rot,Scale):</b> {uPos2:F2} | {uScale2:F2}\n" +
                  $"<b>Punto a transformar:</b> {uPt:F2}");

        Mat4x4 vM1 = new Mat4x4(uM1.GetColumn(0), uM1.GetColumn(1), uM1.GetColumn(2), uM1.GetColumn(3));
        Mat4x4 vM2 = new Mat4x4(uM2.GetColumn(0), uM2.GetColumn(1), uM2.GetColumn(2), uM2.GetColumn(3));

        Vec3 vPos1 = new Vec3(uPos1);
        Quat vRot1 = new Quat(uRot1.x, uRot1.y, uRot1.z, uRot1.w);
        Vec3 vScale1 = new Vec3(uScale1);
        Vec3 vPt = new Vec3(uPt);

        int failedTests = 0;

        if (!TestProperties(uM1, vM1)) failedTests++;
        if (!TestMultiplication(uM1, uM2, vM1, vM2)) failedTests++;
        if (!TestMultiplyPointAndVector(uM1, uPt, uVec, vM1, vPt)) failedTests++;
        if (!TestTRS(uPos1, uRot1, uScale1, vPos1, vRot1, vScale1)) failedTests++;
        if (!TestTransformations(uPos1, uRot1, uScale1, vPos1, vRot1, vScale1)) failedTests++;

        if (failedTests > 0)
        {
            TestManager.error = true;
            Debug.Log($"<color=red>--- NO TODOS LOS TEST PASARON ({failedTests} ERRORES) ---</color>");
        }
        else Debug.Log("<color=green>--- TODOS LOS TESTS MAT4X4 FINALIZADOS CON ÉXITO ---</color>");
    }

    static bool TestProperties(Matrix4x4 uM, Mat4x4 vM)
    {
        bool result = true;
        result &= AssertFloatMatch(uM.determinant, vM.determinant, "Determinante");
        result &= AssertMatrixMatch(uM.inverse, vM.inverse, "Inversa (inverse)");
        result &= AssertMatrixMatch(uM.transpose, vM.transpose, "Transpuesta (transpose)");
        result &= AssertVectorMatch(uM.lossyScale, vM.lossyScale, "Escala Global (lossyScale)");
        return result;
    }

    static bool TestMultiplication(Matrix4x4 uM1, Matrix4x4 uM2, Mat4x4 vM1, Mat4x4 vM2)
    {
        return AssertMatrixMatch(uM1 * uM2, vM1 * vM2, "Multiplicación Matriz * Matriz (*)");
    }

    static bool TestMultiplyPointAndVector(Matrix4x4 uM, Vector3 uPt, Vector4 uVec, Mat4x4 vM, Vec3 vPt)
    {
        bool result = true;
        result &= AssertVectorMatch(uM.MultiplyPoint(uPt), vM.MultiplyPoint(vPt), "MultiplyPoint (Asume w=1 y divide)");
        result &= AssertVectorMatch(uM.MultiplyPoint3x4(uPt), vM.MultiplyPoint3x4(vPt), "MultiplyPoint3x4 (Optimizado w=1)");
        result &= AssertVectorMatch(uM.MultiplyVector(uPt), vM.MultiplyVector(vPt), "MultiplyVector (Asume w=0)");

        Vector4 uRes = uM * uVec;
        Vector4 vRes = vM * uVec;
        if (Vector4.Distance(uRes, vRes) > tolerance)
        {
            Debug.LogError($"[FAILED] Multiplicación por Vector4 (*): Esperado {uRes:F4}, obtenido {vRes:F4}");
            result = false;
        }
        else Debug.Log("[PASSED] Multiplicación por Vector4 (*)");

        return result;
    }

    static bool TestTRS(Vector3 pos, Quaternion rot, Vector3 scale, Vec3 vPos, Quat vRot, Vec3 vScale)
    {
        return AssertMatrixMatch(Matrix4x4.TRS(pos, rot, scale), Mat4x4.TRS(vPos, vRot, vScale), "Creación TRS (Traslación, Rotación, Escala)");
    }

    static bool TestTransformations(Vector3 pos, Quaternion rot, Vector3 scale, Vec3 vPos, Quat vRot, Vec3 vScale)
    {
        bool result = true;
        result &= AssertMatrixMatch(Matrix4x4.Translate(pos), Mat4x4.Translate(vPos), "Matriz de Traslación (Translate)");
        result &= AssertMatrixMatch(Matrix4x4.Rotate(rot), Mat4x4.Rotate(vRot), "Matriz de Rotación (Rotate)");
        result &= AssertMatrixMatch(Matrix4x4.Scale(scale), Mat4x4.Scale(vScale), "Matriz de Escala (Scale)");
        return result;
    }


    private static bool AssertMatrixMatch(Matrix4x4 expected, Mat4x4 actual, string testName)
    {
        bool passed = true;
        float[] expVals = { expected.m00, expected.m01, expected.m02, expected.m03,
                            expected.m10, expected.m11, expected.m12, expected.m13,
                            expected.m20, expected.m21, expected.m22, expected.m23,
                            expected.m30, expected.m31, expected.m32, expected.m33 };

        float[] actVals = { actual.m00, actual.m01, actual.m02, actual.m03,
                            actual.m10, actual.m11, actual.m12, actual.m13,
                            actual.m20, actual.m21, actual.m22, actual.m23,
                            actual.m30, actual.m31, actual.m32, actual.m33 };

        for (int i = 0; i < 16; i++)
        {
            if (Mathf.Abs(expVals[i] - actVals[i]) > tolerance)
            {
                passed = false;
                break;
            }
        }

        if (!passed)
        {
            Debug.LogError($"[FAILED] {testName}: Las matrices difieren drásticamente.\nEsp: \n{expected}\nObt: \n{actual}");
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