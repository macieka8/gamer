using UnityEngine;
using UnityEditor;
using UnityEngine.InputSystem;
using gamer;

[CustomEditor(typeof(PlayerInputController))]
public class PlayerInputControllerEditor : Editor
{
    private readonly GUIContent m_DefaultActionMapText =
        EditorGUIUtility.TrTextContent("Default Map", "Action map to enable by default. If not set, no actions will be enabled by default.");

    SerializedProperty _actionsProperty;
    SerializedProperty _defaultActionMapProperty;

    int _selectedDefaultActionMap;
    bool _actionAssetInitialized;
    GUIContent[] _actionMapOptions;

    public void OnEnable()
    {
        _actionsProperty = serializedObject.FindProperty("_actions");
        _defaultActionMapProperty = serializedObject.FindProperty("_defaultActionMap");
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

            if (_actionMapOptions?.Length > 0)
            {
                var selected = EditorGUILayout.Popup(m_DefaultActionMapText, _selectedDefaultActionMap,
                    _actionMapOptions);
                if (selected != _selectedDefaultActionMap)
                {
                    if (selected == 0)
                    {
                        _defaultActionMapProperty.stringValue = null;
                    }
                    else
                    {
                        var asset = (InputActionAsset)_actionsProperty.objectReferenceValue;
                        var actionMap = asset.FindActionMap(_actionMapOptions[selected].text);
                        if (actionMap != null)
                            _defaultActionMapProperty.stringValue = actionMap.id.ToString();
                    }
                    _selectedDefaultActionMap = selected;
                }
            }

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
            return;
        }

        var selectedDefaultActionMap = !string.IsNullOrEmpty(playerInputController.DefaultActionMap)
                ? asset.FindActionMap(playerInputController.DefaultActionMap)
                : null;

        var actionMaps = asset.actionMaps;

        _actionMapOptions = new GUIContent[actionMaps.Count + 1];
        _actionMapOptions[0] = new GUIContent(EditorGUIUtility.TrTextContent("<None>"));

        for (var i = 0; i < actionMaps.Count; ++i)
        {
            var actionMap = actionMaps[i];
            _actionMapOptions[i + 1] = new GUIContent(actionMap.name);

            if (selectedDefaultActionMap != null && actionMap == selectedDefaultActionMap)
                _selectedDefaultActionMap = i + 1;
        }
        if (_selectedDefaultActionMap <= 0)
            playerInputController.DefaultActionMap = null;
        else
            playerInputController.DefaultActionMap = _actionMapOptions[_selectedDefaultActionMap].text;

        serializedObject.Update();
    }
}
