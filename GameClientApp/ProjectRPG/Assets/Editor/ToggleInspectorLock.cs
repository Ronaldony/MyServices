using System.Reflection;
using UnityEditor;
using UnityEngine;

public static class ToggleInspectorLock
{
    // 단축키: Ctrl/Cmd + L (메뉴 경로: Window > Toggle Inspector Lock)
    [MenuItem("Window/Toggle Inspector Lock %l")]
    public static void ToggleLock()
    {
        // InspectorWindow 타입을 가져옵니다.
        var inspectorType = typeof(Editor).Assembly.GetType("UnityEditor.InspectorWindow");
        if (inspectorType == null)
        {
            Debug.LogError("InspectorWindow 타입을 찾을 수 없습니다.");
            return;
        }

        // 현재 열려있는 InspectorWindow 인스턴스들을 찾습니다.
        Object[] inspectors = Resources.FindObjectsOfTypeAll(inspectorType);
        if (inspectors.Length == 0)
        {
            Debug.LogWarning("열려있는 Inspector 창이 없습니다.");
            return;
        }

        // 각 Inspector 창에 대해 잠금 상태를 토글합니다.
        foreach (var inspector in inspectors)
        {
            // isLocked 속성은 public 또는 internal 속성이므로 BindingFlags를 사용해 접근합니다.
            PropertyInfo isLockedProp = inspectorType.GetProperty("isLocked", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            if (isLockedProp != null)
            {
                // 현재 잠금 상태를 읽어옵니다.
                bool currentLock = (bool)isLockedProp.GetValue(inspector, null);
                // 잠금 상태를 반전시킵니다.
                isLockedProp.SetValue(inspector, !currentLock, null);
                // UI 업데이트를 위해 Repaint를 호출합니다.
                (inspector as EditorWindow)?.Repaint();
            }
            else
            {
                Debug.LogError("isLocked 속성을 찾을 수 없습니다.");
            }
        }
    }
}