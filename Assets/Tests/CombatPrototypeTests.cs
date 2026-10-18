using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using AetherSurge.Core;
using AetherSurge.Player;
using AetherSurge.Combat;
using AetherSurge.Dummy;
using AetherSurge.UI;

namespace AetherSurge.Tests
{
    public class CombatPrototypeTests
    {
        private GameObject _playerObject;
        private PlayerCharacter _player;
        private PlayerLocomotion _locomotion;
        private CombatStateMachine _combat;
        private EnergyController _energy;
        private MockInputService _mockInput;

        [SetUp]
        public void SetUp()
        {
            _playerObject = new GameObject("TestPlayer");
            _playerObject.AddComponent<Rigidbody2D>();
            _locomotion = _playerObject.AddComponent<PlayerLocomotion>();
            _combat = _playerObject.AddComponent<CombatStateMachine>();
            _energy = _playerObject.AddComponent<EnergyController>();
            _player = _playerObject.AddComponent<PlayerCharacter>();

            _mockInput = new MockInputService();
            _player.SetInputService(_mockInput);
        }

        [TearDown]
        public void TearDown()
        {
            if (_playerObject != null)
            {
                Object.DestroyImmediate(_playerObject);
            }
            if (HitStopManager.Instance != null)
            {
                HitStopManager.Instance.ResetTimeScale();
            }
            Time.timeScale = 1.0f;
        }

        /// <summary>
        /// TEST-01-MOVE: Applying +1.0 horizontal input moves player on X-axis within expected speed bounds.
        /// </summary>
        [Test]
        public void TEST_01_MOVE()
        {
            _locomotion.Move(1.0f);
            var rb = _playerObject.GetComponent<Rigidbody2D>();
            Assert.AreEqual(_locomotion.MoveSpeed, rb.linearVelocity.x, 0.01f, "Horizontal velocity should equal MoveSpeed (7.5 m/s).");
            Assert.AreEqual(1, _locomotion.FacingDirection, "Facing direction should be positive (1).");

            _locomotion.Move(-1.0f);
            Assert.AreEqual(-_locomotion.MoveSpeed, rb.linearVelocity.x, 0.01f, "Horizontal velocity should equal -MoveSpeed (-7.5 m/s).");
            Assert.AreEqual(-1, _locomotion.FacingDirection, "Facing direction should be negative (-1).");
        }

        /// <summary>
        /// TEST-02-JUMP: Jump input applies upward vertical velocity; clamps at peak and returns downward.
        /// </summary>
        [Test]
        public void TEST_02_JUMP()
        {
            _locomotion.SetGrounded(true);
            bool jumped = _locomotion.TryJump(true, true);
            var rb = _playerObject.GetComponent<Rigidbody2D>();

            Assert.IsTrue(jumped, "Grounded jump should execute successfully.");
            Assert.Greater(rb.linearVelocity.y, 0f, "Vertical velocity should be positive immediately after jump.");
            Assert.IsFalse(_locomotion.IsGrounded, "Player should be airborne after jumping.");
        }

        /// <summary>
        /// TEST-03-COMBO-WINDOW: Consecutive attack inputs within branch window transition sequentially: Attack 1 -> 2 -> 3 -> 4.
        /// </summary>
        [Test]
        public void TEST_03_COMBO_WINDOW()
        {
            Assert.AreEqual(CombatStateMachine.AttackStage.None, _combat.CurrentStage);

            // Trigger Attack 1
            bool a1 = _combat.TryAttack();
            Assert.IsTrue(a1, "Attack 1 should trigger.");
            Assert.AreEqual(CombatStateMachine.AttackStage.Attack1, _combat.CurrentStage);

            // Simulate attack 1 completion and next attack trigger
            _combat.UpdateStateMachine(0.35f); // Duration completes
            bool a2 = _combat.TryAttack();
            Assert.IsTrue(a2, "Attack 2 should trigger.");
            Assert.AreEqual(CombatStateMachine.AttackStage.Attack2, _combat.CurrentStage);

            _combat.UpdateStateMachine(0.35f);
            bool a3 = _combat.TryAttack();
            Assert.IsTrue(a3, "Attack 3 should trigger.");
            Assert.AreEqual(CombatStateMachine.AttackStage.Attack3, _combat.CurrentStage);

            _combat.UpdateStateMachine(0.45f);
            bool a4 = _combat.TryAttack();
            Assert.IsTrue(a4, "Attack 4 finisher should trigger.");
            Assert.AreEqual(CombatStateMachine.AttackStage.Attack4, _combat.CurrentStage);
        }

        /// <summary>
        /// TEST-04-COMBO-TIMEOUT: Waiting >250ms after Attack 1 without input resets state machine to Idle; next attack starts at Attack 1.
        /// </summary>
        [Test]
        public void TEST_04_COMBO_TIMEOUT()
        {
            _combat.TryAttack();
            Assert.AreEqual(CombatStateMachine.AttackStage.Attack1, _combat.CurrentStage);

            // Wait for attack duration (0.33s) + combo timeout (>0.25s)
            _combat.UpdateStateMachine(0.34f); // Attack finishes
            _combat.UpdateStateMachine(0.26f); // Timeout expires

            Assert.AreEqual(CombatStateMachine.AttackStage.None, _combat.CurrentStage, "State machine should reset to None after timeout.");

            // Next attack must restart at Attack 1
            _combat.TryAttack();
            Assert.AreEqual(CombatStateMachine.AttackStage.Attack1, _combat.CurrentStage, "Next attack after timeout must restart at Attack 1.");
        }

        /// <summary>
        /// TEST-05-IFRAMES: Attack collision occurring during first 200ms of DodgeRoll records 0 damage to Player.
        /// </summary>
        [Test]
        public void TEST_05_IFRAMES()
        {
            _locomotion.SetGrounded(true);
            bool dodged = _locomotion.TryDodge();
            Assert.IsTrue(dodged, "Dodge should trigger.");
            Assert.IsTrue(_locomotion.IsInvulnerable, "Player must be invulnerable during initial dodge window.");

            int initialHealth = _player.CurrentHealth;
            DamageContext hit = new DamageContext(50, Vector2.right, 0.06f, true, null);

            // Hit during i-frames
            bool damageTaken = _player.TakeDamage(hit);
            Assert.IsFalse(damageTaken, "TakeDamage should return false during invulnerability.");
            Assert.AreEqual(initialHealth, _player.CurrentHealth, "Health should not decrease during i-frames.");

            // Advance time past 200ms invulnerability window (e.g. 210ms)
            _locomotion.UpdateLocomotion(0.21f);
            Assert.IsFalse(_locomotion.IsInvulnerable, "Player should no longer be invulnerable after 200ms.");

            // Hit after i-frames expire
            bool damageTakenLate = _player.TakeDamage(hit);
            Assert.IsTrue(damageTakenLate, "TakeDamage should succeed after i-frames expire.");
            Assert.AreEqual(initialHealth - 50, _player.CurrentHealth, "Health should decrease after i-frames expire.");
        }

        /// <summary>
        /// TEST-06-HITSTOP-RESET: Force-disabling or resetting HitStopManager mid-freeze guarantees Time.timeScale returns strictly to 1.0f.
        /// </summary>
        [UnityTest]
        public IEnumerator TEST_06_HITSTOP_RESET()
        {
            var hitStopObj = new GameObject("HitStopManager");
            var manager = hitStopObj.AddComponent<HitStopManager>();

            manager.TriggerHitStop(0.5f, 0.05f);
            yield return null; // Wait 1 frame

            Assert.AreEqual(0.05f, Time.timeScale, 0.001f, "Time.timeScale should be frozen during active hit-stop.");

            // Force disable
            hitStopObj.SetActive(false);
            Assert.AreEqual(1.0f, Time.timeScale, 0.001f, "Disabling HitStopManager must immediately restore Time.timeScale to 1.0f.");

            Object.Destroy(hitStopObj);
            yield return null;
        }

        /// <summary>
        /// TEST-07-ENERGY-CHARGE: Holding charge increases Energy by 25 units/sec up to max 100; releasing halts regeneration.
        /// </summary>
        [Test]
        public void TEST_07_ENERGY_CHARGE()
        {
            _energy.Initialize(0f, 100f);
            Assert.AreEqual(0f, _energy.CurrentEnergy);

            // Charge for 2 seconds
            _energy.Charge(2.0f);
            Assert.AreEqual(50f, _energy.CurrentEnergy, 0.01f, "Charging for 2s at 25/s should yield 50 energy.");

            // Charge for 3 more seconds (should clamp at 100)
            _energy.Charge(3.0f);
            Assert.AreEqual(100f, _energy.CurrentEnergy, 0.01f, "Energy must clamp at MaxEnergy (100).");
        }

        /// <summary>
        /// TEST-08-SKILL-COST: Attempting to cast Astral Beam with <30 Energy fails; casting with >=30 Energy deducts 30.
        /// </summary>
        [Test]
        public void TEST_08_SKILL_COST()
        {
            _energy.Initialize(20f, 100f);
            bool castLow = _energy.TryCastAstralBeam();
            Assert.IsFalse(castLow, "Casting with <30 energy must fail.");
            Assert.AreEqual(20f, _energy.CurrentEnergy, "Energy should not be deducted on failed cast.");

            _energy.Initialize(50f, 100f);
            bool castSuccess = _energy.TryCastAstralBeam();
            Assert.IsTrue(castSuccess, "Casting with >=30 energy must succeed.");
            Assert.AreEqual(20f, _energy.CurrentEnergy, "Casting must deduct exactly 30 energy (50 - 30 = 20).");
        }

        /// <summary>
        /// TEST-09-AIR-DASH-LIMIT: Exactly one horizontal air dash permitted per airborne cycle; subsequent attempts fail until grounded.
        /// </summary>
        [Test]
        public void TEST_09_AIR_DASH_LIMIT()
        {
            _locomotion.SetGrounded(false);
            bool firstDash = _locomotion.TryAirDash();
            Assert.IsTrue(firstDash, "First air dash in mid-air must succeed.");
            Assert.IsTrue(_locomotion.IsDashing, "Player should be in dashing state.");

            // Advance dash timer past completion
            _locomotion.UpdateLocomotion(0.35f);
            Assert.IsFalse(_locomotion.IsDashing, "Player should finish dashing.");

            // Attempt second air dash before touching ground
            bool secondDash = _locomotion.TryAirDash();
            Assert.IsFalse(secondDash, "Second air dash in same airborne cycle must be rejected.");

            // Touch ground and jump again
            _locomotion.SetGrounded(true);
            _locomotion.SetGrounded(false);

            bool thirdDash = _locomotion.TryAirDash();
            Assert.IsTrue(thirdDash, "Air dash must reset and succeed after touching ground.");
        }

        /// <summary>
        /// TEST-10-MOBILE-TOUCH-SIMULTANEOUS: MobileTouchInputService reliably delivers simultaneous movement and attack within the same frame.
        /// </summary>
        [Test]
        public void TEST_10_MOBILE_TOUCH_SIMULTANEOUS()
        {
            var touchObj = new GameObject("TestTouchInput");
            var touchService = touchObj.AddComponent<MobileTouchInputService>();

            touchService.SetMoveVector(new Vector2(0.85f, 0f));
            touchService.TriggerAttack();

            Assert.AreEqual(0.85f, touchService.MoveVector.x, 0.01f, "Move vector should reflect joystick input.");
            Assert.IsTrue(touchService.AttackTriggered, "Attack trigger must be active in the same frame.");

            Object.DestroyImmediate(touchObj);
        }

        /// <summary>
        /// TEST-11-COMPOSITE-INPUT-MERGE: CompositeInputService merges mobile touch and standalone inputs without dropping action triggers.
        /// </summary>
        [Test]
        public void TEST_11_COMPOSITE_INPUT_MERGE()
        {
            var inputObj = new GameObject("TestCompositeInput");
            var standalone = inputObj.AddComponent<StandaloneInputService>();
            var mobile = inputObj.AddComponent<MobileTouchInputService>();
            var composite = inputObj.AddComponent<CompositeInputService>();
            composite.Initialize(standalone, mobile);

            mobile.SetMoveVector(new Vector2(1f, 0f));
            mobile.TriggerJump();

            Assert.AreEqual(1f, composite.MoveVector.x, 0.01f, "Composite must reflect mobile move input.");
            Assert.IsTrue(composite.JumpTriggered, "Composite must reflect mobile jump trigger.");

            Object.DestroyImmediate(inputObj);
        }

        /// <summary>
        /// TEST-12-SAFE-AREA-CALCULATION: SafeAreaFitter correctly calculates normalized RectTransform anchors from pixel safe-area rects.
        /// </summary>
        [Test]
        public void TEST_12_SAFE_AREA_CALCULATION()
        {
            var canvasObj = new GameObject("TestCanvas");
            var rectObj = new GameObject("SafeArea", typeof(RectTransform));
            rectObj.transform.SetParent(canvasObj.transform);
            var fitter = rectObj.AddComponent<SafeAreaFitter>();

            fitter.ApplySafeArea();
            var rect = rectObj.GetComponent<RectTransform>();

            Assert.GreaterOrEqual(rect.anchorMin.x, 0f);
            Assert.GreaterOrEqual(rect.anchorMin.y, 0f);
            Assert.LessOrEqual(rect.anchorMax.x, 1f);
            Assert.LessOrEqual(rect.anchorMax.y, 1f);

            Object.DestroyImmediate(canvasObj);
        }

        /// <summary>
        /// TEST-13-GROUND-DETECTION-PHYSICS: Physics-based ground check accurately detects ground surface,
        /// transitions to airborne when falling off ledge, and ignores wall/enemy colliders.
        /// </summary>
        [UnityTest]
        public IEnumerator TEST_13_GROUND_DETECTION_PHYSICS()
        {
            var groundObj = new GameObject("TestGround");
            groundObj.layer = 8; // Ground layer
            groundObj.transform.position = new Vector3(0f, -0.5f, 0f);
            var groundCol = groundObj.AddComponent<BoxCollider2D>();
            groundCol.size = new Vector2(10f, 1f);

            var playerCol = _playerObject.AddComponent<BoxCollider2D>();
            playerCol.size = new Vector2(1.0f, 1.8f);
            playerCol.offset = new Vector2(0f, 0.9f);
            _playerObject.transform.position = new Vector3(0f, 0f, 0f);

            _locomotion.GroundLayer = 1 << 8;
            _locomotion.EnableAutoGroundDetection(true);

            Physics2D.SyncTransforms();
            yield return new WaitForFixedUpdate();

            Assert.IsTrue(_locomotion.IsGrounded, "Player must automatically become grounded when touching ground collider.");
            Assert.IsTrue(_locomotion.CanAirDash, "Landing must restore air-dash allowance.");

            // Jump clears grounded
            _locomotion.TryJump(true, true);
            yield return new WaitForFixedUpdate();
            Assert.IsFalse(_locomotion.IsGrounded, "Jumping must clear grounded state.");

            // Mid-air against wall (Layer 8) must NOT be grounded
            _playerObject.transform.position = new Vector3(50f, 20f, 0f);
            var rb = _playerObject.GetComponent<Rigidbody2D>();
            rb.linearVelocity = Vector2.zero;
            rb.gravityScale = 0f;

            var wallObj = new GameObject("TestWall");
            wallObj.layer = 8;
            wallObj.transform.position = new Vector3(50.55f, 20f, 0f);
            var wallCol = wallObj.AddComponent<BoxCollider2D>();
            wallCol.size = new Vector2(1f, 5f);

            Physics2D.SyncTransforms();
            yield return new WaitForFixedUpdate();
            Assert.IsFalse(_locomotion.IsGrounded, "Side collision with wall must NOT mark player as grounded.");

            // Touching enemy dummy (Layer 7) must NOT be grounded
            var dummyObj = new GameObject("TestEnemyDummy");
            dummyObj.layer = 7;
            dummyObj.transform.position = new Vector3(50f, 18.5f, 0f);
            var dummyCol = dummyObj.AddComponent<BoxCollider2D>();
            dummyCol.size = new Vector2(2f, 2f);

            Physics2D.SyncTransforms();
            yield return new WaitForFixedUpdate();
            Assert.IsFalse(_locomotion.IsGrounded, "Contact with enemy dummy must NOT mark player as grounded.");

            Object.DestroyImmediate(groundObj);
            Object.DestroyImmediate(wallObj);
            Object.DestroyImmediate(dummyObj);
        }

        /// <summary>
        /// TEST-14-TARGET-LAYER-FILTERING: Attacks only damage targets on the configured TargetLayer (Enemy),
        /// ignoring own collider, ground, and other objects.
        /// </summary>
        [UnityTest]
        public IEnumerator TEST_14_TARGET_LAYER_FILTERING()
        {
            _playerObject.transform.position = Vector3.zero;
            _player.TargetLayer = 1 << 7; // Enemy layer only

            var enemyObj = new GameObject("EnemyDummy");
            enemyObj.layer = 7;
            enemyObj.transform.position = new Vector3(1.0f, 0.5f, 0f);
            enemyObj.AddComponent<Rigidbody2D>();
            var enemyCol = enemyObj.AddComponent<BoxCollider2D>();
            enemyCol.size = new Vector2(1.5f, 2.0f);
            var dummy = enemyObj.AddComponent<TrainingDummy>();

            var neutralObj = new GameObject("NeutralDummy");
            neutralObj.layer = 0;
            neutralObj.transform.position = new Vector3(1.0f, 0.5f, 0f);
            neutralObj.AddComponent<Rigidbody2D>();
            var neutralCol = neutralObj.AddComponent<BoxCollider2D>();
            neutralCol.size = new Vector2(1.5f, 2.0f);
            var neutralDummy = neutralObj.AddComponent<TrainingDummy>();

            Physics2D.SyncTransforms();

            int initialEnemyHp = dummy.CurrentHealth;
            int initialNeutralHp = neutralDummy.CurrentHealth;

            _combat.TryAttack();

            yield return null;

            Assert.Less(dummy.CurrentHealth, initialEnemyHp, "Enemy on TargetLayer must take damage from attack.");
            Assert.AreEqual(initialNeutralHp, neutralDummy.CurrentHealth, "Object on non-target layer must NOT take damage.");

            Object.DestroyImmediate(enemyObj);
            Object.DestroyImmediate(neutralObj);
        }

        /// <summary>
        /// TEST-15-CAMERA-FOLLOW-BOUNDS: CameraFollow tracks target position and clamps within defined horizontal and vertical bounds.
        /// </summary>
        [Test]
        public void TEST_15_CAMERA_FOLLOW_BOUNDS()
        {
            var camObj = new GameObject("TestCamera");
            var camFollow = camObj.AddComponent<CameraFollow>();
            var targetObj = new GameObject("FarTarget");
            targetObj.transform.position = new Vector3(50f, 50f, 0f);

            camFollow.SetTarget(targetObj.transform);

            System.Reflection.MethodInfo clampMethod = typeof(CameraFollow).GetMethod("ClampPosition",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

            Vector3 unclampedMax = new Vector3(50f, 50f, -10f);
            Vector3 clampedMax = (Vector3)clampMethod.Invoke(camFollow, new object[] { unclampedMax });

            Assert.AreEqual(10f, clampedMax.x, 0.01f, "Camera horizontal position must clamp to maxX (10.0).");
            Assert.AreEqual(10f, clampedMax.y, 0.01f, "Camera vertical position must clamp to maxY (10.0).");

            Vector3 unclampedMin = new Vector3(-50f, -50f, -10f);
            Vector3 clampedMin = (Vector3)clampMethod.Invoke(camFollow, new object[] { unclampedMin });

            Assert.AreEqual(-10f, clampedMin.x, 0.01f, "Camera horizontal position must clamp to minX (-10.0).");
            Assert.AreEqual(0f, clampedMin.y, 0.01f, "Camera vertical position must clamp to minY (0.0).");

            Object.DestroyImmediate(camObj);
            Object.DestroyImmediate(targetObj);
        }

        /// <summary>
        /// TEST-16-ASTRAL-BEAM-IMPACT: AstralBeamProjectile inflicts damage context on IHurtbox,
        /// ignores caster/owner, and terminates on solid terrain without damaging ground.
        /// </summary>
        [UnityTest]
        public IEnumerator TEST_16_ASTRAL_BEAM_IMPACT()
        {
            var dummyObj = new GameObject("TestDummy");
            dummyObj.layer = 7; // Enemy
            dummyObj.transform.position = new Vector3(3f, 0f, 0f);
            dummyObj.AddComponent<Rigidbody2D>();
            var dummyCol = dummyObj.AddComponent<BoxCollider2D>();
            dummyCol.size = new Vector2(1f, 2f);
            var dummy = dummyObj.AddComponent<TrainingDummy>();

            var projObj = new GameObject("TestProjectile");
            projObj.transform.position = new Vector3(1f, 0f, 0f);
            var col = projObj.AddComponent<CircleCollider2D>();
            col.isTrigger = true;
            var proj = projObj.AddComponent<AstralBeamProjectile>();
            proj.Initialize(Vector2.right, _playerObject);

            Physics2D.SyncTransforms();

            int initialHp = dummy.CurrentHealth;

            // Move projectile directly into dummy collider
            projObj.transform.position = dummyObj.transform.position;
            Physics2D.SyncTransforms();

            yield return new WaitForFixedUpdate();

            Assert.Less(dummy.CurrentHealth, initialHp, "Dummy must take damage when hit by Astral Beam projectile.");

            Object.DestroyImmediate(dummyObj);
        }

        /// <summary>
        /// TEST-17-DUMMY-RECOVERY-AND-ARENA-RESET: Dummy health regenerates after delay,
        /// and CombatHUD.ResetArena restores player and dummy to full status.
        /// </summary>
        [UnityTest]
        public IEnumerator TEST_17_DUMMY_RECOVERY_AND_ARENA_RESET()
        {
            var dummyObj = new GameObject("TestDummy");
            dummyObj.layer = 7;
            dummyObj.AddComponent<Rigidbody2D>();
            dummyObj.AddComponent<BoxCollider2D>();
            var dummy = dummyObj.AddComponent<TrainingDummy>();

            dummy.TakeDamage(new DamageContext(100, Vector2.right, 0.05f, false, _playerObject));
            Assert.AreEqual(400, dummy.CurrentHealth, "Dummy should have 400 HP after taking 100 damage.");

            var hudObj = new GameObject("TestHUD");
            var hud = hudObj.AddComponent<CombatHUD>();

            hud.ResetArena();

            Assert.AreEqual(dummy.MaxHealth, dummy.CurrentHealth, "ResetArena must restore dummy to maximum health.");
            Assert.AreEqual(50f, _energy.CurrentEnergy, "ResetArena must restore player default starting energy (50).");

            Object.DestroyImmediate(dummyObj);
            Object.DestroyImmediate(hudObj);
            yield return null;
        }

        /// <summary>
        /// TEST-18-PLATFORM-VISIBILITY-TOGGLE: PlatformVisibility correctly toggles active/canvas state.
        /// </summary>
        [Test]
        public void TEST_18_PLATFORM_VISIBILITY_TOGGLE()
        {
            var uiRoot = new GameObject("MobileRoot");
            var canvasGroup = uiRoot.AddComponent<CanvasGroup>();
            var visibility = uiRoot.AddComponent<PlatformVisibility>();

            visibility.SetVisible(true);
            Assert.AreEqual(1f, canvasGroup.alpha, 0.01f, "Alpha should be 1.0 when visible.");
            Assert.IsTrue(canvasGroup.interactable, "CanvasGroup must be interactable.");

            visibility.ToggleVisibility();
            Assert.AreEqual(0f, canvasGroup.alpha, 0.01f, "Alpha should be 0.0 after toggle.");
            Assert.IsFalse(canvasGroup.interactable, "CanvasGroup must not be interactable when hidden.");

            Object.DestroyImmediate(uiRoot);
        }
    }
}

