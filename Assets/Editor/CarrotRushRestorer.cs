// Carrot Rush scene/style recovery. Runs once on first opening the restored project.
// Can also be invoked from Tools > Carrot Rush > Restore Level 1 visuals.
#if UNITY_EDITOR
using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Animations;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[InitializeOnLoad]
internal static class CarrotRushRestorer
{
    const string LevelPath = "Assets/Scenes/Level1.unity";
    const string HudPath = "Assets/Prefabs/HUDCanvas.prefab";
    static string Marker => Path.Combine(Path.GetDirectoryName(Application.dataPath), "ProjectSettings/CarrotRushRestored.flag");

    static CarrotRushRestorer()
    {
        EditorApplication.update += WaitForReady;
    }

    static void WaitForReady()
    {
        if (File.Exists(Marker)) { EditorApplication.update -= WaitForReady; return; }
        if (EditorApplication.isPlayingOrWillChangePlaymode || EditorApplication.isCompiling || EditorApplication.isUpdating)
            return;
        EditorApplication.update -= WaitForReady;
        RunOnce();
    }

    static void RunOnce()
    {
        if (File.Exists(Marker) || EditorApplication.isPlayingOrWillChangePlaymode || EditorApplication.isCompiling || EditorApplication.isUpdating)
            return;
        try
        {
            Restore();
            File.WriteAllText(Marker, "Visuals, animations and HUD repaired. Use Tools/Carrot Rush to run again.\n");
            Debug.Log("Carrot Rush: restored the Level1 characters, animations and mobile HUD.");
        }
        catch (Exception ex)
        {
            Debug.LogError("Carrot Rush automatic restoration failed. Open Tools > Carrot Rush > Restore Level 1 visuals after import. " + ex);
        }
    }

    [MenuItem("Tools/Carrot Rush/Restore Level 1 visuals")]
    public static void RestoreManually()
    {
        Restore();
        File.WriteAllText(Marker, "Restored manually.\n");
        Debug.Log("Carrot Rush: restoration complete.");
    }

    static void Restore()
    {
        if (AssetDatabase.LoadAssetAtPath<SceneAsset>(LevelPath) == null)
            throw new Exception("Level1 scene not found");

        var bunnyIdle = FillClip("Assets/Animation/New Animation.anim", "Assets/Sprites/Player/Bunny/bunny_idle_32.png", true);
        var bunnyJump = FillClip("Assets/Animation/Bunny_Jump.anim", "Assets/Sprites/Player/Bunny/bunny_jump_32.png", false);
        var foxIdle = FillClip("Assets/Animation/Fox_idle.anim", "Assets/Sprites/Enemies/Fox/fox_idle_32.png", true);
        var pandaIdle = FillClip("Assets/Animation/Panda_Idle.anim", "Assets/Sprites/Enemies/Panda/panda_idle_32.png", true);
        var penguinIdle = FillClip("Assets/Animation/Penguin_idle.anim", "Assets/Sprites/Enemies/Penguin/penguin_idle_32.png", true);

        var bunnyController = FillController("Assets/Animation/BunnyVisual.controller", bunnyIdle, bunnyJump);
        var foxController = FillController("Assets/Animation/FoxVisual.controller", foxIdle, null);
        var pandaController = FillController("Assets/Animation/PandaVisual.controller", pandaIdle, null);
        var penguinController = FillController("Assets/Animation/PenguinVisual.controller", penguinIdle, null);

        RestoreHud();
        string previousScene = SceneManager.GetActiveScene().path;
        bool priorSceneDirty = SceneManager.GetActiveScene().isDirty;
        // Preserve currently open unsaved edits rather than discarding them during scene opening.
        if (priorSceneDirty && previousScene != LevelPath)
            throw new Exception("An unsaved scene is open; save it before rerunning the restorer");
        var level = EditorSceneManager.OpenScene(LevelPath, OpenSceneMode.Single);
        MakeVisual("Player", "BunnyVisual", "Assets/Sprites/Player/Bunny/bunny_idle_32.png", new Vector3(0f, 0.1f, 0f), new Vector3(2.14f, 1.5f, 1f), bunnyController);
        MakeVisual("Patroller", "FoxVisual", "Assets/Sprites/Enemies/Fox/fox_idle_32.png", new Vector3(0f, 0.1f, 0f), new Vector3(1.5f, 1.5f, 1f), foxController);
        MakeVisual("Shooter", "PandaVisual", "Assets/Sprites/Enemies/Panda/panda_idle_32.png", Vector3.zero, new Vector3(1.5f, 1.5f, 1f), pandaController);
        MakeVisual("Charger", "PenguinVisual", "Assets/Sprites/Enemies/Penguin/penguin_idle_32.png", Vector3.zero, new Vector3(1.5f, 1.5f, 1f), penguinController);
        EditorSceneManager.MarkSceneDirty(level);
        EditorSceneManager.SaveScene(level);
        AssetDatabase.SaveAssets();
        if (!string.IsNullOrEmpty(previousScene) && previousScene != LevelPath)
            EditorSceneManager.OpenScene(previousScene, OpenSceneMode.Single);
    }

    static Sprite[] Frames(string png)
    {
        var frames = AssetDatabase.LoadAllAssetsAtPath(png).OfType<Sprite>()
            .OrderBy(s => FrameNumber(s.name)).ToArray();
        if (frames.Length == 0)
            throw new Exception("No sprites found in " + png);
        return frames;
    }
    static int FrameNumber(string name)
    {
        int separator = name.LastIndexOf('_');
        return separator >= 0 && int.TryParse(name.Substring(separator + 1), out var i) ? i : 0;
    }

    static AnimationClip FillClip(string clipPath, string png, bool loop)
    {
        var clip = AssetDatabase.LoadAssetAtPath<AnimationClip>(clipPath);
        if (clip == null) throw new Exception("Missing animation: " + clipPath);
        var frames = Frames(png);
        clip.frameRate = 8f;
        var keys = new ObjectReferenceKeyframe[frames.Length];
        for (int i = 0; i < frames.Length; ++i)
            keys[i] = new ObjectReferenceKeyframe { time = i / 8f, value = frames[i] };
        var binding = EditorCurveBinding.PPtrCurve("", typeof(SpriteRenderer), "m_Sprite");
        AnimationUtility.SetObjectReferenceCurve(clip, binding, keys);
        var serializedClip = new SerializedObject(clip);
        var clipSettings = serializedClip.FindProperty("m_AnimationClipSettings");
        if (clipSettings != null)
        {
            var loopTime = clipSettings.FindPropertyRelative("m_LoopTime");
            if (loopTime != null) loopTime.boolValue = loop;
            serializedClip.ApplyModifiedPropertiesWithoutUndo();
        }
        EditorUtility.SetDirty(clip);
        return clip;
    }

    static AnimatorController FillController(string path, AnimationClip idle, AnimationClip jump)
    {
        var controller = AssetDatabase.LoadAssetAtPath<AnimatorController>(path);
        if (controller == null) throw new Exception("Missing Animator Controller: " + path);
        if (controller.layers.Length == 0) controller.AddLayer("Base Layer");
        var machine = controller.layers[0].stateMachine;
        var idleState = machine.states.Select(s => s.state).FirstOrDefault(s => s.name == "Idle" || s.name == "Bunny_Idle" || s.name == "New Animation");
        if (idleState == null) idleState = machine.AddState("Idle", new Vector3(230, 90, 0));
        idleState.motion = idle;
        machine.defaultState = idleState;
        if (jump != null)
        {
            if (!controller.parameters.Any(p => p.name == "IsGrounded"))
                controller.AddParameter("IsGrounded", AnimatorControllerParameterType.Bool);
            var jumpState = machine.states.Select(s => s.state).FirstOrDefault(s => s.name == "Jump" || s.name == "Bunny_Jump");
            if (jumpState == null) jumpState = machine.AddState("Jump", new Vector3(485, 90, 0));
            jumpState.motion = jump;
            var toJump = idleState.transitions.FirstOrDefault(t => t.destinationState == jumpState);
            if (toJump == null) toJump = idleState.AddTransition(jumpState);
            toJump.hasExitTime = false; toJump.hasFixedDuration = true; toJump.duration = 0; toJump.offset = 0;
                toJump.AddCondition(AnimatorConditionMode.IfNot, 0f, "IsGrounded");
            var toIdle = jumpState.transitions.FirstOrDefault(t => t.destinationState == idleState);
            if (toIdle == null) toIdle = jumpState.AddTransition(idleState);
            toIdle.hasExitTime = false; toIdle.hasFixedDuration = true; toIdle.duration = 0; toIdle.offset = 0;
            toIdle.AddCondition(AnimatorConditionMode.If, 0f, "IsGrounded");
            EditorUtility.SetDirty(jumpState);
        }
        EditorUtility.SetDirty(idleState);
        EditorUtility.SetDirty(machine);
        EditorUtility.SetDirty(controller);
        return controller;
    }

    static void MakeVisual(string parentName, string childName, string png, Vector3 position, Vector3 scale, RuntimeAnimatorController controller)
    {
        var parent = GameObject.Find(parentName);
        if (parent == null) throw new Exception("Missing " + parentName + " in Level1");
        var original = parent.GetComponent<SpriteRenderer>();
        if (original != null) original.enabled = false;
        var child = parent.transform.Find(childName);
        if (child == null)
        {
            var obj = new GameObject(childName);
            obj.transform.SetParent(parent.transform, false);
            child = obj.transform;
        }
        child.gameObject.layer = parent.layer;
        child.localPosition = position;
        child.localRotation = Quaternion.identity;
        child.localScale = scale;
        var renderer = child.GetComponent<SpriteRenderer>();
        if (renderer == null) renderer = child.gameObject.AddComponent<SpriteRenderer>();
        renderer.enabled = true;
        renderer.sprite = Frames(png)[0];
        renderer.sortingOrder = 5;
        renderer.color = Color.white;
        var animator = child.GetComponent<Animator>();
        if (animator == null) animator = child.gameObject.AddComponent<Animator>();
        animator.runtimeAnimatorController = controller;
        animator.applyRootMotion = false;
        if (parentName == "Player")
        {
            var movement = parent.GetComponent<PlayerMovement>();
            if (movement != null)
            {
                var so = new SerializedObject(movement);
                var field = so.FindProperty("bunnyAnimator");
                if (field != null) { field.objectReferenceValue = animator; so.ApplyModifiedPropertiesWithoutUndo(); }
            }
        }
        // Mirror the original renderer's warning/damage tint onto the visible character.
        // PlayerHealth and the enemy scripts continue using their existing parent renderer.
        var relay = child.GetComponent<CarrotRushEnemyTint>();
        if (relay == null) relay = child.gameObject.AddComponent<CarrotRushEnemyTint>();
        relay.source = original;
        relay.target = renderer;
    }

    static void RestoreHud()
    {
        var hud = PrefabUtility.LoadPrefabContents(HudPath);
        try
        {
            Transform Find(string localPath)
            {
                var result = hud.transform.Find(localPath);
                if (result == null) throw new Exception("HUD missing: " + localPath);
                return result;
            }
            var jump = Find("SafeArea/Controls/JumpButton");
            var fire = Find("SafeArea/Controls/FireButton");
            var pause = Find("SafeArea/Controls/PauseButton");
            var stick = Find("SafeArea/Controls/Joystick");
            var knob = Find("SafeArea/Controls/Joystick/Knob");
            Color pink = new Color32(232, 165, 183, 255);
            Color dark = new Color32(89, 68, 88, 255);
            ButtonStyle(jump, pink, dark, new Vector2(140, 140));
            ButtonStyle(fire, pink, dark, new Vector2(120, 120));
            ButtonStyle(pause, pink, dark, new Vector2(120, 120));
            var jrect = (RectTransform)jump;
            var frect = (RectTransform)fire;
            jrect.anchoredPosition = new Vector2(-160, 120);
            frect.anchoredPosition = new Vector2(-355, 95);
            var joystickImage = stick.GetComponent<Image>();
            if (joystickImage != null) joystickImage.color = new Color32(232, 165, 183, 135);
            ((RectTransform)stick).sizeDelta = new Vector2(180, 180);
            var knobImage = knob.GetComponent<Image>();
            if (knobImage != null) knobImage.color = Color.white;
            ((RectTransform)knob).sizeDelta = new Vector2(75, 75);
            // Visual icon children are not raycast targets; existing on-screen controls stay intact.
            var oldJumpText = jump.Find("Jump");
            if (oldJumpText != null) oldJumpText.gameObject.SetActive(false);
            var oldFireText = fire.Find("Fire");
            if (oldFireText != null) oldFireText.gameObject.SetActive(false);
            MakeIcon(jump, "JumpIcon", "Assets/Sprites/UI/PNG/Extra/Default/icon_arrow_up_outline.png", new Vector2(55, 55));
            MakeIcon(fire, "FireIcon", "Assets/Sprites/UI/Outline/crosshair-013.png", new Vector2(60, 60));
            PrefabUtility.SaveAsPrefabAsset(hud, HudPath);
        }
        finally { PrefabUtility.UnloadPrefabContents(hud); }
    }

    static void ButtonStyle(Transform control, Color color, Color outlineColor, Vector2 size)
    {
        ((RectTransform)control).sizeDelta = size;
        var image = control.GetComponent<Image>();
        if (image != null) image.color = color;
        var outline = control.GetComponent<UnityEngine.UI.Outline>();
        if (outline == null) outline = control.gameObject.AddComponent<UnityEngine.UI.Outline>();
        outline.effectColor = outlineColor;
        outline.effectDistance = new Vector2(3, -3);
        outline.useGraphicAlpha = true;
    }

    static void MakeIcon(Transform parent, string iconName, string spritePath, Vector2 size)
    {
        var icon = parent.Find(iconName);
        if (icon == null)
        {
            var go = new GameObject(iconName, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            icon = go.transform;
            icon.SetParent(parent, false);
        }
        var img = icon.GetComponent<Image>();
        if (img == null) img = icon.gameObject.AddComponent<Image>();
        var sprite = AssetDatabase.LoadAllAssetsAtPath(spritePath).OfType<Sprite>().FirstOrDefault();
        if (sprite == null) throw new Exception("Missing UI sprite: " + spritePath);
        img.sprite = sprite;
        img.color = Color.white;
        img.preserveAspect = true;
        img.raycastTarget = false;
        var rect = (RectTransform)icon;
        rect.anchorMin = rect.anchorMax = rect.pivot = new Vector2(.5f, .5f);
        rect.anchoredPosition = new Vector2(0, iconName == "JumpIcon" ? 2 : 0);
        rect.sizeDelta = size;
        rect.localScale = Vector3.one;
        icon.gameObject.SetActive(true);
    }
}
#endif
