# iOS-FIRST DEVELOPMENT & TESTING PLAN
**Target Device**: iPhone 16 Pro Max (A18 Pro, 6.9" Super Retina XDR, ProMotion 120Hz)  
**Host Environment**: Windows 11 / Unity 6 LTS (`6000.0.85f1`)  
**Phase**: Phase 2 — iOS Technical Foundations & Whitebox Prototype  
**Status**: APPROVED & INITIALIZED  

---

## 1. EXECUTIVE SUMMARY & PLATFORM STRATEGY
The project has pivoted to an **iOS-First** development priority with the **iPhone 16 Pro Max** as the primary physical benchmark device. 

All core gameplay code, 2.5D cel-shaded visuals, animation state machines, and touch input abstractions are developed and validated cross-platform inside Unity 6 LTS on Windows. The platform architecture guarantees that:
1. **Core Single-Player Campaign is 100% Offline**: Zero mandatory server connections or network gates for gameplay.
2. **Dual Platform Readiness**: Both `iOSSupport` and `AndroidPlayer` modules are installed in Unity 6 LTS. Developing for iOS does not compromise the ability to compile Android builds in later phases.
3. **No Unverified Claims**: Unity Remote 5 is treated strictly as an interactive touch-input inspection tool, not a native performance benchmark. Native performance validation requires compiled `.ipa` / TestFlight profiling.

---

## 2. WINDOWS-TO-iOS BUILD & DEPLOYMENT WORKFLOW

Because Apple strictly requires macOS and Xcode for compiling, code-signing, and packaging native iOS `.ipa` binaries, development follows a structured three-tier pipeline:

```
┌─────────────────────────────────────────────────────────────┐
│                 TIER 1: WINDOWS WORKSTATION                 │
│  - Gameplay logic, 2.5D physics, combat state machines      │
│  - Unity Device Simulator (iPhone 16 Pro Max resolution)    │
│  - Unity Remote 5 (Live touch/gyro streaming via USB)       │
└──────────────────────────────┬──────────────────────────────┘
                               │
               Unity "Build Settings" -> Platform: iOS
               Generates: /Builds/iOS/ (Full Xcode Project)
                               │
                               ▼
┌─────────────────────────────────────────────────────────────┐
│             TIER 2: MACOS COMPILATION & PACKAGING           │
│  Options:                                                   │
│   A) Local Mac: Transfer Xcode project -> Xcode Build/Archive│
│   B) Cloud CI: GitHub Actions macOS Runner / Codemagic      │
│      - Automatically compiles Xcode project to .ipa         │
│      - Signs binary using Apple Distribution Certificate    │
└──────────────────────────────┬──────────────────────────────┘
                               │
                               ▼
┌─────────────────────────────────────────────────────────────┐
│             TIER 3: IPHONE 16 PRO MAX EXECUTION             │
│  - Direct USB deploy (Xcode / Apple Configurator)           │
│  - Apple TestFlight (Over-the-air closed testing)           │
│  - Instruments & Xcode Metal Frame Profiler (GPU/CPU/RAM)   │
└─────────────────────────────────────────────────────────────┘
```

### 2.1 Test Methods Comparison

| Method | Role in Development | Hardware Required | What It Measures | What It CANNOT Measure |
|---|---|---|---|---|
| **Unity Device Simulator** | Day-to-day UI layout & Safe Area checks in Editor. | Windows PC only | Aspect ratio (19.5:9), safe-area insets, anchor scaling. | Real touch latency, thermal throttling, Apple A18 Pro GPU performance. |
| **Unity Remote 5** | Immediate tactile touch & ergonomic feel check. | Windows PC + iPhone via USB | Virtual joystick feel, button reachability on 6.9" display. | Native 60/120 FPS frame rate, Metal 3 shader draw calls, true device battery drain. |
| **Native `.ipa` / TestFlight** | Formal performance, memory, and compliance certification. | macOS (or Cloud CI) + iPhone 16 Pro Max | Exact A18 Pro Metal performance, thermal stability, 60/120 FPS, memory footprint. | Rapid 5-second script iterations (build cycle takes 5–10 mins). |

---

## 3. iPHONE 16 PRO MAX HARDWARE & UX OPTIMIZATIONS

### 3.1 Display Specifications & Safe Area Architecture
- **Screen Resolution**: 2868 x 1320 pixels (~460 ppi, 19.5:9 aspect ratio).
- **Physical Features**: Dynamic Island (pill cutout) and rounded display corners in landscape orientation; iOS Home Indicator bar along bottom edge.
- **Dynamic Safe Area Scripting**:
  - All HUD and control canvases will utilize a dynamic `SafeAreaFitter` component querying `Screen.safeArea` at runtime and upon orientation changes.
  - **Zero Hardcoding**: Never hardcode pixel offsets for Dynamic Island or notch widths. `Screen.safeArea` provides hardware-accurate insets across all iPhone generations.

```csharp
public class SafeAreaFitter : MonoBehaviour
{
    private RectTransform _rectTransform;
    private Rect _lastSafeArea;

    void Awake() => _rectTransform = GetComponent<RectTransform>();

    void Update()
    {
        Rect currentSafeArea = Screen.safeArea;
        if (currentSafeArea != _lastSafeArea)
        {
            ApplySafeArea(currentSafeArea);
        }
    }

    void ApplySafeArea(Rect r)
    {
        _lastSafeArea = r;
        Vector2 anchorMin = r.position;
        Vector2 anchorMax = r.position + r.size;
        anchorMin.x /= Screen.width;
        anchorMin.y /= Screen.height;
        anchorMax.x /= Screen.width;
        anchorMax.y /= Screen.height;

        _rectTransform.anchorMin = anchorMin;
        _rectTransform.anchorMax = anchorMax;
    }
}
```

### 3.2 Landscape Control Ergonomics (6.9" Display)
- **Reach Zones**: On a 6.9-inch device, thumbs naturally rest in the lower left and lower right quadrants:
  - **Left Quadrant**: Floating Virtual Joystick auto-centering on finger touch to prevent thumb fatigue.
  - **Right Quadrant**: Action cluster arranged in an anatomical arc matching thumb sweep radius.
- **Adjustable Target Sizing**: Primary attack button min touch target: 64x64 pt (~14mm physical); secondary skill buttons: 48x48 pt (~10.5mm physical) to prevent missed taps.

### 3.3 Frame Rate & Performance Targets
- **Baseline Target**: **Solid 60 FPS** (Zero stutter, delta frame time ≤ 16.6ms).
- **ProMotion 120Hz (Optimization Tier)**:
  - Set `Application.targetFrameRate = 60` as the robust default to protect battery life and thermal limits during prolonged combat sessions.
  - An optional "High Frame Rate (120 FPS)" toggle will be placed in Settings (`Application.targetFrameRate = 120`) only **after** native Metal profiling proves the A18 Pro maintains thermal equilibrium below 42°C.

---

## 4. APPLE ACCOUNTS, SIGNING & SERVICE PREREQUISITES

| Requirement | Purpose | Minimum Setup Required | Estimated Cost | Timing / Owner |
|---|---|---|---|---|
| **Free Apple ID** | Local device development signing. | Personal Apple ID signed into Xcode. | **$0.00** | Prototype phase / Product Owner. |
| **Apple Developer Program** | TestFlight distribution, In-App Purchases, store release. | Annual enrollment under individual or organization. | **$99 USD / year** | Prior to Phase 7 (QA & TestFlight) / Product Owner. |
| **Certificates & Profiles** | Cryptographic code-signing for iOS builds. | Apple Distribution Cert + App Store Provisioning Profile. | Included in $99/yr | Phase 7 / Lead Dev & Product Owner. |
| **macOS Build Service** | Compiles Unity Xcode project to signed `.ipa`. | **Option A**: Existing Mac.<br>**Option B**: GitHub Actions macOS runner. | GitHub Actions: Free for public repos; ~$0.08/min for private. | Phase 3/7 / Lead Dev. |

> [!NOTE]
> No paid accounts, developer subscriptions, or cloud build credits are required for the initial whitebox combat prototype.

---

## 5. MONETIZATION ALIGNMENT (FREE-TO-PLAY & OFFLINE)
- **Free-to-Play**: Core campaign, character progression, and essential skill trees remain 100% free and fully functional offline.
- **In-Game Purchases**: Optional consumable currency packs (*Astral Crystals*) for cosmetics and supporter perks.
- **Zero Live Billing in Prototype**: Real StoreKit 2 APIs will not be initialized until Phase 7. Early development uses `MockPurchaseService` exclusively.
