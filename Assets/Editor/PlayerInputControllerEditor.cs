using UnityEngine;
using UnityEditor;
using UnityEngine.InputSystem;
using gamer;

[CustomEditor(typeof(PlayerInputController))]
public class PlayerInputControllerEditor : Editor
{
    readonly GUIContent _defaultActionMapText =
        EditorGUIUtility.TrTextContent("Default Map", "Action map to enable by default. If not set, no actions will be enabled by default.");

    readonly GUIContent _globalActionMapText =
        EditorGUIUtility.TrTextContent("Global Map", "Action map that is always enabled. If not set, no actions is global.");

    SerializedProperty _actionsProperty;
    SerializedProperty _defaultActionMapProperty;
    SerializedProperty _globalActionMapProperty;

    int _selectedDefaultActionMap;
    int _selectedGlobalActionMap;
    bool _actionAssetInitialized;
    GUIContent[] _actionMapOptions;

    public void OnEnable()
    {
        _actionsProperty = serializedObject.FindProperty("_actions");
        _defaultActionMapProperty = serializedObject.FindProperty("_defaultActionMap");
        _globalActionMapProperty = serializedObject.FindProperty("_globalActionMap");
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

            ActionMapField(_defaultActionMapProperty, ref _selectedDefaultActionMap, _defaultActionMapText);
            ActionMapField(_globalActionMapProperty, ref _selectedGlobalActionMap, _globalActionMapText);

            if (overallChangeScope.changed)
                serializedObject.ApplyModifiedProperties();
        }
    }

    void OnActionAssetChange()
    {
        serializedObject.ApplyModifiedProperties();
        _actionAssetInitialized = true;

        var playerInputController = (PlayerInputController)target;
        var asset = (InputActionAsset)_actionsProperty.objectReferenceValue;
        if (asset == null)
        {
            _actionMapOptions = null;
            _selectedDefaultActionMap = -1;
            _selectedGlobalActionMap = -1;
            return;
        }

        playerInputController.DefaultActionMap =
            OnChangeActionMap(asset, playerInputController.DefaultActionMap, ref _selectedDefaultActionMap);
        playerInputController.GlobalActionMap =
        OnChangeActionMap(asset, playerInputController.GlobalActionMap, ref _selectedGlobalActionMap);

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
