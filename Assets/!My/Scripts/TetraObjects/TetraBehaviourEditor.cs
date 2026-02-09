using System;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

[CustomEditor(typeof(TetraBehaviour))]
public class TetraBehaviourEditor : Editor
{
    private ReorderableList actionsList;
    private ReorderableList actionsUpdateList;

    private void OnEnable()
    {
        actionsList = CreateList("tetraActions", "Tetra Actions");
        actionsUpdateList = CreateList("tetraActionsUpdate", "Tetra Actions Update");
    }

    private ReorderableList CreateList(string propertyName, string header)
    {
        var list = new ReorderableList(
            serializedObject,
            serializedObject.FindProperty(propertyName),
            true, true, true, true);

        list.drawHeaderCallback = rect =>
        {
            EditorGUI.LabelField(rect, header);
        };

        // ✅ ГЛАВНОЕ — кастомная высота
        list.elementHeightCallback = index =>
        {
            var element = list.serializedProperty.GetArrayElementAtIndex(index);
            if (!element.isExpanded)
                return EditorGUIUtility.singleLineHeight + 6f;

            return EditorGUI.GetPropertyHeight(element, true) + 6f;
        };

        // ✅ Кастомная отрисовка
        list.drawElementCallback = (rect, index, isActive, isFocused) =>
        {
            var element = list.serializedProperty.GetArrayElementAtIndex(index);

            rect.y += 2f;
            rect.height = EditorGUIUtility.singleLineHeight;

            // --- Заголовок ---
            var managed = element.managedReferenceValue as TetraAction;
            string title = managed != null
                ? $"{managed.GetType().Name} {managed.ID}"
                : "<null>";

            element.isExpanded = EditorGUI.Foldout(
                rect,
                element.isExpanded,
                title,
                true);

            // --- Тело ---
            if (!element.isExpanded)
                return;

            rect.y += EditorGUIUtility.singleLineHeight + 2f;
            rect.height = EditorGUI.GetPropertyHeight(element, true);

            EditorGUI.indentLevel++;
            EditorGUI.PropertyField(rect, element, GUIContent.none, true);
            EditorGUI.indentLevel--;
        };

        list.onAddDropdownCallback = (rect, l) =>
        {
            ShowAddMenu(propertyName);
        };

        return list;
    }

    private void ShowAddMenu(string propertyName)
    {
        var menu = new GenericMenu();
        var types = GetAllTetraActionTypes();

        foreach (var type in types)
        {
            menu.AddItem(new GUIContent(type.Name), false, () =>
            {
                var instance = Activator.CreateInstance(type) as TetraAction;

                var prop = serializedObject.FindProperty(propertyName);
                prop.arraySize++;
                prop.GetArrayElementAtIndex(prop.arraySize - 1)
                    .managedReferenceValue = instance;

                serializedObject.ApplyModifiedProperties();
            });
        }

        menu.ShowAsContext();
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        // 🔹 Рисуем все обычные поля TetraBehaviour
        EditorGUILayout.Space();
        DrawPropertiesExcluding(
            serializedObject,
            "m_Script",
            "tetraActions",
            "tetraActionsUpdate"
        );

        EditorGUILayout.Space(8);

        // 🔹 Рисуем кастомные списки
        actionsList.DoLayoutList();
        actionsUpdateList.DoLayoutList();

        serializedObject.ApplyModifiedProperties();
    }

    private Type[] GetAllTetraActionTypes()
    {
        var baseType = typeof(TetraAction);
        var list = new System.Collections.Generic.List<Type>();

        foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            foreach (var type in assembly.GetTypes())
            {
                if (type.IsSubclassOf(baseType) && !type.IsAbstract)
                    list.Add(type);
            }

        return list.ToArray();
    }
}