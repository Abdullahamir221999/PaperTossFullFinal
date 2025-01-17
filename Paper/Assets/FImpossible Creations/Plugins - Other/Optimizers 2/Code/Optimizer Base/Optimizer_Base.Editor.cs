﻿using System.Collections.Generic;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace FIMSpace.FOptimizing
{
    public abstract partial class Optimizer_Base
    {

        public static bool _editor_DragAndDropOptim = false;
        public bool _editor_DrawSetup = true;
        public bool _editor_DrawOptimizeList = true;
        public bool _editor_DrawAddFeatures = false;
        public bool _editor_DrawLODLevelsSetup = true;
        public bool _editor_DrawExtra = false;

        public bool _editor_horizontal = true;
        public enum EFOptEditorCategory { Setup, List, Features, LODs }
        public EFOptEditorCategory _editor_category = EFOptEditorCategory.Setup;

        #region Culling Components Methods


        /// <summary>
        /// Adding unassigned components which can be optimized to 'ToOptimize' list
        /// </summary>
        public virtual void AssignComponentsToOptimizeFrom(Component target, bool includeAdvanced = false)
        { }


        /// <summary>
        /// Checking if there is no other optimizer using this components for optimization
        /// </summary>
        public bool CheckIfAlreadyInUse(LODsControllerBase generatedController, List<Optimizer_Base> childOptims)
        {
            bool alreadyInUse = false;

            if (childOptims != null)
            {
                for (int i = 0; i < childOptims.Count; i++)
                {
                    if (alreadyInUse) break;

                    if (childOptims[i] != this)
                    {
                        ScriptableOptimizer sOpt = childOptims[i] as ScriptableOptimizer;

                        if (sOpt != null)
                        {
                            for (int c = 0; c < sOpt.ToOptimize.Count; c++)
                                if (sOpt.ToOptimize[c].Component == generatedController.Component)
                                {
                                    alreadyInUse = true;
                                    break;
                                }
                        }
                        else
                        {
                            EssentialOptimizer eOpt = childOptims[i] as EssentialOptimizer;
                            if (eOpt != null)
                            {
                                for (int c = 0; c < eOpt.ToOptimize.Count; c++)
                                    if (eOpt.ToOptimize[c].Component == generatedController.Component)
                                    {
                                        alreadyInUse = true;
                                        break;
                                    }
                            }
                        }
                    }
                }
            }

            return alreadyInUse;
        }


        public virtual void AssignCustomComponentToOptimize(MonoBehaviour target) { }
        


        /// <summary>
        /// Searching through whole 'target' for components to optimize and adding them to 'ToOptimize' list if new are found
        /// </summary>
        public virtual void AssignComponentsToBeOptimizedFromAllChildren(GameObject target, bool searchForCustom = false)
        {
            RefreshToOptimizeList();

            if (!searchForCustom)
            {
                foreach (var c in target.GetComponentsInChildren<Transform>(OptimizersSetup.Settings.SearchForComponentsInDeactivatedObjects)) AssignComponentsToOptimizeFrom(c);
            }
            else
            {
                foreach (var c in target.GetComponentsInChildren<Transform>(OptimizersSetup.Settings.SearchForComponentsInDeactivatedObjects))
                    foreach (var m in c.gameObject.GetComponents<MonoBehaviour>()) AssignCustomComponentToOptimize(m);
            }

            TryAutoComputeDetectionShape();
        }


        /// <summary>
        /// Checking if component is already in 'ToOptimize' list
        /// </summary>
        public abstract bool ContainsComponent(Component component);


        /// <summary>
        /// Removing null references
        /// </summary>
        public abstract void RefreshToOptimizeList();

        #endregion


        #region Project Assets Related Methods


        public bool IsPrefabed()
        {
            #region Unity Version Conditional

#if UNITY_EDITOR

#if UNITY_2018_3_OR_NEWER
            if (UnityEditor.PrefabUtility.IsPartOfPrefabInstance(this)) return true;
#else
            if (UnityEditor.PrefabUtility.GetPrefabParent(this)) return true;
#endif

            return false;
#else
            return false;
#endif

            #endregion
        }


        /// <summary>
        /// Refreshing reference variables on nearest LOD level for every optimized object
        /// </summary>
        protected virtual void RefreshInitialSettingsForOptimized() { }


        public virtual void RemoveFromToOptimizeAt(int i) { }


        public virtual void RemoveAllComponentsFromToOptimize() { }


        /// <summary>
        /// Adding and refreshing added component to optimize list
        /// </summary>
        protected abstract LODsControllerBase AddToOptimize(LODsControllerBase lod);


        /// <summary>
        /// Resetting LODs settings when LOD levels count changed
        /// </summary>
        protected abstract void ResetLODs(bool hard = false);


        protected virtual void OnActivationChange(bool active)
        {
            if (OptimizingMethod == EOptimizingMethod.TriggerBased)
            {
                if (!active)
                {
                    // Disconnecting triggers from deactivated object to be able of detecting whem camera will again catch max distance
                    if (triggersContainer.transform.parent != null) triggersContainer.transform.SetParent(null, true);
                }
                else
                    if (triggersContainer.transform.parent == null) triggersContainer.transform.SetParent(transform, true);
            }
        }


        #endregion


        #region Utilities

#if UNITY_EDITOR
        private bool wasSearching = false;
#endif

        public virtual void CheckForNullsToOptimize() { }


        protected virtual void OnDestroy()
        {
            DisposeDynamicOptimizer();
            CleanCullingGroup();
            if (!isQuitting) if (!OptimizersManager.AppIsQuitting) OptimizersManager.Instance.UnRegisterOptimizer(this);
        }

        private bool isQuitting = false;
        private void OnApplicationQuit() { isQuitting = true; CleanCullingGroup(); }

        /// <summary>
        /// When destroying component in prefab file then cleaning referenes stored inside
        /// </summary>
        public virtual void CleanAsset() { }

        public static List<T> FindComponentsInAllChildren<T>(Transform transformToSearchIn) where T : Component
        {
            List<T> components = new List<T>();
            foreach (Transform child in transformToSearchIn.GetComponentsInChildren<Transform>(true))
            {
                T component = child.GetComponent<T>();
                if (component) components.Add(component);
            }

            return components;
        }

        #endregion


        #region Editor Stuff

        /// <summary> Flag for editor usage, triggering SaveAssets() if sub assets was saved  during OnValidate() </summary>
        internal bool Editor_WasSaving = false;
        /// <summary> Flag for editor usage, tagging from enabling inspector window that object is inside isolated scene </summary>
        [HideInInspector]
        public bool Editor_InIsolatedScene = false;

        [HideInInspector]
        public bool Editor_JustCreated = true;

        /// <summary>
        /// Method called when component is added to any game object
        /// </summary>
        protected virtual void OptimizerReset() { }


        protected virtual void OnEnable()
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                wasSearching = false;
                OptimizerOnValidate();
            }
#endif

            // Game Time Feature (not just editor)
            if (wasDisabled)
            {
                ApplyLastEvent();
                wasDisabled = false;
            }
        }


        // Game Time Feature (not just editor)
        protected bool wasDisabled = false;

        /// <summary>
        /// Applying last LOD event
        /// </summary>
        private void ApplyLastEvent()
        {
            if (OptimizingMethod == EOptimizingMethod.Dynamic)
            {
                OutOfCameraView = false;
                DynamicLODUpdate((EOptimizingDistance)CurrentDynamicDistanceCategory, lastDynamicDistance);
            }
            else
            {
                if (OptimizingMethod == EOptimizingMethod.Effective) if (CurrentDynamicDistanceCategory != null) DynamicLODUpdate((EOptimizingDistance)CurrentDynamicDistanceCategory, lastDynamicDistance);

                CullingGroupStateChanged(lastEvent);
            }
        }

        internal bool WasAskingForStatic = false;

        /// <summary>
        /// Executed every when component added and every change inside inspector window
        /// </summary>
        public void OptimizerOnValidate()
        {
#if UNITY_EDITOR

            OnValidateStart();

            if (Application.isPlaying) return;

            if (AutoDistance) SetAutoDistance();

            OnValidateRefreshComponents();
            OnValidateUpdateToOptimize();

            OnValidateCheckForStatic();
#endif
        }

        protected void OnValidateStart()
        {
            if (LODLevels <= 0) LODLevels = 2;
            if (LODLevels > 8) LODLevels = 8;

            if (DetectionRadius < 0f) DetectionRadius = 0f;
        }

        protected virtual void OnValidateRefreshComponents() { }

        protected virtual void OnValidateUpdateToOptimize(bool hard = false) { }


        /// <summary>
        /// Setting auto MaxDistance value basing on detection shape size
        /// </summary>
        public void SetAutoDistance(float multiplier = 1f)
        {
            switch (OptimizingMethod)
            {
                case EOptimizingMethod.Static:
                case EOptimizingMethod.Effective:

                    MaxDistance = DetectionRadius * 550f;
                    MaxDistance *= GetScaler(transform);
                    MaxDistance *= multiplier;

                    if (OptimizersManager.MainCamera)
                        if (MaxDistance > OptimizersManager.MainCamera.farClipPlane) MaxDistance = OptimizersManager.MainCamera.farClipPlane;

                    //Debug.Log("scaler = " + GetScaler(transform) +"  Det Rad = " + DetectionRadius + " mul " + multiplier + " = " + MaxDistance);

                    break;

                case EOptimizingMethod.Dynamic:
                case EOptimizingMethod.TriggerBased:

                    MaxDistance = DetectionBounds.magnitude * 166f;

                    MaxDistance *= GetScaler(transform);

                    if (OptimizersManager.MainCamera)
                        if (MaxDistance > OptimizersManager.MainCamera.farClipPlane) MaxDistance = OptimizersManager.MainCamera.farClipPlane;

                    MaxDistance *= multiplier;

                    break;
            }

        }


        protected virtual void OnValidateCheckForStatic()
        {
#if UNITY_EDITOR

            if (Selection.gameObjects.Contains(gameObject))
            {
                if (gameObject.isStatic)
                {
                    if (FEditor_OneShotLog.CanDrawLog("OptimAskStatic"))
                    {
                        if (OptimizingMethod != EOptimizingMethod.Static)
                        {
                            if (PlayerPrefs.GetInt("Optim_SetStatic", 0) == 1)
                            {
                                OptimizingMethod = EOptimizingMethod.Static;
                            }
                            else
                            {
                                if (!WasAskingForStatic)
                                {
                                    if (UnityEditor.EditorUtility.DisplayDialog("Static Game Object Detected", "Your object is marked as static, can I change optimizing method to 'Static'?", "Yes", "No"))
                                    {
                                        OptimizingMethod = EOptimizingMethod.Static;

                                        if (PlayerPrefs.GetInt("Optim_SetStatic", 0) == 0 || FEditor_OneShotLog.CanDrawLog("Optim_SetStatic"))
                                        {
                                            FEditor_OneShotLog.CanDrawLog("Optim_SetStatic");

                                            if (UnityEditor.EditorUtility.DisplayDialog("Can it be done automatically?", "Can I enable 'Static' optimization method every time static game object is detected?", "Yes do it for me everytime", "No I want to do it by myself"))
                                                PlayerPrefs.SetInt("Optim_SetStatic", 1);
                                            else
                                                PlayerPrefs.SetInt("Optim_SetStatic", -1);
                                        }
                                    }

                                    WasAskingForStatic = true;
                                }

                            }
                        }
                    }
                }
                else // Not static game object
                {
                    if (OptimizingMethod == EOptimizingMethod.Static) // With static optimization method
                    {
                        if (PlayerPrefs.GetInt("Optim_SetStatic", 0) == 1) // If we have enabled auto static then we resume to effective
                        {
                            OptimizingMethod = EOptimizingMethod.Effective;
                        }
                        else // If we didn't set auto static setting then we do nothing
                        {
                        }
                    }
                }
            }

#endif
        }

        /// <summary>
        /// (Editor Usage) Syncing LOD Count with LOD Sets counts
        /// </summary>
        public virtual void SyncWithReferences() { }


        public virtual void EditorUpdate()
        {
            if (LODLevels <= 0) LODLevels = 2;
            if (LODLevels > 8) LODLevels = 8;
            EditorResetLODValues();
        }


        public void EditorResetLODValues(bool logWarning = false)
        {
            if (LODPercent == null)
            {
                LODPercent = new List<float>();
#if UNITY_EDITOR
                if (logWarning) { UnityEngine.Debug.Log("[Optimizers Warning] No LOD list in '" + name + "'. There went something wrong with initialization."); logWarning = false; }
#endif
            }

            if (LODLevels != LODPercent.Count)
            {
#if UNITY_EDITOR
                if (logWarning) UnityEngine.Debug.Log("[Optimizers Warning] No LOD list in '" + name + "'. There went something wrong with initialization.");
#endif

                float pow = Mathf.Lerp(1f, 1.65f, Mathf.InverseLerp(1, 7, LODLevels));

                LODPercent.Clear();
                for (int i = 0; i < LODLevels; i++)
                {
                    float percent = 0f;
                    percent = .05f + Mathf.Pow((float)(i + 1) / (float)(LODLevels + 1), pow);
                    LODPercent.Add(percent);
                }

                LODPercent[LODLevels - 1] = 1f;
            }
        }


        #endregion


        #region Color Utility


        /// <summary> Colors identifying certain LOD levels </summary>
        public static readonly Color[] lODColors =
        {
            new Color(0.2231376f, 0.8011768f, 0.1619608f, 1.0f),
            new Color(0.2070592f, 0.6333336f, 0.7556864f, 1.0f),
            new Color(0.1592160f, 0.5578432f, 0.3435296f, 1.0f),
            new Color(0.1333336f, 0.400000f, 0.7982352f, 1.0f),
            new Color(0.3827448f, 0.2886272f, 0.5239216f, 1.0f),
            new Color(0.8000000f, 0.4423528f, 0.0000000f, 1.0f),
            new Color(0.4886272f, 0.1078432f, 0.801960f, 1.0f),
            new Color(0.7749016f, 0.6368624f, 0.0250984f, 1.0f)
        };

        public static readonly Color culledLODColor = new Color(.4f, 0f, 0f, .5f);


        #endregion

    }
}
