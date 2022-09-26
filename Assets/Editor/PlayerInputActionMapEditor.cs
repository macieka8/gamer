using UnityEngine;
using UnityEngine.InputSystem;
using UnityEditor;
using gamer;

[CustomEditor(typeof(InputActionMapReference))]
public class InputActionMapReferenceEditor : Editor
{
    readonly GUIContent _defaultActionMapText =
        EditorGUIUtility.TrTextContent("Action Map", "Action map");

    SerializedProperty _actionsProperty;
    SerializedProperty _actionMapProperty;

    int _selectedActionMap;
    bool _actionAssetInitialized;
    GUIContent[] _actionMapOptions;

    public void OnEnable()
    {
        _actionsProperty = serializedObject.FindProperty("_actions");
        _actionMapProperty = serializedObject.FindProperty("_actionMap");
    }

    public override void OnInspectorGUI()
    {
        using (var overallChangeScope = new EditorGUI.ChangeCheckScope())
        {
            using (var actionsChangeScope = new EditorGUI.ChangeCheckScope())
            {
                EditorGUILayout.PropertyField(_actionsProperty);
                if (actionsChangeScope.changed || !_actionAssetInitialized)
                {
                    OnActionAssetChange();
                }
            }

            ActionMapField(_actionMapProperty, ref _selectedActionMap, _defaultActionMapText);

            if (overallChangeScope.changed)
                serializedObject.ApplyModifiedProperties();
        }
    }

    void OnActionAssetChange()
    {
        serializedObject.ApplyModifiedProperties();
        _actionAssetInitialized = true;

        var inputActionMapReference = (InputActionMapReference)target;
        var asset = (InputActionAsset)_actionsProperty.objectReferenceValue;
        if (asset == null)
        {
            _actionMapOptions = null;
            _selectedActionMap = -1;
            return;
        }

        inputActionMapReference.ActionMap =
            OnChangeActionMap(asset, inputActionMapReference.ActionMap, ref _selectedActionMap);

        serializedObject.Update();
    }

    void ActionMapField(SerializedProperty actionMapProperty, ref int selectedActionMap, GUIContent actionMapText)
    {
        if (_actionMapOptions?.Length > 0)
        {
            var selected = EditorGUILayout.Popup(actionMapText, selectedActionMap,
                _actionMapOptions);
            if (selected != selectedActionMap)
            {
                if (selected == 0)
                {
                    actionMapProperty.stringValue = null;
                }
                else
                {
                    var asset = (InputActionAsset)_actionsProperty.objectReferenceValue;
                    var actionMap = asset.FindActionMap(_actionMapOptions[selected].text);
                    if (actionMap != null)
                        actionMapProperty.stringValue = actionMap.id.ToString();
                }
                selectedActionMap = selected;
            }
        }
    }

    string OnChangeActionMap(InputActionAsset asset, string actionMapToSelect, ref int selectedActionMapIndex)
    {
        var selectedActionMap = !string.IsNullOrEmpty(actionMapToSelect)
        ? asset.FindActionMap(actionMapToSelect)
        : null;

        var actionMaps = asset.actionMaps;

        _actionMapOptions = new GUIContent[actionMaps.Count + 1];
        _actionMapOptions[0] = new GUIContent(EditorGUIUtility.TrTextContent("<None>"));

        for (var i = 0; i < actionMaps.Count; ++i)
        {
            var actionMap = actionMaps[i];
            _actionMapOptions[i + 1] = new GUIContent(actionMap.name);

            if (selectedActionMap != null && actionMap == selectedActionMap)
                selectedActionMapIndex = i + 1;
        }
        if (selectedActionMapIndex <= 0)
            return null;
        else
            return _actionMapOptions[selectedActionMapIndex].text;
    }
}