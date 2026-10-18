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
using AetherSurge.Enemy;

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

            // 6. Basic Enemy (Rogue Astral Construct on Enemy layer)
            var enemyObj = new GameObject("BasicEnemy");
            enemyObj.layer = LAYER_ENEMY;
            enemyObj.transform.position = new Vector3(4.5f, 1f, 0f);

            var enemyRb = enemyObj.AddComponent<Rigidbody2D>();
            enemyRb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
            enemyRb.freezeRotation = true;
            enemyRb.mass = 1.4f;

            var enemyCol = enemyObj.AddComponent<BoxCollider2D>();
            enemyCol.size = new Vector2(1.2f, 2.0f);

            var enemyVisual = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            enemyVisual.name = "Visual";
            enemyVisual.layer = LAYER_ENEMY;
            enemyVisual.transform.SetParent(enemyObj.transform);
            enemyVisual.transform.localPosition = Vector3.zero;
            enemyVisual.transform.localScale = new Vector3(1.2f, 1.0f, 1.0f);
            Object.DestroyImmediate(enemyVisual.GetComponent<Collider>());
            var enemyMat = enemyVisual.GetComponent<MeshRenderer>();
            if (enemyMat != null && enemyMat.sharedMaterial != null)
            {
                enemyMat.sharedMaterial.color = new Color(0.9f, 0.2f, 0.2f);
            }

            var enemyComp = enemyObj.AddComponent<BasicEnemy>();

            // Backwards-compatible dummy reference
            var dummyObj = enemyObj;
            var dummyComp = enemyComp;

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

            // Set target layer mask to Enemy layer (7) and Default (0)
            playerChar.TargetLayer = (1 << LAYER_ENEMY) | (1 << 0);

            // Input services
            var standaloneInput = playerObj.AddComponent<StandaloneInputService>();
            var mobileInput = playerObj.AddComponent<MobileTouchInputService>();
            var compositeInput = playerObj.AddComponent<CompositeInputService>();
            compositeInput.Initialize(standaloneInput, mobileInput);
            playerChar.SetInputService(compositeInput);

            // Match Manager
            var matchObj = new GameObject("MatchManager");
            var matchManager = matchObj.AddComponent<MatchManager>();
            matchManager.Player = playerChar;
            matchManager.Enemy = enemyComp;
            matchManager.PlayerSpawnPoint = new Vector3(-3.0f, 1.0f, 0f);

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

            // Top Objective Text
            var objectiveObj = new GameObject("Text_Objective");
            objectiveObj.transform.SetParent(safeAreaObj.transform, false);
            var objRect = objectiveObj.AddComponent<RectTransform>();
            objRect.anchorMin = new Vector2(0.5f, 1f);
            objRect.anchorMax = new Vector2(0.5f, 1f);
            objRect.anchoredPosition = new Vector2(0f, -25f);
            objRect.sizeDelta = new Vector2(650f, 40f);
            var objText = objectiveObj.AddComponent<TextMeshProUGUI>();
            objText.text = "OBJECTIVE: Defeat the Rogue Astral Construct";
            objText.fontSize = 22;
            objText.alignment = TextAlignmentOptions.Center;
            objText.color = new Color(0.9f, 0.9f, 0.95f, 0.9f);

            // Top HUD: Restart button
            var resetBtnObj = new GameObject("Btn_ResetArena");
            resetBtnObj.transform.SetParent(safeAreaObj.transform, false);
            var resetRect = resetBtnObj.AddComponent<RectTransform>();
            resetRect.anchorMin = new Vector2(0.5f, 1f);
            resetRect.anchorMax = new Vector2(0.5f, 1f);
            resetRect.anchoredPosition = new Vector2(0f, -75f);
            resetRect.sizeDelta = new Vector2(240f, 50f);
            var resetImg = resetBtnObj.AddComponent<Image>();
            resetImg.color = new Color(0.25f, 0.25f, 0.32f, 0.9f);
            var resetBtn = resetBtnObj.AddComponent<Button>();

            var resetTextObj = new GameObject("Text");
            resetTextObj.transform.SetParent(resetBtnObj.transform, false);
            var resetText = resetTextObj.AddComponent<TextMeshProUGUI>();
            resetText.text = "RESTART MATCH (R)";
            resetText.fontSize = 22;
            resetText.alignment = TextAlignmentOptions.Center;
            resetText.color = Color.white;
            var resetTextRect = resetTextObj.GetComponent<RectTransform>();
            resetTextRect.anchorMin = Vector2.zero;
            resetTextRect.anchorMax = Vector2.one;
            resetTextRect.offsetMin = Vector2.zero;
            resetTextRect.offsetMax = Vector2.zero;

            // Player HP Display (Top Left)
            var playerHpObj = new GameObject("Text_PlayerHP");
            playerHpObj.transform.SetParent(safeAreaObj.transform, false);
            var playerHpRect = playerHpObj.AddComponent<RectTransform>();
            playerHpRect.anchorMin = new Vector2(0f, 1f);
            playerHpRect.anchorMax = new Vector2(0f, 1f);
            playerHpRect.anchoredPosition = new Vector2(160f, -40f);
            playerHpRect.sizeDelta = new Vector2(260f, 40f);
            var playerHpText = playerHpObj.AddComponent<TextMeshProUGUI>();
            playerHpText.text = "Player HP: 200 / 200";
            playerHpText.fontSize = 24;
            playerHpText.alignment = TextAlignmentOptions.Left;
            playerHpText.color = new Color(0.3f, 0.9f, 0.4f);

            // Player Energy Display (Top Left, below HP)
            var energyObj = new GameObject("Text_Energy");
            energyObj.transform.SetParent(safeAreaObj.transform, false);
            var energyRect = energyObj.AddComponent<RectTransform>();
            energyRect.anchorMin = new Vector2(0f, 1f);
            energyRect.anchorMax = new Vector2(0f, 1f);
            energyRect.anchoredPosition = new Vector2(160f, -75f);
            energyRect.sizeDelta = new Vector2(260f, 40f);
            var energyText = energyObj.AddComponent<TextMeshProUGUI>();
            energyText.text = "Energy: 50 / 100";
            energyText.fontSize = 22;
            energyText.alignment = TextAlignmentOptions.Left;
            energyText.color = new Color(0.2f, 0.85f, 1.0f);

            // Enemy HP Display (Top Right)
            var enemyHpObj = new GameObject("Text_EnemyHP");
            enemyHpObj.transform.SetParent(safeAreaObj.transform, false);
            var enemyHpRect = enemyHpObj.AddComponent<RectTransform>();
            enemyHpRect.anchorMin = new Vector2(1f, 1f);
            enemyHpRect.anchorMax = new Vector2(1f, 1f);
            enemyHpRect.anchoredPosition = new Vector2(-360f, -40f);
            enemyHpRect.sizeDelta = new Vector2(280f, 40f);
            var enemyHpText = enemyHpObj.AddComponent<TextMeshProUGUI>();
            enemyHpText.text = "Enemy HP: 250 / 250";
            enemyHpText.fontSize = 24;
            enemyHpText.alignment = TextAlignmentOptions.Right;
            enemyHpText.color = new Color(1.0f, 0.4f, 0.35f);

            // FPS Counter (Top Right, below Enemy HP)
            var fpsObj = new GameObject("Text_FPS");
            fpsObj.transform.SetParent(safeAreaObj.transform, false);
            var fpsRect = fpsObj.AddComponent<RectTransform>();
            fpsRect.anchorMin = new Vector2(1f, 1f);
            fpsRect.anchorMax = new Vector2(1f, 1f);
            fpsRect.anchoredPosition = new Vector2(-100f, -40f);
            fpsRect.sizeDelta = new Vector2(160f, 40f);
            var fpsText = fpsObj.AddComponent<TextMeshProUGUI>();
            fpsText.text = "FPS: --";
            fpsText.fontSize = 22;
            fpsText.alignment = TextAlignmentOptions.Right;
            fpsText.color = Color.green;

            // Outcome Banner (Centered Modal / Banner)
            var bannerRootObj = new GameObject("OutcomeBannerRoot");
            bannerRootObj.transform.SetParent(safeAreaObj.transform, false);
            var bannerRootRect = bannerRootObj.AddComponent<RectTransform>();
            bannerRootRect.anchorMin = new Vector2(0.5f, 0.5f);
            bannerRootRect.anchorMax = new Vector2(0.5f, 0.5f);
            bannerRootRect.anchoredPosition = new Vector2(0f, 50f);
            bannerRootRect.sizeDelta = new Vector2(600f, 220f);
            var bannerBg = bannerRootObj.AddComponent<Image>();
            bannerBg.color = new Color(0.08f, 0.08f, 0.12f, 0.92f);

            var bannerTextObj = new GameObject("Text_OutcomeBanner");
            bannerTextObj.transform.SetParent(bannerRootObj.transform, false);
            var bannerTextRect = bannerTextObj.AddComponent<RectTransform>();
            bannerTextRect.anchorMin = Vector2.zero;
            bannerTextRect.anchorMax = Vector2.one;
            bannerTextRect.offsetMin = new Vector2(20f, 20f);
            bannerTextRect.offsetMax = new Vector2(-20f, -20f);
            var bannerText = bannerTextObj.AddComponent<TextMeshProUGUI>();
            bannerText.text = "VICTORY!";
            bannerText.fontSize = 36;
            bannerText.alignment = TextAlignmentOptions.Center;
            bannerText.color = Color.yellow;

            bannerRootObj.SetActive(false); // Initially hidden

            // Wire HUD
            hudComp.BindControls(resetBtn, fpsText, energyText, enemyHpText, playerHpText, objText, bannerText, bannerRootObj);

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
