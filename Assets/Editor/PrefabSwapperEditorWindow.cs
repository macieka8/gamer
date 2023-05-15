using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

public class PrefabSwapperEditorWindow : EditorWindow
{
    [SerializeField] GameObject _prefab;

    [MenuItem("Window/PrefabSwapper")]
    static void Init()
    {
        var window = (PrefabSwapperEditorWindow)GetWindow(typeof(PrefabSwapperEditorWindow));
        window.Show();
    }

    public void CreateGUI()
    {
        VisualElement root = rootVisualElement;

        Label label = new Label("Swap all selected gameObjects with choosen prefab");
        root.Add(label);

        var prefabField = new ObjectField("Prefab");
        prefabField.name = "prefab";
        prefabField.objectType = typeof(GameObject);
        var prop = new SerializedObject(this).FindProperty("_prefab");
        prefabField.BindProperty(prop);
        root.Add(prefabField);

        Button button = new Button();
        button.clicked += Swap;
        button.name = "button";
        button.text = "Swap";
        root.Add(button);
    }

    void Swap()
    {
        if (_prefab == null)
        {
            Debug.Log($"Prefab is not valid (Current value:{_prefab})");
            return;
        }
        var selectedTransforms = Selection.transforms;
        if (selectedTransforms.Length == 0)
        {
            Debug.Log("No GameObject is selected");
            return;
        }

        for (int i = 0; i < selectedTransforms.Length; i++)
        {
            var transformToSwap = selectedTransforms[i];

            var position = transformToSwap.position;
            var rotation = transformToSwap.rotation;
            var newGameObject = PrefabUtility.InstantiatePrefab(_prefab) as GameObject;
            newGameObject.transform.SetPositionAndRotation(position, rotation);
            Undo.RegisterCreatedObjectUndo(newGameObject, $"Swap selected GameObjects with Prefab: {_prefab.name}");

            if (transformToSwap.transform.parent != null)
            {
                newGameObject.transform.SetParent(transformToSwap.parent);
            }

            Undo.DestroyObjectImmediate(transformToSwap.gameObject);
        }
    }
}