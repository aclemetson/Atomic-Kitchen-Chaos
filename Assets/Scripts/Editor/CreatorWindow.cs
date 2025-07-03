using AtomicKitchenChaos.Data;
using AtomicKitchenChaos.Utility;
using System;
using UnityEditor;
using UnityEngine;

namespace AtomicKitchenChaos.Editor
{
    public abstract class CreatorWindow<T> : EditorWindow
    {
        protected static Func<CreatorWindow<T>> getWindowCallback;

        protected Vector2 scrollPos;
        protected int tileHeight = 100;


        public static void ShowFullscreenWindow(string windowName) {
            var window = getWindowCallback();
            window.titleContent = new GUIContent("Level Creator");

            Rect mainDisplay = UnityEditorInternal.InternalEditorUtility.GetBoundsOfDesktopAtPoint(Vector2.zero);
            float margin = 40f;

            window.position = new Rect(
                mainDisplay.x + margin,
                mainDisplay.y + margin,
                mainDisplay.width - 2 * margin,
                mainDisplay.height - 4 * margin
            );

            window.Show();
        }

        public static void EditExistingObject(string path, string windowName) {
            LoadObjectFromDisk(path, windowName);
        }

        protected void SaveObjectFile(string path, string filename, T objData) {
                        
            string fullPath = Utilities.GetDataPath(path, filename + ".lz4");

            if (DataHandler.TrySaveToFile(objData, fullPath))
                Close();
        }

        protected static void LoadObjectFromDisk(string path, string windowName) {
            string fullPath = EditorUtility.OpenFilePanel("Select Level File", Utilities.GetDataPath(path), "lz4");

            if (string.IsNullOrEmpty(fullPath)) return;

            try {
                DataHandler.TryLoadFromFile(fullPath, out T objData);
                CreatorWindow<T> window = getWindowCallback();
                window.titleContent = new GUIContent(windowName);
                window.PopulateEditorData(objData);
            } catch (Exception ex) {
                Debug.LogError($"Failed to load and parse level file:\n{ex}");
            }
        }

        protected abstract void SaveObject();

        protected abstract void PopulateEditorData(T objData);
    }
}
