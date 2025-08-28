# 📦 Test Scene Package - Important Instructions!

## ⚠️ This Package Contains Conflicting Scripts!

The `test-scene.unitypackage` includes old scripts that will conflict with our new SDK.

## How to Use Safely:

### Option 1: Import and Clean (Recommended)
1. Import `test-scene.unitypackage`
2. **IMMEDIATELY DELETE** these files:
   - `Assets/SubstreamSDK/Scripts/Substream.cs`
   - `Assets/SubstreamSDK/Demos/Scripts/DemoController.cs`
   - `Assets/SubstreamSDK/Scripts/SimpleDemoScript.cs` (if exists)
3. Then add `SubstreamOneClick.cs` to any GameObject

### Option 2: Just Create Your Own Test Scene
Skip the package and create:
- Empty scene
- Add a cube (GameObject → 3D Object → Cube)
- Add `SubstreamOneClick.cs` to it
- Press Play!

## If Unity Won't Import the Package:

Try:
1. **Restart Unity** (sometimes needed)
2. **Drag the .unitypackage** directly onto the Project window
3. **Use Assets → Import Package → Custom Package**
4. **Check file isn't corrupted**: Should be ~593KB

## The Safe Scripts to Use:
- ✅ SubstreamOneClick.cs (simplest, no dependencies)
- ✅ SubstreamSDK.cs (full featured)
- ❌ NOT the scripts in the package!

---

**Remember**: The visual content (cube, particles) is great, but the scripts in the package are outdated!
