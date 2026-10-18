using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using TMPro;
using AetherSurge.Core;
using AetherSurge.Player;
using AetherSurge.Combat;
using AetherSurge.Dummy;
using AetherSurge.UI;

namespace AetherSurge.Editor
{
    public static class WhiteboxSceneBuilder
    {
        // Layer indices matching TagManager.asset
        private const int LAYER_PLAYER = 6;
        private const int LAYER_ENEMY = 7;
        private const int LAYER_GROUND = 8;

        [MenuItem("Aether Surge/Generate Whitebox Arena Scene")]
        public static void BuildArenaScene()
        {
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

            // 1. Camera with follow
            var camObj = new GameObject("Main Camera");
            camObj.tag = "MainCamera";
            var cam = camObj.AddComponent<Camera>();
            cam.orthographic = true;
            cam.orthographicSize = 6.0f;
            cam.clearFlags = CameraClearFlags.SolidColor;
            cam.backgroundColor = new Color(0.12f, 0.12f, 0.16f);
            camObj.transform.position = new Vector3(0f, 2.5f, -10f);
            var cameraFollow = camObj.AddComponent<CameraFollow>();

            // 2. EventSystem
            var eventSystemObj = new GameObject("EventSystem");
            eventSystemObj.AddComponent<EventSystem>();
            eventSystemObj.AddComponent<InputSystemUIInputModule>();

            // 3. Directional Light
            var lightObj = new GameObject("Directional Light");
            var light = lightObj.AddComponent<Light>();
            light.type = LightType.Directional;
            light.color = Color.white;
            light.intensity = 1.0f;
            lightObj.transform.rotation = Quaternion.Euler(50f, -30f, 0f);

            // 4. HitStopManager
            var hitStopObj = new GameObject("HitStopManager");
            hitStopObj.AddComponent<HitStopManager>();

            // 5. Arena Environment (on Ground layer)
            var arenaRoot = new GameObject("Arena");

            // Ground
            var ground = GameObject.CreatePrimitive(PrimitiveType.Cube);
            ground.name = "Ground";
            ground.layer = LAYER_GROUND;
            ground.transform.SetParent(arenaRoot.transform);
            ground.transform.position = new Vector3(0f, -0.5f, 0f);
            ground.transform.localScale = new Vector3(30f, 1f, 2f);
            Object.DestroyImmediate(ground.GetComponent<BoxCollider>());
            var groundCol = ground.AddComponent<BoxCollider2D>();
            groundCol.size = new Vector2(30f, 1f);
            var groundMat = ground.GetComponent<MeshRenderer>();
            if (groundMat != null && groundMat.sharedMaterial != null)
            {
                groundMat.sharedMaterial.color = new Color(0.25f, 0.25f, 0.3f);
            }

            // Left Wall (on Ground layer — static geometry)
            var leftWall = GameObject.CreatePrimitive(PrimitiveType.Cube);
            leftWall.name = "LeftWall";
            leftWall.layer = LAYER_GROUND;
            leftWall.transform.SetParent(arenaRoot.transform);
            leftWall.transform.position = new Vector3(-15f, 5f, 0f);
            leftWall.transform.localScale = new Vector3(1f, 12f, 2f);
            Object.DestroyImmediate(leftWall.GetComponent<BoxCollider>());
            leftWall.AddComponent<BoxCollider2D>();

            // Right Wall (on Ground layer)
            var rightWall = GameObject.CreatePrimitive(PrimitiveType.Cube);
            rightWall.name = "RightWall";
            rightWall.layer = LAYER_GROUND;
            rightWall.transform.SetParent(arenaRoot.transform);
            rightWall.transform.position = new Vector3(15f, 5f, 0f);
            rightWall.transform.localScale = new Vector3(1f, 12f, 2f);
            Object.DestroyImmediate(rightWall.GetComponent<BoxCollider>());
            rightWall.AddComponent<BoxCollider2D>();

            // 6. Training Dummy (on Enemy layer)
            var dummyObj = new GameObject("TrainingDummy");
            dummyObj.layer = LAYER_ENEMY;
            dummyObj.transform.position = new Vector3(3.5f, 1f, 0f);

            var dummyRb = dummyObj.AddComponent<Rigidbody2D>();
            dummyRb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
            dummyRb.freezeRotation = true;
            dummyRb.mass = 2.0f;

            var dummyCol = dummyObj.AddComponent<BoxCollider2D>();
            dummyCol.size = new Vector2(1.2f, 2.0f);

            var dummyVisual = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            dummyVisual.name = "Visual";
            dummyVisual.layer = LAYER_ENEMY;
            dummyVisual.transform.SetParent(dummyObj.transform);
            dummyVisual.transform.localPosition = Vector3.zero;
            dummyVisual.transform.localScale = new Vector3(1.2f, 1.0f, 1.0f);
            Object.DestroyImmediate(dummyVisual.GetComponent<Collider>());
            var dummyMat = dummyVisual.GetComponent<MeshRenderer>();
            if (dummyMat != null && dummyMat.sharedMaterial != null)
            {
                dummyMat.sharedMaterial.color = new Color(0.85f, 0.25f, 0.25f);
            }

            var dummyComp = dummyObj.AddComponent<TrainingDummy>();

            // 7. Player (on Player layer)
            var playerObj = new GameObject("Player");
            playerObj.layer = LAYER_PLAYER;
            playerObj.transform.position = new Vector3(-3.0f, 1f, 0f);

            var playerRb = playerObj.AddComponent<Rigidbody2D>();
            playerRb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
            playerRb.freezeRotation = true;
            playerRb.gravityScale = 3.5f;

            var playerCol = playerObj.AddComponent<BoxCollider2D>();
            playerCol.size = new Vector2(1.0f, 1.8f);

            var playerVisual = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            playerVisual.name = "Visual";
            playerVisual.layer = LAYER_PLAYER;
            playerVisual.transform.SetParent(playerObj.transform);
            playerVisual.transform.localPosition = Vector3.zero;
            playerVisual.transform.localScale = new Vector3(1.0f, 0.9f, 1.0f);
            Object.DestroyImmediate(playerVisual.GetComponent<Collider>());
            var playerMat = playerVisual.GetComponent<MeshRenderer>();
            if (playerMat != null && playerMat.sharedMaterial != null)
            {
                playerMat.sharedMaterial.color = new Color(0.2f, 0.55f, 0.95f);
            }

            var locomotion = playerObj.AddComponent<PlayerLocomotion>();
            locomotion.GroundLayer = 1 << LAYER_GROUND; // Only detect Ground layer

            playerObj.AddComponent<CombatStateMachine>();
            playerObj.AddComponent<EnergyController>();
            var playerChar = playerObj.AddComponent<PlayerCharacter>();

            // Set target layer mask to Enemy layer only (no self-hit, no ground hit)
            playerChar.TargetLayer = 1 << LAYER_ENEMY;

            // Input services
            var standaloneInput = playerObj.AddComponent<StandaloneInputService>();
            var mobileInput = playerObj.AddComponent<MobileTouchInputService>();
            var compositeInput = playerObj.AddComponent<CompositeInputService>();
            compositeInput.Initialize(standaloneInput, mobileInput);
            playerChar.SetInputService(compositeInput);

            // Camera follows player
            cameraFollow.SetTarget(playerObj.transform);

            // 8. UI Canvas
            var canvasObj = new GameObject("Canvas");
            var canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            var scaler = canvasObj.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(2796, 1290);
            scaler.matchWidthOrHeight = 0.5f;
            canvasObj.AddComponent<GraphicRaycaster>();

            // Safe Area Container
            var safeAreaObj = new GameObject("SafeAreaContainer");
            safeAreaObj.transform.SetParent(canvasObj.transform, false);
            var safeRect = safeAreaObj.AddComponent<RectTransform>();
            safeRect.anchorMin = Vector2.zero;
            safeRect.anchorMax = Vector2.one;
            safeRect.offsetMin = Vector2.zero;
            safeRect.offsetMax = Vector2.zero;
            safeAreaObj.AddComponent<SafeAreaFitter>();

            // HUD
            var hudComp = canvasObj.AddComponent<CombatHUD>();

            // 8.1 Mobile Controls (with platform visibility controller)
            var mobileUIObj = new GameObject("MobileControlsRoot");
            mobileUIObj.transform.SetParent(safeAreaObj.transform, false);
            var mobileUIRect = mobileUIObj.AddComponent<RectTransform>();
            mobileUIRect.anchorMin = Vector2.zero;
            mobileUIRect.anchorMax = Vector2.one;
            mobileUIRect.offsetMin = Vector2.zero;
            mobileUIRect.offsetMax = Vector2.zero;
            var mobileControlsUI = mobileUIObj.AddComponent<MobileControlsUI>();
            mobileUIObj.AddComponent<PlatformVisibility>();

            // Virtual Joystick (Left)
            var joystickObj = new GameObject("VirtualJoystick");
            joystickObj.transform.SetParent(mobileUIObj.transform, false);
            var joyRect = joystickObj.AddComponent<RectTransform>();
            joyRect.anchorMin = new Vector2(0f, 0f);
            joyRect.anchorMax = new Vector2(0f, 0f);
            joyRect.anchoredPosition = new Vector2(260f, 260f);
            joyRect.sizeDelta = new Vector2(240f, 240f);
            var joyBg = joystickObj.AddComponent<Image>();
            joyBg.color = new Color(0.15f, 0.15f, 0.2f, 0.45f);

            var handleObj = new GameObject("Handle");
            handleObj.transform.SetParent(joystickObj.transform, false);
            var handleRect = handleObj.AddComponent<RectTransform>();
            handleRect.sizeDelta = new Vector2(100f, 100f);
            var handleImg = handleObj.AddComponent<Image>();
            handleImg.color = new Color(0.3f, 0.7f, 1.0f, 0.8f);

            var virtualJoystick = joystickObj.AddComponent<VirtualJoystick>();

            // Action Buttons (Right)
            var atkBtn = CreateTouchButton("Btn_Attack", "ATK", new Vector2(1f, 0f), new Vector2(-280f, 240f), new Vector2(160f, 160f), mobileUIObj.transform, new Color(0.9f, 0.25f, 0.25f, 0.75f));
            var jumpBtn = CreateTouchButton("Btn_Jump", "JUMP", new Vector2(1f, 0f), new Vector2(-460f, 160f), new Vector2(120f, 120f), mobileUIObj.transform, new Color(0.2f, 0.6f, 0.95f, 0.75f));
            var dashBtn = CreateTouchButton("Btn_Dash", "DASH", new Vector2(1f, 0f), new Vector2(-120f, 320f), new Vector2(120f, 120f), mobileUIObj.transform, new Color(0.2f, 0.8f, 0.4f, 0.75f));
            var skillBtn = CreateTouchButton("Btn_Skill", "SKILL", new Vector2(1f, 0f), new Vector2(-280f, 440f), new Vector2(120f, 120f), mobileUIObj.transform, new Color(0.7f, 0.3f, 0.95f, 0.75f));
            var chargeBtn = CreateTouchButton("Btn_Charge", "CHARGE", new Vector2(1f, 0f), new Vector2(-460f, 320f), new Vector2(120f, 120f), mobileUIObj.transform, new Color(1.0f, 0.65f, 0.1f, 0.75f));

            mobileControlsUI.BindControls(mobileInput, virtualJoystick, atkBtn, jumpBtn, dashBtn, skillBtn, chargeBtn);

            // 8.2 Top HUD: Reset button + FPS counter
            var resetBtnObj = new GameObject("Btn_ResetArena");
            resetBtnObj.transform.SetParent(safeAreaObj.transform, false);
            var resetRect = resetBtnObj.AddComponent<RectTransform>();
            resetRect.anchorMin = new Vector2(0.5f, 1f);
            resetRect.anchorMax = new Vector2(0.5f, 1f);
            resetRect.anchoredPosition = new Vector2(0f, -80f);
            resetRect.sizeDelta = new Vector2(220f, 60f);
            var resetImg = resetBtnObj.AddComponent<Image>();
            resetImg.color = new Color(0.3f, 0.3f, 0.35f, 0.85f);
            resetBtnObj.AddComponent<Button>();

            var resetTextObj = new GameObject("Text");
            resetTextObj.transform.SetParent(resetBtnObj.transform, false);
            var resetText = resetTextObj.AddComponent<TextMeshProUGUI>();
            resetText.text = "RESET ARENA";
            resetText.fontSize = 26;
            resetText.alignment = TextAlignmentOptions.Center;
            resetText.color = Color.white;
            var resetTextRect = resetTextObj.GetComponent<RectTransform>();
            resetTextRect.anchorMin = Vector2.zero;
            resetTextRect.anchorMax = Vector2.one;
            resetTextRect.offsetMin = Vector2.zero;
            resetTextRect.offsetMax = Vector2.zero;

            // FPS Counter
            var fpsObj = new GameObject("Text_FPS");
            fpsObj.transform.SetParent(safeAreaObj.transform, false);
            var fpsRect = fpsObj.AddComponent<RectTransform>();
            fpsRect.anchorMin = new Vector2(1f, 1f);
            fpsRect.anchorMax = new Vector2(1f, 1f);
            fpsRect.anchoredPosition = new Vector2(-120f, -60f);
            fpsRect.sizeDelta = new Vector2(200f, 50f);
            var fpsText = fpsObj.AddComponent<TextMeshProUGUI>();
            fpsText.text = "FPS: --";
            fpsText.fontSize = 28;
            fpsText.alignment = TextAlignmentOptions.Right;
            fpsText.color = Color.green;

            // Save Scene
            if (!AssetDatabase.IsValidFolder("Assets/Scenes"))
            {
                AssetDatabase.CreateFolder("Assets", "Scenes");
            }

            string scenePath = "Assets/Scenes/WhiteboxArena.unity";
            EditorSceneManager.SaveScene(scene, scenePath);
            AssetDatabase.Refresh();
            Debug.LogFormat("[WhiteboxSceneBuilder] Successfully generated Whitebox Arena at: {0}", scenePath);
            Debug.LogFormat("[WhiteboxSceneBuilder] Layers: Player={0}, Enemy={1}, Ground={2}", LAYER_PLAYER, LAYER_ENEMY, LAYER_GROUND);
        }

        private static VirtualTouchButton CreateTouchButton(string name, string label, Vector2 anchor, Vector2 anchoredPos, Vector2 size, Transform parent, Color color)
        {
            var btnObj = new GameObject(name);
            btnObj.transform.SetParent(parent, false);
            var rect = btnObj.AddComponent<RectTransform>();
            rect.anchorMin = anchor;
            rect.anchorMax = anchor;
            rect.anchoredPosition = anchoredPos;
            rect.sizeDelta = size;

            var img = btnObj.AddComponent<Image>();
            img.color = color;

            var touchBtn = btnObj.AddComponent<VirtualTouchButton>();

            var textObj = new GameObject("Label");
            textObj.transform.SetParent(btnObj.transform, false);
            var textRect = textObj.AddComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = Vector2.zero;
            textRect.offsetMax = Vector2.zero;

            var tmp = textObj.AddComponent<TextMeshProUGUI>();
            tmp.text = label;
            tmp.fontSize = 28;
            tmp.fontStyle = FontStyles.Bold;
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.color = Color.white;

            return touchBtn;
        }
    }
}
