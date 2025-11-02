using UnityEngine;
using UnityEngine.AI;
using UnityEditor;
using UnityEditor.EditorTools;

[EditorTool("Click To Go (NavMesh Agent)", typeof(NavMeshAgent))]
public class ClickToGoTool : EditorTool
{
    GUIContent icon;

    void OnEnable()
    {
        icon = new GUIContent()
        {
            image = EditorGUIUtility.IconContent("d_MoveTool On").image,
            text = "Click To Go",
            tooltip = "Shift + Click para mover NavMeshAgent"
        };
    }

    public override GUIContent toolbarIcon => icon;

    public override void OnToolGUI(EditorWindow window)
    {
        // Asegura que esta en la SceneView
        if (!(window is SceneView sceneView))
            return;

        // Asegura que el objeto seleccionado tenga NavMeshAgent
        if (!(target is NavMeshAgent agent) || agent == null)
            return;

        Event e = Event.current;
        
        // Obtener control ID para la tool
        int controlID = GUIUtility.GetControlID(FocusType.Passive);

        switch (e.GetTypeForControl(controlID))
        {
            case EventType.Layout:
                HandleUtility.AddDefaultControl(controlID);
                break;

            case EventType.MouseDown:
                if (e.button == 0 && e.shift)
                {
                    Ray ray = HandleUtility.GUIPointToWorldRay(e.mousePosition);

                    if (Physics.Raycast(ray, out RaycastHit hit))
                    {
                        if (NavMesh.SamplePosition(hit.point, out NavMeshHit navHit, 10f, NavMesh.AllAreas))
                        {
                            Undo.RecordObject(agent.transform, "Move NavMeshAgent");

                            // en play camina, si no, warpea
                            if (Application.isPlaying)
                            {
                                agent.SetDestination(navHit.position);
                            }
                            else
                            {
                                agent.Warp(navHit.position);
                                EditorUtility.SetDirty(agent);
                            }

                            Debug.Log($"[ClickToGo] {agent.name} → {navHit.position}");
                            
                            // Consumir el evento y evitar que Unity lo use para otra cosa
                            GUIUtility.hotControl = controlID;
                            e.Use();
                            
                            sceneView.Repaint();
                        }
                    }
                }
                break;

            case EventType.MouseUp:
                // Liberar control 
                if (GUIUtility.hotControl == controlID)
                {
                    GUIUtility.hotControl = 0;
                    e.Use();
                }
                break;

            case EventType.Repaint:
                // Es solo para el feedback visual
                Handles.color = Color.cyan;
                Handles.DrawWireDisc(agent.transform.position, Vector3.up, 0.3f);
                Handles.ArrowHandleCap(0, agent.transform.position, agent.transform.rotation, 0.5f, EventType.Repaint);
                break;
        }
    }
}