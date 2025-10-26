using System.Collections.Generic;
using System.Linq;
using Minge2025Summer.Scripts.ScriptableObject;
using UnityEditor;
using UnityEditor.VersionControl;
using UnityEngine;

namespace EditorUtility
{
    public class CullingObjectGroupEditor : EditorWindow
    {
        private List<CullingObjectGroup> groups = new List<CullingObjectGroup>();
        private CullingObjectGroup selectedGroup;
        private Vector2 scrollPosition;

        [MenuItem("Tools/Culling Object Group Editor")]
        public static void ShowWindow()
        {
            GetWindow<CullingObjectGroupEditor>("Culling Object Groups");
        }

        /// <summary>
        /// ウィンドウが有効になったときにグループを読み込む
        /// </summary>
        private void OnEnable()
        {
            LoadGroups();
        }

        /// <summary>
        /// ウィンドウのUIを描画する
        /// </summary>
        private void OnGUI()
        {
            EditorGUILayout.LabelField("Culling Object Group Editor", EditorStyles.boldLabel);
            EditorGUILayout.Space();
            
            // グループ作成ボタン
            if (GUILayout.Button("Create New Group"))
            {
                CreateNewGroup();
            }
            
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Groups", EditorStyles.boldLabel);
            
            // グループ一覧表示
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
            for (int i = 0; i < groups.Count; i++)
            {
                if (groups[i] == null)
                    continue;

                EditorGUILayout.BeginHorizontal();
                
                // グループが選択されている場合は色を変える
                GUI.color = (groups[i] == selectedGroup) ? Color.cyan : Color.white;
                if (GUILayout.Button(groups[i].name, GUILayout.Height(25)))
                    selectedGroup = groups[i];
                GUI.color = Color.white;
                
                // グループ削除ボタン
                GUI.backgroundColor = new Color(1.0f, 0.6f, 0.6f);
                if (GUILayout.Button("X", GUILayout.Width(30), GUILayout.Height(25)))
                {
                    if (UnityEditor.EditorUtility.DisplayDialog("Delete Group", $"Are you sure you want to delete the group '{groups[i].name}'?", "Delete", "Cancel"))
                    {
                        DeleteGroup(groups[i]);
                        if (selectedGroup == groups[i])
                            selectedGroup = null;
                        return;
                    }
                }

                GUI.backgroundColor = Color.white;
                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.EndScrollView();

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
            
            // 選択中のグループの操作UI
            if (selectedGroup != null)
                DrawSelectedGroupUI();
            else
                EditorGUILayout.LabelField("Select or create a group to start.", EditorStyles.centeredGreyMiniLabel);
        }

        /// <summary>
        /// 選択されたグループのUIを描画する
        /// </summary>
        private void DrawSelectedGroupUI()
        {
            string newName = EditorGUILayout.TextField("Group Name", selectedGroup.name);
            if (newName != selectedGroup.name)
            {
                string assetPath = AssetDatabase.GetAssetPath(selectedGroup);
                AssetDatabase.RenameAsset(assetPath, newName);
                UnityEditor.EditorUtility.SetDirty(selectedGroup);
            }
            
            EditorGUILayout.Space();
            
            // オブジェクト操作ボタン
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Add Selected Objects"))
                AddSelectedObjectsToGroup();
            
            if (GUILayout.Button("Clear All Objects"))
                if (UnityEditor.EditorUtility.DisplayDialog("Clear Objects", $"Are you sure you want to remove all objects from '{selectedGroup.name}'?", "Clear", "Cancel"))
                    ClearObjectsFromGroup();

            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.Space();
            
            // 表示・非表示切り替えボタン
            EditorGUILayout.LabelField("Toggle Visibility", EditorStyles.boldLabel);
            EditorGUILayout.BeginHorizontal();
            GUI.backgroundColor = new Color(0.6f, 1.0f, 0.6f);
            if (GUILayout.Button("Show All Objects"))
                ToggleObjectsVisibility(true);

            GUI.backgroundColor = new Color(1.0f, 0.8f, 0.6f);
            if (GUILayout.Button("Hide All"))
                ToggleObjectsVisibility(false);
            
            GUI.backgroundColor = Color.white;
            if (GUILayout.Button("Invert"))
                InvertObjectsVisibility();
            
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();
            
            // グループ内のオブジェクト一覧
            EditorGUILayout.LabelField("Objects in Group:", EditorStyles.boldLabel);
            
            // nullオブジェクトをリストの最後に移動させる
            selectedGroup.CullingObjects = selectedGroup.CullingObjects
                .OrderBy(order => order == null)
                .ToList();

            for (int i = 0; i < selectedGroup.CullingObjects.Count; i++)
            {
                EditorGUILayout.BeginHorizontal();
                GameObject obj = selectedGroup.CullingObjects[i];
                
                if (obj == null)
                    EditorGUILayout.LabelField("Missing Object", EditorStyles.centeredGreyMiniLabel);
                else
                    EditorGUILayout.ObjectField(obj,typeof(GameObject), true);

                GUI.backgroundColor = new Color(1.0f, 0.6f, 0.6f);
                if (GUILayout.Button("Remove", GUILayout.Width(80)))
                {
                    RemoveObjectFromGroup(i);
                    return;
                }

                GUI.backgroundColor = Color.white;
                
                EditorGUILayout.EndHorizontal();
            }
        }

        /// <summary>
        /// プロジェクトからObjectGroupアセットをすべて読み込む
        /// </summary>
        private void LoadGroups()
        {
            groups.Clear();
            string[] guids = AssetDatabase.FindAssets("t:CullingObjectGroup");
            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                CullingObjectGroup group = AssetDatabase.LoadAssetAtPath<CullingObjectGroup>(path);
                if (group != null)
                    groups.Add(group);
            }
        }

        /// <summary>
        /// 新しいグループを作成する
        /// </summary>
        private void CreateNewGroup()
        {
            CullingObjectGroup newGroup = CreateInstance<CullingObjectGroup>();
            
            // ユニークなファイルパスを生成する
            string path = AssetDatabase.GenerateUniqueAssetPath("Assets/scripts/EditorAsset/CullingObjectGroup.asset");
            
            AssetDatabase.CreateAsset(newGroup, path);
            AssetDatabase.SaveAssets();
            
            groups.Add(newGroup);
            selectedGroup = newGroup;
        }
        
        /// <summary>
        /// グループを削除する
        /// </summary>
        /// <param name="group">削除するグループ</param>
        private void DeleteGroup(CullingObjectGroup group)
        {
            string path = AssetDatabase.GetAssetPath(group);
            AssetDatabase.DeleteAsset(path);
            groups.Remove(group);
        }

        /// <summary>
        /// 選択中のGameObjectを選択中のグループに追加する
        /// </summary>
        private void AddSelectedObjectsToGroup()
        {
            if (selectedGroup == null)
                return;

            GameObject[] selectedObjects = Selection.gameObjects;
            
            // 重複しないように追加する
            foreach (GameObject obj in selectedObjects)
            {
                if (selectedGroup.CullingObjects.Contains(obj))
                    selectedGroup.CullingObjects.Add(obj);
                
                UnityEditor.EditorUtility.SetDirty(selectedGroup);
            }
        }

        /// <summary>
        /// グループからオブジェクトを削除する
        /// </summary>
        /// <param name="index">削除するオブジェクトのインデックス</param>
        private void RemoveObjectFromGroup(int index)
        {
            if (selectedGroup == null)
                return;
            
            selectedGroup.CullingObjects.RemoveAt(index);
            UnityEditor.EditorUtility.SetDirty(selectedGroup);
        }

        /// <summary>
        /// グループのオブジェクトのすべてをクリアする
        /// </summary>
        private void ClearObjectsFromGroup()
        {
            if (selectedGroup == null)
                return;

            selectedGroup.CullingObjects.Clear();
            UnityEditor.EditorUtility.SetDirty(selectedGroup);
        }

        /// <summary>
        /// グループ内のオブジェクトの表示・非表示を切り替える
        /// </summary>
        /// <param name="visible">表示させるかどうか</param>
        private void ToggleObjectsVisibility(bool visible)
        {
            if (selectedGroup == null)
                return;

            // Undo操作を記録する
            Undo.RecordObjects(selectedGroup.CullingObjects
                .Where(order => order != null)
                .ToArray(), "Toggle Object Visibility");

            foreach (GameObject obj in selectedGroup.CullingObjects)
            {
                if (obj != null)
                    obj.SetActive(visible);
            }
        }

        /// <summary>
        /// グループ内のオブジェクトの表示・非表示を反転する
        /// </summary>
        private void InvertObjectsVisibility()
        {
            if (selectedGroup == null)
                return;
            
            Undo.RecordObjects(selectedGroup.CullingObjects
                .Where(order => order != null)
                .ToArray(), "Invert Object Visibility");

            foreach (GameObject obj in selectedGroup.CullingObjects)
            {
                if (obj != null)
                    obj.SetActive(!obj.activeSelf);
            }
        }
    }
}