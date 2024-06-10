using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using static UnityEngine.GraphicsBuffer;

[CustomEditor(typeof(CameraManager))]
public class CameraManagerEditor : Editor
{
    private bool changeFoldout = false;

    private bool changePositionMode = false;

    CameraClass selection;

    public override void OnInspectorGUI()
    {
        CameraManager cameraManager = target as CameraManager;

        cameraManager.mapManager = EditorGUILayout.ObjectField("Map Manager", cameraManager.mapManager, typeof(MapManager), true) as MapManager;

        GUILayout.BeginHorizontal();

        if (GUILayout.Button("Create Camera"))
        {
            cameraManager.CreateCamera(cameraManager.mapManager.currentPhase - 1);
        }

        if (GUILayout.Button("Remove Camera"))
        {
            cameraManager.RemoveCamera(cameraManager.mapManager.currentPhase - 1);
        }

        GUILayout.EndHorizontal();

        GUILayout.BeginVertical();
        {
            changeFoldout = EditorGUILayout.Foldout(changeFoldout, "Camera List");

            EditorGUILayout.Space(1);

            if (changeFoldout)
            {
                GUILayout.BeginVertical(GUI.skin.GetStyle("HelpBox"));

                if (cameraManager.cameraList.Count > 0)
                {
                    List<CameraClass> currentPhaseCameraList = cameraManager.cameraList[cameraManager.mapManager.currentPhase - 1];

                    for (int i = 0; i < currentPhaseCameraList.Count; i++)
                    {
                        CameraClass cameraClass = currentPhaseCameraList[i];

                        cameraClass.foldOut = EditorGUILayout.Foldout(cameraClass.foldOut, "Camera " + i);

                        if (cameraClass.foldOut)
                        {
                            GUILayout.BeginVertical(GUI.skin.GetStyle("HelpBox"));

                            cameraClass.camera = EditorGUILayout.ObjectField(currentPhaseCameraList[i].camera, typeof(Camera), false) as Camera;

                            EditorGUILayout.Space(1);

                            cameraClass.position = EditorGUILayout.Vector2Field("Position", currentPhaseCameraList[i].camera.transform.position);

                            cameraClass.cameraSize = EditorGUILayout.Slider("Size", cameraClass.cameraSize, 1, 5);
                            cameraClass.ChangeSize();

                            cameraClass.targetDisplay = EditorGUILayout.IntField("Target Display", cameraClass.targetDisplay);
                            cameraClass.ChangeTargetDisplay();

                            if (GUILayout.Button("Change Position"))
                            {
                                changePositionMode = true;
                                selection = cameraClass;
                            }

                            GUILayout.EndVertical();
                        }
                    }
                }
                GUILayout.EndVertical();
            }
        }
        GUILayout.EndVertical();

    }

    private void OnSceneGUI()
    {
        if (changePositionMode && selection != null)
        {
            if (Event.current.type == EventType.MouseDown)
            {
                if (Event.current.button == 1)
                {
                    Ray worldRay = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);

                    selection.position = worldRay.GetPoint(0);
                    selection.ChangePosition();
                    changePositionMode = false;

                }
            }
        }

    }

}
